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
    public delegate void FactorText(string value);

    public partial class DAPEmulatorUI : Form
    {
        DAPEmulatorUI emulatorUI;

        /*create an instance of class named DAPEmulatorComm */
        DAPEmulatorComm dapEmulatorComm;
        DAPParameters dapParameters = new DAPParameters();

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

            this.version.Text = dapParameters.sw_version;
            this.sn.Text = dapParameters.InternalSN;
            this.factor.Text = dapParameters.CorrectionFactor.ToString();
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
            socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //IPAddress ip = IPAddress.Parse(this.ip.Text.Trim());
            IPAddress DAP_ip = IPAddress.Parse(GetAddressIP());
            int port = CheckPortValue(this.port.Text.Trim()); //default value 

            IPEndPoint endPoint = new IPEndPoint(DAP_ip, port);

            UpdateText(port);

            //call listen method
            dapEmulatorComm = new DAPEmulatorComm(this, socketWatch);
            //bind delegate
            dapEmulatorComm.threadDelegate = new DelegateCollection.updateUIControl(UpdateControl);
            Thread subThread = new Thread(new ParameterizedThreadStart(dapEmulatorComm.Listen));
            subThread.IsBackground = true;
            subThread.Start(endPoint);

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

        private void UpdateText(int port)
        {
            this.ip.Text = GetAddressIP();
            Application.DoEvents();

            this.port.Text = port.ToString();
            Application.DoEvents();
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
            int charLen = socketSend.Send(buffer);

            string tempMsg = "[" + DateTime.Now.ToString() + "]" + logStr + socketSend.RemoteEndPoint;
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

        private void msgReceive_TextChanged(object sender, EventArgs e)
        {

        }
    }
}