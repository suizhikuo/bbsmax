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
using System.Data;

namespace MaxLabs.bbsMax.XCmd
{
    public class XCmdManager
    {

        private static Dictionary<string, IXCmd> s_XCmds = new Dictionary<string, IXCmd>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 注册一个XCmd命令
        /// </summary>
        /// <param name="xCmd"></param>
        public static void RegisterXCmd(IXCmd xCmd)
        {
            IXCmd cmd;

            if (s_XCmds.TryGetValue(xCmd.Command, out cmd))
                cmd = xCmd;

            else
                s_XCmds.Add(xCmd.Command, xCmd);
        }

        public static void ProcessXCmd(DataTable data)
        {
            if (data.Columns[0].ColumnName == "XCMD" && data.Rows.Count > 0)
            {
                string xCmd = (string)data.Rows[0][0];
                IXCmd cmdHandler;

                if (s_XCmds.TryGetValue(xCmd, out cmdHandler))
                {
                    cmdHandler.Process(data);
                }
            }
        }
    }
}