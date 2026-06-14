using System.Text.Json;
using InstaladorGeral.Models;

namespace InstaladorGeral;

public partial class MainForm : Form
{
    private readonly InstallerEngine _engine = new();
    private List<Category> _categories = new();
    private Category? _selectedCategory;
    private readonly List<ListViewItem> _allListViewItems = new();
    private readonly List<ProgramInfo> _selectedPrograms = new();
    private readonly Color _darkBg = Color.FromArgb(28, 28, 28);
    private readonly Color _darkPanel = Color.FromArgb(38, 38, 38);
    private readonly Color _selectedCatBg = Color.FromArgb(0, 120, 215);
    private readonly Color _hoverCatBg = Color.FromArgb(50, 50, 50);
    private Button? _activeCategoryButton;
    private bool _isUpdatingChecks;
    private CancellationTokenSource? _searchCts;

    public MainForm()
    {
        InitializeComponent();
        LoadCategories();
        InitializeCategories();
        UpdateStatusBar();
        UpdateButtonStates();
    }

    private void LoadCategories()
    {
        try
        {
            var assembly = typeof(MainForm).Assembly;
            var names = assembly.GetManifestResourceNames();
            var found = names.FirstOrDefault(n => n.EndsWith("categories.json"));
            if (found == null) return;

            using var stream = assembly.GetManifestResourceStream(found);
            if (stream == null) return;

            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            var data = JsonSerializer.Deserialize<CategoryData>(json);
            if (data != null) _categories = data.Categories;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao carregar categorias: {ex.Message}", "Erro",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void InitializeCategories()
    {
        categoryPanel.Controls.Clear();
        int y = 6;

        foreach (var cat in _categories)
        {
            var btn = new Button
            {
                Text = $"  {cat.Icon}  {cat.Name}",
                TextAlign = ContentAlignment.MiddleLeft,
                Size = new Size(232, 40),
                Location = new Point(6, y),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                BackColor = _darkBg,
                ForeColor = Color.FromArgb(200, 200, 200),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Cursor = Cursors.Hand,
                Tag = cat
            };

            btn.MouseEnter += (s, e) => { if (btn != _activeCategoryButton) btn.BackColor = _hoverCatBg; };
            btn.MouseLeave += (s, e) => { if (btn != _activeCategoryButton) btn.BackColor = _darkBg; };
            btn.Click += (s, e) => SelectCategory(cat, btn);

            categoryPanel.Controls.Add(btn);
            y += 46;
        }

        if (_categories.Count > 0 && categoryPanel.Controls[0] is Button first)
            SelectCategory(_categories[0], first);
    }

    private void SelectCategory(Category cat, Button btn)
    {
        _selectedCategory = cat;
        _searchCts?.Cancel();

        if (_activeCategoryButton != null)
        {
            _activeCategoryButton.BackColor = _darkBg;
            _activeCategoryButton.ForeColor = Color.FromArgb(200, 200, 200);
        }

        btn.BackColor = _selectedCatBg;
        btn.ForeColor = Color.White;
        _activeCategoryButton = btn;

        lblCategoryTitle.Text = $"{cat.Icon}  {cat.Name}  ({cat.Programs.Count} programas)";
        txtSearch.Clear();
        LoadPrograms(cat, "");
    }

    private void LoadPrograms(Category cat, string filter)
    {
        _allListViewItems.Clear();
        lstPrograms.BeginUpdate();
        lstPrograms.Items.Clear();

        var filtered = string.IsNullOrWhiteSpace(filter)
            ? cat.Programs
            : cat.Programs.Where(p =>
                p.Name.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                p.WingetId.Contains(filter, StringComparison.OrdinalIgnoreCase)).ToList();

        if (string.IsNullOrWhiteSpace(filter) && filtered.Count > 100)
            filtered = filtered.Take(100).ToList();

        _isUpdatingChecks = true;

        foreach (var prog in filtered)
        {
            var sources = new List<string>();
            if (!string.IsNullOrEmpty(prog.WingetId)) sources.Add("winget");
            if (!string.IsNullOrEmpty(prog.ChocoId)) sources.Add("choco");

            var item = new ListViewItem(prog.Name)
            {
                SubItems = { prog.Description, string.Join("+", sources) },
                Checked = _selectedPrograms.Contains(prog),
                Tag = prog
            };
            _allListViewItems.Add(item);
        }

        lstPrograms.Items.AddRange(_allListViewItems.ToArray());
        lstPrograms.EndUpdate();
        _isUpdatingChecks = false;
    }

    private async void TxtSearch_TextChanged(object? sender, EventArgs e)
    {
        if (_selectedCategory == null) return;

        _searchCts?.Cancel();
        _searchCts = new CancellationTokenSource();
        var ct = _searchCts.Token;

        try
        {
            await Task.Delay(300, ct);
            await SearchAndDisplayAsync(_selectedCategory, txtSearch.Text, ct);
        }
        catch (TaskCanceledException) { }
    }

    private async Task SearchAndDisplayAsync(Category cat, string filter, CancellationToken ct)
    {
        LoadPrograms(cat, filter);

        if (string.IsNullOrWhiteSpace(filter) || !_engine.WingetAvailable) return;

        if (lstPrograms.Items.Count >= 5) return;

        lblSearchStatus.Text = "Buscando online...";
        lblSearchStatus.Visible = true;

        try
        {
            var onlineResults = await _engine.SearchWingetOnlineAsync(filter, ct);

            if (ct.IsCancellationRequested) return;

            onlineResults.RemoveAll(r => _selectedCategory?.Programs.Any(p =>
                p.WingetId == r.WingetId) == true);

            if (onlineResults.Count == 0)
            {
                lblSearchStatus.Text = "Nenhum resultado online";
                return;
            }

            _isUpdatingChecks = true;
            lstPrograms.BeginUpdate();

            if (lstPrograms.Items.Count > 0)
            {
                var sep = new ListViewItem("--- Resultados Online ---")
                {
                    ForeColor = Color.FromArgb(100, 180, 255)
                };
                lstPrograms.Items.Add(sep);
            }

            foreach (var prog in onlineResults)
            {
                var item = new ListViewItem(prog.Name)
                {
                    SubItems = { prog.Description, "winget (online)" },
                    Checked = _selectedPrograms.Contains(prog),
                    Tag = prog
                };
                lstPrograms.Items.Add(item);
            }

            lstPrograms.EndUpdate();
            _isUpdatingChecks = false;
            lblSearchStatus.Text = $"+{onlineResults.Count} online";
        }
        catch (OperationCanceledException) { }
        catch
        {
            lblSearchStatus.Text = "Erro na busca online";
        }
    }

    private void LstPrograms_ItemChecked(object? sender, ItemCheckedEventArgs e)
    {
        if (_isUpdatingChecks) return;

        if (e.Item?.Tag is ProgramInfo prog)
        {
            prog.IsSelected = e.Item.Checked;
            if (e.Item.Checked)
            {
                if (!_selectedPrograms.Contains(prog))
                    _selectedPrograms.Add(prog);
            }
            else
            {
                _selectedPrograms.Remove(prog);
            }
            UpdateButtonStates();
        }
    }

    private void UpdateButtonStates()
    {
        var count = _selectedPrograms.Count;
        btnInstalar.Enabled = count > 0;
        btnLimpar.Enabled = count > 0;
        btnInstalar.Text = count > 0 ? $"Instalar ({count})" : "Instalar";
    }

    private void UpdateStatusBar()
    {
        lblStatusWinget.Text = _engine.WingetAvailable
            ? "winget disponivel"
            : "winget nao encontrado";
        lblStatusWinget.ForeColor = _engine.WingetAvailable
            ? Color.FromArgb(100, 200, 100)
            : Color.FromArgb(200, 100, 100);

        lblStatusChoco.Text = _engine.ChocoAvailable
            ? "chocolatey disponivel"
            : "chocolatey nao encontrado";
        lblStatusChoco.ForeColor = _engine.ChocoAvailable
            ? Color.FromArgb(100, 200, 100)
            : Color.FromArgb(200, 100, 100);

        lblStatusGeral.Text = (!_engine.WingetAvailable && !_engine.ChocoAvailable)
            ? "Nenhum gerenciador encontrado!"
            : "Pronto para instalar";
        lblStatusGeral.ForeColor = (!_engine.WingetAvailable && !_engine.ChocoAvailable)
            ? Color.FromArgb(255, 180, 50)
            : Color.FromArgb(100, 200, 100);
    }

    private void BtnLimpar_Click(object? sender, EventArgs e)
    {
        _isUpdatingChecks = true;

        foreach (var prog in _selectedPrograms)
            prog.IsSelected = false;
        _selectedPrograms.Clear();

        foreach (ListViewItem item in lstPrograms.Items)
            item.Checked = false;

        _isUpdatingChecks = false;
        UpdateButtonStates();
    }

    private void BtnInstalar_Click(object? sender, EventArgs e)
    {
        if (!_engine.WingetAvailable && !_engine.ChocoAvailable)
        {
            using var setupForm = new SetupForm(_engine);
            if (setupForm.ShowDialog(this) != DialogResult.OK)
                return;

            if (!_engine.WingetAvailable && !_engine.ChocoAvailable)
            {
                MessageBox.Show(
                    "Nenhum gerenciador de pacotes foi instalrado.\n\n" +
                    "Instale o winget ou chocolatey manualmente para continuar.",
                    "Gerenciador nao encontrado",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        if (_selectedPrograms.Count == 0) return;

        var installItems = new List<InstallationItem>();
        foreach (var prog in _selectedPrograms)
        {
            if (_engine.WingetAvailable && !string.IsNullOrEmpty(prog.WingetId))
                installItems.Add(new InstallationItem { Program = prog, Source = "winget" });
            else if (_engine.ChocoAvailable && !string.IsNullOrEmpty(prog.ChocoId))
                installItems.Add(new InstallationItem { Program = prog, Source = "choco" });
            else if (_engine.WingetAvailable)
                installItems.Add(new InstallationItem { Program = prog, Source = "winget" });
            else
                installItems.Add(new InstallationItem
                {
                    Program = prog,
                    Source = "winget",
                    Status = InstallationStatus.Skipped,
                    StatusMessage = "Nao disponivel"
                });
        }

        var installForm = new InstallForm(_engine, installItems);
        installForm.ShowDialog(this);

        _engine.Refresh();
        UpdateStatusBar();
    }
}
