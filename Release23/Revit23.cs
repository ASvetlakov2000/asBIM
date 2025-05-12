using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WixSharp;
using static WixSharp.SetupEventArgs;
using static WixSharp.Win32;

namespace Release23
{
    internal class Revit23
    {
        private static string projectName = "asBIM";
        private static string version = "1.0.0";
        static void Main(string[] args)
        {
            var project = new Project()
            {
                Name = projectName,
                UI = WUI.WixUI_ProgressOnly,
                OutDir = "output",
                GUID = new Guid("261E778E-099B-4FB8-9AA8-C64DEE56A53A"),
                MajorUpgrade = MajorUpgrade.Default,
                ControlPanelInfo =
                {
                    Manufacturer = Environment.UserName,
                },

                Dirs = new Dir[]
                {
                    new InstallDir(@"%AppDataFolder%\Autodesk\Revit\Addins\2023\",
                        new File(@"D:\asBIM_Релиз\v1.0.1\asBIM.addin"),
                        new Dir(@"asBIM",
                        new DirFiles(@"D:\asBIM_Релиз\v1.0.1\dll\*.*")))
                },

            };
            project.Version = new Version(version);

            project.BuildMsi();
        }
    }
}
