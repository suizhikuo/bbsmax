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

    public class ValidateCode_Style8 : ValidateCodeType
    {

        public override string Name
        {
            get { return "2年级算术(蓝色)"; }
        }

        public override string Tip
        {
            get
            {
                return "输入计算结果";
            }
        }

        public override byte[] CreateImage(out string resultCode)
        {
            
            string validataCode;
            GetRandom(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }, out validataCode, out resultCode);
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
        int validataCodeLength = 5;
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

        #region 自定义颜色
        Color drawColor = Color.FromArgb(0x32, 0x99, 0xcc);
        public Color DrawColor
        {
            get { return drawColor; }
            set { drawColor = value; }
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
            int width = (int)(((validataCode.Length * this.validataCodeSize) * 1.3) + 10);
            bitMap = new Bitmap(width, this.ImageHeight);
            DisposeImageBmp(ref bitMap);
            CreateImageBmp(ref bitMap, validataCode);
        }

        //绘制验证码图片
        void CreateImageBmp(ref Bitmap bitMap, string validateCode)
        {
            Graphics graphics = Graphics.FromImage(bitMap);
            if (this.fontTextRenderingHint)
                graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
            else
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            Font font = new Font(this.validateCodeFont, this.validataCodeSize, FontStyle.Regular);
            Brush brush = new SolidBrush(this.drawColor);
            int maxValue = Math.Max((this.ImageHeight - this.validataCodeSize) - 5, 0);
            Random random = new Random();
            for (int i = 0; i < validateCode.Length; i++)
            {
                int[] numArray = new int[] { (i * this.validataCodeSize) + i * 5, random.Next(maxValue) };
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

            Pen pen = new Pen(this.DrawColor, 1f);
            Random random = new Random();
            System.Drawing.Point[] pointArray = new System.Drawing.Point[2];

            pen = new Pen(ChaosColor, 1);
            for (int i = 0; i < this.validataCodeLength * 10; i++)
            {
                int x = random.Next(bitmap.Width);
                int y = random.Next(bitmap.Height);
                graphics.DrawRectangle(pen, x, y, 1, 1);
            }
            graphics.Dispose();
        }


        private static void GetRandom(int[] numbers , out string codeString, out string resultCode)
        {
            Random rnd = new Random();
            int num1 = 0, num2 = 0;

        
            for (int i = 0; i < 2; i++)
            {
                int j1 = rnd.Next(numbers.Length);
                if (i == 0 && numbers[j1] == 0)
                {
                    i--;
                    continue;
                }
                num1 += i * 10 + numbers[j1];
            }
            for (int i = 0; i < 2; i++)
            {
                int j1 = rnd.Next(numbers.Length);
                if (i == 0 && numbers[j1] == 0)
                {
                    i--;
                    continue;
                }
                num2 += numbers[j1];
            }

            if (rnd.Next(100) % 2 == 1)
            {
                codeString = num1 + "加" + num2;
                resultCode = (num1 + num2).ToString();
            }
            else
            {
                if (num2 < num1)
                {
                    int temp = num1;
                    num1 = num2;
                    num2 = temp;
                }

                int g1, g2;
                g1 = num1 % 10;
                g2 = num2 % 10;

                if (g1 > g2)
                {
                    num1 -= g1;
                    g1 = g2 / 2;
                    num1 += g1;
                }

                codeString = num2 + "减" + num1;
                resultCode = (Math.Abs(num2 - num1)).ToString();
            }
        }
        #endregion
    }
}