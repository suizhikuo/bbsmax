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

namespace MaxLabs.bbsMax.Util
{
    public class ByteManager
    {

        #region ��������
        public static byte[] wmaHeaderTag = new byte[16] { 0x30, 0x26, 0xB2, 0x75, 0x8E, 0x66, 0xCF, 0x11, 0xA6, 0xD9, 0x00, 0xAA, 0x00, 0x62, 0xCE, 0x6C };
        public static byte[] wmaInfoTag = new byte[16] { 0x33, 0x26, 0xB2, 0x75, 0x8E, 0x66, 0xCF, 0x11, 0xA6, 0xD9, 0x00, 0xAA, 0x00, 0x62, 0xCE, 0x6C };
        public static byte[] id3v1Tag = new byte[3] { 0x54, 0x41, 0x47 };
        public static byte[] id3v2Tag = new byte[3] { 0x49, 0x44, 0x33 };
        public static byte[] rmTag1 = new byte[4] { 0x2e, 0x52, 0x4d, 0x46 };
        public static byte[] rmTag2 = new byte[4] { 0x2e, 0x52, 0x4d, 0x53 };
        public static byte[] rm_field_CONT = new byte[4] { 0x43, 0x4f, 0x4e, 0x54 };


        public static int[, ,] bitrateTable = new int[,,] 
        {
           {
             {32 ,64, 96, 128, 160, 192, 224, 256, 288, 320, 352, 384, 416, 448},
             {32, 48, 56,  64,  80,  96, 112, 128, 160, 192, 224, 256, 320, 384},
             {32, 40, 48,  56,  64,  80,  96, 112, 128, 160, 192, 224, 256, 320}
            },
           {
             {32, 48, 56,  64,  80,  96, 112, 128, 144, 160, 176, 192, 224, 256},
             {8 , 16, 24,  32,  40,  48,  56,  64,  80,  96, 112, 128, 144, 160},
             {8 , 16, 24,  32,  40,  48,  56,  64,  80,  96, 112, 128, 144, 160}
            }
        };
        #endregion

        //����
        public static byte[] CopyByteArray(byte[] data, uint index, uint lenght)
        {
            byte[] bytes = new byte[lenght];
            if (index + lenght > data.Length)
                return bytes;
            for (int i = 0; i < lenght; i++)
            {
                bytes[i] = data[i + index];
            }
            return bytes;
        }

        public static byte[] CopyByteArray(byte[] data, int index, int lenght)
        {
            byte[] bytes = new byte[lenght];
            if (index + lenght > data.Length || index < 0)
                return bytes;
            for (int i = 0; i < lenght; i++)
            {
                bytes[i] = data[i + index];
            }
            return bytes;
        }

        //�Ƚ�
        public static bool CompareByteArray(byte[] data1, byte[] data2)
        {
            if (data1.Length != data2.Length)
                return false;
            for (int i = 0; i < data1.Length; i++)
            {
                if (data1[i] != data2[i])
                    return false;
            }
            return true;
        }

        public static uint ByteCovertToUint(byte[] data, uint index)
        {
            if (index + 4 > data.Length)
                return 0;
            int num = data[index + 3] + (data[index + 2] << 8) + (data[index + 1] << 16) + (data[index + 0] << 24);
            return Convert.ToUInt32(num);
        }
    }
}