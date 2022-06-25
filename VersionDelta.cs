using Newtonsoft.Json.Linq; 
using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using System.Text.Json; 

namespace TechnicSolderPackager
{
    internal class VersionDelta
    {

        public HttpClient client = new();


        public VersionDelta()
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void CompareVersions(string[] args)
        { 
            string modpackSlug = args[2];
            string apiIP  = args[3];
            string version = args[4];

            List<Mod> allModsNewVersion = BuildNewModsList(); 
            List<Mod> allModsOldVersion = GetModsInVersion(modpackSlug, GetLatestVersion(modpackSlug, apiIP), apiIP).ToList();

            List<Mod> removedMods = GetRemovedMods(allModsNewVersion, allModsOldVersion);
            List<Mod> versionChanges = GetVersionChanges(allModsNewVersion, allModsOldVersion);
            List<Mod> addedMods = GetAddedMods(allModsNewVersion, allModsOldVersion);
             
            string delta = JsonSerializer.Serialize<ModpackDelta>(new ModpackDelta() 
            { 
                version = version,
                removedMods = removedMods,
                addedMods = addedMods,
                versionChanged = versionChanges
            }, new JsonSerializerOptions() {Encoder= JavaScriptEncoder.UnsafeRelaxedJsonEscaping, WriteIndented = true});

            File.WriteAllText($"ModpackDelta-{version}.json", delta);
        }


        private List<Mod> BuildNewModsList(/*Dictionary<string,string> modOverrides*/)
        {
            List<Mod> mods = new(); 
            string[] modInfoFiles = Directory.GetFiles("builds"); 
            foreach (string modInfo in modInfoFiles)
            {
                Console.WriteLine(modInfo);
                //var model = Toml.ToModel(new StreamReader(modInfo).ReadToEnd());
                //string filename = (string)model["filename"];
                //foreach (string originalName in modOverrides.Keys)
                //{
                //    if (filename.StartsWith(originalName))
                //    {
                //        filename = filename.Replace(originalName, modOverrides[originalName]);  
                //    }
                //} 
                string filename = modInfo.Split(Path.DirectorySeparatorChar).Last();
                string modName = filename.Split('-')[0];
                string version = filename.Replace(".zip", "").Replace(filename.Split("-")[0] + "-", "");
                mods.Add(new Mod()
                {
                    name = modName,
                    version = version
                });
            }
            return mods;
        }


        public static void ApplyNewModNames(IEnumerable<Mod> mods, Dictionary<string, string> modNamesOverride)
        {
            foreach (Mod mod in mods)
            {
                string name = mod.name;
                string newName = name;
                foreach (string originalName in modNamesOverride.Keys)
                {
                    if (newName.StartsWith(originalName))
                    {
                        newName = name.Replace(originalName, modNamesOverride[originalName]);
                        mod.name = newName;
                        Console.WriteLine("  Transformed jar name: \"{0}\" => \"{1}\"", name, newName); 
                    }
                }

            }
        } 

        private List<Mod> GetAddedMods(List<Mod> allModsNewVersion, List<Mod> allModsOldVersion)
        {
            List<Mod> addedMods = new();

            for (int i = 0; i < allModsNewVersion.Count; i++)
            {
                bool doesModExistInOldVersion = allModsOldVersion.Any(s => s.name.Equals(allModsNewVersion[i].name));
                if (doesModExistInOldVersion == false)
                {
                    addedMods.Add(allModsNewVersion[i]);
                }
            }
            return addedMods;
        }

        private List<Mod> GetVersionChanges(List<Mod> allModsNewVersion, List<Mod> allModsOldVersion)
        {
            List<Mod> versionChange = new();

            for (int i = 0; i < allModsNewVersion.Count; i++)
            {
                Mod newMod = allModsNewVersion[i];
                Mod oldMod = allModsOldVersion.FirstOrDefault(s => s.name.Equals(newMod.name), null);
                if (oldMod != null)
                { 
                    if(newMod.version.Equals(oldMod.version) == false)
                    {
                        versionChange.Add(newMod);
                    }
                }
            }
            return versionChange;
        }

        private List<Mod> GetRemovedMods(List<Mod> allModsNewVersion, List<Mod> allModsOldVersion)
        {
            List<Mod> removedMods = new();

            for (int i = 0; i < allModsOldVersion.Count; i++)
            { 
                if(allModsNewVersion.Any(s=>s.name == allModsOldVersion[i].name) == false)
                {
                    removedMods.Add(allModsOldVersion[i]);
                }
            }
            return removedMods;
        }



        public IEnumerable<Mod> GetModsInVersion(string modpackSlug, string version, string IP)
        {
            var response = client.GetStringAsync($"http://{IP}/api/modpack/{modpackSlug}/{version}"); 
            var result = response.Result;
            if (response.IsCompletedSuccessfully)
            {
                JArray versionsAsArray = JObject.Parse(result)["mods"] as JArray; 
                return versionsAsArray.ToObject<List<Mod>>();
            }
            return new List<Mod>();
        }

        public string GetLatestVersion(string modpackSlug, string IP)
        {
            var response = client.GetStringAsync($"http://{IP}/api/modpack/{modpackSlug}");
            var result = response.Result;
            if (response.IsCompletedSuccessfully)
            {
                string latestVersion = (string)JObject.Parse(result)["latest"];
                return latestVersion;
            }
            return string.Empty;
        }

    }
     
    public class Mod
    {
        public int id { get; set; }
        public string name { get; set; }
        public string version { get; set; }
        public string md5 { get; set; }
        public string filesize { get; set; }
        public string url { get; set; }
    }
     
    public class ModpackDelta
    {
        public string version { get; set; }
        public List<Mod> removedMods { get; set; }
        public List<Mod> addedMods { get; set; }
        public List<Mod> versionChanged { get; set; }
    }

}
