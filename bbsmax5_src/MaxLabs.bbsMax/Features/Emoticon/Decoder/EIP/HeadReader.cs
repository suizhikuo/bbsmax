//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Collections.Generic;
/// <summary>
/// EipHeader 的摘要说明
/// </summary>
namespace MaxLabs.bbsMax.Emoticons
{
    public class Header
    {
        public Header(byte[] Header)
        {
            property_start = BitConverter.ToInt32(Header, Struct_Header_Offset.property_start);
            sbat_start = BitConverter.ToInt32(Header, Struct_Header_Offset.sbat_start);
            sbat_block_count = BitConverter.ToInt32(Header, Struct_Header_Offset.sbat_block_count);
            bat_count =  BitConverter.ToInt32(Header, Struct_Header_Offset.bat_count);
            for (int i = 0; i < bat_count; i++)
                bat_array.Add(BitConverter.ToInt32(Header, Struct_Header_Offset.bat_array + i * Struct_TypeSize.INT_SIZE));
            xbat_start = BitConverter.ToInt32(Header, Struct_Header_Offset.xbat_start);
            xbat_count = BitConverter.ToInt32(Header, Struct_Header_Offset.xbat_count);
        }

        //bool signature;       //是否是OFFICE文档
        int property_start;           //配置表开始索引
        int sbat_start;               //小块配置表开始索引
        int sbat_block_count;         //小块配置表数量
        List<int> bat_array=new List<int>();              //BAT索引数组
        int bat_count;                //BAT数量
        int xbat_start;    //扩展块开始索引
        int xbat_count;    //扩张块数量

        public int Property_start
        {
            get { return property_start; }
        }
        public int Sbat_start
        {
            get { return sbat_start; }
        }
        public int Sbat_block_count
        {
            get { return sbat_block_count; }
        }
        public int Bat_count
        {
            get { return bat_count; }
        }
        public List<int> Bat_array
        {
            get { return bat_array; }
        }
        public int Xbat_start
        {
            get { return xbat_start; }
        }
        public int Xbat_count
        {
            get { return xbat_count; }
        }
    }
}