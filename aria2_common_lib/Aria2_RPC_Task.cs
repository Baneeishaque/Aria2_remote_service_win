using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aria2_common_lib
{
    public class Aria2_RPC_Task
    {
        public string completedLength { get; set; }
        public string connections { get; set; }
        public string dir { get; set; }
        public string downloadSpeed { get; set; }
        public File[] files { get; set; }
        public string gid { get; set; }
        public string numPieces { get; set; }
        public string pieceLength { get; set; }
        public string status { get; set; }
        public string totalLength { get; set; }
        public string uploadLength { get; set; }
        public string uploadSpeed { get; set; }
    }

    public class File
    {
        public string completedLength { get; set; }
        public string index { get; set; }
        public string length { get; set; }
        public string path { get; set; }
        public string selected { get; set; }
        public Uri[] uris { get; set; }
    }

    public class Uri
    {
        public string status { get; set; }
        public string uri { get; set; }
    }
}
