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
using System.Drawing;
using System.Drawing.Imaging;

namespace MaxLabs.bbsMax
{
    public class ImageUtil
    {
        private static ImageCodecInfo s_JpegImageCodeInfo = null;

        /// <summary>
        /// 根据最大宽带和最大高度生成等比缩放的缩略图（注意生成的图片不是固定尺寸的，只是限制了最大尺寸）
        /// </summary>
        /// <param name="sourceImagePath"></param>
        /// <param name="targetImagePath"></param>
        /// <param name="targetMaxWidth"></param>
        /// <param name="targetMaxHeight"></param>
        /// <returns></returns>
        public bool BuildThumbnailWithMaxSize(string sourceImagePath, string targetImagePath, int targetMaxWidth, int targetMaxHeight)
        {

            #region 生成文件路径，并确保文件夹存在

            string targetImageDirectory = Path.GetDirectoryName(targetImagePath);

            if (Directory.Exists(targetImageDirectory) == false)
                Directory.CreateDirectory(targetImageDirectory);

            #endregion

            if (File.Exists(targetImagePath) == false)
            {
                try
                {
                    using (Image image = Image.FromFile(sourceImagePath))
                    {

                        #region 根据图片比例生成缩略图尺寸

                        //得到等比压缩的宽高
                        float w, h;
                        if (image.Width <= targetMaxWidth && image.Height <= targetMaxHeight)
                        {
                            w = image.Width;
                            h = image.Height;
                        }
                        else
                        {
                            w = targetMaxWidth;
                            h = targetMaxHeight;

                            if (image.Width / w > image.Height / h)
                            {
                                //w = targetMaxWidth;
                                h = w * ((float)image.Height / (float)image.Width);
                            }
                            else
                            {
                                //h = targetMaxHeight;
                                w = h * ((float)image.Width / (float)image.Height);
                            }
                        }

                        #endregion

                        #region 生成缩略图并保存

                        using (Bitmap thumb = new Bitmap(image, (int)w, (int)h))
                        {
                            using (EncoderParameters encodeParams = new EncoderParameters())
                            {
                                using (EncoderParameter encodeParam = new EncoderParameter(Encoder.Quality, 80L))
                                {
                                    encodeParams.Param[0] = encodeParam;

                                    if (s_JpegImageCodeInfo == null)
                                    {
                                        ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

                                        foreach (ImageCodecInfo codec in codecs)
                                        {
                                            if (codec.FormatID == ImageFormat.Jpeg.Guid)
                                            {
                                                s_JpegImageCodeInfo = codec;
                                            }
                                        }
                                    }

                                    thumb.Save(targetImagePath, s_JpegImageCodeInfo, encodeParams);
                                }
                            }
                        }

                        #endregion
                    }

                    return true;
                }
                catch
                { }
            }

            return false;
        }
    }
}