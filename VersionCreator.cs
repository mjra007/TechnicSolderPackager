
using System.Text.Json;

namespace TechnicSolderPackager
{
    class VersionCreator
    {

        ModpackDelta modpackDelta;

        public VersionCreator(string[] args)
        { 
            string Ip = args[1]; 
            string user = args[2];
            string password= args[3];
            string modpackSlug = args[4];
            string minecraftVersion  = args[5];
            string file = args[6];
            SolderHelper helper = new(Ip);
            modpackDelta = JsonSerializer.Deserialize<ModpackDelta>(File.ReadAllText(file)); 
            helper.Login(user, password);
            helper.CreateModpackVersion(modpackDelta.version, minecraftVersion, helper.GetModpackIndex(modpackSlug));

            foreach (var mod in modpackDelta.removedMods)
            {
                helper.RemoveModFromModpack(mod.name, mod.version, modpackSlug, modpackDelta.version);
            }

            foreach (var mod in modpackDelta.versionChanged)
            {
                helper.ChangeVersionOfMod(mod.name, mod.version, modpackSlug, modpackDelta.version);
            }

            foreach (var mod in modpackDelta.addedMods)
            {
                helper.AddModToModpack(mod.name, mod.version, modpackSlug, modpackDelta.version);
            } 
        }
         
    }
}
