//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using System.Collections.Generic;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.ExceptableSetting
{
    public abstract class ExceptableItemBase<T1> : MaxLabs.WebEngine.PageBase
    {
        public Exceptable<T1> GetExceptable(string name, MessageDisplay msgDisplay)
        {
            int[] tempIds = StringUtil.Split<int>(_Request.Get("id_" + name, Method.Post, string.Empty));

            List<int> sortOrdes = new List<int>();
            Exceptable<T1> items = new Exceptable<T1>(default(T1));
            ExceptionItem<T1> item;

            List<Guid> roleIDs = new List<Guid>(); 
            foreach (int id in tempIds)
            {
                item = GetExceptionItem(name, id, false, msgDisplay);
                checkItem(items, item, name, id, sortOrdes, roleIDs, msgDisplay);
            }

            item = GetExceptionItem(name, 0, true, msgDisplay);
            checkNewItem(items, item, name, sortOrdes, roleIDs, msgDisplay);

            return items;
        }

        protected abstract T1 GetItemValue(string name, int id, bool isNew, MessageDisplay msgDisplay, out bool hasError);

        /// <summary>
        /// 获取 除了例外的 用户组 排序 级别   还未获取T的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <param name="isNew"></param>
        /// <param name="msgDisplay"></param>
        /// <returns></returns>
        private ExceptionItem<T1> GetExceptionItem(string name, int id, bool isNew, MessageDisplay msgDisplay)
        {
            if (_Request.Get<int>("delete_" + name + "_" + id, Method.Post, 0) == 1)//被删除的项
                return null;

            Guid roleID;
            LevelStatus level = LevelStatus.Currently;
            int sortOrder = 0;
            if (id == 0 && isNew == false)
                roleID = Guid.Empty;
            else
            {
                string roleIDName, sortOrderName, levelName;
                if (isNew)
                {
                    roleIDName = "new_" + name + "_role";
                    sortOrderName = "new_" + name + "_sortorder";
                    levelName = "new_" + name + "_level";
                }
                else
                {
                    roleIDName = name + "_role_" + id;
                    sortOrderName = name + "_sortorder_" + id;
                    levelName = name + "_level_" + id;
                }

                roleID = _Request.Get<Guid>(roleIDName, Method.Post, Guid.Empty);
                if (roleID == Guid.Empty)
                {
                    if (isNew && _Request.Get("display_tr_" + name, Method.Post, "0") == "1")
                    {
                        msgDisplay.AddError("new_" + name, "请选择一个用户组");
                    }
                    else
                        return null;
                }
                string value = _Request.Get(sortOrderName, Method.Post, string.Empty);

                if (value == string.Empty)
                {
                    sortOrder = -1;
                }
                else if (!int.TryParse(value, out sortOrder))
                {
                    if (isNew)
                        msgDisplay.AddError("new_" + name, "排序必须为整数");
                    else
                        msgDisplay.AddError(name, id, "排序必须为整数");
                }

                level = _Request.Get<LevelStatus>(levelName, Method.Post, LevelStatus.Currently);
            }


            bool hasError;

            ExceptionItem<T1> t = new ExceptionItem<T1>();
            t.LevelStatus = level;
            t.RoleID = roleID;
            t.SortOrder = sortOrder;
            t.Value = GetItemValue(name, id, isNew, msgDisplay, out hasError);

            if (hasError)
                return null;

            return t;
        }

        private void checkItem(Exceptable<T1> items, ExceptionItem<T1> item, string name, int id, List<int> sortOrders, List<Guid> roleIDs, MessageDisplay msgDisplay)
        {
            if (item != null)
            {
                //msgDisplay.HasAnyError == false 是为避免  有两个SortOrder 不为数字出错了（这时都为0）  而这里又提示重复
                if (id != 0 && sortOrders.Contains(item.SortOrder) && msgDisplay.HasAnyError() == false)
                {
                    msgDisplay.AddError(name, id, "排序数字不能重复");
                }
                else
                {
                    if (id != 0)
                        sortOrders.Add(item.SortOrder);
                    //items.Add(item);
                }

                if (item.RoleID != Guid.Empty && roleIDs.Contains(item.RoleID))
                {
                    msgDisplay.AddError(name, id, "已经存在对用户组“" + AllSettings.Current.RoleSettings.GetRole(item.RoleID).Name + "”的例外设置，不能重复设置");
                }
                else
                {
                    if (item.RoleID != Guid.Empty)
                    {
                        roleIDs.Add(item.RoleID);
                    }
                    items.Add(item);
                }
            }
        }
        private void checkNewItem(Exceptable<T1> items, ExceptionItem<T1> item, string name, List<int> sortOrders, List<Guid> roleIDs, MessageDisplay msgDisplay)
        {
            if (item != null)
            {
                if (item.SortOrder == -1)
                {
                    int maxSortOrder = 0;
                    foreach (int sortOrder in sortOrders)
                    {
                        if (sortOrder > maxSortOrder)
                            maxSortOrder = sortOrder;
                    }
                    item.SortOrder = maxSortOrder + 1;
                }
                else if (sortOrders.Contains(item.SortOrder) && msgDisplay.HasAnyError() == false)
                {
                    msgDisplay.AddError("new_" + name, "排序数字不能重复");
                }

                if (item.RoleID != Guid.Empty && roleIDs.Contains(item.RoleID))
                {
                    msgDisplay.AddError("new_" + name, "已经存在对用户组“" + AllSettings.Current.RoleSettings.GetRole(item.RoleID).Name + "”的例外设置，不能重复设置");
                }
                else
                {
                    if (item.RoleID != Guid.Empty)
                    {
                        roleIDs.Add(item.RoleID);
                    }
                }
                items.Add(item);
            }
        }


        /// <summary>
        /// 是否应用到所有节点（如版块）
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool AplyAllNode(string name)
        {
            return _Request.Get<bool>(name + "_aplyallnode", Method.Post, false);
        }
    }
}