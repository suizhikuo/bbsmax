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

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class photo_delete : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            photoID = _Request.Get<int>("photoid", Method.All);

            if (photoID == null)
                ShowError("缺少必要参数");

            Photo photo = AlbumBO.Instance.GetPhotoForDelete(MyUserID, photoID.Value);

            if (photo == null)
                ShowError("您要删除的图片不存在");

            if (_Request.IsClick("delete"))
            {
                DeletePhoto();
            }
        }

        int? photoID = null;

        private void DeletePhoto()
        {
            using (new ErrorScope())
            {
                MessageDisplay msgDisplay = CreateMessageDisplay();

                try
                {

                    bool success = AlbumBO.Instance.DeletePhoto(MyUserID, photoID.Value, true);

                    if (!success)
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            msgDisplay.AddError(error);
                        }
                        );
                    }
                    else
                    {
                        Return("photoID", photoID.Value);
                        //msgDisplay.ShowInfo(this);
                    }

                }
                catch (Exception ex)
                {
                    msgDisplay.AddError(ex.Message);
                }
            }
        }
    }
}