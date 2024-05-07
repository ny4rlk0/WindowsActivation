using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace WindowsActivation
{
    public partial class Form1 : MaterialForm
    {
        public Form1()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Grey900, Primary.Grey600, Primary.Grey100, Accent.Teal100, TextShade.WHITE);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            runAsAdministrator();
        }
        #region Admin olarak çalıştırma metodu
        static private void runAsAdministrator()
        {
            var wi = WindowsIdentity.GetCurrent();
            var wp = new WindowsPrincipal(wi);
            bool runAsAdmin = wp.IsInRole(WindowsBuiltInRole.Administrator);
            if (!runAsAdmin)
            {
                var processInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().CodeBase);
                processInfo.UseShellExecute = true;
                processInfo.Verb = "runas";
                try { Process.Start(processInfo); }
                catch (Exception) { MessageBox.Show("Run this program as Administrator!"); }
                System.Windows.Forms.Application.Exit();
                Environment.Exit(0);
            }
        }
        #endregion

        private void materialButton1_Click(object sender, EventArgs e)
        {
            Thread bt = new Thread(() => ps("irm https://massgrave.dev/get | iex"));
            bt.Start();
            materialButton1.Enabled = false;
        }
        #region Powershell komut çalıştırma metodu
        private string ps(string komut)
        {
            string output = "", error = "";
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = "-noprofile -executionpolicy bypass -command \"" + komut + "\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using (Process process = new Process { StartInfo = psi })
                {
                    process.Start();
                    output = process.StandardOutput.ReadToEnd();
                    error = process.StandardError.ReadToEnd();
                    process.WaitForExit();
                    return output.Trim() + error.Trim();
                }
            }
            catch (Exception ex)
            { return ex.ToString(); }
            finally
            {
                this.Invoke((MethodInvoker)delegate { materialButton1.Enabled = true; });
                
            }
        }
        #endregion
    }
}
