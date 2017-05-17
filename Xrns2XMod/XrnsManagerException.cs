using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xrns2XMod
{
    public class XrnsException : Exception
    {
        public XrnsException(string message)
            : base(message)
        {
        }
    }
}
