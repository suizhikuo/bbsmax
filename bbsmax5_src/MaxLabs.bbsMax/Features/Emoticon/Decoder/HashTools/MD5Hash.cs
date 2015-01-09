//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

/*
  VisualHash - A Visual Hash Calculator
  Copyright (C) 2005 Dominik Reichl <oss@dominik-reichl.de>

  This program is free software; you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation; either version 2 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program; if not, write to the Free Software
  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

using System;
using System.Security.Cryptography;

namespace MaxLabs.bbsMax.Common.HashTools
{
	public class MD5Hash : HashModule
	{
		protected MD5CryptoServiceProvider m_md5 = new MD5CryptoServiceProvider();

		public MD5Hash()
		{
		}

		public override bool InitModule()
		{
            strName = "MD5";
            strShortName = "MD5";
            strRef = "RFC 1321";
            uHashSize = 128;
            uInternalBufferSize = 512;

            // pbFinalHash = new byte[16];
            pbFinalHash = null;

            return true;
		}

		public override void ReleaseModule()
		{
			m_md5.Clear();

			SecureZeroArray(pbFinalHash);
			pbFinalHash = null;
		}

		public override void InitNewHash()
		{
			m_md5.Initialize();
		}

        public override void UpdateHash(byte[] data, int offset, bool bIsLastBlock)
		{

            if (bIsLastBlock) 
                m_md5.TransformFinalBlock(data, 0, offset);
            else m_md5.TransformBlock(data, 0, offset, data, 0);
		}

		public override void FinalizeHash()
		{
            pbFinalHash = m_md5.Hash;
		}

        //public override uint Test()
        //{
        //    byte[] pb;
        //    byte[] r1 = { 0x43, 0x8A, 0xBB, 0xBD, 0x57, 0xF0, 0xAF, 0x59,
        //        0x19, 0xFF, 0x0D, 0xEA, 0xFB, 0x6F, 0x58, 0xD2 };
        //    byte[] r2 = { 0xd4, 0x1d, 0x8c, 0xd9, 0x8f, 0x00, 0xb2, 0x04,
        //        0xe9, 0x80, 0x09, 0x98, 0xec, 0xf8, 0x42, 0x7e };
        //    byte[] r3 = { 0x57, 0xed, 0xf4, 0xa2, 0x2b, 0xe3, 0xc9, 0x55,
        //        0xac, 0x49, 0xda, 0x2e, 0x21, 0x07, 0xb6, 0x7a };
        //    byte[] r4 = { 0x0c, 0xc1, 0x75, 0xb9, 0xc0, 0xf1, 0xb6, 0xa8,
        //        0x31, 0xc3, 0x99, 0xe2, 0x69, 0x77, 0x26, 0x61 };

        //    pb = StringToAsciiEx("abcabcabcabcabcabcabcabcabcabcabcabcabcabcabc");
        //    if(!HashAndCompare(pb, r1)) return 1;

        //    InitNewHash();
        //    UpdateHash(pbFinalHash, 0, true);
        //    FinalizeHash();
        //    if(!CompareByteArraysEx(pbFinalHash, r2)) return 2;

        //    pb = StringToAsciiEx("12345678901234567890123456789012345678901234567890123456789012345678901234567890");
        //    if(!HashAndCompare(pb, r3)) return 3;
			
        //    pb = StringToAsciiEx("a");
        //    if(!HashAndCompare(pb, r4)) return 4;

        //    return 0;
        //}
	}
}