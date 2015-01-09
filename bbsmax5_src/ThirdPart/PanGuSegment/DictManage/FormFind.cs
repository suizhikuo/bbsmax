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
    public partial class FormFind : Form
    {
        public enum SearchMode
        {
            None = 0,
            ByPos = 1,
            ByLength = 2,
        }

        SearchMode _SearchMode  = SearchMode.None;

        public SearchMode Mode
        {
            get
            {
                return _SearchMode;
            }

        }

        public int POS
        {
            get
            {
                return posCtrl.Pos;
            }
        }

        public int Length
        {
            get
            {
                return (int)numericUpDownLength.Value;
            }
        }

        public FormFind()
        {
            InitializeComponent();
        }

        private void radioButtonByPos_CheckedChanged(object sender, EventArgs e)
        {
            posCtrl.Enabled = true;
            numericUpDownLength.Enabled = false;
        }

        private void radioButtonByLength_CheckedChanged(object sender, EventArgs e)
        {
            posCtrl.Enabled = false;
            numericUpDownLength.Enabled = true;
        }

        private void buttonFind_Click(object sender, EventArgs e)
        {
            if (radioButtonByLength.Checked)
            {
                _SearchMode = SearchMode.ByLength;
            }
            else if (radioButtonByPos.Checked)
            {
                _SearchMode = SearchMode.ByPos;
            }
            else
            {
                _SearchMode = SearchMode.None;
            }

            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            _SearchMode = SearchMode.None;
            Close();
        }
    }
}