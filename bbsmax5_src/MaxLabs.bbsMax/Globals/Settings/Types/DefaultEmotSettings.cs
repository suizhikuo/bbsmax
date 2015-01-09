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
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Caching;
using System.Threading;


namespace MaxLabs.bbsMax.Settings
{
    public class DefaultEmotSettings:SettingBase
    {
        CacheDependency watcher;
        private bool isInited=false;
        private bool IsWathing { get; set; }
        private string FaceDirectory;

        public DefaultEmotSettings()
        {
            FaceDirectory = Globals.GetPath(SystemDirecotry.Assets_Face);
            if (!Directory.Exists(FaceDirectory))
                Directory.CreateDirectory(FaceDirectory);
            m_groups = new DefaultEmoticonGroupCollection();
            init();
        }

        private DefaultEmoticonGroupCollection m_groups = null;

        public void CreateGroup(int sortorder,string GroupName,MessageDisplay msgdisplay)
        {
            if (GroupName!=null && GroupName.Trim()!=string.Empty)
            {
                GroupName = GroupName.Trim();
                if ( GetEmoticonGroupByName(GroupName) != null)
                {
                    msgdisplay.AddError("groupname", "分组："+GroupName+" 已经存在");
                }
                else
                {
                    if (Regex.IsMatch(GroupName, "[\\/:\\*\\?\"\\<\\>\\|]+"))
                    {
                        msgdisplay.AddError("分组是以文件夹的形式存在。因此，分组名称里不能包含下列字符：/ : * ? \" < > |");
                    }
                    else
                    {

                        bool error = false;
                        try
                        {
                            Directory.CreateDirectory(IOUtil.JoinPath(this.FaceDirectory, GroupName));
                        }
                        catch (Exception ex)
                        {
                            error = true;
                            msgdisplay.AddError("创建表情分组失败， 可能的原因是： 对 " + this.FaceDirectory + " 无写入权限！ 具体错误信息： " + ex.Message);
                        }

                        if (error == false)
                        {
                            init();
                            DefaultEmoticonGroup newGroup = GetEmoticonGroupByName(GroupName);
                            if(newGroup!=null)
                                newGroup.SortOrder = sortorder;
                            SettingManager.SaveSettings(this);
                        }
                    }
                }
            }
            else
            {
                msgdisplay.AddError("groupname", "分组名称不能为空。");
            }
        }

        public void DeleteGroup(int groupID,MessageDisplay msgdisplay)
        {
            DefaultEmoticonGroup group = m_groups.GetValue(groupID);// m_groups[groupName];
            if (group != null)
            {
                bool error = false;

                if (string.IsNullOrEmpty(group.DirectoryName))
                {
                    msgdisplay.AddError("groupname", "不能删除默认分组");
                }
                else
                {
                    //全部文件监控都关闭然后再删除文件夹， 否则会导致重启 ||  事实证明停止监视也会重启
                    EndWach();
                    foreach (DefaultEmoticonGroup tempgroup in m_groups)
                    {
                        tempgroup.EndWach();
                    }

                    try
                    {
                      
                        Directory.Delete(m_groups .GetValue(groupID).FilePath,true);
                    }
                    catch (Exception ex)
                    {
                        group.BeginWach();
                        msgdisplay.AddError("删除表情出错，错误信息：" + ex.Message);
                        error = true;
                    }

                    if (error == false)
                    {
                        SettingManager.SaveSettings(this);
                        init(group.DirectoryName);
                    }
                    else
                    {
                        foreach (DefaultEmoticonGroup tempgroup in m_groups)
                        {
                            tempgroup.BeginWach();
                        }
                        BeginWach();
                    }
                }   
            }
        }

        private void init()
        {
            init(string.Empty);
        }

        public DefaultEmoticonGroup GetEmoticonGroupByName(string groupName)
        {
            foreach (DefaultEmoticonGroup group in this.Groups)
            {
                if (group.GroupName.Equals( groupName, StringComparison.OrdinalIgnoreCase))
                    return group;
            }
            return null;
        }

        public DefaultEmoticonGroup GetEmoticonGroupByID(int groupID)
        {
            foreach (DefaultEmoticonGroup group in this.Groups)
            {
                if (group.GroupID == groupID)
                    return group;
            }
            return null;
        }

