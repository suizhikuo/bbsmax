//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.IO;
using System.Web;
using System.Text;
using System.Web.Caching;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using MaxLabs.bbsMax;
using MaxLabs.bbsMax.RegExp;
using MaxLabs.bbsMax.Entities;
using System.Collections.Specialized;
using System.Collections;
using MaxLabs.bbsMax.Common;


namespace MaxLabs.WebEngine.Template
{
	/// <summary>
	/// 模板文件信息
	/// </summary>
	public class TemplateFile 
	{
		private static readonly Regex s_CSharpNameSpaceRegex = new CSharpNameSpaceRegex();
		private static readonly Regex s_CSharepPageClassRegex = new CSharpPageClassRegex();

		public TemplateFile(string filePath) : this(filePath, null)
		{
		}

        public TemplateFile(string filePath, TemplateFile baseFile)
        {
			FilePath = filePath;

            string virtualPath = filePath.Substring(Globals.ApplicationPath.Length).Replace('\\', '/');

            if (StringUtil.StartsWith(virtualPath, '/'))
                virtualPath = "~" + virtualPath;

            else
                virtualPath = "~/" + virtualPath;

            VirtualPath = virtualPath;

            m_BaseFile = baseFile;
        }

        private TemplateFile m_BaseFile;

		/// <summary>
		/// 获取是否是asp.net用户控件
		/// </summary>
		public bool IsControl 
		{
			get { return StringUtil.EndsWithIgnoreCase(FileName, ".ascx"); } 
		}

		/// <summary>
		/// 获取模板文件的物理路径，即包含盘符的路径，如：c:\template.aspx
		/// </summary>
		public string FilePath { get; private set; }

		/// <summary>
		/// 获取模板文件名，包含文件扩展名，如：template.aspx
		/// </summary>
		//public string FileName { get; private set; }
        public string FileName { get { return Path.GetFileName(FilePath); } }

		private bool m_IsParsed;
        
        //private CacheDependency m_Watcher = null;

        //private bool FileChanged
        //{
        //    get
        //    {
        //        return m_Watcher.HasChanged;
        //    }
        //}

        //private void ResetWatcher()
        //{

        //    List<string> watchFiles = new List<string>();

        //    watchFiles.Add(GetFilePathForWatcher());

        //    GetIncludeFilePathForWatcher(watchFiles);

        //    m_Watcher = new CacheDependency(watchFiles.ToArray());

        //}

        private void GetIncludeFilePathForWatcher(List<string> result)
        {
            if (m_IncludedFiles != null)
            {
                foreach (TemplateFile includedFile in m_IncludedFiles)
                {
                    result.Add(includedFile.GetFilePathForWatcher());

                    includedFile.GetIncludeFilePathForWatcher(result);
                }
            }
        }

        private string GetFilePathForWatcher()
        {
            string watchFilePath = FilePath;

            if (m_BaseFile != null && File.Exists(watchFilePath) == false)
                watchFilePath = m_BaseFile.FilePath;

            return watchFilePath;
        }

		/// <summary>
		/// 获取或设置模板文件是否已被解析
		/// </summary>
		public bool IsParsed 
		{
            get { return m_IsParsed; }
			//{
                //if (m_IsParsed)
                //{
                    //if (FileChanged)
                    //{
                    //    lock (this)
                    //    {
                    //        m_IsParsed = false;
                    //        //m_Watchers.Clear();
                    //        m_Watcher = null;

                    //        //模板源文件已经不存在
                    //        if (File.Exists(FilePath) == false)
                    //            File.Delete(ParsedFilePath);
                    //    }

                    //    return false;
                    //}

                //    return true;
                //}

                //return false;
			//}

			set 
			{
                if (TemplateManager.IsPreParsing == false)
                {
                    m_IsParsed = value;

                    //if (m_IsParsed)
                    //{
                    //    ResetWatcher();
                    //}
                }
			} 
		}

        /// <summary>
        /// 获取模板文件所属的模板文件夹
        /// </summary>
        public string VirtualDirectoryPath
        {
            get { return VirtualPathUtility.GetDirectory(VirtualPath); }
        }


		private string m_ParsedFileVirtualPath;

