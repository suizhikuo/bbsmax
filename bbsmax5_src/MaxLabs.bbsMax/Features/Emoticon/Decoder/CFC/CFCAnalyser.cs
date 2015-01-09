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
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax;


/*
 �����޸��� 6-8 16��26��00
 ������ϣ� �볷������֮ǰ�İ汾
 */

namespace MaxLabs.bbsMax.Emoticons
{
    public class CFCAnalyser:IAnalyser
    {
        byte[] data;

        Dictionary<string, List<EmoticonItem>> emoticons = new Dictionary<string, List<EmoticonItem>>();
        List<EmoticonItem> Items;

        byte[] MD5Byte = new byte[32];
        byte[] cut, FaceName, FaceFileName, file, ThumbnailFileName, ThumbnailFile;

        public CFCAnalyser( byte[] data)
        {
            this.data = data;
            Run();
        }

        public CFCAnalyser( Stream stream)
        {
            int streamLenght = Convert.ToInt32(stream.Length);
            data = new byte[streamLenght];
            stream.Read(data, 0, streamLenght);
            stream.Close();
            Run();
        }

        public CFCAnalyser(string Filename)
            :this(File.ReadAllBytes(Filename))
        {

        }

        void Run()//List<Emoticon>
        {
            int MD5Length;                  //md5���ַ�����ʽ����
            int uintcutLength;              //��ݼ�����
            int FaceNameLength;             //�������Ƴ���
            int FaceFileNameLength;         //�����ļ�������
            int FileLength;                 //�����ļ�����
            int ThumbnailFileNameLength;    //΢��ͼ�ļ�������
            int ThumbnailFileLength;        //΢��ͼ�ļ�����
            int FrameLength;                //�����ļ�֡��

            this.emoticons.Add(string.Empty, new List<EmoticonItem>());
            this.Items = emoticons[string.Empty];

            EmoticonItem emoticon;

            int index = 0;
            for (; ; )
            {
                emoticon = new EmoticonItem();
                if (index >= data.Length)
                    break;

                MD5Length = BitConverter.ToInt32(data, index);// GetByte.GetByteToInt(data, index);
                index += 4;
                uintcutLength = BitConverter.ToInt32(data, index);// GetByte.GetByteToInt(data, index);
                index += 4;
                FaceNameLength = BitConverter.ToInt32(data, index);// GetByte.GetByteToInt(data, index);
                index += 4;
                FaceFileNameLength = BitConverter.ToInt32(data, index);// GetByte.GetByteToInt(data, index);
                index += 4;
                FileLength = BitConverter.ToInt32(data, index);// GetByte.GetByteToInt(data, index);
                index += 4;
                ThumbnailFileNameLength = BitConverter.ToInt32(data, index);// GetByte.GetByteToInt(data, index);
                index += 4;
                ThumbnailFileLength = BitConverter.ToInt32(data, index);// GetByte.GetByteToInt(data, index);
                index += 4;
                FrameLength = BitConverter.ToInt32(data, index);// GetByte.GetByteToInt(data, index);
                index += 4;

                MD5Byte = GetByteArray(data, index, MD5Length); // 32);
                index += MD5Byte.Length;

                cut = new byte[uintcutLength];
                cut = GetByteArray(data, index, uintcutLength);
                index += cut.Length;

                FaceName = new byte[FaceNameLength];
                FaceName = GetByteArray(data, index, FaceNameLength);
                index += FaceName.Length;

                FaceFileName = new byte[FaceFileNameLength];
                FaceFileName = GetByteArray(data, index, FaceFileNameLength);
                index += FaceFileName.Length;

                ThumbnailFileName = new byte[ThumbnailFileNameLength];
                ThumbnailFileName = GetByteArray(data, index, ThumbnailFileNameLength);
                index += ThumbnailFileName.Length;

                file = new byte[FileLength];
                file = GetByteArray(data, index, FileLength);
                index += file.Length;

                ThumbnailFile = new byte[ThumbnailFileLength];
                ThumbnailFile = GetByteArray(data, index, ThumbnailFileLength);
                index += ThumbnailFile.Length;
                string tempFileName = Encoding.Unicode.GetString(FaceFileName);//  Encoding.UTF8.GetString(FaceFileName); //Unicode not is UTF8

                this.TotalEmoticonSize += file.Length;

                if (file.Length > this.MaxEmoticonSize)
                    this.MaxEmoticonSize = file.Length;
                emoticon.MD5 = MD5.GetMD5(file) ;
                emoticon.FileName =  tempFileName;
                emoticon.Shortcut = System.Text.Encoding.UTF8.GetString(cut);
                emoticon.Data = file;
                this.EmoticonCount++;
                this.Items.Add(emoticon);
            }
        }

        byte[] GetByteArray(byte[] data,int index,int length)
        {
            byte[] temp = new byte[length];
            for (int i = 0; i < length; i++)
            {
                temp[i]=data[index + i];
            }
            return temp;
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