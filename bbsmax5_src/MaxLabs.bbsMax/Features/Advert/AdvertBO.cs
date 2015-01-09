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
using System.Data;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Settings;
using System.Diagnostics;

namespace MaxLabs.bbsMax
{
    public class AdvertBO : BOBase<AdvertBO>
    {

        /// <summary>
        /// 根据类型编号、目标，返回 符合条件的广告列表
        /// </summary>
        /// <param name="categoryID"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private AdvertCollection SearchAdverts(int categoryID, string target, ADPosition position, int? Floor,bool? isLastFloor)
        {
            if (AllSettings.Current.AdvertSettings.EnableAdverts == false)
                return new AdvertCollection();

            if(!ADCategory.SystemADCategoryes.ContainsKey(categoryID))
                return new AdvertCollection();

            AdvertCollection ads = ADCategory.SystemADCategoryes.GetValue(categoryID).AdvertList.Limited;

            AdvertCollection adResults ;

            string cacheKey = string.Concat(categoryID, ",", target, position, Floor, isLastFloor);

            if (PageCacheUtil.TryGetValue<AdvertCollection>(cacheKey, out adResults)==false)
            {
                adResults = new AdvertCollection();
                foreach (Advert ad in ads)
                {
                    if (ad.Targets.Contains(",all,") || ad.Targets.IndexOf( string.Concat(",", target,","), StringComparison.OrdinalIgnoreCase) > -1) //投放目标选择
                    {
                        if (position == ADPosition.None || ad.Position == position)  //投放位置选中（左边还是右边还是上边）
                        {
                            if (Floor == null    //忽略
                                || ad.Floor.IndexOf(",-1,") > -1 //全部楼层
                                || ad.Floor.IndexOf("," + Floor + ",") > -1 //匹配特定楼层
                                || (ad.Floor.IndexOf(",-2,") > -1 && (isLastFloor != null && isLastFloor.Value)) //最后一楼
                                ) 
                            {
                                adResults.Add(ad);
                            }
                        }
                    }
                }
                PageCacheUtil.Set(cacheKey, adResults);
            }

            return adResults;
        }

        #region 贴内广告特殊处理
        /// <summary>
        /// 获取贴内广告
        /// </summary>
        /// <param name="target">目标</param>
        /// <param name="postion">位置</param>
        /// <param name="postIndex">楼层</param>
        /// <returns></returns>
        public string GetInPostAD( string target,ADPosition postion ,int Floor,bool isLastFloor)
        {

            //int showBginId;
            //int showTime;
            //string key = "pad_" + target + postion;
            //string keyshowTimes = "pad_st_" + target + postion;

            AdvertCollection ads = SearchAdverts(ADCategory.InPostAd.ID, target, postion, Floor,isLastFloor);
            Random rnd = new Random();
            if (ads.Count > 0)
            {
                //if ( !PageCacheUtil.TryGetValue<int>(key, out showBginId))
                //{
                //    showBginId = rnd.Next(0, 100000) % ads.Count;
                //    PageCacheUtil.Set(key, showBginId);
                //}

                //if (!PageCacheUtil.TryGetValue<int>(keyshowTimes, out showTime))
                //{
                //    showTime = 0;
                //}
                ads.Sort();
                Advert ad = ads[0];
                ad.showTimes++;
                return ad.Code;
            }
            return "";
        }

        /// <summary>
        /// 是否有贴内广告
        /// </summary>
        /// <param name="target"></param>
        /// <param name="postion"></param>
        /// <param name="floor"></param>
        /// <returns></returns>
        public bool HasInPostAD(string target, ADPosition postion, int floor,bool isLastFloor)
        {
            return  SearchAdverts(ADCategory.InPostAd.ID, target, postion, floor,isLastFloor).Count>0;
        }

        #endregion


        /// <summary>
        /// 生成列表形式的广告列表
        /// </summary>
        /// <param name="adList"></param>
        /// <returns></returns>
        private string BuildAdvertList(AdvertCollection adList)
        {
            StringBuilder builder = new StringBuilder("");

            foreach (Advert ad in adList)
            {
                builder.AppendFormat(@"<div style=""padding:5px"">{0}</div>", ad.Code);
            }

            return builder.ToString();
        }

