using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Xrns2XModUI
{
    class Globals
    {
        public WebProxy WebProxy { get; set; }

        private static Globals instance = new Globals();

        public const string URI_UPDATER = "http://starredmediasoft.com/xrns2xmod_updater.xml";

        protected Globals()
        {
        }

        public static Globals Instance
        {
            get { return instance; }
        }
    }
}
