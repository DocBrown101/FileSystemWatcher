namespace FileSystemWatcher.Model
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using Humanizer;

    public class FileOrFolderDataModel
    {
        public string FullPath { get; }

        public WatcherChangeTypes ChangeType { get; }

        public string Path { get; }

        public bool IsFolder { get; }

        public string FileName { get; }

        public string FileSize { get; }

        public FileOrFolderDataModel(string searchPath, string fullPath, WatcherChangeTypes changeType)
        {
            this.FullPath = fullPath;
            this.ChangeType = changeType;
            this.Path = this.GetPath(searchPath ?? throw new ArgumentNullException(nameof(searchPath)));

            if (Directory.Exists(this.FullPath))
            {
                this.IsFolder = true;
                this.FileName = this.LoadFolderData();
            }
            else
            {
                var fileData = this.LoadFileData();
                this.FileName = fileData.FileName;
                this.FileSize = fileData.FileSize;
            }
        }

        private string LoadFolderData()
        {
            var directoryInfo = new DirectoryInfo(this.FullPath);

            return $"Folder: {directoryInfo.Name}";
        }

        [SuppressMessage("Globalization", "CA1305:IFormatProvider", Justification = "unnecessary")]
        private (string FileName, string FileSize) LoadFileData()
        {
            try
            {
                var fileInfo = new FileInfo(this.FullPath);
                var fileSize = fileInfo.Exists ? fileInfo.Length.Bytes().Humanize("#.##") : "Deleted";

                return (fileInfo.Name, fileSize);
            }
            catch
            {
                return ("-", "-");
            }
        }

        private string GetPath(string searchPath)
        {
            var substringLength = searchPath.Length > 3 ? searchPath.Length : 2;

            return $"..{this.FullPath.Substring(substringLength)}";
        }
    }
}
