//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using PanGu;

namespace PosDisplayCtrl
{
    public partial class PosCtrl : UserControl
    {
        const int POS_PER_LINE = 4;
        const int POS_TOP = 0;
        const int POS_LEFT = 0;
        const int POS_WIDTH = 120;
        const int POS_HIGHT = 30;

        Hashtable m_PosTable = new Hashtable();
        int m_Pos = 0;

        public int Pos
        {
            get
            {
                RefreshPos();
                return m_Pos;
            }

            set
            {
                m_Pos = value;
                Display();
            }
        }

        static public String GetChsPos(POS pos)
        {
            switch (pos)
            {
                case POS.POS_D_A:	//	���ݴ� ������
                    return "���ݴ� ������";

                case POS.POS_D_B:	//	����� ��������
                    return "����� ��������";

                case POS.POS_D_C:	//	���� ������
                    return "���� ������";

                case POS.POS_D_D:	//	���� ������
                    return "���� ������";

                case POS.POS_D_E:	//	̾�� ̾����
                    return "̾�� ̾����";

                case POS.POS_D_F:	//	��λ�� ��λ����
                    return "��λ�� ��λ����";

                case POS.POS_D_I:	//	����
                    return "����";

                case POS.POS_D_L:	//	ϰ��
                    return "ϰ��";

                case POS.POS_A_M:	//	���� ������
                    return "���� ������";

                case POS.POS_D_MQ:   //	������
                    return "������";

                case POS.POS_D_N:	//	���� ������
                    return "���� ������";

                case POS.POS_D_O:	//	������
                    return "������";

                case POS.POS_D_P:	//	���
                    return "���";

                case POS.POS_A_Q:	//	���� ������
                    return "���� ������";

                case POS.POS_D_R:	//	���� ������
                    return "���� ������";

                case POS.POS_D_S:	//	������
                    return "������";

                case POS.POS_D_T:	//	ʱ���
                    return "ʱ���";

                case POS.POS_D_U:	//	���� ������
                    return "���� ������";

                case POS.POS_D_V:	//	���� ������
                    return "���� ������";

                case POS.POS_D_W:	//	������
                    return "������";

                case POS.POS_D_X:	//	��������
                    return "��������";

                case POS.POS_D_Y:	//	������ ��������
                    return "������ ��������";

                case POS.POS_D_Z:	//	״̬��
                    return "״̬��";

                case POS.POS_A_NR://	����
                    return "����";

                case POS.POS_A_NS://	����
                    return "����";

                case POS.POS_A_NT://	��������
                    return "��������";

                case POS.POS_A_NX://	�����ַ�
                    return "�����ַ�";

                case POS.POS_A_NZ://	����ר��
                    return "����ר��";

                case POS.POS_D_H:	//	ǰ�ӳɷ�
                    return "ǰ�ӳɷ�";

                case POS.POS_D_K:	//	��ӳɷ�
                    return "��ӳɷ�";

                case POS.POS_UNK://  δ֪����
                    return "δ֪����";

                default:
                    return "δ֪����";

            }
        }


        private void CreatePosCheckBox()
        {
            int pos = 0x40000000;
            this.Width = POS_PER_LINE * POS_WIDTH;
            this.Height = POS_HIGHT * (32 / POS_PER_LINE + 1);

            int j = POS_TOP;
            for (int i = 0; i < 31; i++)
            {
                if (i % POS_PER_LINE == 0)
                {
                    j += POS_HIGHT;
                }

                if (pos == 1)
                {
                    pos = 0;
                }

                POS tPos = (POS)pos;
                CheckBox checkBoxPos = new CheckBox();
                checkBoxPos.CheckedChanged += new EventHandler(checkBox_CheckedChanged);
                m_PosTable[tPos] = checkBoxPos;
                checkBoxPos.Tag = tPos;
                checkBoxPos.Parent = this;
                checkBoxPos.Name = tPos.ToString();
                checkBoxPos.Text = GetChsPos(tPos);

                checkBoxPos.Top = j;
                checkBoxPos.Width = POS_WIDTH;
                checkBoxPos.Left = POS_LEFT + POS_WIDTH * (i % POS_PER_LINE);
                pos >>= 1;
            }
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {

            CheckBox checkBox = (CheckBox)sender;

            if (checkBox.Checked)
            {
                checkBox.ForeColor = Color.Red;

                CheckBox posCheckBox = (CheckBox)m_PosTable[(POS)0];

                if ((POS)checkBox.Tag == POS.POS_UNK)
                {
                    foreach (object key in m_PosTable.Keys)
                    {
                        posCheckBox = (CheckBox)m_PosTable[key];

                        if ((POS)posCheckBox.Tag == POS.POS_UNK)
                        {
                            continue;
                        }

                        posCheckBox.Checked = false;
                    }
                }
                else
                {
                    posCheckBox.Checked = false;
                }
            }
            else
            {
                checkBox.ForeColor = Color.Black;
            }

        }

        private void RefreshPos()
        {
            CheckBox posCheckBox;

            posCheckBox = (CheckBox)m_PosTable[(POS)0];
            m_Pos = 0;

            int pos = 0x40000000;

            for (int i = 0; i < 30; i++)
            {
                POS tPos = (POS)pos;
                posCheckBox = (CheckBox)m_PosTable[tPos];

                if (posCheckBox.Checked)
                {
                    m_Pos |= pos;
                }

                pos >>= 1;
            }
        }

        private void Display()
        {
            CheckBox posCheckBox;

            posCheckBox = (CheckBox)m_PosTable[(POS)0];

            if (m_Pos == 0)
            {
                foreach(object key in m_PosTable.Keys)
                {
                    ((CheckBox)m_PosTable[key]).Checked = false;
                }

                posCheckBox = (CheckBox)m_PosTable[(POS)0];
                posCheckBox.Checked = true;
                return;
            }
            else
            {
                posCheckBox.Checked = false;
            }

            int pos = 0x40000000;

            for (int i = 0; i < 30; i++)
            {
                POS tPos = (POS)pos;
                posCheckBox = (CheckBox)m_PosTable[tPos];

                if ((m_Pos & pos) != 0)
                {
                    posCheckBox.Checked = true;
                }
                else
                {
                    posCheckBox.Checked = false;
                }

                pos >>= 1;
            }
        }

        public PosCtrl()
        {
            InitializeComponent();

            CreatePosCheckBox();

            Display();
        }
    }
}