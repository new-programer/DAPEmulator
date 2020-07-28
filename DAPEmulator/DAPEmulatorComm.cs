using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DAPEmulator
{
    class DAPEmulatorComm
    {
        DAPEmulatorUI emulatorUI = new DAPEmulatorUI();
        Utils utils = new Utils();

        //declare sockets for listening and sending packets
        Socket commSocket;
        Socket listenSocket;

        //declare threads for listening connection created by client and accepting packets from client 
        Thread serverListenThread;
        Thread packetReceiveThread;

        public DAPEmulatorComm()
        { 
        }

        public void Listen(Object obj, int port) 
        {
            listenSocket = obj as Socket;

            IPAddress DAP_ip = IPAddress.Parse(utils.GetAddressIP());

            IPEndPoint point = new IPEndPoint(DAP_ip, port);
            listenSocket.Bind(point);

            emulatorUI.DisplayLog(" listen successfully!");

            listenSocket.Listen(10);

            //create an instance of thread for listening
            serverListenThread = new Thread(new ParameterizedThreadStart(StartListenMethod));
            serverListenThread.IsBackground = true;
            serverListenThread.Start(listenSocket);
        }

        /// <summary>
        /// method for waiting for connection from client and communicating with client
        /// <param name="obj"></param>
        private void StartListenMethod(object obj)
        {
            Socket listenSocket = obj as Socket;

            while (true)
            {
                commSocket = listenSocket.Accept();

                string ipStr = commSocket.RemoteEndPoint.ToString();

                emulatorUI.DisplayLog("client(" + commSocket.RemoteEndPoint + ") connect successfully!");

                //create a thread for receiving packet from client
                Thread threadReceive = new Thread(new ParameterizedThreadStart(Receive));
                threadReceive.IsBackground = true;
                threadReceive.Start(commSocket);
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
            commSocket = obj as Socket;
            Dictionary<string, string> factDict;

            while (true)
            {
                byte[] buffer = new byte[2048];

                int count = commSocket.Receive(buffer);

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
                            {"sw_version", emulatorUI.sw_version},
                            {"InternalSN", emulatorUI.InternalSN},
                            {"CorrectionFactor", emulatorUI.CorrectionFactor.ToString()},
                            {"AirPressure", emulatorUI.AirPressure.ToString()},
                            {"Temperature", emulatorUI.Temperature.ToString()},
                        };

                        string tempJson = JsonConvert.SerializeObject(tempDict, Formatting.Indented);

                        factDict = new Dictionary<string, string>
                        {
                            {"data", "allData" + tempJson}
                        };

                        String logStr = " DAPEmulatorUI sended all data to ";
                        utils.JsonDataOperation(factDict, buffer);
                        commSocket.Send(buffer);
                        emulatorUI.DisplayLog(logStr);
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
                        utils.JsonDataOperation(factDict, buffer);
                        commSocket.Send(buffer);
                        emulatorUI.DisplayLog(logStr);
                    }
                    else if (tempStr.StartsWith("savePT"))
                    {
                        tempStr = tempStr.Remove(0, 6);
                        string[] tempString = tempStr.Split(',');

                        int tempAirPressure = int.Parse(tempString[0]);
                        int tempTemperature = int.Parse(tempString[1]);

                        if ((tempAirPressure != emulatorUI.AirPressure) || (tempTemperature != emulatorUI.Temperature))
                        {
                            emulatorUI.AirPressure = tempAirPressure;
                            emulatorUI.Temperature = tempTemperature;

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

                        utils.JsonDataOperation(factDict, buffer);
                        commSocket.Send(buffer);
                        emulatorUI.DisplayLog(logStr);

                    }
                    else if (tempStr.Equals("reset"))
                    {
                        Dictionary<string, string> tempDict = new Dictionary<string, string>
                        {
                            {"sw_version", emulatorUI.sw_version_factory},
                            {"InternalSN", emulatorUI.InternalSN_factory},
                            {"CorrectionFactor", emulatorUI.CorrectionFactor_factory.ToString()},
                            {"AirPressure", emulatorUI.AirPressure_factory.ToString()},
                            {"Temperature", emulatorUI.Temperature_factory.ToString()},
                        };

                        string tempJson = JsonConvert.SerializeObject(tempDict, Formatting.Indented);
                        factDict = new Dictionary<string, string>
                        {
                            {"data", "reset" + tempJson}
                        };

                        string logStr = " DAPEmulatorUI sended all reset data to ";
                        utils.JsonDataOperation(factDict, buffer);
                        commSocket.Send(buffer);
                        emulatorUI.DisplayLog(logStr);

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
                        int receive = sendSocket.Send(rfdBuffer);


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
                        int receive = sendSocket.Send(rfdBuffer);

                        msgReceive.Invoke(receiveCallBack, tempStr);
                    }
                    else if (tempStr.Equals("TST"))//"TST" command for testing tability of DAP meter
                    {
                        tempStr = tempStr.Trim();
                        factDict = new Dictionary<string, string>
                        {
                            {"data", tempStr}
                        };

                        receiveMsgStr = "[" + DateTime.Now.ToString() + "] message from " + sendSocket.RemoteEndPoint + ":" + tempStr;
                        msgReceive.Invoke(receiveCallBack, receiveMsgStr);
                        string json = JsonConvert.SerializeObject(factDict, Formatting.Indented);
                        byte[] sendBuffer = Encoding.Default.GetBytes(json);

                        try
                        {
                            sendSocket.Send(sendBuffer);

                            string tempMsg = "DAPEmulatorUI sended message to " + sendSocket.RemoteEndPoint + ":" + factDict["data"];
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

    }
}
