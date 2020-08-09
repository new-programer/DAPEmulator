using DAPClibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DAPClient
{
    class ClientCommunication
    {
        //properties
        ClientUI client;
        DAPProtocol dapProtocol;
        UpdateUIDelegate updateUIDelegate;

        public UpdateUIDelegate UpdateUIDelegate { get => updateUIDelegate; set => updateUIDelegate = value; }

        public ClientCommunication()
        { 
        }


        //method
        public void StartConnection(Socket listenSocket, ClientUI clientUI)
        {
            try
            {
                listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Parse(clientUI.IpAddress);
                int port = Convert.ToInt32(clientUI.Port);
                listenSocket.Connect(ip, port);

                if (updateUIDelegate != null)
                {
                    updateUIDelegate("connect successfully", -1, listenSocket.RemoteEndPoint.ToString());
                }

                //create a new thread for communicating
                Thread threadReceive = new Thread(new ParameterizedThreadStart(clientUI.Receive));
                threadReceive.IsBackground = true;
                threadReceive.Start(listenSocket);
            }
            catch
            {
            }
        }

        /// <summary>
        /// receive data from DAPEmulator
        /// </summary>
/*        public void Receive(object socketSend)
        {
            Socket commSocket = socketSend as Socket;
            string command;
            Dictionary<string, string> paraDict;

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
        }*/
    }
}
