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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.DataAccess
{
    public abstract class ThreadCateDao : DaoBase<ThreadCateDao>
    {
        public abstract ThreadCateCollection GetAllCates();

        public abstract bool CreateThreadCate(string cateName, bool enable, int sortOrder);

        public abstract int UpdateThreadCate(int cateID, string cateName, bool enable, int sortOrder);

        public abstract bool UpdateThreadCates(IEnumerable<int> cateIDs, IEnumerable<bool> enables, IEnumerable<int> sortOrders);
        public abstract bool DeleteThreadCates(IEnumerable<int> cateIDs);

        public abstract Dictionary<int,ThreadCateModelCollection> GetAllModels();

        public abstract int CreateModel(int cateID, string modelName, bool enable, int sortOrder);

        public abstract bool UpdateModels(IEnumerable<int> modelIDs, IEnumerable<int> sortOrders, IEnumerable<string> modelNames, IEnumerable<int> enableIDs);

        public abstract bool DeleteModels(IEnumerable<int> modelIDs);

        public abstract bool EnableModels(int cateID, IEnumerable<int> modelIDs);

        public abstract bool DeleteModelFields(IEnumerable<int> fieldIDs);

        public abstract bool UpdateThreadCateModelFields(IEnumerable<int> fieldIDs, IEnumerable<bool> enables, IEnumerable<int> sortOrders
            , IEnumerable<bool> searchs, IEnumerable<bool> advancedSearchs, IEnumerable<bool> displayInLists, IEnumerable<bool> mustFilleds);


        public abstract int UpdateThreadCateModelField(int fieldID, string fieldName, bool enable, int sortOrder
            , string fieldTypeSetting, bool search, bool advancedSearch, bool displayInList, bool mustFilled, string description);


        public abstract int CreateThreadCateModelField(int modelID, string fieldName, bool enable, int sortOrder, string fieldType
            , string fieldTypeSetting, bool search, bool advancedSearch, bool displayInList, bool mustFilled, string description);

        public abstract Dictionary<int, ThreadCateModelFieldCollection> GetAllThreadCateModelField();

        public abstract bool LoadModelFields(int modelID, IEnumerable<int> fieldIDs);
    }
}