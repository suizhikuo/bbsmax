//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;

namespace MaxLabs.bbsMax.Enums
{
    public enum MoveStatus
    {
        /// <summary>
        /// �ظ����ļ���
        /// </summary>
        DuplicateFileName=-1,
        /// <summary>
        /// �ƶ��ɹ�
        /// </summary>
        Success=0,
        /// <summary>
        /// Ŀ¼������
        /// </summary>
        DirectoryIDNotExist = 1,
        /// <summary>
        /// δ֪����
        /// </summary>
        UnKnownError = 2,
        /// <summary>
        /// �����ƶ�����ǰĿ¼
        /// </summary>
        NoMoveSelecteDirectory = 3,
        /// <summary>
        /// �����ƶ�����Ŀ¼
        /// </summary>
        NoMoveChildDirectory = 4
    }

    public enum ExtractStatus
    {
        /// <summary>
        /// ������
        /// </summary>
        NotExists = -1,
        /// <summary>
        /// ��ѹ�ɹ�
        /// </summary>
        Success=0,
        /// <summary>
        /// �����ظ����ļ��л��ļ���
        /// </summary>
        DuplicateFileName=1,
        /// <summary>
        /// δ֪����
        /// </summary>
        UnknownError=101,
        /// <summary>
        /// �ļ������ļ�������ƴ�С
        /// </summary>
        SingleSizeOver=102,
        /// <summary>
        /// ��������Ӳ�̵�����
        /// </summary>
        SizeOver=103,
    }
}