//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Text;
using MaxLabs.bbsMax.Enums;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using MaxLabs.bbsMax.Settings;


namespace MaxLabs.bbsMax.Entities
{
    public class ExtensionList : StringCollection, IExceptableSettingItem
    {
        public static readonly ExtensionList Notset;
        public static readonly ExtensionList AllowAll;

        static ExtensionList()
        {
            Notset = new ExtensionList();
            AllowAll = new ExtensionList();
            AllowAll.Add("*");
        }


        public ExtensionList() { }
        public ExtensionList(string[] extensions)
        {
            if (extensions != null)
                foreach (string s in extensions)
                    Add(s);
        }

        public new void Add(string extension)
        {
            if (string.IsNullOrEmpty(extension))
                return;

            if (extension.StartsWith("."))
                extension = extension.Substring(1);

            extension = extension.ToLower();

            if (!base.Contains(extension))
                base.Add(extension);
        }

        public new void AddRange(string[] extensions)
        {
            if (extensions == null || extensions.Length == 0)
                return;

            foreach (string extension in extensions)
            {
                this.Add(extension);
            }
        }

        public new bool Contains(string extension)
        {
            if (string.IsNullOrEmpty(extension))
                return false;

            if (base.Contains("*"))
                return true;

            if (extension.StartsWith("."))
                extension = extension.Substring(1);

            extension = extension.ToLower();

            return base.Contains(extension);
        }

        public new int IndexOf(string extension)
        {
            if (string.IsNullOrEmpty(extension))
                return -1;

            if (extension.StartsWith("."))
                extension = extension.Substring(1);

            extension = extension.ToLower();

            return base.IndexOf(extension);
        }

        public new void Insert(int index, string extension)
        {
            if (string.IsNullOrEmpty(extension))
                return;

            if (extension.StartsWith("."))
                extension = extension.Substring(1);

            extension = extension.ToLower();

            base.Insert(index, extension);
        }

        public new void Remove(string extension)
        {
            if (string.IsNullOrEmpty(extension))
                return;

            if (extension.StartsWith("."))
                extension = extension.Substring(1);

            extension = extension.ToLower();

            base.Remove(extension);
        }

        public static ExtensionList Combine(ExtensionList list1, ExtensionList list2)
        {
            if (list1.Contains("*") || list2.Contains("*"))
                return ExtensionList.AllowAll;

            ExtensionList newList = new ExtensionList();

            foreach (string extension in list1)
            {
                if (!newList.Contains(extension))
                    newList.Add(extension);
            }

            foreach (string extension in list2)
            {
                if (!newList.Contains(extension))
                    newList.Add(extension);
            }

            return newList;
        }

        public override string ToString()
        {
            return this.ToString(",");
        }

        public string ToString(string spliter)
        {
            bool isFirst = true;
            StringBuilder sb = new StringBuilder();
            foreach (string extension in this)
            {
                if (isFirst)
                    isFirst = false;
                else
                    sb.Append(spliter);

                sb.Append(extension);
            }

            return sb.ToString();
        }

        public string GetFileTypeForSwfUpload()
        {
            StringBuffer sb = new StringBuffer(string.Empty);
            foreach (string f in this)
            {
                sb.InnerBuilder.Append("*." + f + ";");
            }

            return sb.ToString().TrimEnd(';');
        }

        public static ExtensionList Parse(string value)
        {
            ExtensionList list = new ExtensionList();
            if (value != string.Empty)
            {

                string[] listArr = value.Trim().Split(',');
                foreach (string arr in listArr)
                {
                    if (arr.IndexOf("\r\n") == -1)
                    {
                        if (arr.Trim() != "" && !list.Contains(arr.Trim()))
                            list.Add(arr.Trim());
                    }
                    else
                    {
                        string[] strs = Regex.Split(arr, "\r\n");
                        foreach (string str1 in strs)
                        {
                            if (str1.Trim() != "" && !list.Contains(str1.Trim()))
                                list.Add(str1.Trim());
                        }
                    }
                }
            }

            return list;
        }


        #region ISettingItem 成员

        public string GetValue()
        {
            return this.ToString();
        }

        public void SetValue(string value)
        {
            this.Clear();
            string[] items = value.Split(',');
            foreach (string item in items)
            {
                if (item.Trim() != string.Empty)
                {
                    this.Add(item);
                }
            }
        }

        #endregion

        #region ICloneable 成员

        public object Clone()
        {
            ExtensionList temp = new ExtensionList();
            foreach (string item in this)
            {
                temp.Add(item);
            }
            return temp;
        }

        #endregion
    }
}