//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

namespace DictManage
{
    partial class FormMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openBinDictFile13ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveBinDictFile13ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialogDict = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialogDict = new System.Windows.Forms.SaveFileDialog();
            this.textBoxSearch = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.listBoxList = new System.Windows.Forms.ListBox();
            this.contextMenuStripList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonSearch = new System.Windows.Forms.Button();
            this.panelMain = new System.Windows.Forms.Panel();
            this.labelCount = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonBatchInsert = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonUpdate = new System.Windows.Forms.Button();
            this.buttonInsert = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownFrequency = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxWord = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.label = new System.Windows.Forms.ToolStripStatusLabel();
            this.openFileDialogName = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialogText = new System.Windows.Forms.SaveFileDialog();
            this.OpenAsTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveAsTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.posCtrl = new PosDisplayCtrl.PosCtrl();
            this.menuStrip.SuspendLayout();
            this.panel1.SuspendLayout();
            this.contextMenuStripList.SuspendLayout();
            this.panelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrequency)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(792, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openBinDictFile13ToolStripMenuItem,
            this.saveBinDictFile13ToolStripMenuItem,
            this.OpenAsTextToolStripMenuItem,
            this.SaveAsTextToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.fileToolStripMenuItem.Text = "文件(&F)";
            // 
            // openBinDictFile13ToolStripMenuItem
            // 
            this.openBinDictFile13ToolStripMenuItem.Name = "openBinDictFile13ToolStripMenuItem";
            this.openBinDictFile13ToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.openBinDictFile13ToolStripMenuItem.Text = "打开(&O)";
            this.openBinDictFile13ToolStripMenuItem.Click += new System.EventHandler(this.openBinDictFile13ToolStripMenuItem_Click);
            // 
            // saveBinDictFile13ToolStripMenuItem
            // 
            this.saveBinDictFile13ToolStripMenuItem.Name = "saveBinDictFile13ToolStripMenuItem";
            this.saveBinDictFile13ToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.saveBinDictFile13ToolStripMenuItem.Text = "保存(&S)";
            this.saveBinDictFile13ToolStripMenuItem.Click += new System.EventHandler(this.saveBinDictFile13ToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.editToolStripMenuItem.Text = "编辑";
            // 
            // findToolStripMenuItem
            // 
            this.findToolStripMenuItem.Name = "findToolStripMenuItem";
            this.findToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.findToolStripMenuItem.Text = "查找";
            this.findToolStripMenuItem.Click += new System.EventHandler(this.findToolStripMenuItem_Click);
            // 
            // textBoxSearch
            // 
            this.textBoxSearch.Location = new System.Drawing.Point(3, 3);
            this.textBoxSearch.Name = "textBoxSearch";
            this.textBoxSearch.Size = new System.Drawing.Size(162, 20);
            this.textBoxSearch.TabIndex = 2;
            this.textBoxSearch.TextChanged += new System.EventHandler(this.textBoxSearch_TextChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.listBoxList);
            this.panel1.Controls.Add(this.buttonSearch);
            this.panel1.Controls.Add(this.textBoxSearch);
            this.panel1.Location = new System.Drawing.Point(3, 14);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(239, 515);
            this.panel1.TabIndex = 3;
            // 
            // listBoxList
            // 
            this.listBoxList.ContextMenuStrip = this.contextMenuStripList;
            this.listBoxList.FormattingEnabled = true;
            this.listBoxList.Location = new System.Drawing.Point(4, 34);
            this.listBoxList.Name = "listBoxList";
            this.listBoxList.Size = new System.Drawing.Size(228, 472);
            this.listBoxList.TabIndex = 4;
            this.listBoxList.SelectedIndexChanged += new System.EventHandler(this.listBoxList_SelectedIndexChanged);
            // 
            // contextMenuStripList
            // 
            this.contextMenuStripList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportToolStripMenuItem});
            this.contextMenuStripList.Name = "contextMenuStripList";
            this.contextMenuStripList.Size = new System.Drawing.Size(99, 26);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.exportToolStripMenuItem.Text = "导出";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // buttonSearch
            // 
            this.buttonSearch.Location = new System.Drawing.Point(183, 1);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(49, 25);
            this.buttonSearch.TabIndex = 3;
            this.buttonSearch.Text = "查找";
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.labelCount);
            this.panelMain.Controls.Add(this.label4);
            this.panelMain.Controls.Add(this.buttonBatchInsert);
            this.panelMain.Controls.Add(this.buttonDelete);
            this.panelMain.Controls.Add(this.buttonUpdate);
            this.panelMain.Controls.Add(this.buttonInsert);
            this.panelMain.Controls.Add(this.label3);
            this.panelMain.Controls.Add(this.numericUpDownFrequency);
            this.panelMain.Controls.Add(this.label2);
            this.panelMain.Controls.Add(this.textBoxWord);
            this.panelMain.Controls.Add(this.label1);
            this.panelMain.Controls.Add(this.panel1);
            this.panelMain.Controls.Add(this.posCtrl);
            this.panelMain.Enabled = false;
            this.panelMain.Location = new System.Drawing.Point(0, 29);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(792, 585);
            this.panelMain.TabIndex = 4;
            // 
            // labelCount
            // 
            this.labelCount.AutoSize = true;
            this.labelCount.Location = new System.Drawing.Point(351, 20);
            this.labelCount.Name = "labelCount";
            this.labelCount.Size = new System.Drawing.Size(13, 13);
            this.labelCount.TabIndex = 14;
            this.labelCount.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(284, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "单词总数：";
            // 
            // buttonBatchInsert
            // 
            this.buttonBatchInsert.Location = new System.Drawing.Point(526, 505);
            this.buttonBatchInsert.Name = "buttonBatchInsert";
            this.buttonBatchInsert.Size = new System.Drawing.Size(75, 25);
            this.buttonBatchInsert.TabIndex = 12;
            this.buttonBatchInsert.Text = "批量增加";
            this.buttonBatchInsert.UseVisualStyleBackColor = true;
            this.buttonBatchInsert.Click += new System.EventHandler(this.buttonBatchInsert_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Enabled = false;
            this.buttonDelete.Location = new System.Drawing.Point(445, 504);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(75, 25);
            this.buttonDelete.TabIndex = 11;
            this.buttonDelete.Text = "删除";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonUpdate
            // 
            this.buttonUpdate.Enabled = false;
            this.buttonUpdate.Location = new System.Drawing.Point(364, 505);
            this.buttonUpdate.Name = "buttonUpdate";
            this.buttonUpdate.Size = new System.Drawing.Size(75, 25);
            this.buttonUpdate.TabIndex = 10;
            this.buttonUpdate.Text = "修改";
            this.buttonUpdate.UseVisualStyleBackColor = true;
            this.buttonUpdate.Click += new System.EventHandler(this.buttonUpdate_Click);
            // 
            // buttonInsert
            // 
            this.buttonInsert.Enabled = false;
            this.buttonInsert.Location = new System.Drawing.Point(280, 504);
            this.buttonInsert.Name = "buttonInsert";
            this.buttonInsert.Size = new System.Drawing.Size(75, 25);
            this.buttonInsert.TabIndex = 9;
            this.buttonInsert.Text = "添加";
            this.buttonInsert.UseVisualStyleBackColor = true;
            this.buttonInsert.Click += new System.EventHandler(this.buttonInsert_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(284, 120);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "词性";
            // 
            // numericUpDownFrequency
            // 
            this.numericUpDownFrequency.Location = new System.Drawing.Point(667, 48);
            this.numericUpDownFrequency.Maximum = new decimal(new int[] {
            1410065408,
            2,
            0,
            0});
            this.numericUpDownFrequency.Name = "numericUpDownFrequency";
            this.numericUpDownFrequency.Size = new System.Drawing.Size(70, 20);
            this.numericUpDownFrequency.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(619, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "词频";
            // 
            // textBoxWord
            // 
            this.textBoxWord.Location = new System.Drawing.Point(332, 48);
            this.textBoxWord.Name = "textBoxWord";
            this.textBoxWord.Size = new System.Drawing.Size(261, 20);
            this.textBoxWord.TabIndex = 5;
            this.textBoxWord.TextChanged += new System.EventHandler(this.textBoxWord_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(284, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "单词";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.label});
            this.statusStrip.Location = new System.Drawing.Point(0, 591);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(792, 22);
            this.statusStrip.TabIndex = 5;
            this.statusStrip.Text = "statusStrip1";
            // 
            // label
            // 
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(109, 17);
            this.label.Text = "toolStripStatusLabel1";
            // 
            // openFileDialogName
            // 
            this.openFileDialogName.DefaultExt = "*.dct";
            this.openFileDialogName.FileName = "UnknownWords.dct";
            this.openFileDialogName.Filter = "dict|*.dct";
            // 
            // saveFileDialogText
            // 
            this.saveFileDialogText.DefaultExt = "txt";
            this.saveFileDialogText.Filter = "Txt|*.txt";
            // 
            // OpenAsTextToolStripMenuItem
            // 
            this.OpenAsTextToolStripMenuItem.Name = "OpenAsTextToolStripMenuItem";
            this.OpenAsTextToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.OpenAsTextToolStripMenuItem.Text = "以文本文件方式打开";
            this.OpenAsTextToolStripMenuItem.Click += new System.EventHandler(this.OpenAsTextToolStripMenuItem_Click);
            // 
            // SaveAsTextToolStripMenuItem
            // 
            this.SaveAsTextToolStripMenuItem.Name = "SaveAsTextToolStripMenuItem";
            this.SaveAsTextToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.SaveAsTextToolStripMenuItem.Text = "保存为文本文件";
            this.SaveAsTextToolStripMenuItem.Click += new System.EventHandler(this.SaveAsTextToolStripMenuItem_Click);
            // 
            // posCtrl
            // 
            this.posCtrl.Location = new System.Drawing.Point(286, 137);
            this.posCtrl.Name = "posCtrl";
            this.posCtrl.Pos = 0;
            this.posCtrl.Size = new System.Drawing.Size(480, 293);
            this.posCtrl.TabIndex = 1;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 613);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "FormMain";
            this.Text = "字典管理";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.contextMenuStripList.ResumeLayout(false);
            this.panelMain.ResumeLayout(false);
            this.panelMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrequency)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialogDict;
        private System.Windows.Forms.SaveFileDialog saveFileDialogDict;
        private PosDisplayCtrl.PosCtrl posCtrl;
        private System.Windows.Forms.TextBox textBoxSearch;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonSearch;
        private System.Windows.Forms.ListBox listBoxList;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.TextBox textBoxWord;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDownFrequency;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonUpdate;
        private System.Windows.Forms.Button buttonInsert;
        private System.Windows.Forms.Button buttonBatchInsert;
        private System.Windows.Forms.Label labelCount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolStripMenuItem openBinDictFile13ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveBinDictFile13ToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel label;
        private System.Windows.Forms.OpenFileDialog openFileDialogName;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripList;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialogText;
        private System.Windows.Forms.ToolStripMenuItem OpenAsTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveAsTextToolStripMenuItem;

    }
}