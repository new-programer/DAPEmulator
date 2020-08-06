using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAPEmulator
{
    class DAPParameters
    {
        /*define some parameters related of DAP emulator, these parameters all are maybe changed.
         * s/w version, the version of DAP meter
         * Internal Serial Number,
         * DAP Correction Factor, default value is 1.00
         * Air Pressure, default value is 1013Pa
         * Temperature, default value is 20`C / 293.15K
         */
        public readonly string sw_version = "120-131 ETH";
        public readonly string InternalSN = "DAP20200714";
        public double CorrectionFactor = 1.00;
        public int AirPressure = 1013;
        public int Temperature = 20;


        //define some parameters that are used to save default value and just readed only, 
        public readonly string sw_version_factory = "120-131 ETH";
        public readonly string InternalSN_factory = "DAP20200714";
        public readonly double CorrectionFactor_factory = 1.00;
        public readonly int AirPressure_factory = 1013;
        public readonly int Temperature_factory = 20;

    }
}
