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
using System.Web;

using MaxLabs.WebEngine;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Email;

namespace MaxLabs.bbsMax.Templates
{
    [TemplatePackage]
    public class InviteTemplateMembers
    {
        #region 用户邀请页面/邀请码管理页面

        /*********************邀请码邮件发送表单*******************/
        [TemplateTag]
        public void SendInviteForm(string serial, SendInviteFormTemplate body)
        {
            body.Invoke(new SendInviteFormParams(serial));
        }

        public delegate void SendInviteFormTemplate(SendInviteFormParams _this);
        public class SendInviteFormParams
        {
            public SendInviteFormParams(string serial)
            {
                try
                {
                    this.Serial = new Guid(serial);
                }
                catch
                {
                    this.Serial = Guid.Empty;
                }

                InviteSerial theSerial = InviteBO.Instance.GetInviteSerial(Serial);
                if (theSerial != null)
                {
                    if (theSerial.UserID == UserBO.Instance.GetCurrentUserID())
                    {
                        if (theSerial.Status == InviteSerialStatus.Unused || theSerial.Status == InviteSerialStatus.Unused)
                        {
                            InviteEmail email = new InviteEmail(null, Serial, User.Current.Username, User.CurrentID);
                            this.EmailContent = email.Content;
                            this.EmailTitle = email.Subject;
                            this.IsValidSerial = true;
                        }
                    }
                }
            }


            public bool IsValidSerial
            {
                get;
                private set;
            }

            public Guid Serial
            {
                get;
                set;
            }
            public string EmailContent
            {
                get;
                private set;
            }
            public string EmailTitle
            {
                get;
                private set;
            }
        }
        /**********************************************************/


        public delegate void InviteSerialStatTemplate(InviteSerialStat stat);
        [TemplateTag]
        public void InviteSerialStatList(
             string sortBy
            , int pageSize
            , int PageNumber
            , GlobalTemplateMembers.CommonHeadFootTemplate head
            , GlobalTemplateMembers.CommonHeadFootTemplate foot
            , InviteSerialStatTemplate item)
        {
            if (pageSize <= 0) pageSize = Consts.DefaultPageSize ;

            InviteSerialStatus orderType = InviteSerialStatus.All;
            InviteSerialStatCollection Stats;


            if (!string.IsNullOrEmpty(sortBy))
            {
                sortBy = sortBy.Trim();
                if (sortBy.Equals("used", StringComparison.OrdinalIgnoreCase))
                    orderType = InviteSerialStatus.Used;
                else if (sortBy.Equals("unused", StringComparison.OrdinalIgnoreCase))
                    orderType = InviteSerialStatus.Unused;
                else if (sortBy.Equals("noreg", StringComparison.OrdinalIgnoreCase))
                    orderType = InviteSerialStatus.Unused;
                else if (sortBy.Equals("expiress", StringComparison.OrdinalIgnoreCase))
                    orderType = InviteSerialStatus.Expires;
            }

            int rowCount;
            Stats = InviteBO.Instance.GetStatList(orderType, pageSize, PageNumber, out rowCount);

            head(new GlobalTemplateMembers.CommonHeadFootTemplateParams(rowCount, pageSize));
            if (rowCount > 0)
            {
                UserBO.Instance.FillSimpleUsers(Stats);
                foreach (InviteSerialStat stat in Stats)
                {
                    item.Invoke(stat);
                }
            }
            foot(new GlobalTemplateMembers.CommonHeadFootTemplateParams(rowCount, pageSize));
        }


        public delegate void InviteSerialManagerTemplate(InviteSerial serial);

        /// <summary>
        /// 邀请码搜索列表
        /// </summary>
        /// <param name="filter">过滤器字段</param>
        /// <param name="mode">前台还是后台， 如果是后台的话这个值就为admin, 前台的话任意值</param>
        /// <param name="pageNumber"></param>
        /// <param name="listItem"></param>
        /// <param name="foot"></param>
        [TemplateTag]
        public void InviteSerialList(
              string filter
            , string mode
            , int pageNumber
            , InviteSerialManagerTemplate listItem
            , GlobalTemplateMembers.CommonHeadFootTemplate foot
            , GlobalTemplateMembers.CommonHeadFootTemplate head
            )
        {
            InviteSerialFilter searchFilter = InviteSerialFilter.GetFromFilter(filter);
            InviteSerialCollection inviteSerials;

            int rowCount;
            if (searchFilter == null) searchFilter = new InviteSerialFilter();
            int? owner = null;
            if (string.IsNullOrEmpty(mode) || !mode.Equals("admin", StringComparison.OrdinalIgnoreCase))
            {
                if (searchFilter.Status == null)
                {
                    searchFilter.Status = InviteSerialStatus.Unused;
                }
                owner =User.CurrentID;
            }

            inviteSerials = InviteBO.Instance.GetInviteSerials(User.Current, owner,searchFilter, pageNumber, out rowCount);

            UserBO.Instance.FillSimpleUsers(inviteSerials, 0);

            head.Invoke(new GlobalTemplateMembers.CommonHeadFootTemplateParams(rowCount, searchFilter.Pagesize));
            foreach (InviteSerial s in inviteSerials)
            {
                listItem.Invoke(s);
            }
            foot.Invoke(new GlobalTemplateMembers.CommonHeadFootTemplateParams(rowCount, searchFilter.Pagesize));
        }

        public class MyInviteSerialStatFormParams
        {
            public MyInviteSerialStatFormParams() { }

            private InviteSerialStat _stat=null;
            public InviteSerialStat Stat
            {
                get
                {
                    if (_stat == null)
                        _stat = InviteBO.Instance.GetStat(UserBO.Instance.GetCurrentUserID());
                    return _stat;
                }
                
            }
            public int CanBuyCount
            {
                get { return 1; }
            }
        }


        public delegate void MyInviteSerialStatFormTemplate(MyInviteSerialStatFormParams _this);

        [ TemplateTag  ]
        public void MyInviteSerialStatForm(MyInviteSerialStatFormTemplate body)
        {
            body.Invoke(new MyInviteSerialStatFormParams());
        }
        #endregion
    }
}