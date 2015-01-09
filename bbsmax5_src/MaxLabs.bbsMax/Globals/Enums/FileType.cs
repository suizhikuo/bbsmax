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

namespace MaxLabs.bbsMax.Enums
{
    /// <summary>
    /// 文件类别
    /// </summary>
    public enum FileType
    {

        /// <summary>
        /// 默认全部
        /// </summary>
        Default = 0,

        /// <summary>
        /// 图像
        /// </summary>
        Image =1,

        /// <summary>
        /// Flash
        /// </summary>
        Flash = 2,

        /// <summary>
        /// 音频
        /// </summary>
        Audio = 3,

        /// <summary>
        /// 视频
        /// </summary>
        Video = 4,

        /// <summary>
        /// 常用文档
        /// </summary>
        Document = 5,

        /// <summary>
        /// 压缩文档
        /// </summary>
        Zip = 6,

        /// <summary>
        /// 可执行程序
        /// </summary>
        Program = 7
    }

    //图片文件类型
    public enum ImageFileType : short
    {
        All = 0,
        GIF = 0x1,
        JPG = 0x2,
        PNG = 0x4,
        BMP = 0x8,
        PSD = 0x10,
        PCX = 0x20,
        RAS = 0x40,
        SGI = 0x80,
        TIFF= 0x100
    }
}