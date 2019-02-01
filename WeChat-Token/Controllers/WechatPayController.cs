using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.MP.TenPayLib;
using Sky.Logger;
using System;
using System.IO;
using System.Linq;
using WeChat_Token.WeChat;
using WeChatPay.Pay;

namespace WeChat_Token.Controllers
{
    public class WechatPayController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// ajax请求生成订单，插入订单到数据库，
        /// </summary>
        /// <param name="body"></param>
        /// <param name="total_fee"></param>
        /// <param name="product_id"></param>
        /// <returns></returns>
        public IActionResult GetWxSMPayUrl(string body, string total_fee, string product_id)
        {
            string no = DateTime.Now.ToString("yyyyMMddHHmmssfff");//构造订单号                       
            //订单相关逻辑代码

            //订单相关逻辑代码结束

            //构造支付地址信息
            WxPayService wxPayService = new WxPayService(); //服务类，自行优化
            //获取请求ip
            var ip = Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = HttpContext.Connection.RemoteIpAddress.ToString();
            }
            string code_url = wxPayService.GetWxSMPayUrl(no, body, total_fee, ip, product_id);
            return Content(code_url); //返回支付的Url，前端ajax请求得到该url后，将该url赋值到存放图片的src中
        }
        //
        public FileResult MakeQRCode(string data)
        {
            var image = QRCoderHelper.CreateQrcode(data);
            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.DrawingCore.Imaging.ImageFormat.Jpeg);
            return File(ms.ToArray(), "image/jpeg");
        }

        /// <summary>
        /// 微信支付异步回调
        /// </summary>
        /// <returns></returns>
        public string WxNotify()
        {
            try
            {
                //使用微信工具获取ResponseHandler
                ResponseHandler wxResponseHandler = new ResponseHandler(HttpContext);
                string out_trade_no = wxResponseHandler.GetParameter("1010");//订单号
                string total_fee = wxResponseHandler.GetParameter("total_fee");//订单金额，单位分
                total_fee = (Convert.ToDecimal(total_fee) / 100).ToString("9.00");//订单金额，单位元
                Console.WriteLine("微信测试收到数据，订单号：" + out_trade_no + "订单金额：" + total_fee, "【微信支付回调】");

                //验证订单是否有支付过逻辑

                //验证订单信息，获取支付配置
                WxPayConfig payConfigModel = new WxPayConfig();//后面去可以去配置或者数据库中获取<br>
                //验证是否通过微信安全认证
                WxPayService wxPayService = new WxPayService();
                //还没开始写
                bool vxCheck = wxPayService.WxPayCheck(wxResponseHandler);//使用sdk去验证
                if (vxCheck)
                {
                    //更新订单
                    //ProcessOrder(out_trade_no);
                    Log.Info("微信验证成功" + out_trade_no, "【微信支付回调】");
                    return "成功";
                }
                else
                {
                    Log.Info("微信测试失败" + out_trade_no, "【微信支付回调】");
                    return "微信测试失败";
                }
            }
            catch (Exception ex)
            {
                Log.Error("微信测试回调异常", ex, "【微信支付回调】");
                return "微信测试回调异常";
            }

        }

        /// <summary>
        /// 获取微信openid
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetWxOpenid(string code)
        {
            WxPayConfig wxPayConfig = new WxPayConfig();
            //首先构造微信请求授权url，重定向到该url中，其中redirectUrl是回调地址，必须保证该地址在公网上能访问，本例子构造的是返回到本控制器的方法。
            //若是从微信授权返回进入该方法，则会带上一个参数，code就不为空，若code为空则跳转微信授权
            if (string.IsNullOrEmpty(code))
            {
                var redirectUrl = OAuth.GetAuthorizeUrl(wxPayConfig.appid, wxPayConfig.notify_url);
                return Redirect(redirectUrl);
            }
            else
            {
                //根据code和微信参数得到openid
                var openId = OAuth.GetOpenid(wxPayConfig.appid, wxPayConfig.appSecret, code);
                //业务处理
            }
            return View();
        }


    }
}