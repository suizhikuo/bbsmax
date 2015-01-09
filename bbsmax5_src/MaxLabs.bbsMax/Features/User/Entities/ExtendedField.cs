//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Entities
{
    public class ExtendedField : SettingBase, IPrimaryKey<string>, IComparable<ExtendedField>
	{
		public ExtendedField()
		{
			Settings = new StringTable();
            DisplayType = ExtendedFieldDisplayType.AllVisible;
            IsHidden = false;
		}

		/// <summary>
		/// 字段名称
		/// </summary>
		[SettingItem]
		public string Name { get; set; }



		/// <summary>
		/// 描述
		/// </summary>
		[SettingItem]
		public string Description { get; set; }

		/// <summary>
		/// 是否必填
		/// </summary>
		[SettingItem]
		public bool IsRequired { get; set; }


        [SettingItem]
        public bool IsHidden { get; set; }

		///// <summary>
		///// 能否被搜索
        ///// </summary>
        [SettingItem]
		public bool Searchable { get; set; }

        ///// <summary>
        ///// 是否在用户资料页等用户信息显示的地方显示
        ///// </summary>
        //[SettingItem]
        //public bool DisplayInUserInfo { get; set; }
        [SettingItem]
        public ExtendedFieldDisplayType DisplayType { get; set; }

		/// <summary>
		/// 排序
		/// </summary>
		[SettingItem]
		public int SortOrder { get; set; }

		[SettingItem]
		public string Key { get; set; }

		/// <summary>
		/// 扩展字段的类型名
		/// </summary>
		[SettingItem]
		public string FieldTypeName { get; set; }

		/// <summary>
		/// 附加的控件设置，如最大长度，控件默认内容等于控件特性相关的东西
		/// </summary>
		[SettingItem]
		public StringTable Settings { get; set; }



        /// <summary>
        /// 是否是passport取来的
        /// </summary>
        public bool IsPassport { get; set; }


        private bool m_IsEnable;
        /// <summary>
        /// 如果是passport取来的 那么是否起用（只对passport取来的有效）
        /// </summary>
        public bool IsEnable 
        {
            get
            {
                if (IsPassport == false)
                    return true;
                else
                    return m_IsEnable;
            }
            set { m_IsEnable = value; }
        }


		///// <summary>
		///// 子集字段
		///// </summary>
		//private Collection<ChildField> childFields = new Collection<ChildField>();

		//public Collection<ChildField> ChildFields 
		//{
		//    get { return childFields; }
		//    set { childFields = value; } 
		//}

		//private ExtendedFieldChildCollection m_ChildFields = new ExtendedFieldChildCollection();

		/////// <summary>
		/////// 子集字段
		/////// </summary>
		//[SettingItem]
		//public ExtendedFieldChildCollection ChildFields
		//{
		//    get { return m_ChildFields; }
		//    set { m_ChildFields = value; }
		//}

		///// <summary>
		///// 显示的控件代码
		///// </summary>
		//[SettingItem]
		//public string ControlCode { get; set; }



        #region IPrimaryKey<string> 成员

        public string GetKey()
        {
            return Key;
        }

        #endregion

        #region IComparable<int> 成员

        public int CompareTo(ExtendedField other)
        {
            return this.SortOrder.CompareTo(other.SortOrder);
        }

        #endregion
    }

	#region 用户组对象集合
	/// <summary>
	/// 用户组对象集合
	/// </summary>
    public class ExtendedFieldCollection : EntityCollectionBase<string,ExtendedField>, ISettingItem
	{
        ///// <summary>
        ///// 加入时 排序
        ///// </summary>
        ///// <param name="item"></param>
        //public void AddItem(ExtendedField item)
        //{
        //    InsertItem(0, item);
        //}

        //protected override void InsertItem(int index, ExtendedField item)
        //{
        //    for (int i = 0; i < this.Count; i++)
        //    {
        //        if (this[i].SortOrder > item.SortOrder)
        //        {
        //            base.InsertItem(i, item);
        //            return;
        //        }
        //    }

        //    base.InsertItem(index, item);
        //}

		public ExtendedField[] ToArray()
		{
			List<ExtendedField> result = new List<ExtendedField>(this);

			return result.ToArray();
		}

		#region ISettingItem 成员

		public string GetValue()
		{
			StringList list = new StringList();

			foreach (ExtendedField item in this)
			{
				list.Add(item.ToString());
			}

			return list.ToString();
		}

		public void SetValue(string value)
		{
			StringList list = StringList.Parse(value);

			if (list != null)
			{
				foreach (string item in list)
				{
					ExtendedField field = new ExtendedField();

					field.Parse(item);
					this.Add(field);

				}
			}

            this.Sort();
		}


		#endregion
	}
	#endregion

	//public class ExtendedFieldChildCollection : Collection<ExtendedFieldChild>, ISettingItem
	//{
	//    #region ISettingItem 成员

	//    public string GetValue()
	//    {
	//        StringList list = new StringList();

	//        foreach (ExtendedFieldChild item in this)
	//        {
	//            list.Add(item.ToString());
	//        }

	//        return list.ToString();
	//    }

	//    public void SetValue(string value)
	//    {
	//        StringList list = StringList.Parse(value);

	//        if (list != null)
	//        {
	//            foreach (string item in list)
	//            {
	//                ExtendedFieldChild child = new ExtendedFieldChild();

	//                this.Add(child);

	//                child.Parse(item);
	//            }
	//        }
	//    }

	//    #endregion
	//}

	//public class ExtendedFieldChild : SettingBase, ISettingItem
	//{
	//    [SettingItem]
	//    public int ID { get; set; }

	//    [SettingItem]
	//    public string Name { get; set; }

	//    [SettingItem]
	//    public string Content { get; set; }

	//    #region ISettingItem 成员

	//    public string GetValue()
	//    {
	//        return base.ToString();
	//    }

	//    public void SetValue(string value)
	//    {
	//        base.Parse(value);
	//    }

	//    #endregion
	//}
}