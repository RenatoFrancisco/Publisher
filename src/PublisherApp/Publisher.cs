using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;
using System.IO.Compression;

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
            if (!Directory.Exists(sourceFolder))
            {
                Logger.LogError($"Source folder {sourceFolder} does not found!");
                return;
            }

            var destFolders = _options.DestFolders;
            var filesFromSourceFolder = Directory.GetFiles(sourceFolder);
            var totalFiles = filesFromSourceFolder.Length;
            var totalDestFolders = destFolders.Length;

            Logger.LogInfo($"Total of files to be copied: {totalFiles}");
            Logger.LogInfo($"Total of destination folders: {totalDestFolders}");

            var counterFolders = 0;
            destFolders
                .ToList()
                .ForEach(destFolder =>
                {
                    Logger.LogInfo($"Folders: {++counterFolders} of {totalDestFolders}");
                    PrepareDestFolder(destFolder);

                    var counterFiles = 0;
                    filesFromSourceFolder
                        .ToList()
                        .ForEach(sourceFile =>
                        {
                            Logger.LogInfo($"Files: {++counterFiles} of {totalFiles}");
                            CopyFiles(sourceFile, destFolder);
                        });
                });
        }

        private void PrepareDestFolder(string destFolder)
        {
            Logger.LogInfo($"Folder: {destFolder}");
            if (!Directory.Exists(destFolder))
            {
                Logger.LogInfo($"Creating folder: {destFolder}");
                Directory.CreateDirectory(destFolder);
            }
            else
            {
                CreateBackup(destFolder);
                DeleteFiles(destFolder);
            }
        }

        private void CreateBackup(string folderToCompress)
        {
            var outFolder = Path.GetFullPath(Path.Combine(folderToCompress, @"..\"));
            var lastFolder = folderToCompress.Split(Path.DirectorySeparatorChar).Last();
            var outFileZip = Path.Combine(outFolder, $"{lastFolder}_Backup.zip");

            Logger.LogInfo($"Creating backup of folder: {folderToCompress}");
            File.Delete(outFileZip);
            ZipFile.CreateFromDirectory(folderToCompress, outFileZip);
        }

        private void DeleteFiles(string destFolder)
        {
            var deleteFiles = _options.DeleteFiles;
            if (!deleteFiles) return;

            Logger.LogInfo($"Deleting files from folder: {destFolder}");
            Directory.Delete(destFolder, true);
            Directory.CreateDirectory(destFolder);
        }

        private void CopyFiles(string sourceFile, string destFolder)
        {
            var fileName = Path.GetFileName(sourceFile);
            Logger.LogInfo($"=> Copying file: {fileName}");
            var destFileName = Path.Combine(destFolder, fileName);
            Logger.LogInfo($"=> Copying {fileName} from {sourceFile} to {destFileName}");
            File.Copy(sourceFile, destFileName, true);
            Logger.LogSuccess("Done!");
        }
    }
}