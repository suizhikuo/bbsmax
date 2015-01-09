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

namespace MaxLabs.bbsMax.Rescourses
{
	public partial class Lang_Error
	{
		//public const string Permission_NoPermissionUseBlogError = "您没有权限使用日志";

		//public const string Permission_NoPermissionManageBlogError = "您没有权限管理日志";

		//public const string Permission_NoPermissionEditBlogError = "您没有权限编辑该篇日志";

		//public const string Permission_NoPermissionEditBlogCategoryError = "您没有权限编辑该日志分类";

		public const string Permission_NoPermissionEditAdminEditedBlogError = "该篇日志已经被管理员编辑过,您不能再编辑";

		//public const string Permission_NoPermissionDeleteBlogError = "您没有权限删除日志";

		public const string Permission_NoPermissionDeleteEditedBlogError = "该篇日志已经被管理员编辑过,您没有权限删除";

		public const string Permission_NoPermissionDeleteBlogCategoryError = "您没有权限删除日志分类";

		public const string Permission_NoPermissionVisitArticleError = "没有权限访问当前日志";

		public const string Permission_NoPermissionVisitArticleBeacuseNeedPasswordError = "需要密码才能访问当前日志";

		public const string Permission_NoPermissionVisitArticleBeacuseNotFriendError = "需要是作者好友才能访问当前日志";
	}
}