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

using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Entities
{
    public class ADCategory:IPrimaryKey<int>
    {

        public ADCategory()
        {

        }

        public ADCategory(DataReaderWrap readerWrap)
        {
            this.ID = readerWrap.Get<int>("CategoryID");
            this.Name = readerWrap.Get<string>("CategoryName");
        }

        public string Name
        {
            get;
            set;
        }

        public bool IsSystem
        {
            get;
            set;
        }

        public int ID
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public ADShowType ShowType
        {
            get;
            set;
        }

        /// <summary>
        /// 是否投放到论坛显示
        /// </summary>
        public bool ShowInForum
        {
            get;
            set;
        }

        private ADTargetCollection targetCommonPages=new ADTargetCollection();

        public ADTargetCollection CommonPages
        {
            get
            {
                return targetCommonPages;
            }
            set
            {
                targetCommonPages = value;
            }
        }

        private AdvertCollection m_AdvertList;
        public AdvertCollection AdvertList
        {
            get
            {
                if (m_AdvertList == null)
                {
                    m_AdvertList = AdvertBO.Instance.GetAdvertByCategory(this.ID);
                }
                return m_AdvertList;
            }
        }

        internal void ClearCache()
        {
            m_AdvertList = null;
        }

        #region IPrimaryKey Member
        public int GetKey()
        {
            return this.ID;
        }
        #endregion

        #region Static Members

        private static ADCategory _headerAd;
        private static ADCategory _footerAd;
        private static ADCategory _pageWordAd;
        private static ADCategory _inPostAd;
        private static ADCategory _floatAd;
        private static ADCategory _doubleAd;
        private static ADCategory _postLeaderboardAd;
        private static ADCategory _inForumAd;
        private static ADCategory _SignatureAd;
        private static ADCategory _topBanner;
        private static ADCategory _inListAd;
        private static ADCategoryCollection _systemADCategoryes = new ADCategoryCollection();

        public static ADCategoryCollection SystemADCategoryes
        {
            get { return ADCategory._systemADCategoryes; }
        }
               
        static ADCategory()
        {

            _headerAd             = new ADCategory();
            _headerAd.ID          = -1;
            _headerAd.Name        =  Lang.AD_Header;
            _headerAd.ShowType    = ADShowType.Random;
            _headerAd.IsSystem    = true;
            _headerAd.ShowInForum = true;
            _headerAd.Description = Lang.AD_Header_Description;
            _headerAd.CommonPages = ADTargetCommonPages.All;
            _systemADCategoryes.Add(_headerAd);

            _floatAd                = new ADCategory();
            _floatAd.ID             = -5;
            _floatAd.ShowType       = ADShowType.Random;
            _floatAd.Name           = Lang.AD_Float ;
            _floatAd.ShowInForum    = true;
            _floatAd.IsSystem       = true;
            _floatAd.Description    = Lang.AD_Float_Description;
            _floatAd.CommonPages    = ADTargetCommonPages.All;
            _systemADCategoryes.Add(_floatAd);

            _pageWordAd             = new ADCategory();
            _pageWordAd.ID          = -3;
            _pageWordAd.Name        = Lang.AD_PageWord ;
            _pageWordAd.ShowType    = ADShowType.List;
            _pageWordAd.IsSystem    = true;
            _pageWordAd.ShowInForum = true;
            _pageWordAd.Description = Lang.AD_PageWord_Description;
            //_pageWordAd.CommonPages=
            _systemADCategoryes.Add(_pageWordAd);

            _SignatureAd            = new ADCategory();
            _SignatureAd.ID         = -9;
            _SignatureAd.Name       = Lang.AD_Signature;
            _SignatureAd.Description = Lang.AD_Signature_Description;
            _SignatureAd.IsSystem   = true;
            _SignatureAd.ShowInForum = true;
            _SignatureAd.ShowType   = ADShowType.Random;
            _systemADCategoryes.Add(_SignatureAd); 

            _inPostAd               = new ADCategory();
            _inPostAd.ID            = -4;
            _inPostAd.Name          =  Lang.AD_InPost;
            _inPostAd.ShowType      = ADShowType.Random;
            _inPostAd.Description   = Lang.AD_InPost_Description;
            _inPostAd.ShowInForum   = true;
            _inPostAd.IsSystem      = true;
            _systemADCategoryes.Add(_inPostAd);

            _footerAd = new ADCategory();
            _footerAd.ID            = -2;
            _footerAd.Name          = Lang.AD_Footer ;
            _footerAd.ShowType      = ADShowType.Random;
            _footerAd.IsSystem      = true;
            _footerAd.ShowInForum   = true;
            _footerAd.Description   = Lang.AD_Footer_Description;
            _footerAd.CommonPages   = ADTargetCommonPages.All;
            _systemADCategoryes.Add(_footerAd);

            _doubleAd               = new ADCategory();
            _doubleAd.ID            = -6;
            _doubleAd.ShowType      = ADShowType.Random;
            _doubleAd.Name          = Lang.AD_Double ;
            _doubleAd.Description   = Lang.AD_Double_Description;
            _doubleAd.IsSystem      = true;
            _doubleAd.ShowInForum   = true;
            _doubleAd.CommonPages   = ADTargetCommonPages.All;
            _systemADCategoryes.Add(_doubleAd);

            _postLeaderboardAd              = new ADCategory();
            _postLeaderboardAd.ID           = -7;
            _postLeaderboardAd.Name         = Lang.AD_PostLeaderboard ;
            _postLeaderboardAd.ShowType     = ADShowType.Random;
            _postLeaderboardAd.Description  = Lang.AD_PostLeaderboard_Description;
            _postLeaderboardAd.IsSystem     = true;
            _postLeaderboardAd.ShowInForum  = true;
            _systemADCategoryes.Add(_postLeaderboardAd);

            _inForumAd              = new ADCategory();
            _inForumAd.ID           = -8;
            _inForumAd.Name         = Lang.AD_InForum ;
            _inForumAd.Description  = Lang.AD_InForum_Description;
            _inForumAd.IsSystem     = true;
            _inForumAd.ShowInForum  = true;
            _inForumAd.ShowType     = ADShowType.Random;
            _systemADCategoryes.Add(_inForumAd);

            _topBanner              = new ADCategory();
            _topBanner.ID           = -10;
            _topBanner.Name         = Lang.AD_TopBanner;
            _topBanner.Description  = Lang.AD_TopBanner_Description;
            _topBanner.IsSystem     = true;
            _topBanner.ShowInForum  = true;
            _topBanner.CommonPages  = new ADTargetCollection();
            _topBanner.ShowType     = ADShowType.Random;
            _topBanner.CommonPages.Add(ADTargetCommonPages.Index);
            _systemADCategoryes.Add(_topBanner);

            _inListAd = new ADCategory();
            _inListAd.ID = -11;
            _inListAd.Name = Lang.AD_InList;
            _inListAd.Description = Lang.AD_InList_Description;
            _inListAd.IsSystem = true;
            _inListAd.ShowInForum = true;
            _inListAd.CommonPages = new ADTargetCollection();
            _inListAd.ShowType = ADShowType.Random;
            _systemADCategoryes.Add(_inListAd);           
        }

        /// <summary>
        /// 头部横幅广告
        /// </summary>
        public static ADCategory HeaderAd
        {
            get
            {
                return _headerAd;
            }
        }

        /// <summary>
        /// 底部横幅广告
        /// </summary>
        public static ADCategory FooterAd
        {
            get
            {
                return _footerAd;
            }
        }

        /// <summary>
        /// 页内文字广告（头部）
        /// </summary>
        public static ADCategory PageWordAd
        {
            get
            {
                return _pageWordAd;
            }
        }

        /// <summary>
        /// 贴内广告
        /// </summary>
        public static ADCategory InPostAd
        {
            get
            {
                return _inPostAd;
            }
        }

        /// <summary>
        /// 左下角浮动广告
        /// </summary>
        public static ADCategory FloatAd
        {
            get
            {
                return _floatAd;
            }
        }

        /// <summary>
        /// 顶部横幅广告
        /// </summary>
        public static ADCategory TopBanner
        {
            get
            {
                return _topBanner;
            }
        }

        /// <summary>
        /// 签名广告
        /// </summary>
        public static ADCategory SignatureAd
        {
            get
            {
                return _SignatureAd;
            }
        }

        /// <summary>
        /// 左右两侧对联广告
        /// </summary>
        public static ADCategory DoubleAd
        {
            get
            {
                return _doubleAd;
            }
        }

        /// <summary>
        /// 帖间通栏广告(主题和第一回复之间： 随机)
        /// </summary>
        public static ADCategory PostLeaderboardAd
        {
            get
            {
                return _postLeaderboardAd;
            }
        }

        public static ADCategory InListAd
        {
            get
            {
                return _inListAd;
            }
        }

        /// <summary>
        /// 分类间广告
        /// </summary>
        public static ADCategory InForumAd
        {
            get
            {
                return _inForumAd;
            }
        }

        #endregion
    }

    public class ADCategoryCollection:EntityCollectionBase<int,ADCategory>
    {
        public ADCategoryCollection()
        {

        }
    }
}