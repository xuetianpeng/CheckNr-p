using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Net;
using System.Linq;
using System.Configuration;
using System.Xml;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CheckNr
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            
            InitializeComponent();
            
        }

        #region 窗体特效API
        private const uint WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int GWL_STYLE = (-16);
        private const int GWL_EXSTYLE = (-20);
        private const int LWA_ALPHA = 0;
        [DllImport("user32", EntryPoint = "SetWindowLong")]
        private static extern uint SetWindowLong(
        IntPtr hwnd,
        int nIndex,
        uint dwNewLong
        );
        [DllImport("user32", EntryPoint = "GetWindowLong")]
        private static extern uint GetWindowLong(
        IntPtr hwnd,
        int nIndex
        );
        [DllImport("user32", EntryPoint = "SetLayeredWindowAttributes")]
        private static extern int SetLayeredWindowAttributes(
        IntPtr hwnd,
        int crKey,
        int bAlpha,
        int dwFlags
        );
        #endregion

        static string path;//日结备份路径
        static string snpath;
        static string ST;//店号
        static string FileDX;//日结备份文件大小
        static bool ed, nr, jt, pc;//ed01正常、日结备份正常、截图正常、PC正常
        static bool n8, n12, n16, n20;
        static bool j8, j20;
        int zctime, gztime;
        static DateTime lastddtime = DateTime.Now, lastpcddtime = DateTime.Now;
        DateTime lastsntime;
        static string lastdd = "", lastpcdd = "";
        static string dd, fddd;
        static string ph;
        int dded = 0;
        /// <summary>
        /// 设置窗体具有鼠标穿透效果
        /// </summary>
        public void SetPenetrate()
        {
            GetWindowLong(this.Handle, GWL_EXSTYLE);
            SetWindowLong(this.Handle, GWL_EXSTYLE, WS_EX_TRANSPARENT | WS_EX_LAYERED);
        }

        void GetAllFileInfo() 
        {
            ed = nr = jt = pc = true;
            ConfigXmlDocument xml = new ConfigXmlDocument();
            xml.Load("config.xml");
            XmlNode node = xml.SelectSingleNode("/configuration/appSettings");
            path = node.SelectSingleNode("jt").Attributes["value"].Value;
            snpath  = node.SelectSingleNode("su").Attributes["value"].Value;
            ST = node.SelectSingleNode("st").Attributes["value"].Value;
            zctime = Convert.ToInt32(node.SelectSingleNode("zc").Attributes["value"].Value);
            gztime = Convert.ToInt32(node.SelectSingleNode("gz").Attributes["value"].Value);
            dd = node.SelectSingleNode("rb").Attributes["value"].Value;
            fddd = node.SelectSingleNode("fd").Attributes["value"].Value;
            ph = node.SelectSingleNode("ph").Attributes["value"].Value;
            if (snpath == "")
            {
                if (MessageBox.Show("未发现苏宁路径的配置文件，请选择路径后保存", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    folderBrowserDialog1.ShowDialog();
                    ((XmlElement)node.SelectSingleNode("su")).SetAttribute("value", folderBrowserDialog1.SelectedPath);
                    xml.Save("config.xml");
                    snpath = folderBrowserDialog1.SelectedPath;
                }
                else
                {
                    this.Close();
                }
            }
            else
            {

            }
            if (path=="")
            {
                if (MessageBox.Show("未发现备份截图路径的配置文件，请选择路径后保存", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    folderBrowserDialog1.ShowDialog();
                    ((XmlElement)node.SelectSingleNode("jt")).SetAttribute("value", folderBrowserDialog1.SelectedPath);
                    xml.Save("config.xml");
                    path = folderBrowserDialog1.SelectedPath;
                }
                else
                {
                    this.Close();
                }
            }
            else
            {
                
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            CED();
            CheckNRFile();
            CheckPC();
            UpdateImage();
            Setminilo();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            n8 = n12 = n16 = n20 = j8 = j20 = false;
            nr = jt = false;
            GetAllFileInfo();
            GetPingIP();
            CED();
            CheckNRFile();
            CheckPC();
            Setminilo();
            UpdateImage();
            SetPenetrate();
        }
        void Setminilo() 
        {
            int x = SystemInformation.PrimaryMonitorSize.Width - this.Width;
            int y = 0;
            if (ed && nr && jt && pc)
            {
                y = y - this.Height + 22;
                timer1.Interval = zctime;
            }
            else 
            {
                timer1.Interval = gztime;
            }
            this.Location = new Point(x, y);
            toolStripStatusLabel1.BackColor = ed ? Color.Green : Color.Red;
            toolStripStatusLabel2.BackColor = nr ? Color.Green : Color.Red;
            toolStripStatusLabel3.BackColor = jt ? Color.Green : Color.Red;
            toolStripStatusLabel4.BackColor = pc ? Color.Green : Color.Red;
        }

        //需要异步
        void GetPingIP()
        {
            //ConfigXmlDocument xml = new ConfigXmlDocument();
            XmlDocument xml = new XmlDocument();
            xml.Load("config.xml");
            var node = xml.SelectSingleNode("/configuration/Check/PC");
            int i = 0;
            foreach (XmlNode xn in node.ChildNodes)
            {
                if (i % 2 == 0)
                {
                    this.Height = this.Height + 35;
                    Button btn = new Button();
                    this.Controls.Add(btn);
                    btn.Name = xn.Attributes["key"].Value;
                    btn.Location = new System.Drawing.Point(5, this.Height - 53);
                    btn.Size = new System.Drawing.Size(120, 27);
                    btn.BackColor = Color.Green;
                    btn.ForeColor = Color.White;
                    btn.Font = new Font("宋体", 16);
                    btn.Text = xn.Attributes["key"].Value;
                    btn.Tag = xn.Attributes["value"].Value; ;
                    //btn.UseVisualStyleBackColor = true;

                }
                else
                {
                    Button btn = new Button();
                    this.Controls.Add(btn);
                    btn.Name = xn.Attributes["key"].Value;
                    btn.Location = new Point(135, this.Height - 53);
                    btn.Size = new System.Drawing.Size(120, 27);
                    btn.BackColor = Color.Red;
                    btn.ForeColor = Color.White;
                    btn.Font = new Font("宋体", 16);
                    btn.Text = xn.Attributes["key"].Value;
                    btn.Tag = xn.Attributes["value"].Value;
                    //btn.UseVisualStyleBackColor = true;
                }
                i++;
            }

        }

        //需要异步
        void CED() 
        {
            bool nr = System.IO.Directory.Exists(@"\\172.16."+ST+@".10\nr");
            string s = "";
            if (!nr)
            {
                label4.ForeColor = Color.Red;
                s = label4.Text = "NR共享文件夹不可用，请检查！！！！";
                ed = false;
                lastdd = s;
                string paraUrlCoded = "{\"msgtype\":\"text\",\"text\":{\"content\":\"";
                paraUrlCoded += "NR共享文件夹不可用，请检查！！！！";
                paraUrlCoded += "\"},\"at\":{\"atMobiles\":[\"";
                paraUrlCoded += ph;
                paraUrlCoded += "\"],\"isAtAll\": false} }";
                Post(paraUrlCoded);
            }
            else
            {
                label4.ForeColor = Color.Black;
                label4.Text = "NR共享文件夹正常。";
                ed = true;
            }
        }

        //需要异步
        void CheckNRFile() 
        {
            //label5.Text = label6.Text = label7.Text = label8.Text = label9.Text = label10.Text = "";
            DateTime x = DateTime.Now;
            if (x.Hour < 8)
            {
                label5.Text = label6.Text = label7.Text = label8.Text = label9.Text = label10.Text = "";
                nr = jt = true;
            }
            else
            {
                nr = jt = false;
            }
            if (x.Hour >= 8)
            {
                //8点钟
                if (x.Hour == 8 && x.Minute <= 30)
                {

                }
                else
                {
                    if (!n8)
                    {
                        n8 = ExFile(@"\\172.16." + ST + @".10\nr\" + GetWeekInt(DateTime.Now.DayOfWeek).ToString() + @"\", DateTime.Now.ToString("expyyMMdd") + "08", ".dump.gz");
                        SetN(8, n8);
                    }
                    if (!j8)
                    {
                        j8 = ExFile(path + @"\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\", DateTime.Now.ToString("yyyyMMdd"), "-1.jpg");
                        SetJ(8, j8);
                    }
                    if (n8)
                    {
                        nr = true;
                    }
                    if (j8)
                    {
                        jt = true;
                    }
                }
            }
            if (x.Hour >= 12)
            {
                if (x.Hour == 12 && x.Minute <= 30)
                {
                    
                }
                else
                {
                    if (!n12)
                    {
                        n12 = ExFile(@"\\172.16." + ST + @".10\nr\" + GetWeekInt(DateTime.Now.DayOfWeek).ToString() + @"\", DateTime.Now.ToString("expyyMMdd") + "12", ".dump.gz");
                        SetN(12, n12);
                    }
                    if (n8 && n12)
                    {
                        nr = true;
                    }
                    if (j8)
                    {
                        jt = true;
                    }
                }
            }
            if (x.Hour >= 16)
            {
                if (x.Hour == 16 && x.Minute <= 30)
                {

                }
                else
                {
                    if (!n16)
                    {
                        n16 = ExFile(@"\\172.16." + ST + @".10\nr\" + GetWeekInt(DateTime.Now.DayOfWeek).ToString() + @"\", DateTime.Now.ToString("expyyMMdd") + "16", ".dump.gz");
                        SetN(16, n16);
                    }
                    if (n8 && n12 && n16)
                    {
                        nr = true;
                    }
                    if (j8)
                    {
                        jt = true;
                    }
                }
            }
            if (x.Hour >= 20)
            {
                if (x.Hour == 20 && x.Minute <= 30)
                {

                }
                else
                {
                    if (!n20)
                    {
                        n20 = ExFile(@"\\172.16." + ST + @".10\nr\" + GetWeekInt(DateTime.Now.DayOfWeek).ToString() + @"\", DateTime.Now.ToString("expyyMMdd") + "20", ".dump.gz");
                        SetN(20, n20);
                    }
                    if (!j20)
                    {
                        j20 = ExFile(path + @"\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\", DateTime.Now.ToString("yyyyMMdd"), "-2.jpg");
                        SetJ(20, j20);
                    }
                    if (n8 && n12 && n16 && n20)
                    {
                        nr = true;
                    }
                    if (j8 && j20)
                    {
                        jt = true;
                    }
                }
            }
        }

        //需要异步
        void CheckPC() 
        {
            bool has = false;
            foreach (Control c in this.Controls) 
            {
                if (c.GetType().Name == "Button")
                {
                    if (PingPC(((Button)c).Tag.ToString()))
                    {
                        if (((Button)c).Tag.ToString() != "")
                        {
                            ((Button)c).BackColor = Color.Green;
                            if (!has)
                            {
                                pc = true;
                            }
                        }
                    }
                    else
                    {
                        if (((Button)c).Tag.ToString() != "")
                        {
                            ((Button)c).BackColor = Color.Red;
                            has = true;
                            pc = false;
                        }
                    }
                }
            }
        }

        static bool PingPC(string IP) 
        {
            Ping pin = new Ping();
            PingReply pr = pin.Send(IP);
            if (pr.Status == IPStatus.Success)
            {
                return true;
            }
            else
            {
                if (lastpcddtime != null && lastpcdd != "")
                {
                    if ((DateTime.Now - lastpcddtime).Hours < 1 && lastpcdd == IP)
                    {
                        return false;
                    }
                    else
                    {
                        lastpcdd = IP;
                        string paraUrlCoded = "{\"msgtype\":\"text\",\"text\":{\"content\":\"";
                        paraUrlCoded += "IP:" + IP + "网络不通,请检查。";
                        paraUrlCoded += "\"},\"at\":{\"atMobiles\":[\"";
                        paraUrlCoded += ph;
                        paraUrlCoded += "\"],\"isAtAll\": false} }";
                        ////textBox4.Text = paraUrlCoded;
                        Post(paraUrlCoded);
                        return false;
                    }
                }
                else
                {
                    lastpcdd = IP;
                    string paraUrlCoded = "{\"msgtype\":\"text\",\"text\":{\"content\":\"";
                    paraUrlCoded += "IP:" + IP + "网络不通,请检查。";
                    paraUrlCoded += "\"},\"at\":{\"atMobiles\":[\"";
                    paraUrlCoded += ph;
                    paraUrlCoded += "\"],\"isAtAll\": false} }";
                    ////textBox4.Text = paraUrlCoded;
                    Post(paraUrlCoded);
                    return false;
                }
            }
        }

        //需要异步
        bool UpdateImage()
        {
            try
            {
                EDPToolBoxDataContext edp = new CheckNr.EDPToolBoxDataContext();
                var cns = from cn in edp.T_CheckNr where cn.STID == ST select cn;
                cns.FirstOrDefault().LastTime = DateTime.Now;
                string msg = "";
                foreach (Control con in this.Controls)
                {
                    if (con.GetType().Name == "Label")
                    {
                        if (con.Text != "")
                        {
                            msg = con.Text + msg + ";";
                        }
                    }
                    else
                    {
                        if (con.GetType().Name == "Button")
                        {
                            msg = msg + con.Text + (con.BackColor != Color.Red ? " ok;  " : " bad;  ");
                        }
                    }
                    
                }
                cns.FirstOrDefault().LastMsg = msg;
                edp.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            DateTime x = DateTime.Now;
            if (x.Hour >= 22)
            {
                string s = snpath + @"\logs\recallLog_" + x.ToString("yyyy-MM-dd") + @".txt";
                if (File.Exists(s))
                {
                    FileInfo fi = new FileInfo(snpath + @"\logs\recallLog_" + x.ToString("yyyy-MM-dd") + @".txt");
                    if (fi.LastAccessTime.Hour < 22)
                    {
                        if (lastsntime == null)
                        {
                            lastsntime = DateTime.Now;
                            //OpenSuning();
                            string paraUrlCoded = "{\"msgtype\":\"text\",\"text\":{\"content\":\"";
                            paraUrlCoded += "苏宁接口尚未检查,请立即打开重调工具检查。";
                            paraUrlCoded += "\"},\"at\":{\"atMobiles\":[\"";
                            paraUrlCoded += ph;
                            paraUrlCoded += "\"],\"isAtAll\": true} }";
                            ////textBox4.Text = paraUrlCoded;
                            Postnb(paraUrlCoded);
                        }
                        else
                        {
                            if ((DateTime.Now - lastsntime).Hours > 1)
                            {
                                lastsntime = DateTime.Now;
                                string paraUrlCoded = "{\"msgtype\":\"text\",\"text\":{\"content\":\"";
                                paraUrlCoded += "苏宁接口尚未检查,请立即打开重调工具检查。";
                                paraUrlCoded += "\"},\"at\":{\"atMobiles\":[\"";
                                paraUrlCoded += ph;
                                paraUrlCoded += "\"],\"isAtAll\": true} }";
                                ////textBox4.Text = paraUrlCoded;
                                Postnb(paraUrlCoded);
                                //OpenSuning();
                            }
                        }

                    }
                    else
                    {
                        lastsntime = DateTime.Now;
                        string paraUrlCoded = "{\"msgtype\":\"text\",\"text\":{\"content\":\"";
                        paraUrlCoded += "苏宁接口尚未检查,请立即打开重调工具检查。";
                        paraUrlCoded += "\"},\"at\":{\"atMobiles\":[\"";
                        paraUrlCoded += ph;
                        paraUrlCoded += "\"],\"isAtAll\": true} }";
                        ////textBox4.Text = paraUrlCoded;
                        Postnb(paraUrlCoded);
                        //OpenSuning();
                    }
                }
                else
                {
                    lastsntime = DateTime.Now;
                    string paraUrlCoded = "{\"msgtype\":\"text\",\"text\":{\"content\":\"";
                    paraUrlCoded += "苏宁接口尚未检查,请立即打开重调工具检查。";
                    paraUrlCoded += "\"},\"at\":{\"atMobiles\":[\"";
                    paraUrlCoded += ph;
                    paraUrlCoded += "\"],\"isAtAll\": true} }";
                    ////textBox4.Text = paraUrlCoded;
                    Postnb(paraUrlCoded);
                    //OpenSuning();
                }
            }
        }

        private static void Post(string paraUrlCoded)
        {
            string url = dd;//@"https://oapi.dingtalk.com/robot/send?access_token=8e03a1889da305b506dcfcf70f3f5b82f273a56c71d10e2277a1d434a4a8d2bc";
            string strURL = url;
            System.Net.HttpWebRequest request;
            request = (System.Net.HttpWebRequest)WebRequest.Create(strURL);
            request.Method = "POST";

            //判断是否必要性
            request.ContentType = "application/json;charset=UTF-8";
            //request.ContentType = "application/json;";


            //添加cookie测试
            //Uri uri = new Uri(url);
            //Cookie cookie = new Cookie("Name", DateTime.Now.ToString()); // 设置key、value形式的Cookie
            //CookieContainer cookies = new CookieContainer();
            //cookies.Add(uri, cookie);
            //request.CookieContainer = cookies;

            //发送请求的另外形式
            //request.Method = "POST";
            //request.ContentType = "application/x-www-form-urlencoded";

            byte[] payload;
            payload = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
            request.ContentLength = payload.Length;
            Stream writer = request.GetRequestStream();
            writer.Write(payload, 0, payload.Length);
            writer.Close();


            System.Net.HttpWebResponse response;
            response = (System.Net.HttpWebResponse)request.GetResponse();
            System.IO.Stream s;
            s = response.GetResponseStream();
            string StrDate = "";
            string strValue = "";
            StreamReader Reader = new StreamReader(s, Encoding.UTF8);
            while ((StrDate = Reader.ReadLine()) != null)
            {
                strValue += StrDate + "\r\n";
            }

            //添加关闭相应
            Reader.Close();
            response.Close();

            //改变返回结果形式以看全部提示
            //label3.Text = strValue;
            // MessageBox.Show(strValue);
        }

        private static void Postnb(string paraUrlCoded)
        {
            string url = fddd; //@"https://oapi.dingtalk.com/robot/send?access_token=8e03a1889da305b506dcfcf70f3f5b82f273a56c71d10e2277a1d434a4a8d2bc";
            string strURL = url;
            System.Net.HttpWebRequest request;
            request = (System.Net.HttpWebRequest)WebRequest.Create(strURL);
            request.Method = "POST";

            //判断是否必要性
            request.ContentType = "application/json;charset=UTF-8";
            //request.ContentType = "application/json;";


            //添加cookie测试
            //Uri uri = new Uri(url);
            //Cookie cookie = new Cookie("Name", DateTime.Now.ToString()); // 设置key、value形式的Cookie
            //CookieContainer cookies = new CookieContainer();
            //cookies.Add(uri, cookie);
            //request.CookieContainer = cookies;

            //发送请求的另外形式
            //request.Method = "POST";
            //request.ContentType = "application/x-www-form-urlencoded";

            byte[] payload;
            payload = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
            request.ContentLength = payload.Length;
            Stream writer = request.GetRequestStream();
            writer.Write(payload, 0, payload.Length);
            writer.Close();


            System.Net.HttpWebResponse response;
            response = (System.Net.HttpWebResponse)request.GetResponse();
            System.IO.Stream s;
            s = response.GetResponseStream();
            string StrDate = "";
            string strValue = "";
            StreamReader Reader = new StreamReader(s, Encoding.UTF8);
            while ((StrDate = Reader.ReadLine()) != null)
            {
                strValue += StrDate + "\r\n";
            }

            //添加关闭相应
            Reader.Close();
            response.Close();

            //改变返回结果形式以看全部提示
            //label3.Text = strValue;
            // MessageBox.Show(strValue);
        }
        public static byte[] Bitmap2Byte(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] data = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(data, 0, Convert.ToInt32(stream.Length));
                return data;
            }
        }

       //检测文件大小
        bool ExFile(string paths, string startstr, string endstr) 
        {
            if (System.IO.Directory.Exists(paths))
            {
                string[] Files = Directory.GetFiles(paths);
                foreach (string i in Files)
                {
                    if (i.StartsWith(paths + startstr) && i.ToLower().EndsWith(endstr))
                    {
                        FileInfo fi = new FileInfo(i);
                        FileDX = (fi.Length / 1024 / 1024).ToString() + "MB";
                        return true;
                    }
                }
            }
            return false;
        }

        //转换日期
        int GetWeekInt(DayOfWeek dw) 
        {
            switch (dw) 
            {
                case DayOfWeek.Monday :
                    return 1;
                case DayOfWeek.Tuesday :
                    return 2;
                case DayOfWeek.Wednesday:
                    return 3;
                case DayOfWeek.Thursday:
                    return 4;
                case DayOfWeek.Friday:
                    return 5;
                case DayOfWeek.Saturday:
                    return 6;
                default :
                    return 0;
            }
        }

        void OpenSuning()
        {
            if (DateTime.Now.Hour == 22)
            {

            }
            else
            {

            }
        }

        void SetN(int N, bool sucess)
        {
            string s = "";
            if (sucess)
            {
                switch (N)
                {
                    case 8:
                        label7.ForeColor = Color.Black;
                        s = label7.Text = "8点备份正常。" + FileDX;
                        break;
                    case 12:
                        label8.ForeColor = Color.Black;
                        s = label8.Text = "12点备份正常。" + FileDX;
                        break;
                    case 16:
                        label9.ForeColor = Color.Black;
                        s = label9.Text = "16点备份正常。" + FileDX;
                        break;
                    case 20:
                        label10.ForeColor = Color.Black;
                        s = label10.Text = "20点备份正常。" + FileDX;
                        break;
                }
            }
            else
            {
                switch (N)
                {
                    case 8:
                        label7.ForeColor = Color.Red;
                        s = label7.Text = "8点备份异常，请检查。";
                        break;
                    case 12:
                        label8.ForeColor = Color.Red;
                        s = label8.Text = "12点备份异常，请检查。";
                        break;
                    case 16:
                        label9.ForeColor = Color.Red;
                        s = label9.Text = "12点备份异常，请检查。";
                        break;
                    case 20:
                        label10.ForeColor = Color.Red;
                        s = label10.Text = "20点备份异常，请检查。";
                        break;
                }
                if (lastddtime != null && lastdd != "")
                {
                    if ((DateTime.Now - lastddtime).Hours < 1 && lastdd == s)
                    {

                    }
                    else
                    {
                        lastdd = s;
                        string paraUrlCoded = "{\"msgtype\":\"text\",\"text\":{\"content\":\"";
                        paraUrlCoded += Convert.ToString(N) + "点备份失败,请检查。";
                        paraUrlCoded += "\"},\"at\":{\"atMobiles\":[\"";
                        paraUrlCoded += ph;
                        paraUrlCoded += "\"],\"isAtAll\": false} }";
                        //textBox4.Text = paraUrlCoded;
                        Post(paraUrlCoded);
                    }
                }
                else
                {
                    lastdd = s;
                    string paraUrlCoded = "{\"msgtype\":\"text\",\"text\":{\"content\":\"";
                    paraUrlCoded += Convert.ToString(N) + "点备份失败,请检查。";
                    paraUrlCoded += "\"},\"at\":{\"atMobiles\":[\"";
                    paraUrlCoded += ph;
                    paraUrlCoded += "\"],\"isAtAll\": false} }";
                    //textBox4.Text = paraUrlCoded;
                    Post(paraUrlCoded);
                }
                
            }
        }

        void SetJ(int J, bool sucess)
        {
            string s = "";
            if (sucess)
            {
                switch (J)
                {
                    case 8:
                        label5.ForeColor = Color.Black;
                        s = label5.Text = "08点截图已保存。";
                        break;
                    case 20:
                        label6.ForeColor = Color.Black;
                        s = label6.Text = "20点截图已保存。";
                        break;
                }
            }
            else
            {
                switch (J)
                {
                    case 8:
                        label5.ForeColor = Color.Red;
                        s = label5.Text = "08点截图未保存。";
                        break;
                    case 20:
                        label6.ForeColor = Color.Red;
                        s = label6.Text = "20点截图未保存";
                        break;
                }
                if (lastddtime != null && lastdd != "")
                {
                    if ((DateTime.Now - lastddtime).Hours < 1 && lastdd == s)
                    {

                    }
                    else
                    {
                        lastdd = s;
                        string paraUrlCoded = "{\"msgtype\":\"text\",\"text\":{\"content\":\"";
                        paraUrlCoded += Convert.ToString(J) + "点截图未保存,请检查。";
                        paraUrlCoded += "\"},\"at\":{\"atMobiles\":[\"";
                        paraUrlCoded += ph;
                        paraUrlCoded += "\"],\"isAtAll\": false} }";
                        //textBox4.Text = paraUrlCoded;
                        Post(paraUrlCoded);
                    }
                }
                else
                {
                    lastdd = s;
                    string paraUrlCoded = "{\"msgtype\":\"text\",\"text\":{\"content\":\"";
                    paraUrlCoded += Convert.ToString(J) + "点截图未保存,请检查。";
                    paraUrlCoded += "\"},\"at\":{\"atMobiles\":[\"";
                    paraUrlCoded += ph;
                    paraUrlCoded += "\"],\"isAtAll\": false} }";
                    //textBox4.Text = paraUrlCoded;
                    Post(paraUrlCoded);
                }
            }
        }

    }
}