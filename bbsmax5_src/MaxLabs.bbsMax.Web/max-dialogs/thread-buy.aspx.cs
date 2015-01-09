//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections;
using System.Web;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.FileSystem;
using MaxLabs.bbsMax.PointActions;


namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class thread_buy : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
		{

            threadID = _Request.Get<int>("threadID", Method.Get, 0);

            if (threadID <= 0)
                ShowError(new InvalidParamError("threadID"));


            if (_Request.IsClick("buy"))
            {
                ProcessBuyThread();
            }
        }


        int threadID = 0;



        private void ProcessBuyThread()
        {
            BasicThread thread = PostBOV5.Instance.GetThread(threadID);

            bool success;
            if (IsLogin == false)
                ShowError("您还没有登陆，不能购买！");
            else if (thread.Price < 1)
                ShowError("该主题不需要购买");
            else if (thread.PostUserID == MyUserID)
                ShowError("自己的主题不需要购买");
            else if (thread.IsBuyed(My))
                ShowError("您已经购买过该主题");
            else
            {

                UserPoint tradePoint = ForumPointAction.Instance.GetUserPoint(thread.PostUserID, ForumPointValueType.SellThread, thread.ForumID);
                if (tradePoint == null)
                {
                    ShowError("系统没有设置交易积分！");
                }

                using (ErrorScope es = new ErrorScope())
                {
                    success = UserBO.Instance.TradePoint(MyUserID, thread.PostUserID, thread.Price, tradePoint.Type, false, true, null);
                    if (success == false)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            if (error is UserPointOverMinValueError)
                            {
                                UserPointOverMinValueError tempError = (UserPointOverMinValueError)error;
                                NotEnoughPointBuyThread notEnoughPointBuyThread = new NotEnoughPointBuyThread("", tempError.UserPoint, thread.Price, My.ExtendedPoints[(int)tempError.UserPoint.Type]);
                                ShowError(notEnoughPointBuyThread);
                            }
                            else
                                ShowError(error.Message);
                        });
                    }
                    else
                    {
                        try
                        {
                            success = PostBOV5.Instance.CreateThreadExchange(thread.ThreadID, MyUserID, thread.Price);
                            if (success)
                            {
                                PostBOV5.Instance.SetThreadBuyedInCache(My, threadID, true);
                            }
                            else
                            {
                                ShowError("创建交易记录失败！");
                            }
                        }
                        catch (Exception ex)
                        {
                            ShowError(ex.Message);
                        }
                    }

                    if (success)
                    {
                        ShowSuccess("购买成功！", true);
                    }
                }
            }
        }
    }
}