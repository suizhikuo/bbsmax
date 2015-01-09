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


namespace MaxLabs.bbsMax.Filters
{
    public enum InviteSerialSearchField
    {
        UserID,
        Username,
        ToUserID,
        ToUsername,
        ToEmail,
        Serial
    }

    /// <summary>
    /// 用户列表排序字段
    /// </summary>
    public enum OrderBy : byte
    {
        CreateDate,
        ExpiresDate
    }

    public class InviteSerialFilter:FilterBase<InviteSerialFilter>
    {
        public InviteSerialFilter() 
        { 
            this.Pagesize = Consts.DefaultPageSize;
            this.Order = OrderBy.CreateDate;
            this.IsDesc = true;
        }

        [FilterItem(FormName = "status")]
        public InviteSerialStatus? Status
        {
            get;
            set;
        }

        [FilterItem(FormName = "UserID")]
        public int? UserID
        {
            get;
            set;
        }

        [FilterItem(FormName = "Username")]
        public string Username
        {
            get;
            set;
        }

        [FilterItem(FormName = "CreateDateBegin")]
        public DateTime? CreateDateBegin
        {
            get;
            set;
        }

        [FilterItem(FormName = "CreateDateEnd")]
        public DateTime? CreateDateEnd
        {
            get;
            set;
        }

        [FilterItem(FormName = "ExpiresDateBegin")]
        public DateTime? ExpiresDateBegin
        {
            get;
            set;
        }

        [FilterItem(FormName = "ExpiresDateEnd")]
        public DateTime? ExpiresDateEnd
        {
            get;
            set;
        }
        
        [FilterItem(FormName="SearchField")]
        public InviteSerialSearchField? SearchField
        {
            get;
            set;
        }

        [FilterItem(FormName="SearchKey")]
        public string SearchKey
        {
            get;
            set;
        }

        [FilterItem(FormName = "pagesize")]
        public int Pagesize
        {
            get;
            set;
        }

        [FilterItem(FormName="orderBy")]
        public OrderBy? Order
        {
            get;
            set;
        }

        [FilterItem(FormName="IsDesc")]
        public bool? IsDesc
        {
            get;
            set;
        }
    }
}