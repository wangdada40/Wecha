using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using WeChat_Token.Models;
using WeChat_Token.WeChat;
using WeChat_Token.WeChat.GetHttps;

namespace WeChat_Token.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public string Index2()
        {
            GetTokenHttp getHttp = new GetTokenHttp();
            string url = "http://t.weather.sojson.com/api/weather/city/101030100";
            var ce = getHttp.GetJson(url);
            return ce + "!!!!!!!";
        }

        /// <summary>
        /// 定义Token，与微信公共平台上的Token保持一致
        /// </summary>
        private const string Token = "StupidMe";

        /// <summary>
        /// 验证签名，检验是否是从微信服务器上发出的请求
        /// </summary>
        /// <param name="model">请求参数模型 Model</param>
        /// <returns>是否验证通过</returns>
        private bool CheckSignature(WeChatRequestModel model)
        {
            string signature, timestamp, nonce, tempStr;
            //获取请求来的参数
            signature = model.signature;
            timestamp = model.timestamp;
            nonce = model.nonce;
            //创建数组，将 Token, timestamp, nonce 三个参数加入数组
            string[] array = { Token, timestamp, nonce };
            //进行排序
            Array.Sort(array);
            //拼接为一个字符串
            tempStr = String.Join("", array);
            //对字符串进行 SHA1加密
            tempStr = Get_SHA1_Method2(tempStr);
            //判断signature 是否正确
            if (tempStr.Equals(signature))
            {
                return true;
            }
            else
            {
                return false;
            }
            //return true;
        }

        //代替FormsAuthentication.HashPasswordForStoringInConfigFile(tempStr, "SHA1");
        public string Get_SHA1_Method2(string strSource)
        {
            string strResult = "";

            //Create 
            System.Security.Cryptography.SHA1 md5 = System.Security.Cryptography.SHA1.Create();

            //注意编码UTF8、UTF7、Unicode等的选择 
            byte[] bytResult = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(strSource));

            //字节类型的数组转换为字符串 
            for (int i = 0; i < bytResult.Length; i++)
            {
                //16进制转换 
                strResult = strResult + bytResult[i].ToString("X");
            }
            return strResult.ToLower();
        }

        /// <summary>
        /// 整个验证的核心
        /// </summary>
        /// <param name="model"></param>

        //可以给WeChatRequestModel 加东西，比如我加了Code。
        //登录的时候我就可以从微信拿到Code
        public GetToken Valid(WeChatRequestModel model)
        {
            //获取请求来的 echostr 参数
            string echoStr = model.echostr;
            //通过验证，出于安全考虑。（也可以跳过）
            if (CheckSignature(model))
            {
                if (!string.IsNullOrEmpty(echoStr))
                {
                    //将随机生成的 echostr 参数 原样输出
                    Response.WriteAsync(echoStr);
                    //截止输出流
                    //Response.end();
                }
            }
            //解析连接的类
            GetTokenHttp getHttp = new GetTokenHttp();
          //  GetUserHttp getUserHttp = new GetUserHttp();
            WxPayConfig wxPayConfig = new WxPayConfig();
            //通过code换取网页授权access_token
            string url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid=" + wxPayConfig.appid + "&secret=" + wxPayConfig.appSecret + "&code=" + model.code + "&grant_type=authorization_code";
            var parameter = getHttp.GetJson(url);
            //拉取用户信息
            //如果网页授权作用域为snsapi_userinfo，则此时开发者可以通过access_token和openid拉取用户信息了。
         //   string Userurl = "https://api.weixin.qq.com/sns/userinfo?access_token="+parameter.access_token+"&openid="+parameter.openid+"&lang=zh_CN";
          // var User = getUserHttp.GetUsreJson(Userurl);1
            return parameter;
        }
    }
}
