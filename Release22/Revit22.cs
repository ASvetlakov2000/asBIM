using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WixSharp;

namespace Release22
{
    internal class Revit22
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
                GUID = new Guid("63E31826-FFAC-4331-B5DC-330201949AF4"),
                MajorUpgrade = MajorUpgrade.Default,
                ControlPanelInfo =
                {
                    Manufacturer = Environment.UserName,
                },

                Dirs = new Dir[]
                {
                    new InstallDir(@"%AppDataFolder%\Autodesk\Revit\Addins\2022\",
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
