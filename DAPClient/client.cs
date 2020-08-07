using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using Newtonsoft.Json;
using DAPClibrary;

namespace DAPClient
{
    public partial class client : Form
    {
        public client()
        {
            /*click https://blog.csdn.net/weixin_44913038/article/details/102996655
             */
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;


            InitializeComponent();
        }

        DAPProtocol dapProtocol;


        private delegate void SetTextCallBack(string strValue);
        private SetTextCallBack setCallBack;

        private delegate void ReceiveMsgCallBack(string strMsg);
        private ReceiveMsgCallBack receiveCallBack;

        
        Socket socketSend;//Socket for creating connecing
        Thread threadReceive; //Socket for receiving data from DAPEmulator

        string sw_version = "";
        string InternalSN = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0)).ToString();
        double CorrectionFactor = -1.00;
        int AirPressure = 1013;
        int Temperature = 20;

        /// <summary>
        /// create an socket connection
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

                setCallBack = new SetTextCallBack(SetValue);
                receiveCallBack = new ReceiveMsgCallBack(SetValue);
                this.clientLog.Invoke(setCallBack, "[" + DateTime.Now.ToString() + "] connect successfully!");

                //create a new thread for communicating
                threadReceive = new Thread(new ThreadStart(Receive));

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
        /// receive data from DAPEmulator
        /// </summary>
        private void Receive()
        {
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[2048];

                    int count = socketSend.Receive(buffer);


                    if (count == 0)
                    {
                        break;
                    }
                    else
                    {
                        string jsonStr = Encoding.Default.GetString(buffer, 0, count);
                        Dictionary<string, string> paraDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonStr);

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
                                    tempStr = tempStr.Remove(0, 7);
                                }
                                else if (tempStr.StartsWith("reset"))
                                {
                                    tempStr = tempStr.Remove(0, 5);
                                    Dictionary<string, string> allData = JsonConvert.DeserializeObject<Dictionary<string, string>>(tempStr);

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
                                    tempStr = tempStr.Remove(0, 8);

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

                                    Application.DoEvents();
                                }
                                else if (tempStr.Equals("TST"))
                                {
                                    string str = "[" + DateTime.Now.ToString() + "] from " + socketSend.RemoteEndPoint + ":" + tempStr;
                                    this.clientLog.Invoke(receiveCallBack, str);
                                    MessageBox.Show("stability test successfully");
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error From DAP_emulator：\n" + ex.ToString());
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
            this.clientLog.Invoke(receiveCallBack, tempMsg);
        }


        /// <summary>
        /// send data to DAPEmulator
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
                buffer = Encoding.Default.GetBytes(json);
                int receive = socketSend.Send(buffer);

                string tempMsg = "[" + DateTime.Now.ToString() + "] to " + socketSend.RemoteEndPoint + ":" + paraDict["data"];
                clientLog.Invoke(receiveCallBack, tempMsg); 
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

                string tempMsg = "[" + DateTime.Now.ToString() + "] to " + socketSend.RemoteEndPoint + ":" + msgStr;
                clientLog.Invoke(receiveCallBack, tempMsg);

            }
            catch (Exception ex)
            {
                MessageBox.Show("ReadDAP exception：\n" + ex.Message);
            }
        }

        private void client_Load(object sender, EventArgs e)
        {
            this.ipAddress.Text = new Utils().GetAddressIP();
            this.port.Text = "1111";
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

                string tempMsg = "[" + DateTime.Now.ToString() + "] to " + socketSend.RemoteEndPoint + ":" + command;
                clientLog.Invoke(receiveCallBack, tempMsg); 
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

                        string tempMsg = "[" + DateTime.Now.ToString() + "] to " + socketSend.RemoteEndPoint + ":" + msgStr;
                        clientLog.Invoke(receiveCallBack, tempMsg); 
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

                string tempMsg = "[" + DateTime.Now.ToString() + "] to " + socketSend.RemoteEndPoint + ":" + command;
                clientLog.Invoke(receiveCallBack, tempMsg); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("ReadDAP exception：\n" + ex.Message);
            }
        }
    }
}
