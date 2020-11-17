using System.Windows;
using WPFLocalizeExtension.Engine;

namespace FileSystemWatcher
{
    public partial class App : Application
    {
        public App()
        {
            LocalizeDictionary.Instance.Culture = new System.Globalization.CultureInfo("en");
            //LocalizeDictionary.Instance.Culture = System.Threading.Thread.CurrentThread.CurrentCulture;
        }
    }
}
