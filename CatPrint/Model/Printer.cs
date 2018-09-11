using CatPrint.Code;
using CatPrint.Enum;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WPF.Test.Practise2.Code;

namespace CatPrint.Model
{
    public class Printer : INotifyPropertyChanged, ICloneable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 打印商品中数量占用的纸张长度
        /// </summary>
        private const int QuantityLen = 6;
        /// <summary>
        /// 打印商品中价格占用的商品长度
        /// </summary>
        private const int PriceLen = 8;
        private string _id;
        /// <summary>
        /// 打印机ID
        /// </summary>
        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Id"));
            }
        }

        private string _name;
        /// <summary>
        /// 打印机名称
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
            }
        }

        private string _ip;
        /// <summary>
        /// 打印机IP地址
        /// </summary>
        public string IP
        {
            get { return _ip; }
            set
            {
                _ip = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IP"));
            }
        }

        private int _port;
        /// <summary>
        /// 打印机端口号
        /// </summary>
        public int Port
        {
            get { return _port; }
            set
            {
                _port = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Port"));
            }
        }

        public int _type;
        /// <summary>
        /// 打印机类型，[1:前台，2：后厨]
        /// </summary>
        public int Type
        {
            get { return _type; }
            set
            {
                _type = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Type"));
            }
        }

        private int _state;
        /// <summary>
        /// 打印机当前状态，[1:正常，2:停用]
        /// </summary>
        public int State
        {
            get { return _state; }
            set
            {
                _state = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("State"));
            }
        }

        private int _quantity;
        /// <summary>
        /// 每次打印数量
        /// </summary>
        public int Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Quantity"));
            }
        }

        private PrinterMode _mode;
        /// <summary>
        /// 打印模式
        /// </summary>
        public PrinterMode Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Mode"));
            }
        }

        private int _format;
        /// <summary>
        /// 打印规格，58mm(32字符), 80mm(48字符)
        /// </summary>
        public int Format
        {
            get { return _format; }
            set
            {
                _format = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Format"));
            }
        }

        /// <summary>
        /// 规格对应的字符长度
        /// </summary>
        public int FormatLen
        {
            get
            {
                return Format == 80 ? 48 : 32;
            }
        }

        /// <summary>
        /// 打印商品列表中名称的占用纸张长度
        /// </summary>
        public int NameLen
        {
            get
            {
                return FormatLen - QuantityLen - PriceLen;
            }
        }

        private ObservableCollection<int> _foods;
        /// <summary>
        /// 关联的菜单
        /// </summary>
        public ObservableCollection<int> Foods
        {
            get { return _foods; }
            set
            {
                _foods = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Foods"));
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// 打印订单
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public string Print(Order order)
        {
            if (Type == 2)
            {
                if (order.Products.Count == 0 || Foods.Count == 0) return null;
            }
            var mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var ipAddress = IPAddress.Parse(IP);
            var ipEndPoint = new IPEndPoint(ipAddress, Port);
            try
            {
                mySocket.Connect(ipEndPoint);
                if(Type == 1)
                {
                    ReceptionPrint(order, mySocket);
                }
                else if(Type == 2)
                {
                    Backstage(order, mySocket);
                }
            }
            catch (Exception ex)
            {
                return $"打印机[{Name}]出错，原因：" + ex.Message;
            }
            finally
            {
                mySocket.Close();
            }
            return null;

        }
        /// <summary>
        /// 前台打印
        /// </summary>
        /// <param name="order"></param>
        /// <param name="socket"></param>
        private void ReceptionPrint(Order order, Socket socket)
        {
            var bufferArr = new List<byte[]>();
            // 打印当日序号
            bufferArr.Add(PrinterCmdUtils.AlignCenter());
            bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(4));
            bufferArr.Add(TextToByte("#" + order.Identifier));
            bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(2));
            bufferArr.Add(TextToByte("简单猫"));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 打印小票类别
            bufferArr.Add(PrinterCmdUtils.AlignLeft());
            bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(1));
            bufferArr.Add(TextToByte("前台小票"));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 分隔
            bufferArr.Add(PrinterCmdUtils.SplitLine("-", Format));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 备注
            if(!string.IsNullOrEmpty(order.Remark))
            {
                bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(2));
                bufferArr.Add(TextToByte($"备注：{order.Remark}"));
                bufferArr.Add(PrinterCmdUtils.NextLine());
                bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(1));
                bufferArr.Add(PrinterCmdUtils.NextLine());
            }
            // 商户名称
            bufferArr.Add(PrinterCmdUtils.AlignCenter());
            bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(2));
            bufferArr.Add(TextToByte(ApplicationObject.App.Business.Name));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            bufferArr.Add(PrinterCmdUtils.AlignLeft());
            bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(1));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 下单时间
            bufferArr.Add(TextToByte($"下单时间：{order.PayTime:yyyy-MM-dd HH:mm:ss}"));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 订单编号
            bufferArr.Add(TextToByte($"订单编号：{order.OrderCode}"));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 商品分隔
            bufferArr.Add(PrinterCmdUtils.SplitText("-", "购买商品", Format));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 打印商品
            foreach (var product in order.Products)
            {
                var buffer = ProductLine(product);
                buffer.ForEach(a => {
                    bufferArr.Add(a);
                    bufferArr.Add(PrinterCmdUtils.NextLine());
                });
            }
            // 分隔
            bufferArr.Add(PrinterCmdUtils.SplitText("-", "其他", Format));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 配送
            bufferArr.Add(PrintLineLeftRight("配送费", double.Parse(order.Freight + "") + ""));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 订单金额
            bufferArr.Add(PrinterCmdUtils.AlignRight());
            bufferArr.Add(TextToByte("实付："));
            bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(2));
            bufferArr.Add(TextToByte(order.Price + "元"));
            bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(1));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            bufferArr.Add(PrinterCmdUtils.AlignLeft());
            // 分隔
            bufferArr.Add(PrinterCmdUtils.SplitLine("*", Format));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            // 地址
            bufferArr.Add(PrinterCmdUtils.FontSizeSetBig(2));
            bufferArr.Add(TextToByte(order.ReceiverAddress));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            bufferArr.Add(TextToByte(order.Phone));
            bufferArr.Add(PrinterCmdUtils.NextLine());
            bufferArr.Add(TextToByte(order.ReceiverName));
            bufferArr.Add(PrinterCmdUtils.NextLine());

            // 切割
            bufferArr.Add(PrinterCmdUtils.FeedPaperCutAll());

            // 打印
            bufferArr.ForEach(a => socket.Send(a));
        }
        /// <summary>
        /// 后台打印
        /// </summary>
        /// <param name="order"></param>
        /// <param name="socket"></param>
        private void Backstage(Order order, Socket socket)
        {
            var enumName = System.Enum.GetName(typeof(PrinterMode), Mode);
            var type = System.Type.GetType($"CatPrint.Code.{enumName}Print");
            var obj = (BackstagePrint)Activator.CreateInstance(type, order, this, socket);
            obj.Print();
        }

        /// <summary>
        /// 将内容转化为字节数组
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private byte[] TextToByte(string text)
        {
            return Encoding.GetEncoding("gbk").GetBytes(text);
        }
        /// <summary>
        /// 打印订单商品
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        private List<byte[]> ProductLine(OrderProduct product)
        {
            var name = product.Name;
            var zhQuantity = 0;              // 中文字符数
            var enQuantity = 0;              // 其他字符数
            var cutName = string.Empty;      // 截取的名称
            while (true)
            {
                zhQuantity = PrinterCmdUtils.CalcZhQuantity(name);
                enQuantity = name.Length - zhQuantity;
                if (zhQuantity * 2 + enQuantity > NameLen)
                {
                    cutName += name.Substring(name.Length - 2);
                    name = name.Substring(0, name.Length - 2);
                }
                else
                {
                    break;
                }
            }
            var line = name;
            // 商品名称
            var nameLen = zhQuantity * 2 + enQuantity;
            for (int i = 0; i < NameLen - nameLen; i++)
            {
                line += " ";
            }
            // 商品数量
            var count = "*" + Convert.ToDouble(product.Quantity);
            var countLen = count.Length;
            for (int i = 0; i < QuantityLen - countLen; i++)
            {
                count += " ";
            }
            line += count;
            // 商品价格
            var price = Convert.ToDouble(product.Price).ToString();
            var priceLength = price.Length;
            for (int i = 0; i < PriceLen - priceLength; i++)
            {
                price = " " + price;
            }
            line += price;

            // 返回二进制数组
            var bufferArr = new List<byte[]>();
            bufferArr.Add(TextToByte(line));
            // 超出的商品名称
            if (!string.IsNullOrEmpty(cutName))
            {
                bufferArr.Add(TextToByte(cutName));
            }
            // 规格、属性
            if (!string.IsNullOrEmpty(product.Description))
            {
                bufferArr.Add(TextToByte($"（{product.Description}）"));
            }
            return bufferArr;
        }

        /// <summary>
        /// 打印左右对齐的行
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private byte[] PrintLineLeftRight(string left, string right, int fontSize = 1)
        {
            var zhLeft = PrinterCmdUtils.CalcZhQuantity(left);          // 左边文本的中文字符长度
            var enLeft = left.Length - zhLeft;          // 左边文本的其他字符长度
            var zhRight = PrinterCmdUtils.CalcZhQuantity(right);        // 右边文本的中文字符长度
            var enRight = right.Length - zhRight;       // 右边文本的其他字符长度
            var len = FormatLen - ((zhLeft * 2 + enLeft + zhRight * 2 + enRight) * fontSize);            // 缺少的字符长度
            if(len > 0)
            {
                for (int i = 0; i < len / fontSize; i++)
                {
                    left += " ";
                }
            }
            else
            {
                var times = 1;
                while (true)
                {
                    if(FormatLen * times + len > 0)
                    {
                        break;
                    }
                    times++;
                }
                for (int i = 0; i < (FormatLen * times + len) / fontSize; i++)
                {
                    left += " ";
                }
            }
            return TextToByte(left + right);
        }
    }
}
