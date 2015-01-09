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
using System.Drawing;
using System.Web;
using System.Drawing.Text;
using System.IO;
using System.Drawing.Imaging;
using System.Collections;

namespace MaxLabs.bbsMax.ValidateCodes
{

    public class ValidateCode_Style7 : ValidateCodeType
    {

        public override string Name
        {
            get { return "中文(蓝色)"; }
        }

        public override bool DisableIme
        {
            get
            {
                return false;
            }
        }

        public override byte[] CreateImage(out string validataCode)
        {
            string strFormat = "丰,王,井,开,夫,天,无,元,专,云,扎,艺,木,五,支,厅,不,太,犬,区,历,尤,友,匹,车,巨,牙,屯,比,互,切,瓦,止,少,日,中,冈,贝,内,水,见,午,牛,手,毛,气,升,长,仁,什,片,仆,化,仇,币,仍,仅,斤,爪,反,介,父,从,今,凶,分,乏,公,仓,月,氏,勿,欠,风,丹,匀,乌,凤,勾,文,六,方,火,为,斗,忆,订,计,户,认,心,尺,引,丑,巴,孔,队,办,以,允,予,劝,双,书,幻,玉,刊,示,末,未,击,打,巧,正,扑,扒,功,扔,去,甘,世,古,节,本,术,可,丙,左,厉,右,石,布,龙,平,灭,轧,东,卡,北,占,业,旧,帅,归,且,旦,奏,春,帮,珍,玻,毒,型,挂,封,持,项,垮,挎,城,挠,政,赴,赵,挡,挺,括,拴,拾,挑,指,垫,挣,挤,拼,挖,按,挥,挪,某,甚,革,荐,巷,带,草,茧,茶,荒,茫,荡,荣,故,胡,南,药,标,枯,柄,栋,相,查,柏,柳,柱,柿,栏,树,要,咸,威,歪,研,砖,厘,厚,砌,砍,面,耐,耍,牵,残,殃,轻,鸦,皆,背,战,点,临,览,竖,省,削,尝,是,盼,眨,哄,显,哑,冒,映,星,昨,畏,趴,胃,贵,界,虹,虾,蚁,思,蚂,虽,品,咽,骂,哗,咱,响,哈,咬,咳,哪,炭,峡,罚,贱,贴,骨,钞,钟,钢,钥,钩,卸,缸,拜,看,矩,怎,牲,选,适,秒,香,种,秋,科,重,复,竿,段,便,俩,贷,顺,修,保,促,侮,俭,俗,俘,信,皇,泉,鬼,侵,追,俊,盾,待,律,很,须,叙,剑,逃,食,盆,胆,胜,胞,胖,脉,勉,狭,狮,独,狡,狱,狠,贸,怨,急,饶,蚀,饺,饼,弯,将,奖,哀,亭,亮,度,迹,庭,疮,疯,疫,疤,姿,亲,音,帝,施,闻,阀,阁,差,养,美,姜,叛,送,类,迷,前,首,逆,总,炼,炸,炮,烂,剃,洁,洪,洒,浇,浊,洞,测,洗,活,派,洽,染,济,洋,洲,浑,浓,津,恒,恢,恰,恼,恨,举,觉,宣,室,宫,宪,突,穿,窃,客,冠,语,扁,袄,祖,神,祝,误,诱,说,诵,垦,退,既,屋,昼,费,陡,眉,孩,除,险,院,娃,姥,姨,姻,娇,怒,架,贺,盈,勇,怠,柔,垒,绑,绒,结,绕,骄,绘,给,络,骆,绝,绞,统";
            GetRandom(strFormat, this.ValidataCodeLength, out validataCode);
            Bitmap bitmap;
            MemoryStream stream = new MemoryStream();

            ImageBmp(out bitmap, validataCode);
            bitmap.Save(stream, ImageFormat.Png);
            bitmap.Dispose();

            bitmap = null;
            stream.Close();
            stream.Dispose();

            return stream.GetBuffer();
        }
        #region 验证码美化
        bool fontTextRenderingHint = false;
        bool FontTextRenderingHint
        {
            get { return fontTextRenderingHint; }
            set { fontTextRenderingHint = value; }
        }
        #endregion