		/// <summary>
		/// 获取模板文件解析后的文件所在的虚拟路径，如：~/max-temp/parsed-template/template.aspx
		/// </summary>
		public string ParsedFileVirtualPath 
		{
			get 
			{
                if (m_BaseFile != null && m_IncludedSkinFile == false)
                {
                    return m_BaseFile.ParsedFileVirtualPath;
                }
                else
                {
                    if (string.IsNullOrEmpty(m_ParsedFileVirtualPath))
                    {
                        m_ParsedFileVirtualPath = string.Concat(Config.Current.TemplateOutputPath, VirtualDirectoryPath.Substring(2), FileName);

                        string folder = Path.GetDirectoryName(IOUtil.MapPath(m_ParsedFileVirtualPath));

                        if (Directory.Exists(folder) == false)
                            Directory.CreateDirectory(folder);
                    }

                    return m_ParsedFileVirtualPath;
                }
			}
		}
		
		private string m_ParsedFilePath;

		/// <summary>
		/// 获取模板文件解析后的文件所在的物理路径，如：C:\max-temp\parsed-template\template.aspx
		/// </summary>
		public string ParsedFilePath
		{
			get
			{
				if (string.IsNullOrEmpty(m_ParsedFilePath))
					m_ParsedFilePath = IOUtil.MapPath(ParsedFileVirtualPath);

				return m_ParsedFilePath;
			}
		}

		/// <summary>
		/// 获取模板文件的虚拟路径
		/// </summary>
        public string VirtualPath { get; private set; }

		public string CodeFilePath
		{
            get
            {
                if (m_BaseFile != null)
                    return m_BaseFile.CodeFilePath;

                if (VirtualPath.StartsWith("~/max-templates/"))
                {
                    int i = VirtualPath.IndexOf("/", "~/max-templates/".Length);
                    return IOUtil.JoinPath(Globals.GetPath(SystemDirecotry.Code), VirtualPath.Substring(i + 1) + ".cs");
                }

                else
                    return FilePath + ".cs";

            }
		}

		/// <summary>
		/// 获取模板文件的代码文件是否存在
		/// </summary>
        public bool CodeFileExists
        {
            get { return File.Exists(CodeFilePath); }
        }

        private string m_CodeFileInherits = null;

		/// <summary>
		/// 获取模板文件的父类
		/// </summary>
		public string CodeFileInherits
		{
			get 
			{
                string codeFileInherits = m_CodeFileInherits;

                if (codeFileInherits == null)
                {

                    if (CodeFileExists)
                    {
                        string codeFileContent = File.ReadAllText(CodeFilePath, Encoding.Default);

                        Match matchNameSpace = s_CSharpNameSpaceRegex.Match(codeFileContent);
                        Match matchClassName = s_CSharepPageClassRegex.Match(codeFileContent);

                        if (matchClassName.Success)
                        {
                            if (matchNameSpace.Success && matchNameSpace.Groups["name"].Success)
                                codeFileInherits = string.Concat(matchNameSpace.Groups["name"].Value, ".", matchClassName.Groups["name"].Value);
                            else
                                codeFileInherits = matchClassName.Groups["name"].Value;
                        }
                    }

                    if (codeFileInherits == null)
                    {
                        if (IsControl)
                            codeFileInherits = typeof(PagePartBase).FullName;
                        else
                            codeFileInherits = typeof(PageBase).FullName;
                    }

                    m_CodeFileInherits = codeFileInherits;
                }

                return codeFileInherits;
			}
        }

        private Hashtable m_Skins = null;

        public void AddSkin(string name, TemplateFile skin)
        {
            if (m_Skins == null)
                m_Skins = CollectionsUtil.CreateCaseInsensitiveHashtable();

            m_Skins.Add(name, skin);
        }

        public TemplateFile GetSkin(string name)
        {
            if (m_Skins == null)
                return this;

            if (m_Skins.ContainsKey(name) == false)
                return this;

            return (TemplateFile)m_Skins[name];
        }

        private static readonly Regex s_MatchInclude = new TemplateMSIncludeTagRegex();
        private static readonly Regex s_MatchPreInclude = new TemplateIncludeTagRegex();
        private static readonly Regex s_MatchNotes = new TemplateNoteRegex();

