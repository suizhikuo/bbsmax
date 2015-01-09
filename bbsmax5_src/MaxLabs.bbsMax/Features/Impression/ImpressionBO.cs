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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Filters;
using System.Collections;

namespace MaxLabs.bbsMax
{
    public class ImpressionBO : BOBase<ImpressionBO>
    {
        protected SpacePermissionSet Permission
        {
            get
            {
                return AllSettings.Current.SpacePermissionSet;
            }
        }

        protected BackendPermissions ManagePermission
        {
            get
            {
                return AllSettings.Current.BackendPermissions;
            }
        }

        public bool CreateImpression(AuthUser operatorUser, User targetUser, string text)
        {
            if (operatorUser == null || Permission.Can(operatorUser, SpacePermissionSet.Action.UseImpression) == false)
                return false;

            if (targetUser == null || Permission.Can(targetUser, SpacePermissionSet.Action.UseImpression) == false)
                return false;

            if (text == null)
            {
                ThrowError(new ImpressionTextEmptyError("text"));
                return false;
            }

            text = text.Trim();

            if (ValidateText(text) == false)
                return false;

            bool succeed = ImpressionDao.Instance.CreateImpression(operatorUser.UserID, targetUser.UserID, text, AllSettings.Current.ImpressionSettings.TimeLimit);

            if (succeed)
            {
                Notify notify = new ImpressionNotify(operatorUser.UserID, targetUser.UserID);
                NotifyBO.Instance.AddNotify(operatorUser, notify);
                RemoveCachedTargetUserImpressionRecordsTotalCount(targetUser.UserID);
            }

            return succeed;
        }

        public void DeleteImpressionTypeForUser(AuthUser operatorUser, int typeID)
        {
            ImpressionDao.Instance.DeleteImpressionTypeForUser(operatorUser.UserID, typeID);
        }

        public bool DeleteImpressionTypesForAdmin(AuthUser operatorUser, AdminImpressionTypeFilter filter, int stepDeleteCount, out int stepCount)
        {
            stepCount = 0;

            if (ManagePermission.Can(operatorUser, BackendPermissions.Action.Manage_ImpressionRecord) == false)
            {
                return false;
            }

            return ImpressionDao.Instance.DeleteImpressionTypesForAdmin(filter, stepDeleteCount, out stepCount);
        }

        public bool DeleteImpressionTypes(AuthUser operatorUser, int[] typeIDs)
        {
            if (ManagePermission.Can(operatorUser, BackendPermissions.Action.Manage_ImpressionRecord) == false)
            {
                return false;
            }

            return ImpressionDao.Instance.DeleteImpressionTypes(typeIDs);
        }

        public bool DeleteImpressionRecordsForAdmin(AuthUser operatorUser, AdminImpressionRecordFilter filter, int stepDeleteCount, out int stepCount)
        {
            stepCount = 0;

            if (ManagePermission.Can(operatorUser, BackendPermissions.Action.Manage_ImpressionRecord) == false)
            {
                return false;
            }

            return ImpressionDao.Instance.DeleteImpressionRecordsForAdmin(filter, stepDeleteCount, out stepCount);
        }

        public bool DeleteImpressionRecords(AuthUser operatorUser, int[] recordIDs)
        {
            if (ManagePermission.Can(operatorUser, BackendPermissions.Action.Manage_ImpressionRecord) == false)
            {
                return false;
            }

            return ImpressionDao.Instance.DeleteImpressionRecords(recordIDs);
        }

        public FriendCollection GetFriendsHasImpressions(int userID, int pageNumber, int pageSize)
        {
            pageNumber = pageNumber < 0 ? 1 : pageNumber;

            return ImpressionDao.Instance.GetFriendsHasImpressions(userID, pageNumber, pageSize);
        }

        public Hashtable GetTopImressionTypesForUsers(int[] userIDs, int top)
        {
            if (userIDs.Length == 0)
                return new Hashtable();

            return ImpressionDao.Instance.GetImressionsTypesForUsers(userIDs, top);
        }

        public ImpressionTypeCollection GetImpressionTypesForUse(int targetUserID, int usrCount, int sysCount)
        {
            return ImpressionDao.Instance.GetImpressionTypesForUse(targetUserID, usrCount, sysCount);
        }

