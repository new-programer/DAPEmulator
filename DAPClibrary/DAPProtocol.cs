using System;
using System.Collections.Generic;

namespace DAPClibrary
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

        public string Head { get => head; set => head = value; }
        public Dictionary<string, string> Body { get => body; set => body = value; }

        public DAPProtocol()
        {

        }

        public DAPProtocol(string head, Dictionary<string, string> body)
        {
            this.Head = head;
            this.Body = body;
        }

    }
}
