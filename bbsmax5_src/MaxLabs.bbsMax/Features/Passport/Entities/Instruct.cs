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
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.Entities
{
    public class Instruct
    {
        public Instruct(DataReaderWrap wrap)
        {
            this.ClientID = wrap.Get<int>("ClientID");
            this.CreateDate = wrap.Get<DateTime>("CreateDate");
            this.InstructID = wrap.Get<long>("InstructID");
            this.InstructType = wrap.Get<InstructType>("InstructType");
            this.Datas = wrap.Get<string>("Datas");
            this.TargetID = wrap.Get<int>("TargetID");
        }

        /// <summary>
        /// 客户端编号
        /// </summary>
        public int ClientID { get; set; }

        public Instruct() { this.CreateDate = DateTimeUtil.UtcNow; }

        /// <summary>
        /// 指令编号
        /// </summary>
        public long InstructID { get; set; }

        /// <summary>
        /// 重试次数
        /// </summary>
        public int SendTimes { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public string Datas { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        public int TargetID { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime CreateDate
        {
            get;
            set;
        }

        /// <summary>
        /// 同步命令或者异步命令
        /// </summary>
        public bool IsSync { get; set; }

        /// <summary>
        /// 指令类型
        /// </summary>
        public InstructType InstructType { get; set; }
    }
}