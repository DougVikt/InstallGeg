namespace InstaladorGeral;

public partial class SetupForm : Form
{
    private readonly InstallerEngine _engine;
    private bool _installing;

    public SetupForm(InstallerEngine engine)
    {
        _engine = engine;
        InitializeComponent();
    }

    private void SetupForm_Shown(object? sender, EventArgs e)
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        var wingetOk = _engine.WingetAvailable;
        var chocoOk = _engine.ChocoAvailable;

        lblWingetIcon.Text = wingetOk ? "V" : "X";
        lblWingetIcon.ForeColor = wingetOk ? Color.FromArgb(76, 175, 80) : Color.FromArgb(244, 67, 54);
        lblWingetStatus.Text = wingetOk ? "winget disponivel" : "winget nao encontrado";
        lblWingetStatus.ForeColor = wingetOk ? Color.FromArgb(100, 200, 100) : Color.FromArgb(200, 100, 100);
        btnInstallWinget.Text = wingetOk ? "OK" : "Instalar";
        btnInstallWinget.Enabled = !wingetOk && !_installing;
        btnInstallWinget.BackColor = wingetOk ? Color.FromArgb(48, 48, 48) : Color.FromArgb(0, 120, 215);

        lblChocoIcon.Text = chocoOk ? "V" : "X";
        lblChocoIcon.ForeColor = chocoOk ? Color.FromArgb(76, 175, 80) : Color.FromArgb(244, 67, 54);
        lblChocoStatus.Text = chocoOk ? "chocolatey disponivel" : "chocolatey nao encontrado";
        lblChocoStatus.ForeColor = chocoOk ? Color.FromArgb(100, 200, 100) : Color.FromArgb(200, 100, 100);
        btnInstallChoco.Text = chocoOk ? "OK" : "Instalar";
        btnInstallChoco.Enabled = !chocoOk && !_installing;
        btnInstallChoco.BackColor = chocoOk ? Color.FromArgb(48, 48, 48) : Color.FromArgb(0, 120, 215);

        btnContinue.Enabled = wingetOk || chocoOk;
        btnRefresh.Enabled = !_installing;
    }

    private async void BtnInstallWinget_Click(object? sender, EventArgs e)
    {
        btnInstallWinget.Enabled = false;
        _installing = true;
        btnRefresh.Enabled = false;

        var progress = new Progress<(int pct, string status)>();
        progress.ProgressChanged += (s, args) =>
        {
            if (IsDisposed) return;
            UpdateProgress(args.pct, args.status);
        };

        await _engine.InstallWingetAsync(progress);

        progressBar.Value = 0;
        lblStatusText.Text = "";
        _installing = false;
        UpdateUI();
    }

    private async void BtnInstallChoco_Click(object? sender, EventArgs e)
    {
        btnInstallChoco.Enabled = false;
        _installing = true;
        btnRefresh.Enabled = false;

        var progress = new Progress<(int pct, string status)>();
        progress.ProgressChanged += (s, args) =>
        {
            if (IsDisposed) return;
            UpdateProgress(args.pct, args.status);
        };

        await _engine.InstallChocoAsync(progress);

        progressBar.Value = 0;
        lblStatusText.Text = "";
        _installing = false;
        UpdateUI();
    }

    private void UpdateProgress(int pct, string status)
    {
        if (progressBar.InvokeRequired)
        {
            progressBar.Invoke(() => UpdateProgress(pct, status));
            return;
        }
        progressBar.Value = Math.Min(pct, 100);
        lblStatusText.Text = status;
    }

    private void BtnRefresh_Click(object? sender, EventArgs e)
    {
        _engine.Refresh();
        UpdateUI();
    }

    private void BtnContinue_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.OK;
        Close();
    }

    private void BtnFechar_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
