namespace ECTL
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0} \n {1}",ex.Message, ex.StackTrace));
            }
        }     
    }
}

