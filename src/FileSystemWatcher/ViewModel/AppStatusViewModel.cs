namespace FileSystemWatcher.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Windows.Shell;

    public class AppStatusViewModel : Observable
    {
        public ApplicationState ApplicationState { get; }

        public TaskbarItemProgressState TaskbarItemProgressState { get; }

        public bool IsIndeterminate { get; }

        public string StartStopCommandText { get; }

        public AppStatusViewModel(ApplicationState applicationState)
        {
            this.ApplicationState = applicationState;

            switch (this.ApplicationState)
            {
                case ApplicationState.Nothing:
                case ApplicationState.Stop:
                    {
                        this.TaskbarItemProgressState = TaskbarItemProgressState.None;
                        this.IsIndeterminate = false;
                        this.StartStopCommandText = "Start";
                    }

                    break;
                case ApplicationState.Running:
                    {
                        this.TaskbarItemProgressState = TaskbarItemProgressState.Indeterminate;
                        this.IsIndeterminate = true;
                        this.StartStopCommandText = "Stop";
                    }

                    break;
                case ApplicationState.Aborting:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(applicationState));
            }
        }
    }
}
