using CatPrint.Enum;
using CatPrint.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// AddPrinter.xaml 的交互逻辑
    /// </summary>
    public partial class EditPrinter : Window
    {
        public Printer Printer { get; set; }
        public bool IsSave { get; set; }
        public EditPrinter(Printer printer = null)
        {
            if(printer == null)
            {
                Printer = new Printer { Id = Guid.NewGuid().ToString(), Format = 80, Port = 9100, Quantity = 1, State = 1, Type = 1, Mode = PrinterMode.Food, Foods = new ObservableCollection<int>() };
            }
            else
            {
                Printer = printer.Clone() as Printer;
            }

            InitializeComponent();
            this.DataContext = Printer;
            cbbType.ItemsSource = new ArrayList { new { Name = "前台", Value = 1 }, new { Name = "后厨", Value = 2 } };
            cbbState.ItemsSource = new ArrayList { new { Name = "正常", Value = 1 }, new { Name = "停用", Value = 2 } };
            cbbMode.ItemsSource = new ArrayList { new { Name = "一菜一打", Value = PrinterMode.Food }, new { Name = "一份一打", Value = PrinterMode.Share }, new { Name = "一单一打", Value = PrinterMode.Order } };
            cbbFormat.ItemsSource = new ArrayList { new { Name = "58mm", Value = 58 }, new { Name = "80mm", Value = 80 } };
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(Printer.Name) || string.IsNullOrEmpty(Printer.Name.Trim()))
            {
                MessageBox.Show("请输入打印机名称");
                return;
            }
            if (string.IsNullOrEmpty(Printer.IP) || string.IsNullOrEmpty(Printer.IP.Trim()))
            {
                MessageBox.Show("请输入IP地址");
                return;
            }
            else
            {
                var pattern = new Regex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
                if(!pattern.IsMatch(Printer.IP.Trim()))
                {
                    MessageBox.Show("请输入正确的IP地址");
                    return;
                }
            }
            if(Printer.Port == 0)
            {
                MessageBox.Show("请输入端口号");
                return;
            }
            IsSave = true;
            this.Close();
        }
        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
