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

using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class album_update : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int? albumID = _Request.Get<int>("albumid");

            if(albumID == null)
            {
                ShowError("缺少必要的页面参数");
            }

            m_Album = AlbumBO.Instance.GetAlbum(albumID.Value);

            if (m_Album == null)
            {
                ShowError("指定的相册不存在");
            }

            if(_Request.IsClick("update"))
            {
                using (ErrorScope es = new ErrorScope())
                {
                    MessageDisplay md = CreateMessageDisplay();

                    string albumName = _Request.Get("albumname");
                    string description = _Request.Get("description");
                    string albumPassword = _Request.Get("albumpassword");

                    PrivacyType privacyType = _Request.Get<PrivacyType>("albumprivacy", Method.Post, PrivacyType.AllVisible);

                    if (AlbumBO.Instance.UpdateAlbum(MyUserID, albumID.Value, albumName, description, privacyType, albumPassword) == false)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            ShowError(error);
                        });
                    }
                    else
                    {
                        ShowSuccess("相册信息修改成功");
                    }
                }
            }
        }

        private Album m_Album;

        public Album Album
        {
            get
            {
                return m_Album;
            }
        }
    }
}