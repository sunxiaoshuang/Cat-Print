﻿using CatPrint.Enum;
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
    public class OrderStatusToStringValueConverter : IValueConverter
    {
        private static Dictionary<OrderStatus, string> dic;
        static OrderStatusToStringValueConverter()
        {
            dic = new Dictionary<OrderStatus, string>();
            foreach (OrderStatus status in System.Enum.GetValues(typeof(OrderStatus)))
            {
                dic.Add(status, status.GetDescription());
            }
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (OrderStatus)value;
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
