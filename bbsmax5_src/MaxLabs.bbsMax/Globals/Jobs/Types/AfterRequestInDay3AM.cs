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
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.FileSystem;
using MaxLabs.bbsMax.Common;
using System.IO;

namespace MaxLabs.bbsMax.Jobs
{
    /// <summary>
    /// 每天3点执行 
    /// </summary>
    public class AfterRequestInDay3AM:JobBase
    {

        public override ExecuteType ExecuteType
        {
            get { return ExecuteType.AfterRequest; }
        }

        public override TimeType TimeType
        {
            get { return TimeType.Day; }
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
            //清理用户相关的过期数据（如过期的用户组、过期的点亮图标等）
            try
            {
                UserBO.Instance.ClearExpiresUserData();
            }
            catch (Exception ex)
            {
                LogHelper.CreateErrorLog(ex);
            }


            //清除设置中的过期数据
            AllSettings.Current.PointActionSettings.ClearExperiesData();

#if !Passport
            AllSettings.Current.RateSettings.ClearExperiesData();
#endif

            //清理旧的（超过24小时未使用的）临时文件，这些文件可能是残留的垃圾文件
            try
            {
                ClearOldTempFiles(Globals.GetPath(SystemDirecotry.Temp_Upload));
                ClearOldTempFiles(Globals.GetPath(SystemDirecotry.Temp_Avatar));
            }
            catch (Exception ex)
            {
                LogHelper.CreateErrorLog(ex);
            }

            //清理旧的错误日志文件
            try
            {
                LogHelper.ClearExpiresLogs();
            }
            catch (Exception ex)
            {
                LogHelper.CreateErrorLog(ex);
            }
        }

        protected override void SetTime()
        {
            SetDayExecuteTime(3,0,0);
        }

        /// <summary>
        /// 清理旧的（超过24小时未使用的）临时文件，这些文件可能是残留的垃圾文件
        /// </summary>
        private void ClearOldTempFiles(string path)
        {

            DirectoryInfo dir = new DirectoryInfo(path);

            if (dir.Exists)
            {
                FileInfo[] fileInfos = dir.GetFiles("*", SearchOption.AllDirectories);

                DateTime time = DateTime.UtcNow.AddHours(-24);

                foreach (FileInfo fileInfo in fileInfos)
                {
                    if (fileInfo.CreationTimeUtc < time && fileInfo.LastAccessTimeUtc < time && fileInfo.LastWriteTimeUtc < time)
                    {
                        fileInfo.Delete();
                    }
                }
            }

        }
    }
}