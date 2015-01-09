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

using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Common;
using System.IO;
using MaxLabs.WebEngine.Template;

namespace MaxLabs.bbsMax
{
    public class ThreadCateBO : BOBase<ThreadCateBO>
    {
        private static ThreadCateCollection allThreadCates = null;
        private const int threadCateNameMaxLength = 20;
        private const int modelNameMaxLength = 20;
        private const int fieldNameMaxLength = 20; 

        public ThreadCateCollection GetAllCates()
        {
            if (allThreadCates == null)
                allThreadCates = ThreadCateDao.Instance.GetAllCates();

            return allThreadCates;
        }

        public bool CreateThreadCate(AuthUser operatorUser, string cateName, bool enable, int sortOrder)
        {
            if (AllSettings.Current.BackendPermissions.Can(operatorUser, BackendPermissions.Action.Manage_ThreadCate) == false)
            {
                ThrowError(new NoPermissionManageThreadCateError());
                return false;
            }
            if (string.IsNullOrEmpty(cateName))
            {
                ThrowError(new EmptyThreadCateNameError("cateName"));
                return false;
            }
            if (StringUtil.GetByteCount(cateName) > threadCateNameMaxLength)
            {
                ThrowError(new InvalidThreadCateNameLengthError("cateName", cateName, threadCateNameMaxLength));
                return false;
            }
            bool success = ThreadCateDao.Instance.CreateThreadCate(cateName, enable, sortOrder);
            if (success)
            {
                allThreadCates = null;
                models = null;
            }
            return success;
        }

        public bool UpdateThreadCate(AuthUser operatorUser, int cateID, string cateName, bool enable, int sortOrder, IEnumerable<int> enableModelIDs)
        {

            if (AllSettings.Current.BackendPermissions.Can(operatorUser, BackendPermissions.Action.Manage_ThreadCate) == false)
            {
                ThrowError(new NoPermissionManageThreadCateError());
                return false;
            }

            if (string.IsNullOrEmpty(cateName))
            {
                ThrowError(new EmptyThreadCateNameError("cateName"));
                return false;
            }
            if (StringUtil.GetByteCount(cateName) > threadCateNameMaxLength)
            {
                ThrowError(new InvalidThreadCateNameLengthError("cateName", cateName, threadCateNameMaxLength));
                return false;
            }
            int result = ThreadCateDao.Instance.UpdateThreadCate(cateID, cateName, enable, sortOrder);
            if (result > 0)
            {
                allThreadCates = null;
                EnableModels(operatorUser, cateID, enableModelIDs);
            }
            else
            {
                ThrowError(new ThreadCateNotExistsError(cateID));
                return false;
            }
            return true;
        }

        public bool UpdateThreadCates(AuthUser operatorUser, IEnumerable<int> cateIDs, IEnumerable<bool> enables, IEnumerable<int> sortOrders)
        {

            if (AllSettings.Current.BackendPermissions.Can(operatorUser, BackendPermissions.Action.Manage_ThreadCate) == false)
            {
                ThrowError(new NoPermissionManageThreadCateError());
                return false;
            }
            bool success = ThreadCateDao.Instance.UpdateThreadCates(cateIDs, enables, sortOrders);
            if (success)
                allThreadCates = null;

            return success;
        }

        public bool DeleteThreadCates(AuthUser operatorUser, IEnumerable<int> cateIDs)
        {
            if (AllSettings.Current.BackendPermissions.Can(operatorUser, BackendPermissions.Action.Manage_ThreadCate) == false)
            {
                ThrowError(new NoPermissionManageThreadCateError());
                return false;
            }
            if (ValidateUtil.HasItems<int>(cateIDs) == false)
                return true;

            bool success = ThreadCateDao.Instance.DeleteThreadCates(cateIDs);
            if (success)
                allThreadCates = null;

            return success;
        }



        private Dictionary<int, ThreadCateModelCollection> models = null;
        public ThreadCateModelCollection GetModels(int cateID)
        {
            if (models == null)
            {
                models = ThreadCateDao.Instance.GetAllModels();
            }

            ThreadCateModelCollection tempModels;

            models.TryGetValue(cateID,out tempModels);

            if (tempModels == null)
                return new ThreadCateModelCollection();
            else
                return tempModels;
        }

        public ThreadCateModel GetThreadCateModel(int cateID, int modelID)
        {
            return GetModels(cateID).GetValue(modelID);
        }

