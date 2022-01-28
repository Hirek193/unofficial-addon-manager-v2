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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using IniParser;
using IniParser.Model;
using System.Reflection;
using Newtonsoft.Json;
using RestSharp;
using System.ComponentModel;
using System.Net.Http;
using System.Net;
using System.Diagnostics;

namespace Menedżer_nieoficialnych_dodatków_do_MaSzyny
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FileIniDataParser parser;
        IniData config_ini;
        List<Addon> addons;
        int key;
        public MainWindow()
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            brandLbl.Content = "Menedżer nieoficialnych dodatków do MaSzyny " + Globals.version;
            selectCategoryLbl.Visibility = Visibility.Visible;
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            configuration configWin = new configuration();
            configWin.ShowDialog();
        }

        private void aboutProject_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            HideAll();
            string selectedCategory = ((Border)((Button)sender).Parent).Name;
            addonScroller.Children.Clear();
            selectCategoryLbl.Visibility = Visibility.Visible;
            selectCategoryLbl.Content = "Ładowanie...";
            switch (selectedCategory)
            {
                case "electricLocos":
                    Task.Run(() => getServerData(1));
                    break;
                case "dieselLocos":
                    Task.Run(() => getServerData(2));
                    break;
                default:
                    throw new NotImplementedException();
                    break;
            }
        }

        private void loadThumb(Image thumbObj, string url)
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead("http://www.stapox.cal24.pl/" + url);
            BitmapFrame bitmapFrame = BitmapFrame.Create(stream,
              BitmapCreateOptions.None,
              BitmapCacheOption.OnLoad);
            Dispatcher.Invoke(() =>
            {
                thumbObj.Source = bitmapFrame;
            });
        }

        private void getServerData(int key)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    selectCategoryLbl.Content = "Ładowanie...";
                });
                var tcs = new TaskCompletionSource<int>();
                string uri = "http://" + Globals.api + "/addons?key=" + key;
                var client = new RestClient(uri);
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    addons = JsonConvert.DeserializeObject<List<Addon>>(response.Content);
                    for (int i = 0; i < addons.Count; i++)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            // Border
                            Border border = new Border();
                            border.Name = "addon" + i;
                            border.Height = 225;
                            border.Margin = new Thickness(0, 0, 10, 10);
                            border.Background = new SolidColorBrush(Color.FromRgb(0x0, 0x0, 0x2f));
                            border.CornerRadius = new CornerRadius(10);
                            // Grid for elements
                            Grid borderGrid = new Grid();

                            // Image
                            Image mini = new Image();
                            mini.Name = "thumb" + i;
                            //mini.Width = 450;
                            //mini.Height = 260;
                            mini.HorizontalAlignment = HorizontalAlignment.Left;
                            mini.VerticalAlignment = VerticalAlignment.Top;
                            mini.Stretch = Stretch.Uniform;
                            mini.Margin = new Thickness(10);
                            loadThumb(mini, addons[i].screen1);

                            borderGrid.Children.Add(mini);

                            // Title label
                            Label titleLabel = new Label();
                            titleLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                            string displayName = addons[i].name;
                            if (displayName.Length > 35)
                            {
                                displayName = addons[i].name.Remove(35);
                                displayName += "...";
                            }
                            titleLabel.Content = displayName;
                            titleLabel.ToolTip = addons[i].name;
                            titleLabel.Margin = new Thickness(460, 10, 10, 10);
                            titleLabel.Width = 700;
                            titleLabel.FontSize = 19;
                            titleLabel.HorizontalAlignment = HorizontalAlignment.Left;
                            titleLabel.VerticalAlignment = VerticalAlignment.Top;
                            borderGrid.Children.Add(titleLabel);

                            //Description label
                            TextBlock descriptionLabel = new TextBlock();
                            descriptionLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                            descriptionLabel.Text = addons[i].shortDesc;
                            descriptionLabel.Margin = new Thickness(460, 50, 10, 10);
                            //descriptionLabel.Width = 100;
                            descriptionLabel.Height = 150;
                            descriptionLabel.FontSize = 16;
                            descriptionLabel.TextWrapping = TextWrapping.Wrap;
                            descriptionLabel.HorizontalAlignment = HorizontalAlignment.Left;
                            descriptionLabel.VerticalAlignment = VerticalAlignment.Top;
                            borderGrid.Children.Add(descriptionLabel);

                            // Show more button
                            // Button
                            Button moreBtn = new Button();
                            moreBtn.Name = "moreInfo" + i + "Btn";
                            moreBtn.Content = "Więcej";
                            moreBtn.Click += MoreBtn_Click;
                            Style buttonInv = Application.Current.FindResource("buttonInv") as Style;
                            moreBtn.Style = buttonInv;
                            // Border
                            Border moreBorder = new Border();
                            moreBorder.Margin = new Thickness(10, 10, 120, 10);
                            moreBorder.HorizontalAlignment = HorizontalAlignment.Right;
                            moreBorder.VerticalAlignment = VerticalAlignment.Bottom;
                            moreBorder.Width = 100;
                            moreBorder.Height = 30;
                            Style moreBorderStyle = Application.Current.FindResource("moreAddonInfoBorder") as Style;
                            moreBorder.Style = moreBorderStyle;
                            moreBorder.Child = moreBtn;
                            borderGrid.Children.Add(moreBorder);

                            // Install button
                            // Button
                            Button installBtn = new Button();
                            installBtn.Name = "install" + i + "Btn";
                            installBtn.Content = "Zainstaluj";
                            installBtn.Click += installBtn_CLick;
                            installBtn.Style = buttonInv;
                            // Border
                            Border installBorder = new Border();
                            installBorder.Margin = new Thickness(10, 10, 10, 10);
                            installBorder.HorizontalAlignment = HorizontalAlignment.Right;
                            installBorder.VerticalAlignment = VerticalAlignment.Bottom;
                            installBorder.Width = 100;
                            installBorder.Height = 30;
                            Style installBorderStyle = Application.Current.FindResource("installBorder") as Style;
                            installBorder.Style = installBorderStyle;
                            installBorder.Child = installBtn;
                            borderGrid.Children.Add(installBorder);




                            border.Child = borderGrid;                            
                            addonScroller.Children.Add(border);
                        });
                    }
                    Dispatcher.Invoke(() =>
                    {
                        selectCategoryLbl.Visibility = Visibility.Hidden;
                        addonCanvas.Visibility = Visibility.Visible;
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        selectCategoryLbl.Content = "Niepowodzenie ładowania";
                        MessageBox.Show("Wystąpił błąd pobierania danych " + response.StatusCode.ToString(), "Błąd pobierania danych",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Wystąpił błąd pobierania i prezentacji danych:\n" + e.ToString(), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                Dispatcher.Invoke(() =>
                {
                    selectCategoryLbl.Content = "Niepowodzenie ładowania";
                });
            }
        }

        private void installBtn_CLick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void MoreBtn_Click(object sender, RoutedEventArgs e)
        {
            string btnName = ((Button)sender).Name;

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            // Simple process start to open webpage of facebook fp
            System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = "https://www.facebook.com/Nieoficjalne-dodatki-do-Maszyny-551258932040537",
                UseShellExecute = true
            });
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            // Simple process start to open github issues
            System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/Hirek193/unofficial-addon-manager-v2/issues",
                UseShellExecute = true
            });
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            HideAll();
            selectCategoryLbl.Visibility = Visibility.Hidden;
            contactCanvas.Visibility = Visibility.Visible;

        }
        private void HideAll()
        {
            contactCanvas.Visibility = Visibility.Hidden;
            addonCanvas.Visibility = Visibility.Hidden;
            selectCategoryLbl.Visibility = Visibility.Visible;
        }
    }

    public class Addon
    {
        [JsonProperty("id")]
        public int id { get; set; }
        [JsonProperty("nazwa")]
        public string name { get; set; }
        [JsonProperty("type")]
        public int type { set; get; }
        [JsonProperty("zdjecie1")]
        public string screen1 { set; get; }
        [JsonProperty("zdjecie2")]
        public string screen2 { set; get; }
        [JsonProperty("zdjecie3")]
        public string screen3 { set; get; }
        [JsonProperty("kompatybilnosc")]
        public string compatibility { set; get; }
        [JsonProperty("autorzy")]
        public string authors { get; set; }
        [JsonProperty("opis_krotki")]
        public string shortDesc { set; get; }
        [JsonProperty("opis_dlugi")]
        public string longDesc { get; set; }
        [JsonProperty("r_i_high")]
        public string highScript { get; set; }
        [JsonProperty("r_i_low")]
        public string lowScript { get; set; }
        [JsonProperty("data_wydania")]
        public string publishDate { set; get; }

    }
}
