//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Text;
using System.Reflection;
using System.Collections.Specialized;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Enums;


namespace MaxLabs.bbsMax.Filters
{
    public class FilterBase<T> where T : FilterBase<T>, new()
	{
        protected FilterBase()
        {
            this.IsNull = true;
        }

        public bool IsNull { get; private set; }

		public static T GetFromForm()
		{
			T obj = new T();

			obj.DoGetFromForm();

			return obj;
		}

		public static T GetFromFilter(string filterFormName)
		{
			string value = HttpContext.Current.Request.QueryString[filterFormName];
			
            T obj = new T();
			
            if (string.IsNullOrEmpty(value))
				return obj;

			obj.DoParse(value);

			return obj;
		}

        private int m_TotalResults = -1;

        [FilterItem]
        public int TotalResults
        {
            get { return m_TotalResults; }
            set { m_TotalResults = value; }
        }

		protected virtual void DoGetFromForm()
		{
			foreach (PropertyInfo propertyInfo in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
			{
				if (!propertyInfo.IsDefined(typeof(FilterItemAttribute), true))
					continue;

				FilterItemAttribute attribute = (FilterItemAttribute)(propertyInfo.GetCustomAttributes(typeof(FilterItemAttribute), true)[0]);
				
				string formName = attribute.FormName;
				
				if (string.IsNullOrEmpty(formName))
					formName = propertyInfo.Name;

				string text = HttpContext.Current.Request.Form[formName];

				if (text != null)
				{
                    //此时可以认为Filter不为空
                    if (IsNull) this.IsNull = false;

					switch (Type.GetTypeCode(propertyInfo.PropertyType))
					{
						case TypeCode.String:
							text = HttpUtility.HtmlEncode(text);
							propertyInfo.SetValue(this, text, null);
							break;
						default:
							//暂时先这么写了，以后要改善这部分的设计
                            if (attribute.FormType == FilterItemFormType.BeginDate || attribute.FormType == FilterItemFormType.EndDate)
                            {
                                DateTime? date = null;
                                if (!string.IsNullOrEmpty(text))
                                {
                                    using (ErrorScope es = new ErrorScope())
                                    {
                                        if (attribute.FormType == FilterItemFormType.EndDate)
                                        {
                                            date = DateTimeUtil.ParseEndDateTime(text);
                                        }
                                        else if (attribute.FormType == FilterItemFormType.BeginDate)
                                        {
                                            date = DateTimeUtil.ParseBeginDateTime(text);
                                        }
                                        es.IgnoreError<ErrorInfo>();
                                    }
                                    propertyInfo.SetValue(this, date, null);
                                }
                                break;
                            }


							if (propertyInfo.PropertyType == typeof(ExtendedFieldSearchInfo))
							{
								propertyInfo.SetValue(this, ExtendedFieldSearchInfo.Parse(text), null);
							}
							else
							{
								object objValue;
								using (ErrorScope es = new ErrorScope())
								{
									if (StringUtil.TryParse(propertyInfo.PropertyType, text, out objValue))
										propertyInfo.SetValue(this, objValue, null);

									es.IgnoreError<ErrorInfo>();
								}
							}
							break;
					}
				}
			}
		}

        public static T Parse(string text)
        {
            T filter = new T();
            filter.DoParse(text);
            return filter;
        }

		protected virtual void DoParse(string value)
		{
			if (string.IsNullOrEmpty(value))
				return;

			try
			{
				value = SecurityUtil.DesDecode(value);
			}
			catch
			{
				return;
			}

			int index = value.IndexOf('|');

			if (index < 1)
				return;

			int[] valueLengths = StringUtil.Split<int>(value.Substring(0, index), ',');

			if (valueLengths.Length == 0)
				return;

            //此时可以认为Filter不为空
            this.IsNull = false;

			index++;

			int i = 0;
            foreach (PropertyInfo propertyInfo in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
			{
				if (!propertyInfo.IsDefined(typeof(FilterItemAttribute), true))
					continue;

				if (valueLengths.Length <= i)
					break;

				int valueLength = valueLengths[i];

                FilterItemAttribute attribute = (FilterItemAttribute)(propertyInfo.GetCustomAttributes(typeof(FilterItemAttribute), true)[0]);

				string text = value.Substring(index, valueLength);
				switch (Type.GetTypeCode(propertyInfo.PropertyType))
				{
					case TypeCode.Boolean:
						if (text == "1")
							propertyInfo.SetValue(this, true, null);
						else
							propertyInfo.SetValue(this, false, null);
						break;

					case TypeCode.String:
						propertyInfo.SetValue(this, text, null);
						break;
					default:
						//暂时先这么写了，以后要改善这部分的设计

                        if (attribute.FormType == FilterItemFormType.BeginDate || attribute.FormType == FilterItemFormType.EndDate)
                        {
                            DateTime? date = null;
                            if (!string.IsNullOrEmpty(text))
                            {
                                using (ErrorScope es = new ErrorScope())
                                {
                                    if (attribute.FormType == FilterItemFormType.EndDate)
                                    {
                                        date = DateTimeUtil.ParseEndDateTime(text);
                                    }
                                    else if (attribute.FormType == FilterItemFormType.BeginDate)
                                    {
                                        date = DateTimeUtil.ParseBeginDateTime(text);
                                    }
                                    es.IgnoreError<ErrorInfo>();
                                }
                                propertyInfo.SetValue(this, date, null);
                            }
                            break;
                        }

						if (propertyInfo.PropertyType == typeof(ExtendedFieldSearchInfo))
						{
							propertyInfo.SetValue(this, ExtendedFieldSearchInfo.Parse(text), null);
						}
						else
						{
							object objValue;
							using (ErrorScope es = new ErrorScope())
							{
								if (StringUtil.TryParse(propertyInfo.PropertyType, text, out objValue))
									propertyInfo.SetValue(this, objValue, null);

								es.IgnoreError<ErrorInfo>();
							}
						}
						break;
				}

				index += valueLength;
				i++;
			}
		}

        public override string ToString()
        {
            StringBuilder indexBuilder = new StringBuilder();
            StringBuilder valueBuilder = new StringBuilder();
            foreach (PropertyInfo propertyInfo in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
            {
                if (!propertyInfo.IsDefined(typeof(FilterItemAttribute), true))
                    continue;

                object value = propertyInfo.GetValue(this, null);
                if (value == null)
                {
                    indexBuilder.Append(0).Append(",");
                    continue;
                }

                string text;
                switch (Type.GetTypeCode(propertyInfo.PropertyType))
                {
                    case TypeCode.Boolean:
                        if ((bool)propertyInfo.GetValue(this, null) == true)
                            text = "1";
                        else
                            text = "0";
                        break;

                    default:
                        text = value.ToString();
                        break;
                }

                indexBuilder.Append(text.Length).Append(",");
                valueBuilder.Append(text);
            }

            if (indexBuilder.Length > 0)
                indexBuilder.Remove(indexBuilder.Length - 1, 1);

            indexBuilder.Append("|").Append(valueBuilder.ToString());

            return SecurityUtil.DesEncode(indexBuilder.ToString());
        }

        /// <summary>
        /// 获取搜索结果后确认跳转,后带搜索条件的参数
        /// </summary>
        /// <param name="formName">搜索条件网址参数名称</param>
        /// <param name="filterValue">序列化的搜索条件值</param>
        public void Apply(string filterParamName, string pageParamName)
        {
            UrlScheme scheme = BbsRouter.GetCurrentUrlScheme();

            scheme.AttachQuery(filterParamName, this.ToString());
            scheme.AttachQuery(pageParamName, "1");

            HttpContext.Current.Response.Redirect(scheme.ToString());
        }


        /// <summary>
        /// 获取搜索结果后确认跳转,后带搜索条件的参数
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="extraQuery">附带的查询参数</param>
        /// <param name="extraValue">附带的参数值</param>
        public void Apply(string filterParamName, string pageParamName,string extraQuery, string extraValue)
        {
            UrlScheme scheme = BbsRouter.GetCurrentUrlScheme();

            scheme.AttachQuery(filterParamName, this.ToString());
            scheme.AttachQuery(pageParamName, "1");
            scheme.AttachQuery(extraQuery, extraValue);

            HttpContext.Current.Response.Redirect(scheme.ToString());
        }
    }
}