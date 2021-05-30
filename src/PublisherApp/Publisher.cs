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
                    CopyFiles(sourceFolder, destFolder, _options.CopySubfolders);
                });

            Logger.LogInfo("Done!");
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

            Logger.LogWarn($"Deleting files from folder: {destFolder}");
            Directory.Delete(destFolder, true);
            Directory.CreateDirectory(destFolder);
        }

        private void CopyFiles(string sourceFolder, string destFolder, bool copySubfolders)
        {
            var dir = new DirectoryInfo(sourceFolder);
            var subfolders = dir.GetDirectories();
            Logger.LogInfo($"=> Total of sub folders: {subfolders.Count()}");

            Directory.CreateDirectory(destFolder);
            
            var files = dir.GetFiles();
            Logger.LogInfo($"==> Total of files: {files.Count()}");
            foreach (var file in files)
            {
                Logger.LogInfo($"===> Copying file: {file.Name} from {sourceFolder} to {destFolder}");
                var tempPath = Path.Combine(destFolder, file.Name);
                file.CopyTo(tempPath, true);
            }
            
            if (copySubfolders)
            {
                foreach (var subFolder in subfolders)
                {
                    Logger.LogInfo($"====> Copying file(s) from subfolder {subFolder}");
                    var tempPath = Path.Combine(destFolder, subFolder.Name);
                    CopyFiles(subFolder.FullName, tempPath, copySubfolders);
                }
            }
        }
    }
}