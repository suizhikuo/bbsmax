//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using MaxLabs.WebEngine;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MaxLabs.bbsMax.Web.max_plugins.DefaultExtendedFields
{
	public partial class dropdown_frontend : ExtendedFieldControlBase
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		//private static readonly Regex s_SelectionValueRegex = new Regex(@"'([^'\\\r\n]*(?:\\.[^'\\\r\n]*)*)'(?>-'([^'\\\r\n]*(?:\\.[^'\\\r\n]*)*)')*");

		protected bool CommonScriptOutputed
		{
			get
			{
                object temp = null;

                if (PageCacheUtil.TryGetValue<object>("DropDownScriptOutputed", out temp) == false)
                {
                    PageCacheUtil.Set("DropDownScriptOutputed", new object());
                    return false;
                }

				return true;
			}
		}

		protected string SelectionDataToJson(string data)
		{
			bool debug = true;

            if (string.IsNullOrEmpty(data))
                return "{}";

			StringBuilder result = new StringBuilder(data.Length);

			result.Append("{");

			if (debug)
				result.AppendLine();

			int maxlevel = 0;

			int level = -1;

			for (int i = 0; i < data.Length; )
			{
				if (maxlevel < level)
					maxlevel = level;

				char c = data[i];

				switch (c)
				{
					case ' ':
					case '\t':
					case '\r':
					case '\n':
						i++;
						break;

					case '-':

						if (data[i + 1] == '-')
						{
							if (debug)
								result.Append("\t\t");

							if (level == 2)
								result.Append(',');

							int endOfText1 = 0;
							int endOfLine1 = 0;

							i = i + 2;

							IndexOfLineEnd(data, i, out endOfText1, out endOfLine1);

							result.Append("'");

                            result.Append(StringUtil.ToJavaScriptString(data .Substring(i,endOfText1-i+1)));

                            //for (int j = i; j <= endOfText1; j++)
                            //{
                            //    if (data[j] == '\'')
                            //        result.Append("\\'");
                            //    else
                            //        result.Append(data[j]);
                            //}

							result.Append("'");

							if (debug)
								result.AppendLine();

							level = 2;

							i = endOfText1 + 1;
						}
						else
						{
							if (debug)
								result.Append("\t");

							if (level == 1 || level == 2)
								result.Append("],");

							int endOfText1 = 0;
							int endOfLine1 = 0;

							i = i + 1;

							IndexOfLineEnd(data, i, out endOfText1, out endOfLine1);

							result.Append("'");

                            result.Append(StringUtil.ToJavaScriptString(data.Substring(i, endOfText1 - i + 1)));

                            //for (int j = i; j <= endOfText1; j++)
                            //{
                            //    if (data[j] == '\'')
                            //        result.Append("\\'");
                            //    else
                            //        result.Append(data[j]);
                            //}

							result.Append("':[");

							if (debug)
								result.AppendLine();

							level = 1;

							i = endOfText1 + 1;
						}

						break;

					default:
						if (level == 0)
						{
							result.Append("},");

							if (debug)
								result.AppendLine();
						}
						else if (level == 1 || level == 2)
						{
							if (debug)
								result.Append("\t");

							result.Append(']');

							if (debug)
								result.AppendLine();

							result.Append("},");
						}

						int endOfText = 0;
						int endOfLine = 0;

						IndexOfLineEnd(data, i, out endOfText, out endOfLine);

						result.Append("'");

                        result.Append(StringUtil.ToJavaScriptString(data.Substring(i, endOfText - i + 1)));

						//for (int j = i; j <= endOfText; j++)
						//{
                            //if (data[j] == '\'')
                            //    result.Append("\\'");
                            //else
                            //    result.Append(data[j]);
						//}

						result.Append("':{");

						if (debug)
							result.AppendLine();

						level = 0;

						i = endOfText + 1;

						break;
				}
			}

			if (level == 0)
				result.Append('}');
			else if (level == 1 || level == 2)
			{
				if (debug)
					result.Append("\t");

				result.Append("]");

				if (debug)
					result.Append("\r\n");

				result.Append("}");
			}

			if (debug)
				result.AppendLine();

			result.Append("}");

			return HttpUtility.HtmlEncode( result.ToString()) + "," + maxlevel;
		}

		private void IndexOfLineEnd(string str, int startIndex, out int endOfText, out int endOfLine)
		{
			int i = startIndex; 

			while (i < str.Length - 1)
			{
				i++;

				if (str[i] == '\r' || str[i] == '\n')
				{
					if (str[i + 1] == '\r' || str[i] == '\n')
					{
						endOfLine = i + 1;
					}
					else
					{
						endOfLine = i;
					}

					endOfText = i - 1;

					return;
				}
			}

			endOfText = str.Length - 1;
			endOfLine = -1;
		}

		//protected class DataNode
		//{
		//    public DataNode(string text)
		//    {
		//        Text = text.Replace("'", "\\'");
		//        Childs = new List<DataNode>();
		//    }

		//    public string Text;
		//    public List<DataNode> Childs;
		//}

		//protected DataNode ParseData(string dataText)
		//{

		//    string[] lines = dataText.Split('\n');

		//    DataNode data = new DataNode(string.Empty);
		//    DataNode level0 = null;
		//    DataNode level1 = null;
		//    DataNode level2 = null;

		//    foreach (string l in lines)
		//    {
		//        string line = l.Trim();

		//        if (line == string.Empty)
		//            continue;

		//        if (line.StartsWith("-"))
		//        {
		//            if (line.StartsWith("---"))
		//            {
		//                if (level2 != null)
		//                {
		//                    level2.Childs.Add(new DataNode(line.Substring(3)));
		//                }
		//            }
		//            else if (line.StartsWith("--"))
		//            {
		//                if (level1 != null)
		//                {
		//                    level2 = new DataNode(line.Substring(2));

		//                    level1.Childs.Add(level2);
		//                }
		//            }
		//            else
		//            {
		//                if (level0 != null)
		//                {
		//                    level1 = new DataNode(line.Substring(1));
		//                    level2 = null;

		//                    level0.Childs.Add(level1);
		//                }
		//            }
		//        }
		//        else
		//        {
		//            level0 = new DataNode(line);
		//            level1 = null;
		//            level2 = null;

		//            data.Childs.Add(level0);
		//        }
		//    }

		//    level0 = null;
		//    level1 = null;
		//    level2 = null;

		//    return data;
		//}

		//[TemplateFunction]
		//public string GenerateTree(string dataText, string selected)
		//{
		//    StringBuilder sb = new StringBuilder(dataText.Length);

		//    DataNode data = ParseData(dataText);

		//    GenerateTree(data, sb, selected);

		//    return sb.ToString();
		//}

		//protected void GenerateTree(DataNode data, StringBuilder output, string selected)
		//{
		//    Match match = s_SelectionValueRegex.Match(selected);

		//    int selectedLevel = -1;
		//    string selectedLevel0 = null;
		//    string selectedLevel1 = null;
		//    string selectedLevel2 = null;
		//    string selectedLevel3 = null;

		//    if (match.Success)
		//    {
		//        if (match.Groups[1].Success)
		//        {
		//            selectedLevel += 1;
		//            selectedLevel0 = match.Groups[1].Value;

		//            if (match.Groups[2].Success)
		//            {
		//                foreach (Capture capture in match.Groups[2].Captures)
		//                {
		//                    selectedLevel += 1;

		//                    switch (selectedLevel)
		//                    {
		//                        case 1:
		//                            selectedLevel1 = capture.Value;
		//                            break;

		//                        case 2:
		//                            selectedLevel2 = capture.Value;
		//                            break;

		//                        case 3:
		//                            selectedLevel3 = capture.Value;
		//                            break;
		//                    }
		//                }
		//            }
		//        }
		//    }

		//    for (int i = 0; i < data.Childs.Count; i++)
		//    {
		//        string level0 = data.Childs[i].Text;
		//        bool endLevel0 = i == data.Childs.Count - 1;
		//        bool level0Match = selectedLevel >= 0 && selectedLevel0 == level0;

		//        if (level0Match && selectedLevel == 0)
		//            output.Append("<option selected=\"selected\" value=\"'");
		//        else
		//            output.Append("<option value=\"'");

		//        output.Append(level0.Replace("\\'", "'")).Append("'\">");

		//        if (endLevel0)
		//            output.Append("└─");
		//        else
		//            output.Append("├─");

		//        output.Append(level0.Replace("\\'", "'")).AppendLine("</option>");

		//        for (int j = 0; j < data.Childs[i].Childs.Count; j++)
		//        {
		//            string level1 = data.Childs[i].Childs[j].Text;
		//            bool endLevel1 = j == data.Childs[i].Childs.Count - 1;
		//            bool level1Match = level0Match && selectedLevel1 == level1;

		//            if (level1Match && selectedLevel == 1)
		//                output.Append("<option selected=\"selected\" value=\"'");
		//            else
		//                output.Append("<option value=\"'");

		//            output.Append(level0).Append("'-'").Append(level1).Append("'\">");

		//            if (endLevel0)
		//                output.Append("&nbsp;&nbsp;");
		//            else
		//                output.Append("│");

		//            output.Append("&nbsp;&nbsp;");

		//            if (endLevel1)
		//                output.Append("└─");
		//            else
		//                output.Append("├─");

		//            output.Append(level1.Replace("\\'", "'")).AppendLine("</option>");

		//            for (int k = 0; k < data.Childs[i].Childs[j].Childs.Count; k++)
		//            {
		//                string level2 = data.Childs[i].Childs[j].Childs[k].Text;
		//                bool endLevel2 = k == data.Childs[i].Childs[j].Childs.Count - 1;
		//                bool level2Match = level1Match && selectedLevel2 == level2;

		//                if (level2Match && selectedLevel == 2)
		//                    output.Append("<option selected=\"selected\" value=\"'");
		//                else
		//                    output.Append("<option value=\"'");

		//                output.Append(level0).Append("'-'").Append(level1).Append("'-'").Append(level2).Append("'\">");

		//                if (endLevel0)
		//                    output.Append("&nbsp;&nbsp;");
		//                else
		//                    output.Append("│");

		//                output.Append("&nbsp;&nbsp;");

		//                if (endLevel1)
		//                    output.Append("&nbsp;&nbsp;");
		//                else
		//                    output.Append("│");

		//                output.Append("&nbsp;&nbsp;");

		//                if (endLevel2)
		//                    output.Append("└─");
		//                else
		//                    output.Append("├─");

		//                output.Append(level2.Replace("\\'","'")).AppendLine("</option>");

		//                for (int l = 0; l < data.Childs[i].Childs[j].Childs[k].Childs.Count; l++)
		//                {
		//                    string level3 = data.Childs[i].Childs[j].Childs[k].Childs[l].Text;
		//                    bool endLevel3 = l == data.Childs[i].Childs[j].Childs[k].Childs.Count - 1;
		//                    bool level3Match = level2Match && selectedLevel3 == level3;

		//                    if (level3Match && selectedLevel == 3)
		//                        output.Append("<option selected=\"selected\" value=\"'");
		//                    else
		//                        output.Append("<option value=\"'");

		//                    output.Append(level0).Append("'-'").Append(level1).Append("'-'").Append(level2).Append("'-'").Append(level3).Append("'\">");

		//                    if (endLevel0)
		//                        output.Append("&nbsp;&nbsp;");
		//                    else
		//                        output.Append("│");

		//                    output.Append("&nbsp;&nbsp;");

		//                    if (endLevel1)
		//                        output.Append("&nbsp;&nbsp;");
		//                    else
		//                        output.Append("│");

		//                    output.Append("&nbsp;&nbsp;");

		//                    if (endLevel2)
		//                        output.Append("&nbsp;&nbsp;");
		//                    else
		//                        output.Append("│");

		//                    output.Append("&nbsp;&nbsp;");

		//                    if (endLevel3)
		//                        output.Append("└─");
		//                    else
		//                        output.Append("├─");

		//                    output.Append(level3.Replace("\\'", "'")).AppendLine("</option>");
		//                }
		//            }
		//        }
		//    }
		//}
	}
}