//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class album_deletephotos : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string[] ids = _Request.Get("photoids", Method.Post, string.Empty).Split(',');

            if (ids.Length == 0)
            {
                ShowError("请先选择要删除的相片");
                return;
            }

            if (_Request.IsClick("delete"))
			{
				DeletePhotos(ids);
			}
        }

        private void DeletePhotos(string[] ids)
        {
            using (ErrorScope es = new ErrorScope())
            {
                if (ids.Length == 1 && ids[0] == string.Empty)
                {
                    ShowError("请先选择要删除的相片");
                    return;
                }

                int[] photoIDs = new int[ids.Length];

                for (int i = 0; i < ids.Length; i++)
                {
                    photoIDs[i] = int.Parse(ids[i]);
                }

                if (AlbumBO.Instance.DeletePhotos(MyUserID, photoIDs, true))
                {
                    ShowSuccess("删除成功", new object());
                }
                else
                {
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        ShowError(error);
                    });

                    ShowError("您所在的用户组没有权限删除该相片");
                }
            }
        }
    }
}