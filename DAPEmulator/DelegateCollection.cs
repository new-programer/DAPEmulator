using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAPEmulator
{
    public delegate void updateUIControl(string logContent, bool flag, string pointStr);


    public class DelegateCollection
    {
        public delegate void updateUIControlDelegate(string logContent, bool flag, string pointStr);
    }
}
