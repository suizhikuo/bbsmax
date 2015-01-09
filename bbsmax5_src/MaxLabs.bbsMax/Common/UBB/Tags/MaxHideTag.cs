//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections.Generic;

namespace MaxLabs.bbsMax.Ubb
{
    public class MaxHideTag : UbbTagHandler
    {
        public delegate CheckPermissionResults CheckPermissionCallback(string hideTagParam);

        private CheckPermissionCallback m_OnCheckPermission;

        public CheckPermissionCallback OnCheckPermission
        {
            get { return this.m_OnCheckPermission; }
            set { this.m_OnCheckPermission = value; }
        }

        public override string HtmlTagName
        {
            get { throw new NotImplementedException(); }
        }

        public override string UbbTagName
        {
            get { return "hide"; }
        }

        public override string BuildHtml(UbbParser parser, string param, string content)
        {
            if (OnCheckPermission == null)
                throw new ArgumentNullException("OnCheckPermission");

            CheckPermissionResults result = OnCheckPermission(param);

            if (result == CheckPermissionResults.NeedComment)
            {
                //TODO:国际化
                return "<p class=\"maxcode-hidetips\">隐藏内容, 需要回复可见</p>";
            }
            else if (result == CheckPermissionResults.NoPermission)
            {
                //TODO:国际化
                return "<p class=\"maxcode-hidetips\">隐藏内容, 您所在用户组没有权限查看</p>";
            }
            else if (result == CheckPermissionResults.HasPermission)
            {
                //TODO:国际化
                return string.Format("<div class=\"maxcode-hidecontent\"><p class=\"maxcode-hidetips\">隐藏内容, 已获得查看权限</p>{0}</div>", content);
            }
            else if (result == CheckPermissionResults.HasManagePermission)
            { //TODO:国际化
                return string.Format("<div class=\"maxcode-hidecontent\"><p class=\"maxcode-hidetips\">隐藏内容, 您有管理权限,已获得查看权限</p>{0}</div>", content);
            }
            else
            {
                return content;
            }
        }

        public enum CheckPermissionResults
        {
            HasPermission, HasManagePermission, NeedComment, NoPermission, NoProcessHideTag
        }
    }
}