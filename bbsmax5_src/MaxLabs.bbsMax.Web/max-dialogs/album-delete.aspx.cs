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
    public partial class album_delete : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
		{
			albumID = _Request.Get<int>("albumid", Method.All);

			if (albumID == null)
				ShowError("缺少必要参数");

			Album album = AlbumBO.Instance.GetAlbumForEdit(MyUserID, albumID.Value);

			if (album == null)
				ShowError("您要删除的相册不存在");

            if (_Request.IsClick("delete"))
            {
                DeleteAlbum();
            }
        }

		int? albumID = null;

        private void DeleteAlbum()
        {
            bool success = false;
            using (ErrorScope es = new ErrorScope())
            {
                MessageDisplay msgDisplay = CreateMessageDisplay();

                try
                {
                    success = AlbumBO.Instance.DeleteAlbum(MyUserID, albumID.Value, true);

                }
                catch (Exception ex)
                {
                    msgDisplay.AddException(ex);
                }
                if (success == false)
                {
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });
                }
            }

            if (success)
                Return("albumID", albumID.Value);
        }
    }
}