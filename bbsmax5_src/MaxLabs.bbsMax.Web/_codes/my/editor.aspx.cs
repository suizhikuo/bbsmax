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

namespace MaxLabs.bbsMax.Web.max_pages
{
    public partial class editor : BbsPageBase
    {

        protected override string PageName
        {
            get
            {
                return "setting";
            }
        }

        List<List<string>> bigToolbarItems = new List<List<string>>();
        List<List<string>> smallToolbarItems = new List<List<string>>();

        protected void Page_Load(object sender, EventArgs e)
        {
            List<string> temp = new List<string>();
            temp.AddRange(new string[] { "fontname", "fontsize", "textcolor", "bgcolor", "bold", "italic", "underline", "strikethrough" });
            smallToolbarItems.Add(temp);

            temp = new List<string>();
            temp.AddRange(new string[]{
         "justifyleft", "justifycenter", "justifyright", "indent", "outdent", "insertorderedlist", "insertunorderedlist"
         ,"removeformat","plainpaste", "wordpaste","link", "unlink","table","hide","free","quote"});
            smallToolbarItems.Add(temp);

            temp = new List<string>();
            temp.AddRange(new string[] { "emoticons", "photo", "attachment", "video", "audio", "flash" });
            bigToolbarItems.Add(temp);

            temp = new List<string>();
            temp.AddRange(new string[] { "fullscreen" });
            bigToolbarItems.Add(temp);
        }



        protected bool WyswygMode
        {
            get
            {
                return true;
            }
        }

        protected List<List<string>> BigToolbarItems
        {
            get
            {
                bool flag = false;
                List<string> emptyBottonGroup;
                do
                {
                    emptyBottonGroup = null;
                    foreach (List<string> s in bigToolbarItems)
                    {
                        if (s.Count == 0)
                        {
                            emptyBottonGroup = s;
                            flag = true;
                            break;
                        }
                    }
                    if (emptyBottonGroup != null)
                        bigToolbarItems.Remove(emptyBottonGroup);
                } while (flag);

                return bigToolbarItems;
            }
        }

        protected bool UseMaxCode
        {
            get
            {
                return false;
            }
        }



        protected List<List<string>> ToolbarGroup
        {
            get
            {
                bool flag = false;
                List<string> emptyBottonGroup;
                do
                {
                    emptyBottonGroup = null;
                    foreach (List<string> s in smallToolbarItems)
                    {
                        if (s.Count == 0)
                        {
                            emptyBottonGroup = s;
                            flag = true;
                            break;
                        }
                    }
                    if (emptyBottonGroup != null)
                        smallToolbarItems.Remove(emptyBottonGroup);
                } while (flag);
                return smallToolbarItems;
            }
        }
    }
}