        private bool m_IncludedSkinFile = false;
        private List<TemplateFile> m_IncludedFiles = null;

        public string GetIncludeFileVirtualPath(string includeVirtualPath)
        {
            //string result = includeVirtualPath;

            //if (includeVirtualPath.StartsWith("/"))
            //{
            //    result = "~" + includeVirtualPath;
            //}

            if (includeVirtualPath.StartsWith("~/"))
                return UrlUtil.ConvertDots(includeVirtualPath);

            if (includeVirtualPath.StartsWith("/"))
                return UrlUtil.ConvertDots("~" + includeVirtualPath);


            string temp = IOUtil.JoinPath(Path.GetDirectoryName(this.FilePath), includeVirtualPath);

            if (m_BaseFile != null && File.Exists(temp) == false)
            {
                temp = IOUtil.JoinPath(Path.GetDirectoryName(m_BaseFile.FilePath), includeVirtualPath);
            }

            temp = new DirectoryInfo(temp).FullName;

            temp = temp.Substring(Globals.ApplicationPath.Length);

            return UrlUtil.JoinUrl("~/", temp);

            //return result;
        }

        public string GetFullTemplate(string skinID)
        {
            m_IncludedSkinFile = false;
            m_IncludedFiles = new List<TemplateFile>();

            string path = this.FilePath;

            if (m_BaseFile != null)
            {
                if (File.Exists(path) == false)
                {
                    path = m_BaseFile.FilePath;
                }
                else
                {
                    m_IncludedSkinFile = true;
                }
            }

            string content = File.ReadAllText(path, Encoding.Default);

            MatchEvaluator callback = delegate(Match match)
            {
                string includeVirtualPath = match.Groups[1].Value;

                includeVirtualPath = GetIncludeFileVirtualPath(includeVirtualPath);

                TemplateFile includeFile = null;

                if (TemplateManager.GetTemplateFiles().TryGetValue(includeVirtualPath, out includeFile))
                {
                    if (skinID != MaxLabs.bbsMax.Consts.DefaultSkinID)
                        includeFile = includeFile.GetSkin(skinID);

                    string includePath = IOUtil.MapPath(includeVirtualPath);

                    //includeFile.ResetWatcher();

                    m_IncludedFiles.Add(includeFile);

                    string result = includeFile.GetFullTemplate(skinID);

                    if (m_IncludedSkinFile == false && includeFile.m_IncludedSkinFile)
                        m_IncludedSkinFile = true;

                    //开始处理包含文件的参数
                    string includeParams = match.Groups["param"].Value;
                    MatchCollection matchs = Pool<TemplateAttributeListRegex>.Instance.Matches(includeParams);

                    foreach (Match param in matchs)
                    {
                        //应该计算索引位置，否则会存在误判和错误替换
                        result = result.Replace("$" + param.Groups["name"].Value, param.Groups["value"].Value);
                    }

                    return result;
                }
                else
                    return string.Empty;
            };

            //content = regex_pager.Replace(content, delegate(Match match) {
            //    return OnMatchPager(match, skinID);
            //});
            content = regex_pager4.Replace(content, delegate(Match match)
            {
                return OnMatchPager(match, skinID);
            });

            content = s_MatchNotes.Replace(content, string.Empty);

            content = s_MatchInclude.Replace(content, callback);

            content = s_MatchPreInclude.Replace(content, callback);

            content = regex_fast_css.Replace(content, new MatchEvaluator(ReplaceFast));
            content = regex_fast_js.Replace(content, new MatchEvaluator(ReplaceFast));



            content = content.Trim();

            return content;

            //StringBuilder sb = new StringBuilder();

            //string vpath = path.Substring(Globals.GetPath(SystemDirecotry.Root).Length);

            //sb.AppendLine("<!--begin ").Append(vpath).AppendLine("-->");
            //sb.Append(content.Trim());
            //sb.AppendLine("<!--end ").Append(vpath).AppendLine("-->");

            //return sb.ToString();
        }

        string ReplaceFast(Match m)
        {
            return string.Concat(m.Groups["begin"].Value, m.Groups["fast"].Value, "<%= _FastAspx %>", m.Groups["end"].Value);
        }

