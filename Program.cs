using System.IO.Compression;
using System.Net;
using Tomlyn;
using Tomlyn.Model;

args[0] = "packager";
args[1] = "3.0";
args[2] = "EnchantedOasis";
if (args[0].Equals("downloader") && args[1] != null)
{
    Console.WriteLine("Curseforge downloader!");
    string apiKey = args[1];
    string[] modInfoFiles = Directory.GetFiles("mods"); 
    var webClient = new WebClient();  
    foreach (string modInfo in modInfoFiles)
    {
        Console.WriteLine(modInfo);
        string slug = modInfo.Split(Path.PathSeparator).Last().Replace(".pw.toml","");
        var model = Toml.ToModel(new StreamReader(modInfo).ReadToEnd());
        TomlTable curseforgeSection = (TomlTable)((TomlTable)model["update"])["curseforge"];
        long projectID = (long)curseforgeSection["project-id"];
        long fileID = (long)curseforgeSection["file-id"];
        string fileIDFirst = new(fileID.ToString().Take(4).ToArray()) ;
        string fileIDLast = new(fileID.ToString()
                                  .Skip(4)
                                  .Take(3).ToArray());
        string filename = (string)model["filename"];
        webClient.DownloadFile(@$"https://edge.forgecdn.net/files/{fileIDFirst}/{fileIDLast}/{filename}?api-key={apiKey}", filename); 
    } 
}
else if (args[0].Equals("packager") && args[1] != null && args[2] != null) {
    Console.WriteLine("TechnicSolder Packager!");
    string packname = args[2];
    string packVersion = args[1];
    string currentPath = Environment.CurrentDirectory;
    string[] mods = Directory.GetFiles(currentPath).Where(s => s.Contains("jar")).ToArray();

    if (!Directory.Exists("builds"))
    {
        Directory.CreateDirectory("builds");
    }
    else
    {
        Directory.Delete("builds", true);
        Directory.CreateDirectory("builds");
    }

    foreach (string mod in mods)
    {
        string fileName = mod.Split("\\").Last();
        string fileNameNoJar = fileName.Replace(".jar", "");
        string folderModName = fileName.Split("-")[0];
        Console.WriteLine(mod);
        Directory.CreateDirectory( Path.Combine("builds", folderModName, "mods"));
        File.Copy(mod, Path.Combine("builds", folderModName, "mods", fileName));
        ZipFile.CreateFromDirectory( Path.Combine("builds" , folderModName), Path.Combine( "builds" , fileNameNoJar + ".zip"));
        Directory.Delete(Path.Combine("builds" , folderModName), true);
    }
    
    ZipFile.CreateFromDirectory($"config", Path.Combine("builds", $"config-{ packVersion}.zip"), CompressionLevel.NoCompression, includeBaseDirectory: true);
    ZipFile.CreateFromDirectory($"animation", Path.Combine("builds", $"animation-{ packVersion}.zip"), CompressionLevel.NoCompression, includeBaseDirectory: true);
    ZipFile.CreateFromDirectory($"customnpcs", Path.Combine("builds", $"customnpcs-{ packVersion}.zip"), CompressionLevel.NoCompression, includeBaseDirectory: true);
    ZipFile.CreateFromDirectory($"resources", Path.Combine("builds", $"resources-{ packVersion}.zip"), CompressionLevel.NoCompression, includeBaseDirectory: true);
    Console.WriteLine($"{packname}-{packVersion}.zip ");
    ZipFile.CreateFromDirectory("builds", $"{packname}-{packVersion}.zip");
}


