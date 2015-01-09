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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Logs
{
    [OperationType(RoleChange.TYPE_NAME)]
    class RoleChange : Operation
    {
        public const string TYPE_NAME = "用户组变更";

        private bool _isadd;
        private string _targetUsername;
        private Role[] _roles;

        public RoleChange(int operatorUserID, string operatorUsername, int targetUserID, string targetUsername, Role role, string ip, bool isAdd)
            : base(operatorUserID, operatorUsername, ip, targetUserID)
        {
            this._isadd = isAdd;
            this._targetUsername = targetUsername;
            this._roles = new Role[] { role };
        }

        public RoleChange(int operatorUserID, string operatorUsername, int targetUserID, string targetUsername, Role[] roles, string ip, bool isAdd)
            : base(operatorUserID, operatorUsername, ip, targetUserID)
        {
            this._isadd = isAdd;
            this._targetUsername = targetUsername;
            this._roles = roles;
        }

        public RoleChange(int operatorUserID, string operatorUsername, int targetUserID, string targetUsername, Guid roleID, string ip, bool isAdd)
            : base(operatorUserID, operatorUsername, ip, targetUserID)
        {
            this._isadd = isAdd;
            this._targetUsername = targetUsername;

            Role role = AllSettings.Current.RoleSettings.GetRole(roleID);

            if (role == null)
                this._roles = new Role[0] { };

            else
                this._roles = new Role[] { role };
        }

        public RoleChange(int operatorUserID, string operatorUsername, int targetUserID, string targetUsername, IEnumerable<Guid> roleIds, string ip, bool isAdd)
            : base(operatorUserID, operatorUsername, ip, targetUserID)
        {
            this._isadd = isAdd;
            this._targetUsername = targetUsername;

            List<Role> roles = new List<Role>();

            foreach (Guid roleID in roleIds)
            {
                Role role = AllSettings.Current.RoleSettings.GetRole(roleID);
                if (role != null)
                    roles.Add(role);
            }

            this._roles = roles.ToArray();
        }

        public override string TypeName
        {
            get { return TYPE_NAME; }
        }

        public override string Message
        {
            get
            {
                string rolesString = string.Empty;

                foreach (Role role in _roles)
                {
                    if (rolesString.Length == 0)
                        rolesString += role.Name;
                    else
                        rolesString += "、" + role.Name;
                }

                return string.Format("<a href=\"{0}\">{1}</a> 被 <a href=\"{2}\">{3}</a> {4}"
                    , BbsRouter.GetUrl("space/" + TargetID_1)
                    , _targetUsername
                    , BbsRouter.GetUrl("space/" + OperatorID)
                    , OperatorName
                    , _isadd ? "加入“" + rolesString + "”用户组" : "从“" + rolesString + "”用户组移除"
                    );
            }
        }
    }
}