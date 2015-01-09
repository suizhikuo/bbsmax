//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Web;
//using System.Collections;

//using MaxLabs.WebEngine;
//using MaxLabs.bbsMax.Entities;
//using MaxLabs.bbsMax.ExtendedFieldControls;
//using MaxLabs.bbsMax.Enums;
//using MaxLabs.bbsMax.Settings;

//namespace MaxLabs.bbsMax.Templates
//{
//    [TemplatePackage]
//    public class ExtendedFieldTemplateMembers
//    {

//        //public delegate void ExtendedFieldListTemplate(int i, ExtendedField field);

//        ///// <summary>
//        ///// 扩展字段列表
//        ///// </summary>
//        //[TemplateTag]
//        //public void ExtendedFieldList(bool? outputJS/*输出js*/, bool? isSearch, ExtendedFieldListTemplate template)
//        //{
//        //    StringBuffer js = new StringBuffer("<script type=\"text/javascript\" language=\"javascript\">");
//        //    int i = 0;
//        //    ExtendedFieldCollection fields = ExtendedFieldBO.Instance.GetAllExtendedFields();
            
//        //    User user = null;
//        //    if (isSearch != true)
//        //        user = User.Current;
//        //    foreach (ExtendedField field in fields)
//        //    {
//        //        //获取控件html代码默认值
//        //        string[] values = new string[1];
//        //        if (user != null && user.ID > 0)
//        //        {
//        //            if (field.ChildFields.Count > 0)
//        //            {
//        //                values = new string[field.ChildFields.Count];
//        //                for (int t = 0; t < field.ChildFields.Count; t++)
//        //                    values[t] = user.ExtendedFields[field.ChildFields[t].ID.ToString()];
//        //            }
//        //            else
//        //                values[0] = user.ExtendedFields[field.ID.ToString()];
//        //        }
//        //        else
//        //        {
//        //            HttpRequest request = HttpContext.Current.Request;
//        //            if (field.ChildFields.Count > 0)
//        //            {
//        //                values = new string[field.ChildFields.Count];
//        //                for (int t = 0; t < field.ChildFields.Count; t++)
//        //                    values[t] = request.Form[string.Format(Consts.ExtendedFieldID, field.ChildFields[t].ID)];
//        //            }
//        //            else
//        //                values[0] = request.Form[string.Format(Consts.ExtendedFieldID, field.ID.ToString())];
//        //        }
                

//        //        field.ControlCode = ExtendedFieldBO.Instance.GetControlCode(field, values, OutputType.OutputHtml);
//        //        if (outputJS != false)
//        //            js += ExtendedFieldBO.Instance.GetControlCode(field, values, OutputType.OutputJS);
//        //        template(i++, field);
//        //    }

//        //    if (outputJS != false) //输出js
//        //    {
//        //        js += ExtendedFieldBO.Instance.GetControlJS(fields);
//        //        js += "</script>";
//        //        HttpContext.Current.Response.Write(js.ToString());
//        //    }
//        //}

//        //public delegate void UserExtendedFieldListTemplate(int i, ExtendedField field);
//        ///// <summary>
//        ///// 用户的扩展字段
//        ///// </summary>
//        //[TemplateTag]
//        //public void UserExtendedFieldList(string userID, UserExtendedFieldListTemplate template)
//        //{
//        //    int id = StringUtil.GetInt(userID, 0);
//        //    if (id <= 0)
//        //        id = UserBO.Instance.GetUserID();            

//        //    User user = UserBO.Instance.GetUser(id);
//        //    ExtendedFieldCollection fields = ExtendedFieldBO.Instance.GetAllExtendedFields();
//        //    int i = 0;
//        //    foreach (ExtendedField field in fields)
//        //    {
//        //        if (field.IsInvisible)
//        //            continue;

//        //        //控件html代码
//        //        string[] values = new string[1];
//        //        if (user != null && user.ID > 0)
//        //        {
//        //            if (field.ChildFields.Count > 0)
//        //            {
//        //                values = new string[field.ChildFields.Count];
//        //                for (int t = 0; t < field.ChildFields.Count; t++)
//        //                    values[t] = user.ExtendedFields[field.ChildFields[t].ID.ToString()];
//        //            }
//        //            else
//        //                values[0] = user.ExtendedFields[field.ID.ToString()];
//        //        }

//        //        field.ControlCode = ExtendedFieldBO.Instance.GetControlCode(field, values, OutputType.OutputValue);
//        //        if (!string.IsNullOrEmpty(field.ControlCode))
//        //            template(i++, field);
//        //    }
//        //}

//        //public delegate void ExtendedFieldViewTemplate(ExtendedField field);
//        //[TemplateTag]
//        //public void ExtendedFieldView(ExtendedFieldViewTemplate template)
//        //{
//        //    int id = 0;
//        //    if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["id"]))
//        //        int.TryParse(HttpContext.Current.Request.QueryString["id"], out id);
//        //    if (id > 0)
//        //    {
//        //        ExtendedField field = ExtendedFieldBO.Instance.GetExtendedField(id);
//        //        if (field.ControlType == FieldControlType.HasChildren)
//        //        {
//        //            HttpContext.Current.Response.Write("<script type=\"text/javascript\">");
//        //            if (field.ChildFields.Count > 0 && !string.IsNullOrEmpty(field.ChildFields[0].Content))
//        //            {
//        //                StringBuilder str = new StringBuilder();
//        //                foreach (string s in field.ChildFields[0].Content.Split(','))
//        //                {
//        //                    str.Append("'");
//        //                    str.Append(s);
//        //                    str.Append("'");
//        //                    str.Append(",");
//        //                }
//        //                if (str.Length > 1)
//        //                    str.Remove(str.Length - 1, 1);

//        //                HttpContext.Current.Response.Write("Array0 = new Array(" + str + ");");
//        //            }
//        //            if (field.ChildFields.Count > 1)
//        //                HttpContext.Current.Response.Write("Array1 = new Array(" + field.ChildFields[1].Content + ");");
//        //            if (field.ChildFields.Count > 2)
//        //                HttpContext.Current.Response.Write("Array2 = new Array(" + field.ChildFields[2].Content + ");");
//        //            HttpContext.Current.Response.Write("</script>");
//        //        }
//        //        template(field);
//        //    }
//        //    else
//        //        template(new ExtendedField());
//        //}

//        //[TemplateFunction]
//        //public string Print_Attribute(string attribute,string key)
//        //{
//        //    StringTable items = StringTable.Parse(attribute);
//        //    if (items.ContainsKey(key))
//        //        return items[key];
//        //    else
//        //        return string.Empty;
//        //}
//    }
//}