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
using System.Xml;

namespace MaxLabs.bbsMax.Emoticons
{
    class XML
    {
        public static List<Model_XML> CreateXML(byte[] data, PropertyReader property)
        {
            byte[] temp = new byte[property.FaceXML.Size + 42];
            byte[] gb2312 = new byte[] { 0x3c, 0x3f, 0x78, 0x6d, 0x6c, 0x20, 0x76, 0x65, 0x72, 0x73, 0x69, 0x6f, 0x6e, 0x3d, 0x22, 0x31, 0x2e, 0x30, 0x22, 0x20, 0x65, 0x6e, 0x63, 0x6f, 0x64, 0x69, 0x6e, 0x67, 0x3d, 0x22, 0x67, 0x62, 0x32, 0x33, 0x31, 0x32, 0x22, 0x20, 0x3f, 0x3e, 0x0d, 0x0a };
            for (int i = 0; i < gb2312.Length; i++)
            {
                temp[i] = gb2312[i];
            }
            int StartBigBlock;
            if (property.FaceXML.Size < Struct_POIFSConstants.BIG_Image_Min)
                StartBigBlock = property.RootEntry.StartBlock;
            else
                StartBigBlock = property.FaceXML.StartBlock;

            for (int i = 42; i < temp.Length; i++)
            {
                temp[i] = data[(1 + StartBigBlock) * Struct_POIFSConstants.BIG_BLOCK_SIZE + i-42];
            }
            //�ո��滻Ϊ��-��
            for (int i = 42; i < temp.Length; i++)
            {
                if (i < temp.Length - 5)
                    if (temp[i] == 0x46)
                        if (temp[i + 1] == 0x49)
                            if (temp[i + 2] == 0x4c)
                                if (temp[i + 3] == 0x45)
                                    if (temp[i + 4] == 0x20)
                                        temp[i + 4] = 0x2d;
            }
            
            //if (!Directory.Exists(EmoticonManager.userRootPath))
            //    Directory.CreateDirectory(EmoticonManager.userRootPath);
            //File.WriteAllBytes(EmoticonManager.userRootPath + "/Face.xml", temp);

            MemoryStream stream = new MemoryStream(temp);
            //Common.CreateFile.Create(_path+"/Face.xml", temp);
            return AnalyseXML(stream);
        }

        /// <summary>
        /// ����XML���������ļ���
        /// </summary>
        static List<Model_XML> AnalyseXML(Stream stream)
        {
            int total = 0;     //XML����
            XmlDocument doc = new XmlDocument();
            //doc.Load(EmoticonManager.userRootPath + "Face.xml");
            doc.Load(stream);
            stream.Close();
            stream.Dispose();
            

            XmlNode defaultNode = doc.SelectSingleNode("//CUSTOMFACE");
            if (defaultNode!=null)
                total += int.Parse(defaultNode.Attributes["count"].Value);

            XmlNodeList GROUPnodeList = doc.SelectNodes("//CUSTOMFACEGROUP");
            if (GROUPnodeList != null)
            {
                foreach (XmlNode childNode in GROUPnodeList)
                    total += int.Parse(childNode.Attributes["count"].Value);
            }

            List<Model_XML> XMLList = new List<Model_XML>(total);
            Model_XML tempModel;

            //Ĭ�Ͻڵ�
            string FolderName = "δ��������";
            //Directory.CreateDirectory(_path + "/" + FolderName);
            if (defaultNode != null)
            {
                foreach (XmlElement childNode in defaultNode)
                {
                    tempModel = new Model_XML();
                    tempModel.File_ORG = childNode.SelectSingleNode("FILE-ORG").InnerText;
                    tempModel.File_FIXED = childNode.SelectSingleNode("FILE-FIXED").InnerText;
                    tempModel.Id = childNode.Attributes["id"].Value;
                    tempModel.Tip = childNode.Attributes["tip"].Value;
                    tempModel.Multiframe = childNode.Attributes["multiframe"].Value;
                    tempModel.Shortcut = childNode.Attributes["shortcut"].Value;
                    tempModel.FileIndex = int.Parse(childNode.Attributes["FileIndex"].Value);
                    tempModel.Folder = FolderName;
                    XMLList.Add(tempModel);
                }
            }

            if (GROUPnodeList != null)
            {
                //��ڵ�
                foreach (XmlNode node in GROUPnodeList)
                {
                    string GroupFolderName = ((XmlElement)node).Attributes["name"].Value;
                    //�õ���ǰ�ڵ��µ������ӽ��
                    //Directory.CreateDirectory(_path + "/" + GroupFolderName);
                    XmlNodeList childNodeList = node.ChildNodes;
                    foreach (XmlNode childNode in childNodeList)
                    {
                        tempModel = new Model_XML();
                        tempModel.File_ORG = childNode.SelectSingleNode("FILE-ORG").InnerText;
                        tempModel.File_FIXED = childNode.SelectSingleNode("FILE-FIXED").InnerText;
                        tempModel.Id = ((XmlElement)childNode).Attributes["id"].Value;
                        tempModel.Tip = ((XmlElement)childNode).Attributes["tip"].Value;
                        tempModel.Multiframe = ((XmlElement)childNode).Attributes["multiframe"].Value;
                        tempModel.FileIndex = int.Parse(((XmlElement)childNode).Attributes["FileIndex"].Value);
                        tempModel.Folder = GroupFolderName;
                        XMLList.Add(tempModel);
                    }
                }
            }
            //File.Delete(_path + "/Face.xml");
            //File.Delete(EmoticonManager.userRootPath + "Face.xml");
            return XMLList;
        }
    }
}