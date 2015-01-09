//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DictManage
{
    public partial class FormEncoder : Form
    {
        bool m_Ok;

        public FormEncoder()
        {
            InitializeComponent();
        }

        public String Encoding
        {
            get
            {
                return comboBoxEncoder.Text;
            }
        }

        new public DialogResult ShowDialog()
        {
            m_Ok = false;
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
            Close();
        }
    }
}