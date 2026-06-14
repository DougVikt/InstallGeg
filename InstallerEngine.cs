using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using InstaladorGeral.Models;

namespace InstaladorGeral;

public class InstallerEngine
{
    public bool WingetAvailable { get; private set; }
    public bool ChocoAvailable { get; private set; }
    public string WingetPath { get; private set; } = "";
    public string ChocoPath { get; private set; } = "";

    public InstallerEngine()
    {
        CheckAvailability();
    }

    private void CheckAvailability()
    {
        WingetAvailable = FindExecutable("winget.exe", out var wingetPath);
        WingetPath = wingetPath;

        ChocoAvailable = FindExecutable("choco.exe", out var chocoPath);
        ChocoPath = chocoPath;
    }

    private static bool FindExecutable(string filename, out string fullPath)
    {
        fullPath = "";

        try
        {
            var paths = Environment.GetEnvironmentVariable("PATH") ?? "";
            foreach (var dir in paths.Split(Path.PathSeparator))
            {
                try
                {
                    var candidate = Path.Combine(dir.Trim(), filename);
                    if (File.Exists(candidate))
                    {
                        fullPath = candidate;
                        return true;
                    }
                }
                catch { }
            }
        }
        catch { }

        return false;
    }

    private static string RunCommand(string command, string arguments)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = command,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var proc = Process.Start(psi);
            if (proc == null) return "";

