//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.IO;
using System.Web;

namespace Max.Installs
{
    public class PermissionTester
    {

        private PermissionTester()
        {
        }

        public static string CheckFile(string fileName)
        {
            string filepath = Globals.RootPath() + "\\" + fileName;
            if (!File.Exists(filepath))
            {
                #region �������ļ����Խ����ļ���ɾ��
                
                FileStream fs = null;

                //��鴴���ļ�
                try
                {
                    fs = new FileStream(filepath, FileMode.Create, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine("create file...");
                    fs.Close();
                }
                catch
                {
                    try
                    {
                        if (fs != null)
                            fs.Close();
                    }
                    catch { }
                    return "�޷������ļ�";
                }

                //���༭�ļ�
                try
                {
                    fs = new FileStream(filepath, FileMode.Append, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine("modify file...");
                    fs.Close();
                }
                catch
                {
                    try
                    {
                        if (fs != null)
                            fs.Close();
                    }
                    catch { }
                    return "�޷��༭�ļ�";
                }

                //���ɾ���ļ�
                try
                {
                    File.Delete(filepath);
                }
                catch
                {
                    return "�޷�ɾ���ļ�";
                }
                #endregion

                return string.Empty;
            }

            try
            {
                FileInfo fileInfo = new FileInfo(filepath);

                DateTime lastWriteTime = fileInfo.LastWriteTime;

                fileInfo.IsReadOnly = false;
                fileInfo.LastWriteTime = DateTime.Now;
                fileInfo.LastWriteTime = lastWriteTime;
            }
            catch
            {
                return "�޷�д������";
            }

            return string.Empty;
        }

        public static string CheckDirectory(string directoryName, bool checkSubDirectory)
        {
            bool deleteDirectory = false;
            if (!Directory.Exists(Globals.RootPath() + directoryName))
            {
                try
                {
                    Directory.CreateDirectory(Globals.RootPath() + directoryName);
                }
                catch
                {
                    return "�޷�����Ŀ¼";
                }

                deleteDirectory = true;
            }

            string filepath, directorypath;
            if (directoryName == string.Empty || directoryName == "\\" || directoryName == "/")
            {
                filepath = Globals.RootPath() + "\\max_test.txt";
                directorypath = Globals.RootPath() + "\\max_test";
            }
            else
            {
                filepath = Globals.RootPath() + directoryName + "\\max_test.txt";
                directorypath = Globals.RootPath() + directoryName + "\\max_test";
            }

            FileStream fs = null;

            //��鴴���ļ�
            try
            {
                fs = new FileStream(filepath, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine("create file...");
                fs.Close();
            }
            catch
            {
                try
                {
                    if (fs != null)
                        fs.Close();
                }
                catch { }
                return "�޷������ļ�";
            }

            //���༭�ļ�
            try
            {
                fs = new FileStream(filepath, FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine("modify file...");
                fs.Close();
            }
            catch
            {
                try
                {
                    if (fs != null)
                        fs.Close();
                }
                catch { }
                return "�޷��༭�ļ�";
            }

            //���ɾ���ļ�
            try
            {
                File.Delete(filepath);
            }
            catch
            {
                return "�޷�ɾ���ļ�";
            }

            if (checkSubDirectory)
            {
                //��鴴��Ŀ¼
                try
                {
                    Directory.CreateDirectory(directorypath);
                }
                catch
                {
                    return "�޷�����Ŀ¼";
                }

                //���ɾ��Ŀ¼
                try
                {
                    if (Directory.Exists(directorypath))
                        Directory.Delete(directorypath, true);
                }
                catch
                {
                    return "�޷�ɾ��Ŀ¼";
                }
            }

            if (deleteDirectory && Directory.Exists(Globals.RootPath() + directoryName))
            {
                try
                {
                    Directory.Delete(Globals.RootPath() + directoryName);
                }
                catch
                {
                    return "�޷�ɾ��Ŀ¼";
                }
            }

            return string.Empty;
        }
    }
}