        const string GN_PARAM_NAME = "name";
        const string GN_PARAM_VALUE = "value";
        const string REGEX_PARSE_PARAM = "(?>\"[^\"\\\\\r\n]*(?:\\\\.[^\"\\\\\r\n]*)*\")|(?>'[^'\\\\\r\n]*(?:\\\\.[^'\\\\\r\n]*)*')|(?<name>[\\w]+)\\x20*=\\x20*(?>\"(?<value>[^\"\\\\\r\n]*(?:\\\\.[^\"\\\\\r\n]*)*)\"|'(?<value>[^'\\\\\r\n]*(?:\\\\.[^'\\\\\r\n]*)*)')";
        
        static readonly Regex regex_param = new Regex(REGEX_PARSE_PARAM, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        //static readonly Regex regex_pager = new Regex("<!--\\x20*\\#\\x20*pager(?<param>(?>\\x20+[\\w]+\\x20*=\\x20*(?>(?>\"[^\"\\\\\r\n]*(?:\\\\.[^\"\\\\\r\n]*)*\")|(?>'[^'\\\\\r\n]*(?:\\\\.[^'\\\\\r\n]*)*')|[\\w]+))*)\\x20*-->", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        static readonly Regex regex_pager4 = new Regex("<!--\\x20*\\[\\x20*pager(?<param>(?>\\x20+[\\w]+\\x20*=\\x20*(?>(?>\"[^\"\\\\\r\n]*(?:\\\\.[^\"\\\\\r\n]*)*\")|(?>'[^'\\\\\r\n]*(?:\\\\.[^'\\\\\r\n]*)*')|[\\w]+))*)\\x20*\\]-->", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        static readonly Regex regex_fast_css = new Regex(@"(?<begin><link[^>]*?href\s*=\s*""(?:(?!\.\.|/>|://)[^?])*?)(?<fast>\.css)(?<end>\s*"".*?>)", RegexOptions.IgnoreCase);
        static readonly Regex regex_fast_js = new Regex(@"(?<begin><script[^>]*?src\s*=\s*""(?:(?!\.\.|</|://)[^?])*?)(?<fast>\.js)(?<end>\s*"".*?>\s*</script>)", RegexOptions.IgnoreCase);

        private int ajaxLoaderIndex = 0;

        //private string ProcessStringParamVariable(string paramValue)
        //{
        //    if (paramValue == null)
        //        return string.Empty;

        //    //如果参数中不存在变量，直接返回
        //    if (paramValue.IndexOf('$') == -1)
        //    {
        //        return paramValue;
        //    }
        //    else
        //    {
        //        string[] ids = paramValue.Split(',');

        //        StringBuilder ajaxpanelids = new StringBuilder();
        //        foreach (string id in ids)
        //        {
        //            if (id.IndexOf('$') >= 0)
        //            {
        //                ajaxpanelids.Append("{maxtmpvar1}").Append(id).Append("{maxtmpvar0}").Append(",");
        //            }
        //            else
        //                ajaxpanelids.Append(id).Append(",");
        //        }

        //        if (ajaxpanelids.Length > 0)
        //            paramValue = ajaxpanelids.ToString(0, ajaxpanelids.Length - 1);
        //        else
        //            paramValue = "";

        //        return paramValue;
        //    }
        //}

        //private string ProcessNumberParamVariable(string paramValue, int numberDefaultValue)
        //{
        //    //数字类型的参数，要么整个都是变量，要么整个都是常量，不存在变量出现在中间的情况

        //    //如果不是变量
        //    if (paramValue.IndexOf('$') != 0)
        //    {
        //        int tempValue;
        //        if (int.TryParse(paramValue, out tempValue))
        //            return tempValue.ToString();
        //        else
        //            return numberDefaultValue.ToString();
        //    }
        //    else
        //    {
        //        return string.Concat("{maxtmpvar2}", paramValue, "{maxtmpvar0}");
        //    }
        //}

        private void AppendPagerStringParam(StringBuilder builder, string paramValue, bool isLastParam)
        {
            if (string.IsNullOrEmpty(paramValue))
                builder.Append("\"\"");

            else if (StringUtil.StartsWith(paramValue, '$'))
                builder.Append(paramValue);

            else
                builder.Append("\"").Append(paramValue).Append("\"");

            if (isLastParam == false)
                builder.Append(", ");
        }

        private void AppendPagerNumberParam(StringBuilder builder, string paramValue, bool isLastParam)
        {
            if (string.IsNullOrEmpty(paramValue))
                builder.Append("-1");

            else if (StringUtil.StartsWith(paramValue, '$'))
                builder.Append(paramValue);

            else
            {
                int intParamValue;

                if (int.TryParse(paramValue, out intParamValue))
                    builder.Append(paramValue);

                else
                    builder.Append("-1");

            }

            if (isLastParam == false)
                builder.Append(", ");
        }

        private string OnMatchPager(Match match, string skinID)
        {
            if (match.Groups["param"].Success == false)
                return match.Value;

            string param = match.Groups["param"].Value;

            string name = null;
            string skin = null;
            string ajaxpanelid = null;
            string ajaxloader = null;

            string buttonCount = null;
            string totalRecords = null;
            string pageSize = null;
            string pageNumber = null;
            
            string urlFormat = null;

            foreach (Match paramMatch in regex_param.Matches(param))
            {
                string paramName = paramMatch.Groups[GN_PARAM_NAME].Value;
                string paramValue = paramMatch.Groups[GN_PARAM_VALUE].Value;

                switch (paramName.ToLower())
                {
                    case "id":
                    case "name":
                        name = paramValue.ToLower();
                        break;
                    case "skin":
                        skin = paramValue.ToLower();
                        break;
                    case "ajaxpanelid":
                        ajaxpanelid = paramValue.ToLower();
                        break;
                    case "ajaxloader":
                        ajaxloader = paramValue.ToLower();
                        break;
                    case "buttoncount":
                        buttonCount = paramValue;
                        break;
                    case "count":
                    case "totalrecords":
                        totalRecords = paramValue;
                        break;
                    case "pagesize":
                        pageSize = paramValue;
                        break;
                    case "page":
                    case "pagenumber":
                        pageNumber = paramValue;
                        break;
                    case "urlformat":
                        urlFormat = paramValue.Replace("\"", "\\\"");
                        break;
                    default:
                        break;
                }
            }
            if (string.IsNullOrEmpty(name))
            {
                name = string.Concat("__max_", ajaxLoaderIndex.ToString());
                ajaxLoaderIndex++;
            }

            if (skin == null)
                skin = string.Empty;

            //if (string.IsNullOrEmpty(ajaxpanelid))
            //    ajaxpanelid = string.Empty;

            if (string.IsNullOrEmpty(ajaxloader))
            {
                ajaxloader = string.Concat("__max_", ajaxLoaderIndex.ToString());
                ajaxLoaderIndex++;
            }

            skin = GetIncludeFileVirtualPath(skin);

            TemplateFile skinFile = null;

            if (TemplateManager.GetTemplateFiles().TryGetValue(skin, out skinFile))
            {
                StringBuilder builder = new StringBuilder();

                builder.Append("{=$__SetCurrentPager(");

                AppendPagerStringParam(builder, name, false);
                AppendPagerStringParam(builder, ajaxpanelid, false);
                AppendPagerStringParam(builder, ajaxloader, false);

                AppendPagerNumberParam(builder, pageSize, false);
                AppendPagerNumberParam(builder, pageNumber, false);
                AppendPagerNumberParam(builder, totalRecords, false);
                AppendPagerNumberParam(builder, buttonCount, false);

                AppendPagerStringParam(builder, urlFormat, true);
                    //.Append(name).Append("\", ")

                    //.Append("\"").Append(ajaxpanelid).Append("\", ")

                    //.Append("\"").Append(ajaxloader).Append("\", ")

                
                    //.Append(pageSize).Append(", ")

                
                    //.Append(pageNumber).Append(", ")

                
                    //.Append(totalRecords).Append(", ")

                
                    //.Append(buttonCount).Append(", ")

                
                    //.Append(urlFormat.Replace("\"", "\\\"")).Append(")");

                builder.Append(")}");
                builder.Append(skinFile.GetFullTemplate(skinID));

                return builder.ToString();
            }

            return "找不到所需的包含文件";
        }

	}

}