        private void init(string excludePath)
        {
            DefaultEmoticonGroupCollection tempGroups = new DefaultEmoticonGroupCollection();

            DirectoryInfo faceRoot = new DirectoryInfo(FaceDirectory);

            tempGroups.Add(new DefaultEmoticonGroup(string.Empty));

            foreach (DirectoryInfo dir in faceRoot.GetDirectories())
            {
                if (!string.IsNullOrEmpty(excludePath) && dir.Name.Equals(excludePath, StringComparison.OrdinalIgnoreCase)) continue;
                    tempGroups.Add(new DefaultEmoticonGroup(dir.Name));
            }

            foreach (DefaultEmoticonGroup group in m_groups)
            {
                if (group.IsWatching)
                    group.EndWach();
            }

            UniteEmotGroup(tempGroups, false);

            //sortedGroups = null;

            if (isInited == false)
            {
                for (int i = 0; i < m_groups.Count; i++)
                {
                    m_groups[i].SortOrder = (i+1) * 10;
                }
            }


            //开始文件监控
            foreach (DefaultEmoticonGroup group in m_groups)
            {
                if (!group.IsWatching)
                {
                    group.BeginWach();
                }
            }

            BeginWach();
            isInited = true;
        }

        private void BeginWach()
        {
            if (watcher != null)
                watcher.Dispose();
            watcher = new CacheDependency(FaceDirectory);
            IsWathing = true;
        }

        private void EndWach()
        {
            if (watcher != null)
                watcher.Dispose();
            IsWathing = false;
        }

        /// <summary>
        /// 合并表情分组
        /// </summary>
        /// <param name="groups">新分组</param>
        /// <param name="useOriginal">以原有分组为准 （如果是文件变动引起的合并， 必须以新的为准）</param>
        private void UniteEmotGroup(DefaultEmoticonGroupCollection groups, bool useOriginal)
        {
            if (groups != null)
            {
                List<int> notExistGroups = new List<int>();
                bool isExist = false; ;
                int i=0;

                //数据库里的和实际文件夹下的表情合并
                foreach (DefaultEmoticonGroup tempGroup in groups)
                {
                   
                    isExist = false;
                    foreach (DefaultEmoticonGroup eg in this.m_groups)
                    {
                        if (eg.DirectoryName == tempGroup.DirectoryName)
                        {
                            if (useOriginal)
                            {
                                eg.GroupName    = tempGroup.GroupName;
                                eg.SortOrder    = tempGroup.SortOrder;
                                eg.Disabled     = tempGroup.Disabled;
                                eg.UniteEmotincos(tempGroup.Emoticons);

                            }
                            else
                            {
                                tempGroup.Disabled  = eg.Disabled;
                                tempGroup.GroupName = eg.GroupName;
                                tempGroup.SortOrder =eg.SortOrder;
                                tempGroup.UniteEmotincos(eg.Emoticons);
                            }
                            isExist = true;
                            break;
                        }

                        if (!useOriginal)
                            eg.Dispose();
                    }
                    if(useOriginal)
                        tempGroup.Dispose();


                    if (!isExist)
                    {
                        notExistGroups.Add(i);
                    }
                    i++;
                }

                if (!useOriginal)
                {
                    DefaultEmoticonGroupCollection tempEmotGroup = new DefaultEmoticonGroupCollection();

                    foreach (DefaultEmoticonGroup egroup in groups)
                    {
                        tempEmotGroup.Add(egroup);
                    }

                    m_groups = tempEmotGroup;
                }
                else
                {
                    
                }
            }
        }


        public DefaultEmoticonGroupCollection AvailableGroups
        {
            get
            {
                int index = 0;
                DefaultEmoticonGroupCollection m_availblegroups = new DefaultEmoticonGroupCollection();
                foreach (DefaultEmoticonGroup group in m_groups)
                {

                    if (group.Disabled)
                        continue;

                    index = 0;
                    for (int i = 0; i < m_availblegroups.Count; i++)
                    {
                        if (m_availblegroups[i].SortOrder > group.SortOrder)
                            break;
                        index++;
                    }
                    m_availblegroups.Insert(index, group);
                }

                return m_availblegroups;
            }
        }

        [SettingItem]
        public DefaultEmoticonGroupCollection Groups
        {
            get
            {
                if (IsWathing && watcher.HasChanged)
                {
                    init();
                }

                int index=0;
                DefaultEmoticonGroupCollection sortedGroups = new DefaultEmoticonGroupCollection();
                foreach (DefaultEmoticonGroup group in m_groups)
                {
                    index=0;
                    for (int i = 0; i < sortedGroups.Count; i++)
                    {   
                      
                        if (sortedGroups[i].SortOrder>group.SortOrder )
                            break;
                        index ++;
                    }
                    sortedGroups.Insert(index,group);
                }
                return sortedGroups;
            }
        }

        public override void SetPropertyValue(System.Reflection.PropertyInfo property, string value, bool isParse)
        {
            if (property.PropertyType == typeof(DefaultEmoticonGroupCollection))
            {
                DefaultEmoticonGroupCollection groups = new DefaultEmoticonGroupCollection();
                groups.SetValue(value);

                this.UniteEmotGroup(groups, true);
            }
        }
    }
}