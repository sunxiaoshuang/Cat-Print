using CatPrint.Code;
using CatPrint.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
//using System.Net.Sockets;
//using System.Net.WebSockets;
using System.Text;
using System.Threading;
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
using WebSocketSharp;

namespace CatPrint
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        //public Socket Socket { get; set; }
        //private ManualResetEvent connectDone = new ManualResetEvent(false);
        public bool InitSuccess = true;
        private WebSocket socket;
        public MainWindow()
        {
            InitializeComponent();
            InitWebSocket();
            //InitSocket();
            //if (InitSuccess)
            //{
            this.Closing += (a, e) =>
            {
                var result = MessageBox.Show("确定退出吗？", "提示", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            };
            //}


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

        #region ClientWebSocket

        private void InitWebSocket()
        {
            //try
            //{
            //    var client = new ClientWebSocket();
            //    client.ConnectAsync(new Uri(string.Format(ConfigurationManager.AppSettings["ClientWebSocketUri"], ApplicationObject.App.Business.ID)), CancellationToken.None).Wait();
            //    StartReceiving(client);
            //}
            //catch (Exception ex)
            //{
            //    InitSuccess = false;
            //    MessageBox.Show(ex.Message);
            //}

            socket = new WebSocket(string.Format(ConfigurationManager.AppSettings["ClientWebSocketUri"], ApplicationObject.App.Business.ID));
            socket.Connect();
            socket.OnMessage += MessageHandler;
            socket.OnClose += (sender, e) =>
            {
                MessageBox.Show("网络异常或者另外一个用户接入，新订单提醒已关闭");
            };
        }


        //private async void StartReceiving(ClientWebSocket client)
        //{
        //    try
        //    {
        //        while (true)
        //        {
        //            var buffer = new byte[512];
        //            var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        //            if (result.MessageType == WebSocketMessageType.Text)
        //            {
        //                string code = Encoding.UTF8.GetString(buffer, 0, result.Count);
        //                var order = await Request.GetOrder(code);
        //                if (order != null)
        //                {
        //                    this.Dispatcher.Invoke(() =>
        //                    {
        //                        ApplicationObject.Print(order);
        //                    });
        //                    var filename = string.Empty;
        //                    if (order.Status == Enum.OrderStatus.Payed)
        //                    {
        //                        filename = "1.mp3";
        //                    }
        //                    else if (order.Status == Enum.OrderStatus.Receipted)
        //                    {
        //                        filename = "2.mp3";
        //                    }
        //                    var player = new MediaPlayer();
        //                    player.Open(new Uri("Assets/Video/" + filename, UriKind.Relative));
        //                    player.Play();
        //                    player.Volume = 1;
        //                }
        //                else
        //                {
        //                    MessageBox.Show(code);
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        MessageBox.Show("订单接收异常或者有一个新的连接进入，如果需要重新接收订单提醒，请重新启动程序");
        //    }
        //}

        private async void MessageHandler(object sender, MessageEventArgs e)
        {
            try
            {

                var buffer = new byte[512];
                string code = e.Data;
                var order = await Request.GetOrder(code);
                if (order != null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        ApplicationObject.Print(order);
                    });
                    var filename = string.Empty;
                    if (order.Status == Enum.OrderStatus.Payed)
                    {
                        filename = "1.mp3";
                    }
                    else if (order.Status == Enum.OrderStatus.Receipted)
                    {
                        filename = "2.mp3";
                    }
                    var player = new MediaPlayer();
                    player.Open(new Uri("Assets/Video/" + filename, UriKind.Relative));
                    player.Play();
                    player.Volume = 1;

                }
                else
                {
                    MessageBox.Show(code);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

    }
}
