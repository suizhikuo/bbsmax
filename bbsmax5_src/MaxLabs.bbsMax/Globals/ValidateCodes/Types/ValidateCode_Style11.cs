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

    public class ValidateCode_Style11 : ValidateCodeType
    {

        public override string Name
        {
            get { return "GIF闪烁动画(彩色)"; }
        }

        public override byte[] CreateImage(out string validataCode)
        {
            string strFormat = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z";
            GetRandom(strFormat, this.ValidataCodeLength, out validataCode);

            Bitmap bitmap;
            MemoryStream stream = new MemoryStream();

            AnimatedGifEncoder encoder = new AnimatedGifEncoder();
            encoder.Start();
            encoder.SetDelay(1);
            encoder.SetRepeat(0);

            Random rand = new Random();
            for (int i = 0; i < validataCode.Length; i++)
            {
                colors.Add(DrawColors[rand.Next(DrawColors.Length)]);
            }

            for (int i = 0; i < 3; i++)
            {
                string[] splitCode = SplitCode(validataCode);

                for (int j = 0; j < 2; j++)
                {
                    if (j == 0)
                        ImageBmp(out bitmap, splitCode[0]);
                    else
                        ImageBmp(out bitmap, splitCode[1]);
                    //    this.CreateImageBmp(out bitmap, splitCode[1]);
                    //this.DisposeImageBmp(ref bitmap);
                    bitmap.Save(stream, ImageFormat.Png);
                    //imageFrame.Save(string.Format("d://vali/{0}{1}.bmp", i, j));
                    encoder.AddFrame(System.Drawing.Image.FromStream(stream));
                    stream = new MemoryStream();
                    bitmap.Dispose();
                }
            }
            encoder.OutPut(ref stream);

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

        #region 验证码长度(默认4个验证码的长度)
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

        #region 是否输出燥点(默认不输出)
        bool chaos = true;
        public bool Chaos
        {
            get { return chaos; }
            set { chaos = value; }
        }
        #endregion

        #region 输出燥点的颜色(默认灰色)
        Color chaosColor = Color.FromArgb(0xaa, 0xaa, 0x33);
        public Color ChaosColor
        {
            get { return chaosColor; }
            set { chaosColor = value; }
        }
        #endregion

        #region 燥点模式(1：点式  2：块式  3：线式)
        int chaosMode = 1;
        public int ChaosMode
        {
            get { return chaosMode; }
            set { chaosMode = value; }
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

        #region 自定义颜色（要给验证码添加新颜色 在这加!!）
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
            int width = (int)(((this.validataCodeLength * this.validataCodeSize) * 1.3) + 4);
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
            int maxValue = Math.Max((this.ImageHeight - this.validataCodeSize) - 4, 0);
            Random random = new Random();
            for (int i = 0; i < this.validataCodeLength; i++)
            {
                Brush brush = new SolidBrush(colors[i]);
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

            Pen pen;
            Random random = new Random();
            System.Drawing.Point[] pointArray = new System.Drawing.Point[2];

            if (this.Chaos)
            {
                switch (this.chaosMode)
                {
                    case 1:
                        pen = new Pen(ChaosColor, 1);
                        for (int i = 0; i < this.validataCodeLength * 10; i++)
                        {
                            int x = random.Next(bitmap.Width);
                            int y = random.Next(bitmap.Height);
                            graphics.DrawRectangle(pen, x, y, 1, 1);
                        }
                        break;
                    case 2:
                        pen = new Pen(ChaosColor, this.validataCodeLength * 4);
                        for (int i = 0; i < this.validataCodeLength * 10; i++)
                        {
                            int x = random.Next(bitmap.Width);
                            int y = random.Next(bitmap.Height);
                            graphics.DrawRectangle(pen, x, y, 1, 1);
                        }
                        break;
                    case 3:
                        pen = new Pen(ChaosColor, 1);
                        for (int i = 0; i < this.validataCodeLength * 2; i++)
                        {
                            pointArray[0] = new System.Drawing.Point(random.Next(bitmap.Width), random.Next(bitmap.Height));
                            pointArray[1] = new System.Drawing.Point(random.Next(bitmap.Width), random.Next(bitmap.Height));
                            graphics.DrawLine(pen, pointArray[0], pointArray[1]);
                        }
                        break;
                    default:
                        pen = new Pen(ChaosColor, 1);
                        for (int i = 0; i < this.validataCodeLength * 10; i++)
                        {
                            int x = random.Next(bitmap.Width);
                            int y = random.Next(bitmap.Height);
                            graphics.DrawRectangle(pen, x, y, 1, 1);
                        }
                        break;
                }
            }

            graphics.Dispose();
        }

        List<Color> colors = new List<Color>();


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

        string[] SplitCode(string srcCode)
        {
            Random rand = new Random();
            string[] splitCode = new string[2];
            foreach (char c in srcCode)
            {
                if (rand.Next(Math.Abs((int)DateTimeUtil.Now.Ticks)) % 2 == 0)
                {
                    splitCode[0] += c.ToString();
                    splitCode[1] += " ";
                }
                else
                {
                    splitCode[1] += c.ToString();
                    splitCode[0] += " ";
                }
            }
            return splitCode;
        }
        #endregion
    }
}