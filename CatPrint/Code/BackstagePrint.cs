﻿using CatPrint.Enum;
using CatPrint.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CatPrint.Code
{
    /// <summary>
    /// 后厨打印
    /// </summary>
    public abstract class BackstagePrint
    {
        public Order Order { get; set; }
        public Printer Printer { get; set; }
        private Socket _socket;
        public List<byte[]> BufferList { get; set; }
        public List<OrderProduct> Products { get; set; }

        public BackstagePrint(Order order, Printer printer, Socket socket)
        {
            this.Order = order;
            this.Printer = printer;
            _socket = socket;
            Products = Order.Products.Where(a => a.Feature == ProductFeature.SetMeal || Printer.Foods.Contains(a.ProductId.Value)).ToList();
        }
        public virtual void Print()
        {
            if (Products.Count == 0) return;
            BeforePrint();
            Printing();
            AfterPrint();
            Send();
        }
        public void Send()
        {
            if (BufferList == null || BufferList.Count == 0) return;
            BufferList.ForEach(a =>
            {
                _socket.Send(a);
            });
        }
        protected virtual void BeforePrint()
        {
            BufferList = new List<byte[]>();
            // 打印当日序号
            BufferList.Add(PrinterCmdUtils.AlignCenter());
            BufferList.Add(PrinterCmdUtils.FontSizeSetBig(2));
            BufferList.Add(Encoding.GetEncoding("gbk").GetBytes("#" + Order.Identifier + " 简单猫"));
            BufferList.Add(PrinterCmdUtils.NextLine());
            BufferList.Add(PrinterCmdUtils.NextLine());
            BufferList.Add(PrinterCmdUtils.AlignLeft());
            // 备注
            if (!string.IsNullOrEmpty(Order.Remark))
            {
                BufferList.Add(PrinterCmdUtils.FontSizeSetBig(2));
                BufferList.Add(PrinterCmdUtils.BoldOn());
                BufferList.Add(Encoding.GetEncoding("gbk").GetBytes($"备注：{Order.Remark}"));
                BufferList.Add(PrinterCmdUtils.NextLine());
                BufferList.Add(PrinterCmdUtils.FontSizeSetBig(1));
                BufferList.Add(PrinterCmdUtils.BoldOff());
                BufferList.Add(PrinterCmdUtils.NextLine());
            }
            BufferList.Add(PrinterCmdUtils.FontSizeSetBig(1));

            BufferList.Add(Encoding.GetEncoding("gbk").GetBytes("下单时间：" + Order.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss")));

            BufferList.Add(PrinterCmdUtils.NextLine());
            BufferList.Add(PrinterCmdUtils.SplitLine("-", Printer.Format));
            BufferList.Add(PrinterCmdUtils.NextLine());
            BufferList.Add(PrinterCmdUtils.FontSizeSetBig(2));
        }
        protected virtual void AfterPrint()
        {
            BufferList.Add(PrinterCmdUtils.NextLine(2));
            BufferList.Add(PrinterCmdUtils.FeedPaperCutAll());
        }
        protected abstract void Printing();
    }
}
