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

using MaxLabs.bbsMax.DataAccess;


namespace MaxLabs.bbsMax.FileSystem
{
    public abstract class FileDao : DaoBase<FileDao>
    {
        //============ 创建临时文件 CreateTempUploadFile ====================

        /// <summary>
        /// 创建一个临时文件
        /// </summary>
        public abstract int CreateTempUploadFile(int userID, string uploadAction, string searchInfo, StringList customParamList, string filename, string serverFilePath, string md5, long fileSize, string fileID);

        //============ 根据条件获取指定的临时文件 ===========================

        public abstract TempUploadFile GetUserTempUploadFile(int userID, int tempUploadFileID);

        /// <summary>
        /// 根据条件获取指定的临时文件
        /// </summary>
        public abstract TempUploadFileCollection GetUserTempUploadFiles(int userID, string uploadAction, string searchInfo);

        public abstract TempUploadFileCollection GetUserTempUploadFiles(int userID, IEnumerable<int> tempUploadFileIds);

        //============ 用户删除自己的临时文件 ===============================

        /// <summary>
        /// 删除指定的临时文件
        /// </summary>
        public abstract string DeleteUserTempUploadFile(int userID, int tempUploadFileID);

        //============ 将临时文件正式保存 ===================================

        public abstract PhysicalFileFromTemp SaveFile(int userID, int tempUploadFileID);

        /// <summary>
        /// 将临时文件正式保存，并将返回文件系统中的文件
        /// </summary>
        public abstract PhysicalFileFromTempCollection SaveFiles(int userID, IEnumerable<int> allTempUploadFileIds, IEnumerable<int> saveTempUploadFileIds, IEnumerable<int> deleteTempUploadFileIds);

        ////============ 创建文件系统中的文件 CreateFile ======================

        ///// <summary>
        ///// 写入真实文件数据,并返回此真实文件数据的唯一ID
        ///// </summary>
        ///// <param name="serverFileName">存于服务器上的路径</param>
        ///// <param name="md5Code">文件的MD5值</param>
        ///// <param name="fileSize">文件大小</param>
        ///// <returns>返回此真实文件数据的唯一ID</returns>
        //public abstract Guid CreateFile(string serverFileName, string md5Code, long fileSize);

        ///// <summary>
        ///// 批量添加文件
        ///// </summary>
        ///// <param name="files"></param>
        ///// <returns>返回添加成功的文件</returns>
        //public abstract PhysicalFileCollection CreateFiles(PhysicalFileCollection files);

        ////============ 删除文件系统中的文件 DeleteFile ======================

        ///// <summary>
        ///// 删除真实文件数据
        ///// </summary>
        ///// <param name="fileIDs">要删除的真实文件的唯一ID</param>
        ///// <returns>返回要删除的真实文件的路径</returns>
        //public abstract void DeleteFiles(IEnumerable<string> fileIds);

        public abstract void SetFilesDeleted(IEnumerable<int> deletingFileIds);

        public abstract List<DeletingFile> GetDeletingFiles();

        //============ 根据一组文件ID获取文件系统中的文件 ===================

        /// <summary>
        /// 获取一个真实文件
        /// </summary>
        /// <param name="fileID"></param>
        /// <returns></returns>
        public abstract PhysicalFile GetFile(string fileID);

        /// <summary>
        /// 获取一组真实文件
        /// </summary>
        public abstract PhysicalFileCollection GetFiles(IEnumerable<string> fileIds);


        //============ 清理过期的临时文件用 =================================

        /// <summary>
        /// 获取过期的临时文件
        /// </summary>
        public abstract TempUploadFileCollection ClearExperisTempUploadFiles();

        ///// <summary>
        ///// 删除指定ID的临时文件
        ///// </summary>
        ///// <param name="tempUploadFileID"></param>
        ///// <returns></returns>
        //public abstract bool DeleteTempUploadFiles(IEnumerable<int> tempUploadFileID);

    }
}