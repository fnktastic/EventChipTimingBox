namespace Template_WinForms
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class SpeedwayReaderView : UserControl
    {
        private Label _readerNameLabel;
        private Label _statusLabel;
        private Button button1;
        private IContainer components;
        private TableLayoutPanel tableLayoutPanel1;

        public SpeedwayReaderView()
        {
            this.InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new TableLayoutPanel();
            this._readerNameLabel = new Label();
            this._statusLabel = new Label();
            this.button1 = new Button();
            this.tableLayoutPanel1.SuspendLayout();
            base.SuspendLayout();
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this._readerNameLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._statusLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.button1, 1, 0);
            this.tableLayoutPanel1.Dock = DockStyle.Fill;
            this.tableLayoutPanel1.Location = new Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
            this.tableLayoutPanel1.Size = new Size(0x41, 0x1a);
            this.tableLayoutPanel1.TabIndex = 0;
            this._readerNameLabel.AutoSize = true;
            this._readerNameLabel.Location = new Point(3, 0);
            this._readerNameLabel.Name = "_readerNameLabel";
            this._readerNameLabel.Size = new Size(40, 13);
            this._readerNameLabel.TabIndex = 0;
            this._readerNameLabel.Text = "0.0.0.0";
            this._statusLabel.AutoSize = true;
            this._statusLabel.Location = new Point(3, 13);
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.Size = new Size(0x29, 13);
            this._statusLabel.TabIndex = 0;
            this._statusLabel.Text = "(status)";
            this.button1.Dock = DockStyle.Top;
            this.button1.Location = new Point(0x2f, 0);
            this.button1.Margin = new Padding(0);
            this.button1.Name = "button1";
            this.tableLayoutPanel1.SetRowSpan(this.button1, 2);
            this.button1.Size = new Size(0x12, 0x12);
            this.button1.TabIndex = 1;
            this.button1.Text = "x";
            this.button1.UseVisualStyleBackColor = true;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            this.AutoSize = true;
            base.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            base.Controls.Add(this.tableLayoutPanel1);
            base.Name = "SpeedwayReaderView";
            base.Size = new Size(0x41, 0x1a);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            base.ResumeLayout(false);
            base.PerformLayout();
        }
    }
}

