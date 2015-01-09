//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

#if !Passport

using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class a_edit : AdminDialogPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_A; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("save"))
            {
                SaveAdvert();
            }
        }

        protected int FloorCount
        {
            get
            {
                return AllSettings.Current.BbsSettings.PostsPageSize;
            }
        }


        protected ADPosition ADPosition
        {
            get
            {
                string pos = Request.QueryString["pos"];
                switch (pos)
                {
                    case "Top":
                        return ADPosition.Top;
                    case "Left":
                        return ADPosition.Left;
                    case "Right":
                        return ADPosition.Right;
                    case "Bottom":
                        return ADPosition.Bottom;

                }

                return ADPosition.None;
            }
        }

        protected ADCategory Category
        {
            get
            {
                if(Advert.ADID>0)
                {
                    return Advert.Category;
                }

                int _categoryId = _Request.Get<int>("categoryid", Method.Get, 0);
                return AdvertBO.Instance.GetCategory(_categoryId);
            }
        }

        private List<string> forumSpliter;

        protected List<string> ForumSpliter
        {
            get
            {
                if (forumSpliter == null)
                {
                    ForumBO.Instance.GetTreeForums("&nbsp;&nbsp;&nbsp;&nbsp;", delegate(Forum forum) { return true; }, out _forumList, out forumSpliter);
                    //ForumManager.GetTreeForums("&nbsp;&nbsp;&nbsp;&nbsp;", new int[0], false, out _forumList, out forumSpliter);
                }
                return forumSpliter;
            }
        }

        protected override bool EnableClientBuffer
        {
            get
            {
                return false;
            }
        }

        private ForumCollection _forumList;

        protected ForumCollection ForumList
        {
            get
            {
                if (_forumList == null)
                {
                    ForumBO.Instance.GetTreeForums("&nbsp;&nbsp;&nbsp;&nbsp;", delegate(Forum forum) { return true; }, out _forumList, out forumSpliter);
                    //ForumManager.GetTreeForums("&nbsp;&nbsp;&nbsp;&nbsp;", new int[0], false, out _forumList, out forumSpliter);
                }
                return _forumList;
            }
        }

        protected ADTargetCollection CommonPageList
        {
            get
            {
                return ADTargetCommonPages.All;
            }
        }

        protected bool AdInFloor( int floor  )
        {
            if (!string.IsNullOrEmpty(Advert.Floor))
            {
                if (Advert.Floor.Contains("," + floor + ","))
                    return true;
                if (Advert.Floor.Contains(",-1,") && floor != -2)
                    return true;
            }
            return false;
        }

        public ADCategoryCollection ADCategoryList
        {
            get
            {
                return AdvertBO.Instance.GetAllAdvertCategory();
            }
        }

        private void SaveAdvert()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("src", "code", "width", "height", "href", "text","floor");

            int adid = 0, height = 0, width = 0, fontsize = 14, categoryId, index;
            ADType adtype = ADType.Code;
            string href = "", resourcehref = "",floor=",0,", text = "", color = "", code = "", title = "", targets = "",floorLast ="";
            DateTime begindate, enddate;
            bool available = true;
            ADPosition position;

            position    =_Request.Get<ADPosition>("position",Method.Post, ADPosition.None);
            begindate   = DateTimeUtil.ParseBeginDateTime(_Request.Get("begindate", Method.Post));
            enddate     = DateTimeUtil.ParseEndDateTime(_Request.Get("enddate", Method.Post));
            adid        = _Request.Get<int>("adid", Method.Get, 0);
            adtype      = _Request.Get<ADType>("adtype", Method.Post, ADType.Code);
            title       = _Request.Get("title", Method.Post);
            available   = _Request.Get<bool>("Available", Method.Post, false);
            categoryId  = Category.ID;
            targets     = _Request.Get("targets", Method.Post, "");
            index       = _Request.Get<int>("index", Method.Post, 0);
            floor       = _Request.Get("floor", Method.Post);
            floorLast   = _Request.Get("floorLast", Method.Post, "");

            if(!string.IsNullOrEmpty(floorLast))
                floor += "," + floorLast;//最底楼特殊处理，拼接上去

            switch (adtype)
            {
                case ADType.Code:
                    code            = _Request.Get("code", Method.Post,"",false);
                    break;
                case ADType.Flash:
                    resourcehref    = _Request.Get("flashsrc", Method.Post);
                    height          = _Request.Get<int>("flashheight", Method.Post, 0);
                    width           = _Request.Get<int>("flashwidth", Method.Post, 0);
                    break;
                case ADType.Image:
                    resourcehref    = _Request.Get("imagesrc", Method.Post);
                    height          = _Request.Get<int>("imageheight", Method.Post, 0);
                    width           = _Request.Get<int>("imagewidth", Method.Post, 0);
                    text            = _Request.Get("alt", Method.Post);
                    href            = _Request.Get("imagehref", Method.Post);
                    break;
                case ADType.Text:
                    text            = _Request.Get("text", Method.Post);
                    href            = _Request.Get("href", Method.Post);
                    color           = _Request.Get("color", Method.Post);
                    fontsize        = _Request.Get<int>("fontsize", Method.Post, 12);
                    break;
            }

            Advert CurrentAD = null;

            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    CurrentAD = AdvertBO.Instance.SaveAdvert(
                          MyUserID, adid, index, categoryId, position
                        , adtype, available, title, href, text
                        , fontsize, color, resourcehref, width, height, begindate
                        , enddate, code, targets, floor);
                }
                catch (Exception ex)
                {
                    msgDisplay.AddException(ex);
                }

                if (CurrentAD != null)
                    Return(CurrentAD, IsEditMode);

                if (es.HasUnCatchedError)
                {
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });
                }
            }
        }

        Advert _advert;
        protected Advert Advert
        {
            get
            {
                int _adid = _Request.Get<int>("adid", Method.Get, 0);
                if (_adid == 0 && _advert == null)
                {
                    _advert = new Advert();
                    _advert.Code = StringUtil.HtmlEncode(_advert.Code);
                }
                if (_advert == null)
                {
                    _advert = AdvertBO.Instance.GetAdvert(_adid);
                    _advert.Code = StringUtil.HtmlEncode(_advert.Code);
                }

                return _advert;
            }
        }

        protected bool HasTarget(object t)
        {
            
            return Advert.Targets.Contains("," + t.ToString() + ",");
        }

        protected bool HasForumTarget( int forumID)
        {
            return HasTarget(ForumPrefix +forumID);
        }

        protected string AdBeginDate
        {
            get
            {
                if (Advert.BeginDate.Year > 1900)
                    return OutputDate(Advert.BeginDate);
                return string.Empty;
            }
        }

        protected string AdEndDate
        {
            get
            {
                if (Advert.EndDate.Year < 9999 && Advert.EndDate.Year > 1900)
                    return OutputDate(Advert.EndDate);
                return string.Empty;
            }
        }

        protected string ForumPrefix
        {
            get
            {
                return ADTarget.ForumPrefix;
            }
        }

        protected bool IsEditMode
        {
            get
            {
                return Advert.ADID > 0;
            }
        }
    }
}
#endif