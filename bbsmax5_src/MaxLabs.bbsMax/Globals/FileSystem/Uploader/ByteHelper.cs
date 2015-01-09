//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace MaxLabs.bbsMax.FileSystem
//{
//    /// <summary>
//    /// 提供字节相关的便捷操作
//    /// </summary>
//    public class ByteHelper
//    {
//        /// <summary>
//        /// 合并字节数组
//        /// </summary>
//        /// <param name="array1">数组1</param>
//        /// <param name="array2">数组2</param>
//        /// <returns></returns>
//        public static byte[] MergeArrays(byte[] array1, byte[] array2)
//        {
//            int totalLength = 0;

//            if (array1 != null)
//                totalLength += array1.Length;

//            if (array2 != null)
//                totalLength += array2.Length;

//            byte[] newArray = new byte[totalLength];

//            if (array1 != null)
//            {
//                array1.CopyTo(newArray, 0);

//                if (array2 != null)
//                    array2.CopyTo(newArray, array1.Length);

//            }
//            else if (array2 != null)
//            {
//                array2.CopyTo(newArray, 0);
//            }

//            return newArray;
//        }

//        /// <summary>
//        /// 在给定的字节数组中搜索目标字节数组
//        /// </summary>
//        /// <param name="searchFor">要搜索的字节数组</param>
//        /// <param name="theArray">目标字节数组</param>
//        /// <param name="startIndex">搜索的起始位置</param>
//        /// <returns>最先匹配的位置，或-1</returns>
//        public static int SearchBytesInArray(byte[] searchFor, byte[] theArray, int startIndex)
//        {

//            for (int i = startIndex; i < theArray.Length; i++)
//            {
//                //剩余的字节长度如果小1于要搜索的字节长度就没必要搜索了
//                if (theArray.Length - i < searchFor.Length)
//                    break;

//                //第一个字节不匹配后面也没必要搜索了
//                if (theArray[i] != searchFor[0])
//                    continue;

//                int dataIndex = i;
//                bool found = true;

//                for (int j = 1; j < searchFor.Length; j++)
//                {
//                    dataIndex++;

//                    if (theArray[dataIndex] != searchFor[j])
//                    {
//                        found = false;
//                        break;
//                    }
//                }

//                if (found)
//                    return i;
//            }

//            return -1;
//        }
//    }
//}