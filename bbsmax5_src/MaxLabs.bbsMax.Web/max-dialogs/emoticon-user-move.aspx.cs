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
using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class emoticon_user_move : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!EmoticonBO.Instance.CanUseEmoticon(MyUserID))
            {
                ShowError(new NoPermissionUseEmoticonError());
                return;
            }

            if (GroupList.Count < 2)
            {
                ShowError("您的表情分组少于2，没有其他的分组可转移！");
            }
            if (CurrentGroup == null)
            {
                ShowError("不存在的表情分组!");
            }

            if (_Request.IsClick("move"))
            {
                Move();
            }
        }

        private void Move()
        {
            int[] emoticons = _Request.GetList<int>("emoticonids", Method.Post, new int[0]);
            if (
            EmoticonBO.Instance.MoveEmoticons(MyUserID, CurrentGroup.GroupID
                , _Request.Get<int>("targetgroup", Method.Post, 0), emoticons))
            {
                Return(true);
            }
            else
            {

            }
        }

        protected EmoticonGroupCollection GroupList
        {
            get
            {
                return EmoticonBO.Instance.GetEmoticonGroups(MyUserID);
            }
        }

        protected EmoticonGroup CurrentGroup
        {
            get
            {
                return EmoticonBO.Instance.GetEmoticonGroup(MyUserID, _Request.Get<int>("groupid", Method.Get, 0));
            }
        }
    }
}