            var output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit(5000);
            return output;
        }
        catch
        {
            return "";
        }
    }

    public async Task InstallProgramAsync(
        InstallationItem item,
        IProgress<(InstallationItem item, int progress, string status)>? progress = null,
        CancellationToken ct = default)
    {
        item.Status = InstallationStatus.Installing;
        item.Progress = 0;
        item.StatusMessage = "Iniciando...";
        progress?.Report((item, 0, "Iniciando..."));

        try
        {
            string command;
            string args;

            if (item.Source == "winget" && WingetAvailable && !string.IsNullOrEmpty(item.Program.WingetId))
            {
                command = WingetPath;
                args = $"install --silent --accept-package-agreements --id \"{item.Program.WingetId}\"";
            }
            else if (item.Source == "choco" && ChocoAvailable && !string.IsNullOrEmpty(item.Program.ChocoId))
            {
                command = ChocoPath;
                args = $"install \"{item.Program.ChocoId}\" -y --silent --acceptlicense";
            }
            else
            {
                item.Status = InstallationStatus.Skipped;
                item.Progress = 0;
                item.StatusMessage = "Nenhum gerenciador disponível para este programa";
                progress?.Report((item, 0, "Ignorado"));
                return;
            }

            var psi = new ProcessStartInfo
            {
                FileName = command,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = System.Text.Encoding.UTF8,
                StandardErrorEncoding = System.Text.Encoding.UTF8
            };

            using var proc = Process.Start(psi);
            if (proc == null)
            {
                item.Status = InstallationStatus.Failed;
                item.StatusMessage = "Falha ao iniciar processo";
                progress?.Report((item, 0, "Falhou"));
                return;
            }

            item.StatusMessage = "Instalando...";
            item.Progress = 10;
            progress?.Report((item, 10, "Instalando..."));

            var outputTask = Task.Run(() =>
            {
                try
                {
                    while (!proc.StandardOutput.EndOfStream && !ct.IsCancellationRequested)
                    {
                        var line = proc.StandardOutput.ReadLine();
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        var msg = line.Trim();
                        item.StatusMessage = msg.Length > 60 ? msg[..57] + "..." : msg;

                        if (msg.Contains("%", StringComparison.OrdinalIgnoreCase))
                        {
                            var percentMatch = System.Text.RegularExpressions.Regex.Match(msg, @"(\d+)\s*%");
                            if (percentMatch.Success && int.TryParse(percentMatch.Groups[1].Value, out var pct))
                            {
                                item.Progress = Math.Max(item.Progress, Math.Min(pct, 95));
                            }
                            else
                            {
                                item.Progress = Math.Min(item.Progress + 5, 95);
                            }
                        }
                        else
                        {
                            item.Progress = Math.Min(item.Progress + 2, 90);
                        }

                        progress?.Report((item, item.Progress, item.StatusMessage));
                    }
                }
                catch { }
            }, ct);

            var errorTask = Task.Run(() =>
            {
                try
                {
                    var errors = proc.StandardError.ReadToEnd();
                    if (!string.IsNullOrWhiteSpace(errors))
                    {
                        item.StatusMessage = errors.Trim().Length > 60
                            ? errors.Trim()[..57] + "..."
                            : errors.Trim();
                        progress?.Report((item, item.Progress, item.StatusMessage));
                    }
                }
                catch { }
            }, ct);

            await Task.WhenAll(outputTask, errorTask);

            proc.WaitForExit();

            if (ct.IsCancellationRequested)
            {
                try { proc.Kill(); } catch { }
                item.Status = InstallationStatus.Skipped;
                item.StatusMessage = "Cancelado";
                progress?.Report((item, item.Progress, "Cancelado"));
                return;
            }

            if (proc.ExitCode == 0)
            {
                item.Status = InstallationStatus.Completed;
                item.Progress = 100;
                item.StatusMessage = "Concluído com sucesso!";
                progress?.Report((item, 100, "Concluído"));
            }
            else
            {
                item.Status = InstallationStatus.Failed;
                item.StatusMessage = $"Falhou (código {proc.ExitCode})";
                progress?.Report((item, item.Progress, "Falhou"));
            }
        }
        catch (OperationCanceledException)
        {
            item.Status = InstallationStatus.Skipped;
            item.StatusMessage = "Cancelado";
            progress?.Report((item, item.Progress, "Cancelado"));
        }
        catch (Exception ex)
        {
            item.Status = InstallationStatus.Failed;
            item.StatusMessage = $"Erro: {ex.Message}";
            progress?.Report((item, 0, "Erro"));
        }
    }

    public async Task InstallAllAsync(
        List<InstallationItem> items,
        IProgress<(InstallationItem item, int progress, string status)>? progress = null,
        CancellationToken ct = default)
    {
        foreach (var item in items)
        {
            if (ct.IsCancellationRequested) break;
            await InstallProgramAsync(item, progress, ct);
        }
    }

    public async Task<bool> InstallChocoAsync(IProgress<(int progress, string status)>? progress = null)
    {
        progress?.Report((0, "Iniciando instalacao do Chocolatey..."));

        var psi = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = "-NoProfile -ExecutionPolicy Bypass -Command \"[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        using var proc = Process.Start(psi);
        if (proc == null)
        {
            progress?.Report((0, "Falha ao iniciar processo"));
            return false;
        }

        progress?.Report((30, "Executando script de instalacao..."));
        var output = await proc.StandardOutput.ReadToEndAsync();
        var error = await proc.StandardError.ReadToEndAsync();

        if (!proc.WaitForExit(120000))
        {
            try { proc.Kill(); } catch { }
            progress?.Report((0, "Tempo excedido (2 min)"));
            return false;
        }

        if (proc.ExitCode == 0)
        {
            progress?.Report((70, "Chocolatey instalado! Atualizando..."));
            Refresh();
            progress?.Report((100, "Chocolatey instalado com sucesso!"));
            return true;
        }

        progress?.Report((0, $"Falha (codigo {proc.ExitCode})"));
        return false;
    }

    public async Task<bool> InstallWingetAsync(IProgress<(int progress, string status)>? progress = null)
    {
        progress?.Report((10, "Obtendo versao mais recente do winget..."));

        try
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("InstaladorGeral/1.0");
            client.Timeout = TimeSpan.FromSeconds(30);

            var json = await client.GetStringAsync("https://api.github.com/repos/microsoft/winget-cli/releases/latest");
            var doc = JsonDocument.Parse(json);
            var tag = doc.RootElement.GetProperty("tag_name").GetString() ?? "";
            var assets = doc.RootElement.GetProperty("assets");

            string? downloadUrl = null;
            foreach (var asset in assets.EnumerateArray())
            {
                var name = asset.GetProperty("name").GetString() ?? "";
                if (name.EndsWith(".msixbundle") && name.Contains("Microsoft.DesktopAppInstaller"))
                {
                    downloadUrl = asset.GetProperty("browser_download_url").GetString();
                    break;
                }
            }

            if (string.IsNullOrEmpty(downloadUrl))
            {
                progress?.Report((0, "Pacote winget nao encontrado no GitHub"));
                return false;
            }

            var tempDir = Path.Combine(Path.GetTempPath(), "InstaladorGeral");
            Directory.CreateDirectory(tempDir);
            var filePath = Path.Combine(tempDir, $"winget-{tag}.msixbundle");

            progress?.Report((20, "Baixando winget..."));
            var response = await client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var totalBytes = response.Content.Headers.ContentLength ?? -1;
            using var stream = await response.Content.ReadAsStreamAsync();
            using var fileStream = File.Create(filePath);

            var buffer = new byte[81920];
            long bytesRead = 0;
            int read;
            while ((read = await stream.ReadAsync(buffer)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, read);
                bytesRead += read;
                if (totalBytes > 0)
                {
                    var pct = 20 + (int)(bytesRead * 60 / totalBytes);
                    progress?.Report((pct, $"Baixando winget... {bytesRead * 100 / totalBytes}%"));
                }
            }

            await fileStream.FlushAsync();
            fileStream.Close();

            progress?.Report((80, "Instalando winget via PowerShell..."));

            var psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -Command \"Add-AppxPackage -Path '{filePath}'\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            using var proc = Process.Start(psi);
            if (proc == null)
            {
                progress?.Report((0, "Falha ao iniciar PowerShell"));
                return false;
            }

            var output = await proc.StandardOutput.ReadToEndAsync();
            var error = await proc.StandardError.ReadToEndAsync();
            proc.WaitForExit(120000);

            if (proc.ExitCode == 0)
            {
                progress?.Report((90, "Winget instalado! Atualizando..."));
                Refresh();
                progress?.Report((100, "Winget instalado com sucesso!"));
                return true;
            }

            var errMsg = string.IsNullOrWhiteSpace(error) ? output : error;
            progress?.Report((0, $"Falha ao instalar: {errMsg.Trim()}"));
            return false;
        }
        catch (Exception ex)
        {
            progress?.Report((0, $"Erro: {ex.Message}"));
            return false;
        }
    }

    public async Task<List<ProgramInfo>> SearchWingetOnlineAsync(string query, CancellationToken ct = default)
    {
        var results = new List<ProgramInfo>();

        if (!WingetAvailable || string.IsNullOrWhiteSpace(query))
            return results;

        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = WingetPath,
                Arguments = $"search \"{query}\" --format json --accept-source-agreements",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            using var proc = Process.Start(psi);
            if (proc == null) return results;

            var output = await proc.StandardOutput.ReadToEndAsync();
            ct.ThrowIfCancellationRequested();
            proc.WaitForExit(30000);

            if (proc.ExitCode != 0 || string.IsNullOrWhiteSpace(output))
                return TryParseWingetSearchText(output, results);

            if (output.TrimStart().StartsWith("["))
                ParseWingetSearchJson(output, results);
            else
                TryParseWingetSearchText(output, results);
        }
        catch (OperationCanceledException) { }
        catch { }

        return results;
    }

    private static void ParseWingetSearchJson(string json, List<ProgramInfo> results)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);

            JsonElement.ArrayEnumerator? itemsEnum = null;

            if (doc.RootElement.ValueKind == JsonValueKind.Array)
                itemsEnum = doc.RootElement.EnumerateArray();
            else if (doc.RootElement.TryGetProperty("Packages", out var packages) && packages.ValueKind == JsonValueKind.Array)
                itemsEnum = packages.EnumerateArray();

            if (itemsEnum == null) return;

            foreach (var item in itemsEnum.Value)
            {
                try
                {
                    var id = item.TryGetProperty("Id", out var idProp) ? idProp.GetString() ?? "" : "";
                    var name = item.TryGetProperty("Name", out var nameProp) ? nameProp.GetString() ?? "" : "";
                    if (string.IsNullOrEmpty(id)) continue;

                    results.Add(new ProgramInfo
                    {
                        Name = string.IsNullOrEmpty(name) ? id : name,
                        WingetId = id,
                        Description = $"Encontrado via winget search"
                    });
                }
                catch { }
            }
        }
        catch { }
    }

    private static List<ProgramInfo> TryParseWingetSearchText(string output, List<ProgramInfo> results)
    {
        try
        {
            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            bool headerPassed = false;

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmed)) continue;
                if (trimmed.StartsWith("---")) { headerPassed = true; continue; }
                if (!headerPassed || trimmed.StartsWith("Nome") || trimmed.StartsWith("Name")) continue;

                var parts = Regex.Split(trimmed, @"\s{2,}");
                if (parts.Length >= 2)
                {
                    var name = parts[0].Trim();
                    var id = parts[1].Trim();

                    if (!string.IsNullOrEmpty(id) && results.All(r => r.WingetId != id))
                    {
                        results.Add(new ProgramInfo
                        {
                            Name = string.IsNullOrEmpty(name) ? id : name,
                            WingetId = id,
                            Description = "Encontrado via winget search"
                        });
                    }
                }
            }
        }
        catch { }

        return results;
    }

    public void Refresh()
    {
        CheckAvailability();
    }
}
