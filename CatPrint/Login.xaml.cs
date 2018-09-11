using CatPrint.Code;
using CatPrint.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CatPrint
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
            this.KeyDown += (a, b) =>
            {
                if (b.Key != Key.Enter) return;
                Request();
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Request();
        }
        private async void Request()
        {
            var name = username.Text.Trim();
            var password = pwd.Password.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("请输入用户名");
                return;
            }
            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("请输入密码");
                return;
            }
            var data = await Code.Request.Login(name, password);
            if (data.Success)
            {
                var business = JsonConvert.DeserializeObject<Business>(data.Data.ToString());
                ApplicationObject.App.Business = business;
                ApplicationObject.App.Init();
                try
                {
                    var window = new MainWindow();
                    if(!window.InitSuccess)
                    {
                        window.Close();
                        return;
                    }
                    Application.Current.MainWindow = window;
                    Application.Current.MainWindow.Show();
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("系统错误，" + ex.Message);
                }
            }
            else
            {
                MessageBox.Show(data.Msg);
            }
        }
    }
}
