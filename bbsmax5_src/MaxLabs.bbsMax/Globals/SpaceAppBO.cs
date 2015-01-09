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
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax
{
	public abstract class SpaceAppBO<T> : BOBase<T> where T : BOBase<T>,  new()
	{
		/// <summary>
		/// 获取使用的动作，用于权限检查
		/// </summary>
		protected abstract SpacePermissionSet.Action UseAction
		{
			get;
		}

		/// <summary>
		/// 获取管理的动作，用于权限检查
		/// </summary>
        protected abstract BackendPermissions.ActionWithTarget ManageAction
		{
			get;
		}

		/// <summary>
		/// 获取管理权限
		/// </summary>
		public BackendPermissions ManagePermission
		{
			get
			{
                return AllSettings.Current.BackendPermissions;
			}
		}

		/// <summary>
		/// 获取使用权限
		/// </summary>
		public SpacePermissionSet Permission
		{
			get
			{
				return AllSettings.Current.SpacePermissionSet;
			}
		}

		protected PrivacyType GetPrivacyTypeForFeed(SpacePrivacyType spacePrivacyType, SpacePrivacyType appPrivacyType, PrivacyType dataPrivacyType)
		{
			if (spacePrivacyType == SpacePrivacyType.Friend || appPrivacyType == SpacePrivacyType.Friend || dataPrivacyType == PrivacyType.FriendVisible)
			{
				return PrivacyType.FriendVisible;
			}

			if (spacePrivacyType == SpacePrivacyType.Self || appPrivacyType == SpacePrivacyType.Self || dataPrivacyType == PrivacyType.SelfVisible)
			{
				return PrivacyType.SelfVisible;
			}

			return dataPrivacyType;
		}

		/****************************************************************************************************************************
		 *                                    Validate开头的函数会抛出错误信息，Check开头的只检查不抛错                             *
		 ****************************************************************************************************************************/

		/// <summary>
		/// 验证操作者是否具有使用权限
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <returns>操作者具有使用权限则返回true否则返回false</returns>
		protected bool ValidateUsePermission<ErrorType>(int operatorID) where ErrorType : ErrorInfo, new()
		{
			if (Permission.Can(operatorID, UseAction))
				return true;

			ThrowError<ErrorType>(new ErrorType());

			return false;
		}

		/// <summary>
		/// 验证操作者的管理权限，没权限时会ThrowError（只要对任意一个用户组有管理权限都算有权限）
		/// </summary>
		/// <typeparam name="ErrorType">错误类型</typeparam>
		/// <param name="operatorID">操作者ID</param>
		/// <returns></returns>
		protected bool ValidateAdminPermission<ErrorType>(int operatorID) where ErrorType : ErrorInfo, new() //老达TODO:最后好改成NoPermissionError基类
		{
			if (ManagePermission.HasPermissionForSomeone(operatorID, ManageAction))
				return true;

			ThrowError<ErrorType>(new ErrorType());

			return false;
		}

		/// <summary>
		/// 验证操作者是否具有删除某条数据的权限
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="dataOwnerID">数据所有者ID</param>
		/// <param name="lastEditorID">数据最后编辑者ID</param>
		/// <returns>操作者具有删除权限则返回true否则返回false</returns>
		protected bool CheckDeletePermission(int operatorID, int dataOwnerID)
		{
			if (operatorID == dataOwnerID)
			{
				return true; //2009-07-09 喳喳鸟又说不判断最后编辑者了 Permission.Can(operatorID, UseAction);
			}
			else
			{
				NoPermissionType reason = NoPermissionType.NoPermission;

				return ManagePermission.Can(operatorID, ManageAction, dataOwnerID, out reason);
			}
		}

		/// <summary>
		/// 验证操作者是否具有删除某条数据的权限
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="dataOwnerID">数据所有者ID</param>
		/// <param name="lastEditorID">数据最后编辑者ID</param>
		/// <returns>操作者具有删除权限则返回true否则返回false</returns>
		protected bool CheckDeletePermission(int operatorID, int dataOwnerID, int lastEditorID)
		{
			if (operatorID == dataOwnerID)
			{
				return true; //2009-07-09 喳喳鸟又说不判断最后编辑者了 Permission.Can(operatorID, UseAction);
			}
			else
			{
				NoPermissionType reason = NoPermissionType.NoPermission;

				return ManagePermission.Can(operatorID, ManageAction, lastEditorID, out reason);
			}
		}

		/// <summary>
		/// 验证操作者是否具有编辑某条数据的权限
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="dataOwnerID">数据所有者ID</param>
		/// <param name="lastEditorID">数据最后编辑者ID</param>
		/// <returns>操作者具有编辑权限则返回true否则返回false</returns>
		protected bool CheckEditPermission(int editorID, int dataOwnerID)
		{
			if (editorID == dataOwnerID)
			{
				return true; //2009-07-09 喳喳鸟又说不判断最后编辑者了 Permission.Can(editorID, UseAction);
			}
			else
			{
				NoPermissionType reason = NoPermissionType.NoPermission;

				return ManagePermission.Can(editorID, ManageAction, dataOwnerID, out reason);
			}
		}

		/// <summary>
		/// 验证操作者是否具有编辑某条数据的权限
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="dataOwnerID">数据所有者ID</param>
		/// <param name="lastEditorID">数据最后编辑者ID</param>
		/// <returns>操作者具有编辑权限则返回true否则返回false</returns>
		protected bool CheckEditPermission(int editorID, int dataOwnerID, int lastEditorID)
		{
			if (editorID == dataOwnerID) //2009-07-09 喳喳鸟又说不判断最后编辑者了 && editorID == lastEditorID)
			{
				return true; //2009-07-09 喳喳鸟又说不判断最后编辑者了 Permission.Can(editorID, UseAction);
			}
			else
			{
				/* 2009-0714 据喳喳鸟要求在没有专门给管理员编辑内容的页面前，不要提供让管理员编辑别人数据的功能*/
                /*什么乱七八糟的，不懂。 注释的代码被我恢复了 wen*/
				NoPermissionType reason = NoPermissionType.NoPermission;
				return ManagePermission.Can(editorID, ManageAction, dataOwnerID, lastEditorID, out reason);
			}
		}

		/// <summary>
		/// 检查访问权限返回值
		/// </summary>
		protected enum CheckVisitPermissionResult
		{
			/// <summary>
			/// 允许访问
			/// </summary>
			CanVisit,

			/// <summary>
			/// 不能访问，只有数据所有者可以访问
			/// </summary>
			OnlyDataOwnerCanVisit,

			/// <summary>
			/// 不能访问，只有数据所有者好友可以访问
			/// </summary>
			OnlyDataOwnerFriendsCanVisit,

			/// <summary>
			/// 不能访问，只有数据密码持有者可以访问
			/// </summary>
			OnlyDataPasswordHolderCanVisit
		}

		/// <summary>
		/// 验证操作者是否具有浏览某条数据的权限
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="dataOwnerID">数据所有者ID</param>
		/// <param name="lastEditorID">数据最后编辑者ID</param>
		/// <returns>操作者具有浏览权限则返回true否则返回false</returns>
		protected CheckVisitPermissionResult CheckVisitPermission(int visitorID, int dataOwnerID, PrivacyType dataPrivacyType)
		{
			return CheckVisitPermission(visitorID, dataOwnerID, dataPrivacyType, 0, null, null);
		}

		/// <summary>
		/// 验证操作者是否具有浏览某条数据的权限
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="dataOwnerID">数据所有者ID</param>
		/// <param name="lastEditorID">数据最后编辑者ID</param>
		/// <param name="operatorPassword">操作者提供的密码，如果为null将尝试从操作者的PasswordBox获取</param>
		/// <param name="dataID">数据的ID，用于生成PasswordBox的Key</param>
		/// <param name="dataPassword">真实的密码，如果为null时将不会判断隐私类型为“需要密码”的情况</param>
		/// <returns>操作者具有浏览权限则返回true否则返回false</returns>
		protected CheckVisitPermissionResult CheckVisitPermission(int visitorID, int dataOwnerID, PrivacyType dataPrivacyType, int dataID, string dataPassword, string visitorPassword)
		{
			CheckVisitPermissionResult result = CheckVisitPermissionResult.CanVisit;

			if (visitorID != dataOwnerID && dataPrivacyType != PrivacyType.AllVisible)
			{
				if (dataPrivacyType == PrivacyType.SelfVisible)
				{
					result = visitorID == dataOwnerID ? CheckVisitPermissionResult.CanVisit : CheckVisitPermissionResult.OnlyDataOwnerCanVisit;
				}
				else if (dataPrivacyType == PrivacyType.FriendVisible)
				{
					result = FriendBO.Instance.IsFriend(visitorID, dataOwnerID) ? CheckVisitPermissionResult.CanVisit : CheckVisitPermissionResult.OnlyDataOwnerFriendsCanVisit;
				}
				else if (dataPrivacyType == PrivacyType.NeedPassword)
				{
					if (dataPassword != null)
					{
						string passwordBoxKey = GetPasswordBoxKey(dataID);

						AuthUser visitor = UserBO.Instance.GetAuthUser(visitorID);

                        if (visitorPassword == null)
                            visitorPassword = visitor.TempDataBox.GetData(passwordBoxKey); //visitor.PasswordBox[passwordBoxKey] as string;
						else
							visitorPassword = SecurityUtil.Encrypt(EncryptFormat.bbsMax, visitorPassword);

						if (visitorPassword != dataPassword)
						{
							result = CheckVisitPermissionResult.OnlyDataPasswordHolderCanVisit;
						}
						else
						{
                            visitor.TempDataBox.SetData(passwordBoxKey, dataPassword);
							//visitor.PasswordBox[passwordBoxKey] = dataPassword;
						}
					}
					else
					{
						result = CheckVisitPermissionResult.OnlyDataPasswordHolderCanVisit;
					}
				}
			}

			return result;
		}

		/// <summary>
		/// 获取PasswordBox的Key
		/// </summary>
		/// <param name="id">对象的ID，比如日志ID</param>
		/// <returns></returns>
		protected string GetPasswordBoxKey(int id)
		{
			return this.GetType().Name + "/" + id;
		}
	}
}