        public bool CreateModel(AuthUser operatorUser, int cateID, string modelName, bool enable, int sortOrder)
        {
            if (AllSettings.Current.BackendPermissions.Can(operatorUser, BackendPermissions.Action.Manage_ThreadCate) == false)
            {
                ThrowError(new NoPermissionManageThreadCateError());
                return false;
            }
            if (string.IsNullOrEmpty(modelName))
            {
                ThrowError(new EmptyThreadCateModelNameError("modelName", 0));
                return false;
            }
            if (StringUtil.GetByteCount(modelName) > modelNameMaxLength)
            {
                ThrowError(new InvalidThreadCateModelNameLengthError("modelName", 0, modelName, modelNameMaxLength));
                return false;
            }

            int result = ThreadCateDao.Instance.CreateModel(cateID, modelName, enable, sortOrder);
            if (result > 0)
            {
                models = null;
                return true;
            }
            else
            {
                ThrowError(new ThreadCateNotExistsError(cateID));
                return false;
            }
        }

        public bool UpdateModels(AuthUser operatorUser, IEnumerable<int> modelIDs, IEnumerable<int> sortOrders, IEnumerable<string> modelNames, IEnumerable<int> enableIDs)
        {
            if (AllSettings.Current.BackendPermissions.Can(operatorUser, BackendPermissions.Action.Manage_ThreadCate) == false)
            {
                ThrowError(new NoPermissionManageThreadCateError());
                return false;
            }
            if (ValidateUtil.HasItems<int>(modelIDs) == false)
                return true;

            int i = 0;
            foreach (string name in modelNames)
            {
                if (string.IsNullOrEmpty(name))
                {
                    ThrowError(new EmptyThreadCateModelNameError("modelName", i));
                }
                if (StringUtil.GetByteCount(name) > modelNameMaxLength)
                {
                    ThrowError(new InvalidThreadCateModelNameLengthError("modelName", i, name, modelNameMaxLength));
                }
                i++;
            }

            if (HasUnCatchedError)
                return false;

            bool success = ThreadCateDao.Instance.UpdateModels(modelIDs, sortOrders, modelNames, enableIDs);
            if (success)
            {
                models = null;
            }

            return success;
        }

        public bool DeleteModels(AuthUser operatorUser, IEnumerable<int> modelIDs)
        {
            if (AllSettings.Current.BackendPermissions.Can(operatorUser, BackendPermissions.Action.Manage_ThreadCate) == false)
            {
                ThrowError(new NoPermissionManageThreadCateError());
                return false;
            }
            if (ValidateUtil.HasItems<int>(modelIDs) == false)
                return true;

            bool success = ThreadCateDao.Instance.DeleteModels(modelIDs);
            if (success)
            {
                models = null;
            }
            return success;
        }

        /// <summary>
        /// 起用该分类主题下的模板  
        /// </summary>
        /// <param name="cateID"></param>
        /// <param name="modelIDs">启用的 其它的都禁用</param>
        /// <returns></returns>
        public bool EnableModels(AuthUser operatorUser, int cateID, IEnumerable<int> modelIDs)
        {
            if (AllSettings.Current.BackendPermissions.Can(operatorUser, BackendPermissions.Action.Manage_ThreadCate) == false)
            {
                ThrowError(new NoPermissionManageThreadCateError());
                return false;
            }
            bool success = ThreadCateDao.Instance.EnableModels(cateID, modelIDs);
            if (success)
                models = null;

            return success;
        }



        private Dictionary<int, ThreadCateModelFieldCollection> allThreadCateModelFields = null;

        public ThreadCateModelField GetField(int modelID, int fieldID)
        {
            ThreadCateModelField field;
            GetFields(modelID).TryGetValue(fieldID,out field);

            return field;
        }

        public ThreadCateModelFieldCollection GetFields(int modelID)
        {
            if (allThreadCateModelFields == null)
            {
                allThreadCateModelFields = ThreadCateDao.Instance.GetAllThreadCateModelField();
            }

            ThreadCateModelFieldCollection fields;

            allThreadCateModelFields.TryGetValue(modelID, out fields);

            if (fields == null)
                return new ThreadCateModelFieldCollection();
            else
                return fields;
        }

        public bool DeleteModelFields(AuthUser operatorUser, IEnumerable<int> fieldIDs)
        {

            if (AllSettings.Current.BackendPermissions.Can(operatorUser, BackendPermissions.Action.Manage_ThreadCate) == false)
            {
                ThrowError(new NoPermissionManageThreadCateError());
                return false;
            }

            if (ValidateUtil.HasItems<int>(fieldIDs) == false)
                return true;

            bool success = ThreadCateDao.Instance.DeleteModelFields(fieldIDs);
            if (success)
                allThreadCateModelFields = null;

            return success;
        }

