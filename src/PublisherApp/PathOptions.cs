namespace PublisherApp
{
    public class PathOptions
    {
        public const string Path = "Path";

        public string SourceFolder { get; set; }
        public string[] DestFolders { get; set; }
        public bool DeleteFiles { get; set; }
        public bool CopySubfolders { get; set; }
    }
}