using Newtonsoft.Json;
using System;
using System.Net.Http;

namespace WeChat_Token.WeChat.GetHttps
{
    public class GetTokenHttp
    {
        //是第一步，利用http协议，解析https://api.weixin.qq.com/sns/oauth2/access_token?appid=APPID&secret=SECRET&code=CODE&grant_type=authorization_code

        //得到必要的获取微信参数的信息
        public string GetJson(string url)
        {
            //1、创建一个Uri：Uri类
            Uri uri = new Uri(url);

            //2、创建一个HttpClient客户端对象：HttpClient类
            HttpClient httpClient = new HttpClient();

            //3、设置HttpClient客户端要获取资源数据的地址：BaseAddress属性
            httpClient.BaseAddress = uri;

            //4、发送一个get方法的请求:GetAsync方法
            var PushAction = httpClient.GetAsync(url);

            //5、获取get方法请求之后返回的数据：Result属性
            var responseResult = PushAction.Result;
            //6、判断返回的数据状态
            //responseResult.StatusCode:获取返回的状态码。
            //HttpStatusCode.OK:等效于 HTTP 状态 200。 System.Net.HttpStatusCode.OK 指示请求成功，且请求的信息包含在响应中。 这是最常接收的状态代码。
            if (responseResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //Content:获取 HTTP 响应消息的内容。
                //ReadAsStringAsync():以异步操作将 HTTP 内容写入流。
                //Result:获取此 System.Threading.Tasks.Task<TResult> 的结果值。
                var result = responseResult.Content.ReadAsStringAsync().Result;

                //6.5 转换数据格式
                //把结果字符串反序列化成List对象。
                //    var list = JsonConvert.DeserializeObject<GetToken>(result);
                //7、释放资源 
                httpClient.Dispose();
                
                return result;

            }
            else
            {
                //7、释放资源 
                httpClient.Dispose();
                return null;
            }

        }






    }
}
