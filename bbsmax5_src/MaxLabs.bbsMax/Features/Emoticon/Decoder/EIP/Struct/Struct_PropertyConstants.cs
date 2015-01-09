//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;

namespace MaxLabs.bbsMax.Emoticons
{
    public struct Struct_PropertyConstants
    {
        //�ڵ���ɫ 0 (��) or 1 (��)
        public  int NODE_COLOR;
        //1 (Ŀ¼), 2 (�ļ�), or 5 (�����)
		public  int ROOT_TYPE;
        //��ʼ��
        public int START_BLOCK;
        //
        public int SIZE;
        //����������
        public int PREVIOUS_PROP;
        
        public int NEXT_PROP;
        //����������
        public int CHILD_PROP;
	}
}