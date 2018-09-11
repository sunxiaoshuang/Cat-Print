using CatPrint.Code;
using CatPrint.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// MenuBinding.xaml 的交互逻辑
    /// </summary>
    public partial class MenuBinding : Window
    {
        public Printer Printer { get; set; }
        public bool IsSave { get; set; }
        private IEnumerable<ProductType> TypeList{get;set;}
        public MenuBinding(Printer printer)
        {
            InitializeComponent();
            Printer = printer.Clone() as Printer;
            LoadData();
        }
        private async void LoadData()
        {
            TypeList = await Request.GetTypes(ApplicationObject.App.Business);
            foreach (var type in TypeList)
            {
                foreach (var food in Printer.Foods)
                {
                    var product = type.Products.FirstOrDefault(a => a.ID == food);
                    if(product != null)
                    {
                        product.Checked = true;
                    }
                }
            }
            lbTypes.ItemsSource = TypeList;
        }
        private void Save(object sender, RoutedEventArgs e)
        {
            var foodIds = new ObservableCollection<int>();
            foreach (var type in TypeList)
            {
                foreach (var item in type.Products)
                {
                    if (item.Checked)
                    {
                        foodIds.Add(item.ID);
                    }
                }
            }
            Printer.Foods = foodIds;
            IsSave = true;
            this.Close();
        }
        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
