using System.Windows;
using FileSystemWatcher.ViewModel;
using Jot;

namespace FileSystemWatcher.View
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            this.InitializeComponent();

            GetTracker().Track(this);
        }

        private static Tracker GetTracker()
        {
            var tracker = new Tracker();
            tracker.Configure<Window>()
                   .Id(w => w.Name, SystemParameters.VirtualScreenWidth)
                   .Properties(w => new { w.Top, w.Width, w.Height, w.Left, w.WindowState })
                   .PersistOn(nameof(Window.Closing))
                   .StopTrackingOn(nameof(Window.Closing));

            return tracker;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ((MainViewModel)this.DataContext).Stop();
        }
    }
}
