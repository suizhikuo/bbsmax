//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Errors;
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Web.max_dialogs.user
{
    public partial class user_points : UserDialogPageBase
    {
        private User m_User;
        private  UserPointCollection points;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!UserBO.Instance.CanEditUserPoints(My, UserID))
            {
                ShowError(new NoPermissionEditUserPointError());
                return;
            }

            m_User = UserBO.Instance.GetUser(UserID);

            if (m_User == null)
            {
                ShowError(new UserNotExistsError("id", UserID));
                return;
            }

            points = AllSettings.Current.PointSettings.EnabledUserPoints;
            if (_Request.IsClick("updatepoint"))
            {
                UpdatePoint();
            }
        }

        protected User user
        {
            get { return m_User; }
        }

        protected List<PointInfo> PointList
        {
            get { return UserBO.Instance.GetUserPointInfos(UserID); }
        }

        protected void UpdatePoint()
        {

            MessageDisplay msgDisplay = CreateMessageDisplay();

            int curPoint;
            bool flag = false;
            int[] newPoints = new int[8];
            for (int i = 0; i < newPoints.Length; i++)
            {
                flag = false;
                curPoint = _Request.Get<int>("point" + i, MaxLabs.WebEngine.Method.Post, int.MinValue);

                foreach (UserPoint up in PointTypeList)
                {
                    if ((int)up.Type == i)
                    {
                        if (curPoint >= up.MinValue && curPoint <= up.MaxValue)
                        {
                            newPoints[i] = curPoint;
                            flag = true;
                            break;
                        }
                    }
                }

                if (flag == false) newPoints[i] = user.ExtendedPoints[i];
            }
            using (ErrorScope es = new ErrorScope())
            {
                UserBO.Instance.UpdateUserPoints(My, UserID, newPoints,"重置积分", string.Concat( "管理员：",My.Username,"，后台手动设置积分"));

                if (es.HasError)
                {
                    es.CatchError(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });
                }
            }
        }

        protected UserPointCollection PointTypeList
        {
            get
            {
                return points;
            }
        }

        protected string GeneralPointExpression
        {
            get
            {
                return AllSettings.Current.PointSettings.GeneralPointExpression;
            }
        }

        protected int GetPointIndex( UserPoint up )
        {
            return (int)up.Type;
        }

        protected int GetPointValue(object pointIndex )
        {
            return user.ExtendedPoints[(int)pointIndex];
        }
    }
}