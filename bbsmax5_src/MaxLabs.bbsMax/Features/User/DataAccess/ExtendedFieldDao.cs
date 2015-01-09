//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;


using MaxLabs.bbsMax.Entities;


namespace MaxLabs.bbsMax.DataAccess
{
    public abstract class ExtendedFieldDao : DaoBase<ExtendedFieldDao>
    {

        /// <summary>
        /// 添加自定义字段
        /// </summary>
        //public abstract void SaveExtendedField(ExtendedField extendedField, Collection<ChildField> children);

        /// <summary>
        /// 获取全部自定义字段
        /// </summary>
        /// <returns></returns>
        public abstract ExtendedFieldCollection GetAllExtendedFields();

        /// <summary>
        /// 保存用户的扩展字段
        /// </summary>
        public abstract void SaveUserExtendedFields(int userID, StringTable fieldValues);

        /// <summary>
        /// 批量删除扩展字段
        /// </summary>
        public abstract void DeleteExtendedFields(IEnumerable<int> fieldIDs);
    }
}