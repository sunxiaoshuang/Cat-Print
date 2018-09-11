using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CatPrint.Pages
{
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : Page
    {
        public Main()
        {
            InitializeComponent();
            sp1.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler((a, b) => {
                MessageBox.Show("dd");
            }));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var ele = (Button)sender;
            var flag = ele.Tag.ToString();
            NavigationService.Navigate(new Uri($@"/Pages/{flag}.xaml", UriKind.Relative));
        }
    }
}
