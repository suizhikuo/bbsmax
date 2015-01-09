//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;
using MaxLabs.bbsMax.Entities;

namespace zzbird.Common.Disk
{
    public class IconManager
    {
        [DllImport("shell32.dll", EntryPoint = "SHGetFileInfo")]
        public static extern int GetFileInfo(string pszPath, int dwFileAttributes, ref FileInfomation psfi, int cbFileInfo, int uFlags);

        ///<summary>  
        ///ͨ��·����ȡСͼ�� 
        ///</summary>  
        ///<param name="path">�ļ����ļ���·��</param>  
        ///<returns>��ȡ��ͼ��</returns>   
        public static Icon GetIcon(string path)
        {
            FileInfomation info = new FileInfomation();

            GetFileInfo(path, 0, ref info, Marshal.SizeOf(info), (int)(GetFileInfoFlags.SHGFI_ICON | GetFileInfoFlags.SHGFI_SMALLICON));
            try
            {
                return Icon.FromHandle(info.hIcon);
            }
            catch
            {
                return null;
            }
        }

        ///<summary>  
        ///ͨ��·����ȡͼ�� 
        ///</summary>  
        ///<param name="path">�ļ����ļ���·��</param>  
        ///<param name="uFlags">uFlags:GetFileInfoFlags.SHGFI_SMALLICONСͼ��;GetFileInfoFlags.SHGFI_LARGEICON��ͼ��</param>
        ///<returns>��ȡ��ͼ��</returns>   
        public static Icon GetIcon(string path,GetFileInfoFlags uFlags)
        {
            FileInfomation info = new FileInfomation();

            GetFileInfo(path, 0, ref info, Marshal.SizeOf(info), (int)(GetFileInfoFlags.SHGFI_ICON | uFlags));
            try
            {
                return Icon.FromHandle(info.hIcon);
            }
            catch
            {
                return null;
            }
        }

        ///<summary>  
        ///ͨ��·����ȡСͼ�� 
        ///</summary>  
        ///<param name="path">�ļ����ļ���·��</param>  
        ///<returns>��ȡ��ͼ��</returns>  
        ///<remarks>
        ///Ч�����ã�����ʹ��GetIcon��������ȡ<see cref="Icon"/>ʵ����ͨ��ToBitmap���õ�<see cref="T:Bitmap"/>����
        ///</remarks>
        public static Bitmap GetBitmap(string path)
        {
            FileInfomation info = new FileInfomation();

            GetFileInfo(path, 0, ref info, Marshal.SizeOf(info), (int)(GetFileInfoFlags.SHGFI_ICON | GetFileInfoFlags.SHGFI_SMALLICON));
            try
            {
                return Bitmap.FromHicon(info.hIcon);
            }
            catch
            {
                return null;
            }
        }

        public enum FileAttributeFlags : int
        {
            FILE_ATTRIBUTE_READONLY = 0x00000001,
            FILE_ATTRIBUTE_HIDDEN = 0x00000002,
            FILE_ATTRIBUTE_SYSTEM = 0x00000004,
            FILE_ATTRIBUTE_DIRECTORY = 0x00000010,
            FILE_ATTRIBUTE_ARCHIVE = 0x00000020,
            FILE_ATTRIBUTE_DEVICE = 0x00000040,
            FILE_ATTRIBUTE_NORMAL = 0x00000080,
            FILE_ATTRIBUTE_TEMPORARY = 0x00000100,
            FILE_ATTRIBUTE_SPARSE_FILE = 0x00000200,
            FILE_ATTRIBUTE_REPARSE_POINT = 0x00000400,
            FILE_ATTRIBUTE_COMPRESSED = 0x00000800,
            FILE_ATTRIBUTE_OFFLINE = 0x00001000,
            FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x00002000,
            FILE_ATTRIBUTE_ENCRYPTED = 0x00004000
        }

        public enum GetFileInfoFlags : int
        {
            SHGFI_ICON = 0x000000100, //get icon 
            SHGFI_DISPLAYNAME = 0x000000200, //get display name 
            SHGFI_TYPENAME = 0x000000400, //get type name 
            SHGFI_ATTRIBUTES = 0x000000800, //get attributes 
            SHGFI_ICONLOCATION = 0x000001000, //get icon location 
            SHGFI_EXETYPE = 0x000002000, //return exe type 
            SHGFI_SYSICONINDEX = 0x000004000, //get system icon index 
            SHGFI_LINKOVERLAY = 0x000008000,  //put a link overlay on icon 
            SHGFI_SELECTED = 0x000010000, //show icon in selected state 
            SHGFI_ATTR_SPECIFIED = 0x000020000, //get only specified attributes 
            SHGFI_LARGEICON = 0x000000000, //get large icon 
            SHGFI_SMALLICON = 0x000000001, //get small icon 
            SHGFI_OPENICON = 0x000000002, //get open icon 
            SHGFI_SHELLICONSIZE = 0x000000004, //get shell size icon 
            SHGFI_PIDL = 0x000000008, //pszPath is a pidl 
            SHGFI_USEFILEATTRIBUTES = 0x000000010, //use passed dwFileAttribute 
            SHGFI_ADDOVERLAYS = 0x000000020, //apply the appropriate overlays 
            SHGFI_OVERLAYINDEX = 0x000000040  //Get the index of the overlay 
        }
    }
}