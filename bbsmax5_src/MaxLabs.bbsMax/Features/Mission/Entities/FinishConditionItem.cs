//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using MaxLabs.bbsMax.Enums;

using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Settings;
using System.Text.RegularExpressions;

namespace MaxLabs.bbsMax.Entities 
{
    /// <summary>
    /// 完成任务条件
    /// </summary>
    public class FinishConditionItem
    {

        public FinishConditionItem()
        { }

        /// <summary>
        /// 用于保存值时的标志 同一级条件的Key不能一样
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 名称（如果是Radio,CheckBox，就是他们的Text）
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 输入框类型
        /// </summary>
        public InputType InputType { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 保存的值或者默认值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 项的值，（如Radio,CheckBox的value）
        /// </summary>
        public string ItemValue { get; set; }

        /// <summary>
        /// 值的约束
        /// </summary>
        public Regex ValueConstraint { get; set; }

        /// <summary>
        /// 子项（如果是RadioList，CheckboxList就有子项）
        /// </summary>
        public List<FinishConditionItem> ChildItems { get; set; }
    }
    /// <summary>
    /// 任务对象集合
    /// </summary>
    public class FinishConditionItemCollection : Collection<FinishConditionItem>
    {

        public FinishConditionItemCollection()
        {
        }



        //public ApplyConditionCollection(IDataReader reader)
        //{
            
        //    DataReaderWrap readerWrap = new DataReaderWrap(reader);

        //    while (readerWrap.Next)
        //    {
        //        this.Add(new ApplyCondition(readerWrap));
        //    }
        //}
    }



}