using QRCoder;
using System;
using System.DrawingCore;
using System.DrawingCore.Imaging;

namespace WeChatPay.Pay
{
    public class QRCoderHelper
    {
           /// <summary>
        /// 生成二维码
        /// </summary>
        /// <returns></returns>
        public static Bitmap CreateQrcode(string codeToken, int version = 10)
        {
            EncoderParameter myEncoderParameter;
            EncoderParameters myEncoderParameters;
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            // 设置二维码排错率，可选L(7%)、M(15%)、Q(25%)、H(30%)，排错率越高可存储的信息越少，但对二维码清晰度的要求越小
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(codeToken, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            // 设置设置二维码版本，取值范围1-40，值越大尺寸越大，可存储的信息越大(实测9（297*297），10（330*330），20（660*600），每个挡位33左右，3个挡位100个像素)
            Bitmap qrCodeImage = qrCode.GetGraphic(version);
            Encoder myEncoder = Encoder.Quality;
            myEncoderParameters = new EncoderParameters(1);
            myEncoderParameter = new EncoderParameter(myEncoder, 25L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            return qrCodeImage;
        }

    }
}
