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
using System.Reflection;
using System.Text.RegularExpressions;

namespace MaxLabs.bbsMax.Entities
{
    public class Exif : FileProperty
    {
        private string bySoftware;   //�������
        public string BySoftware
        {
            get { return bySoftware; }
            set { bySoftware = value; }
        }
        private string datetime;     //����ʱ�� 
        public string DateTime
        {
            get { return datetime; }
            set { datetime = value; }
        }
        private string isoSpeed;     //ISO�ٶ�
        public string IsoSpeed
        {
            get { return isoSpeed; }
            set { isoSpeed = value; }
        }
        private string equipmentMake;   //�豸����
        public string EquipmentMake
        {
            get { return equipmentMake; }
            set { equipmentMake = value; }
        }
        private string exposureTime;//�ع�ʱ��
        public string ExposureTime
        {
            get { return exposureTime; }
            set { exposureTime = value; }
        }
        private string flash;//�����
        public string Flash
        {
            get { return flash; }
            set { flash = value; }
        }
        private string xResolution;        //ˮƽ�ֱ���
        public string XResolution
        {
            get { return xResolution; }
            set { xResolution = value; }
        }
        private string yResolution;        //��ֱ�ֱ���
        public string YResolution
        {
            get { return yResolution; }
            set { yResolution = value; }
        }
        private string imageWidth;//��Ƭ���
        public string ImageWidth
        {
            get { return imageWidth; }
            set { imageWidth = value; }
        }
        private string imageHeight;//��Ƭ�߶�
        public string ImageHeight
        {
            get { return imageHeight; }
            set { imageHeight = value; }
        }
        private string fNumber;//  fֵ����Ȧ��
        public string FNumber
        {
            get { return fNumber; }
            set { fNumber = value; }
        }

        public static string GetShowString(Exif exif)
        {
            string showString = "";
            if (!string.IsNullOrEmpty(exif.bySoftware))
                showString += "�������:" + exif.bySoftware + "<br />";
            if (!string.IsNullOrEmpty(exif.datetime))
                showString += "����ʱ��;" + exif.datetime + "<br />";
            if (!string.IsNullOrEmpty(exif.isoSpeed))
                showString += "iso�ٶ�:" + exif.isoSpeed + "<br />";
            if (!string.IsNullOrEmpty(exif.equipmentMake))
                showString += "���:" + exif.equipmentMake + "<br />";
            if (!string.IsNullOrEmpty(exif.exposureTime))
                showString += "�ع�ʱ��:" + exif.exposureTime + "<br />";
            if (!string.IsNullOrEmpty(exif.flash))
                showString += "�����:" + exif.flash + "<br />";
            if (!string.IsNullOrEmpty(exif.xResolution))
                showString += "ˮƽ�ֱ���:" + exif.xResolution + "<br />";
            if (!string.IsNullOrEmpty(exif.yResolution))
                showString += "��ֱ�ֱ���:" + exif.yResolution + "<br />";
            if (!string.IsNullOrEmpty(exif.yResolution))
                showString += "��Ƭ���:" + exif.imageWidth + "<br />";
            if (!string.IsNullOrEmpty(exif.yResolution))
                showString += "��Ƭ�߶�:" + exif.imageHeight + "<br />";
            if (!string.IsNullOrEmpty(exif.yResolution))
                showString += "��Ȧ��:" + exif.fNumber;
            return showString;
        }

        public override string ToString()
        {
            StringBuilder info = new StringBuilder();
            StringBuilder value = new StringBuilder();
            foreach (PropertyInfo pro in this.GetType().GetProperties())
            {
                if (pro.GetValue(this, null) == null)
                {
                    info.AppendFormat("@{0}:{1}", pro.Name, 0);
                    continue;
                }
                else
                {
                    info.AppendFormat("@{0}:{1}", pro.Name, pro.GetValue(this, null).ToString().Length);
                    value.Append(pro.GetValue(this, null).ToString());
                }
            }
            string top = (info.Length.ToString().Length + info.Length + 3).ToString();
            info.Append("sun");
            info.Append(value);
            return (top + info.ToString()).Replace("|", "<$%s|fghjw>");
        }

        public static Exif Parse(string strFormat)
        {
            Exif exif = new Exif();
            try
            {
                string info = Regex.Match(strFormat, @"\d+(@.+?:\d+)+sun").Value;
                MatchCollection mc = Regex.Matches(info, @"@(.*?):(\d+)");
                string value = strFormat.Replace(info, "");
                int start = 0;
                foreach (Match match in mc)
                {
                    int lenght = int.Parse(match.Groups[2].Value);
                    exif.GetType().GetProperty(match.Groups[1].Value).SetValue(exif, value.Substring(start, lenght), null);
                    start += lenght;
                }
            }
            catch
            {
                return new Exif();
            }
            return exif;
        }

    }

}