using AutomotiveBuilder.Statics;
using System.Configuration;
using System.Data;
using System.Windows;

namespace AutomotiveBuilder
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public App()
        {
            StaticMethods.CreateFolders();
        }
    }

}