        public string[] GetAdList( int categoryID,string target,int count)
        {
            AdvertCollection ads =SearchAdverts(categoryID, target, ADPosition.None,null,null);
            if ( ads.Count == 0)
                return new string[0];

            string[] adArray = new string[ads.Count >count ? count :ads.Count];
            for (int i = 0; i < adArray.Length; i++)
            {
                adArray[i] = ads[i].Code;
            }
            return adArray;
        }

        /// <summary>
        /// 调用广告
        /// </summary>
        /// <param name="categoryID"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public string ShowAdvert(int categoryID, string target, ADPosition position)
        {
            AdvertCollection ads = SearchAdverts(categoryID, target, position,null,null);

            Random rnd = new Random();
            if (ads.Count > 0)
            {
                ADCategory category = GetCategory(categoryID);
                switch (category.ShowType)
                {
                    case ADShowType.Random:
                        //int i = 0 ;
                        //if (ads.Length > 1)
                        //{
                        //    i = new Random().Next(ads.Length);
                        //}
                    case ADShowType.Order:
                        return ads[rnd.Next(0, 100000) % ads.Count].Code;
                    case ADShowType.List:
                        return BuildAdvertList(ads);
                }
            }
            return "";
        }


        /// <summary>
        /// 是否存在某类广告
        /// </summary>
        /// <param name="categoryID"></param>
        /// <param name="target"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool HasAdvert(int categoryID, string target, ADPosition position)
        {
           return  SearchAdverts(categoryID, target, position, null, null).Count > 0;
        }


        /// <summary>
        /// 修改或者添加广告
        /// </summary>
        /// <param name="adid"></param>
        /// <param name="categoryID"></param>
        /// <param name="position"></param>
        /// <param name="adType"></param>
        /// <param name="available"></param>
        /// <param name="title"></param>
        /// <param name="href"></param>
        /// <param name="text"></param>
        /// <param name="fontsize"></param>
        /// <param name="color"></param>
        /// <param name="src"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="code"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public Advert SaveAdvert(
              int operatorUserID
            , int adid
            , int index
            , int categoryID
            , ADPosition position
            , ADType adType
            , bool available
            , string title
            , string href
            , string text
            , int fontsize
            , string color
            , string src
            , int width
            , int height
            , DateTime beginDate
            , DateTime endDate
            , string code
            , string targets
            , string floor 
            )
        {
            if (!CanManageAdvert(operatorUserID))
            {
                ThrowError(new NoPermissionManageAdvertError());
                return null;
            }

            if (string.IsNullOrEmpty(title))
                title = GetTitle();

            switch (adType)
            {
                case ADType.Code:

                    if (string.IsNullOrEmpty(code))
                        ThrowError(new EmptyAdvertCodeError("code"));
                    break;

                case ADType.Image:

                    if (string.IsNullOrEmpty(src))
                        ThrowError(new EmptyAdvertResourceError("src"));

                    if (string.IsNullOrEmpty(href))
                        ThrowError(new EmptyAdvertHrefError("href"));
                    break;

                case ADType.Flash:

                    if (string.IsNullOrEmpty(src))
                        ThrowError(new EmptyAdvertResourceError("src"));

                    if (width == 0)
                        ThrowError(new EmptyAdvertWidthError("width"));
                    
                    if (height == 0)
                        ThrowError(new EmptyAdvertHeightError("height"));
                    break;

                case ADType.Text:

                    if (string.IsNullOrEmpty(text))
                        ThrowError(new EmptyAdvertTextError("text"));

                    if (string.IsNullOrEmpty(href))
                        ThrowError(new EmptyAdvertHrefError("href"));
                    break;
                    
            }


            if (string.IsNullOrEmpty(targets))
                targets = ",all,";
            else if (!string.IsNullOrEmpty(targets.Trim(',')))
                targets = string.Format(",{0},", targets.Trim(',')); //前后逗号补充

            if (!string.IsNullOrEmpty(floor) && !string.IsNullOrEmpty(floor.Trim(',')))
                floor = string.Format(",{0},", floor.Trim(','));

            if ( categoryID ==ADCategory.InPostAd.ID &&  string.IsNullOrEmpty(floor))
            {
                ThrowError(new CustomError("floor","请选择广告显示的楼层"));
            }

            if (HasUnCatchedError) return null;

            Advert ad = AdvertDao.Instance.SaveAdvert(
              adid
            , index
            , categoryID
            , position
            , adType
            , available
            , title
            , href
            , text
            , fontsize
            , color
            , src
            , width
            , height
            , beginDate
            , endDate
            , code
            , targets
            , floor
            );

            if (ADCategory.SystemADCategoryes.ContainsKey(categoryID))
                ADCategory.SystemADCategoryes.GetValue(categoryID).ClearCache();

            return ad;
        }


