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
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.DataAccess
{
    public abstract class AdvertDao : DaoBase<AdvertDao>
    {
        public abstract Advert SaveAdvert(
                    int id,int index, int categoryID, ADPosition position, ADType adType, bool available, string title
                  , string href, string text, int fontsize, string color, string src
                  , int width, int height, DateTime beginDate, DateTime endDate, string code, string targets,string floor);

        public abstract AdvertCollection GetAdverts(int categoryID, ADPosition adPosition, int pageSize, int pageNumber, out int totalCount);

        public abstract AdvertCollection GetAdverts(int categoryID, IEnumerable<int> excludeAdverts);

        public abstract AdvertCollection GetAdverts( IEnumerable<int> adids);

        public abstract AdvertCollection GetAdverts();
        
        public abstract Advert GetAdvert(int id);

        public abstract void DeleteAdverts(IEnumerable<int> adIDs);

        public abstract AdvertCollection GetAdvertByCategory(int categoryID, bool avalible);

        public abstract AdvertCollection GetAdvertByCategory(int categoryID);

        public abstract void SetAdvertAvailable(IEnumerable<int> adids, bool available);
    }
}