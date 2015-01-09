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
using System.Data;
using System.Collections.ObjectModel;

namespace MaxLabs.bbsMax.Entities
{
    /// <summary>
    /// 积分公式字段
    /// </summary>
    public class PointExpressionColum
    {
        public PointExpressionColum() { }

        /// <summary>
        /// 与bbsmax_Users表对应的字段名
        /// </summary>
        public string Colum { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 字段简写
        /// </summary>
        public string FriendlyShow { get; set; }


    }

    #region 
    /// <summary>
    /// 用户组对象集合
    /// </summary>
    public class PointExpressionColumCollection : Collection<PointExpressionColum>
    {
        public void Add(string colum, string friendlyShow, string description)
        {
            PointExpressionColum temp = new PointExpressionColum();
            temp.Colum = colum;
            temp.Description = description;
            temp.FriendlyShow = friendlyShow;
            this.Add(temp);
        }
    }
    #endregion
}