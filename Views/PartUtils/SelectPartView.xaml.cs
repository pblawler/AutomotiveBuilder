using AutomotiveBuilder.DAL;
using AutomotiveBuilder.ViewModels.Parts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AutomotiveBuilder.Views.PartUtils
{
    /// <summary>
    /// Interaction logic for SelectPartView.xaml
    /// </summary>
    public partial class SelectPartView : Window
    {
        private Part _SelectedPart = null;

        public Part SelectedPart
        {
            get { return _SelectedPart; }
            set { _SelectedPart = value; }
        }

        public SelectPartView()
        {
            InitializeComponent();
        }


        private void Label_MouseUp(object sender, MouseButtonEventArgs e)
        {
            string url = PartURL.Text;

            if ((url == null) || (url == "")) return;
            // For .NET Core, .NET 5+, .NET 6+, etc.
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PartSelectVM vm = (PartSelectVM)this.DataContext;
            SelectedPart = vm.SelectedPart;
            
            this.Hide();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
