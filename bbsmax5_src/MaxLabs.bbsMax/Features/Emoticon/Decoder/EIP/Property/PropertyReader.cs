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
    class PropertyReader
    {
        byte[] Propertys;
        ushort[] s_FaceXML=new ushort[]{0x66,0x61,0x63,0x65,0x2e,0x78,0x6d,0x6c,0};  
        //Face.XML
        ushort[] s_config = new ushort[] { 0x63, 0x6f, 0x6e, 0x66, 0x69, 0x67, 0 };  
        //config
        ushort[] s_RootEntry = new ushort[] { 0x52, 0x6f, 0x6f, 0x74, 0x20, 0x45, 0x6e, 0x74, 0x72, 0x79, 0 }; 
        //Root Entry
        Model_Property p_Config=new Model_Property();
        Model_Property p_FaceXML = new Model_Property();
        Model_Property p_RootEntry = new Model_Property();
        List<Model_Property> p_Image = new List<Model_Property>();
        List<Model_Property> p_ImageFixed = new List<Model_Property>();
        public PropertyReader(byte[] AllPropertyData)
        {
            Propertys = AllPropertyData;
            byte[] PropertyBytes;
            List<byte[]> PropertyList = new List<byte[]>();
            for (int i = 0; i < AllPropertyData.Length; i += Struct_POIFSConstants.PROPERTY_SIZE)
            {
                PropertyBytes = new byte[Struct_POIFSConstants.PROPERTY_SIZE];
                for (int j = 0; j < Struct_POIFSConstants.PROPERTY_SIZE; j++)
                {
                    PropertyBytes[j] = AllPropertyData[i + j];                   
                }
                PropertyList.Add(PropertyBytes);
            }
            //get property
            foreach (byte[] TempByte in PropertyList)
            {
                Model_Property M_Property = new Model_Property();
                M_Property.NameSize = BitConverter.ToUInt16(TempByte, Struct_Property_Offset.NAME_SIZE);
                ushort[] ShortTemp = new ushort[M_Property.NameSize/2];
                int ShortTempIndex = 0;
                for (int i = 0; i < M_Property.NameSize; i += 2)
                {
                    ShortTemp[ShortTempIndex] = BitConverter.ToUInt16(TempByte, i);
                    ShortTempIndex++;
                }
                
                M_Property.Name = ShortTemp;
                M_Property.PropertyType = TempByte[ Struct_Property_Offset.PROPERTY_TYPE];
                M_Property.NodeColor = TempByte[ Struct_Property_Offset.NODE_COLOR];
                M_Property.PreviousProp = BitConverter.ToInt32(TempByte, Struct_Property_Offset.PREVIOUS_PROP);
                M_Property.NextProp = BitConverter.ToInt32(TempByte, Struct_Property_Offset.NEXT_PROP);
                M_Property.ChildProp = BitConverter.ToInt32(TempByte, Struct_Property_Offset.CHILD_PROP);
                M_Property.Seconds1 = BitConverter.ToInt32(TempByte, Struct_Property_Offset.SECONDS_1);
                M_Property.Days1 = BitConverter.ToInt32(TempByte, Struct_Property_Offset.DAYS_1);
                M_Property.Seconds2 = BitConverter.ToInt32(TempByte, Struct_Property_Offset.SECONDS_2);
                M_Property.Days2 = BitConverter.ToInt32(TempByte, Struct_Property_Offset.DAYS_2);
                M_Property.StartBlock = BitConverter.ToInt32(TempByte, Struct_Property_Offset.START_BLOCK);
                M_Property.Size = BitConverter.ToInt32(TempByte, Struct_Property_Offset.SIZE);
                
                if (Contrast.ShortArray(M_Property.Name, s_FaceXML))
                    p_FaceXML = M_Property;
                else if (Contrast.ShortArray(M_Property.Name, s_RootEntry))
                    p_RootEntry = M_Property;
                else if (Contrast.ShortArray(M_Property.Name, s_config))
                    p_Config = M_Property;
                else if (Contrast.IsImage(M_Property))
                    p_Image.Add(M_Property);
                else if (Contrast.IsImageFixed(M_Property))
                    p_ImageFixed.Add(M_Property);
            }

            List<Model_Property> p_Image_temp = new List<Model_Property>();
            int imageCount = p_Image.Count;
            for (int i = 0; i < imageCount; i++)
            {
                for (int j = 0; j < p_Image.Count; j++)
                {
                    if(NameConvertToInt(p_Image[j].Name) == i)
                    {
                        p_Image_temp.Add(p_Image[j]);
                        p_Image.RemoveAt(j);
                        break;
                    }
                }
            }
            p_Image = p_Image_temp;
        }

        int NameConvertToInt(ushort[] name)
        {
            double sum = 0;
            double multiplier = 10;
            double add = 0;
            for (int i = name.Length - 2; i >= 0; i--)
            {
                sum += (name[i] - 0x30) * Math.Pow(multiplier, add);
                add++;
            }
            return Convert.ToInt32(sum);  
        }

        public Model_Property Config
        {
            get{return p_Config;}
        }

        public Model_Property FaceXML
        {
            get { return p_FaceXML; }
        }

        public Model_Property RootEntry
        {
            get { return p_RootEntry; }
        }

        public List<Model_Property> Image
        { 
            get { return p_Image; } 
        }

        public List<Model_Property> ImageFixed
        { 
            get { return p_ImageFixed; } 
        }
    }
}