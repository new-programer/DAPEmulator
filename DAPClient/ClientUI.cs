﻿using System;
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
    public partial class ClientUI : Form
    {
        //Fields
        UpdateUIDelegate updateUIDelegate;
        ClientCommunication clientCommunication;
        //Properties
        public string IpAddress 
        {
            get
            {
                return this.ipAddress.Text.Trim();
            }
            set
            {
                ipAddress.Text = value;
            }
        }

        public string Port 
        {
            get 
            {
                return this.port.Text.Trim();
            }
            set 
            {
                port.Text = value; 
            } 
        }

        public string ClientLog
        {
            get
            {
                return this.clientLog.Text.Trim();
            }
            set
            {
                clientLog.AppendText(value);
            }
        }

        public string MsgBox
        {
            get
            {
                return this.msgText.Text.Trim();
            }
            set
            {
                msgText.Text = value;
            }
        }


















        public ClientUI()
        {
            /*click https://blog.csdn.net/weixin_44913038/article/details/102996655
             */
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;


            InitializeComponent();
        }

        //declare DAPProtocol
        DAPProtocol dapProtocol;
        string command;
        Dictionary<string, string> paraDict;
        


        private delegate void SetTextCallBack(string strValue);
        private SetTextCallBack setCallBack;

        private delegate void ReceiveMsgCallBack(string strMsg);
        private ReceiveMsgCallBack receiveCallBack;


        Socket commSocket;//Socket for creating connecing
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
            clientCommunication = new ClientCommunication();
            clientCommunication.UpdateUIDelegate = UpdateControl;//bind delegate
            clientCommunication.StartConnection(commSocket, this);
        }

        private void StartConnection(Socket listenSocket)
        {
            try
            {
                listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Parse(this.ipAddress.Text.Trim());
                int port = Convert.ToInt32(this.port.Text.Trim());
                listenSocket.Connect(ip, port);

                setCallBack = new SetTextCallBack(SetValue);
                receiveCallBack = new ReceiveMsgCallBack(SetValue);
                this.clientLog.Invoke(setCallBack, "[" + DateTime.Now.ToString() + "] connect successfully!");

                //create a new thread for communicating
                threadReceive = new Thread(new ParameterizedThreadStart(Receive));

                threadReceive.IsBackground = true;
                threadReceive.Start(listenSocket);
            }
            catch (Exception ex)
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
        public void Receive(object socketSend)
        {
            commSocket = socketSend as Socket;

            try
            {
                while (true)
                {
                    byte[] buffer = new byte[2048];

                    int count = commSocket.Receive(buffer);


                    if (count == 0)
                    {
                        break;
                    }
                    else
                    {
                        string jsonStr = Encoding.Default.GetString(buffer, 0, count);
                        dapProtocol = JsonConvert.DeserializeObject<DAPProtocol>(jsonStr);
                        command = dapProtocol.Head;
                        paraDict = dapProtocol.Body;


                        if (command.Equals(""))
                        {
                            MessageBox.Show("do not get the value of 'DAP_corr_fctor'");
                        }
                        else
                        {
                            switch (command)
                            {
                                case "LoadAllData":

                                    sw_version = paraDict["sw_version"];
                                    InternalSN = paraDict["InternalSN"];
                                    CorrectionFactor = double.Parse(paraDict["CorrectionFactor"]);
                                    AirPressure = int.Parse(paraDict["AirPressure"]);
                                    Temperature = int.Parse(paraDict["Temperature"]);

                                    version.Text = sw_version.Trim();
                                    sn.Text = InternalSN;
                                    factor.Text = CorrectionFactor.ToString();
                                    pressure.Text = AirPressure.ToString();
                                    T.Text = Temperature.ToString();
                                    Application.DoEvents();//refresh textbox

                                    break;

                                case "reset":
                                    sw_version = paraDict["sw_version"];
                                    InternalSN = paraDict["InternalSN"];
                                    CorrectionFactor = double.Parse(paraDict["CorrectionFactor"]);
                                    AirPressure = int.Parse(paraDict["AirPressure"]);
                                    Temperature = int.Parse(paraDict["Temperature"]);

                                    version.Text = sw_version.Trim();
                                    sn.Text = InternalSN;
                                    factor.Text = CorrectionFactor.ToString();
                                    pressure.Text = AirPressure.ToString();
                                    T.Text = Temperature.ToString();

                                    Application.DoEvents();

                                    break;

                                case "savePT":
                                    MessageBox.Show(paraDict["data"]);
                                    break;

                                case "%RFD":
                                    double tempCorrectionFactor = double.Parse(paraDict["data"]);
                                    tempCorrectionFactor = 1 / (1 + 0.0451) * tempCorrectionFactor;
                                    Application.DoEvents();
                                    break;

                                case "%SFD":
                                    MessageBox.Show("now Correction Factor is: " + paraDict["data"]);
                                    break;

                                case "TST":
                                    MessageBox.Show("stability test successfully");
                                    break;

                                default:
                                    MessageBox.Show("invalid command");
                                    break;
                            }

                            string log = "[" + DateTime.Now.ToString() + "] from " + commSocket.RemoteEndPoint + ":" + command;
                            clientLog.Invoke(receiveCallBack, log);
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
            int charLen = commSocket.Send(buffer);

            string tempMsg = "[" + DateTime.Now.ToString() + "]" + logStr + commSocket.RemoteEndPoint;
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

                paraDict = new Dictionary<string, string>
                {
                    {"data", command},
                };

                dapProtocol = new DAPProtocol(command, paraDict);

                string json = JsonConvert.SerializeObject(dapProtocol, Formatting.Indented);

                byte[] buffer = new byte[2048];
                buffer = Encoding.Default.GetBytes(json);
                int receive = commSocket.Send(buffer);

                string tempMsg = "[" + DateTime.Now.ToString() + "] to " + commSocket.RemoteEndPoint + ":" + paraDict["data"];
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
                string command = "%RFD";

                paraDict = new Dictionary<string, string>
                {
                    {"data", command}
                };

                dapProtocol = new DAPProtocol(command, paraDict);

                string json = JsonConvert.SerializeObject(dapProtocol, Formatting.Indented);


                byte[] buffer = new byte[2048];
                buffer = Encoding.Default.GetBytes(json);
                int receive = commSocket.Send(buffer);

                string tempMsg = "[" + DateTime.Now.ToString() + "] to " + commSocket.RemoteEndPoint + ":" + command;
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

                paraDict = new Dictionary<string, string>
                {
                    {"data", command}
                };

                dapProtocol = new DAPProtocol(command, paraDict);

                string json = JsonConvert.SerializeObject(dapProtocol, Formatting.Indented);

                byte[] commBuffer = new byte[2048];
                commBuffer = Encoding.Default.GetBytes(json);
                int len = commSocket.Send(commBuffer);

                string tempMsg = "[" + DateTime.Now.ToString() + "] to " + commSocket.RemoteEndPoint + ":" + command;
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
                        string command = tempCorrectioFactor;

                        paraDict = new Dictionary<string, string>
                        {
                            {"data", command}
                        };

                        dapProtocol = new DAPProtocol("%SFD", paraDict);
                        string json = JsonConvert.SerializeObject(dapProtocol, Formatting.Indented);

                        string tempMsg = "[" + DateTime.Now.ToString() + "] to " + commSocket.RemoteEndPoint + ":" + dapProtocol.Head + command;
                        clientLog.Invoke(receiveCallBack, tempMsg);

                        byte[] buffer = new byte[2048];
                        buffer = Encoding.Default.GetBytes(json);
                        int receive = commSocket.Send(buffer);

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
            SendJsonData("savePT" + "?" + pressure + "&" + T);
        }

        public void SendJsonData(string dataContent)
        {
            try
            {
                if (dataContent.Equals("reset"))
                {
                    paraDict = new Dictionary<string, string>
                    {
                        {"data", dataContent},
                    };

                    dapProtocol = new DAPProtocol(dataContent, paraDict);
                }
                else 
                {
                    string[] commandStr = dataContent.Split('?');
                    string[] dataStr = commandStr[1].Split('&');

                    paraDict = new Dictionary<string, string>
                    {
                        {"AirPressure", dataStr[0]},
                        {"Temperature", dataStr[1]}
                    };

                    dapProtocol = new DAPProtocol(commandStr[0], paraDict);
                }


                string json = JsonConvert.SerializeObject(dapProtocol, Formatting.Indented);

                byte[] commBuffer = new byte[2048];
                commBuffer = Encoding.Default.GetBytes(json);
                int len = commSocket.Send(commBuffer);

                string tempMsg = "[" + DateTime.Now.ToString() + "] to " + commSocket.RemoteEndPoint + ":" + command;
                clientLog.Invoke(receiveCallBack, tempMsg);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ReadDAP exception：\n" + ex.Message);
            }
        }


        //binded by updateUIControlDelegate
        public void UpdateControl(string logContent, int flag, string endPoint)
        {
            string temp = "";
            switch (flag)
            {
                case 1:
                    temp = " from ";
                    break;
                case 0:
                    temp = " to ";
                    break;
                default://flag == -1
                    temp = " connect to ";
                    break;
            }

            logContent = "[" + DateTime.Now.ToString() + "]" + temp + endPoint + ":" + logContent;

            clientLog.AppendText(logContent + "\n");
            Application.DoEvents();
        }
    }
}
