using CatPrint.Code;
using CatPrint.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

namespace CatPrint.Pages
{
    /// <summary>
    /// Order.xaml 的交互逻辑
    /// </summary>
    public partial class Order : Page
    {
        public PagingQuery Paging { get; set; }
        public Order()
        {
            InitializeComponent();
            Paging = new PagingQuery { PageIndex = 1, PageSize = 20 };
            this.Loaded += (a, b) =>
            {
                LoadData();
            };
        }

        private async void LoadData()
        {
            try
            {
                var result = await Request.GetOrders(ApplicationObject.App.Business, Paging);
                list.ItemsSource = result;
                btnPre.IsEnabled = Paging.PageIndex > 1;
                btnNext.IsEnabled = Paging.PageIndex < Paging.PageCount;
            }
            catch (HttpRequestException)
            {
                MessageBox.Show("网络请求错误，请检查网络连接");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Back(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
        private void Pre(object sender, RoutedEventArgs e)
        {
            Paging.PageIndex--;
            LoadData();
        }
        private void Next(object sender, RoutedEventArgs e)
        {
            Paging.PageIndex++;
            LoadData();
        }
        private void Print(object sender, RoutedEventArgs e)
        {
            var parent = LogicalTreeHelper.GetParent(e.OriginalSource as Button) as StackPanel;
            var order = parent.DataContext as Model.Order;
            if (ApplicationObject.App.Printers.Count == 0)
            {
                MessageBox.Show("没有配置任何打印机");
                return;
            }
            ApplicationObject.Print(order);
        }
        private void Detail(object sender, RoutedEventArgs e)
        {
            var parent = LogicalTreeHelper.GetParent(e.OriginalSource as Button) as StackPanel;
            var order = parent.DataContext as Model.Order;
            var detail = new OrderDetail(order);
            detail.ShowDialog();
        }

    }
}
