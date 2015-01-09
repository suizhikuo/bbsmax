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
        //public const string Blog_CategoryNotExists = "";

        public const string Template_TagNotCloseException = "模板文件\"{1}\"中模版标签\"{0}\"未闭合，请检查模版。";

        public const string Template_VariableNotExistsException = "模板文件\"{1}\"中模版变量\"{0}\"不存在，请检查模版。";
         
        public const string AD_CodeEmptyError = "广告代码不能为空";

        public const string AD_EmptyAdvertHeightError = "广告高度不能为空";

        public const string AD_EmptyAdvertHrefError = "广告地址不能为空";

        public const string AD_EmptyAdvertResourceError = "图片或者Flash地址不能为空";

        public const string AD_EmptyAdvertTextError = "广告文字不能为空";

        public const string AD_EmptyAdvertWidthError = "广告宽度不能为空";

        public const string Announcement_EmptyAnnouncementSubjectError = "公告标题不能为空";

        public const string Announcement_EmptyAnnouncementContentError = "公告内容不能为空";

        public const string Chinese = @"中文";

        public const string Comment_EmptyCommentError = @"至少应该写一点东西";

        public const string Comment_InvalidCommentLengthError = @"评论长度不能大于{1}";

        public const string Comment_KeywordBannedError = @"内容中含有被禁止的关键字,请修改后再发表";

        public const string DataNoSaveError = "注意：由于表单中的某些项不符合要求， 因此数据没有保存成功, 请检查！";

        public const string English = @"英文";

        public const string Emoticon_NoPermissionManageEmoticon = "您没有权限管理系统默认表情";

        public const string Emoticon_EmptyEmoticonGroupNameError = "表情分组名称不能为空";

        public const string Feed_EmptySiteFeedContentError = @"动态内容不能为空";

        public const string Feed_EmptySiteFeedTitleError = @"动态标题不能为空";

        public const string Feed_FeedDescriptionLengthError = @"动态内容模板长度不能超过{1}个字节（一个汉字等于2个字节）";

        public const string Feed_FeedIconUrlLength = @"动态图标URL长度不能超过{1}个字节（一个汉字等于2个字节）";

        public const string Feed_FeedNotExistsError = @"不存在feedID为{0}的动态";

        public const string Feed_FeedTitleLengthError = @"动态标题模板长度不能超过{1}个字节（一个汉字等于2个字节）";

        public const string Feed_NotSiteFeedError = @"feedID为{0}的动态不是全局动态";

        public const string Feed_ShieldFeedNotFriendError = @"您和{0}不是好友，不能屏蔽他(她)的动态";

        public const string Feed_SiteFeedCreateDateFormatError = @"发布日期格式不正确";

        public const string Feed_FeedJobExecuteTimeFormatError = @"自动清理时间必须为整数";

        public const string Feed_FeedJobInvalidExecuteTimeError = @"自动清理时间必须为0到24之间的一个整数";

        public const string Feed_FeedJobDayFormatError = @"天数必须为整数，且不能为负数";

        public const string Forum_ForumNotExistsError = "指定的论坛版块不存在";


        public const string Friend_AlreadyFriendError = @"该用户已经在您的好友名单中";

        public const string Friend_BlacklistSelfError = @"不能对自己进行操作";

        public const string Friend_EmptyFriendGroupNameError = @"好友分组不能为空";

        public const string Friend_FriendGroupLengthError = @"好友分组名称不能多于 {1} 个字";

        public const string Friend_DuplicateFriendGroupNameError = @"好友分组名称不能重复";

        public const string Friend_NotExistsFriendGroupError = @"好友分组不存在";

        public const string Friend_FriendGroupNumberError = @"您最多只能建立 {0} 个好友分组";

        public const string Friend_FriendNumberError = @"您的好友已达到上限{0}，不能添加新的好友";

        public const string Friend_InVerifyError = @"正在等待对方验证";

        //public const string Friend_IsBlacklistError = @"由于对方设置了隐私设置，您不能添加他为好友";

        public const string Friend_InBlacklistError = @"由于您在对方的黑名单中，您不能添加对方为好友";

        public const string Friend_InMyBlacklistError = @"由于对方在您的黑名单中，您不能添加对方为好友";

        public const string Friend_NotFriendError = @"指定的用户不是您的好友，请确认";

        public const string Global_EmailDisabled = @"系统已经关闭邮件发送功能";

        public const string Global_EmailFormatError = @"电子邮件地址格式错误";

        public const string Global_InvalidParamError = @"“{0}”参数错误";

        public const string Global_BeyondMin = "低于";

        public const string Global_BeyondMax = "超出";

        public const string Invite_SerialBuyOverflow = @"邀请码购买超出限制， 当前系统设置每{0}只能购买{1}个邀请码";

        public const string Link_EmptyUrlError = "友情链接的地址不能为空";

        public const string Link_EmptyNameError = "友情链接的名称不能为空";

        public const string Mission_AbandonMissionError = @"只有未申请过的任务或者未完成的任务能被放弃";

        public const string Mission_ApplyMissionNeedFinishOtherMissionError = @"您必须先完成以下任务：{0}";

        public const string Mission_ApplyMissionNeedOnlineTimeError = @"您的在线时间不够,不能申请该任务,申请该任务需要的在线时间：{1}小时 您当前的在线时间：{0}小时";

        public const string Mission_ApplyMissionNeedTotalTopicsError = @"您的发帖数不够,不能申请该任务,申请该任务需要的发帖数：{1} 您当前的发帖数：{0}";

        public const string Mission_ApplyMissionNeedUserGroupError = @"申请该任务需要以下用户组中的一个：{0}";

        public const string Mission_ApplyMissionNoEnoughDeductPointtError = @"申请该任务需要扣除{1}：{2}，您的当前{1}：{0}，不够扣除，您不能申请该任务";

        public const string Mission_ApplyMissionNoEnoughPointError = @"您的{1}不够,不能申请该任务。需要{1}：{2}，您的当前{1}：{0}";

        public const string Mission_ApplyMissionNoEnoughTotalPointError = @"您的总积分不够,不能申请该任务。需要总积分：{1}，您的当前总积分：{0}";

        public const string Mission_EmptyMissionNameError = @"请填写任务名称";

        public const string Mission_ExceededMaxApplyError = @"已经达到任务的最大申请人次限制,您不能申请";

        public const string Mission_InvalidInviteSerialCountError = @"邀请码个数必须大于0";

        public const string Mission_MedalTimeFormatError = @"用户组有效时间必须为整数";

        public const string Mission_MissionBaseNotExistsError = @"不存在type为""{0}""的任务类型";

        public const string Mission_MissionBeginDateFormatError = @"上线时间格式不正确";

        public const string Mission_MissionEndDateFormatError = @"下线时间格式不正确";

        public const string Mission_MissionHadAppliedError = @"您已经申请过该任务，不能重复申请";

        public const string Mission_MissionIconUrlLengthError = @"任务图标长度不能超过{1}个字符（1个汉字相当于2个字符）";

        public const string Mission_MissionMaxApplyCountFormatError = @"任务的最大申请人数必须为整数";

        public const string Mission_MissionNameLengthError = @"任务名称长度不能超过{1}个字符（1个汉字相当于2个字符）";

        public const string Mission_MissionNotExistsError = @"不存在ID为""{0}""的任务";

        public const string Mission_MissionNotFinishError = @"任务还未完成不能领取奖励";

        public const string Mission_MissionOnlineTimeFormatError = @"在线小时数必须为整数";

        public const string Mission_MissionSortOrderFormatError = @"排序数字必须为整数";

        public const string Mission_MissionTotalPostsFormatError = @"发帖总数必须为整数";

        public const string Mission_NeedPrizeMedalError = @"请至少选择一个奖励勋章";

        public const string Mission_NeedPrizePointError = @"请至少奖励一个类型的积分";

        public const string Mission_NeedPrizeUserGroupError = @"请至少选择一个奖励用户组";

        /*
        public const string Mission_NoPermissionManageMission = @"您没有权限管理任务";
        */

        public const string Mission_NotHaveMissionBaseError = @"没有可用的任务类型，不能创建任务";

        public const string Mission_UserGroupTimeFormatError = @"勋章有效时间必须为整数";

        public const string Online_EmptyLogoUrlError = "用户组在线图标不能为空";

        public const string Online_EmpryRoleNameError = "在线用户组显示名称不能为空";

        public const string PointShow_InvalidPointError = @"填写的积分必须大于0，并且小于您的积分数";

        public const string Report_EmptyReportError = @"举报内容不能为空";

        public const string Report_InvalidReportLengthError = @"举报内容长度不能大于{1}";

        public const string Role_DuplicatePointError = "用户组所需升级点数重复， 请检查第{0}行和第{1}行";

        public const string Role_EmptyRoleNameError = "用户组名称不能为空！";

        public const string Role_EmptyTitleError = "用户组头衔不能为空！";

        public const string Setting_EmptySettingItem = @"设置项不能为空";

        public const string Share_InvalidShareContentError = @"非法的分享内容,请不要尝试篡改分享内容";

        /*

        public const string Share_NoPermissionCreateShare = @"您没有权限添加分享";

        public const string Share_NoPermissionDeleteABatchShares = @"您没有权限批量删除分享或者可能有一条或多条数据您没有权限删除";

        public const string Share_NoPermissionDeleteShare = @"您没有权限删除该条分享";

        public const string Share_NoPermissionDeleteShareSearchResult = @"您没有权限删除分享的所有搜索结果";

        public const string Share_NoPermissionSearchFeeds = @"您没有权限搜索动态";

        public const string Share_NoPermissionSearchShares = @"您没有权限搜索分享";

        */

        public const string Share_ShareDescriptionBannedKeywordError = @"描述中含有禁止关键字";

        public const string Share_ShareDescriptionLengthError = @"分享描述不能超过{1}个字符（1个汉字相当于2个字符）";

        public const string Space_PrivacyError = @"由于对方的隐私设置，你不能访问当前内容";

        public const string User_BanUserNoForumIDError = "请指定要屏蔽的版块ID";

        public const string User_InviteSerialDisableError = @"系统已关闭了邀请码功能";

        public const string User_NotActiveError = @"您的账号需要通过Email激活后才能使用。<br /> <a href=""{1}"">点击重新发送激活邮件到 {0}</a>";

        public const string User_EmailNotValidatedError = @"您的Email还未验证，<br /><a href=""{1}"">点击此处重新发送验证邮件到{0}</a>";

        public const string User_EmailLoginRepeat = @"系统发现您的Email同时也被其他帐号所使用，所以需要确认您确实是该邮箱的所有者。<br /><a href=""{1}"">点击此处验证并绑定邮箱 {0}</a>";

        public const string User_PointFormatError2 = @"""{0}""格式错误，请填写正整数或负整数";

        public const string User_RealnameFormatError = @"真实姓名格式错误，只能是{0}，且不能为空。";

        public const string User_UsernameIsExists = @"用户名：{0} 已经存在";

        public const string User_UserPointCanNotDisablePointsError = @"不能禁用以下积分：{0}；因为总积分公式中使用了这些积分，如要禁用，请先修改总积分公式。";

        public const string User_UserPointCannotExchangeError = @"不能从“{0}”兑换到“{1}”，请选择两个不同的积分类型";

        public const string User_UserPointCannotTransferError = @"系统不允许将“{0}”转给别人";

        public const string User_UserPointCannotTransferToSelfError = @"您不能把积分转给自己";

        public const string User_UserPointEmptyExchangePointTypeError = @"请选择要兑换的积分类型";

        public const string User_UserPointEmptyExchangeTargetPointTypeError = @"请选择要兑换的目标积分类型";

        public const string User_UserPointEmptyGeneralPointNameError = @"总积分名称不能为空";

        public const string User_UserPointEmptyPointsExpressionError = @"总积分公式不能为空";

        public const string User_UserPointEmptyTransferPointTypeError = @"请选择要转帐的积分类型";

        public const string User_UserPointExchangeFormatError = @"{0}兑换比例必须为大于0的整数";

        public const string User_UserPointExchangeMinRemainingError = @"您的余额不足，不能兑换。系统允许兑换后的最小{0}必须为：{1}，您兑换后的{0}为{2}";

        public const string User_UserPointExchangeRemainingValueFormatError = @"兑换后剩余最低余额必须为整数";

        public const string User_UserPointExchangeRuleNotExistsError = @"系统不允许从“{0}”兑换到“{1}”";

        public const string User_UserPointExchangeTaxRateFormatError = @"兑换税率的值必须为不小于0的整数";

        public const string User_UserPointExchangeUnenabledPointError = @"不能从“{0}”兑换到“{1}”，因为“{2}”未启用";

        public const string User_UserPointExechangePointValueError = @"要兑换的积分数量必须为大于0的整数";

        public const string User_UserPointExpressionColumsError = @"积分公式不能使用以下字段{0}";

        public const string User_UserPointExpressionDivisorError = @"不能用字段“{0}”做除数";

        public const string User_UserPointIconEmptyError = @"等级图标不能为空";

        public const string User_UserPointIconIsExistsError = @"等级标标不能重复";

        public const string User_UserPointIconValueError = @"一个初级图标所需{0}必须为大于0的整数";

        public const string User_UserPointInitialValueFormatError = @"积分初始值必须为整数";

        public const string User_UserPointInvalidMinRemainingError = @"积分最低余额必须大于等于积分下限";

        public const string User_UserPointInvalidTradeMaxValueError = @"交易最高值必须大于等于交易最低值";

        public const string User_UserPointIsExistsExchangeRuleError = @"已经存在从“{0}”兑换到“{1}”的规则";

        public const string User_UserPointMaxValueFormatError = @" 积分最大值必须为整数";

        public const string User_UserPointMinRemainingFormatError = @"积分最低余额必须为整数";

        public const string User_UserPointMinValueFormatError = @"积分最小值必须为整数";

        public const string User_UserPointNotSellectExchangePointError = @"请选择要兑换的积分类型";

        public const string User_UserPointNotSellectExchangeTargetPointError = @"请选择要兑换到的目标积分类型";

        public const string User_UserPointPointsExpressionFormatError = @"总积分公式格式不对，请按照积分公式说明填写";

        public const string User_UserPointTradeMaxValueError = @"操作失败!<br />本次操作允许的最高值为{0}：{1} 您当前填的值：{2}";

        public const string User_UserPointTradeMaxValueFormatError = @"交易最高值必须为大于0的整数";

        public const string User_UserPointTradeMinValueError = @"操作失败!<br />本次操作允许的最低值为{0}：{1} 您当前填的值：{2}";

        public const string User_UserPointTradeMinValueFormatError = @"交易最低值必须为大于0的整数";

        public const string User_UserPointTradeRateFormatError = @"积分交易税率必须为大于0的整数";

        public const string User_UserPointTradeRemainingError = @"操作失败!<br />本次操作后您将剩余{0}：{1} 低余系统允许的最低值：{2}";

        public const string User_UserPointTransferMinRemainingError = @"您的余额不足，不能转帐。系统允许转帐后的最小{0}必须为：{1}，您转帐后的{0}为{2}";

        public const string User_UserPointTransferMinRemainingValueFormatError = @"剩余最低余额必须为整数";

        public const string User_UserPointTransferPointValueError = @"要转帐的积分数量必须为大于0的整数";

        public const string User_UserPointTradePointValueError = @"积分数量必须为大于0的整数";

        public const string User_UserPointTransferTaxRateFormatError = @"积分转帐税率必须为不小于0的整数";

        public const string User_UserPointUpgradeIconCountError = @"上升一级图标所需当前图标个数必须为大于0的整数";

        public const string User_UserPointActionDubleSortOrderError = "排序数字不能重复";

        public const string User_UserPointActionInvalidSortOrderError = "排序必须为整数";

        public const string User_UserPointActionEmptyRoleIDError = "请选择一个用户组";

        public const string User_BuyInviteSerialPointError = "购买邀请码积分不足";

        public const string User_RegisterInviterPointShort = "您的邀请人积分不足，因此您不能注册";

        public const string User_UsernameCharError = "用户名：{0} 包含被禁止的关键字，因此，不能注册！";

        public const string User_UsernameHasForbiddenWordsError = "用户名：{0} 已被禁止注册";


        public const string ValidateCode_ValidateCodeActionCannotSetExceptRoleID = "“{0}”不允许设置例外";

        public const string ValidateCode_EmptyValidateCodeError = "请输入验证码";

        public const string ValidateCode_InvalidValidateCodeError = "验证码不正确";

        public const string ValidateCode_LimitedTimeError = "时间限制必须为大于等于0的整数";

        public const string ValidateCode_LimitedCountError = "次数限制必须为大于等于0的整数";

        public const string ValidateCode_RegistValidateCodeTypeError = "已经存在类名为“{0}”的验证码，为避免冲突请修改插件中验证码类名";

        public const string ValidateCode_RegistValidateCodeActionError = "已经存在类型为“{0}”的动作，为避免冲突请修改动作的Type";


        public const string Job_RegistJobTypeError = "已经存在类名为“{0}”的任务，为避免冲突请修改插件中任务类名";

        public const string Judgement_EmptyDescriptionError = "主题鉴定描述不能为空";

        public const string Judgement_EmptyLogoUrlError = "主题图标URL不能为空";


        //public const string Permission_NoPermissionError = @"您没有权限{0}";

        public const string Permission_NoPermissionManageUserMedalError = "管理用户勋章";

        //public const string Permission_NoPermissionBanUserError = "屏蔽、解除屏蔽用户";


        public const string Permission_NoPermissionUseAlbumError = "您没有权限使用相册";


        public const string Permission_NoPermissionUseShareError = "您没有权限使用分享";

        public const string Permission_NoPermissionUseCollectionError = "您没有权限使用收藏";

        //public const string Permission_NoPermissionUseShareAndCollectionError = "您没有权限使用分享和收藏";

        public const string Permission_NoPermissionDeleteShareAndCollectionError = "您没有权限删除分享或者收藏";

        public const string Permission_NoPermissionManageSharAndCollectionError = "您没有权限管理分享或收藏";

		public const string Permission_NoPermissionDeleteMessageError = "您没有权限删除短消息";

		public const string Permission_NoPermissionUseMessageError = "您没有权限使用短消息功能";

		public const string Permission_NoPermissionEditMessageError = "您没有权限编辑短消息";

        public const string Permission_NoPermissionAddCommentError = "您没有权限发表评论";

        public const string Permission_NoPermissionDeleteCommentError = "您没有权限删除评论";

        public const string Permission_NoPermissionDeleteEditedCommentError = "该条评论已经被管理员编辑过,您没有权限删除";

        public const string Permission_NoPermissionApproveCommentError = "您没有权限审核评论";

        public const string Permission_NoPermissionEditCommentError = "您没有权限编辑该评论";

        public const string Permission_NoPermissionEditAdminEditedCommentError = "该评论已经被管理员编辑过,您不能再编辑";

        public const string Permission_NoPermissionDeleteFeedError = @"您没有权限删除动态";

        /*

        public const string Feed_NoPermissionDeleteAllUserFeed = @"您没有权限删除全站动态";

        public const string Feed_NoPermissionDeleteFeedSearchResult = @"您没有权限删除所有搜索结果";

        public const string Feed_NoPermissionDeleteOtherUserFeed = @"您没有权限删除别人的动态";

        public const string Feed_NoPermissionDeleteUserFeed = @"您没有权限删除动态";

        */

        public const string Permission_NoPermissionSetFeedTemplateError = @"您没有权限修改动态模板";

        public const string Permission_NoPermissionManageSiteFeedError = @"您没有权限管理全局动态";

        public const string Permission_NoPermissionCreateSiteFeedError = @"您没有权限创建全局动态";

		public const string Permission_NoPermissionEditAlbumError = "您没有权限编辑相册";

		public const string Permission_NoPermissionDeletePhotoErrror = "您没有权限删除相片";

		public const string Permission_NoPermissionMovePhotoError = "您没有权限移动相片";

		public const string Permission_NoPermissionUpdatePhotoError = "您没有权限更新相片";

		public const string Permission_NoPermissionUpdateAlbumLogoError = "您没有权限更新此相册的图标";

		public const string Permission_NoPermissionManageAlbumError = "您没有权限管理相册";

        public const string Permission_NoPermissionCreateMissionError = "您没有权限创建任务";

        public const string Permission_NoPermissionDeleteMissionError = "您没有权限删除任务";

        public const string Permission_NoPermissionUpdateMissionError = "您没有权限编辑任务";

		public const string Permission_NoPermissionDeleteNotifyError = "您没有权限删除通知";

        //public const string Permission_NoPermissionCancelRateError = "您没有权限撤消评分";


        public const string Permission_NoPermissionCreateAttachmentError = "您在版块“{0}”没有权限上传附件";

        public const string Attachment_NotAllowFileTypeError = "您在版块“{0}”不允许上传扩展名为“{1}”的附件";
        public const string Attachment_InvalidAttachmentError = "附件数据存在异常";
        public const string Attachment_OverAttachmentMaxSingleFileSize = "您在版块“{0}”允许上传的单个附件最大大小为“{1}”,您上传的附件“{2}”大小为“{3}”超过了允许的最大值";
        public const string Attachment_OverTodayAlowAttachmentCount = "超过了今天允许上传的最大附件个数{0}";
        public const string Attachment_OverTodayAlowAttachmentFileSize = "您今天已经上传了{0}的附件，还允许上传附件的总大小为{1}，您当前的附件大小为{2}，超过了系统允许的最大值";
        public const string Attachment_OverMaxTopicAttachmentCount = "您在版块“{0}”发表的主题最多允许上传{1}个附件，您当前已经超过单个主题允许上传的最大附件个数";
        public const string Attachment_OverMaxPostAttachmentCount = "您在版块“{0}”发表的回复最多允许上传{1}个附件，您当前已经超过单个回复允许上传的最大附件个数";

        public const string Topic_PostRateOverMaxValue = @"您评的“{0}:{1}”超过了系统允许的最大值“{2}”";
        public const string Topic_PostRateOverMinValue = @"您评的“{0}:{1}”超过了系统允许的最小值“{2}”";
        public const string Topic_PostRateOverMaxValueInTime = @"您评的“{0}:{1}”超过了24小时内允许评分的最大值“{2}”";
        public const string Topic_PostRateHadRated = @"对不起，您不能对同一个帖子重复评分。";

        public const string Topic_PostNotExists = @"帖子不存在。";
        public const string Topic_PostRateNotExists = @"您要撤消的评分不存在。";
    }
}