        public ImpressionRecord GetLastImpressionRecord(int userID, int targetUserID)
        {
            return ImpressionDao.Instance.GetLastImpressionRecord(userID, targetUserID);
        }

        public ImpressionRecordCollection GetTargetUserImpressionRecords(int targetUserID, int pageNumber, int pageSize)
        {
            ImpressionRecordCollection result = null;

            string cacheKey = GetCacheKeyForTargetUserImpressionRecordsTotalCount(targetUserID);

            int? totalCount = null;

            bool totalCountCached = CacheUtil.TryGetValue<int?>(cacheKey, out totalCount);

            result = ImpressionDao.Instance.GetTargetUserImpressionRecords(targetUserID, pageNumber, pageSize, ref totalCount);

            if (totalCountCached == false)
                CacheUtil.Set<int?>(cacheKey, totalCount);

            ProcessKeyword(result, ProcessKeywordMode.TryUpdateKeyword);

            return result;
        }

        public ImpressionTypeCollection GetImpressionTypesForAdmin(AuthUser operatorUser, AdminImpressionTypeFilter filter, int pageNumber)
        {
            if (ManagePermission.Can(operatorUser, BackendPermissions.Action.Manage_ImpressionType) == false)
            {
                return null;
            }

            return ImpressionDao.Instance.GetImpressionTypesForAdmin(filter, pageNumber);
        }

        public ImpressionRecordCollection GetImpressionRecordsForAdmin(AuthUser operatorUser, AdminImpressionRecordFilter filter, int pageNumber)
        {
            if (ManagePermission.Can(operatorUser, BackendPermissions.Action.Manage_ImpressionRecord) == false)
            {
                return null;
            }

            return ImpressionDao.Instance.GetImpressionRecordsForAdmin(filter, pageNumber);
        }

        private void ProcessKeyword(ImpressionRecord record, ProcessKeywordMode mode)
        {
            //更新关键字模式，如果这个文章并不需要处理，直接退出
            if (mode == ProcessKeywordMode.TryUpdateKeyword)
            {
                if (AllSettings.Current.ContentKeywordSettings.ReplaceKeywords.NeedUpdate(record) == false)
                    return;
            }

            ImpressionRecordCollection records = new ImpressionRecordCollection();

            records.Add(record);

            ProcessKeyword(records, mode);
        }

        private void ProcessKeyword(ImpressionRecordCollection records, ProcessKeywordMode mode)
        {
            if (records.Count == 0)
                return;

            KeywordReplaceRegulation keyword = AllSettings.Current.ContentKeywordSettings.ReplaceKeywords;

            bool needProcess = false;

            //更新关键字模式，只在必要的情况下才取恢复信息并处理
            if (mode == ProcessKeywordMode.TryUpdateKeyword)
            {
                needProcess = keyword.NeedUpdate<ImpressionRecord>(records);
            }
            //填充原始内容模式，始终都要取恢复信息，但不处理
            else
            {
                needProcess = true;
            }

            if (needProcess)
            {
                RevertableCollection<ImpressionRecord> recordsWithReverter = ImpressionDao.Instance.GetImpressionRecordsWithReverters(records.GetKeys());

                if (recordsWithReverter != null)
                {
                    if (keyword.Update(recordsWithReverter))
                    {
                        ImpressionDao.Instance.UpdateImpressionRecordKeywords(recordsWithReverter);
                    }

                    //将新数据填充到旧的列表
                    recordsWithReverter.FillTo(records);
                }
            }
        }


        private bool ValidateText(string text)
        {
            int maxLength = 100;

            if (string.IsNullOrEmpty(text))
            {
                ThrowError(new ImpressionTextEmptyError("text"));
                return false;
            }

            if (StringUtil.GetByteCount(text) > maxLength)
            {
                ThrowError(new ImpressionTextLengthError("text", text, maxLength));
                return false;
            }

            ContentKeywordSettings keywords = AllSettings.Current.ContentKeywordSettings;

            string keyword = null;

            if (keywords.BannedKeywords.IsMatch(text, out keyword))
            {
                ThrowError(new ImpressionTextBannedKeywordsError("text", keyword));
                return false;
            }

            return true;
        }

