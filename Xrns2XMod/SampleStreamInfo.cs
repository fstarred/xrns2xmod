using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xrns2XMod
{
    public enum FORMAT { NONE, WAV, AIFF, MP3, OGG, FLAC, AAC }

    public class SampleStreamInfo
    {
        public System.IO.Stream Stream { get; set; }  
        
        public FORMAT Format { get; set; }
    }
}
