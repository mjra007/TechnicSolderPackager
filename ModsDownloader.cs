using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using System.Net; 
using Tomlyn;
using Tomlyn.Model;

namespace TechnicSolderPackager
{
    internal class ModsDownloader
    {
        public static void ExecuteDownloader(string[] args)
        {
            Console.WriteLine("Curseforge web based downloader!");
            string apiKey = args[1];
            string[] modInfoFiles = Directory.GetFiles("mods");
            var webClient = new WebClient();
            foreach (string modInfo in modInfoFiles)
            {
                Console.WriteLine(modInfo);
                string slug = modInfo.Split(Path.DirectorySeparatorChar).Last().Replace(".pw.toml", "");
                var model = Toml.ToModel(new StreamReader(modInfo).ReadToEnd());
                TomlTable curseforgeSection = (TomlTable)((TomlTable)model["update"])["curseforge"];
                long projectID = (long)curseforgeSection["project-id"];
                long fileID = (long)curseforgeSection["file-id"];
                string fileIDFirst = new(fileID.ToString().Take(4).ToArray());
                string fileIDLast = new(fileID.ToString()
                                          .Skip(4)
                                          .Take(3).ToArray());
                string filename = (string)model["filename"];
                webClient.DownloadFile(@$"https://edge.forgecdn.net/files/{fileIDFirst}/{fileIDLast}/{filename}?api-key={apiKey}", filename);
            }
        }

        public static void ExecuteBrowserBasedDownloader(string[] args)
        {
            //Console.WriteLine("Curseforge downloader!"); 
            //string[] modInfoFiles = Directory.GetFiles("mods");
            //ChromeOptions options = new();
            ////options.SetPreference("browser.download.alwaysOpenPanel", false); 
            ////options.SetPreference("browser.download.manager.showWhenStarting", false);
            ////options.SetPreference("browser.download.manager.focusWhenStarting", false);
            ////options.SetPreference("browser.download.folderList", 2); //Last downloaded folder
            ////options.SetPreference("browser.download.dir", Environment.CurrentDirectory); // Set your default download directory's path
            ////options.SetPreference("browser.helperApps.neverAsk.saveToDisk", "application/jar");
            ////options.SetPreference("browser.download.useDownloadDir", true);
            ////options.SetPreference("browser.download.manager.closeWhenDone", true);
            ////options.SetPreference("browser.download.manager.useWindow", false);

            //ChromeDriver driver = new();
            //driver.Navigate().GoToUrl("chrome://settings/?search=downloads");
            //var X =driver.FindElements(By.CssSelector("/settings-toggle-button"));
 
            //foreach (string modInfo in modInfoFiles)
            //{
            //    Console.WriteLine(modInfo);
            //    string slug = modInfo.Split(Path.DirectorySeparatorChar).Last().Replace(".pw.toml", "");
            //    var model = Toml.ToModel(new StreamReader(modInfo).ReadToEnd());
            //    TomlTable curseforgeSection = (TomlTable)((TomlTable)model["update"])["curseforge"]; 
            //    long fileID = (long)curseforgeSection["file-id"];
            //    string fileName = (string)model["filename"];
            //    string url = $"https://www.curseforge.com/minecraft/mc-mods/{slug}/download/{fileID}/file";
             
            //    while (File.Exists(fileName) == false)
            //    {
            //        driver.Navigate().GoToUrl(url); 
         
            //    }



            }
        }

 
}
