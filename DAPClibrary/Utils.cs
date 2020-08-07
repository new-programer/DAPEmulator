using DAPClibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DAPClibrary
{
    /// <summary>
    /// a tool class for convenient operation
    /// </summary>
    public class Utils
    {
        public Utils()
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
        /// a common method for processing and sending data 
        /// </summary>
        /// <param name="dataDicts"></param>
        /// <param name="buffer"></param>
        public byte[] JsonDataOperationProtocol(DAPProtocol protocol)
        {
            byte[] buffer = new byte[2048];
            String json = JsonConvert.SerializeObject(protocol, Formatting.Indented);
            buffer = Encoding.Default.GetBytes(json);
            return buffer;
        }

        /// <summary>
        /// a common method for processing and sending data 
        /// </summary>
        /// <param name="dataDicts"></param>
        /// <param name="buffer"></param>
        public void JsonDataOperation(Dictionary<string, string> dataDicts, byte[] buffer)
        {
            String json = JsonConvert.SerializeObject(dataDicts, Formatting.Indented);
            buffer = Encoding.Default.GetBytes(json);
        }
    }
}
