using Senparc.Weixin.MP.TenPayLib;
using Sky.Logger;
using System;
using System.IO;
using System.Net.Http;
using System.Text;

namespace WeChat_Token.WeChat
{
    public class WxPayService
    {

        /// <summary>
        /// post请求，将字符串转为流上传到url中
        /// </summary>
        /// <param name="url"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public string PostWithStringFile(string url, string file)
        {
            var formDataBytes = file == null ? new byte[0] : Encoding.UTF8.GetBytes(file);//将xml字符串转为字节流
            MemoryStream ms = new MemoryStream(formDataBytes);//将字节流转为内存流
            StreamContent streamContent = new StreamContent(ms);//封装为StreamContent对象
            //发起post异步请求，获取返回的内容
            var result = httpClient.PostAsync(wxPayConfig.api_url, streamContent).Result.Content.ReadAsStringAsync().Result;
            return result;
        }


          public bool WxPayCheck(ResponseHandler wxResponseHandler)
        {
            return true; ;
        }


        /// <summary>
        /// 获取微信扫码支付URL
        /// </summary>
        /// <param name="out_trade_no">订单号</param>
        /// <param name="body">描述</param>
        /// <param name="total_fee">总价</param>
        /// <param name="ip">客户IP</param>
        /// <param name="product_id">商品id</param>
        /// <returns></returns>
        ///
        //以及服务类包含字段
        #region 字段
        public WxPayConfig wxPayConfig = new WxPayConfig();//微信配置文件
        public HttpClient httpClient = new HttpClient();//http请求客户端
        #endregion
        public string GetWxSMPayUrl(string out_trade_no, string body, string total_fee, string ip, string product_id)
        {
            Senparc.Weixin.MP.TenPayLibV3.RequestHandler packageReqHandler = new Senparc.Weixin.MP.TenPayLibV3.RequestHandler();
            #region 构造请求参数
            packageReqHandler.SetParameter("appid", wxPayConfig.appid);//APPID
            packageReqHandler.SetParameter("mch_id", wxPayConfig.mchid);//商户号
            packageReqHandler.SetParameter("nonce_str", Senparc.Weixin.MP.TenPayLibV3.TenPayV3Util.GetNoncestr());//随机串
            packageReqHandler.SetParameter("body", body);
            packageReqHandler.SetParameter("out_trade_no", out_trade_no);//订单号
            packageReqHandler.SetParameter("total_fee", (int)(Convert.ToDecimal(total_fee) * 100) + ""); //金额,以分为单位
            packageReqHandler.SetParameter("spbill_create_ip", ip);//IP
            packageReqHandler.SetParameter("notify_url", wxPayConfig.notify_url); //回调地址
            packageReqHandler.SetParameter("trade_type", "NATIVE");//扫码支付
            packageReqHandler.SetParameter("product_id", product_id);//商品ID
            packageReqHandler.SetParameter("sign", packageReqHandler.CreateMd5Sign("key", wxPayConfig.key));//商户API密钥（签名）
            #endregion

            //将参数转为xml字符串
            string data = packageReqHandler.ParseXML();
            //发起post异步请求，获取返回的内容
            var result = PostWithStringFile(wxPayConfig.api_url, data);
            Log.Info("【GetWxSMPayUrl】订单：" + out_trade_no + ",请求得到的xml：" + result, "微信支付");

            //解析xml，获取扫码需要的mweb_url。
            var res = System.Xml.Linq.XDocument.Parse(result);
            try
            {
                string mweb_url = res.Element("xml").Element("code_url").Value;
                Log.Info("【GetWxSMPayUrl】订单：" + out_trade_no + ",请求得到的url：" + mweb_url, "微信支付");
                return mweb_url;
            }
            catch (Exception ex)
            {
                Log.Info("【GetWxSMPayUrl】订单：" + out_trade_no + ",异常:" + ex.ToString(), "微信支付");
                return "";
            }
        }

        /// <summary>
   /// 获取H5方式的手机微信支付地址，异常返回""
   /// </summary>
   /// <param name="out_trade_no">订单号</param>
   /// <param name="body">描述</param>
   /// <param name="total_fee">总价</param>
   /// <param name="ip">客户IP</param>
   /// <returns></returns>
   public string GetWxH5PayUrl(string out_trade_no, string body, string total_fee, string ip)
   {
       Senparc.Weixin.MP.TenPayLibV3.RequestHandler packageReqHandler = new Senparc.Weixin.MP.TenPayLibV3.RequestHandler();
       #region 构造请求参数
       packageReqHandler.SetParameter("appid", wxPayConfig.appid);//APPID
       packageReqHandler.SetParameter("mch_id", wxPayConfig.mchid);//商户号
       packageReqHandler.SetParameter("nonce_str", Senparc.Weixin.MP.TenPayLibV3.TenPayV3Util.GetNoncestr());
       packageReqHandler.SetParameter("body", body);
       packageReqHandler.SetParameter("out_trade_no", out_trade_no);//订单号
       packageReqHandler.SetParameter("total_fee", (int)(Convert.ToDecimal(total_fee) * 100) + ""); //金额,以分为单位
       packageReqHandler.SetParameter("spbill_create_ip", ip);//IP
       packageReqHandler.SetParameter("notify_url", wxPayConfig.notify_url); //回调地址
       packageReqHandler.SetParameter("trade_type", "MWEB");//这个不可以改。固定为Mweb
       packageReqHandler.SetParameter("sign", packageReqHandler.CreateMd5Sign("key", wxPayConfig.key));//商户API密钥
       #endregion
 
       //将参数转为xml字符串
       string data = packageReqHandler.ParseXML();
       //发起post异步请求，获取返回的内容
       var result = PostWithStringFile(wxPayConfig.api_url, data);
       Log.Info("【GetWxH5PayUrl】订单：" + out_trade_no + ",请求得到的xml：" + result, "微信支付");
 
       var res = System.Xml.Linq.XDocument.Parse(result);
       try
       {
           string mweb_url = res.Element("xml").Element("mweb_url").Value;
           Log.Info("【GetWxH5PayUrl】订单：" + out_trade_no + ",请求得到的url：" + mweb_url, "微信支付");
           return mweb_url;
       }
       catch (Exception ex)
       {
           Log.Info("【GetWxH5PayUrl】订单：" + out_trade_no + ",异常:" + ex.ToString(), "微信支付");
           return "";
       }
   }



    }
}