        #region 验证码长度
        int validataCodeLength = 4;
        public int ValidataCodeLength
        {
            get { return validataCodeLength; }
            set { validataCodeLength = value; }
        }
        #endregion

        #region 验证码字体大小(默认15像素，可以自行修改)
        int validataCodeSize = 16;
        public int ValidataCodeSize
        {
            get { return validataCodeSize; }
            set { validataCodeSize = value; }
        }
        #endregion

        #region 图片高度
        int imageHeight = 30;
        public int ImageHeight
        {
            get { return imageHeight; }
            set { imageHeight = value; }
        }
        #endregion

        #region 边框补(默认1像素)
        int padding = 1;
        public int Padding
        {
            get { return padding; }
            set { padding = value; }
        }
        #endregion

        #region 输出燥点的颜色
        Color chaosColor = Color.FromArgb(0xaa, 0xaa, 0x33);
        public Color ChaosColor
        {
            get { return chaosColor; }
            set { chaosColor = value; }
        }
        #endregion

        #region 自定义背景色(默认白色)
        Color backgroundColor = Color.White;
        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set { backgroundColor = value; }
        }
        #endregion

        #region 自定义颜色
        Color drawColor = Color.FromArgb(0x32, 0x99, 0xcc);
        public Color DrawColor
        {
            get { return drawColor; }
            set { drawColor = value; }
        }
        #endregion

        #region 自定义字体
        string validateCodeFont = "Arial";
        public string ValidateCodeFont
        {
            get { return validateCodeFont; }
            set { validateCodeFont = value; }
        }
        #endregion

        #region CreateImage
        void ImageBmp(out Bitmap bitMap, string validataCode)
        {
            int width = (int)(((this.validataCodeLength * this.validataCodeSize) * 1.3) + 10);
            bitMap = new Bitmap(width, this.ImageHeight);
            DisposeImageBmp(ref bitMap);
            CreateImageBmp(ref bitMap, validataCode);
        }

        //绘制验证码图片
        void CreateImageBmp(ref Bitmap bitMap, string validateCode)
        {
            Graphics graphics = Graphics.FromImage(bitMap);
            if (this.fontTextRenderingHint)
                graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
            else
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            Font font = new Font(this.validateCodeFont, this.validataCodeSize, FontStyle.Regular);
            Brush brush = new SolidBrush(this.drawColor);
            int maxValue = Math.Max((this.ImageHeight - this.validataCodeSize) - 5, 0);
            Random random = new Random();
            for (int i = 0; i < this.validataCodeLength; i++)
            {
                int[] numArray = new int[] { (i * this.validataCodeSize) + i * 5, random.Next(maxValue) };
                System.Drawing.Point point = new System.Drawing.Point(numArray[0], numArray[1]);
                graphics.DrawString(validateCode[i].ToString(), font, brush, (PointF)point);
            }
            graphics.Dispose();
        }

        //混淆验证码图片
        void DisposeImageBmp(ref Bitmap bitmap)
        {
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);

            Pen pen = new Pen(this.DrawColor, 1f);
            Random random = new Random();
            System.Drawing.Point[] pointArray = new System.Drawing.Point[2];

            pen = new Pen(ChaosColor, 1);
            for (int i = 0; i < this.validataCodeLength * 10; i++)
            {
                int x = random.Next(bitmap.Width);
                int y = random.Next(bitmap.Height);
                graphics.DrawRectangle(pen, x, y, 1, 1);
            }
            graphics.Dispose();
        }


        //获取随机数
        private static void GetRandom(string formatString, int len, out string codeString)
        {
            int j1;
            codeString = string.Empty;
            string[] strResult = formatString.Split(new char[] { ',' });//把上面字符存入数组
            Random rnd = new Random();
            for (int i = 0; i < len; i++)
            {
                j1 = rnd.Next(100000) % strResult.Length;
                codeString = codeString + strResult[j1].ToString();
            }
        }
        #endregion
    }
}