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
using System.Reflection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;


using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.Entities
{
    /// <summary>
    /// 好友分组
    /// </summary>
    public class FriendGroup : IPrimaryKey<int>
    {

        public FriendGroup()
        { }

        public FriendGroup(DataReaderWrap readerWrap)
        {
            GroupID = readerWrap.Get<int>("GroupID");
            UserID = readerWrap.Get<int>("UserID");
            Name = readerWrap.Get<string>("GroupName");
            //IsSystem = readerWrap.Get<bool>("IsSystem");
            IsShield = readerWrap.Get<bool>("IsShield");
            TotalFriends = readerWrap.Get<int>("TotalFriends");
        }

        /// <summary>
        /// 好友分组ID
        /// </summary>
        public int GroupID { get; set; }

        public int UserID { get; set; }

        /// <summary>
        ///好友分组名
        /// </summary>
        public string Name { get; set; }

        ///// <summary>
        ///// 是否是系统分组
        ///// </summary>
        //public bool IsSystem { get; set; }

        /// <summary>
        /// 是否被屏蔽动态？
        /// </summary>
        public bool IsShield { get; set; }

        public int TotalFriends { get; set; }

        #region IPrimaryKey<int> Members

        public int GetKey()
        {
            return GroupID;
        }

        #endregion

        #region 在模板中使用的属性

        [Obsolete("此ID仅在模板中使用")]
        public int ID
        {
            get { return GroupID; }
        }

        public bool CanManage
        {
            get { return GroupID > 0; }
        }

        public bool IsSelect
        {
            get;
            set;
        }

        public int OnlineCount
        {
            get;
            set;
        }

        #endregion

        public static FriendGroup GetDefaultGroup(int userID, int totalFriends)
        {
            FriendGroup defaultGroup = new FriendGroup();
            defaultGroup.GroupID = 0;
            defaultGroup.IsShield = false;
            defaultGroup.Name = "未分组";
            defaultGroup.UserID = userID;
            defaultGroup.TotalFriends = totalFriends;

            return defaultGroup;
        }
    }

    public class FriendGroupCollection : EntityCollectionBase<int, FriendGroup>//, IStringConverter<FriendGroup>
    {

        public FriendGroupCollection()
        { }

        public FriendGroupCollection(FriendGroupCollection oldCollection)
            :base(oldCollection)
        {

        }

        public FriendGroupCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                this.Add(new FriendGroup(readerWrap));
            }
        }

        #region 原来根据字符串反序列化的

        //public static FriendGroupCollection Parse(string friendGroupString)
        //{
        //    FriendGroupCollection friendGroups = new FriendGroupCollection();
        //    friendGroups.ConvertFromString(friendGroupString);
        //    return friendGroups;
        //}
        

        //#region IStringConverter<FriendGroupCollection> 成员

        //public string ConvertToString()
        //{
        //    StringTable friendGroupTables = new StringTable();

        //    foreach (FriendGroup friendGroup in this)
        //    {
        //        string friendGroupString;
        //        if (friendGroup.IsShield)
        //            friendGroupString = "!";
        //        else
        //            friendGroupString = string.Empty;

        //        if (!friendGroup.IsSystem)
        //            friendGroupString += friendGroup.Name;

        //        if (!string.IsNullOrEmpty(friendGroupString))
        //            friendGroupTables.Add(friendGroup.GroupID.ToString(), friendGroupString);
        //    }

        //    return friendGroupTables.ToString();
        //}

        //public void ConvertFromString(string valueString)
        //{
        //    if (AllSettings.Current == null)
        //        return;

        //    //系统分组
        //    LineCollection systemFriendGroups = AllSettings.Current.FriendSettings.FriendGroupNames;
        //    //自定义分组
        //    StringTable friendGroupTables = StringTable.Parse(valueString);

        //    for (int i = 0; i < AllSettings.Current.FriendSettings.MaxFriendGroupCount; i++)
        //    {
        //        FriendGroup friendGroup = new FriendGroup();
        //        friendGroup.GroupID = i;
        //        friendGroup.IsSystem = true;
        //        friendGroup.Name = null;

        //        if (friendGroupTables.ContainsKey(i.ToString()))
        //        {
        //            string friendGroupString = friendGroupTables[i.ToString()];

        //            if (friendGroupString.StartsWith("!"))
        //            {
        //                friendGroup.IsShield = true;
        //                friendGroupString = friendGroupString.Remove(0, 1);
        //            }
        //            else
        //                friendGroup.IsShield = false;

        //            if (!string.IsNullOrEmpty(friendGroupString))
        //            {
        //                friendGroup.IsSystem = false;
        //                friendGroup.Name = friendGroupString;
        //            }
        //        }

        //        if (friendGroup.IsSystem && friendGroup.Name == null)
        //        {
        //            if (systemFriendGroups.Count > i)
        //                friendGroup.Name = systemFriendGroups[i];
        //            else
        //                friendGroup.Name = string.Format("自定义分组{0}", i - systemFriendGroups.Count + 1);//自定义分组
        //        }
        //        this.Add(friendGroup);
        //    }
        //}

        //#endregion

        #endregion

    }
}