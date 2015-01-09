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

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax
{
    public class BOBase<T> where T : BOBase<T>, new()
    {

        private static T s_Instance = null;

        /// <summary>
        /// 获取当前业务逻辑对象的实例
        /// </summary>
        public static T Instance
        {
            get
            {
                if (s_Instance == null)
                    s_Instance = new T();
                return s_Instance;
            }
        }

        /// <summary>
        /// 获取当前用户的ID
        /// </summary>
        [Obsolete]
        protected int ExecutorID
        {
            get
            {
				return BbsContext.Current.ExecutorID;
            }
        }

        /// <summary>
        /// 获取当前是否注册登陆用户（否则为游客）
        /// </summary>
        [Obsolete]
        protected bool IsExecutorLogin 
		{ 
			get { return ExecutorID > 0; } 
		}

        /// <summary>
        /// 获取当前是否安全模式
        /// </summary>
        [Obsolete]
        protected bool SafeMode 
		{ 
			get { return BbsContext.Current.SafeMode; } 
		}

        /// <summary>
        /// 获取当前上下文中是否存在未被捕捉的错误
        /// </summary>
        protected bool HasUnCatchedError
        {
            get
            {
                if (ErrorScope.Current != null)
                    return ErrorScope.Current.HasUnCatchedError;

                if(Context.Current != null)
                    return Context.Current.Errors.HasUnCatchedError;

                return false;
            }
        }

		/// <summary>
		/// 抛出错误信息（不会中止当前线程继续执行）
		/// </summary>
		/// <typeparam name="TError">错误信息类型</typeparam>
		/// <param name="error">错误信息</param>
		public void ThrowError<TError>(TError error) where TError : ErrorInfo
		{
			WebEngine.Context.ThrowError<TError>(error);
		}

		/// <summary>
		/// 忽略调用此方法之前在当前错误区段发生的指定类型的错误。
		/// 这些错误信息不会真正从上下文的Errors集合移除，只是被标示成已捕获。
		/// 和CatchError一样，次方法支持错误信息的继承关系，即清除类型A的错误信息时，属于A的子类型错误信息也将被清除。
		/// </summary>
		/// <typeparam name="TError"></typeparam>
		public void IgnoreError<TError>() where TError : ErrorInfo
		{
			if (ErrorScope.Current != null)
				ErrorScope.Current.IgnoreError<TError>();
		}

		/// <summary>
		/// 捕获指定类型的错误信息，和C#的catch类似，所有属于指定类型的子类错误信息也将被捕获。
		/// 比如通过捕获ErrorInfo类型的错误可以捕获所有类型的错误信息。
		/// </summary>
		/// <typeparam name="TError">错误类型</typeparam>
		/// <param name="callback"></param>
		public void CatchError<TError>(MaxLabs.WebEngine.ErrorScope.CatchCallback<TError> onError) where TError : ErrorInfo
		{
			if (ErrorScope.Current != null)
				ErrorScope.Current.CatchError<TError>(onError);
		}

        /// <summary>
        /// 清空当前上下文中的所有错误
        /// </summary>
        protected void ClearErrors()
        {
            if (ErrorScope.Current != null)
                ErrorScope.Current.IgnoreError<ErrorInfo>();
		}

		/// <summary>
		/// 检查用户ID数据合法性
		/// </summary>
		/// <param name="userID"></param>
		/// <returns></returns>
		protected bool ValidateUserID(int userID)
		{
			if (userID < 0)
			{
				ThrowError(new InvalidParamError("userID"));
				return false;
			}

			return true;
		}

		/// <summary>
		/// 根据操作者和数据所有者的关系获取操作者的数据访问级别
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="dataownerID">数据所有者ID</param>
		/// <returns>操作者数据访问级别</returns>
		protected DataAccessLevel GetDataAccessLevel(int operatorID, int dataOwnerID)
		{
			if (operatorID == dataOwnerID)
				return DataAccessLevel.DataOwner;
			else if (FriendBO.Instance.IsFriend(dataOwnerID, operatorID))
				return DataAccessLevel.Friend;
			else
				return DataAccessLevel.Normal;
		}
    }
}