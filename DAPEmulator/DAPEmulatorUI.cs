using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Management.Instrumentation;

namespace DAPEmulator
{
    /*define and declare delegate*/
    public delegate void SetTextValueCallBack(string strValue);
    public delegate void ReceiveMsgCallBack(string strReceive);
    public delegate void SendFileCallBack(byte[] bf);

    public partial class DAPEmulatorUI : Form
    {
        /*create an instance of class named DAPEmulatorComm */
        DAPEmulatorComm dapEmulatorComm = new DAPEmulatorComm();

        /*define some parameters related of DAP emulator, these parameters all are maybe changed.
         * s/w version, the version of DAP meter
         * Internal Serial Number,
         * DAP Correction Factor, default value is 1.00
         * Air Pressure, default value is 1013Pa
         * Temperature, default value is 20`C / 293.15K
         */
        public readonly string sw_version = "120-131 ETH";
        public readonly string InternalSN = "DAP20200714";
        public double CorrectionFactor = 1.00;
        public int AirPressure = 1013;
        public int Temperature = 20;
        //define some parameters that are used to save default value and just readed only, 
        public readonly string sw_version_factory = "120-131 ETH";
        public readonly string InternalSN_factory = "DAP20200714";
        public readonly double CorrectionFactor_factory = 1.00;
        public readonly int AirPressure_factory = 1013;
        public readonly int Temperature_factory = 20;


        /*declare delegate*/ 
        SetTextValueCallBack setCallBack;
        ReceiveMsgCallBack receiveCallBack;


        //declare sockets for listening and sending packets
        Socket socketSend;
        Socket socketWatch;

        //declare threads for listening connection created by client and accepting packets from client 
        Thread serverListenThread;
        Thread packetReceiveThread;


        /// <summary>
        /// Constructor
        /// </summary>
        public DAPEmulatorUI()
        {
            //ignore cross-thread check
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;

            InitializeComponent();

            this.version.Text = sw_version;
            this.sn.Text = InternalSN;
            this.factor.Text = CorrectionFactor.ToString();
        }


        /// <summary>
        /// the method to be executed by delegate.
        /// </summary>
        /// <param name="strValue"></param>
        private void SetTextValue(string strValue)
        {
            this.msgReceive.AppendText(strValue + "\n");
        }


        private void ReceiveMsg(string strMsg)
        {
            this.msgReceive.AppendText(strMsg + "\n");
        }


        /// <summary>
        /// start DAP emulator and listen client which try to connect with the emulator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0017:Simplify object initialization", Justification = "<Pending>")]

        private void StartListen(object sender, EventArgs e)
        {
            socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //IPAddress ip = IPAddress.Parse(this.ip.Text.Trim());
            IPAddress DAP_ip = IPAddress.Parse(GetAddressIP());

            this.ip.Text = GetAddressIP();
            Application.DoEvents();

            int port = CheckPortValue(this.port.Text.Trim()); //default value 
            this.port.Text = port.ToString();
            Application.DoEvents();

            dapEmulatorComm.Listen(socketWatch, port);


            IPEndPoint point = new IPEndPoint(DAP_ip, port);
            socketWatch.Bind(point);
            this.msgReceive.AppendText("[" + DateTime.Now.ToString() + "] listen successfully!" + "\n");
            socketWatch.Listen(10);

            setCallBack = new SetTextValueCallBack(SetTextValue);
            receiveCallBack = new ReceiveMsgCallBack(ReceiveMsg);

            //create an instance of thread for listening
            serverListenThread = new Thread(new ParameterizedThreadStart(StartListenMethod));
            serverListenThread.IsBackground = true;
            serverListenThread.Start(socketWatch);
        }

        /// <summary>
        /// method for waiting for connection from client and communicating with client
        /// <param name="obj"></param>
        private void StartListenMethod(object obj)
        {
            Socket socketWatch = obj as Socket;

            while (true) 
            {
                socketSend = socketWatch.Accept();

                string ipStr = socketSend.RemoteEndPoint.ToString();

                string strMsg = "[" + DateTime.Now.ToString() + "] client(" + socketSend.RemoteEndPoint + ") connect successfully!";

                msgReceive.Invoke(setCallBack, strMsg);

                //create a thread for receiving packet from client
                Thread threadReceive = new Thread(new ParameterizedThreadStart(Receive));
                threadReceive.IsBackground = true;
                threadReceive.Start(socketSend);
            }
        }

