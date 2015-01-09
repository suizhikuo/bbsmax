//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.IO;
using Gif.Components;
using System.Drawing;
using System.Drawing.Imaging;
using MaxLabs.bbsMax;

namespace Max.Installs
{
    public class AvatarBuilder
    {
        static string bigAvatarOutputPath;
        static string smallAvatarOutputPath;
        static string normalAvatarOutputPath;

        static readonly ImageCodecInfo jpegImageCodeInfo;

        static readonly Size bigAvatarSize = new Size(120, 120);
        static readonly Size smallAvatarSize = new Size(24, 24);
        static readonly Size normalAvatarSize = new Size(48, 48);

        static AvatarBuilder()
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == ImageFormat.Jpeg.Guid)
                {
                    jpegImageCodeInfo = codec;
                }
            }
        }

        public static void SetAvatarDirectory(string avatarDirectory)
        {
            bigAvatarOutputPath = MaxLabs.bbsMax.IOUtil.JoinPath(avatarDirectory, "Big");
            normalAvatarOutputPath = MaxLabs.bbsMax.IOUtil.JoinPath(avatarDirectory, "Default");
            smallAvatarOutputPath = MaxLabs.bbsMax.IOUtil.JoinPath(avatarDirectory, "Small");
        }

        public static void GenerateAvatar(int userID, string stringInDatabase)
        {
            string extension = Path.GetExtension(stringInDatabase);

            string fileName = SetupManager.GetAvatarFilename(userID, stringInDatabase);// userID + extension;
            string srcPath = SetupManager.GetSrcAvatarPath(userID, stringInDatabase);

            string tempFileName = fileName.Replace("\\", "/").Replace(userID + "/", "");

            string bigAvatarPath = IOUtil.JoinPath(bigAvatarOutputPath, tempFileName);
            string smallAvatarPath = IOUtil.JoinPath(smallAvatarOutputPath, tempFileName);
            string normalAvatarPath = IOUtil.JoinPath(normalAvatarOutputPath, tempFileName);

            string bigDir = bigAvatarPath.Replace("/", "\\");
            bigDir = bigDir.Substring(0, bigDir.LastIndexOf("\\"));

            string smallDir = smallAvatarPath.Replace("/", "\\");
            smallDir = smallDir.Substring(0, smallDir.LastIndexOf("\\"));

            string normalDir = normalAvatarPath.Replace("/", "\\");
            normalDir = normalDir.Substring(0, normalDir.LastIndexOf("\\"));

            if (Directory.Exists(bigDir) == false)
                Directory.CreateDirectory(bigDir);

            if (Directory.Exists(smallDir) == false)
                Directory.CreateDirectory(smallDir);

            if (Directory.Exists(normalDir) == false)
                Directory.CreateDirectory(normalDir);


            if (string.Compare(".gif", extension, true) == 0)
            {
                GifDecoder gifDecoder = new GifDecoder();

                if (gifDecoder.Read(srcPath) == GifDecoder.STATUS_OK)
                {
                    GenerateThumbGif(gifDecoder, smallAvatarPath, smallAvatarSize);
                    GenerateThumbGif(gifDecoder, normalAvatarPath, normalAvatarSize);
                    GenerateThumbGif(gifDecoder, bigAvatarPath, bigAvatarSize);
                }
                else
                {
                    using (Image image = Image.FromFile(srcPath))
                    {
                        GenerateThumb(image, smallAvatarPath, smallAvatarSize);
                        GenerateThumb(image, normalAvatarPath, normalAvatarSize);
                        GenerateThumb(image, bigAvatarPath, bigAvatarSize);
                    }
                }
            }
            else
            {
                using (Image image = Image.FromFile(srcPath))
                {
                    GenerateThumb(image, smallAvatarPath, smallAvatarSize);
                    GenerateThumb(image, normalAvatarPath, normalAvatarSize);
                    GenerateThumb(image, bigAvatarPath, bigAvatarSize);
                }
            }
        }

        private static void GenerateThumbGif(GifDecoder decoder, string thumbPath, Size thumbSize)
        {
            GifEncoder encoder = new GifEncoder();

            encoder.Start(thumbPath);

            encoder.SetSize(thumbSize.Width, thumbSize.Height);

            encoder.SetRepeat(decoder.GetLoopCount());

            Bitmap bitmap = new Bitmap(thumbSize.Width, thumbSize.Height);

            for (int i = 0; i < decoder.GetFrameCount(); i++)
            {
                encoder.SetDelay(decoder.GetDelay(i));

                int dispose = decoder.GetDispose(i);

                Color tranColor = decoder.GetTransparent(i);

                if (tranColor.IsEmpty == false)
                    encoder.SetTransparent(decoder.GetTransparent(i));

                if (dispose != 1)
                {
                    bitmap.Dispose();
                    bitmap = new Bitmap(thumbSize.Width, thumbSize.Height);
                }

                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    if (dispose == 2)
                    {
                        g.FillRectangle(new SolidBrush(tranColor), 0, 0, thumbSize.Width, thumbSize.Height);
                    }
                    //else if(dispose == 5)
                    //{
                    //    g.FillRectangle(new SolidBrush(Color.Black), 0, 0, thumbSize.Width, thumbSize.Height);
                    //}

                    g.DrawImage(decoder.GetFrame(i), 0, 0, thumbSize.Width, thumbSize.Height);
                }

                encoder.AddFrame(bitmap);
            }

            bitmap.Dispose();

            //using (Bitmap bitmap = new Bitmap(thumbSize.Width, thumbSize.Height))
            //{
            //    for (int i = 0; i < decoder.GetFrameCount(); i++)
            //    {
            //        encoder.SetDelay(decoder.GetDelay(i));

            //        Color tran = decoder.GetTransparent(i);

            //        using (Graphics g = Graphics.FromImage(bitmap))
            //        {
            //            if (tran != Color.Empty)
            //            {
            //                g.FillRectangle(new SolidBrush(tran), 0, 0, thumbSize.Width, thumbSize.Height);
            //            }
            //            else
            //            {

            //            }

            //            g.DrawImage(decoder.GetFrame(i), 0, 0, thumbSize.Width, thumbSize.Height);
            //        }

            //        encoder.AddFrame(bitmap);
            //    }
            //}

            encoder.Finish();
        }

        private static void GenerateThumb(Image image, string thumbPath, Size thumbSize)
        {
            using (Bitmap thumb = new Bitmap(image, thumbSize))
            {
                using (EncoderParameters encodeParams = new EncoderParameters())
                {
                    using (EncoderParameter encodeParam = new EncoderParameter(Encoder.Quality, 80L))
                    {
                        encodeParams.Param[0] = encodeParam;

                        thumb.Save(thumbPath, jpegImageCodeInfo, encodeParams);
                    }
                }
            }
        }
    }
}