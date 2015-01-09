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
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;

namespace MaxLabs.bbsMax.Common
{
    public class ImageHelper
    {
        /// <summary>
        /// ���ɸ�����������ͼ
        /// </summary>
        /// <param name="srcPath">ԭͼ·��</param>
        /// <param name="destPath">����ͼ·��</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        ///  <param name="imageFormat">ͼƬ�Ĵ洢��ʽ</param>
        /// <param name="isCover">�Ƿ񸲸�</param>
        public static void CreateThunmbnailImage(string srcPath, string destPath, int width, int height, ImageFormat imageFormat,bool isCover)
        {
            if (!File.Exists(srcPath))
                return;
            if (File.Exists(destPath) && isCover == false)
                return;
            if (!Directory.Exists(Path.GetDirectoryName(destPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(destPath));

            //Bitmap bmp = new Bitmap(width, height);
            //Bitmap bmpSrc = new Bitmap(srcPath);
            ////�õ��ȱ�ѹ���Ŀ��
            //float w = width, h = height;
            //if (bmpSrc.Width / w > bmpSrc.Height / h)
            //{
            //    w = width;
            //    h = w * ((float)bmpSrc.Height/(float)bmpSrc.Width);
            //}
            //else
            //{
            //    h = height;
            //    w = h * ((float)bmpSrc.Width / (float)bmpSrc.Height);
            //}
            //Graphics g = Graphics.FromImage(bmp);
            //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            //g.FillRectangle(new SolidBrush(Color.White), 0, 0, bmp.Width, bmp.Height);
            //g.DrawImage(bmpSrc, (width - w) / 2, (height - h) / 2, w, h);
            //g.Dispose();
            Bitmap bmp = GetThunmbnailImage(srcPath,width,height);
            bmp.Save(destPath, imageFormat);
            bmp.Dispose();
            //bmpSrc.Dispose();
        }



        /// <summary>
        /// ���ɸ�����������ͼ
        /// </summary>
        /// <param name="srcPath">ԭͼ·��</param>
        /// <param name="destPath">����ͼ·��</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        ///  <param name="imageFormat">ͼƬ�Ĵ洢��ʽ</param>
        /// <param name="isCover">�Ƿ񸲸�</param>
        public static Bitmap GetThunmbnailImage(string srcPath, int width, int height)
        {
            if (!File.Exists(srcPath))
                return null;

            Bitmap bmp = new Bitmap(width, height);
            Bitmap bmpSrc = new Bitmap(srcPath);
            //�õ��ȱ�ѹ���Ŀ��
            float w = width, h = height;
            if (bmpSrc.Width / w > bmpSrc.Height / h)
            {
                w = width;
                h = w * ((float)bmpSrc.Height / (float)bmpSrc.Width);
            }
            else
            {
                h = height;
                w = h * ((float)bmpSrc.Width / (float)bmpSrc.Height);
            }
            Graphics g = Graphics.FromImage(bmp);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            g.FillRectangle(new SolidBrush(Color.Transparent), 0, 0, bmp.Width, bmp.Height);
            g.DrawImage(bmpSrc, (width - w) / 2, (height - h) / 2, w, h);
            g.Dispose();
            bmpSrc.Dispose();
            return bmp;
            //bmp.Save(destPath, imageFormat);
            //bmp.Dispose();
            //bmpSrc.Dispose();
        }

        public static void CreateThunmbnailImage(Image image, Stream destPath, int width, int height, ImageFormat imageFormat)
        {
            Bitmap bmp = new Bitmap(width, height);
            //�õ��ȱ�ѹ���Ŀ��
            float w = width, h = height;
            if (image.Width / w > image.Height / h)
            {
                w = width;
                h = w * ((float)image.Height / (float)image.Width);
            }
            else
            {
                h = height;
                w = h * ((float)image.Width / (float)image.Height);
            }
            Graphics g = Graphics.FromImage(bmp);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.FillRectangle(new SolidBrush(Color.White), 0, 0, bmp.Width, bmp.Height);
            g.DrawImage(image, (width - w) / 2, (height - h) / 2, w, h);
            g.Dispose();
            bmp.Save(destPath, imageFormat);
            bmp.Dispose();
        }



        public static bool TextMark(string srcPath, string text, int markx, int marky, bool bold, Color textColor, int FontSize, FontFamily fontFamily, string outPath)
        {
            Image img = Image.FromFile(srcPath);
            //�������жϸ�ͼƬ�Ƿ��� gif���������Ϊgif��������ͼƬ���иĶ�
            foreach (Guid guid in img.FrameDimensionsList)
            {
                FrameDimension dimension = new FrameDimension(guid);
                //if (img.GetFrameCount(dimension) > gifMaxFrame)
                //{
                //    return false;
                //}
                //else if (img.GetFrameCount(dimension) > 1)
                //{
                //    Bitmap bit = new Bitmap(srcPath);
                //    return WaterMarkWithText(bit, text, outPath);
                //}
                if (img.GetFrameCount(dimension) > 1) //GIF����
                    return false;
            }
            try
            {
                //����ԴͼƬ�����µ�Bitmap������Ϊ��ͼ����Ϊ�˸�gifͼƬ���ˮӡ�����д�����
                Bitmap newBitmap = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
                //�����½�λͼ�÷ֱ���
                newBitmap.SetResolution(img.HorizontalResolution, img.VerticalResolution);
                //����Graphics�����ԶԸ�λͼ���в���
                Graphics g = Graphics.FromImage(newBitmap);
                //�������
                g.SmoothingMode = SmoothingMode.AntiAlias;
                //��ԭͼ��������ͼ��
                g.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel);
                //�����������
                Font cFont = null;
                //��������ˮӡ�ı����ȵó���
                SizeF size = new SizeF();
                //̽���һ���ʺ�ͼƬ��С�������С������Ӧˮӡ���ִ�С������Ӧ
                for (int i = 0; i < 6; i++)
                {
                    //����һ���������
                    cFont = new Font(fontFamily, FontSize);
                    //�Ƿ�Ӵ�
                    if (!bold)
                    {
                        cFont = new Font(fontFamily, FontSize, FontStyle.Regular);
                    }
                    else
                    {
                        cFont = new Font(fontFamily, FontSize, FontStyle.Bold);
                    }
                    //�����ı���С
                    size = g.MeasureString(text, cFont);
                    //ƥ���һ������Ҫ��������С
                    if ((ushort)size.Width < (ushort)img.Width)
                    {
                        break;
                    }
                }
                //����ˢ�Ӷ���׼����ͼƬд������
                Brush brush = new SolidBrush(textColor);
                //��ָ����λ��д������
                g.DrawString(text, cFont, brush, markx, marky);
                //�ͷ�Graphics����
                g.Dispose();
                //�����ɵ�ͼƬ����MemoryStream
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                newBitmap.Save(ms, ImageFormat.Jpeg);
                //��������Image����
                img = System.Drawing.Image.FromStream(ms);
                img.Save(outPath);
                //�����µ�Image����
                return true;


            }
            catch
            { return false; }
        }


        public static bool ImageMark(Stream srcImageStream, Image markImg, int markx, int marky, float transparence, string outPath)
        {
            return ImageMark(Image.FromStream(srcImageStream),markImg,markx,marky,transparence,outPath);
        }
        /// <summary>
        /// ͼƬˮӡ
        /// </summary>
        /// <param name="srcPath">ԭͼ·��</param>
        /// <param name="waterImg">ˮӡͼ</param>
        /// <param name="markx">X����</param>
        /// <param name="marky">Y����</param>
        /// <param name="transparence">͸����</param>
        /// <param name="outPath">ˮӡ���·��</param>
        /// <returns>�Ƿ�ɹ�ˮӡ</returns>
        public static bool ImageMark(Image img, Image markImg, int markx, int marky, float transparence, string outPath)
        {
            //Image img = Image.FromFile(srcPath);
            //�������жϸ�ͼƬ�Ƿ��� gif���������Ϊgif��������ͼƬ���иĶ�
            foreach (Guid guid in img.FrameDimensionsList)
            {
                FrameDimension dimension = new FrameDimension(guid);
                if (img.GetFrameCount(dimension) > 1)
                {
                    return false;
                }
            }
            try
            {
                //���ˮӡͼ��
                //Image markImg = waterImg;
                //������ɫ����
                float[][] ptsArray ={ 
                                                                  new float[] {1, 0, 0, 0, 0},
                                                                  new float[] {0, 1, 0, 0, 0},
                                                                  new float[] {0, 0, 1, 0, 0},
                                                                  new float[] {0, 0, 0, transparence, 0}, //ע�⣺�˴�Ϊ0.0fΪ��ȫ͸����1.0fΪ��ȫ��͸��
                                                                  new float[] {0, 0, 0, 0, 1}};
                ColorMatrix colorMatrix = new ColorMatrix(ptsArray);
                //�½�һ��Image����
                ImageAttributes imageAttributes = new ImageAttributes();
                //����ɫ������ӵ�����
                imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default,
                      ColorAdjustType.Default);
                //����λͼ��ͼ��
                Bitmap newBitmap = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
                //���÷ֱ���
                newBitmap.SetResolution(img.HorizontalResolution, img.VerticalResolution);
                //����Graphics
                Graphics g = Graphics.FromImage(newBitmap);
                //�������
                g.SmoothingMode = SmoothingMode.AntiAlias;
                //����ԭͼ����ͼ��
                g.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel);
                //���ԭͼ��С
                if (markImg.Width > img.Width || markImg.Height > img.Height)
                {
                    System.Drawing.Image.GetThumbnailImageAbort callb = null;
                    //��ˮӡͼƬ��������ͼ,��С��ԭͼ��1/4
                    System.Drawing.Image new_img = markImg.GetThumbnailImage(img.Width / 4, markImg.Height * img.Width / markImg.Width, callb, new System.IntPtr());
                    //���ˮӡ
                    g.DrawImage(new_img, new Rectangle(markx, marky, new_img.Width, new_img.Height), 0, 0, new_img.Width, new_img.Height, GraphicsUnit.Pixel, imageAttributes);
                    //�ͷ�����ͼ
                    new_img.Dispose();
                    //�ͷ�Graphics
                    g.Dispose();
                    img.Dispose();

                    newBitmap.Save(outPath);
                    return true;
                }
                //ԭͼ�㹻��
                else
                {
                    //���ˮӡ
                    g.DrawImage(markImg, new Rectangle(markx, marky, markImg.Width, markImg.Height), 0, 0, markImg.Width, markImg.Height, GraphicsUnit.Pixel, imageAttributes);
                    //�ͷ�Graphics
                    g.Dispose();
                    img.Dispose();

                    newBitmap.Save(outPath);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /*
        /// <summary>
        /// �ϲ�ͼƬ��ֻ����20*20Сͼ��
        /// </summary>
        /// <param name="destPath">����ͼ·��</param>
        /// <param name="imagePaths">Сͼ·��</param>
        /// <returns></returns>
        public static bool ImageUnite(string destPath, List<string> imagePaths, int width, int height)
        {
            try
            {
                Bitmap bitmap = new Bitmap(imagePaths.Count * width, height);
                Graphics g = Graphics.FromImage(bitmap);
                g.Clear(Color.White);
                int i = 0;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
                foreach (string fileName in imagePaths)
                {
                    //string newFileName = System.Text.RegularExpressions.Regex.Replace(fileName,@"\.(gif|jpg)",".png",RegexOptions.IgnoreCase);
                    Bitmap b1 = new Bitmap(fileName);
                    //string tempFile = null;
                    if (b1.Width > width || b1.Height > height)
                    {
                        //continue;
                        //TODO : �޸�Ϊ�ȱ�����С����ͼ
                    }
                    //else
                    //{
                    g.DrawImage(b1, i * width, 0);
                    //}
                    i++;
                }
                bitmap.Save(destPath, ImageFormat.Png);

                return true;
            }
            catch
            {
                return false;
            }
        }
        */

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="destPath">����ͼ·��</param>
        /// <param name="imagePaths">Сͼ·��</param>
        /// <returns></returns>
        public static bool ImageUnite(string destPath, List<string> imagePaths, ImageFormat imageFormat, Color backgroundColor, int width, int height)
        {
            try
            {
                Bitmap bitmap = new Bitmap(imagePaths.Count * width, height);
                Graphics g = Graphics.FromImage(bitmap);
                g.Clear(backgroundColor);
                int i = 0;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.CompositingQuality = CompositingQuality.HighQuality;
                foreach (string fileName in imagePaths)
                {
                    //string newFileName = System.Text.RegularExpressions.Regex.Replace(fileName,@"\.(gif|jpg)",".png",RegexOptions.IgnoreCase);
                    Bitmap tempImage;
                    string tempfileName;
                    try
                    {
                        tempImage = new Bitmap(fileName);
                        //b1 = Image.FromFile(fileName);
                        tempfileName = fileName;
                        //string tempFile = null;
                    }
                    catch
                    {
                        tempImage = new Bitmap(Globals.CommonImagePath + "/notImage.png");
                        tempfileName = Globals.CommonImagePath + "/notImage.png";
                        //tempFile = "";
                    }
                    if (tempImage.Width > width || tempImage.Height > height)
                    {
                        tempImage = GetThunmbnailImage(tempfileName, width, height);
                    }
                    int x = 0, y = 0;
                    //��������ͼ��λ��
                    if (tempImage.Width < width)//��ͼƬ���С��ָ�����ʱ ʹ��λ���м�
                    {
                        x = (width - tempImage.Width) / 2;
                    }
                    if (tempImage.Height < height)
                    {
                        y = (height - tempImage.Height) / 2;
                    }

                    g.DrawImage(tempImage, i * width + x, 0 + y, tempImage.Width, tempImage.Height);
                    i++;
                }
                bitmap.Save(destPath, imageFormat);

                return true;
            }
            catch
            {
                return false;
            }
        }

		public static bool ImageUnite(string destPath, string imagePath, ImageFormat imageFormat, Color backgroundColor, int width, int height)
		{
			try
			{
				Bitmap bitmap = new Bitmap(width, height);
				Graphics g = Graphics.FromImage(bitmap);
				g.Clear(backgroundColor);

				g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
				g.CompositingQuality = CompositingQuality.HighQuality;

				Bitmap tempImage;
				string tempfileName;
				try
				{
					tempImage = new Bitmap(imagePath);
					//b1 = Image.FromFile(fileName);
					tempfileName = imagePath;
					//string tempFile = null;
				}
				catch
				{
					tempImage = new Bitmap(Globals.CommonImagePath + "/notImage.png");
					tempfileName = Globals.CommonImagePath + "/notImage.png";
					//tempFile = "";
				}

				if (tempImage.Width > width || tempImage.Height > height)
				{
					tempImage = GetThunmbnailImage(tempfileName, width, height);
				}
				int x = 0, y = 0;
				//��������ͼ��λ��
				if (tempImage.Width < width)//��ͼƬ���С��ָ�����ʱ ʹ��λ���м�
				{
					x = (width - tempImage.Width) / 2;
				}
				if (tempImage.Height < height)
				{
					y = (height - tempImage.Height) / 2;
				}

				g.DrawImage(tempImage, x, y, tempImage.Width, tempImage.Height);

				bitmap.Save(destPath, imageFormat);

				return true;
			}
			catch
			{
				return false;
			}
		}
        */

        //public static Stream Get
    }
}