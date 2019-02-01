using Newtonsoft.Json;
using Sky.Logger;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;

namespace WeChatPay.Pay
{
    //工具类，
    public class OAuth
    {
        //发送json连接
        //得到授权连接
        public static HttpClient httpClient = new HttpClient();
        public static string GetAuthorizeUrl(string appId, string redirectUrl, string state = "1", string scope = "snsapi_userinfo", string responseType = "code")
        {
            if (!string.IsNullOrEmpty(redirectUrl))
            {
                redirectUrl = HttpUtility.UrlEncode(redirectUrl, System.Text.Encoding.UTF8);
            }
            else
            {
                redirectUrl = null;
            }
            object[] args = new object[] { appId, redirectUrl, responseType, scope, state };
            return string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type={2}&scope={3}&state={4}#wechat_redirect", args);

        }
        //获取回调
        public static string GetOpenIdUrl(string appId, string secret, string code, string grantType = "authorization_code")
        {
            object[] args = new object[] { appId, secret, code, grantType };
            string requestUri = string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type={3}", args);
            return requestUri;
        }

        /// <summary>
        /// 获取openid
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="secret"></param>
        /// <param name="code"></param>
        /// <param name="grantType"></param>
        /// <returns></returns>
        public static string GetOpenid(string appId, string secret, string code, string grantType = "authorization_code")
        {
            //获取回调
            string requestUri = GetOpenIdUrl(appId, secret, code, grantType);
            //访问回调连接
            var responseStr = httpClient.GetAsync(requestUri).Result.Content.ReadAsStringAsync().Result;
            //编辑格式
            var obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseStr);
            string openid = string.Empty;
            if (!obj.TryGetValue("openid", out openid))
            {
                Log.Info($"获取openid失败appId={appId}，secret={secret}，code={code}");
            }
            return openid;
        }

    }
}
