using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xrns2XMod
{
    public abstract class InstrumentDataBase
    {
        public class SampleDataBase
        {
            // used for: 9xx effect value, loop sample values for mod
            public int Length { get; set; }
            // used for xm to achieve the base note, for mod to write the note
            public int RelatedNote { get; set; }
            // used for mod / xm 
            public int FineTune { get; set; }
        }

        public SampleDataBase[] Samples { get; set; }
    }
}
