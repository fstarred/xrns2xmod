using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xrns2XMod
{
    public interface IConverter
    {
        event ProgressHandler EventProgress;

        byte[] Convert(SongData song);
    }
}
