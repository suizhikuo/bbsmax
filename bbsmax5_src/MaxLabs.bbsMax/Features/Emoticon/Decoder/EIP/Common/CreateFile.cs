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
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace MaxLabs.bbsMax.Emoticons
{
    class CreateFile
    {
        public static void Create(string path, byte[] data)
        {
            string dir=Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            if (!File.Exists(path))
                File.WriteAllBytes(path, data);
        }

        public static void Create(string path, Stream stream)
        {
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            if (!File.Exists(path))
                File.WriteAllBytes(path, buffer);
        }

        public static void Create(string path,  byte[] Data, int DataIndex, int DataSize)
        {
            string dir=Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            if (!File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Create);
                BinaryWriter w = new BinaryWriter(fs);
                w.Write(Data, DataIndex, DataSize);
                w.Close();
                fs.Dispose();
                fs.Close();
            }
        }

        public static void CreateNullFile(string path)
        {
            string dir=Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            if (!File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Create);
                fs.Dispose();
                fs.Close();
            }
        }


        /// <summary>
        /// ���ɸ�����������ͼ
        /// </summary>
        /// <param name="srcPath">ԭͼ·��</param>
        /// <param name="destPath">����ͼ·��</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="isCover">�Ƿ񸲸�</param>
        public static void CreateThunmbnailImage(string srcPath, string destPath, int width, int height, bool isCover)
        {
            if (!File.Exists(srcPath))
                return;
            if (File.Exists(destPath) && isCover == false)
                return;
            if (!Directory.Exists(Path.GetDirectoryName(destPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(destPath));
            Bitmap bmp = new Bitmap(width, height);
            Bitmap bmpSrc = new Bitmap(srcPath);
            //�õ��ȱ�ѹ���Ŀ��
            int w = width, h = height;
            if (bmpSrc.Width > bmpSrc.Height)
            {
                w = width;
                h = bmpSrc.Height * height / bmpSrc.Width;
            }
            else
            {
                h = height;
                w = bmpSrc.Width * width / bmpSrc.Height;
            }
            Graphics g = Graphics.FromImage(bmp);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.FillRectangle(new SolidBrush(Color.White), 0, 0, bmp.Width, bmp.Height);
            g.DrawImage(bmpSrc, (width - w) / 2, (height - h) / 2, w, h);
            g.Dispose();
            bmp.Save(destPath, ImageFormat.Gif);
            bmp.Dispose();
            bmpSrc.Dispose();
        }

        static int thunmbnailImageWidth = 24;
        static int thunmbnailImageHeight = 24;
        public static void CreateThunmbnailImage(string srcPath, string destPath, bool isCover)
        {
            if (!File.Exists(srcPath))
                return;
            if (File.Exists(destPath) && isCover == false)
                return;
            if (!Directory.Exists(Path.GetDirectoryName(destPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(destPath));
            Bitmap bmp = new Bitmap(thunmbnailImageWidth, thunmbnailImageHeight);
            Bitmap bmpSrc = new Bitmap(srcPath);
            //�õ��ȱ�ѹ���Ŀ��
            int w = thunmbnailImageWidth, h = thunmbnailImageHeight;
            if (bmpSrc.Width > bmpSrc.Height)
            {
                w = thunmbnailImageWidth;
                h = bmpSrc.Height * thunmbnailImageHeight / bmpSrc.Width;
            }
            else
            {
                h = thunmbnailImageHeight;
                w = bmpSrc.Width * thunmbnailImageWidth / bmpSrc.Height;
            }
            Graphics g = Graphics.FromImage(bmp);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.FillRectangle(new SolidBrush(Color.White), 0, 0, bmp.Width, bmp.Height);
            g.DrawImage(bmpSrc, (thunmbnailImageWidth - w) / 2, (thunmbnailImageHeight - h) / 2, w, h);
            g.Dispose();
            bmp.Save(destPath, ImageFormat.Gif);
            bmp.Dispose();
            bmpSrc.Dispose();
        }
    }
}