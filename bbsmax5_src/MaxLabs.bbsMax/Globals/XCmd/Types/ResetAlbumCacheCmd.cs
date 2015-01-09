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
using System.Collections.Generic;

using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.XCmd
{
	public class ResetAlbumCacheCmd : IXCmd
	{

		#region ICmdHandler 成员

		public string Command
		{
			get { return "ResetAlbum"; }
		}

        public void Process(DataTable data)
		{
            foreach (DataRow row in data.Rows)
            {
                int albumID = (int)row["AlbumID"];

                string[] propertyNames = new string[data.Columns.Count - 2];
                object[] propertyValues = new object[data.Columns.Count - 2];

                for (int i = 2; i < data.Columns.Count; i++)
                {
                    propertyNames[i - 2] = data.Columns[i].ColumnName;
                    propertyValues[i - 2] = row[i];
                }

                AlbumBO.Instance.UpdateCachedAlbumData(albumID, propertyNames, propertyValues);
            }
		}

		#endregion
	}
}