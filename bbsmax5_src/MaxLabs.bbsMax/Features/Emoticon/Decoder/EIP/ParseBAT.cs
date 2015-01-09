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

namespace MaxLabs.bbsMax.Emoticons
{
    class ParseBAT
    {
        public static List<string> Parse(List<int> BAT,int RootIndex,out List<int> RootChain)
        {
            List<int> Readed = new List<int>();
            List<string> Chain = new List<string>();
            string StrTemp;
            int tempInt = 0, indexInt = 0;
            bool IsReaded = false;
            foreach (int value in BAT)
            {
                indexInt++;
                if (value == -3)
                    continue;
                if (value == -1)
                    continue;
                if (value == -2)
                    continue;
                //-4��ʾ��2��ͷ�� ��ǰû�г��ֹ�
                if (value == -4)
                    continue;
                
                IsReaded = false;
                for (int i = 0; i < Readed.Count; i++)
                {
                    if (value==Readed[i])
                    {
                        IsReaded = true;
                        tempInt=i;
                    }
                }
                if (IsReaded)
                    continue;
                StrTemp = "";
                StrTemp = (indexInt-1).ToString() + "-";
                int tempvalue = value;
                for (; ; )
                {
                    StrTemp = StrTemp + tempvalue.ToString() + "-";
                    Readed.Add(tempvalue);
                    tempvalue = BAT[tempvalue];
                    if (tempvalue == -2)
                    {
                        Chain.Add(StrTemp);
                        break;
                    }
                }
            }
            RootChain = new List<int>();
            for (int i = 0; i < Chain.Count; i++)
            {
                string[] temp = Chain[i].Split('-');
                if (temp[0] == RootIndex.ToString())
                {
                    for (int j = 0; j < temp.Length-1;j++ )
                    {
                        RootChain.Add(int.Parse(temp[j]));
                    }
                }
            }
            return Chain;
        }
    }
}