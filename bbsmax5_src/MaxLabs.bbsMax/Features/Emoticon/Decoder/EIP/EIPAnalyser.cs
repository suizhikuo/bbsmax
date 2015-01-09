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
using System.Drawing.Imaging;

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Common.HashTools;

/*
 �����޸��� 6-8 16��26��00
 ������ϣ� �볷������֮ǰ�İ汾
 */

namespace MaxLabs.bbsMax.Emoticons
{
    class EIPAnalyser:IAnalyser
    {
        Dictionary<string, List<EmoticonItem>> emoticons = new Dictionary<string, List<EmoticonItem>>();
        byte[] dataHeader = new byte[Struct_POIFSConstants.BIG_BLOCK_SIZE];
        List<int> bat;                  // Bat:�������������
        List<int> propertyIndex;        //���Կ�������
        List<int> sbatIndex;            //С���ݿ�������
        List<string> allChainList = new List<string>();    //ȫ������
        List<int> rootChainList = new List<int>();         //����

        List<Struct_PropertyConstants> propertyList = new List<Struct_PropertyConstants>();     //���Կ鼯��
        Header header;
        PropertyReader propertyReader;
        List<Model_XML> xmlList;

        void Init(byte[] datas)
        {
            //ͷ�����
            header = new Header(GetBytes.GetHeaderBigBlocks(datas));
            //�����������
            bat = BATBlock.GetAllBatBlock(datas, header.Bat_array, header.Bat_count);
            //����  
            propertyIndex = GetIndexFromBAT.GetIndex(bat, header.Property_start);
            //С��
            sbatIndex = GetIndexFromBAT.GetIndex(bat, header.Sbat_start);
            //���Ա�����
            byte[] temp = GetBytes.GetBigBlocksFromIndexs(datas, propertyIndex);
            //���Ա����
            propertyReader = new PropertyReader(temp);
            //XML���ɽ������������ļ���
            xmlList = XML.CreateXML(datas, propertyReader);
            //�������д����
            allChainList = ParseBAT.Parse(bat, propertyReader.RootEntry.StartBlock, out rootChainList);

        }

        List<EmoticonItem> Items = new List<EmoticonItem>();

        public EIPAnalyser(string FileName)
            :this(File.ReadAllBytes(FileName))
        { 
            
        }

        public EIPAnalyser(byte[] fileData)
        {
            AnalyseFromEIP(fileData);
        }

        public EIPAnalyser(Stream stream)
        {
            byte[] datas;
            using (stream)
            {
                stream.Seek(0, SeekOrigin.Begin);
                datas = new byte[stream.Length];
                stream.Read(datas, 0, datas.Length);
                stream.Close();
            }
            AnalyseFromEIP(datas);

        }

        private  void AnalyseFromEIP(byte[] data)
        {
            Init(data);

            AnalyseFromEIP(data,    "");
        }

