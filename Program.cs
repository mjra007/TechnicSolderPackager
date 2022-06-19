using TechnicSolderPackager;

  
switch (args[0])
{
    //downloads mods listed in packwiz
    case "browserBasedDownloader":
        ModsDownloader.ExecuteBrowserBasedDownloader(args);
        break;
    //downloads mods listed in packwiz
    case "downloader":
        ModsDownloader.ExecuteDownloader(args);
        break;
    //packages mods in the solder deliverable format
    case "packager":
        SolderDeliverableCreator.Pack(args);
        break;
    //Extracts mods into their respective solder folders
    case "unpacker":
        SolderUnpacker.Unpack(args);
        break; 
    //tries to register mods on solder
    case "uploader":
        SolderHelper.ExecuteUploader(args);
        break; 
}










