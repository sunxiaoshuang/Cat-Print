using CatPrint.Enum;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using CatPrint.Code;

namespace CatPrint.Converter
{
    public class PrintModelToStringTypeConverter : IValueConverter
    {
        private static Dictionary<PrinterMode, string> dic;
        static PrintModelToStringTypeConverter()
        {
            dic = new Dictionary<PrinterMode, string>();
            foreach (PrinterMode status in System.Enum.GetValues(typeof(PrinterMode)))
            {
                dic.Add(status, status.GetDescription());
            }
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (PrinterMode)value;
            if (dic.ContainsKey(status))
            {
                return dic[status];
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
