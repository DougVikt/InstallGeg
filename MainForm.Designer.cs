namespace InstaladorGeral;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;
    private Panel topBar;
    private Panel categoryPanel;
    private Panel programPanel;
    private FlowLayoutPanel actionPanel;
    private Button btnLimpar;
    private Button btnInstalar;
    private StatusStrip statusBar;
    private ToolStripStatusLabel lblStatusWinget;
    private ToolStripStatusLabel lblStatusChoco;
    private ToolStripStatusLabel lblStatusGeral;
    private Label lblCategoryTitle;
    private TextBox txtSearch;
    private ListView lstPrograms;
    private ColumnHeader colPrograma;
    private ColumnHeader colDescricao;
    private ColumnHeader colFonte;
    private Label lblContador;
    private Label lblSearchText;
    private Label lblSearchStatus = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        topBar = new Panel();
        actionPanel = new FlowLayoutPanel();
        btnLimpar = new Button();
        btnInstalar = new Button();
        lblContador = new Label();
        categoryPanel = new Panel();
        programPanel = new Panel();
        lblCategoryTitle = new Label();
        txtSearch = new TextBox();
        lblSearchText = new Label();
        lblSearchStatus = new Label();
        lstPrograms = new ListView();
        colPrograma = new ColumnHeader();
        colDescricao = new ColumnHeader();
        colFonte = new ColumnHeader();
        statusBar = new StatusStrip();
        lblStatusWinget = new ToolStripStatusLabel();
        lblStatusChoco = new ToolStripStatusLabel();
        lblStatusGeral = new ToolStripStatusLabel();

        topBar.SuspendLayout();
        actionPanel.SuspendLayout();
        programPanel.SuspendLayout();
        statusBar.SuspendLayout();
        SuspendLayout();

        // topBar
        topBar.Dock = DockStyle.Top;
        topBar.Height = 60;
        topBar.BackColor = Color.FromArgb(32, 32, 32);
        topBar.Padding = new Padding(12, 0, 12, 0);
        topBar.Controls.Add(actionPanel);
        topBar.Controls.Add(lblContador);

        // actionPanel
        actionPanel.FlowDirection = FlowDirection.RightToLeft;
        actionPanel.Dock = DockStyle.Right;
        actionPanel.Height = 60;
        actionPanel.Width = 360;
        actionPanel.Padding = new Padding(0, 12, 0, 12);
        actionPanel.Controls.Add(btnInstalar);
        actionPanel.Controls.Add(btnLimpar);

        // btnLimpar
        btnLimpar.Text = " Limpar";
        btnLimpar.Size = new Size(140, 36);
        btnLimpar.FlatStyle = FlatStyle.Flat;
        btnLimpar.FlatAppearance.BorderColor = Color.Black;
        btnLimpar.BackColor = Color.FromArgb(135,206,235);
        btnLimpar.ForeColor = Color.White; 
        btnLimpar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        btnLimpar.Cursor = Cursors.Hand;
        btnLimpar.Enabled = false;
        btnLimpar.Click += BtnLimpar_Click;

        // btnInstalar
        btnInstalar.Text = " Instalar";
        btnInstalar.Size = new Size(140, 36);
        btnInstalar.FlatStyle = FlatStyle.Flat;
        btnInstalar.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215);
        btnInstalar.BackColor = Color.FromArgb(0, 120, 215);
        btnInstalar.ForeColor = Color.White;
        btnInstalar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        btnInstalar.Cursor = Cursors.Hand;
        btnInstalar.Enabled = false;
        btnInstalar.Click += BtnInstalar_Click;

        // lblContador
        lblContador.Text = "InstallGet";
        lblContador.Font = new Font("Segoe UI", 16, FontStyle.Bold);
        lblContador.ForeColor = Color.White;
        lblContador.Location = new Point(16, 14);
        lblContador.AutoSize = true;

        // categoryPanel
        categoryPanel.Dock = DockStyle.Left;
        categoryPanel.Width = 250;
        categoryPanel.BackColor = Color.FromArgb(28, 28, 28);
        categoryPanel.AutoScroll = true;
        categoryPanel.Padding = new Padding(6, 6, 6, 6);

        // programPanel
        programPanel.Dock = DockStyle.Fill;
        programPanel.BackColor = Color.FromArgb(38, 38, 38);
        programPanel.Padding = new Padding(12, 8, 12, 8);
        programPanel.Controls.Add(lblCategoryTitle);
        programPanel.Controls.Add(lblSearchText);
        programPanel.Controls.Add(txtSearch);
        programPanel.Controls.Add(lblSearchStatus);
        programPanel.Controls.Add(lstPrograms);

        // lblCategoryTitle
        lblCategoryTitle.Text = "Selecione uma categoria";
        lblCategoryTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
        lblCategoryTitle.ForeColor = Color.FromArgb(220, 220, 220);
        lblCategoryTitle.Location = new Point(16, 10);
        lblCategoryTitle.AutoSize = true;

        // lblSearchText
        lblSearchText.Text = "Buscar:";
        lblSearchText.Location = new Point(10, 44);
        lblSearchText.Size = new Size(75, 27);
        lblSearchText.TextAlign = ContentAlignment.MiddleLeft;
        lblSearchText.Font = new Font("Segoe UI", 10);
        lblSearchText.ForeColor = Color.White;

        // txtSearch
        txtSearch.Location = new Point(85, 44);
        txtSearch.Size = new Size(340, 27);
        txtSearch.BackColor = Color.FromArgb(50, 50, 50);
        txtSearch.ForeColor = Color.FromArgb(220, 220, 220);
        txtSearch.BorderStyle = BorderStyle.FixedSingle;
        txtSearch.Font = new Font("Segoe UI", 10);
        txtSearch.TextChanged += TxtSearch_TextChanged;

        // lblSearchStatus
        lblSearchStatus.Text = "";
        lblSearchStatus.Location = new Point(430, 44);
        lblSearchStatus.Size = new Size(280, 27);
        lblSearchStatus.TextAlign = ContentAlignment.MiddleLeft;
        lblSearchStatus.Font = new Font("Segoe UI", 9, FontStyle.Italic);
        lblSearchStatus.ForeColor = Color.FromArgb(100, 180, 255);

        // lstPrograms
        lstPrograms.Location = new Point(16, 78);
        lstPrograms.Size = new Size(730, 520);
        lstPrograms.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lstPrograms.View = View.Details;
        lstPrograms.CheckBoxes = true;
        lstPrograms.FullRowSelect = true;
        lstPrograms.MultiSelect = true;
        lstPrograms.HeaderStyle = ColumnHeaderStyle.Clickable;
        lstPrograms.BackColor = Color.FromArgb(42, 42, 42);
        lstPrograms.ForeColor = Color.FromArgb(210, 210, 210);
        lstPrograms.Font = new Font("Segoe UI", 10);
        lstPrograms.BorderStyle = BorderStyle.None;
        lstPrograms.Columns.AddRange(new[] { colPrograma, colDescricao, colFonte });
        lstPrograms.ItemChecked += LstPrograms_ItemChecked;

        // colPrograma
        colPrograma.Text = "Programa";
        colPrograma.Width = 260;

        // colDescricao
        colDescricao.Text = "Descrição";
        colDescricao.Width = 350;

        // colFonte
        colFonte.Text = "Fonte";
        colFonte.Width = 90;

        // statusBar
        statusBar.BackColor = Color.FromArgb(32, 32, 32);
        statusBar.ForeColor = Color.FromArgb(180, 180, 180);
        statusBar.Font = new Font("Segoe UI", 9);
        statusBar.Items.AddRange(new ToolStripItem[] {
            lblStatusWinget, lblStatusChoco, lblStatusGeral
        });

        lblStatusWinget.Text = "Verificando winget...";
        lblStatusWinget.AutoSize = true;

        lblStatusChoco.Text = "Verificando choco...";
        lblStatusChoco.AutoSize = true;

        lblStatusGeral.Text = "Pronto";
        lblStatusGeral.Spring = true;
        lblStatusGeral.TextAlign = ContentAlignment.MiddleRight;

        // MainForm
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 680);
        MinimumSize = new Size(800, 500);
        Controls.Add(programPanel);
        Controls.Add(categoryPanel);
        Controls.Add(topBar);
        Controls.Add(statusBar);
        BackColor = Color.FromArgb(30, 30, 30);
        ForeColor = Color.FromArgb(200, 200, 200);
        Font = new Font("Segoe UI", 10);
        Text = "InstallGet";
        StartPosition = FormStartPosition.CenterScreen;
        using (var iconStream = typeof(MainForm).Assembly.GetManifestResourceStream("InstaladorGeral.Resources.instalador.ico"))
            if (iconStream != null) Icon = new Icon(iconStream);

        topBar.ResumeLayout(false);
        topBar.PerformLayout();
        actionPanel.ResumeLayout(false);
        programPanel.ResumeLayout(false);
        programPanel.PerformLayout();
        statusBar.ResumeLayout(false);
        statusBar.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }
}
