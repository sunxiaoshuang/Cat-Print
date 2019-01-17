using CatPrint.Code;
using CatPrint.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Runtime.InteropServices;
//using System.Net.Sockets;
//using System.Net.WebSockets;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using WebSocketSharp;

namespace CatPrint
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);
        /// <summary>
        /// Socket连接是否正常
        /// </summary>
        public bool IsConnect { get; set; }
        /// <summary>
        /// 订单提醒的Socket
        /// </summary>
        private WebSocket socket;
        /// <summary>
        /// Socket连接字符串
        /// </summary>
        private string socketLink;
        public MainWindow()
        {
            InitializeComponent();
            //socketLink = string.Format(ConfigurationManager.AppSettings["ClientWebSocketUri"], ApplicationObject.App.Business.ID);
            //InitWebSocket();
            InitTimer();
            this.Closing += (a, e) =>
            {
                var result = MessageBox.Show("确定退出吗？", "提示", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            };

        }
        #region Socket

        //private void InitSocket()
        //{
        //    var business = ApplicationObject.App.Business;
        //    var ip = ConfigurationManager.AppSettings["OrderNotifyIp"];
        //    var port = int.Parse(ConfigurationManager.AppSettings["OrderNotifyPort"]);
        //    IPHostEntry ipHostInfo = Dns.GetHostEntry(ip);
        //    IPAddress ipAddress = ipHostInfo.AddressList[0];
        //    IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
        //    Socket = new Socket(remoteEP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        //    Socket.BeginConnect(remoteEP,
        //        new AsyncCallback(ConnectCallback), Socket);
        //    connectDone.WaitOne();
        //    if (!InitSuccess) return;
        //    Send(Socket, business.ID.ToString());

        //    Receive(Socket);

        //}

        //private void ConnectCallback(IAsyncResult ar)
        //{
        //    try
        //    {
        //        Socket client = (Socket)ar.AsyncState;

        //        client.EndConnect(ar);
        //        connectDone.Set();
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show("新订单提醒连接异常：" + e.Message);
        //        InitSuccess = false;
        //        connectDone.Set();
        //    }
        //}

        //private void Receive(Socket client)
        //{
        //    try
        //    {
        //        StateObject state = new StateObject();
        //        state.workSocket = client;

        //        client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show("新订单消息监听异常：" + e.Message);
        //    }
        //}

        //private void ReceiveCallback(IAsyncResult ar)
        //{
        //    StateObject state = (StateObject)ar.AsyncState;
        //    Socket client = state.workSocket;
        //    try
        //    {
        //        var position = client.EndReceive(ar);
        //        string code = Encoding.UTF8.GetString(state.buffer, 0, position);
        //        var req = Request.GetOrder(code);
        //        var order = req.Result;
        //        if (order != null)
        //        {
        //            this.Dispatcher.Invoke(() =>
        //            {
        //                ApplicationObject.Print(order);
        //            });
        //            var filename = string.Empty;
        //            if (order.Status == Enum.OrderStatus.Payed)
        //            {
        //                filename = "1.mp3";
        //            }
        //            else if (order.Status == Enum.OrderStatus.Receipted)
        //            {
        //                filename = "2.mp3";
        //            }
        //            var player = new MediaPlayer();
        //            player.Open(new Uri("Assets/Video/" + filename, UriKind.Relative));
        //            player.Play();
        //            player.Volume = 1;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show("新订单消息接收异常：" + e.Message);
        //    }
        //    finally
        //    {
        //        state.buffer = new byte[StateObject.BufferSize];
        //        client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
        //    }
        //}

        //private static void Send(Socket client, String data)
        //{
        //    byte[] byteData = Encoding.ASCII.GetBytes(data);

        //    client.BeginSend(byteData, 0, byteData.Length, 0,
        //        new AsyncCallback(SendCallback), client);
        //}

        //private static void SendCallback(IAsyncResult ar)
        //{
        //    try
        //    {
        //        Socket client = (Socket)ar.AsyncState;

        //        client.EndSend(ar);
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show("新订单提示匹配异常：" + e.Message);
        //    }
        //}

        #endregion

        #region websocket连接
        /// <summary>
        /// 初始化Socket连接
        /// </summary>
        //private void InitWebSocket()
        //{
        //    socket = new WebSocket(socketLink);
        //    socket.Connect();
        //    socket.OnMessage += MessageHandler;
        //    socket.OnClose += (sender, e) =>
        //    {
        //        if (e.Code == 1000)
        //        {
        //            MessageBox.Show(e.Reason);
        //        }
        //        else
        //        {
        //            var isSuccess = Connect();
        //            if (!isSuccess)
        //            {
        //                IsConnect = false;
        //                PlayMedia("Assets/Video/4.mp3");
        //                //while (MessageBox.Show($"网络异常，新订单提醒已关闭，点击确定后重新连接", "通信异常", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
        //                //{
        //                //    if(Connect())
        //                //    {
        //                //        MessageBox.Show("已成功连接");
        //                //        break;
        //                //    }
        //                //}
        //                ThreadPool.QueueUserWorkItem(obj =>
        //                {
        //                    var index = 0;
        //                    var control = (MainWindow)obj;
        //                    while (!control.IsConnect)
        //                    {
        //                        if (InternetGetConnectedState(out index, 0))
        //                        {
        //                            ((Control)obj).Dispatcher.Invoke(() =>
        //                            {
        //                                var result = Connect();
        //                                if (result)
        //                                {
        //                                    MessageBox.Show("新订单提醒已恢复正常");
        //                                    control.IsConnect = true;
        //                                    return;
        //                                }
        //                            });
        //                        }
        //                    }
        //                }, this);
        //            }
        //        }
        //    };
        //}


        ///// <summary>
        ///// 连接断开后重新连接
        ///// </summary>
        ///// <returns></returns>
        //private bool Connect()
        //{
        //    // 尝试2次连接
        //    var times = 2;
        //    for (int i = 0; i < times; i++)
        //    {
        //        InitWebSocket();
        //        if (socket.ReadyState == WebSocketState.Open)
        //        {
        //            return true;
        //        }
        //        // 每次连接后等待10秒再连
        //        Thread.Sleep(10000);
        //    }
        //    return false;
        //}

        private async void MessageHandler(object sender, MessageEventArgs e)
        {
            try
            {
                var buffer = new byte[512];
                var code = e.Data;
                code = code.Split('|')[0];
                var order = await Request.GetOrder(code);
                this.Dispatcher.Invoke(() =>
                {
                    var filename = string.Empty;
                    if (order.Status == Enum.OrderStatus.Payed)
                    {
                        filename = "1.mp3";
                    }
                    else
                    {
                        filename = "2.mp3";
                    }
                    PlayMedia("Assets/Video/" + filename);
                    ApplicationObject.Print(order);
                }, DispatcherPriority.Normal);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region 定时请求服务器，读取新订单
        private static DispatcherTimer readDataTimer = new DispatcherTimer();
        private static string orderUrl;
        private static bool isError = false;    // 记录是否出错
        private void InitTimer()
        {
            readDataTimer.Tick += new EventHandler(HandleOrder);
            readDataTimer.Interval = new TimeSpan(0, 0, 0, 5);          // 5秒取一次
            orderUrl = string.Format(ConfigurationManager.AppSettings["OrderUrl"], ApplicationObject.App.Business.ID);
            readDataTimer.Start();
        }

        private async void HandleOrder(object sender, EventArgs e)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var res = await client.GetAsync(orderUrl);
                    res.EnsureSuccessStatusCode();
                    var content = await res.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<JsonData>(content);
                    if (result.Data == null) return;
                    var data = (JArray)result.Data;
                    var orders = new List<Order>();
                    foreach (string item in data)
                    {
                        orders.Add(JsonConvert.DeserializeObject<Order>(item));
                    }
                    var firstOrder = orders[0];
                    this.Dispatcher.Invoke(() =>
                    {
                        var filename = string.Empty;
                        if (firstOrder.Status == Enum.OrderStatus.Payed)
                        {
                            filename = "1.mp3";
                        }
                        else
                        {
                            filename = "2.mp3";
                        }
                        PlayMedia("Assets/Video/" + filename);
                    }, DispatcherPriority.Normal);
                    orders.ForEach(order =>
                    {
                        ApplicationObject.Print(order);
                    });
                }
            }
            catch (Exception ex)
            {
                if (isError) return;
                //isError = true;
                //var result = MessageBox.Show("读取新订单错误：" + ex.Message, "提示", MessageBoxButton.OK);
                //if (result == MessageBoxResult.OK)
                //{
                //    isError = false;
                //}
                AutoClosingMessageBox.Show("读取新订单错误：" + ex.Message, "提示", 3000);
            }
        }

        public class AutoClosingMessageBox
        {
            Timer _timeoutTimer;
            string _caption;
            AutoClosingMessageBox(string text, string caption, int timeout)
            {
                _caption = caption;
                _timeoutTimer = new Timer(OnTimerElapsed, null, timeout, Timeout.Infinite);
                MessageBox.Show(text, caption);
            }
            public static void Show(string text, string caption, int timeout)
            {
                new AutoClosingMessageBox(text, caption, timeout);
            }
            void OnTimerElapsed(object state)
            {
                IntPtr mbWnd = FindWindow(null, _caption);
                if (mbWnd != IntPtr.Zero)
                    SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                _timeoutTimer.Dispose();
            }
            const int WM_CLOSE = 0x0010;
            [DllImport("user32.dll", SetLastError = true)]
            static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        }

        #endregion


        /// <summary>
        /// 播放提示音
        /// </summary>
        /// <param name="path"></param>
        private void PlayMedia(string path)
        {
            var player = new MediaPlayer();
            player.Open(new Uri(path, UriKind.Relative));
            player.Play();
            player.Volume = 1;
            player.MediaEnded += (a, b) =>
            {
                player.Clone();
            };
        }

    }
    public delegate void OrderMessageHandler(JsonData jsonData);
}
