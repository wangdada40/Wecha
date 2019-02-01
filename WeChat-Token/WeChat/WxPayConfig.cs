namespace WeChat_Token.WeChat
{
    public class WxPayConfig
    {
        public static WxPayConfig Instance = new WxPayConfig();

        public string appid = "wx5cbc752c4bd1b951";//APPID

        public string mchid = "";//商户号

        public string key = "";//商户API密钥

        public string appSecret = "66df01a9311dcbbebbe456d7f7e0a70d";//公众号支付和app支付时候将用到

        public string notify_url = "http://w545268514.uicp.io/Home/Valid";//回调页地址

        public string api_url = "https://api.mch.weixin.qq.com/pay/unifiedorder";//微信支付调用接口地址
    }
}
