namespace InstaladorGeral;

partial class SetupForm
{
    private System.ComponentModel.IContainer components = null;
    private Panel panelHeader;
    private Label lblTitle;
    private Panel panelBody;
    private Label lblWingetIcon;
    private Label lblWingetStatus;
    private Button btnInstallWinget;
    private Label lblChocoIcon;
    private Label lblChocoStatus;
    private Button btnInstallChoco;
    private ProgressBar progressBar;
    private Label lblStatusText;
    private Panel panelBottom;
    private Button btnRefresh;
    private Button btnContinue;
    private Button btnFechar;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        panelHeader = new Panel();
        lblTitle = new Label();
        panelBody = new Panel();
        lblWingetIcon = new Label();
        lblWingetStatus = new Label();
        btnInstallWinget = new Button();
        lblChocoIcon = new Label();
        lblChocoStatus = new Label();
        btnInstallChoco = new Button();
        progressBar = new ProgressBar();
        lblStatusText = new Label();
        panelBottom = new Panel();
        btnRefresh = new Button();
        btnContinue = new Button();
        btnFechar = new Button();

        panelHeader.SuspendLayout();
        panelBody.SuspendLayout();
        panelBottom.SuspendLayout();
        SuspendLayout();

        // panelHeader
        panelHeader.Dock = DockStyle.Top;
        panelHeader.Height = 50;
        panelHeader.BackColor = Color.FromArgb(32, 32, 32);
        panelHeader.Controls.Add(lblTitle);

        // lblTitle
        lblTitle.Text = "  Gerenciadores de Pacotes";
        lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
        lblTitle.ForeColor = Color.White;
        lblTitle.Location = new Point(8, 10);
        lblTitle.AutoSize = true;

        // panelBody
        panelBody.Dock = DockStyle.Fill;
        panelBody.BackColor = Color.FromArgb(38, 38, 38);
        panelBody.Padding = new Padding(16);
        panelBody.Controls.Add(lblWingetIcon);
        panelBody.Controls.Add(lblWingetStatus);
        panelBody.Controls.Add(btnInstallWinget);
        panelBody.Controls.Add(lblChocoIcon);
        panelBody.Controls.Add(lblChocoStatus);
        panelBody.Controls.Add(btnInstallChoco);
        panelBody.Controls.Add(progressBar);
        panelBody.Controls.Add(lblStatusText);

        // lblWingetIcon
        lblWingetIcon.Text = "?";
        lblWingetIcon.Font = new Font("Segoe UI", 18);
        lblWingetIcon.Location = new Point(16, 16);
        lblWingetIcon.Size = new Size(40, 36);
        lblWingetIcon.TextAlign = ContentAlignment.MiddleCenter;

        // lblWingetStatus
        lblWingetStatus.Text = "winget: verificando...";
        lblWingetStatus.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        lblWingetStatus.ForeColor = Color.FromArgb(220, 220, 220);
        lblWingetStatus.Location = new Point(60, 18);
        lblWingetStatus.Size = new Size(340, 32);
        lblWingetStatus.TextAlign = ContentAlignment.MiddleLeft;

        // btnInstallWinget
        btnInstallWinget.Text = "Instalar";
        btnInstallWinget.Size = new Size(110, 32);
        btnInstallWinget.Location = new Point(410, 18);
        btnInstallWinget.FlatStyle = FlatStyle.Flat;
        btnInstallWinget.BackColor = Color.FromArgb(0, 120, 215);
        btnInstallWinget.ForeColor = Color.White;
        btnInstallWinget.Font = new Font("Segoe UI", 10);
        btnInstallWinget.Cursor = Cursors.Hand;
        btnInstallWinget.Click += BtnInstallWinget_Click;

        // lblChocoIcon
        lblChocoIcon.Text = "?";
        lblChocoIcon.Font = new Font("Segoe UI", 18);
        lblChocoIcon.Location = new Point(16, 66);
        lblChocoIcon.Size = new Size(40, 36);
        lblChocoIcon.TextAlign = ContentAlignment.MiddleCenter;

        // lblChocoStatus
        lblChocoStatus.Text = "chocolatey: verificando...";
        lblChocoStatus.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        lblChocoStatus.ForeColor = Color.FromArgb(220, 220, 220);
        lblChocoStatus.Location = new Point(60, 68);
        lblChocoStatus.Size = new Size(340, 32);
        lblChocoStatus.TextAlign = ContentAlignment.MiddleLeft;

