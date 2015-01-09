//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Data;
using System.Collections.Generic;

using MaxLabs.bbsMax.Entities;
using System.IO;
using MaxLabs.bbsMax.FileSystem;

namespace MaxLabs.bbsMax.XCmd
{
	public class DeleteFileCmd : IXCmd
	{

		#region ICmdHandler 成员

		public string Command
		{
			get { return "DeleteFile"; }
		}

		public void Process(DataTable data)
		{
            List<int> deletingFileIds = new List<int>();

            foreach(DataRow row in data.Rows)
            {

                int deletingFileID = (int)row["DeletingFileID"];
                string serverFilePath = (string)row["ServerFilePath"];

                string path = Globals.GetPath(SystemDirecotry.Upload_File, serverFilePath);

                try
                {
                    File.Delete(path);
                    deletingFileIds.Add(deletingFileID);
                }
                catch { }

            }

            FileManager.SetFileDeleted(deletingFileIds);
		}

		#endregion
	}
}