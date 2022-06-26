using System.IO.Compression; 

namespace TechnicSolderPackager
{
    internal class SolderUnpacker
    {
        public static void Unpack(string[] args)
        {
            string releaseFileName = args[1];
            string[] existingModFolders = Directory.GetDirectories("." + Path.DirectorySeparatorChar)
                .Select(s => s.Replace("." + Path.DirectorySeparatorChar, "").ToLower()) 
                .ToArray();

            if (Directory.Exists("newBuild"))
            {
                Directory.Delete("newBuild", true);
            }
            else
            {
                Directory.CreateDirectory("newBuild");
            }

            ZipFile.ExtractToDirectory(releaseFileName, "newBuild");
           
            foreach (var zipFile in Directory.GetFiles("newBuild"))
            {
                string modName = zipFile.Split(Path.DirectorySeparatorChar).Last().Split('-')[0];
                Console.WriteLine("Starting to extract {0}", zipFile);
                if (existingModFolders.Contains(modName.ToLower()))
                { 
                    Console.WriteLine(" => Mod folder already exists! ");
                    IEnumerable<string> versions = Directory.GetFiles(modName).Select(s => s.Split(Path.DirectorySeparatorChar).Last());
                    string fileName = zipFile.Split(Path.DirectorySeparatorChar).Last(); 
                    Console.WriteLine("Versions: "+string.Join($",{Environment.NewLine}", versions));
                    Console.WriteLine("Filename: " + fileName);

                    //if mod file doesnt exists
                    if (versions.Any(s=>s.ToLower().Equals(fileName.ToLower())))
                    { 
                        Console.WriteLine("   => Mod version already exists...");
                    }
                    else
                    {
                        Console.WriteLine("   => Copying new mod version to folder...");
                        Console.WriteLine("    => Source: {0}", zipFile);
                        Console.WriteLine("    => Destination: {0}", Path.Combine(modName, zipFile.Split(Path.DirectorySeparatorChar).Last()));
                        File.Copy(zipFile, Path.Combine(modName, zipFile.Split(Path.DirectorySeparatorChar).Last()));
                    }
                }
                else
                {
                    Console.WriteLine(" => Creating mod folder for {0}", modName);
                    Directory.CreateDirectory(modName);
                    Console.WriteLine("   => Copying new mod version to folder...");
                    Console.WriteLine("    => Source: {0}", zipFile);
                    Console.WriteLine("    => Destination: {0}", Path.Combine(modName, zipFile.Split(Path.DirectorySeparatorChar).Last()));
                    File.Copy(zipFile, Path.Combine(modName, zipFile.Split(Path.DirectorySeparatorChar).Last()));
                }
            }
          
        }


    }
}
