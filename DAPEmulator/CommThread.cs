using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DAPEmulator
{
    public class CommThread
    {
        public DelegateCollection.updateUIControl threadDelegate;
        Utils utils = new Utils();
        DAPParameters dapParameters = new DAPParameters();


        //declare sockets for listening and sending packets
        Socket commSocket;
        DAPProtocol dapProtocol;

        DAPEmulatorUI emulatorUI;


        public CommThread()
        {
            
        }

        public CommThread(DAPEmulatorUI emulatorUI, Socket commSocket)
        {
            this.emulatorUI = emulatorUI;
            this.commSocket = commSocket;
        }

        /// <summary> 
        ///  method for receiving and processing packets from client in a loop;
        ///  there are different prefixes in data string:
        ///  "%SFD", it is used to write Correction Factor of DAP to DAP Emulator;
        ///  
        /// </summary>
        /// <param name="obj"></param>
        public void RunThread()
        {
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
                            threadDelegate(command, true, commSocket.RemoteEndPoint.ToString());

                            dapProtocol = new DAPProtocol("TST", null);
                            utils.JsonDataOperation(dapProtocol, buffer);
                            commSocket.Send(buffer);

                            threadDelegate(command, false, commSocket.RemoteEndPoint.ToString());
                            break;

                        case "%RFD":
                            threadDelegate(command, true, commSocket.RemoteEndPoint.ToString());

                            dataContent = dapParameters.CorrectionFactor.ToString();
                            factDict = new Dictionary<string, string>
                            {
                                {"data", dataContent}
                            };
                            dapProtocol = new DAPProtocol("%RFD", factDict);
                            utils.JsonDataOperation(factDict, buffer);
                            commSocket.Send(buffer);

                            threadDelegate(command, false, commSocket.RemoteEndPoint.ToString());
                            break;

                        case "%SFD":
                            threadDelegate(command, true, commSocket.RemoteEndPoint.ToString());

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

                            threadDelegate(command, false, commSocket.RemoteEndPoint.ToString());
                            break;

                        case "LoadAllData":
                            threadDelegate(command, true, commSocket.RemoteEndPoint.ToString());

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

                            threadDelegate(command, false, commSocket.RemoteEndPoint.ToString());
                            break;

                        case "reset":
                            threadDelegate(command, true, commSocket.RemoteEndPoint.ToString());

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

                            threadDelegate(command, false, commSocket.RemoteEndPoint.ToString());

                            break;

                        case "savePT":
                            threadDelegate(command, true, commSocket.RemoteEndPoint.ToString());

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

                            threadDelegate(command, false, commSocket.RemoteEndPoint.ToString());
                            break;
                        case "Validate":
                            threadDelegate(command, true, commSocket.RemoteEndPoint.ToString());

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

                            threadDelegate(command, false, commSocket.RemoteEndPoint.ToString());
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
