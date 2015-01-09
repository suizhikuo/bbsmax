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
using System.Security.Cryptography;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax;

using MaxLabs.bbsMax.Common;


namespace MaxLabs.bbsMax.Emoticons
{
    public class CFCBuilder
    {
        internal static Strcut_CFCBlock GetFaceBlockFromImage(Emoticon emoticon)
        {
            string filePath = IOUtil.ResolvePath(emoticon.ImageSrc);
            Strcut_CFCBlock fb = new Strcut_CFCBlock();
            //���ļ���   

            //����ļ��Ƿ�ɾ��
            if (!IOUtil.ExistsFile(filePath))
                return null;


            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            byte[] faceData = new byte[fs.Length];

            fs.Read(faceData, 0, (int)fs.Length);
            //��ȡͼ��

            Image img;
            try
            {
                img = Image.FromStream(fs);
            }
            catch
            {
                fs.Close();
                return null;
            }
            
            
            //Image thumbnail = img.GetThumbnailImage(20, 20, null, IntPtr.Zero);
            MemoryStream ms = new MemoryStream();
            //����ͼͼת����byte����
            ImageHelper.CreateThunmbnailImage(img, ms, 20, 20, ImageFormat.Bmp);
            byte[] thumbnailData = ms.ToArray();
            ms.Close();
            ms.Dispose();
            //thumbnail.Dispose();

            //�õ�һ��Ψһ��MD5�ַ���
            string md5 = GetMD5(fs);
            fs.Close();
            //fs.Dispose();


            //�ļ�������ʽΪ:md5 + ��չ��
            string fileName = md5 + Path.GetExtension(emoticon.ImageSrc);
            //����ͼ�ļ�������ʽΪ��md5 + fixed.bmp
            string thumbnailName = string.Format("{0}fixed.bmp", md5);
            //����һ����ݼ�
            string uintcuts = emoticon.Shortcut;


            //ȡ���ܵ�֡��
            System.Drawing.Imaging.FrameDimension fd = System.Drawing.Imaging.FrameDimension.Resolution;
            int frameCount = img.FrameDimensionsList.Length;
            img.Dispose();

            fb.MD5 = md5;
            fb.MD5Length = (uint)md5.Length;
            fb.uintcuts = uintcuts;
            fb.uintcutLength = (uint)uintcuts.Length;
            fb.FaceName = uintcuts;
            fb.FaceNameLength = (uint)uintcuts.Length;
            fb.FaceFileName = fileName;
            fb.FaceFileNameLength = (uint)fileName.Length;
            fb.ThumbnailFileName = thumbnailName;
            fb.ThumbnailFileNameLength = (uint)thumbnailName.Length;
            fb.FaceData = File.ReadAllBytes(IOUtil.ResolvePath(emoticon.ImageSrc));
            fb.FileLength = (uint)fb.FaceData.Length;
            fb.ThumbnailData = thumbnailData;
            fb.ThumbnailFileLength = (uint)thumbnailData.Length;
            fb.FrameLength = (uint)frameCount;
            return fb;
        }

        #region Helper

        //���ļ������MD5
        internal static string GetMD5(FileStream fs)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] md5byte = md5.ComputeHash(fs);
            string str = string.Empty;
            int i, j;
            foreach (byte b in md5byte)
            {
                i = Convert.ToInt32(b);
                j = i >> 4;
                str += (Convert.ToString(j, 16));
                j = ((i << 4) & 0x00ff) >> 4;
                str += (Convert.ToString(j, 16));
            }
            return str.ToUpper();
        }
        #endregion

        ///// <summary>
        ///// ��һ��Ŀ¼����һ��CFC�ļ�����
        ///// </summary>
        ///// <param name="directory">Ŀ¼</param>
        ///// <param name="path">����CFC��·��(�����ļ���)</param>
        //public static void BuildCFCFileFromDirectory(string directory,string path)
        //{
        //    List<byte> bytes = new List<byte>();
        //    foreach (string file in Directory.GetFiles(directory))
        //    {
        //        if (!IsImageFile(file))
        //            continue;

        //        bytes.AddRange(Strcut_CFCBlock.FromImage(file).ToBytes());
        //    }
        //    FileStream fs = File.Create(path);
        //    fs.Write(bytes.ToArray(), 0, bytes.Count);
        //    fs.Close();
        //}

        /// <summary>
        /// ����һ��CFC�ļ�����
        /// </summary>
        /// <param name="emoticons"></param>
        /// <returns></returns>
        public static byte[] BuildCFCFileFromBytes(EmoticonCollection emoticons)
        {
            List<byte> bytes = new List<byte>();

            foreach (Emoticon emoticon in emoticons)
            {
                string path = IOUtil.ResolvePath(emoticon.ImageSrc);
                if (!FileHelper.IsPictureFile(path))
                    continue;
                Strcut_CFCBlock block = Strcut_CFCBlock.FromImage(emoticon);
                if (block != null)
                    bytes.AddRange(block.ToBytes());
            }
            return bytes.ToArray();
        }

        //�ж��Ƿ�Ϊͼ���ļ�
        //private static bool IsImageFile(string file)
        //{
        //    if (file == null)
        //        return false;

        //    file = file.Trim().ToLower();
        //    if (file.EndsWith(".gif") || file.EndsWith(".jpg") || file.EndsWith(".jpeg"))
        //        return true;
        //    else
        //        return false;
        //    //List<string> validExt = new List<string>(new string[]{
        //    //    ".jpg",
        //    //    ".gif",
        //    //});
        //    //return validExt.Contains(Path.GetExtension(file).ToLower());
        //}
    }
}