using CatPrint.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CatPrint.Converter
{
    public class PrintTypeTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var printer = (Printer)value;
            if(printer.Type == 1)
            {
                return "前台打印机";
            }
            else if(printer.Type == 2)
            {
                return "后台打印机";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