        private  void AnalyseFromEIP(byte[] data, string appointGroupName)
        {
            Init(data);

            int r = 0;
            foreach (Model_Property property in propertyReader.Image)
            {
                if (property.Size == 0)
                    continue;
                if (property.Size >= Struct_POIFSConstants.BIG_Image_Min)
                {
                    byte[] tempArray = new byte[property.Size];
                    int index = (property.StartBlock + 1) * Struct_POIFSConstants.BIG_BLOCK_SIZE;
                    for (int i = 0; i < property.Size; i++)
                    {
                        tempArray[i] = data[index + i];
                    }
                    CheckAndCreateImage(tempArray, xmlList[r].File_ORG, xmlList[r].Folder, xmlList[r].Shortcut, appointGroupName);
                }
                else
                {
                    int property_START_BLOCK = property.StartBlock;
                    int property_Size = property.Size;

                    //ȷ����ʼ����
                    int BigBlockIndex = property_START_BLOCK / 8;

                    //smalldataindex:��ʼ����
                    int smalldataindex = ((rootChainList[BigBlockIndex]) + 1) * Struct_POIFSConstants.BIG_BLOCK_SIZE + (property_START_BLOCK % 8) * Struct_POIFSConstants.SAMll_BLOCK_SIZE;
                    //smalldataBigBlockIndex:��鿪ʼ����
                    int smalldataBigBlockIndex = rootChainList[BigBlockIndex + 1];
                    //SpareSmallBlockNum:��ʼ�Ĵ����벹���С����
                    int SpareSmallBlockNum = 8 - property_START_BLOCK % 8;
                    //smalldata:����Сͼ����ֽ�����
                    byte[] smalldata = new byte[property.Size];
                    //smalldataBytesindex:smalldata������
                    int smalldataBytesindex = 0;
                    // ����ڿ���
                    //�����޸ĵĵط�    ��������ж����޸Ĺ��ķ�ֹ�ļ���С С��һ����� 9MEIP���и��ֽڵ�ͼƬ���������
                    if (property.Size > Struct_POIFSConstants.BIG_BLOCK_SIZE)
                    {
                        for (int i = 0; i < SpareSmallBlockNum * Struct_POIFSConstants.SAMll_BLOCK_SIZE; i++)
                        {
                            smalldata[smalldataBytesindex] = data[smalldataindex + i];
                            smalldataBytesindex++;
                        }
                    }

                    //�����1�����ʣ����ֽ���
                    int Spareproperty_Size = property_Size - SpareSmallBlockNum * Struct_POIFSConstants.SAMll_BLOCK_SIZE;
                    //ʣ��Ĵ������
                    int SpareBigBlockNum = Spareproperty_Size / Struct_POIFSConstants.BIG_BLOCK_SIZE;
                    //���ʣ����ֽ���
                    int Spareproperty2_Size = Spareproperty_Size - SpareBigBlockNum * Struct_POIFSConstants.BIG_BLOCK_SIZE;

                    for (int i = 0; i < SpareBigBlockNum; i++)
                    {
                        for (int j = 0; j < Struct_POIFSConstants.BIG_BLOCK_SIZE; j++)
                        {
                            smalldata[smalldataBytesindex] = data[(rootChainList[BigBlockIndex + 1 + i] + 1) * 512 + j];
                            smalldataBytesindex++;
                        }
                    }

                    for (int i = 0; i < Spareproperty2_Size; i++)
                    {
                        smalldata[smalldataBytesindex] = data[(rootChainList[BigBlockIndex + 1 + SpareBigBlockNum] + 1) * Struct_POIFSConstants.BIG_BLOCK_SIZE + i];
                        smalldataBytesindex++;
                    }

                    //����ͼƬ
                    CheckAndCreateImage(smalldata, xmlList[r].File_ORG, xmlList[r].Folder, xmlList[r].Shortcut, appointGroupName);
                }
                r++;
            }
         //   EmoticonManager.CreateEmoticons(userID, emoticons);
        }

        private void CheckAndCreateImage(byte[] data, string filename, string groupname, string shortcut, string appointGroupName)
        {
            if (appointGroupName == "Ĭ�Ϸ���")
                appointGroupName = "{default}";
            EmoticonItem emoticon;

            //string postfix = Check.GetFileNamePostfix(filename);
            //bool tempBool = false;
            ////string newFileName;
            //if (postfix == "GIF" || postfix == "JPG")
            //{
            //    if (Check.CheckImage(data))
            //        tempBool = true;
            //}
            //if (tempBool)
            
                string MD5FileName = MD5.GetMD5(data);

                if (appointGroupName != "")
                    groupname = appointGroupName;

                emoticon = new EmoticonItem();
                emoticon.MD5 = MD5.GetMD5(data);
                emoticon.FileName = filename;
                emoticon.Shortcut = shortcut;
                emoticon.Data = data;

                if (!emoticons.ContainsKey(groupname))
                    emoticons.Add(groupname, new List<EmoticonItem>());

                emoticons[groupname].Add(emoticon);

                this.EmoticonCount++;
                this.TotalEmoticonSize += data.Length;
                
                if (this.MaxEmoticonSize < data.Length) this.MaxEmoticonSize = data.Length;

               // string imagePath = imageRootPath + "\\" + newFileName[0] + "\\" + newFileName[1] + "\\" + newFileName;
               // CreateFile.Create(imagePath, data);

                //��������ͼ
                //ImageHelper.CreateThunmbnailImage(imagePath, EmoticonManager.GetThunmbnailFilePath(newFileName), 24, 24, ImageFormat.Png, true);
                //CreateFile.CreateThunmbnailImage(imagePath, EmoticonManager.ThunmbnailImageRootPath + emoticon.RelativePath,true);
        }

        #region IAnalyser ��Ա

        public int EmoticonCount
        {
            get;
            private set;
        }

        public int TotalEmoticonSize
        {
            get;
            private set;
        }

        public int MaxEmoticonSize
        {
            get;
            private set;
        }

        public  Dictionary<string, List<EmoticonItem>> GetGroupedEmoticons()
        {
            return this.emoticons;
        }

        #endregion
    }
}