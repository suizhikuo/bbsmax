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
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Util
{
    public class FileProAnalyzer
    {

        public static FileProperty GetExFileProfile(string extentName, Stream stream)
        {
            FileProperty info;
            if (extentName == ".jpg" || extentName == ".jpeg")
            {
                info = FileProAnalyzer.ExifAnalyse(stream);
            }
            else if (extentName == ".rm" || extentName == ".rmvb" || extentName == ".mp3"
                            || extentName == ".wma" || extentName == ".wmv")
            {
                info = FileProAnalyzer.MediaAnalyse(stream);

            }
            else if (extentName == ".torrent")
            {
                info = FileProAnalyzer.TorrentAnalyse(stream);
            }
            else
            {
                info = new FileProperty();
            }

            return info;
        }

        #region ����Ƶ����
        public static Media MediaAnalyse(Stream stream)
        {
            byte[] header16 = new byte[16];
            stream.Read(header16, 0, 16);
            try
            {
                if (ByteManager.CompareByteArray(ByteManager.CopyByteArray(header16, 0, 3), ByteManager.id3v2Tag))  //MP3
                {
                    return Mp3Analyse(stream);
                }
                else if (ByteManager.CompareByteArray(ByteManager.CopyByteArray(header16, 0, 4), ByteManager.rmTag1)
                    || ByteManager.CompareByteArray(ByteManager.CopyByteArray(header16, 0, 4), ByteManager.rmTag2))  //RM or RMVB
                {
                    return RmAnalyse(stream, header16);
                }
                else if (ByteManager.CompareByteArray(header16, ByteManager.wmaHeaderTag))  //Windows Media Player(wma,wmv)
                {
                    return WmaAnalyse(stream);
                }
            }
            catch { }
            return new Media();
        }

        static Media RmAnalyse(Stream stream, byte[] header16)
        {
            Media music = new Media();
            byte[] Id3v1 = new byte[128];
            stream.Seek(stream.Length - 128, SeekOrigin.Begin);
            stream.Read(Id3v1, 0, 128);

            stream.Seek(0, SeekOrigin.Begin);
            if (!ByteManager.CompareByteArray(ByteManager.id3v1Tag, ByteManager.CopyByteArray(Id3v1, 0, 3)))
                return AnalyseRmID3V2(stream, header16);
            music.Title = Encoding.Default.GetString(ByteManager.CopyByteArray(Id3v1, 3, 30)).Replace("\0", "");
            music.Artist = Encoding.Default.GetString(ByteManager.CopyByteArray(Id3v1, 33, 30)).Replace("\0", "");
            music.DiscTitle = Encoding.Default.GetString(ByteManager.CopyByteArray(Id3v1, 63, 30)).Replace("\0", "");
            music.PublishYear = Encoding.Default.GetString(ByteManager.CopyByteArray(Id3v1, 93, 4)).Replace("\0", "");
            music.Remark = Encoding.Default.GetString(ByteManager.CopyByteArray(Id3v1, +97, 30)).Replace("\0", "");

            return music;
        }

        static Media AnalyseRmID3V2(Stream stream, byte[] header)
        {
            Media music = new Media();
            byte[] field = new byte[4];
            uint lenght = ByteManager.ByteCovertToUint(header, 4);
            stream.Seek(lenght, SeekOrigin.Begin);
            for (; ; )
            {
                stream.Read(field, 0, 4);
                uint offset = Convert.ToUInt32((stream.ReadByte() << 24) + (stream.ReadByte() << 16) + (stream.ReadByte() << 8) + stream.ReadByte());
                if (ByteManager.CompareByteArray(ByteManager.rm_field_CONT, field))
                {
                    stream.Seek(2, SeekOrigin.Current);
                    byte[] contInfo = new byte[offset - 10];
                    stream.Read(contInfo, 0, contInfo.Length);
                    music = AnalyseCONT(contInfo);
                    break;
                }
                else
                    stream.Read(new byte[offset - 8], 0, Convert.ToInt32(offset - 8));
            }
            return music;
        }

        static Media AnalyseCONT(byte[] contInfo)
        {
            Media music = new Media();

            int startIndex = 0;

            int titleLenght = contInfo[startIndex + 1];
            music.Title = Encoding.Default.GetString(ByteManager.CopyByteArray(contInfo, startIndex + 2, titleLenght));
            startIndex += titleLenght + 2;

            int artistLenght = contInfo[startIndex + 1];
            music.Artist = Encoding.Default.GetString(ByteManager.CopyByteArray(contInfo, startIndex + 2, artistLenght));
            startIndex += artistLenght + 2;

            int copyrightLenght = contInfo[startIndex + 1];
            music.Copyright = Encoding.Default.GetString(ByteManager.CopyByteArray(contInfo, startIndex + 2, copyrightLenght));
            startIndex += copyrightLenght + 2;

            return music;
        }

        static Media Mp3Analyse(Stream stream)
        {
            Media music = new Media();
            //��ȡ���128�ֽ�
            byte[] Id3v1 = new byte[128];
            stream.Seek(stream.Length - 128, SeekOrigin.Begin);
            stream.Read(Id3v1, 0, 128);

            stream.Seek(0, SeekOrigin.Begin);
            if (!ByteManager.CompareByteArray(ByteManager.id3v1Tag, ByteManager.CopyByteArray(Id3v1, 0, 3)))
                return music;
            music.Title = Encoding.Default.GetString(ByteManager.CopyByteArray(Id3v1, 3, 30)).Replace("\0", "");
            music.Artist = Encoding.Default.GetString(ByteManager.CopyByteArray(Id3v1, 33, 30)).Replace("\0", "");
            music.DiscTitle = Encoding.Default.GetString(ByteManager.CopyByteArray(Id3v1, 63, 30)).Replace("\0", "");
            music.PublishYear = Encoding.Default.GetString(ByteManager.CopyByteArray(Id3v1, 93, 4)).Replace("\0", "");
            music.Remark = Encoding.Default.GetString(ByteManager.CopyByteArray(Id3v1, +97, 30)).Replace("\0","");

            return music;
        }

        static Media WmaAnalyse(Stream stream)
        {
            Media music = new Media();
            try
            {
                StreamIndex(stream, ByteManager.wmaInfoTag);
                for (int i = 0; i < 8; i++)
                    stream.ReadByte();

                int titleLenght = stream.ReadByte() + (stream.ReadByte() << 8);
                int artistLenght = stream.ReadByte() + (stream.ReadByte() << 8);
                int copyrightLenght = stream.ReadByte() + (stream.ReadByte() << 8);
                int remarkLenght = stream.ReadByte() + (stream.ReadByte() << 8);

                byte[] titleBytes = new byte[titleLenght];
                stream.Read(titleBytes, 0, titleLenght);
                music.Title = Encoding.Unicode.GetString(titleBytes);

                byte[] artistBytes = new byte[artistLenght];
                stream.Read(artistBytes, 0, artistLenght);
                music.Artist = Encoding.Unicode.GetString(artistBytes);

                byte[] copyrightBytes = new byte[copyrightLenght];
                stream.Read(copyrightBytes, 0, copyrightLenght);
                music.Copyright = Encoding.Unicode.GetString(copyrightBytes);

                byte[] remarkBytes = new byte[remarkLenght];
                stream.Read(remarkBytes, 0, remarkLenght);
                music.Remark = Encoding.Unicode.GetString(remarkBytes);
            }
            catch { }
            return music;
        }

        static void StreamIndex(Stream stream, byte[] bytes)
        {
            stream.Seek(0, SeekOrigin.Begin);
            int index = 0;
            for (int i = 0; i < stream.Length; i++)
            {
                if (stream.ReadByte() == Convert.ToByte(bytes[index]))
                {
                    if (index == bytes.Length - 1)
                        return;
                    index++;
                }
                else
                    index = 0;
            }
        }
        #endregion

        #region JPEGͼƬ����

        #region ȡ��ͼƬ��EXIF��Ϣ
        public static Exif ExifAnalyse(Stream stream)
        {
            Exif exif = new Exif();
            try
            {
                // ����һ��ͼƬ��ʵ��
                /******************************************************
                 * ���ܳ���
                 * 
                 *System.ArgumentException:
                 * 
                 *        ����û����Ч��ͼ���ʽ- �� -stream Ϊ null��
                 * 
                 * modify:����try{}catch{}��myxbing,2007-8-31
                 * *****************************************************/
                Image MyImage = Image.FromStream(stream);
                // ����һ�������������洢ͼ�������������ID
                int[] MyPropertyIdList = MyImage.PropertyIdList;
                //����һ�����ͼ�����������ʵ��
                PropertyItem[] MyPropertyItemList = new PropertyItem[MyPropertyIdList.Length];

                // ASCII����
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();

                int index = 0;
                int MyPropertyIdListCount = MyPropertyIdList.Length;
                if (MyPropertyIdListCount != 0)
                {
                    foreach (int MyPropertyId in MyPropertyIdList)
                    {
                        MyPropertyItemList[index] = MyImage.GetPropertyItem(MyPropertyId);

                        #region ��ʼ��������ֵ
                        string myPropertyIdString = MyImage.GetPropertyItem(MyPropertyId).Id.ToString("x");
                        switch (myPropertyIdString)
                        {
                            case "131":
                                exif.BySoftware = asciiEncoding.GetString(MyPropertyItemList[index].Value);
                                break;
                            case "132":
                                exif.DateTime = asciiEncoding.GetString(MyPropertyItemList[index].Value);
                                break;
                            case "829a":
                                exif.ExposureTime = GetExposureTimeValue(MyPropertyItemList[index].Value);
                                break;
                            case "829d":
                                exif.FNumber = GetFnumberValue(MyPropertyItemList[index].Value);
                                break;
                            case "9209":       //�����
                                exif.Flash = BoolToEXIFValue(MyPropertyItemList[index].Value);
                                break;
                            case "8827":
                                exif.IsoSpeed = "ISO-" + ShortToEXIFValue(MyPropertyItemList[index].Value);
                                break;
                            case "10f":
                                exif.EquipmentMake = asciiEncoding.GetString(MyPropertyItemList[index].Value);
                                break;
                        }
                        #endregion
                        index++;
                    }
                }

                exif.XResolution = MyImage.HorizontalResolution.ToString() + "DPI";
                exif.YResolution = MyImage.VerticalResolution.ToString() + "DPI";
                exif.ImageHeight = MyImage.Height.ToString();
                exif.ImageWidth = MyImage.Width.ToString();
                MyImage.Dispose();
            }
            catch
            {
            }
            return exif;
        }
        #endregion

        static string BoolToEXIFValue(byte[] data)
        {
            string boolString = "��";
            for (int i = 0; i < data.Length; i++)
            {
                if (i == 0 && data[i] == 1)
                {
                    boolString = "��";
                }
                else if (i != 0 && data[i] != 0)
                {
                    return boolString = "��";
                }
            }
            return boolString;
        }
        static string ShortToEXIFValue(byte[] data)
        {
            uint value = Convert.ToUInt32((data[1] << 8) + data[0]);
            return value.ToString();
        }
        static string GetFnumberValue(byte[] data)
        {
            double value = data[0] / 10.0;
            return "F/" + value.ToString();
        }
        static string GetExposureTimeValue(byte[] data)
        {
            string value = data[4].ToString();
            return "1/" + value + "��";
        }
        #endregion

        #region �£����ӷ���
        public static Torrent TorrentAnalyse(Stream stream)
        {
            try
            {
                Stack<string> stack = new Stack<string>();
                Queue<byte> queue = new Queue<byte>();
                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                queue = new Queue<byte>();
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] == 0x65)
                    {
                        switch (stack.Pop())
                        {
                            case "dictionaries":
                                {
                                    queue.Enqueue(0x7d); //}
                                    break;
                                }
                            case "list":
                                {
                                    queue.Enqueue(0x5d); //]
                                    break;
                                }
                            case "int":
                                {
                                    queue.Enqueue(0x22);//"
                                    break;
                                }
                        }
                    }
                    else if (data[i] == 0x64)   //d
                    {
                        stack.Push("dictionaries");
                        queue.Enqueue(0x7b);
                    }
                    else if (data[i] == 0x6c)
                    {
                        stack.Push("list");
                        queue.Enqueue(0x5b);
                    }
                    else if (data[i] == 0x69)
                    {
                        stack.Push("int");
                        queue.Enqueue(0x22);
                    }
                    else if (data[i] >= 0x30 && data[i] <= 0x39 && stack.Peek() != "int")
                    {
                        i += AnalyzeInt(data, i, ref queue);
                    }
                    else if (stack.Peek() == "int")
                    {
                        queue.Enqueue(data[i]);
                    }
                }

                byte[] changedData = new byte[queue.Count];
                int index = 0;
                while (queue.Count != 0)
                {
                    changedData[index] = queue.Dequeue();
                    index++;
                }
                return ConvertToEntity(Encoding.UTF8.GetString(changedData));
            }
            catch
            { return new Torrent(); }
        }

        static Torrent ConvertToEntity(string analyzerString)
        {
            Torrent torrent = new Torrent();
            torrent.TrackerUrl = Regex.Match(analyzerString, "\"announce\"\"(.*?)\"").Groups[1].Value;
            torrent.CreateBy = Regex.Match(analyzerString, "\"created by\"\"(.*?)\"").Groups[1].Value;

            foreach (Match m in Regex.Matches(analyzerString, "\"length\"\"(.*?)\".*?\"path.utf-8\"[[{]*\"(.*?)\""))
            {
                TorrentIncludeFile fileInfo = new TorrentIncludeFile();
                fileInfo.FileLenght = Convert.ToInt32(m.Groups[1].Value);
                fileInfo.FileName = m.Groups[2].Value;
                torrent.FilesInfo.Add(fileInfo);
            }
            return torrent;
        }

        static int AnalyzeInt(byte[] data, int index, ref Queue<byte> queue)
        {
            int tempInt = 0;
            int tempInt2 = 0;
            for (int i = index; i < data.Length; i++)
            {
                if (data[i] >= 0x30 && data[i] <= 0x39)
                {
                    tempInt *= 10;
                    tempInt += data[i] - 0x30;
                    tempInt2++;
                }
                else if (data[i] == 0x3A)  //:
                {
                    queue.Enqueue(0x22);
                    for (int j = 0; j < tempInt; j++)
                        queue.Enqueue(data[i + j + 1]);
                    queue.Enqueue(0x22);
                    break;
                }
            }

            return tempInt + tempInt2;
        }

        #endregion
    }
}