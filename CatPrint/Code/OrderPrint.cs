using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CatPrint.Model;
using WPF.Test.Practise2.Code;

namespace CatPrint.Code
{
    public class OrderPrint : BackstagePrint
    {
        public OrderPrint(Order order, Printer printer, Socket socket) : base(order, printer, socket)
        {
        }

        protected override void Printing()
        {
            BufferList.Add(PrinterCmdUtils.FontSizeSetBig(4));
            BufferList.Add(PrinterCmdUtils.AlignLeft());
            foreach (var product in Products)
            {
                var name = product.Name;
                if (!string.IsNullOrEmpty(product.Description))
                {
                    name += $"({product.Description})";
                }
                BufferList.Add(PrinterCmdUtils.PrintLineLeftRight(name, "*" + double.Parse(product.Quantity + "").ToString(), Printer.FormatLen, 4));
                BufferList.Add(PrinterCmdUtils.NextLine());
            }
        }
    }
}
