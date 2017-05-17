using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xrns2XMod
{
    class InstrumentDataMOD : InstrumentDataBase
    {
        public class SampleDataMOD : InstrumentDataBase.SampleDataBase
        {
            /*
             * used to achieve the sample loop value in case of mod frequence resampling (8363 Mhz)
             * because sample length might result small than original
             * formula is:
             * loop value = originalLoopValue : x = originalLength : length
             * */
            public int OriginalLength { get; set; }
        }
        public new SampleDataMOD[] Samples { get; set; }
    }
}
