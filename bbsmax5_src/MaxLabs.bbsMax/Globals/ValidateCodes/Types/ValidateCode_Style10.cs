//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Web;
using System.Drawing.Text;
using System.IO;
using System.Drawing.Imaging;
using System.Collections;

namespace MaxLabs.bbsMax.ValidateCodes
{

    public class ValidateCode_Style10 : ValidateCodeType
    {

        public override string Name
        {
            get { return "线条干扰(彩色)"; }
        }

        public override byte[] CreateImage(out string validataCode)
        {
            string strFormat = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z";
            GetRandom(strFormat, this.ValidataCodeLength, out validataCode);
            Bitmap bitmap;
            MemoryStream stream = new MemoryStream();

            ImageBmp(out bitmap, validataCode);
            bitmap.Save(stream, ImageFormat.Png);
            bitmap.Dispose();

            bitmap = null;
            stream.Close();
            stream.Dispose();

            return stream.GetBuffer();
        }
        #region 验证码美化
        bool fontTextRenderingHint = false;
        bool FontTextRenderingHint
        {
            get { return fontTextRenderingHint; }
            set { fontTextRenderingHint = value; }
        }
        #endregion

        #region 验证码长度
        int validataCodeLength = 4;
        public int ValidataCodeLength
        {
            get { return validataCodeLength; }
            set { validataCodeLength = value; }
        }
        #endregion

        #region 验证码字体大小(默认15像素，可以自行修改)
        int validataCodeSize = 16;
        public int ValidataCodeSize
        {
            get { return validataCodeSize; }
            set { validataCodeSize = value; }
        }
        #endregion

        #region 图片高度
        int imageHeight = 30;
        public int ImageHeight
        {
            get { return imageHeight; }
            set { imageHeight = value; }
        }
        #endregion

        #region 边框补(默认1像素)
        int padding = 1;
        public int Padding
        {
            get { return padding; }
            set { padding = value; }
        }
        #endregion

        #region 输出燥点的颜色
        Color chaosColor = Color.FromArgb(0xaa, 0xaa, 0x33);
        public Color ChaosColor
        {
            get { return chaosColor; }
            set { chaosColor = value; }
        }
        #endregion

        #region 自定义背景色(默认白色)
        Color backgroundColor = Color.White;
        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set { backgroundColor = value; }
        }
        #endregion

        #region 自定义颜色（要给验证码添加新颜色 在这加！！）
        Color[] drawColors = new Color[] { Color.FromArgb(0x6B, 0x42, 0x26), Color.FromArgb(0x4F, 0x2F, 0x4F), Color.FromArgb(0x32, 0x99, 0xcc), Color.FromArgb(0xCD, 0x7F, 0x32), Color.FromArgb(0x23, 0x23, 0x8E), Color.FromArgb(0x70, 0xDB, 0x93), Color.Red, Color.FromArgb(0xbc, 0x8f, 0x8E) };
        public Color[] DrawColors
        {
            get { return drawColors; }
            set { drawColors = value; }
        }
        #endregion

        #region 自定义字体
        string validateCodeFont = "Arial";
        public string ValidateCodeFont
        {
            get { return validateCodeFont; }
            set { validateCodeFont = value; }
        }
        #endregion

        #region CreateImage
        void ImageBmp(out Bitmap bitMap, string validataCode)
        {
            int width = (int)(((this.validataCodeLength * this.validataCodeSize) * 1.2));
            bitMap = new Bitmap(width, this.ImageHeight);
            DisposeImageBmp(ref bitMap);
            CreateImageBmp(ref bitMap, validataCode);
        }

        //绘制验证码图片
        void CreateImageBmp(ref Bitmap bitMap, string validateCode)
        {
            Graphics graphics = Graphics.FromImage(bitMap);
            Random random = new Random();

            if (this.fontTextRenderingHint)
                graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
            else
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            Font font = new Font(this.validateCodeFont, this.validataCodeSize, FontStyle.Regular);


            int maxValue = Math.Max((this.ImageHeight - this.validataCodeSize) - 5, 0);
            for (int i = 0; i < this.validataCodeLength; i++)
            {
                Color c = DrawColors[random.Next(DrawColors.Length)];
                Brush brush = new SolidBrush(c);
                int[] numArray = new int[] { (i * this.validataCodeSize) + random.Next(1) + 3, random.Next(maxValue) - 4 };
                System.Drawing.Point point = new System.Drawing.Point(numArray[0], numArray[1]);
                graphics.DrawString(validateCode[i].ToString(), font, brush, (PointF)point);
            }
            graphics.Dispose();
        }

        //混淆验证码图片
        void DisposeImageBmp(ref Bitmap bitmap)
        {
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);

            Random random = new Random();
            System.Drawing.Point[] pointArray = new System.Drawing.Point[2];

            Random rand = new Random();
            for (int i = 0; i < this.validataCodeLength * 2; i++)
            {
                Pen pen = new Pen(this.DrawColors[rand.Next(DrawColors.Length)], 1f);
                pointArray[0] = new System.Drawing.Point(rand.Next(bitmap.Width), rand.Next(bitmap.Height));
                pointArray[1] = new System.Drawing.Point(rand.Next(bitmap.Width), rand.Next(bitmap.Height));
                graphics.DrawLine(pen, pointArray[0], pointArray[1]);
            }
            graphics.Dispose();
        }


        //获取随机数
        private static void GetRandom(string formatString, int len, out string codeString)
        {
            int j1;
            codeString = string.Empty;
            string[] strResult = formatString.Split(new char[] { ',' });//把上面字符存入数组
            Random rnd = new Random();
            for (int i = 0; i < len; i++)
            {
                j1 = rnd.Next(100000) % strResult.Length;
                codeString = codeString + strResult[j1].ToString();
            }
        }

        #endregion
    }
}