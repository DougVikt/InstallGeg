using System.Text.Json.Serialization;

namespace InstaladorGeral.Models;

public class CategoryData
{
    [JsonPropertyName("categories")]
    public List<Category> Categories { get; set; } = new();
}

public class Category
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("icon")]
    public string Icon { get; set; } = "";

    [JsonPropertyName("programs")]
    public List<ProgramInfo> Programs { get; set; } = new();
}

public class ProgramInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("winget")]
    public string WingetId { get; set; } = "";

    [JsonPropertyName("choco")]
    public string ChocoId { get; set; } = "";

    [JsonPropertyName("description")]
    public string Description { get; set; } = "";

    [JsonIgnore]
    public bool IsSelected { get; set; }
}

public class InstallationItem
{
    public ProgramInfo Program { get; set; } = new();
    public string Source { get; set; } = "winget";
    public InstallationStatus Status { get; set; } = InstallationStatus.Pending;
    public int Progress { get; set; }
    public string StatusMessage { get; set; } = "Aguardando...";
}

public enum InstallationStatus
{
    Pending,
    Installing,
    Completed,
    Failed,
    Skipped
}