        private string GetCacheKeyForTargetUserImpressionRecordsTotalCount(int targetUserID)
        {
            return "Impression/" + targetUserID + "/TargetUserImpressionRecordsTotalCount";
        }

        private void RemoveCachedTargetUserImpressionRecordsTotalCount(int targetUserID)
        {
            CacheUtil.Remove(GetCacheKeyForTargetUserImpressionRecordsTotalCount(targetUserID));
        }
    }
}

namespace MaxLabs.bbsMax.Errors
{
    public class ImpressionTextEmptyError : ErrorInfo
    {
        public ImpressionTextEmptyError(string target)
            : base(target)
        {
        }

        public override string Message
        {
            get { return "印象不能为空"; }
        }
    }

    public class ImpressionTextLengthError :ParamError<string>
    {
        private int m_MaxLength;

        public ImpressionTextLengthError(string target, string value, int maxLength)
            : base(target, value)
        {
            m_MaxLength = maxLength;
        }

        public override string Message
        {
            get
            {
                int length = StringUtil.GetByteCount(ParamValue);

                return string.Format("印象超过最大允许的{0}个字", m_MaxLength);
            }
        }

        public int MaxLength { get { return m_MaxLength; } }

    }

    public class ImpressionTextBannedKeywordsError : ErrorInfo
    {
        public ImpressionTextBannedKeywordsError(string target, string keyword)
            : base(target)
        {
            Keyword = keyword;
        }

        public string Keyword
        {
            get;
            private set;
        }

        public override string Message
        {
            get
            {
                if (IsShowKeywordContent)
                    return "印象中包含被禁止的关键字：" + Keyword;
                else
                    return "印象中包含被禁止的关键字.";
            }
        }

        public override bool HtmlEncodeMessage
        {
            get { return true; }
        }

        private bool IsShowKeywordContent
        {
            get { return AllSettings.Current.ContentKeywordSettings.IsShowKeywordContent; }
        }
    }
}

namespace MaxLabs.bbsMax.Filters
{
    public class AdminImpressionTypeFilter : FilterBase<AdminImpressionTypeFilter>
    {
        public enum OrderBy
        {
            TypeID = 1,

            RecordCount = 2
        }

        public AdminImpressionTypeFilter()
        {
            this.IsDesc = true;
            this.PageSize = Consts.DefaultPageSize;
        }

        [FilterItem(FormName = "searchkey")]
        public string SearchKey
        {
            get;
            set;
        }

        [FilterItem(FormName = "order")]
        public OrderBy Order
        {
            get;
            set;
        }

        [FilterItem(FormName = "IsDesc")]
        public bool IsDesc
        {
            get;
            set;
        }

        [FilterItem(FormName = "pagesize")]
        public int PageSize
        {
            get;
            set;
        }

    }
}

namespace MaxLabs.bbsMax.Filters
{
    public class AdminImpressionRecordFilter : FilterBase<AdminImpressionRecordFilter>
    {
        public AdminImpressionRecordFilter()
        {
            this.IsDesc = true;
            this.PageSize = Consts.DefaultPageSize;
        }

        [FilterItem(FormName = "typeid")]
        public int? TypeID
        {
            get;
            set;
        }

        [FilterItem(FormName = "searchkey")]
        public string SearchKey
        {
            get;
            set;
        }

        [FilterItem(FormName = "userid")]
        public int? UserID
        {
            get;
            set;
        }

        [FilterItem(FormName = "targetuserid")]
        public int? TargetUserID
        {
            get;
            set;
        }

        [FilterItem(FormName = "user")]
        public string User
        {
            get;
            set;
        }

        [FilterItem(FormName = "targetuser")]
        public string TargetUser
        {
            get;
            set;
        }

        [FilterItem(FormName = "BeginDate", FormType = FilterItemFormType.BeginDate)]
        public DateTime? BeginDate
        {
            get;
            set;
        }

        [FilterItem(FormName = "EndDate", FormType = FilterItemFormType.EndDate)]
        public DateTime? EndDate
        {
            get;
            set;
        }

        [FilterItem(FormName = "IsDesc")]
        public bool IsDesc
        {
            get;
            set;
        }

        [FilterItem(FormName = "pagesize")]
        public int PageSize
        {
            get;
            set;
        }

    }
}