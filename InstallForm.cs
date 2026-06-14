using InstaladorGeral.Models;

namespace InstaladorGeral;

public partial class InstallForm : Form
{
    private readonly InstallerEngine _engine;
    private readonly List<InstallationItem> _items;
    private readonly Dictionary<string, (ProgressBar bar, Label statusLabel, Label nameLabel)> _uiItems = new();
    private CancellationTokenSource? _cts;
    private int _completedCount;
    private int _totalCount;

    public InstallForm(InstallerEngine engine, List<InstallationItem> items)
    {
        _engine = engine;
        _items = items;
        InitializeComponent();
        InitializeInstallItems();
    }

    private void InitializeInstallItems()
    {
        int y = 10;
        _totalCount = _items.Count;
        _completedCount = 0;
        lblTotal.Text = $"Total: 0/{_totalCount} concluídos";

        foreach (var item in _items)
        {
            var itemPanel = new Panel
            {
                Size = new Size(panelInstalls.Width - 30, 70),
                Location = new Point(10, y),
                BackColor = Color.FromArgb(42, 42, 42),
                BorderStyle = BorderStyle.None
            };

            var nameLabel = new Label
            {
                Text = $"  {item.Program.Name}",
                Location = new Point(6, 6),
                Size = new Size(350, 22),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 220, 220)
            };

            var sourceLabel = new Label
            {
                Text = $"fonte: {item.Source}",
                Location = new Point(370, 8),
                Size = new Size(120, 18),
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = Color.FromArgb(140, 140, 140)
            };

            var progressBar = new ProgressBar
            {
                Location = new Point(10, 34),
                Size = new Size(panelInstalls.Width - 60, 24),
                Minimum = 0,
                Maximum = 100,
                Value = 0,
                Style = ProgressBarStyle.Continuous,
                ForeColor = Color.FromArgb(0, 120, 215),
                BackColor = Color.FromArgb(30, 30, 30)
            };

            var statusLabel = new Label
            {
                Text = "Aguardando...",
                Location = new Point(10, 60),
                Size = new Size(panelInstalls.Width - 60, 18),
                Font = new Font("Segoe UI", 8, FontStyle.Regular),
                ForeColor = Color.FromArgb(140, 140, 140)
            };

            itemPanel.Controls.Add(nameLabel);
            itemPanel.Controls.Add(sourceLabel);
            itemPanel.Controls.Add(progressBar);
            itemPanel.Controls.Add(statusLabel);

            panelInstalls.Controls.Add(itemPanel);
            _uiItems[item.Program.Name] = (progressBar, statusLabel, nameLabel);

            y += 84;
        }

        panelInstalls.AutoScrollMinSize = new Size(0, y + 10);
    }

    private async void InstallForm_Shown(object? sender, EventArgs e)
    {
        _cts = new CancellationTokenSource();
        btnCancelar.Enabled = true;
        btnFechar.Enabled = false;

        var progress = new Progress<(InstallationItem item, int progress, string status)>();
        progress.ProgressChanged += (s, args) =>
        {
            if (IsDisposed) return;
            UpdateItemUI(args.item, args.progress, args.status);
        };

        try
        {
            await Task.Run(() =>
                _engine.InstallAllAsync(_items, progress, _cts.Token));
        }
        catch (Exception ex)
        {
            if (!IsDisposed)
                MessageBox.Show($"Erro durante instalação: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            if (!IsDisposed)
            {
                btnCancelar.Enabled = false;
                btnFechar.Enabled = true;
                UpdateTotalLabel();
            }
        }
    }

    private void UpdateItemUI(InstallationItem item, int progress, string status)
    {
        if (!_uiItems.TryGetValue(item.Program.Name, out var ui)) return;

        if (ui.bar.InvokeRequired)
        {
            ui.bar.Invoke(() => UpdateItemUI(item, progress, status));
            return;
        }

        ui.bar.Value = Math.Min(progress, 100);
        ui.statusLabel.Text = status;

        switch (item.Status)
        {
            case InstallationStatus.Completed:
                ui.bar.ForeColor = Color.FromArgb(76, 175, 80);
                ui.nameLabel.ForeColor = Color.FromArgb(100, 200, 100);
                ui.statusLabel.ForeColor = Color.FromArgb(76, 175, 80);
                _completedCount++;
                break;
            case InstallationStatus.Failed:
                ui.bar.ForeColor = Color.FromArgb(244, 67, 54);
                ui.nameLabel.ForeColor = Color.FromArgb(244, 67, 54);
                ui.statusLabel.ForeColor = Color.FromArgb(244, 67, 54);
                break;
            case InstallationStatus.Skipped:
                ui.bar.ForeColor = Color.FromArgb(255, 152, 0);
                ui.nameLabel.ForeColor = Color.FromArgb(255, 152, 0);
                ui.statusLabel.ForeColor = Color.FromArgb(255, 152, 0);
                break;
            case InstallationStatus.Installing:
                ui.bar.ForeColor = Color.FromArgb(0, 120, 215);
                ui.statusLabel.ForeColor = Color.FromArgb(100, 180, 255);
                break;
        }

        UpdateTotalLabel();
    }

    private void UpdateTotalLabel()
    {
        var completed = _items.Count(i => i.Status == InstallationStatus.Completed);
        var failed = _items.Count(i => i.Status == InstallationStatus.Failed);
        var skipped = _items.Count(i => i.Status == InstallationStatus.Skipped);

        var parts = new List<string>();
        if (completed > 0) parts.Add($"✅ {completed} concluídos");
        if (failed > 0) parts.Add($"❌ {failed} falhas");
        if (skipped > 0) parts.Add($"⚠️ {skipped} ignorados");
        var pending = _items.Count - completed - failed - skipped;
        if (pending > 0) parts.Add($"⏳ {pending} pendentes");

        lblTotal.Text = $"Total: {string.Join(" | ", parts)}";
    }

    private void BtnCancelar_Click(object? sender, EventArgs e)
    {
        _cts?.Cancel();
        btnCancelar.Enabled = false;
    }

    private void BtnFechar_Click(object? sender, EventArgs e)
    {
        Close();
    }
}
