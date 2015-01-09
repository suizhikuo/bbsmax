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

using MaxLabs.bbsMax.Providers;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
	public class DataProvider : IDataProvider
	{
		#region IDataAccessProvider 成员

		public T Create<T>() where T : DaoBase<T>
		{
			if (typeof(T) == typeof(DataAccess.DatabaseInfoDao))
				return new DatabaseInfoDao() as T;

			if (typeof(T) == typeof(Settings.SettingDao))
				return new SettingDao() as T;

            //if (typeof(T) == typeof(Permissions.PermissionDao))
            //    return new PermissionDao() as T;

            //if (typeof(T) == typeof(DataAccess.UserGroupDao))
            //    return new UserGroupDao() as T;

			if (typeof(T) == typeof(DataAccess.FriendDao))
				return new FriendDao() as T;

			if (typeof(T) == typeof(DataAccess.UserDao))
				return new UserDao() as T;

            if( typeof(T) == typeof(DataAccess.UserTempDataDao) )
                return new UserTempDataDao() as T;

            if (typeof(T) == typeof(DataAccess.UserTempAvatarDao))
                return new UserTempAvatarDao() as T;

			//if (typeof(T) == typeof(DataAccess.ExtendedFieldDao))
			//    return new ExtendedFieldDao() as T;

			if (typeof(T) == typeof(FileSystem.FileDao))
				return new FileDao() as T;

            //if (typeof(T) == typeof(DataAccess.MessageDao))
            //    return new MessageDao() as T;

            if (typeof(T) == typeof(DataAccess.ChatDao))
                return new ChatDao() as T;

			if (typeof(T) == typeof(DataAccess.NotifyDao))
				return new NotifyDao() as T;

			if (typeof(T) == typeof(DataAccess.InviteDao))
				return new InviteDao() as T;

            if (typeof(T) == typeof(DataAccess.AdvertDao))
                return new AdvertDao() as T;

            if (typeof(T) == typeof(DataAccess.VarDao))
                return new VarDao() as T;

            if (typeof(T) == typeof(ValidateCodes.ValidateCodeDao))
                return new ValidateCodeDao() as T;

            if (typeof(T) == typeof(Jobs.JobDao))
                return new JobDao() as T;

#if !Passport
            if (typeof(T) == typeof(DataAccess.BannedUserDao))
                return new BannedUserDao() as T;
			if (typeof(T) == typeof(DataAccess.FeedDao))
				return new FeedDao() as T;

            //if (typeof(T) == typeof(DataAccess.BlacklistDao))
            //    return new BlacklistDao() as T;

			if (typeof(T) == typeof(DataAccess.DoingDao))
				return new DoingDao() as T;

			if (typeof(T) == typeof(DataAccess.CommentDao))
				return new CommentDao() as T;

			if (typeof(T) == typeof(DataAccess.ShareDao))
				return new ShareDao() as T;

			if (typeof(T) == typeof(DataAccess.BlogDao))
				return new BlogDao() as T;

            if (typeof(T) == typeof(DataAccess.SpaceDao))
                return new SpaceDao() as T;

            if (typeof(T) == typeof(DataAccess.AlbumDao))
                return new AlbumDao() as T;

            if (typeof(T) == typeof(DataAccess.MissionDao))
                return new MissionDao() as T;

			if (typeof(T) == typeof(DataAccess.DenouncingDao))
				return new DenouncingDao() as T;

            if (typeof(T) == typeof(DataAccess.PointShowDao))
                return new PointShowDao() as T;


            if (typeof(T) == typeof(DataAccess.TagDao))
                return new TagDao() as T;

            //if (typeof(T) == typeof(DataAccess.ForumDao))
            //    return new ForumDao() as T;

            if (typeof(T) == typeof(DataAccess.ForumDaoV5))
                return new ForumDaoV5() as T;

            if (typeof(T) == typeof(DataAccess.PostDaoV5))
                return new PostDaoV5() as T;

            //if (typeof(T) == typeof(DataAccess.PostDao))
            //    return new PostDao() as T;

            if (typeof(T) == typeof(DataAccess.AnnouncementDao))
                return new AnnouncementDao() as T;

            if (typeof(T) == typeof(DataAccess. EmoticonDao))
                return new EmoticonDao() as T;

            if (typeof(T) == typeof(DataAccess.DiskDao))
                return new DiskDao() as T;

			if (typeof(T) == typeof(DataAccess.ClubDao))
				return new ClubDao() as T;

            if (typeof(T) == typeof(DataAccess.ImpressionDao))
                return new ImpressionDao() as T;

            if (typeof(T) == typeof(DataAccess.PropDao))
                return new PropDao() as T;

            if (typeof(T) == typeof(DataAccess.ThreadCateDao))
                return new ThreadCateDao() as T;

            if (typeof(T) == typeof(DataAccess.ClickLogDao))
                return new ClickLogDao() as T;

            if (typeof(T) == typeof(DataAccess.RoleDao))
                return new RoleDao() as T;
#endif

            if (typeof(T) == typeof(StepByStepTasks.StepByStepTaskDao))
                return new StepByStepTaskDao() as T;

            if (typeof(T) == typeof(Logs.OperationLogDao))
                return new OperationLogDao() as T;

            if (typeof(T) == typeof(DataAccess.SerialDao))
                return new SerialDao() as T;

            
           if (typeof(T) == typeof(DataAccess.PayDao))
                return new PayDao() as T;

            if (typeof(T) == typeof(DataAccess.PassportDao))
                return new PassportDao() as T;

            if (typeof(T) == typeof(DataAccess.PointLogDao))
                return new PointLogDao() as T;

            throw new Exception(string.Format("数据访问类\"{0}\"未在DataProvider中注册", typeof(T)));
		}

		#endregion

		#region IDataProvider 成员


        public System.Data.IDbTransaction BeginTransaction(IsolationLevel isolationLevel)
		{
			return new SqlSession().BeginTransaction(isolationLevel);
		}

		#endregion
	}
}