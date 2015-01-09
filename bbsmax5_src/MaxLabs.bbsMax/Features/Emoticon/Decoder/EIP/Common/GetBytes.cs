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

namespace MaxLabs.bbsMax.Emoticons
{
    class GetBytes
    {
        public static byte[] GetBigBlocksFromIndexs(byte[] data,List<int> Indexs)
        {
            byte[] temp = new byte[Indexs.Count * Struct_POIFSConstants.BIG_BLOCK_SIZE];
            int tempindex=0;
            for (int i = 0; i < Indexs.Count ; i++)
            {
                for (int j = 0; j < Struct_POIFSConstants.BIG_BLOCK_SIZE; j++)
                {
                    temp[tempindex] = data[(Indexs[i] + 1) * Struct_POIFSConstants.BIG_BLOCK_SIZE + j];
                    tempindex++;
                }
            }

            return temp;
        }

        public static byte[] GetHeaderBigBlocks(byte[] data)
        {
            List<byte> temp = new List<byte>();
            for (int i = 0; i < Struct_POIFSConstants.BIG_BLOCK_SIZE; i++)
            {
                temp.Add(data[i]);
            }

            //�ص����EIP̫��1���ͷ�������Ҫ���
            int nextHeader = BitConverter.ToInt32(temp.ToArray(), 0x44);// GetByte.GetByteToInt(temp.ToArray(), 0x44);
            if (nextHeader != -2)
            {
                for (int i = 0; i < Struct_POIFSConstants.BIG_BLOCK_SIZE; i++)
                {
                    temp.Add(data[(nextHeader + 1) * Struct_POIFSConstants.BIG_BLOCK_SIZE + i]);
                }
            }
            return temp.ToArray();
        }
    }
}