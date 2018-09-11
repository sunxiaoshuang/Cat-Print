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

namespace CatPrint.Pages
{
    /// <summary>
    /// OrderDetail.xaml 的交互逻辑
    /// </summary>
    public partial class OrderDetail : Window
    {
        public Model.Order Order { get; set; }
        public OrderDetail(Model.Order order)
        {
            this.Order = order;
            InitializeComponent();
            this.DataContext = order;
            LoadData();
        }

        private void LoadData()
        {
            address.Content = Order.ReceiverName + "-" + Order.Phone + "-" + Order.ReceiverAddress;
            fullReduce.Content = Order.SaleFullReduce == null ? "" : ("-￥" + Order.SaleFullReduce.ReduceMoney);
            coupon.Content = Order.SaleCouponUser == null ? "" : ("-￥" + Order.SaleCouponUser.Value);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
