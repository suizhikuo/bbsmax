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
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Emoticons
{
    #region Struct
    public class Strcut_CFCBlock
    {
        public uint MD5Length; //32
        public uint uintcutLength; //4
        public uint FaceNameLength; //4
        public uint FaceFileNameLength; //36 md5 + extension
        public uint FileLength;
        public uint ThumbnailFileNameLength; //41 md5 + fixed.bmp
        public uint ThumbnailFileLength;
        public uint FrameLength;
        public string MD5;
        public string uintcuts;
        public string FaceName;
        public string FaceFileName;
        public string ThumbnailFileName;
        public byte[] FaceData;
        public byte[] ThumbnailData;
        
        public static Strcut_CFCBlock FromImage(Emoticon emoticon)
        {
            return CFCBuilder.GetFaceBlockFromImage(emoticon);
        }

        byte[] GetBytes(uint value)
        {
            byte[] bt = BitConverter.GetBytes(value);
            List<byte> bytes = new List<byte>();
            bytes.AddRange(bt);
            if (bytes.Count < 4)
            {
                int l = 4 - bytes.Count;
                for (int i = 0; i < l; i++)
                    bytes.Add((byte)0);
            }
            return bytes.ToArray();
        }

        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();
            Encoding e = Encoding.ASCII;
            bytes.AddRange(GetBytes(MD5Length));
            bytes.AddRange(GetBytes(uintcutLength));
            bytes.AddRange(GetBytes(FaceNameLength));
            bytes.AddRange(GetBytes(FaceFileNameLength));
            bytes.AddRange(GetBytes(FileLength));
            bytes.AddRange(GetBytes(ThumbnailFileNameLength));
            bytes.AddRange(GetBytes(ThumbnailFileLength));
            bytes.AddRange(GetBytes(FrameLength));

            bytes.AddRange(e.GetBytes(MD5));
            bytes.AddRange(e.GetBytes(uintcuts));
            bytes.AddRange(e.GetBytes(FaceName));
            bytes.AddRange(e.GetBytes(FaceFileName));
            bytes.AddRange(e.GetBytes(ThumbnailFileName));

            bytes.AddRange(FaceData);
            bytes.AddRange(ThumbnailData);

            return bytes.ToArray();
        }
    }
    #endregion
}