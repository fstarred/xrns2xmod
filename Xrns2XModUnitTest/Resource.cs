using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xrns2XModUnitTest
{
    class Resource : Attribute
    {
        public Resource(string path)
        {
            this.Path = path;
        }

        public string Path { get;  }
    }
}