        public AdvertCollection GetAdverts()
        {
            return AdvertDao.Instance.GetAdverts();
        }

        public AdvertCollection GetAdverts( int categoryID, ADPosition position, int pageSize,int pegeNumber,out int totalCount)
        {
            if (pageSize <= 0) pageSize = Consts.DefaultPageSize;
            if (pegeNumber <= 0) pegeNumber = 1;

            return AdvertDao.Instance.GetAdverts(categoryID, position,pageSize, pegeNumber, out totalCount);
        }

        public AdvertCollection GetAdverts(int category)
        {
            return  AdvertDao.Instance.GetAdvertByCategory(category, true);
        }

        public AdvertCollection GetAdverts(IEnumerable<int> adids)
        {
            return AdvertDao.Instance.GetAdverts(adids);
        }

        public ADCategoryCollection GetAllAdvertCategory()
        {
            ADCategoryCollection advertClass = new ADCategoryCollection();
            advertClass.AddRange(ADCategory.SystemADCategoryes);
            return advertClass;
        }

        /// <summary>
        /// 只会返回有效的
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public AdvertCollection GetAdvertByCategory(int categoryID)
        {
            return AdvertDao.Instance.GetAdvertByCategory(categoryID, true);
        }

        public AdvertCollection GetAdvertByCategory(int categoryID, int pageSize, int pageNumber, out int totalCount)
        {
            totalCount = 0;
            if (pageNumber <= 0)
                pageNumber = 1;

            return AdvertDao.Instance.GetAdverts(categoryID, ADPosition.None, pageSize,  pageNumber, out totalCount);
        }

        public void SetAdvertAvailabel(int operatorUserID, IEnumerable<int> adIds,bool available)
        {
            if (CanManageAdvert(operatorUserID))
            {

                if (!ValidateUtil.HasItems<int>(adIds))
                {
                    return;
                }

                AdvertDao.Instance.SetAdvertAvailable(adIds, available);
                ClearAllAdvertCache();
              
            }
            else
            {
                ThrowError(new NoPermissionManageAdvertError());
            }
        }

        public Advert GetAdvert(int id)
        {
            return AdvertDao.Instance.GetAdvert(id);
        }

        public bool CanManageAdvert(int userID)
        {
            return  AllSettings.Current.BackendPermissions.Can(userID, BackendPermissions.Action.Setting_A);
        }

        public ADCategory GetCategory(int categoryID)
        {
            if (categoryID < 0)
            {
                foreach (ADCategory cat in ADCategory.SystemADCategoryes)
                {
                    if (categoryID == cat.ID)
                    {
                        return cat;
                    }
                }
            }
            else if (categoryID == 0)
                return null;

            return null;
            //return AdvertDao.Instance.GetAdvertCategory(categoryID);
        }

        //清除系统中的广告
        private void ClearAllAdvertCache()
        {
            foreach (ADCategory ac in ADCategory.SystemADCategoryes)
            {
                ac.ClearCache();
            }
        }

        /// <summary>
        /// 后台批量删除记录
        /// </summary>
        /// <param name="doingIDs"></param>
        public bool RemoveAdvertisements(IEnumerable<int> adIDs)
        {
            AdvertDao.Instance.DeleteAdverts(adIDs);
            //缓存
            ClearAllAdvertCache();
            return true;
        }

        private string GetTitle()
        {
            return string.Format("AD{0}", DateTimeUtil.Now);
        }
    }
}