using System.IO.Compression;
 
Console.WriteLine("TechnicSolder Packager!");

string currentPath = Environment.CurrentDirectory; 
string[] mods = Directory.GetFiles(currentPath).Where(s => s.Contains("jar")).ToArray();

if (!Directory.Exists(currentPath+"\\builds"))
{
    Directory.CreateDirectory(currentPath+"\\builds");
}
else
{
    Directory.Delete(currentPath+"\\builds", true);
    Directory.CreateDirectory(currentPath+"\\builds"); 
}

foreach (string mod in mods)
{
    string fileName = mod.Split(@"\").Last();
    string fileNameNoJar = fileName.Replace(".jar", "");
    string folderModName = fileName.Split("-")[0];
    Console.WriteLine(mod);
    Directory.CreateDirectory(  currentPath+"\\builds" + "\\" + folderModName + "\\mods");
    File.Copy(mod, currentPath+"\\builds" + "\\" + folderModName + "\\mods"+"\\"+ fileName);
    ZipFile.CreateFromDirectory(currentPath + "\\builds" + "\\" + folderModName , currentPath + "\\builds" + "\\" +fileNameNoJar+".zip");
    Directory.Delete(currentPath + "\\builds" + "\\" + folderModName);
}