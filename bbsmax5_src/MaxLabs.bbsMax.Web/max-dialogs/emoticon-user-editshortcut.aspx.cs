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
using System.Collections.Generic;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class emoticon_user_editshortcut : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!EmoticonBO.Instance.CanUseEmoticon(MyUserID))
            {
                ShowError(new NoPermissionUseEmoticonError());
                return;
            }

            if (_Request.IsClick("update"))
            {
                Update();
            }
        }

        private void Update()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();
            List<int> emoticonids = new List<int>();
            List<string> shortcuts = new List<string>();
            foreach (Emoticon emote in EmoticonList)
            {
                emoticonids.Add(emote.EmoticonID);
                shortcuts.Add(_Request.Get("shortcut." + emote.EmoticonID, Method.Post));
            }

            if (!EmoticonBO.Instance.RenameEmoticonShortcut(MyUserID, Group.GroupID, emoticonids, shortcuts))
            {
                if (HasUnCatchedError)
                {
                    CatchError<ErrorInfo>(delegate(ErrorInfo err) { msgDisplay.AddError(err); });
                }
                else
                {
                    msgDisplay.AddError("快捷方式已存在");
                }
            }
            else
            {
                Return(true);
            }
        }

        protected EmoticonCollection m_emoticonList;

        private EmoticonGroup m_group;

        protected EmoticonGroup Group
        {
            get
            {
                if (m_group == null)
                {
                    m_group = EmoticonBO.Instance.GetEmoticonGroup(MyUserID, _Request.Get<int>("groupid", Method.Get, 0));
                }

                return m_group;
            }
        }

        protected EmoticonCollection EmoticonList
        {
            get
            {
                if (m_emoticonList == null)
                {
                    int[] emoteIds = _Request.GetList<int>("emoticonids", Method.Post, new int[0]);
                    m_emoticonList = EmoticonBO.Instance.GetEmoticons(MyUserID, emoteIds);
                }
                return m_emoticonList;
            }
        }
    }
}