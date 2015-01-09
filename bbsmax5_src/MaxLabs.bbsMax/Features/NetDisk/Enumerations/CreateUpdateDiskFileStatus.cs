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

namespace MaxLabs.bbsMax.Enums
{
    public enum CreateUpdateDiskFileStatus
    {
        /// <summary>
        /// δ֪���󣬿��ܷ�����ҵ���/���ݲ�֮��
        /// </summary>
        UnknownError = -1,

        /// <summary>
        /// �޸ĳɹ�
        /// </summary>
        Success = 0,

        /// <summary>
        /// ��չ����Ч��
        /// </summary>
        InvalidFileExtension = 1,

        /// <summary>
        /// ����Ҫ����ļ�������
        /// </summary>
        InvalidFileNameLength = 2,

        /// <summary>
        /// �ظ����ļ���
        /// </summary>
        DuplicateFileName = 3,

        /// <summary>
        /// ����Ҫ����ļ���
        /// </summary>
        InvalidFileName=4,

        /// <summary>
        /// �ļ�������Ϊ��
        /// </summary>
        EmptyFileName = 5,

        /// <summary>
        /// �����ļ���С��
        /// </summary>
        SizeOver=101,

        /// <summary>
        /// �����ļ�������С��
        /// </summary>
        SingleSizeOver=102,

        /// <summary>
        /// ��ֹ���ļ�����
        /// </summary>
        Deny=103,
        /// <summary>
        /// ����ļ����������
        /// </summary>
        CreateDiskFileAndImportEmoticon = 104,

        /// <summary>
        /// Ŀ¼������
        /// </summary>
        DirectoryNotExist = 105,
        /// <summary>
        /// �ļ�������
        /// </summary>
        FileNotExist = 106
    }
}