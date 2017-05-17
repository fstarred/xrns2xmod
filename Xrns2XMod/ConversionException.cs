using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xrns2XMod
{
    public class ConversionException : Exception
    {
        //public enum ERROR { SOURCE, DESTINATION }

        //public ERROR Error { get; private set; }

        public ConversionException(string message) : base(message)
        {
            //this.Error = error;
        }
    }
}
