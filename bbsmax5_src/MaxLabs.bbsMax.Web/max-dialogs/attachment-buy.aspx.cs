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
    public partial class attachment_buy : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
		{

            tempAttachmentID = _Request.Get<int>("AttachmentID", Method.Get, 0);

            if (tempAttachmentID <= 0)
                ShowError(new InvalidParamError("attachmentID"));


            if (_Request.IsClick("buy"))
            {
                ProcessBuyAttachment();
            }
        }


        int tempAttachmentID = 0;



        private void ProcessBuyAttachment()
        {
            if (IsLogin == false)
            {
                ShowError("您还没有登陆，不能购买附件！");
            }
            else
            {

                Attachment tempAttachment = null;
                MaxLabs.bbsMax.FileSystem.PhysicalFile phyFile = null;
                if (tempAttachmentID <= 0)
                {
                    ShowError(new InvalidParamError("attachmentID"));
                }
                else
                {
                    tempAttachment = PostBOV5.Instance.GetAttachment(tempAttachmentID);

                    if (tempAttachment == null)
                    {
                        ShowError("该附件不存在,可能被移动或被删除！");
                    }
                    //diskFile = zzbird.Common.Disk.DiskManager.GetDiskFile(tempAttachment.DiskFileID);
                    phyFile = FileManager.GetFile(tempAttachment.FileID);
                }
                if (phyFile == null)
                {
                    ShowError("该附件不存在,可能被移动或被删除！");
                }

                if (MyUserID == tempAttachment.UserID || tempAttachment.Price == 0)
                {
                    ShowError("该附件您不需要购买！");
                }
                if (!tempAttachment.IsBuyed(My))//没购买过
                {
                    int trade = Math.Abs(tempAttachment.Price);

                    PostV5 post = PostBOV5.Instance.GetPost(tempAttachment.PostID, false);
                    if(post == null)
                        ShowError("该附件不存在,可能被移动或被删除！");

                    UserPoint tradePoint = ForumPointAction.Instance.GetUserPoint(tempAttachment.UserID, ForumPointValueType.SellAttachment, post.ForumID);

                    if (tradePoint == null)
                    {
                        ShowError("系统交易积分错误！");
                    }

                    using (ErrorScope es = new ErrorScope())
                    {
                        bool success = UserBO.Instance.TradePoint(MyUserID, tempAttachment.UserID, tempAttachment.Price, tradePoint.Type, false, true, null);
                        if (success == false)
                        {
                            es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                            {

                                if (error is UserPointOverMinValueError)
                                {
                                    UserPointOverMinValueError tempError = (UserPointOverMinValueError)error;
                                    NotEnoughPointBuyAttachment notEnoughPointBuyAttachment = new NotEnoughPointBuyAttachment("", tempError.UserPoint, tempAttachment.Price, My.ExtendedPoints[(int)tempError.UserPoint.Type]);
                                    ShowError(notEnoughPointBuyAttachment);
                                }
                                else
                                    ShowError(error.Message);
                            });
                        }
                        else
                        {
                            //创建交易记录
                            if (!PostBOV5.Instance.CreateAttachmentExchange(tempAttachment.AttachmentID, MyUserID, tempAttachment.Price))
                            {
                                ShowError("购买成功，创建交易记录失败！");
                            }
                            else
                            {
                                //下载
                                //更新userprofile
                                if (My.BuyedAttachments.ContainsKey(tempAttachment.AttachmentID))
                                {
                                    My.BuyedAttachments[tempAttachment.AttachmentID] = true;
                                }

                                //Thread.FirstPost = null;
                                ShowSuccess("购买成功,现在可以查看、下载或收藏！", true);

                            }
                        }
                    }
                }
                else
                {
                    ShowError("您已经购买此附件，无需再次购买!");
                }
            }
        }
    }
}