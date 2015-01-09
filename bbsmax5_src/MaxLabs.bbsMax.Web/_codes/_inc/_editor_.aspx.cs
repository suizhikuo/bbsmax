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
    public partial class _editor_ : BbsPageBase
    {
        string defaultWidth = "100%";
        int defaultHeight = 300;
        string id = "content";

        List<List<string>> bigToolbarItems = new List<List<string>>();
        List<List<string>> smallToolbarItems = new List<List<string>>();

        protected override string PageName
        {
            get { return "editor"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Mode == "signature")
            {
                List<string> temp = new List<string>();
                temp.AddRange(new string[] { 
                "fontname", "fontsize", "textcolor", "bgcolor"
                , "-"
                ,"bold", "italic", "underline", "strikethrough" });
                smallToolbarItems.Add(temp);

                temp = new List<string>();
                temp.AddRange(new string[]{
             "justifyleft", "justifycenter", "justifyright", "indent", "outdent", "insertorderedlist", "insertunorderedlist","removeformat",
             "-",
             "link", "unlink","table"});
                smallToolbarItems.Add(temp);

                Attachment = false;
                FullScreen = false;
                Code = false;
            }
            else if (Mode == "blog")
            {

                List<string> temp = new List<string>();
                temp.AddRange(new string[] { 
                "fontname", "fontsize", "textcolor", "bgcolor"
                , "-"
                ,"bold", "italic", "underline", "strikethrough" });
                smallToolbarItems.Add(temp);

                temp = new List<string>();
                temp.AddRange(new string[]{
                "justifyleft", "justifycenter", "justifyright", "indent", "outdent", "insertorderedlist", "insertunorderedlist","removeformat",
                "-",
                "plainpaste", "wordpaste","link", "unlink","table","quote"});
                smallToolbarItems.Add(temp);
                Attachment = false;
            }
            else
            {
                List<string> temp = new List<string>();
                temp.AddRange(new string[] { 
                "fontname", "fontsize", "textcolor", "bgcolor"
                , "-"
                ,"bold", "italic", "underline", "strikethrough" });
                smallToolbarItems.Add(temp);

                temp = new List<string>();
                temp.AddRange(new string[]{
                "justifyleft", "justifycenter", "justifyright", "indent", "outdent", "insertorderedlist", "insertunorderedlist","removeformat",
                "-",
                "plainpaste", "wordpaste","link", "unlink","table","hide","free","quote"});
                smallToolbarItems.Add(temp);

            }
        }

        /// <summary>
        /// 是否支持可视化编辑
        /// </summary>
        protected bool SupportWyswygMode
        {
            get
            {
                string os = RequestUtil.GetPlatformName(Request);

                if (
                       os == "iPad"
                    || os == "iPhone"
                    || os == "Android"
                )
                    return false;
                return true;
            }
        }

        protected bool WyswygMode
        {
            get
            {
                if (SupportWyswygMode == false)
                    return false;
                if (Parameters["WyswygMode"] == null)
                {
                    return true;
                }
                return bool.Parse(Parameters["WyswygMode"].ToString());
            }
        }

        protected string Id
        {
            get
            {
                if (Parameters["id"] != null)
                {
                    return Parameters["id"].ToString();
                }
                return id;
            }
        }

        protected string Mode
        {
            get
            {
                return Parameters["mode"] + "";
            }
        }

        protected List<List<string>> SmallToolbarGroup
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

        protected string Value
        {
            get
            {
                if (Parameters["value"] != null)
                    return (string)Parameters["value"];
                return string.Empty;
            }
        }

        private bool? m_FullScreen = null;
        protected bool FullScreen
        {
            get
            {
                if(m_FullScreen==null)
                 m_FullScreen = Parameters["FullScreen"] == null ? true : bool.Parse(Parameters["FullScreen"].ToString());
                return m_FullScreen.Value;
            }
            private set
            {
                m_FullScreen = value;
            }
        }

        protected bool Audio
        {
            get
            {
                return Parameters["Audio"] == null ? true : bool.Parse(Parameters["Audio"].ToString());
            }
        }

        protected bool Video
        {
            get
            {
                return Parameters["Video"] == null ? true : bool.Parse(Parameters["Video"].ToString());
            }
        }

        protected bool Emoticons
        {
            get
            {
                return Parameters["Emoticons"] == null ? true : bool.Parse(Parameters["Emoticons"].ToString());
            }
        }

        protected bool Flash
        {
            get
            {
                return Parameters["Flash"] == null ? true : bool.Parse(Parameters["Flash"].ToString());
            }
        }

        protected bool Photo
        {
            get
            {
                return Parameters["photo"] == null ? true : bool.Parse(Parameters["photo"].ToString());
            }
        }

        private bool? m_code = null;
        protected bool Code
        {
            get
            {
                if(m_code==null)
                 m_code = Parameters["code"] == null ? true : bool.Parse(Parameters["code"].ToString());
                return m_code.Value;
            }
            private set {
                m_code = value;
            }

        }

        protected bool Image
        {
            get
            {
                return Parameters["image"] == null ? true : bool.Parse(Parameters["image"].ToString());
            }
        }

        private bool? m_Attachment=null;
        protected bool Attachment
        {
            get
            {
                if (m_Attachment == null)
                    m_Attachment = Parameters["attachment"] == null ? true : bool.Parse(Parameters["attachment"].ToString());
                return m_Attachment.Value;
            }
            private set
            {
                m_Attachment = value;
            }
        }

        protected int ToolbarHeight
        {
            get
            {
                return 50;
            }
        }
        protected int BodyHeight
        {
            get
            {
                return this.Height - ToolbarHeight -BottomHeight - BorderWidth *2;
            }
        }

        protected int BottomHeight
        {
            get
            {
                return 30;
            }
        }

        protected bool UseMaxCode
        {
            get
            {
                if (Parameters["usemaxcode"] != null)
                    return bool.Parse(Parameters["usemaxcode"].ToString());
                return false;
            }
        }

        protected int BorderWidth
        {
            get
            {
                return 5;
            }
        }

        protected string Width
        {
            get
            {
                if (Parameters["width"] == null)
                    return defaultWidth;
                return Parameters["width"] + "";
            }
        }

        protected int Height
        {
            get
            {
                return int.Parse(Parameters["height"] + "");
            }
        }


        /*
        public class _editor_ : BbsPageBase
        {
          string[] defaultToolbarItems =  new string[]{"source", "preview", "fullscreen", "undo", "redo", "cut", "copy", "paste",
            "plainpaste", "wordpaste", "justifyleft", "justifycenter", "justifyright",
            "justifyfull", "insertorderedlist", "insertunorderedlist", "indent", "outdent", "subscript",
            "superscript", "-",
            "fontname", "fontsize", "textcolor", "bgcolor", "bold",
            "italic", "underline", "strikethrough", "removeformat", "selectall", "image",
            "flash", "media", "layer", "table",
            "emoticons", "link", "unlink"
        };

          string defaultWidth = "100%";
          int defaultHeight = 300;
          string id = "ke_editor";
          int toolButtonHeight = 27; //工具栏按钮每行高度
          private string[] m_toolbarItems;


          protected bool WyswygMode
          {
              get
              {
                  if (Parameters["WyswygMode"] == null)
                  {
                      return true;
                  }
                  return bool.Parse(Parameters["WyswygMode"].ToString());
              }
          }

          protected string Id
          {
              get
              {
                  if (Parameters["id"] !=null )
                  {
                      return Parameters["id"].ToString();
                  }
                  return id;
              }
          }

          string m_JsToolbarData = null;
          protected string JsToolbarData
          {
              get
              {
                  if (m_JsToolbarData == null)
                  {
                      m_JsToolbarData = JsonBuilder.GetJson(ToolbarItems);
                  }
                  return m_JsToolbarData;
              }
          }

         
          protected string[] ToolbarItems
          {
              get
              {
                  if(m_toolbarItems!=null) return  m_toolbarItems;
                  if (Parameters["items"] != null)
                  {
                      string s = (string)Parameters["items"];
                      s = s.Replace(" ", "");
                      m_toolbarItems = s.Split(',');
                  }
                  else
                  {
                      m_toolbarItems = defaultToolbarItems;
                  }
                  return m_toolbarItems;
              }
          }
          protected int ToolbarHeight
          {
              get
              {
                  return this.ToolbarGroup.Count * toolButtonHeight;
              }
          }
          protected int BodyHeight
          {
              get
              {
                  return this.Height - ToolbarHeight - 30;
              }
          }

        

          List< string[]> m_toolbarGroup;
          protected List<string[]> ToolbarGroup
          {
              get
              {
                  if (m_toolbarGroup == null)
                  {

                      string tg = string.Join(",", ToolbarItems);


                      List<string[]> group = new List<string[]>();

                      //group.AddRange(tg.Split('-'));
                      string[] temp;
                      temp = tg.Split('-');

                      foreach (string s in temp)
                      {
                          group.Add(s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                      }
                      m_toolbarGroup = group;
                  }

                  return m_toolbarGroup;
              
              }
          }

        }
        */
    }
}