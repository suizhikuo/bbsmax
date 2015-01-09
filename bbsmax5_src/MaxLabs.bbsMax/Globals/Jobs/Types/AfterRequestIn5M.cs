//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using MaxLabs.bbsMax.FileSystem;
using MaxLabs.bbsMax.Common;

namespace MaxLabs.bbsMax.Jobs
{
    /// <summary>
    /// 每5分钟执行一次  的清理过期数据的任务
    /// </summary>
    public class AfterRequestIn5M : JobBase
    {

        public override ExecuteType ExecuteType
        {
            get { return ExecuteType.AfterRequest; }
        }

        public override TimeType TimeType
        {
            get { return TimeType.Interval; }
        }

        public override bool Enable
        {
            get 
            {
                return true;
            }
        }

        public override void Action()
        {

            //清理最后的300个已经被标记为正在删除状态的文件
            try
            {
                FileManager.ClearDeletingFiles();
            }
            catch (Exception ex)
            {
                LogHelper.CreateErrorLog(ex);
            }

#if !Passport

            try
            {
                PostBOV5.Instance.ClearSearchResult();
            }
            catch (Exception ex)
            {
                LogHelper.CreateErrorLog(ex);
            }

#endif
            //DeleteTempFile();

        }

        protected override void SetTime()
        {
            SetIntervalExecuteTime(5 * 60);
        }









        ///// <summary>
        ///// 清理用户上传临时文件的任务，清理已经超过60分钟的临时文件
        ///// 需要修改为通过查询数据库删除
        ///// </summary>
        //private void DeleteTempFile()
        //{

        //    try
        //    {
        //        List<FileInfo> files = GetTempFile();
        //        foreach (FileInfo file in files)
        //        {
        //            try
        //            {
        //                file.Delete();
        //            }
        //            catch
        //            { }
        //        }
        //    }
        //    catch { }
        //}

        //private List<FileInfo> GetTempFile()
        //{
        //    DirectoryInfo dir = new DirectoryInfo(Globals.GetPath(SystemDirecotry.Temp_Upload));

        //    if (dir.Exists)
        //    {
        //        FileInfo[] fileInfos = dir.GetFiles("*", SearchOption.AllDirectories);

        //        DateTime time = DateTimeUtil.Now.AddMinutes(-60);

        //        List<FileInfo> files = new List<FileInfo>();

        //        for (int i = 0; i < fileInfos.Length; i++)
        //        {
        //            if (fileInfos[i].CreationTime < time && fileInfos[i].LastAccessTime < time && fileInfos[i].LastWriteTime < time)
        //            {
        //                files.Add(fileInfos[i]);
        //            }
        //        }

        //        return files;
        //    }
        //    else
        //        return new List<FileInfo>();

        //}
    }

}