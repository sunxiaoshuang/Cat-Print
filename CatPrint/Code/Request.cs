using CatPrint.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WPF.Test.Practise2.Code;

namespace CatPrint.Code
{
    public static class Request
    {
        public static string ApiUrl { get; set; }
        static Request()
        {
            ApiUrl = ConfigurationManager.AppSettings["ApiUrl"];
        }
        public async static Task<JsonData> Login(string code, string pwd)
        {
            using (var client = new HttpClient())
            {
                var ret = await client.GetAsync($"{ApiUrl}/Business/login?username={code}&pwd={pwd}");
                var result = await ret.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<JsonData>(result);
                return data;
            }
        }
        public async static Task<List<Order>> GetOrders(Business business, PagingQuery paging)
        {
            using (var client = new HttpClient())
            {
                var param = new StringContent(JsonConvert.SerializeObject(paging));
                param.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                var res = await client.PostAsync(ApiUrl + $"/order/getOrder/0?businessId={ApplicationObject.App.Business.ID}&createTime={DateTime.Now:yyyy-MM-dd}", param);
                //var res = await client.PostAsync(ApiUrl + $"/order/getOrder/0?businessId={ApplicationObject.App.Business.ID}", param);
                var content = await res.Content.ReadAsStringAsync();
                var jObj = JObject.Parse(content);
                var list = JsonConvert.DeserializeObject<List<Order>>(jObj["data"]["list"].ToString());
                paging.RecordCount = int.Parse(jObj["data"]["rows"].ToString());
                double a1 = paging.RecordCount, a2 = paging.PageSize;
                paging.PageCount = (int)Math.Ceiling(a1 / a2);
                return list;
            }
        }
        public async static Task<Order> GetOrder(string code)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var res = await client.GetAsync(ApiUrl + $"/order/singleByCode?code={code}");
                    var content = await res.Content.ReadAsStringAsync();
                    var order = JsonConvert.DeserializeObject<Order>(content);
                    return order;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public async static Task<List<ProductType>> GetTypes(Business business)
        {
            using (var client = new HttpClient())
            {
                var res = await client.GetAsync(ApiUrl + $"/product/types/{business.ID}");
                var content = await res.Content.ReadAsStringAsync();
                var list = JsonConvert.DeserializeObject<List<ProductType>>(content);
                return list;
            }
        }

        public static bool Print(Order order)
        {
            Socket mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAddress = IPAddress.Parse("192.168.0.87");
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 9100);

            mySocket.Connect(ipEndPoint);
            mySocket.Send(PrinterCmdUtils.AlignCenter());
            mySocket.Send(PrinterCmdUtils.FontSizeSetBig(3));
            mySocket.Send(TextToByte("#3"));
            mySocket.Send(PrinterCmdUtils.NextLine());
            mySocket.Send(PrinterCmdUtils.AlignLeft());
            mySocket.Send(PrinterCmdUtils.FontSizeSetBig(1));
            mySocket.Send(TextToByte("商家小票"));
            mySocket.Send(PrinterCmdUtils.NextLine());
            mySocket.Send(PrinterCmdUtils.SplitLine("-"));
            mySocket.Send(PrinterCmdUtils.NextLine());
            mySocket.Send(PrinterCmdUtils.AlignCenter());
            mySocket.Send(PrinterCmdUtils.FontSizeSetBig(2));
            mySocket.Send(TextToByte("简单猫科技"));
            mySocket.Send(PrinterCmdUtils.NextLine());
            mySocket.Send(PrinterCmdUtils.NextLine());
            mySocket.Send(PrinterCmdUtils.AlignLeft());
            mySocket.Send(PrinterCmdUtils.FontSizeSetBig(1));
            mySocket.Send(TextToByte($"下单时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}"));
            mySocket.Send(PrinterCmdUtils.NextLine());
            mySocket.Send(TextToByte("订单编号：201807283486948349"));
            mySocket.Send(PrinterCmdUtils.NextLine());
            mySocket.Send(TextToByte("--------------------购买商品--------------------"));
            mySocket.Send(PrinterCmdUtils.NextLine());
            mySocket.Send(TextToByte("劲爆花甲"));
            mySocket.Send(PrinterCmdUtils.AlignCenter());
            mySocket.Send(TextToByte("* 1"));
            mySocket.Send(PrinterCmdUtils.AlignRight());
            mySocket.Send(TextToByte("12"));
            mySocket.Send(PrinterCmdUtils.NextLine());
            mySocket.Send(PrinterCmdUtils.AlignLeft());
            mySocket.Send(TextToByte("麻辣虾球"));
            mySocket.Send(PrinterCmdUtils.AlignCenter());
            mySocket.Send(TextToByte("* 2"));
            mySocket.Send(PrinterCmdUtils.AlignRight());
            mySocket.Send(TextToByte("36"));
            mySocket.Send(PrinterCmdUtils.NextLine());
            mySocket.Send(TextToByte("----------------------其他----------------------"));
            mySocket.Send(PrinterCmdUtils.NextLine());
            mySocket.Send(PrinterCmdUtils.AlignLeft());
            mySocket.Send(TextToByte("配送费"));
            mySocket.Send(PrinterCmdUtils.AlignRight());
            mySocket.Send(TextToByte("5"));
            mySocket.Send(PrinterCmdUtils.NextLine());
            mySocket.Send(TextToByte("------------------------------------------------"));
            mySocket.Send(PrinterCmdUtils.NextLine());
            mySocket.Send(PrinterCmdUtils.FeedPaperCutAll());
            mySocket.Close();
            return true;
        }

        private static byte[] TextToByte(string text)
        {
            return Encoding.Default.GetBytes(text);
        }
    }
}
