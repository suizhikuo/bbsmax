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
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Enums;


namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class setting_search : AdminPageBase
	{
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_Search; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SaveSetting<SearchSettings>("savesetting");
		}

        protected override bool SetSettingItemValue(SettingBase setting, System.Reflection.PropertyInfo property)
        {
            if (property.Name == "SearchType")
            {
                SearchType searchType = _Request.Get<SearchType>("SearchType", Method.Post, SearchType.LikeStatement);

                if (searchType != AllSettings.Current.SearchSettings.SearchType)
                {
                    if (searchType == SearchType.FullTextIndex)
                    {
                        if (false == PostBOV5.Instance.StartPostFullTextIndex())
                        {
                            MsgDisplayForSaveSettings.AddError("开启全文索引失败，可能由于您没有足够的数据库权限");
                            return false;
                        }
                    }
                    else
                    {
                        if (false == PostBOV5.Instance.StopPostFullTextIndex())
                        {
                            MsgDisplayForSaveSettings.AddError("关闭全文索引失败，可能由于您没有足够的数据库权限");
                            return false;
                        }
                    }
                }
            }
            else if (property.Name == "EnableSearch")
            {
                SearchSettings temp = (SearchSettings)setting;
                temp.EnableSearch = new ExceptableSetting.ExceptableItem_bool().GetExceptable("EnableSearch", MsgDisplayForSaveSettings);

                if (MsgDisplayForSaveSettings.HasAnyError())
                {
                    return false;
                }
                return true;
            }

            else if (property.Name == "CanSearchTopicContent")
            {
                SearchSettings temp = (SearchSettings)setting;
                temp.CanSearchTopicContent = new ExceptableSetting.ExceptableItem_bool().GetExceptable("CanSearchTopicContent", MsgDisplayForSaveSettings);

                if (MsgDisplayForSaveSettings.HasAnyError())
                {
                    return false;
                }
                return true;
            }

            else if (property.Name == "CanSearchAllPost")
            {
                SearchSettings temp = (SearchSettings)setting;
                temp.CanSearchAllPost = new ExceptableSetting.ExceptableItem_bool().GetExceptable("CanSearchAllPost", MsgDisplayForSaveSettings);

                if (MsgDisplayForSaveSettings.HasAnyError())
                {
                    return false;
                }
                return true;
            }

            else if (property.Name == "CanSearchUserTopic")
            {
                SearchSettings temp = (SearchSettings)setting;
                temp.CanSearchUserTopic = new ExceptableSetting.ExceptableItem_bool().GetExceptable("CanSearchUserTopic", MsgDisplayForSaveSettings);

                if (MsgDisplayForSaveSettings.HasAnyError())
                {
                    return false;
                }
                return true;
            }

            else if (property.Name == "CanSearchUserPost")
            {
                SearchSettings temp = (SearchSettings)setting;
                temp.CanSearchUserPost = new ExceptableSetting.ExceptableItem_bool().GetExceptable("CanSearchUserPost", MsgDisplayForSaveSettings);

                if (MsgDisplayForSaveSettings.HasAnyError())
                {
                    return false;
                }
                return true;
            }

            else if (property.Name == "MaxResultCount")
            {
                SearchSettings temp = (SearchSettings)setting;
                temp.MaxResultCount = new ExceptableSetting.ExceptableItem_Int_MoreThenZero().GetExceptable("MaxResultCount", MsgDisplayForSaveSettings);

                if (MsgDisplayForSaveSettings.HasAnyError())
                {
                    return false;
                }
                return true;
            }

            else if (property.Name == "SearchTime")
            {
                SearchSettings temp = (SearchSettings)setting;
                temp.SearchTime = new ExceptableSetting.ExceptableItem_Int_MoreThenZero().GetExceptable("SearchTime", MsgDisplayForSaveSettings);

                if (MsgDisplayForSaveSettings.HasAnyError())
                {
                    return false;
                }
                return true;
            }

            return base.SetSettingItemValue(setting, property);
        }

        protected SearchSettings SearchSettings
        {
            get
            {
                return AllSettings.Current.SearchSettings;
            }
        }
    }
}