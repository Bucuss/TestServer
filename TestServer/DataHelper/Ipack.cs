using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestServer
{
    public interface Ipack
    {
        byte[] WriteAsBytes(int HeadData);
        int ReadByBytes(byte[] result);
    }
}
