//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using MaxLabs.bbsMax.Enums;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;


namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class manage_a : AdminPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_A; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("deletesubmit"))
            {
                Delete();
            }

            else if (_Request.IsClick("batchEnable"))
            {
                batckEnable(true);
            }

            else if (_Request.IsClick("batchDisable"))
            {
                batckEnable(false);
            }
        }

        private void batckEnable(bool availible)
        {
            int[] adIds = StringUtil.Split<int>(_Request.Get("AdID", Method.Post));
            AdvertBO.Instance.SetAdvertAvailabel(MyUserID, adIds, availible);
        }

        private ADCategory m_Category = null;
        protected ADCategory Category
        {
            get
            {
                if (m_Category == null)
                {
                    int _categoryId = _Request.Get<int>("categoryid", Method.Get, 0);
                    m_Category = AdvertBO.Instance.GetCategory(_categoryId);
                    if (m_Category == null)
                    {
                        m_Category = new ADCategory();
                        m_Category.Name = "所有广告";
                        m_Category.ID = 0;
                        m_Category.Description = "所有广告";
                    }
                }
                return m_Category;
            }
        }

        private void Delete()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            int[] ids = StringUtil.Split<int>(_Request.Get("AdID", Method.All, ""), ',');

            try
            {
                AdvertBO.Instance.RemoveAdvertisements(ids);
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }
        }

        private int _totalCount;

        public int TotalCount
        {
            get { return _totalCount; }
            set { _totalCount = value; }
        }

        private AdvertCollection _ads;


        protected AdvertCollection AdvertList
        {
            get
            {
                int pageNumber = _Request.Get<int>("page", Method.Get, 1);
                int _categoryId = _Request.Get<int>("categoryid", Method.Get, 0);
               
                if (_ads == null)
                    _ads = AdvertBO.Instance.GetAdverts(_categoryId, ADPosition,  20, pageNumber, out _totalCount);
                return _ads;
            }
        }

        protected ADPosition ADPosition
        {
            get
            {
                string pos = Request.QueryString["pos"];
                switch (pos)
                {
                    case "top":
                        return ADPosition.Top;
                    case "left":
                        return ADPosition.Left;
                    case "right":
                        return ADPosition.Right;
                    case "bottom":
                        return ADPosition.Bottom;

                }

                return ADPosition.None;
            }
        }

        protected bool IsEnable
        {
            get
            {
                return AllSettings.Current.AdvertSettings.EnableAdverts;
            }
        }
    }
}