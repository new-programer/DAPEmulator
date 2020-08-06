using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using Newtonsoft.Json;

namespace SocketClient
{
    public partial class client : Form
    {
        public client()
        {
            /*这条语句存在的意义：解除异常（线程间操作无效：从不是创建控件“dapFactor”的线程访问它........）
             * 这只是一种解决方法，还有一种方法是通过委托机制来解决，具体见链接： https://blog.csdn.net/weixin_44913038/article/details/102996655
             */
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;


            InitializeComponent();
        }

        DAPProtocol dapProtocol;


        //定义回调
        private delegate void SetTextCallBack(string strValue);
        //声明
        private SetTextCallBack setCallBack;

        //定义接收服务端发送消息的回调
        private delegate void ReceiveMsgCallBack(string strMsg);
        //声明
        private ReceiveMsgCallBack receiveCallBack;

        //创建连接的Socket
        Socket socketSend;

        //创建接收客户端发送消息的线程
        Thread threadReceive;

        //定义 DAP factor
        //int DAP_corr_factor = -100;

       /* 临时定义和DAP相关的所有参数*/
        //double DAP_corr_factor = 1.00; //定义 DAP 校正因子 默认为 1.00
        string sw_version = ""; //版本号
        string InternalSN = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0)).ToString(); //序列号
        double CorrectionFactor = -1.00;//校正因子
        int AirPressure = 1013;  //气压，默认值 1013Pa
        int Temperature = 20;  //温度，默认值 20`C / 293.15K

        /// <summary>
        /// 创建网络通信连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateConnection(object sender, EventArgs e)
        {
            try
            {
                socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Parse(this.ipAddress.Text.Trim());
                socketSend.Connect(ip, Convert.ToInt32(this.port.Text.Trim()));

                //实例化回调
                setCallBack = new SetTextCallBack(SetValue);
                receiveCallBack = new ReceiveMsgCallBack(SetValue);
                this.clientLog.Invoke(setCallBack, "[" + DateTime.Now.ToString() + "] connect successfully!");

                //开启一个新的线程不停的接收服务器发送消息的线程
                threadReceive = new Thread(new ThreadStart(Receive));

                //设置为后台线程
                threadReceive.IsBackground = true;
                threadReceive.Start();
            }
            catch(Exception ex)
            {
                MessageBox.Show("DAP_emulator connection error: \n" + ex.Message);
            }
        }

        private void SetValue(string strMsg)
        {
            clientLog.AppendText(strMsg + "\n");
        }

        /// <summary>
        /// 接收 模拟器 发送的数据
        /// </summary>
        private void Receive()
        {
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[2048];
                    //实际接收到的字节数
                    int count = socketSend.Receive(buffer);
                    if (count == 0)
                    {
                        break;
                    }
                    else
                    {
                        /*获取DAP_emulator 发来的数据*/
                        string jsonStr = Encoding.Default.GetString(buffer, 0, count);
                        Dictionary<string, string> paraDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonStr);

                        //解析收到的信息
                        if (paraDict["data"].Equals(""))
                        {
                            MessageBox.Show("do not get the value of 'DAP_corr_fctor'");
                        }
                        else
                        {
                            string tempStr = paraDict["data"];

                            if (tempStr.StartsWith("allData") || tempStr.StartsWith("reset"))
                            {
                                if (tempStr.StartsWith("allData"))
                                {
                                    tempStr = tempStr.Remove(0, 7); //去掉前缀 “allData”
                                }
                                else if (tempStr.StartsWith("reset"))
                                {
                                    tempStr = tempStr.Remove(0, 5); //去掉前缀 “reset”
                                }
                                Dictionary<string, string> allData = JsonConvert.DeserializeObject<Dictionary<string, string>>(tempStr);

                                //从获取的 allData 中取出每个数据展示在界面上
                                sw_version = allData["sw_version"];
                                InternalSN = allData["InternalSN"];
                                CorrectionFactor = double.Parse(allData["CorrectionFactor"]);
                                AirPressure = int.Parse(allData["AirPressure"]);
                                Temperature = int.Parse(allData["Temperature"]);

                                version.Text = sw_version.Trim();
                                sn.Text = InternalSN;
                                factor.Text = CorrectionFactor.ToString();
                                pressure.Text = AirPressure.ToString();
                                T.Text = Temperature.ToString();

                                Application.DoEvents();//使textbook 立即刷新刚赋的值

                            }
                            else if (tempStr.StartsWith("savePT"))
                            {
                                tempStr = tempStr.Remove(0, 6);
                                MessageBox.Show(tempStr);
                            }
                            else if (tempStr.StartsWith("success") || tempStr.StartsWith("failure"))
                            {
                                tempStr = tempStr.Remove(0, 8); //去掉success,或failure,

                                MessageBox.Show(tempStr);
                            }
                            else if (tempStr.StartsWith("%SFD"))
                            {
                                tempStr = tempStr.Remove(0, 4);
                                MessageBox.Show("now Correction Factor is: " + tempStr);
                            }
                            else if (tempStr.StartsWith("%RFD"))
                            {
                                tempStr = tempStr.Remove(0, 4);
                                //this.dapFactor.Text = tempStr.ToString();

                                double tempCorrectionFactor = double.Parse(tempStr);

                                tempCorrectionFactor = 1 / (1 + 0.0451) * tempCorrectionFactor;

                                Application.DoEvents();//使textbook 立即刷新刚赋的值
                            }
                            else if (tempStr.Equals("TST"))
                            {
                                string str = "[" + DateTime.Now.ToString() + "] received data from " + socketSend.RemoteEndPoint + ":" + tempStr;
                                this.clientLog.Invoke(receiveCallBack, str);
                                MessageBox.Show("stability test successfully");
                            }

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("receive data from DAP_emulator failurely：\n" + ex.ToString());
            }
        }


        /// <summary>
        /// a common method for processing and sending data 
        /// </summary>
        /// <param name="dataDicts"></param>
        /// <param name="logStr"></param>
        /// <param name="buffer"></param>
        public void DataOperation(Dictionary<string, string> dataDicts, string logStr, byte[] buffer)
        {
            String json = JsonConvert.SerializeObject(dataDicts, Formatting.Indented);
            buffer = Encoding.Default.GetBytes(json);
            int charLen = socketSend.Send(buffer);

            string tempMsg = "[" + DateTime.Now.ToString() + "]" + logStr + socketSend.RemoteEndPoint;
            this.clientLog.Invoke(receiveCallBack, tempMsg); //显示在日志框里
        }


        /// <summary>
        /// 向 DAP模拟器 发送数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendMsg(object sender, EventArgs e)
        {
            try
            {
                string command = "TST";

                string tempText = this.msgText.Text;
                if (!tempText.Equals(""))
                {
                    command = tempText;
                }

                Dictionary<string, string> paraDict = new Dictionary<string, string>
                {
                    {"data", command},
                };

                dapProtocol = new DAPProtocol(command, paraDict);

                string json = JsonConvert.SerializeObject(dapProtocol, Formatting.Indented);

                byte[] buffer = new byte[2048];
                buffer = Encoding.Default.GetBytes(json);//发送json字符串
                int receive = socketSend.Send(buffer);

                string tempMsg = "[" + DateTime.Now.ToString() + "] client sended message to " + socketSend.RemoteEndPoint + ":" + paraDict["data"];
                clientLog.Invoke(receiveCallBack, tempMsg); //显示在日志框里
            }
            catch (Exception ex)
            {
                MessageBox.Show("data send exception：\n" + ex.Message);
            }
        }

        private void DoCalibration(object sender, EventArgs e)
        {
            //double DAP_corr_factor = 1.00;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReadDAP(object sender, EventArgs e)
        {
            try
            {
                string msgStr = "%RFD";

                Dictionary<string, string> paraDict = new Dictionary<string, string>
                {
                    {"data", msgStr}
                };

                string json = JsonConvert.SerializeObject(paraDict, Formatting.Indented);

                byte[] buffer = new byte[2048];
                buffer = Encoding.Default.GetBytes(json);
                int receive = socketSend.Send(buffer);

                string tempMsg = "[" + DateTime.Now.ToString() + "] client sended message to " + socketSend.RemoteEndPoint + ":" + msgStr;
                clientLog.Invoke(receiveCallBack, tempMsg); //显示在日志框里
            }
            catch (Exception ex)
            {
                MessageBox.Show("ReadDAP exception：\n" + ex.Message);
            }
        }

        private void client_Load(object sender, EventArgs e)
        {

        }

        private void CalibrationValidate(object sender, EventArgs e)
        {
            //SendJsonData("Validate");

            double factorV = double.Parse(this.factor.Text.Trim());
            if ((factorV > 0.25) && (factorV < 2.50))
            {
                MessageBox.Show("inputed correction value is within tolerance");
            }
            else
            {
                MessageBox.Show("inputed correction value is not within tolerance");
            }
        }

        private void Reset(object sender, EventArgs e)
        {
            SendJsonData("reset");
        }

        private void LoadData(object sender, EventArgs e)
        {
            try
            {
                string command = "LoadAllData";

                Dictionary<string, string> commDict = new Dictionary<string, string>
                {
                    {"data", command}
                };

                string json = JsonConvert.SerializeObject(commDict, Formatting.Indented);

                byte[] commBuffer = new byte[2048];
                commBuffer = Encoding.Default.GetBytes(json);
                int len = socketSend.Send(commBuffer);

                string tempMsg = "[" + DateTime.Now.ToString() + "] client sended message to " + socketSend.RemoteEndPoint + ":" + command;
                clientLog.Invoke(receiveCallBack, tempMsg); //显示在日志框里
            }
            catch (Exception ex)
            {
                MessageBox.Show("ReadDAP exception：\n" + ex.Message);
            }
        }

        private void SaveCorrectionFactor(object sender, EventArgs e)
        {
            if (this.factor.Text.Equals(""))
            {
                MessageBox.Show("the value of CorrectionFactor is empty");
            }
            else
            {
                try
                {
                    if (CorrectionFactor == -1.00)
                    {
                        MessageBox.Show("please click 'Read Data' button firstly !");
                    }
                    else
                    {
                        string tempCorrectioFactor = this.factor.Text;

                        //string command = "%SFD" + CorrectionFactor.ToString();
                        string msgStr = "%SFD" + tempCorrectioFactor;

                        Dictionary<string, string> paraDict = new Dictionary<string, string>
                        {
                            {"data", msgStr}
                        };

                        string json = JsonConvert.SerializeObject(paraDict, Formatting.Indented);

                        byte[] buffer = new byte[2048];
                        buffer = Encoding.Default.GetBytes(json);
                        int receive = socketSend.Send(buffer);

                        string tempMsg = "[" + DateTime.Now.ToString() + "] client sended message to " + socketSend.RemoteEndPoint + ":" + msgStr;
                        clientLog.Invoke(receiveCallBack, tempMsg); //显示在日志框里
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("data send exception：\n" + ex.Message);
                }
            }


        }

        private void SavePT(object sender, EventArgs e)
        {
            string pressure = this.pressure.Text.Trim();
            string T = this.T.Text.Trim();
            SendJsonData("savePT" + pressure + "," + T);
        }

        public void SendJsonData(string commamd)
        {
            try
            {
                string command = commamd;

                Dictionary<string, string> commDict = new Dictionary<string, string>
                {
                    {"data", command}
                };

                string json = JsonConvert.SerializeObject(commDict, Formatting.Indented);

                byte[] commBuffer = new byte[2048];
                commBuffer = Encoding.Default.GetBytes(json);
                int len = socketSend.Send(commBuffer);

                string tempMsg = "[" + DateTime.Now.ToString() + "] client sended message to " + socketSend.RemoteEndPoint + ":" + command;
                clientLog.Invoke(receiveCallBack, tempMsg); //显示在日志框里
            }
            catch (Exception ex)
            {
                MessageBox.Show("ReadDAP exception：\n" + ex.Message);
            }
        }
    }
}
