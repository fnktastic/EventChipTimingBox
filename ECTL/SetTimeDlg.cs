namespace ECTL
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class SetTimeDlg : Form
    {
        private Button _btnSetTime;
        private NumericUpDown _tbHour;
        private NumericUpDown _tbMin;
        private NumericUpDown _tbSec;
        private IContainer components;
        private GroupBox groupBox1;
        private Label label1;
        private Label label2;

        public SetTimeDlg()
        {
            this.InitializeComponent();
            DateTime now = DateTime.Now;
            this._tbHour.Text = now.ToString("HH");
            this._tbMin.Text = now.ToString("mm");
            this._tbSec.Text = now.ToString("ss");
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
            this.groupBox1 = new GroupBox();
            this._tbSec = new NumericUpDown();
            this.label2 = new Label();
            this._tbMin = new NumericUpDown();
            this._tbHour = new NumericUpDown();
            this.label1 = new Label();
            this._btnSetTime = new Button();
            this.groupBox1.SuspendLayout();
            this._tbSec.BeginInit();
            this._tbMin.BeginInit();
            this._tbHour.BeginInit();
            base.SuspendLayout();
            this.groupBox1.Controls.Add(this._tbSec);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this._tbMin);
            this.groupBox1.Controls.Add(this._tbHour);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = new Font("Calibri", 18f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.groupBox1.Location = new Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(0x13f, 100);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Time";
            this._tbSec.Font = new Font("Calibri", 24f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this._tbSec.Location = new Point(0xe9, 0x24);
            int[] bits = new int[4];
            bits[0] = 0x3b;
            this._tbSec.Maximum = new decimal(bits);
            this._tbSec.Name = "_tbSec";
            this._tbSec.Size = new Size(0x42, 0x2f);
            this._tbSec.TabIndex = 7;
            this._tbSec.TextAlign = HorizontalAlignment.Center;
            int[] numArray2 = new int[4];
            numArray2[0] = 0x17;
            this._tbSec.Value = new decimal(numArray2);
            this.label2.AutoSize = true;
            this.label2.Font = new Font("Calibri", 21.75f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.label2.Location = new Point(0xca, 0x26);
            this.label2.Name = "label2";
            this.label2.Size = new Size(0x17, 0x24);
            this.label2.TabIndex = 6;
            this.label2.Text = ":";
            this.label2.TextAlign = ContentAlignment.MiddleCenter;
            this._tbMin.Font = new Font("Calibri", 24f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this._tbMin.Location = new Point(0x80, 0x24);
            int[] numArray3 = new int[4];
            numArray3[0] = 0x3b;
            this._tbMin.Maximum = new decimal(numArray3);
            this._tbMin.Name = "_tbMin";
            this._tbMin.Size = new Size(0x42, 0x2f);
            this._tbMin.TabIndex = 5;
            this._tbMin.TextAlign = HorizontalAlignment.Center;
            int[] numArray4 = new int[4];
            numArray4[0] = 0x17;
            this._tbMin.Value = new decimal(numArray4);
            this._tbHour.Font = new Font("Calibri", 24f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this._tbHour.Location = new Point(0x17, 0x24);
            int[] numArray5 = new int[4];
            numArray5[0] = 0x17;
            this._tbHour.Maximum = new decimal(numArray5);
            this._tbHour.Name = "_tbHour";
            this._tbHour.Size = new Size(0x42, 0x2f);
            this._tbHour.TabIndex = 4;
            this._tbHour.TextAlign = HorizontalAlignment.Center;
            int[] numArray6 = new int[4];
            numArray6[0] = 0x17;
            this._tbHour.Value = new decimal(numArray6);
            this.label1.AutoSize = true;
            this.label1.Font = new Font("Calibri", 21.75f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.label1.Location = new Point(0x62, 0x26);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x17, 0x24);
            this.label1.TabIndex = 1;
            this.label1.Text = ":";
            this.label1.TextAlign = ContentAlignment.MiddleCenter;
            this._btnSetTime.Font = new Font("Calibri", 18f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this._btnSetTime.Location = new Point(0x79, 0x7c);
            this._btnSetTime.Margin = new Padding(2, 3, 2, 3);
            this._btnSetTime.Name = "_btnSetTime";
            this._btnSetTime.Size = new Size(0x65, 0x26);
            this._btnSetTime.TabIndex = 0;
            this._btnSetTime.Text = "Set Time";
            this._btnSetTime.UseVisualStyleBackColor = true;
            this._btnSetTime.Click += new EventHandler(this.OnSetTimeClicked);
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            base.ClientSize = new Size(0x157, 0xad);
            base.Controls.Add(this._btnSetTime);
            base.Controls.Add(this.groupBox1);
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "SetTimeDlg";
            this.Text = "Set Time";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this._tbSec.EndInit();
            this._tbMin.EndInit();
            this._tbHour.EndInit();
            base.ResumeLayout(false);
        }

        private void OnKeyPressed(object sender, KeyPressEventArgs e)
        {
            TextBox box = sender as TextBox;
            if (e.KeyChar != '\b')
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))
                {
                    e.Handled = true;
                }
                else
                {
                    string str = box.Text + e.KeyChar;
                    e.Handled = Convert.ToInt32(str) > 0x17;
                }
            }
        }

        private void OnSetTimeClicked(object sender, EventArgs e)
        {
            SYSTEMTIME lpLocalTime = new SYSTEMTIME();
            lpLocalTime.FromDateTime(DateTime.Now);
            lpLocalTime.Hour = Convert.ToInt16(this._tbHour.Text);
            lpLocalTime.Minute = Convert.ToInt16(this._tbMin.Text);
            lpLocalTime.Second = Convert.ToInt16(this._tbSec.Text);
            SetLocalTime(ref lpLocalTime);
            base.Close();
        }

        [DllImport("kernel32.dll")]
        private static extern bool SetLocalTime([In] ref SYSTEMTIME lpLocalTime);

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEMTIME
        {
            public short Year;
            public short Month;
            public short DayOfWeek;
            public short Day;
            public short Hour;
            public short Minute;
            public short Second;
            public short Millisecond;
            public void FromDateTime(DateTime time)
            {
                this.Year = (short) time.Year;
                this.Month = (short) time.Month;
                this.DayOfWeek = (short) time.DayOfWeek;
                this.Day = (short) time.Day;
                this.Hour = (short) time.Hour;
                this.Minute = (short) time.Minute;
                this.Second = (short) time.Second;
                this.Millisecond = (short) time.Millisecond;
            }
        }
    }
}

