using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;

namespace PublisherApp
{
    public class Publisher
    {
        private readonly PathOptions _options;

        public Publisher(IOptions<PathOptions> options)
        {
            _options = options.Value;
        }

        public void Publish() 
        {
            var sourceFolder = _options.SourceFolder;
            var destFolders = _options.DestFolders;
            var filesFromSourceFolder = Directory.GetFiles(sourceFolder);
            var totalFiles = filesFromSourceFolder.Length;
            var totalDestFolders = destFolders.Length;

            Logger.LogInfo($"Total of files to be copied: {totalFiles}");
            Logger.LogInfo($"Total of destination folders: {totalDestFolders}");

            var counterFolders = 0;
            destFolders
                .ToList()
                .ForEach(destFolder => {
                    Logger.LogInfo($"Folders: {++counterFolders} of {totalDestFolders}");
                    Logger.LogInfo($"Copying to folder: {destFolder}");

                        if (!Directory.Exists(destFolder)) 
                        {
                            Logger.LogInfo($"Creating folder: {destFolder}");
                            Directory.CreateDirectory(destFolder);
                        }
                        else 
                        {
                            Logger.LogInfo($"Deleting files from folder: {destFolder}");
                            Directory.Delete(destFolder, true);
                            Directory.CreateDirectory(destFolder);
                        }

                    var counterFiles = 0;
                    filesFromSourceFolder
                        .ToList()
                        .ForEach(sourceFile => {
                            Logger.LogInfo($"Files: {++counterFiles} of {totalFiles}");
                            var fileName = Path.GetFileName(sourceFile);
                            Logger.LogInfo($"=> Copying file: {fileName}");
                            var destFileName = Path.Combine(destFolder, fileName);
                            Logger.LogInfo($"=> Copying {fileName} from {sourceFile} to {destFileName}");
                            File.Copy(sourceFile, destFileName, true);
                            Logger.LogInfo("Done!");
                        });
                });
        }
    }
}