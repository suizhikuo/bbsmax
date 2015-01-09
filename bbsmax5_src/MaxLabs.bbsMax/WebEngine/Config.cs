//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Configuration;
using System.Collections.Generic;

namespace MaxLabs.WebEngine
{
	public class Config
	{
		private static Config s_Current;

		public static Config Current
		{
			get 
			{
				if (s_Current == null)
					s_Current = new Config();

				return s_Current;
			}

			set
			{
				if (s_Current != null)
					return;

				s_Current = value;
			}
		}

		private string[] m_TemplateImports;

		public string[] TemplateImports
		{
			get 
			{
				if (m_TemplateImports == null)
				{
					m_TemplateImports = GetTemplateImports();

					if (m_TemplateImports == null)
						m_TemplateImports = new string[0];
				}

				return m_TemplateImports;
			}
		}

		protected virtual string[] GetTemplateImports()
		{
			string setting = ConfigurationManager.AppSettings[Consts.TemplateImports];

			if (string.IsNullOrEmpty(setting) == false)
				return setting.Split(';');

			return null;
		}

		private string[] m_TemplatePackages;

		public string[] TemplatePackages
		{
			get
			{
				if (m_TemplatePackages == null)
				{
					m_TemplatePackages = GetTemplatePackages();

					if(m_TemplatePackages == null)
						m_TemplatePackages = new string[0];
				}

				return m_TemplatePackages;
			}
		}

		protected virtual string[] GetTemplatePackages()
		{
			string setting = ConfigurationManager.AppSettings[Consts.TemplatePackages];

			if (string.IsNullOrEmpty(setting) == false)
				return setting.Split(';');

			return null;
		}

		private string m_TemplateOutputPath;

		public string TemplateOutputPath
		{
			get
			{
				if (m_TemplateOutputPath == null)
				{
					m_TemplateOutputPath = GetTemplateOutputPath();

					if (string.IsNullOrEmpty(m_TemplateOutputPath))
						m_TemplateOutputPath = "~/max-temp/parsed-template/";
				}

				return m_TemplateOutputPath;
			}
		}

		protected virtual string GetTemplateOutputPath()
		{
			return ConfigurationManager.AppSettings[Consts.TemplateOutputPath];
		}

        //private string[] m_TemplateDirectories;

        //public string[] TemplateDirectoies
        //{
        //    get
        //    {
        //        if (m_TemplateDirectories == null)
        //        {
        //            m_TemplateDirectories = GetTemplateDirectories();

        //            if (m_TemplateDirectories == null)
        //                m_TemplateDirectories = new string[0];
        //        }

        //        return m_TemplateDirectories;
        //    }
        //}

        //protected virtual string[] GetTemplateDirectories()
        //{
        //    string setting = ConfigurationManager.AppSettings[Consts.TemplateDirectories];

        //    if (string.IsNullOrEmpty(setting) == false)
        //        return setting.Split(';');

        //    return null;
        //}
	}
}