        public bool UpdateThreadCateModelFields(AuthUser operatorUser, IEnumerable<int> fieldIDs, IEnumerable<bool> enables, IEnumerable<int> sortOrders
            , IEnumerable<bool> searchs, IEnumerable<bool> advancedSearchs, IEnumerable<bool> displayInLists, IEnumerable<bool> mustFilleds)
        {

            if (AllSettings.Current.BackendPermissions.Can(operatorUser, BackendPermissions.Action.Manage_ThreadCate) == false)
            {
                ThrowError(new NoPermissionManageThreadCateError());
                return false;
            }

            if (ValidateUtil.HasItems<int>(fieldIDs) == false)
                return true;

            bool success = ThreadCateDao.Instance.UpdateThreadCateModelFields(fieldIDs, enables, sortOrders, searchs, advancedSearchs, displayInLists, mustFilleds);
            if (success)
                allThreadCateModelFields = null;

            return success;
        }


        public bool UpdateThreadCateModelField(AuthUser operatorUser, int fieldID, string fieldName, bool enable, int sortOrder
            , string fieldTypeSetting, bool search, bool advancedSearch, bool displayInList, bool mustFilled, string description)
        {

            if (AllSettings.Current.BackendPermissions.Can(operatorUser, BackendPermissions.Action.Manage_ThreadCate) == false)
            {
                ThrowError(new NoPermissionManageThreadCateError());
                return false;
            }

            if (string.IsNullOrEmpty(fieldName))
            {
                ThrowError(new EmptyThreadCateModelFieldNameError("fieldName", 0));
                return false;
            }

            if (StringUtil.GetByteCount(fieldName) > fieldNameMaxLength)
            {
                ThrowError(new InvalidThreadCateModelFieldNameLengthError("fieldName", 0, fieldName, fieldNameMaxLength));
                return false;
            }

            int returnValue = ThreadCateDao.Instance.UpdateThreadCateModelField(fieldID, fieldName, enable, sortOrder, fieldTypeSetting, search, advancedSearch, displayInList, mustFilled, description);

            if (returnValue == 0)
            {
                ThrowError(new ThreadCateModelFieldNotExistsError(fieldID));
                return false;
            }
            else
                allThreadCateModelFields = null;

            return true;
        }


        public bool CreateThreadCateModelField(AuthUser operatorUser, int modelID, string fieldName, bool enable, int sortOrder, string fieldType
            , string fieldTypeSetting, bool search, bool advancedSearch, bool displayInList, bool mustFilled, string description)
        {

            if (AllSettings.Current.BackendPermissions.Can(operatorUser, BackendPermissions.Action.Manage_ThreadCate) == false)
            {
                ThrowError(new NoPermissionManageThreadCateError());
                return false;
            }

            if (string.IsNullOrEmpty(fieldName))
            {
                ThrowError(new EmptyThreadCateModelFieldNameError("fieldName", 0));
                return false;
            }

            if (StringUtil.GetByteCount(fieldName) > fieldNameMaxLength)
            {
                ThrowError(new InvalidThreadCateModelFieldNameLengthError("fieldName", 0, fieldName, fieldNameMaxLength));
                return false;
            }

            int returnValue = ThreadCateDao.Instance.CreateThreadCateModelField(modelID, fieldName, enable, sortOrder, fieldType, fieldTypeSetting, search, advancedSearch, displayInList, mustFilled, description);

            if (returnValue == 0)
            {
                ThrowError(new ThreadCateModelNotExistsError(modelID));
                return false;
            }
            else
                allThreadCateModelFields = null;

            return true;
        }

        public bool LoadModelFields(AuthUser operatorUser, int modelID, IEnumerable<int> fieldIDs)
        {
            if (AllSettings.Current.BackendPermissions.Can(operatorUser, BackendPermissions.Action.Manage_ThreadCate) == false)
            {
                ThrowError(new NoPermissionManageThreadCateError());
                return false;
            }

            if (ValidateUtil.HasItems<int>(fieldIDs) == false)
                return true;

            bool success = ThreadCateDao.Instance.LoadModelFields(modelID, fieldIDs);
            if (success)
            {
                allThreadCateModelFields = null;
            }

            return success;
        }

    }
}