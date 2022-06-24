﻿using System.IO.Compression; 

namespace TechnicSolderPackager
{
    internal class SolderUnpacker
    {
        public static void Unpack(string[] args)
        {
            string releaseFileName = args[1];
            string[] existingModFolders = Directory.GetDirectories("." + Path.DirectorySeparatorChar).Select(s => s.Replace("." + Path.DirectorySeparatorChar, "").ToLower()).ToArray();
            List<string> modFilesThatNeedToBeHashed = new();


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
                    //if mod file doesnt exists
                    if (Directory.GetFiles(modName).Select(s => s.ToLower()).Contains(zipFile))
                    {
                        Console.WriteLine("   => Copying new mod version to folder...");
                        Console.WriteLine("    => Source: {0}", zipFile);
                        Console.WriteLine("    => Destination: {0}", Path.Combine(modName, zipFile.Split(Path.DirectorySeparatorChar).Last()));
                        File.Copy(zipFile, Path.Combine(modName, zipFile.Split(Path.DirectorySeparatorChar).Last()));
                        modFilesThatNeedToBeHashed.Add(zipFile.Split(Path.DirectorySeparatorChar).Last());
                    }
                    else
                    {
                        Console.WriteLine("   => Mod version already exists...");
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
                    modFilesThatNeedToBeHashed.Add(zipFile.Split(Path.DirectorySeparatorChar).Last());
                }
            }

            if (File.Exists("NewMods.csv"))
            {
                File.AppendAllText("NewMods.csv", string.Join($"{Environment.NewLine}", modFilesThatNeedToBeHashed));
            }
            else
            {
                File.WriteAllText("NewMods.csv", string.Join($"{Environment.NewLine}", modFilesThatNeedToBeHashed));
            }
        }


    }
}