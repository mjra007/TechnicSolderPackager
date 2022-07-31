
using System.Text.Json;

namespace TechnicSolderPackager
{
    internal class ModsRegisterHelper
    {
        public static void RegisterDeltaVersion(string[] args)
        { 
            string username = args[1];
            string password = args[2];
            ModpackDelta modpackDelta = JsonSerializer.Deserialize<ModpackDelta>(File.ReadAllText($"ModpackDelta.json"));
            SolderHelper solderHelper = new(args[3]);    
            Console.WriteLine("Trying to login!");
            solderHelper.Login(username, password); 
            Console.WriteLine("Starting to register mods!");


            List<Mod> modsToAddOrChangeVersion = modpackDelta.versionChanged;
            modsToAddOrChangeVersion.AddRange(modpackDelta.addedMods);

            foreach (Mod mod in modsToAddOrChangeVersion)
            { 
                Console.WriteLine($"===================Name:{mod.name} Version:{mod.version}===================");
                RegisterMod(solderHelper, mod);
                Thread.Sleep(10000);
            } 
        }

        public static void RegisterMod(SolderHelper solderHelper, Mod mod)
        {
            if (solderHelper.DoesModExist(mod.name))
            {
                Console.WriteLine("This mod is registered already so only need to register the new version!"); 
                RegisterVersion(solderHelper, mod);  
            }
            else
            {
                Console.WriteLine($"Registering new Mod: {mod.name}");
                solderHelper.CreateMod(mod.name);  
                if (solderHelper.DoesModExist(mod.name) == false)
                {
                    Console.WriteLine("Could not add mod {0}", mod.name);
                }
                else
                {
                    RegisterVersion(solderHelper, mod); 
                }  
            }
        }

        private static void RegisterVersion(SolderHelper solderHelper, Mod mod)
        {
            Console.WriteLine($"Adding new mod version to solder! Mod: {mod.name}, Version: {mod.version}");
            if (solderHelper.DoesModVersionExist(mod.name, mod.version) == false)
            {
                Console.WriteLine("This version does not exist will attempt to register it...");
                Console.WriteLine($"Adding new mod version to solder! Mod: {mod.name}, Version: {mod.version}");
                if (solderHelper.GetModID(mod.name) == -1)
                {
                    Console.WriteLine("Could not get modID for {0}!", mod.name);
                }
                else
                {
                    solderHelper.AddModVersion(solderHelper.GetModID(mod.name), mod.version);
                    if (solderHelper.GetModVersions(mod.name).Contains(mod.version) == false)
                    {
                        Console.WriteLine("Failed to add version {0} of {1}", mod.version, mod.name);
                    }
                }
            }
            else
            {
                Console.WriteLine("This mod version is already registered!");
            }
        }
    }
}
