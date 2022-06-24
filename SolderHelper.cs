
namespace TechnicSolderPackager
{
    internal class SolderHelper
    {
        public static void ExecuteUploader(string[] args)
        { 
            string username = args[1];
            string password = args[2];
            IEnumerable<string> newMods = File.ReadAllLines("NewMods.csv");
            TechnicSolderUploader technicSolderUploader = new();
            technicSolderUploader.GetVersions("jei");
            technicSolderUploader.Login(username, password);
            List<string> mods = new();
            foreach (string mod in newMods)
            { 
                string modName = mod.Split('-')[0];
                string version = mod.Replace(".zip", "").Replace(mod.Split("-")[0]+"-", "");
                if (technicSolderUploader.DoesModExist(modName))
                {
                    if(technicSolderUploader.DoesModVersionExist(modName, version) == false){
                        Console.WriteLine($"Adding new mod version to solder! Mod: {modName}, Version: {version}");
                        if(technicSolderUploader.GetModID(modName) == -1)
                        {
                            Console.WriteLine("Could not get modID for {0}!", modName);
                        }
                        else
                        { 
                            technicSolderUploader.AddVersion(technicSolderUploader.GetModID(modName), version);
                            if (technicSolderUploader.GetVersions(modName).Contains(version) == false)
                            {
                                Console.WriteLine("Failed to add version {0} of {1}", version, modName);
                            }
                        }
                    } 

                }
                else
                { 
                    Console.WriteLine($"Registering new Mod: {modName}");
                    technicSolderUploader.RegisterMod(modName);

                    if(technicSolderUploader.DoesModExist(modName) == false)
                    {
                        Console.WriteLine("Could not add mod {0}", modName);
                    }
                    else
                    {
                        Console.WriteLine($"Adding new mod version to solder! Mod: {modName}, Version: {version}");
                        if (technicSolderUploader.GetModID(modName) == -1)
                        {
                            Console.WriteLine("Could not get modID for {0}!", modName);
                        }
                        else
                        {
                            technicSolderUploader.AddVersion(technicSolderUploader.GetModID(modName), version);
                            if (technicSolderUploader.GetVersions(modName).Contains(version) == false)
                            {
                                Console.WriteLine("Failed to add version {0} of {1}", version, modName);
                            }
                        }
                    }


                }
            }
            File.WriteAllLines("NewBuildMods.csv",  mods);
        }


    }
}
