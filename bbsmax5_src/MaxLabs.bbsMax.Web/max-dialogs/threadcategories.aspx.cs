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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using System.Collections.Generic;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class threadcategories : AdminDialogPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Manage_Forum; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Forum.ParentID == 0)
            {
                ShowError("当前版块是分类版块不能添加主题分类");
            }

            if (_Request.IsClick("savecatagories"))
            {
                SaveThreadCatalogs();
                this.IsChanged = true;
            }
            else if (_Request.Get("action", Method.Get, string.Empty).ToLower().Trim() == "delete")
            {
                DeleteThreadCatalog();
            }
        }


        protected bool IsChanged
        {
            get;
            set;
        }

        private void DeleteThreadCatalog()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();
            int threadCatalogID = _Request.Get<int>("threadCatalogID", Method.Get, 0);

            if (threadCatalogID < 1)
            {
                msgDisplay.AddError(new InvalidParamError("threadcatalogID").Message);
                return;
            }

            try
            {
                ForumBO.Instance.DeleteForumThreadCatalog(Forum.ForumID, threadCatalogID);
                forum = null;
                _Request.Clear(Method.Post);
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }
        }

        private void SaveThreadCatalogs()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("sortOrder", "threadcatalogname");

            ThreadCatalogStatus threadCatalogStatus = _Request.Get<ThreadCatalogStatus>("threadCategorySet", Method.Post, ThreadCatalogStatus.Enable);

            if (threadCatalogStatus == ThreadCatalogStatus.DisEnable)
            {
                if (Forum.ThreadCatalogStatus != threadCatalogStatus)
                {
                    try
                    {
                        bool success = ForumBO.Instance.UpdateForumThreadCatalogStatus(Forum.ForumID, threadCatalogStatus);
                        if (!success)
                        {
                            CatchError<ErrorInfo>(delegate(ErrorInfo error)
                            {
                                msgDisplay.AddError(error);
                            });
                        }
                        else
                        {
                            forum = null;
                            _Request.Clear(Method.Post);
                        }
                    }
                    catch (Exception ex)
                    {
                        msgDisplay.AddError(ex.Message);
                    }

                    return;
                }
            }


            int[] indexs = _Request.GetList<int>("catagories", Method.Post, new int[0]);

            //names = new Dictionary<int, string>();


            //List<ForumThreadCatalog> forumThreadCatalogs = new List<ForumThreadCatalog>();

            forumThreadCatalogs = new ForumThreadCatalogCollection();

            Dictionary<int, string> indexAndNames = new Dictionary<int, string>();

            List<int> sortOrders = new List<int>();

            List<string> newCatalogNames = new List<string>();

            foreach (int i in indexs)
            {
                ForumThreadCatalog catalog = new ForumThreadCatalog();
                catalog.ForumID = Forum.ForumID;
                catalog.ThreadCatalogID = _Request.Get<int>("threadCatagories_" + i, Method.Post, 0);
                catalog.SortOrder = _Request.Get<int>("sortorder_" + i, Method.Post, 0);
                string name = _Request.Get("threadCatalogName_" + i, Method.Post, string.Empty, false).Trim();
                catalog.ThreadCatalog = new ThreadCatalog();
                catalog.ThreadCatalog.ThreadCatalogName = name;


                if (name == string.Empty)
                {
                    msgDisplay.AddError("threadcatalogname", i, "分类名称不能为空");
                }
                else
                {
                    if (indexAndNames.ContainsValue(name))
                    {
                        msgDisplay.AddError("threadcatalogname", i, "重复的分类名称");
                    }
                    indexAndNames.Add(i, name);
                }


                if (sortOrders.Contains(catalog.SortOrder))
                {
                    msgDisplay.AddError("SortOrder", i, "重复的排序数字");
                }

                if (catalog.ThreadCatalogID == 0)
                    newCatalogNames.Add(name);

                sortOrders.Add(catalog.SortOrder);

                if (forumThreadCatalogs.GetValue(catalog.ForumID, catalog.ThreadCatalogID) == null)
                    forumThreadCatalogs.Add(catalog);

            }


            int[] newIndexs = _Request.GetList<int>("newcatagories", Method.Post, new int[0]);


            List<ForumThreadCatalog> newForumThreadCatalogs = new List<ForumThreadCatalog>();
            int j = 0;
            foreach (int i in newIndexs)
            {
                int tempI = j + indexs.Length;
                ForumThreadCatalog catalog = new ForumThreadCatalog();
                catalog.ForumID = Forum.ForumID;
                catalog.ThreadCatalogID = _Request.Get<int>("new_threadCatagories_" + i, Method.Post, 0);
                catalog.SortOrder = _Request.Get<int>("new_sortorder_" + i, Method.Post, 0);
                string name = _Request.Get("new_threadCatalogName_" + i, Method.Post, string.Empty, false);
                catalog.ThreadCatalog = new ThreadCatalog();
                catalog.ThreadCatalog.ThreadCatalogName = name;
                catalog.IsNew = true;


                if (name == string.Empty)
                {
                    msgDisplay.AddError("threadcatalogname", tempI, "分类名称不能为空");
                }
                else
                {
                    if (indexAndNames.ContainsValue(name))
                    {
                        msgDisplay.AddError("threadcatalogname", tempI, "重复的分类名称");
                    }
                    indexAndNames.Add(tempI, name);
                }


                if (sortOrders.Contains(catalog.SortOrder))
                {
                    msgDisplay.AddError("SortOrder", tempI, "重复的排序数字");
                }

                if (catalog.ThreadCatalogID == 0)
                    newCatalogNames.Add(name);

                sortOrders.Add(catalog.SortOrder);

                //if (forumThreadCatalogs.GetValue(catalog.ForumID, catalog.ThreadCatalogID) == null)
                //forumThreadCatalogs.Add(catalog);
                newForumThreadCatalogs.Add(catalog);
                j++;
            }



            if (msgDisplay.HasAnyError())
            {
                //forumThreadCatalogs.AddRange(newForumThreadCatalogs);
                int i = 0;
                foreach (ForumThreadCatalog catalog in newForumThreadCatalogs)
                {
                    catalog.ThreadCatalogID = i;
                    forumThreadCatalogs.Add(catalog);
                    i--;
                }
                return;
            }

            try
            {
                if (indexAndNames.Count == 0)
                {
                    msgDisplay.AddError("您已经启用了主题分类，必须至少添加一个主题分类");
                    return;
                }

                if (Forum.ThreadCatalogStatus != threadCatalogStatus)
                {
                    try
                    {
                        bool success = ForumBO.Instance.UpdateForumThreadCatalogStatus(Forum.ForumID, threadCatalogStatus);
                        if (!success)
                        {
                            CatchError<ErrorInfo>(delegate(ErrorInfo error)
                            {
                                msgDisplay.AddError(error);
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        msgDisplay.AddError(ex.Message);
                        return;
                    }
                }

                ThreadCatalogCollection threadCatalogs = ForumBO.Instance.CreateThreadCatelogs(newCatalogNames);

                //List<ForumThreadCatalog> results = new List<ForumThreadCatalog>();

                //ForumManager.UpdateThreadCatalogs

                ThreadCatalogCollection needUpdateThreadCatalogs = new ThreadCatalogCollection();
                for (int i = 0; i < forumThreadCatalogs.Count; i++)
                {
                    //if (forumThreadCatalogs[i].ThreadCatalogID != 0)
                    //{
                        ThreadCatalog tempThreadCatalog = new ThreadCatalog();
                        tempThreadCatalog.ThreadCatalogName = forumThreadCatalogs[i].ThreadCatalog.ThreadCatalogName;
                        tempThreadCatalog.ThreadCatalogID = forumThreadCatalogs[i].ThreadCatalogID;
                        tempThreadCatalog.LogoUrl = string.Empty;
                        needUpdateThreadCatalogs.Add(tempThreadCatalog);
                    //}
                    //else
                    //{
                    //    foreach (ThreadCatalog threadCatalog in threadCatalogs)
                    //    {
                    //        if (forumThreadCatalogs[i].ThreadCatalog.ThreadCatalogName == threadCatalog.ThreadCatalogName)
                    //        {
                    //            forumThreadCatalogs[i].ThreadCatalogID = threadCatalog.ThreadCatalogID;
                    //            //results.Add(forumThreadCatalogs[i]);
                    //            break;
                    //        }

                    //    }
                    //}

                }

                ForumBO.Instance.UpdateThreadCatalogs(needUpdateThreadCatalogs);
                //ForumManager.UpdateThreadCatalogs(needUpdateThreadCatalogs);

                for (int i = 0; i < newForumThreadCatalogs.Count; i++)
                {
                    foreach (ThreadCatalog threadCatalog in threadCatalogs)
                    {
                        if (newForumThreadCatalogs[i].ThreadCatalog.ThreadCatalogName == threadCatalog.ThreadCatalogName)
                        {
                            newForumThreadCatalogs[i].ThreadCatalogID = threadCatalog.ThreadCatalogID;
                            forumThreadCatalogs.Add(newForumThreadCatalogs[i]);
                            //results.Add(forumThreadCatalogs[i]);
                            break;
                        }

                    }
                }


                //if (ForumManager.AddThreadCatalogToForum(Forum.ForumID, forumThreadCatalogs) == true)
                if (ForumBO.Instance.AddThreadCatalogToForum(Forum.ForumID, forumThreadCatalogs) == true)
                {
                    forumThreadCatalogs = null;
                    forum = null;
                    _Request.Clear(Method.Post);
                }
                else
                {
                    msgDisplay.AddError("添加主题分类失败");
                }
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }

        }

        protected string ToJsString(string text)
        {
            return StringUtil.ToJavaScriptString(text);
        }

        protected string GetName(string name)
        {
            return StringUtil.ClearAngleBracket(name);
        }

        private Forum forum;
        protected Forum Forum
        {
            get
            {
                if (forum == null)
                {
                    int forumID = _Request.Get<int>("forumID", Method.Get, 0);
                    forum = ForumBO.Instance.GetForum(forumID, false);
                }
                return forum;
            }
        }

        //protected Dictionary<int, ThreadCatalog>.ValueCollection ThreadCatalogList
        protected ThreadCatalogCollection ThreadCatalogList
        {
            get
            {
                //return ForumManager.GetAllThreadCatalogs().Values;
                return ForumBO.Instance.GetAllThreadCatalogs();
            }
        }

        private ForumThreadCatalogCollection forumThreadCatalogs;
        protected ForumThreadCatalogCollection ForumThreadCatalogList
        {
            get
            {
                if (forumThreadCatalogs == null)
                {
                    forumThreadCatalogs = ForumBO.Instance.GetThreadCatalogsInForums(Forum.ForumID);//ForumManager.GetThreadCatalogsInForums(Forum.ForumID);
                }
                return forumThreadCatalogs;
            }
        }
    }
}