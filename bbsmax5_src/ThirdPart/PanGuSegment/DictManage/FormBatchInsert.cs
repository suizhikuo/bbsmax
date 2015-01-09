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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PanGu;

namespace DictManage
{
    public partial class FormBatchInsert : Form
    {
        WordAttribute m_Word = new WordAttribute();
        bool m_Ok;

        public WordAttribute Word
        {
            get
            {
                return m_Word;
            }

            set
            {
                m_Word = value;
            }
        }

        public bool AllUse
        {
            get
            {
                return checkBoxAllUse.Checked;
            }
        }

        public FormBatchInsert()
        {
            InitializeComponent();
        }

        new public DialogResult ShowDialog()
        {
            m_Ok = false;
            textBoxWord.Text = m_Word.Word;
            numericUpDownFrequency.Value = (decimal)m_Word.Frequency;
            posCtrl.Pos = (int)m_Word.Pos;

            base.ShowDialog();

            if (m_Ok)
            {
                return DialogResult.OK;
            }
            else
            {
                return DialogResult.Cancel;
            }
        }


        private void buttonOk_Click(object sender, EventArgs e)
        {
            m_Ok = true;
            m_Word.Frequency = (int)numericUpDownFrequency.Value;
            m_Word.Pos = (POS)posCtrl.Pos;

            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}