namespace FileSystemWatcher.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;
    using FileSystemWatcher.Model;

    [SuppressMessage("Design", "CA1001", Justification = "Never released")]
    public class MainViewModel : Observable
    {
        private readonly FileSystemWatcher fileSystemWatcher;

        public FileSystemWatcherParameter FileSystemWatcherParameter { get; private set; }

        public ObservableCollection<FileOrFolderDataModel> FileDataModels { get; private set; }

        public AppStatusViewModel AppStatus { get; private set; }

        public RelayCommand StartStopCommand { get; private set; }

        public RelayCommand ResetCommand { get; private set; }

        public RelayCommand SetWindowsStoreAppDataPathCommand { get; private set; }

        public RelayCommand<FileOrFolderDataModel> OpenFileInExplorerCommand { get; private set; }

        public MainViewModel()
        {
            this.AppStatus = new AppStatusViewModel(ApplicationState.Nothing);
            this.FileSystemWatcherParameter = GetInitialFileSystemWatcherParameter();
            this.FileDataModels = new ObservableCollection<FileOrFolderDataModel>();

            this.fileSystemWatcher = new FileSystemWatcher();
            this.fileSystemWatcher.Changed += (sender, e) => this.ShowFileData(e);
            this.fileSystemWatcher.Created += (sender, e) => this.ShowFileData(e);
            this.fileSystemWatcher.Deleted += (sender, e) => this.ShowFileData(e);
            this.fileSystemWatcher.Renamed += (sender, e) => this.ShowFileData(e);

            this.StartStopCommand = new RelayCommand(this.StartStop);
            this.ResetCommand = new RelayCommand(this.Reset);
            this.SetWindowsStoreAppDataPathCommand = new RelayCommand(this.SetWindowsStoreAppDataPath);
            this.OpenFileInExplorerCommand = new RelayCommand<FileOrFolderDataModel>(this.OpenFileInExplorer);
        }

        public void Stop()
        {
            this.fileSystemWatcher.EnableRaisingEvents = false;

            this.AppStatus = new AppStatusViewModel(ApplicationState.Stop);
        }

        private static FileSystemWatcherParameter GetInitialFileSystemWatcherParameter()
        {
            var roamingApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            var fileSystemWatcherParameter = new FileSystemWatcherParameter
            {
                FileSystemPath = roamingApplicationData.Replace("Roaming", string.Empty),
                ListOnlyUniqueFiles = true,
                IncludeSubdirectories = true
            };
            return fileSystemWatcherParameter;
        }

        private void SetWindowsStoreAppDataPath()
        {
            var roamingApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            this.FileSystemWatcherParameter.FileSystemPath = Path.Combine(roamingApplicationData.Replace("Roaming", string.Empty), @"Local\Packages");
            this.OnPropertyChanged(nameof(this.FileSystemWatcherParameter));
        }

        private void StartStop()
        {
            switch (this.AppStatus.ApplicationState)
            {
                case ApplicationState.Nothing:
                case ApplicationState.Stop:
                    this.Start();
                    break;
                case ApplicationState.Running:
                    this.Stop();
                    break;
            }

            this.OnPropertyChanged(nameof(this.AppStatus));
        }

        private void Start()
        {
            if (!Directory.Exists(this.FileSystemWatcherParameter.FileSystemPath.Trim()))
            {
                return;
            }

            this.AppStatus = new AppStatusViewModel(ApplicationState.Running);

            this.StartFileSystemWatcher();
        }

        private void StartFileSystemWatcher()
        {
            this.Reset();
            this.fileSystemWatcher.IncludeSubdirectories = this.FileSystemWatcherParameter.IncludeSubdirectories;
            this.fileSystemWatcher.Path = this.FileSystemWatcherParameter.FileSystemPath.Trim();
            this.fileSystemWatcher.Filter = "*";

            this.fileSystemWatcher.EnableRaisingEvents = true;
        }

        private void Reset()
        {
            this.FileDataModels.Clear();
        }

        private void ShowFileData(FileSystemEventArgs eventArgs)
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(
                    () =>
                        {
                            var dataModel = new FileOrFolderDataModel(this.FileSystemWatcherParameter.FileSystemPath, eventArgs.FullPath, eventArgs.ChangeType);
                            var isUniqueFile = true;

                            if (this.FileSystemWatcherParameter.ListOnlyUniqueFiles)
                            {

                                if (this.FileDataModels.AsParallel()
                                        .Any(item => item.FullPath == eventArgs.FullPath && item.ChangeType == eventArgs.ChangeType))
                                {
                                    isUniqueFile = false;
                                }
                            }

                            if (isUniqueFile)
                            {
                                this.FileDataModels.Add(dataModel);
                            }
                        }));
        }

        private void OpenFileInExplorer(FileOrFolderDataModel fileDataModel)
        {
            if (fileDataModel != null)
            {
                var fileInfo = new FileInfo(fileDataModel.FullPath);
                var args = $"/e, /select, \"{(fileInfo.Exists ? fileInfo.FullName : fileInfo.DirectoryName)}\"";

                Process.Start("explorer", args);
            }
        }
    }
}