        /// <summary> 
        ///  method for receiving and processing packets from client in a loop;
        ///  there are different prefixes in data string:
        ///  "%SFD", it is used to write Correction Factor of DAP to DAP Emulator;
        ///  
        /// </summary>
        /// <param name="obj"></param>
        private void Receive(object obj)
        {
            Socket socketSend = obj as Socket;
            Dictionary<string, string> factDict;

            while (true) 
            {
                byte[] buffer = new byte[2048];

                int count = socketSend.Receive(buffer);

                string receiveMsgStr = "";
                string sendData = "";

                if (count == 0)
                {
                    break;
                }
                else
                {
                    string jsonStr = Encoding.Default.GetString(buffer, 0, count);
                    Dictionary<string, string> paraDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonStr);

                    string tempStr = paraDict["data"];
                    if (tempStr.Equals("LoadAllData"))
                    {
                        Dictionary<string, string> tempDict = new Dictionary<string, string>
                        {
                            {"sw_version", sw_version},
                            {"InternalSN", InternalSN},
                            {"CorrectionFactor", CorrectionFactor.ToString()},
                            {"AirPressure", AirPressure.ToString()},
                            {"Temperature", Temperature.ToString()},
                        };

                        string tempJson = JsonConvert.SerializeObject(tempDict, Formatting.Indented);

                        factDict = new Dictionary<string, string>
                        {
                            {"data", "allData" + tempJson}
                        };

                        String logStr = " DAPEmulatorUI sended all data to ";
                        DataOperation(factDict, logStr, buffer);
                        DisplayLog(logStr);
                    }
                    else if (tempStr.Equals("Validate"))
                    {
                        tempStr = tempStr.Remove(0, 8);
                        double tempCorrectionFactor = double.Parse(tempStr);
                        string temp = "success,Validate Successfully";

                        if ((tempCorrectionFactor < 2.50) && (tempCorrectionFactor > 0.25))
                        {
                            temp = "failure,sorry, inputed correction factor is not within tolerance";
                        }

                        factDict = new Dictionary<string, string>
                        {
                            {"data", temp}
                        };


                        string logStr = "DAPEmulatorUI Calibration Validate";
                        DataOperation(factDict, logStr, buffer);
                    }
                    else if (tempStr.StartsWith("savePT"))
                    {
                        tempStr = tempStr.Remove(0, 6);
                        string[] tempString = tempStr.Split(',');

                        int tempAirPressure = int.Parse(tempString[0]);
                        int tempTemperature = int.Parse(tempString[1]);

                        if ((tempAirPressure != AirPressure) || (tempTemperature != Temperature))
                        {
                            AirPressure = tempAirPressure;
                            Temperature = tempTemperature;

                            tempStr = "save successfully";
                        }
                        else
                        {
                            tempStr = "the value of pressure or temperature does not change";
                        }

                        factDict = new Dictionary<string, string>
                        {
                            {"data", "savePT" + tempStr}
                        };

                        string logStr = " DAPEmulatorUI savePT ----> ";


                        DataOperation(factDict, logStr, buffer);

                        DisplayLog(logStr);
                    }
                    else if (tempStr.Equals("reset"))
                    {
                        Dictionary<string, string> tempDict = new Dictionary<string, string>
                        {
                            {"sw_version", sw_version_factory},
                            {"InternalSN", InternalSN_factory},
                            {"CorrectionFactor", CorrectionFactor_factory.ToString()},
                            {"AirPressure", AirPressure_factory.ToString()},
                            {"Temperature", Temperature_factory.ToString()},
                        };

                        string tempJson = JsonConvert.SerializeObject(tempDict, Formatting.Indented);
                        factDict = new Dictionary<string, string>
                        {
                            {"data", "reset" + tempJson}
                        };

                        string logStr = " DAPEmulatorUI sended all reset data to ";
                        DataOperation(factDict, logStr, buffer);

                        this.factor.Text = CorrectionFactor_factory.ToString();
                        Application.DoEvents();
                    }
                    else if (tempStr.StartsWith("%SFD"))  
                    {
                        //remove prefix contained "%SFD" of tempStr
                        double tempCorrectionFactor = double.Parse(tempStr.Remove(0, 4));


                        if ((int)(tempCorrectionFactor * 100) != (int)(CorrectionFactor * 100))
                        {
                            CorrectionFactor = tempCorrectionFactor;
                            this.factor.Text = CorrectionFactor.ToString();
                            Application.DoEvents();

                            tempStr = "Correction factor save successfully";
                        }
                        else
                        {
                            tempStr = "Correction factor does not change";
                        }

                        factDict = new Dictionary<string, string>
                        {
                            {"data", "%SFD" + tempStr}
                        };

                        string logStr = " " + tempStr + " ";
                        DataOperation(factDict, logStr, buffer);

                        string json = JsonConvert.SerializeObject(paraDict, Formatting.Indented);

                        byte[] rfdBuffer = new byte[2048];
                        rfdBuffer = Encoding.Default.GetBytes(json);
                        int receive = socketSend.Send(rfdBuffer);


                    }
                    else if (tempStr.StartsWith("%RFD"))
                    {
                        tempStr = "%RFD" + CorrectionFactor.ToString();

                        factDict = new Dictionary<string, string>
                        {
                            {"data", tempStr}
                        };

                        string json = JsonConvert.SerializeObject(factDict, Formatting.Indented);

                        byte[] rfdBuffer = new byte[2048];
                        rfdBuffer = Encoding.Default.GetBytes(json);
                        int receive = socketSend.Send(rfdBuffer);

                        msgReceive.Invoke(receiveCallBack, tempStr);
                    }
                    else if (tempStr.Equals("TST"))//"TST" command for testing tability of DAP meter
                    {
                        tempStr = tempStr.Trim();
                        factDict = new Dictionary<string, string>
                        {
                            {"data", tempStr}
                        };

                        receiveMsgStr = "[" + DateTime.Now.ToString() + "] message from " + socketSend.RemoteEndPoint + ":" + tempStr;
                        msgReceive.Invoke(receiveCallBack, receiveMsgStr); 
                        string json = JsonConvert.SerializeObject(factDict, Formatting.Indented);
                        byte[] sendBuffer = Encoding.Default.GetBytes(json);

                        try
                        {
                            socketSend.Send(sendBuffer);

                            string tempMsg = "DAPEmulatorUI sended message to " + socketSend.RemoteEndPoint + ":" + factDict["data"];
                            //msgReceive.Invoke(receiveCallBack, tempMsg);
                            DisplayLog(tempMsg);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("data send exception: \n" + ex.Message);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// record logs in log box
        /// </summary>
        /// <param name="logContent"></param>
        public void DisplayLog(string logContent)
        {
            logContent = "[" + DateTime.Now.ToString() + "] " + logContent; 
            msgReceive.AppendText(logContent + "\n");
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
            //msgReceive.Invoke(receiveCallBack, tempMsg);

            DisplayLog(tempMsg);
        }


        /*---------properties--------*/

        /// <summary>
        /// stop listen event, close sockets and end threads
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopListen(object sender, EventArgs e)
        {
            socketWatch.Close();
            socketSend.Close();

            serverListenThread.Abort();
            packetReceiveThread.Abort();
        }


        public void Receive(string command)
        { 

        }


        /// <summary>
        /// get local IP address
        /// </summary>
        /// <returns></returns>
        public string GetAddressIP()
        {
            string AddressIP = string.Empty;

            IPAddress[] iPAddresses = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            foreach (IPAddress _IPAddress in iPAddresses)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    AddressIP = _IPAddress.ToString();
                }
            }

            return AddressIP;
        }

        /// <summary>
        /// check if the port is valid
        /// </summary>
        /// <param name="portValue"></param>
        /// <returns></returns>
        public int CheckPortValue(String portValue)
        {
            int port = 1111;

            if (!portValue.Equals(""))
            {
                port = int.Parse(portValue);

                if (!(port >= 1024) && (port <= IPEndPoint.MaxPort))
                {
                    port = 1111;
                    MessageBox.Show("port value is not between 1024 and 65535, it is setted as '1111'");
                }
            }

            return port;
        }
    }
}