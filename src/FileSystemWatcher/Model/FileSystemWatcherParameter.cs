namespace FileSystemWatcher.Model
{
    public class FileSystemWatcherParameter
    {
        public string FileSystemPath { get; set; }

        public bool ListFiles { get; set; }

        public bool ListDirectories { get; set; }

        public bool ListOnlyUniqueFiles { get; set; }

        public bool IncludeSubdirectories { get; set; }
    }
}
