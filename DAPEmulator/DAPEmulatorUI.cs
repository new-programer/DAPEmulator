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
using DAPClibrary;

namespace DAPEmulator
{
    /*define and declare delegate*/
    public delegate void SetTextValueCallBack(string strValue);
    public delegate void ReceiveMsgCallBack(string strReceive);
    public delegate void FactorText(string value);



    public partial class DAPEmulatorUI : Form
    {
        EndPoint point;
        DAPEmulatorUI emulatorUI;
        DAPProtocol dapProtocol;
        Utils utils;

        /*create an instance of class named DAPEmulatorComm */
        DAPEmulatorComm dapEmulatorComm;
        DAPParameters dapParameters = new DAPParameters();

        /*declare delegate*/ 
        SetTextValueCallBack setCallBack;
        ReceiveMsgCallBack receiveCallBack;
        DelegateCollection.updateUIControlDelegate UpdateUIDelegate;
        DelegateCollection.updateUIControlDelegate threadDelegate;


         //declare sockets for listening and sending packets
        Socket commSocket;
        Socket listenSocket;

        //declare threads for listening connection created by client and accepting packets from client 
        Thread serverListenThread;
        Thread commThread;


        /// <summary>
        /// Constructor
        /// </summary>
        public DAPEmulatorUI()
        {
            //ignore cross-thread check
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;

            InitializeComponent();

            this.version.Text = dapParameters.sw_version;
            this.sn.Text = dapParameters.InternalSN;
            this.factor.Text = dapParameters.CorrectionFactor.ToString();
            //bind method
            UpdateUIDelegate = new DelegateCollection.updateUIControlDelegate(this.UpdateControl);
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

        private void FactorText(string value)
        {
            this.factor.Text = value;
        }

        /// <summary>
        /// start DAP emulator and listen client which try to connect with the emulator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0017:Simplify object initialization", Justification = "<Pending>")]

        private void StartListen(object sender, EventArgs e)
        {
            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //IPAddress ip = IPAddress.Parse(this.ip.Text.Trim());
            IPAddress DAP_ip = IPAddress.Parse(this.ip.Text.Trim());
            int port = CheckPortValue(this.port.Text.Trim()); //default value 

            IPEndPoint endPoint = new IPEndPoint(DAP_ip, port);

            //call listen method
            dapEmulatorComm = new DAPEmulatorComm(this, listenSocket);
            //bind delegate
            dapEmulatorComm.threadDelegate = new DelegateCollection.updateUIControlDelegate(UpdateControl);
            Thread subThread = new Thread(new ParameterizedThreadStart(dapEmulatorComm.Listen));
            subThread.IsBackground = true;
            subThread.Start(endPoint);

        }

        /*---------------------------method-------------------------------*/
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="port"></param>
        public void Listen(object endPoint)
        {
            EndPoint point = endPoint as IPEndPoint;
            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            listenSocket.Bind(point);
            listenSocket.Listen(10);

            //create an instance of thread for listening
            //serverListenThread = new Thread(new ParameterizedThreadStart(StartListenMethod));
            //serverListenThread.IsBackground = true;
            //serverListenThread.Start(listenSocket);


            StartListenMethod(listenSocket);

        }

        /// <summary>
        /// method for waiting for connection from client and communicating with client
        /// <param name="obj"></param>
        private void StartListenMethod(object obj)
        {
            Socket listenSocket = obj as Socket;
            //emulatorUI.DisplayLog("listen successfully!", true, point.ToString());

            if (UpdateUIDelegate != null)
            {
                UpdateUIDelegate("listen successfully!", true, point.ToString());
            }

            while (true)
            {
                commSocket = listenSocket.Accept();

                string ipStr = commSocket.RemoteEndPoint.ToString();
                emulatorUI.ip.Text = ipStr;
                Application.DoEvents();

                UpdateControl("connect successfully!", true, ipStr);


                //create a thread for receiving packet from client
                Thread threadReceive = new Thread(new ParameterizedThreadStart(RunThread));
                threadReceive.IsBackground = true;

                threadReceive.Start(UpdateUIDelegate);
            }
        }

        /// <summary> 
        ///  method for receiving and processing packets from client in a loop;
        ///  there are different prefixes in data string:
        ///  "%SFD", it is used to write Correction Factor of DAP to DAP Emulator;
        ///  
        /// </summary>
        /// <param name="obj"></param>
        public void RunThread(object UpdataUIDelegate)
        {
            threadDelegate = UpdataUIDelegate as DelegateCollection.updateUIControlDelegate;
            Dictionary<string, string> factDict;

            string jsonStr = string.Empty;
            string command = string.Empty;
            string dataContent = string.Empty;
            string logStr = string.Empty;

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
                    jsonStr = Encoding.Default.GetString(buffer, 0, count);
                    dapProtocol = JsonConvert.DeserializeObject<DAPProtocol>(jsonStr);
                    command = dapProtocol.Head;
                    switch (command)
                    {
                        case "TST":
                            if (threadDelegate != null)
                            {
                                threadDelegate(command, true, commSocket.RemoteEndPoint.ToString());
                            }

                            dapProtocol = new DAPProtocol("TST", null);
                            utils.JsonDataOperation(dapProtocol, buffer);
                            commSocket.Send(buffer);

                            if (threadDelegate != null)
                            {
                                threadDelegate(command, false, commSocket.RemoteEndPoint.ToString());
                            }
                            break;

                        case "%RFD":
                            if (threadDelegate != null)
                            {
                                threadDelegate(command, true, commSocket.RemoteEndPoint.ToString());
                            }

                            dataContent = dapParameters.CorrectionFactor.ToString();
                            factDict = new Dictionary<string, string>
                            {
                                {"data", dataContent}
                            };
                            dapProtocol = new DAPProtocol("%RFD", factDict);
                            utils.JsonDataOperation(factDict, buffer);
                            commSocket.Send(buffer);

                            if (threadDelegate != null)
                            {
                                threadDelegate(command, false, commSocket.RemoteEndPoint.ToString());
                            }
                            break;

                        case "%SFD":
                            if (threadDelegate != null)
                            {
                                threadDelegate(command, true, commSocket.RemoteEndPoint.ToString());
                            }

                            double tempCorrectionFactor = double.Parse(dapProtocol.Body["CorrectionFactor"]);
                            if ((int)(tempCorrectionFactor * 100) != (int)(dapParameters.CorrectionFactor * 100))
                            {
                                dapParameters.CorrectionFactor = tempCorrectionFactor;
                                emulatorUI.factor.Text = dapParameters.CorrectionFactor.ToString();
                                Application.DoEvents();

                                dataContent = "Correction factor save successfully";
                            }
                            else
                            {
                                dataContent = "Correction factor does not change";
                            }
                            factDict = new Dictionary<string, string>
                            {
                                {"data", dataContent}
                            };
                            dapProtocol = new DAPProtocol("%SFD", factDict);
                            utils.JsonDataOperation(factDict, buffer);
                            commSocket.Send(buffer);

                            if (threadDelegate != null)
                            {
                                threadDelegate(command, false, commSocket.RemoteEndPoint.ToString());
                            }
                            break;

                        case "LoadAllData":
                            if (threadDelegate != null)
                            {
                                threadDelegate(command, true, commSocket.RemoteEndPoint.ToString());
                            }

                            factDict = new Dictionary<string, string>
                            {
                                {"sw_version", dapParameters.sw_version},
                                {"InternalSN", dapParameters.InternalSN},
                                {"CorrectionFactor", dapParameters.CorrectionFactor.ToString()},
                                {"AirPressure", dapParameters.AirPressure.ToString()},
                                {"Temperature", dapParameters.Temperature.ToString()},
                            };
                            dapProtocol = new DAPProtocol("%RFD", factDict);
                            utils.JsonDataOperation(factDict, buffer);
                            commSocket.Send(buffer);

                            if (threadDelegate != null)
                            {
                                threadDelegate(command, false, commSocket.RemoteEndPoint.ToString());
                            }
                            break;

                        case "reset":
                            if (threadDelegate != null)
                            {
                                threadDelegate(command, true, commSocket.RemoteEndPoint.ToString());
                            }

                            factDict = new Dictionary<string, string>
                            {
                                {"sw_version", dapParameters.sw_version},
                                {"InternalSN", dapParameters.InternalSN},
                                {"CorrectionFactor", dapParameters.CorrectionFactor.ToString()},
                                {"AirPressure", dapParameters.AirPressure.ToString()},
                                {"Temperature", dapParameters.Temperature.ToString()},
                            };
                            dapProtocol = new DAPProtocol("%RFD", factDict);
                            utils.JsonDataOperation(factDict, buffer);
                            commSocket.Send(buffer);

                            if (threadDelegate != null)
                            {
                                threadDelegate(command, false, commSocket.RemoteEndPoint.ToString());
                            }

                            break;

                        case "savePT":
                            if (threadDelegate != null)
                            {
                                threadDelegate(command, true, commSocket.RemoteEndPoint.ToString());
                            }

                            int tempAirPressure = int.Parse(dapProtocol.Body["AirPressure"]);
                            int tempTemperature = int.Parse(dapProtocol.Body["Temperature"]);

                            if ((tempAirPressure != dapParameters.AirPressure) || (tempTemperature != dapParameters.Temperature))
                            {
                                dapParameters.AirPressure = tempAirPressure;
                                dapParameters.Temperature = tempTemperature;

                                dataContent = "save successfully";
                            }
                            else
                            {
                                dataContent = "the value of pressure or temperature does not change";
                            }

                            factDict = new Dictionary<string, string>
                            {
                                {"data",dataContent}
                            };
                            dapProtocol = new DAPProtocol("%RFD", factDict);
                            utils.JsonDataOperation(factDict, buffer);
                            commSocket.Send(buffer);

                            if (threadDelegate != null)
                            {
                                threadDelegate(command, false, commSocket.RemoteEndPoint.ToString());
                            }
                            break;
                        case "Validate":
                            if (threadDelegate != null)
                            {
                                threadDelegate(command, true, commSocket.RemoteEndPoint.ToString());
                            }

                            double tempFactor = double.Parse(dapProtocol.Body["CorrectionFactor"]);
                            dataContent = "success,Validate Successfully";
                            if ((tempFactor < 2.50) && (tempFactor > 0.25))
                            {
                                dataContent = "failure,sorry, inputed correction factor is not within tolerance";
                            }

                            factDict = new Dictionary<string, string>
                            {
                                {"data", dataContent}
                            };

                            dapProtocol = new DAPProtocol("%RFD", factDict);
                            utils.JsonDataOperation(factDict, buffer);
                            commSocket.Send(buffer);

                            if (threadDelegate != null)
                            {
                                threadDelegate(command, false, commSocket.RemoteEndPoint.ToString());
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public void OnConnected(Object state)
        {
            List<string> logList = state as List<string>;
            if ("true".Equals(logList[1]))
            {
                DisplayLog(logList[0], true, logList[2]);
            }
            else
            {
                DisplayLog(logList[0], false, logList[2]);
            }

        }

        /// <summary>
        /// record logs in log box
        /// </summary>
        /// <param name="logContent"></param>
        /// <param name="flag">flag: true stand for packet from client,false stand for packet to client</param>
        /// <param name="commSocket">communicate socket</param>
        public string DisplayLog(string logContent, bool flag, string pointStr)
        {
            string temp = " from ";
            if (!flag) 
            {
                temp = "to";
            };

            logContent = "[" + DateTime.Now.ToString() + "]" + temp + pointStr + ":" + logContent; 
            msgReceive.AppendText(logContent + "\n");
            return logContent;
        }

        //binded by updateUIControlDelegate
        public void UpdateControl(string logContent, bool flag, string pointStr)
        {
            string temp = " from ";
            if (!flag)
            {
                temp = "to";
            };

            logContent = "[" + DateTime.Now.ToString() + "]" + temp + pointStr + ":" + logContent;

            msgReceive.AppendText(logContent + "\n");
            Application.DoEvents();
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
            //msgReceive.Invoke(receiveCallBack, tempMsg);

           // DisplayLog(tempMsg);
        }


        /*---------properties--------*/

        /// <summary>
        /// stop listen event, close sockets and end threads
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopListen(object sender, EventArgs e)
        {
            listenSocket.Close();
            commSocket.Close();

            serverListenThread.Abort();
            commThread.Abort();
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

        private void msgReceive_TextChanged(object sender, EventArgs e)
        {

        }

        private void DAPEmulatorUI_Load(object sender, EventArgs e)
        {
            this.ip.Text = GetAddressIP();
            this.port.Text = "1111";
            Application.DoEvents();
        }
    }
}