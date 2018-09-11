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
    public class SharePrint : BackstagePrint
    {
        public SharePrint(Order order, Printer printer, Socket socket) : base(order, printer, socket)
        {
        }

        public override void Print()
        {
            if (Products.Count == 0) return;
            foreach (var product in Products)
            {
                for (int i = 0; i < product.Quantity; i++)
                {
                    BeforePrint();
                    var name = product.Name;
                    if(!string.IsNullOrEmpty(product.Description))
                    {
                        name += $"({product.Description})";
                    }
                    BufferList.Add(PrinterCmdUtils.FontSizeSetBig(4));
                    BufferList.Add(PrinterCmdUtils.AlignLeft());
                    BufferList.Add(PrinterCmdUtils.PrintLineLeftRight(name, "*1", Printer.FormatLen, 4));
                    BufferList.Add(PrinterCmdUtils.NextLine());
                    AfterPrint();
                    Send();
                }
            }
        }
        protected override void Printing()
        {
        }
    }
}
