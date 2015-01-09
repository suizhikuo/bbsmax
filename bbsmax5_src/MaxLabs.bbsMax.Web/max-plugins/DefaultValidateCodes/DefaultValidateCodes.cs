//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//


using MaxLabs.WebEngine.Plugin;
using MaxLabs.bbsMax.ValidateCodes;
using System.Drawing;
using System.Drawing.Text;
using System;
using System.Drawing.Imaging;
using System.IO;

namespace MaxLabs.bbsMax.Web.plugins
{
	public class DefaultValidateCodes : PluginBase
    {
        public override string DisplayName
        {
            get
            {
                return "默认验证码类型包";
            }
        }

        public override string Description
        {
            get
            {
                return "此插件用于提供系统默认的验证码类型";
            }
        }

		public override void Initialize()
		{
			ValidateCodeManager.RegisterValidateCodeType(new ValidateCode_Style13());
			ValidateCodeManager.RegisterValidateCodeType(new ValidateCode_Style14());
		}
	}

	public class ValidateCode_Style13 : ValidateCodeType
	{
		public override string Name
		{
			get { return "字体旋转(简单)"; }
		}

		public override byte[] CreateImage(out string resultCode)
		{
			string strFormat = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z";
			GetRandom(strFormat, 4, out resultCode);
			Bitmap bitmap;
			MemoryStream stream = new MemoryStream();

			ImageBmp(out bitmap, resultCode);
			bitmap.Save(stream, ImageFormat.Png);
			bitmap.Dispose();

			bitmap = null;
			stream.Close();
			stream.Dispose();

			return stream.GetBuffer();
		}


		#region 验证码美化
		bool fontTextRenderingHint = true;
		bool FontTextRenderingHint
		{
			get { return fontTextRenderingHint; }
			set { fontTextRenderingHint = value; }
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
			bitMap = new Bitmap(120, 30);
			DisposeImageBmp(ref bitMap);
			CreateImageBmp(ref bitMap, validataCode);
		}

		//绘制验证码图片
		void CreateImageBmp(ref Bitmap bitMap, string validateCode)
		{
			Graphics graphics = Graphics.FromImage(bitMap);
			if (this.fontTextRenderingHint)
				graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
			else
				graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
			Font font = new Font(this.validateCodeFont, this.validataCodeSize, FontStyle.Regular);
			Brush brush = new SolidBrush(this.drawColor);
			Random random = new Random();
			for (int i = 0; i < 4; i++)
			{
				Bitmap tmpB = new Bitmap(30, 30);
				Graphics tmpG = Graphics.FromImage(tmpB);
				tmpG.TranslateTransform(4, 0);
				tmpG.RotateTransform(random.Next(20));
				Point point = new Point(4, -2);
				tmpG.DrawString(validateCode[i].ToString(), font, brush, (PointF)point);
				graphics.DrawImage(tmpB, i * 30, 0);
				tmpG.Dispose();
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
			Point[] pointArray = new Point[2];

			pen = new Pen(ChaosColor, 1);
			for (int i = 0; i < 4 * 10; i++)
			{
				int x = random.Next(bitmap.Width);
				int y = random.Next(bitmap.Height);
				graphics.DrawRectangle(pen, x, y, 1, 1);
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

	public class ValidateCode_Style14 : ValidateCodeType
	{

		public override string Name
		{
			get { return "2年级算术(彩色)"; }
		}

		public override string Tip
		{
			get
			{
				return "请输入计算结果";
			}
		}

		public override byte[] CreateImage(out string resultCode)
		{
            int[] numbers = new int[] { 1,2,3,4,5,6,7,8,9,0};
			string validataCode;
            GetRandom(numbers, out validataCode, out resultCode);
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
			int width = (int)(((this.validataCodeLength * this.validataCodeSize) * 1.3) + 10);
			bitMap = new Bitmap(width, this.ImageHeight);
			DisposeImageBmp(ref bitMap,validataCode.Length);
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
			int maxValue = Math.Max((this.ImageHeight - this.validataCodeSize) - 5, 0);
			Random random = new Random();
			for (int i = 0; i < validateCode.Length; i++)
			{
				Brush brush = new SolidBrush(drawColors[random.Next(drawColors.Length)]);
				int[] numArray = new int[] { (i * this.validataCodeSize) + i * 5, random.Next(maxValue) };
				Point point = new Point(numArray[0], numArray[1]);
				graphics.DrawString(validateCode[i].ToString(), font, brush, (PointF)point);
			}
			graphics.Dispose();
		}

		//混淆验证码图片
		void DisposeImageBmp(ref Bitmap bitmap,int len)
		{
			Graphics graphics = Graphics.FromImage(bitmap);
			graphics.Clear(Color.White);

			Pen pen;
			Random random = new Random();
			Point[] pointArray = new Point[2];

			pen = new Pen(ChaosColor, 1);
            for (int i = 0; i < len * 10; i++)
			{
				int x = random.Next(bitmap.Width);
				int y = random.Next(bitmap.Height);
				graphics.DrawRectangle(pen, x, y, 1, 1);
			}
			graphics.Dispose();
		}


        private static void GetRandom(int[] numbers, out string codeString, out string resultCode)
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