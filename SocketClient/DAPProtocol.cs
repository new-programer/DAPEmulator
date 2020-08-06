using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketClient
{
    /// <summary>
    /// a class for protocol package. 
    /// </summary>
    [Serializable]
    public class DAPProtocol
    {
        //the head and body of DAP protocol
        string head;
        Dictionary<string, string> body;

        public DAPProtocol()
        {
            
        }

        public DAPProtocol(string head, Dictionary<string, string> body)
        {
            this.head = head;
            this.body = body;
        }
    }
}
