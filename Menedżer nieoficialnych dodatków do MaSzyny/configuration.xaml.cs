using Microsoft.Win32;
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
using System.IO;

namespace Menedżer_nieoficialnych_dodatków_do_MaSzyny
{
    /// <summary>
    /// Logika interakcji dla klasy configuration.xaml
    /// </summary>
    public partial class configuration : Window
    {
        public configuration()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = "eu07";
            dlg.DefaultExt = ".exe";
            dlg.Filter = "Aplikacje (.exe)|*.exe";
            dlg.Title = "Wybierz plik wykonywalny symulatora";
            if (dlg.ShowDialog() == true)
            {
                string simDir = System.IO.Path.GetDirectoryName(dlg.FileName);
                simDirTxt.Text = simDir;
                Globals.simDir = simDir;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
