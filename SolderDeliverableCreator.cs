using System.IO.Compression; 

namespace TechnicSolderPackager
{
    internal class SolderDeliverableCreator
    {
        public static void Pack(string[] args)
        {
            Console.WriteLine("TechnicSolder creating deliverable!");
            string packname = args[2];
            string packVersion = args[1];
            string currentPath = Environment.CurrentDirectory;
            string[] mods = Directory.GetFiles(currentPath).Where(s => s.Contains("jar")).ToArray();
            Dictionary<string, string> modNamesOverride = GetModNameOverrides();

            if (!Directory.Exists("builds"))
            {
                Directory.CreateDirectory("builds");
            }
            else
            {
                Directory.Delete("builds", true);
                Directory.CreateDirectory("builds");
            }

            //Changing the modnames ensures compatiblity with technic solder naming conventions
            ApplyNewModNames(mods, modNamesOverride);

            //get new paths after the rename
            mods = Directory.GetFiles(currentPath).Where(s => s.Contains("jar")).ToArray();

            foreach (string mod in mods)
            {
                string fileName = mod.Split(Path.DirectorySeparatorChar).Last();
                string fileNameNoJar = fileName.Replace(".jar", "");
                string folderModName = fileName.Split("-")[0];

                Console.WriteLine(mod);
                Directory.CreateDirectory(Path.Combine("builds", folderModName, "mods"));
                File.Copy(mod, Path.Combine("builds", folderModName, "mods", fileName));
                ZipFile.CreateFromDirectory(Path.Combine("builds", folderModName), Path.Combine("builds", fileNameNoJar + ".zip"));
                Directory.Delete(Path.Combine("builds", folderModName), true);
            }

            ZipFile.CreateFromDirectory($"config", Path.Combine("builds", $"config-{ packVersion}.zip"), CompressionLevel.NoCompression, includeBaseDirectory: true);
            ZipFile.CreateFromDirectory($"animation", Path.Combine("builds", $"animation-{ packVersion}.zip"), CompressionLevel.NoCompression, includeBaseDirectory: true);
            ZipFile.CreateFromDirectory($"customnpcs", Path.Combine("builds", $"customnpcs-{ packVersion}.zip"), CompressionLevel.NoCompression, includeBaseDirectory: true);
            ZipFile.CreateFromDirectory($"resources", Path.Combine("builds", $"resources-{ packVersion}.zip"), CompressionLevel.NoCompression, includeBaseDirectory: true);
            Console.WriteLine($"{packname}-{packVersion}.zip ");
            File.Delete($"{packname}-{packVersion}.zip");
            ZipFile.CreateFromDirectory("builds", $"{packname}-{packVersion}.zip"); 
        }

        public static void ApplyNewModNames(string[] mods, Dictionary<string, string> modNamesOverride)
        {
            foreach (string mod in mods)
            {
                string fileName = mod.Split(Path.DirectorySeparatorChar).Last();
                string newName = fileName;
                foreach (string originalName in modNamesOverride.Keys)
                {
                    if (fileName.StartsWith(originalName))
                    {
                        newName = newName.Replace(originalName, modNamesOverride[originalName]); 
                        Console.WriteLine("  Transformed jar name: \"{0}\" => \"{1}\"", fileName, newName);
                        //There is no official method to rename things in c# so you have to use File.Move
                        //However File.Move will not work if the name change is just changing the letter case therefore this hack is needed to ensure the file name is changed
                        string temporaryName = "asdadadadadadatemp" + fileName;
                        File.Move(fileName, temporaryName);
                        File.Move(temporaryName, newName);
                    }
                } 

            }
        }

       public static Dictionary<string, string> GetModNameOverrides()
        {
            Dictionary<string, string> modNames = new();
            if (File.Exists(".modnamesoverride"))
            {
               IEnumerable<string> modNamesOverride = File.ReadLines(".modnamesoverride");
               foreach (string modNameOverridePair in modNamesOverride)
               {
                    string[] pair = modNameOverridePair.Split("|");
                    modNames.Add(pair[0], pair[1]); 
               }
                return modNames;
            }
            else
            {
                return modNames;
            }
        }

       
  
    }
}
