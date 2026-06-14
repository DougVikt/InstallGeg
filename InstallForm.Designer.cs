namespace InstaladorGeral;

partial class InstallForm
{
    private System.ComponentModel.IContainer components = null;
    private Panel panelHeader;
    private Label lblTitle;
    private Panel panelInstalls;
    private Panel panelBottom;
    private Button btnFechar;
    private Button btnCancelar;
    private Label lblTotal;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        panelHeader = new Panel();
        lblTitle = new Label();
        panelInstalls = new Panel();
        panelBottom = new Panel();
        btnFechar = new Button();
        btnCancelar = new Button();
        lblTotal = new Label();

        panelHeader.SuspendLayout();
        panelBottom.SuspendLayout();
        SuspendLayout();

        // panelHeader
        panelHeader.Dock = DockStyle.Top;
        panelHeader.Height = 50;
        panelHeader.BackColor = Color.FromArgb(32, 32, 32);
        panelHeader.Controls.Add(lblTitle);

        // lblTitle
        lblTitle.Text = "  📦 Instalação em Andamento";
        lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
        lblTitle.ForeColor = Color.White;
        lblTitle.Location = new Point(8, 10);
        lblTitle.AutoSize = true;

        // panelInstalls
        panelInstalls.Dock = DockStyle.Fill;
        panelInstalls.BackColor = Color.FromArgb(38, 38, 38);
        panelInstalls.AutoScroll = true;
        panelInstalls.Padding = new Padding(10);

        // panelBottom
        panelBottom.Dock = DockStyle.Bottom;
        panelBottom.Height = 50;
        panelBottom.BackColor = Color.FromArgb(32, 32, 32);
        panelBottom.Controls.Add(btnFechar);
        panelBottom.Controls.Add(btnCancelar);
        panelBottom.Controls.Add(lblTotal);

        // btnFechar
        btnFechar.Text = " Fechar";
        btnFechar.Size = new Size(120, 32);
        btnFechar.Location = new Point(panelBottom.Width - 140, 9);
        btnFechar.Anchor = AnchorStyles.Right | AnchorStyles.Top;
        btnFechar.FlatStyle = FlatStyle.Flat;
        btnFechar.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
        btnFechar.BackColor = Color.FromArgb(48, 48, 48);
        btnFechar.ForeColor = Color.FromArgb(200, 200, 200);
        btnFechar.Font = new Font("Segoe UI", 10);
        btnFechar.Cursor = Cursors.Hand;
        btnFechar.Enabled = false;
        btnFechar.Click += BtnFechar_Click;

        // btnCancelar
        btnCancelar.Text = " Cancelar";
        btnCancelar.Size = new Size(120, 32);
        btnCancelar.Location = new Point(panelBottom.Width - 270, 9);
        btnCancelar.Anchor = AnchorStyles.Right | AnchorStyles.Top;
        btnCancelar.FlatStyle = FlatStyle.Flat;
        btnCancelar.FlatAppearance.BorderColor = Color.FromArgb(200, 80, 80);
        btnCancelar.BackColor = Color.FromArgb(60, 30, 30);
        btnCancelar.ForeColor = Color.FromArgb(255, 150, 150);
        btnCancelar.Font = new Font("Segoe UI", 10);
        btnCancelar.Cursor = Cursors.Hand;
        btnCancelar.Click += BtnCancelar_Click;

        // lblTotal
        lblTotal.Text = "Total: 0/0 concluídos";
        lblTotal.Font = new Font("Segoe UI", 10, FontStyle.Regular);
        lblTotal.ForeColor = Color.FromArgb(180, 180, 180);
        lblTotal.Location = new Point(12, 14);
        lblTotal.AutoSize = true;

        // InstallForm
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(700, 550);
        MinimumSize = new Size(500, 350);
        Controls.Add(panelInstalls);
        Controls.Add(panelHeader);
        Controls.Add(panelBottom);
        BackColor = Color.FromArgb(30, 30, 30);
        ForeColor = Color.FromArgb(200, 200, 200);
        Font = new Font("Segoe UI", 10);
        Text = "Instalação em Andamento";
        StartPosition = FormStartPosition.CenterParent;
        ShowIcon = false;
        ShowInTaskbar = false;
        Shown += InstallForm_Shown;

        panelHeader.ResumeLayout(false);
        panelHeader.PerformLayout();
        panelBottom.ResumeLayout(false);
        panelBottom.PerformLayout();
        ResumeLayout(false);
    }
}
