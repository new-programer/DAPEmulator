using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAPClibrary
{
    public class DAPCommand
    {
        //for checking up validaty of communicating data
        public const string key = "DAP";

        public string command { get; set; }

        /// <summary>
        /// do command sign for validate data
        /// </summary>
        /// <returns></returns>
        public string GetCommandSign()
        {
            string sign = string.Empty;


            byte[] keyBytes = System.Text.Encoding.Unicode.GetBytes(key);
            byte[] commandBytes = System.Text.Encoding.Unicode.GetBytes(command);

            byte[] signBytes = new byte[commandBytes.Length];

            for (int i = 0; i < commandBytes.Length; i++)
            {
                signBytes[i] = (byte)(commandBytes[i] ^ keyBytes[i]);
            }

            string result = System.Text.Encoding.Unicode.GetString(signBytes);

            return result;
        }
    }


}