        // btnInstallChoco
        btnInstallChoco.Text = "Instalar";
        btnInstallChoco.Size = new Size(110, 32);
        btnInstallChoco.Location = new Point(410, 68);
        btnInstallChoco.FlatStyle = FlatStyle.Flat;
        btnInstallChoco.BackColor = Color.FromArgb(0, 120, 215);
        btnInstallChoco.ForeColor = Color.White;
        btnInstallChoco.Font = new Font("Segoe UI", 10);
        btnInstallChoco.Cursor = Cursors.Hand;
        btnInstallChoco.Click += BtnInstallChoco_Click;

        // progressBar
        progressBar.Location = new Point(16, 116);
        progressBar.Size = new Size(504, 24);
        progressBar.Minimum = 0;
        progressBar.Maximum = 100;
        progressBar.Value = 0;
        progressBar.Style = ProgressBarStyle.Continuous;
        progressBar.ForeColor = Color.FromArgb(0, 120, 215);
        progressBar.BackColor = Color.FromArgb(30, 30, 30);

        // lblStatusText
        lblStatusText.Text = "";
        lblStatusText.Font = new Font("Segoe UI", 9);
        lblStatusText.ForeColor = Color.FromArgb(140, 140, 140);
        lblStatusText.Location = new Point(16, 144);
        lblStatusText.Size = new Size(504, 20);
        lblStatusText.TextAlign = ContentAlignment.MiddleLeft;

        // panelBottom
        panelBottom.Dock = DockStyle.Bottom;
        panelBottom.Height = 50;
        panelBottom.BackColor = Color.FromArgb(32, 32, 32);
        panelBottom.Controls.Add(btnFechar);
        panelBottom.Controls.Add(btnContinue);
        panelBottom.Controls.Add(btnRefresh);

        // btnFechar
        btnFechar.Text = "Fechar";
        btnFechar.Size = new Size(100, 32);
        btnFechar.Location = new Point(420, 9);
        btnFechar.Anchor = AnchorStyles.Right | AnchorStyles.Top;
        btnFechar.FlatStyle = FlatStyle.Flat;
        btnFechar.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
        btnFechar.BackColor = Color.FromArgb(48, 48, 48);
        btnFechar.ForeColor = Color.FromArgb(200, 200, 200);
        btnFechar.Font = new Font("Segoe UI", 10);
        btnFechar.Cursor = Cursors.Hand;
        btnFechar.Click += BtnFechar_Click;

        // btnContinue
        btnContinue.Text = "Continuar";
        btnContinue.Size = new Size(120, 32);
        btnContinue.Location = new Point(290, 9);
        btnContinue.Anchor = AnchorStyles.Right | AnchorStyles.Top;
        btnContinue.FlatStyle = FlatStyle.Flat;
        btnContinue.BackColor = Color.FromArgb(0, 120, 215);
        btnContinue.ForeColor = Color.White;
        btnContinue.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        btnContinue.Cursor = Cursors.Hand;
        btnContinue.Enabled = false;
        btnContinue.Click += BtnContinue_Click;

        // btnRefresh
        btnRefresh.Text = "Atualizar";
        btnRefresh.Size = new Size(100, 32);
        btnRefresh.Location = new Point(12, 9);
        btnRefresh.FlatStyle = FlatStyle.Flat;
        btnRefresh.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
        btnRefresh.BackColor = Color.FromArgb(48, 48, 48);
        btnRefresh.ForeColor = Color.FromArgb(200, 200, 200);
        btnRefresh.Font = new Font("Segoe UI", 10);
        btnRefresh.Cursor = Cursors.Hand;
        btnRefresh.Click += BtnRefresh_Click;

        // SetupForm
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(540, 240);
        MinimumSize = new Size(540, 240);
        MaximumSize = new Size(540, 280);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Controls.Add(panelBody);
        Controls.Add(panelHeader);
        Controls.Add(panelBottom);
        BackColor = Color.FromArgb(30, 30, 30);
        ForeColor = Color.FromArgb(200, 200, 200);
        Font = new Font("Segoe UI", 10);
        Text = "Gerenciadores de Pacotes";
        StartPosition = FormStartPosition.CenterParent;
        ShowIcon = false;
        ShowInTaskbar = false;
        Shown += SetupForm_Shown;

        panelHeader.ResumeLayout(false);
        panelHeader.PerformLayout();
        panelBody.ResumeLayout(false);
        panelBottom.ResumeLayout(false);
        ResumeLayout(false);
    }
}
