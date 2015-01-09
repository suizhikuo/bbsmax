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
using MaxLabs.bbsMax.DataAccess;


namespace MaxLabs.bbsMax.Entities
{
    /// <summary>
    /// 分类信息
    /// </summary>
    public class ThreadCateModelField : IPrimaryKey<int>
    {
        public ThreadCateModelField()
        { }

        public ThreadCateModelField(DataReaderWrap readerWrap)
        {
            FieldID = readerWrap.Get<int>("FieldID");
            ModelID = readerWrap.Get<int>("ModelID");
            FieldKey = readerWrap.Get<Guid>("FieldKey");
            FieldName = readerWrap.Get<string>("FieldName");
            Enable = readerWrap.Get<bool>("Enable");
            SortOrder = readerWrap.Get<int>("SortOrder");
            FieldType = readerWrap.Get<string>("FieldType");
            FieldTypeSetting = readerWrap.Get<string>("FieldTypeSetting");
            Search = readerWrap.Get<bool>("Search");
            AdvancedSearch = readerWrap.Get<bool>("AdvancedSearch");
            DisplayInList = readerWrap.Get<bool>("DisplayInList");
            MustFilled = readerWrap.Get<bool>("MustFilled");
            Description = readerWrap.Get<string>("Description");

            ExtendedField field = new ExtendedField();
            field.Settings = StringTable.Parse(FieldTypeSetting);
            field.FieldTypeName = FieldType;
            field.Key = "fieldKey_"+FieldID.ToString();//FieldKey.ToString();

            ExtendedField = field;
        }


        public int ModelID { get; set; }

        public int FieldID { get; set; }

        public Guid FieldKey { get; set; }

        public string FieldName { get; set; }

        public bool Enable { get; set; }

        public int SortOrder { get; set; }

        public string FieldType { get; set; }

        public string FieldTypeSetting { get; set; }

        public bool Search { get; set; }

        public bool AdvancedSearch { get; set; }


        public bool DisplayInList { get; set; }

        public bool MustFilled { get; set; }

        public string Description { get; set; }


        public ExtendedField ExtendedField
        {
            get;
            set;
        }

        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return FieldID;
        }

        #endregion
    }

    public class ThreadCateModelFieldCollection : HashedCollectionBase<int, ThreadCateModelField>
    {
        public ThreadCateModelFieldCollection()
        { }

        public ThreadCateModelFieldCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                ThreadCateModelField field = new ThreadCateModelField(readerWrap);

                this.Add(field);
            }
        }
    }
}