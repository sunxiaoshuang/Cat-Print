﻿using CatPrint.Model;
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

namespace CatPrint.Pages
{
    /// <summary>
    /// Setting.xaml 的交互逻辑
    /// </summary>
    public partial class Setting : Page
    {
        public Setting()
        {
            InitializeComponent();
            printView.ItemsSource = ApplicationObject.App.Printers;
        }
        private void Back(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
        private void Add(object sender, RoutedEventArgs e)
        {
            var editWindow = new EditPrinter();
            editWindow.ShowDialog();
            if (!editWindow.IsSave) return;
            ApplicationObject.App.Printers.Add(editWindow.Printer);
            if (editWindow.Printer.State == 1)
            {
                editWindow.Printer.Open();      // 开始打印任务
            }
            ApplicationObject.App.Save();
        }
        private void Update(object sender, RoutedEventArgs e)
        {
            var parent = LogicalTreeHelper.GetParent(e.OriginalSource as Button) as StackPanel;
            var printer = parent.DataContext as Printer;
            var editWindow = new EditPrinter(printer);
            editWindow.ShowDialog();
            if (!editWindow.IsSave) return;
            var index = ApplicationObject.App.Printers.IndexOf(printer);
            ApplicationObject.App.Printers[index] = editWindow.Printer;
            if (editWindow.Printer.State == 1)
            {
                editWindow.Printer.Restart();      // 重新开始打印任务
            }
            else
            {
                editWindow.Printer.Close();        // 停止任务
            }
            ApplicationObject.App.Save();
        }
        private void Remove(object sender, RoutedEventArgs e)
        {
            if (!(printView.SelectedItem is Printer printer))
            {
                return;
            }

            var result = MessageBox.Show($"确定删除打印机：{printer.Name}吗？", "提示", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No) return;
            ApplicationObject.App.Printers.Remove(printer);
            // 移除打印机前，关闭现有任务
            printer.Close();
            ApplicationObject.App.Save();
        }
        private void SetMenu(object sender, RoutedEventArgs e)
        {
            var parent = LogicalTreeHelper.GetParent(e.OriginalSource as Button) as StackPanel;
            var printer = parent.DataContext as Printer;
            var menu = new MenuBinding(printer);
            menu.ShowDialog();
            if (!menu.IsSave) return;
            var index = ApplicationObject.App.Printers.IndexOf(printer);
            ApplicationObject.App.Printers[index] = menu.Printer;
            ApplicationObject.App.Save();
        }
    }
}
