﻿//
//      Code is mostly from:
//      https://code.google.com/archive/p/ssqlib/
//

using System;
using System.Text;

namespace Phantom.CSourceQuery.Network
{
    internal class Packet
    {
        internal int RequestId = 0;
        internal string Data = "";

        //Output the packet data as a byte array
        internal byte[] outputAsBytes()
        {
            byte[] data_byte = null;

            if (Data.Length > 0)
            {
                //Create a new packet based on the length of the request
                data_byte = new byte[Data.Length + 5];

                //Fill the first 4 bytes with 0xff
                data_byte[0] = 0xff;
                data_byte[1] = 0xff;
                data_byte[2] = 0xff;
                data_byte[3] = 0xff;

                //Copy the data to the new request
                Array.Copy(Encoding.UTF8.GetBytes(Data), 0, data_byte, 4, Data.Length);
            }
            //Empty request to get challenge
            else
            {
                data_byte = new byte[5];

                //Fill the first 4 bytes with 0xff
                data_byte[0] = 0xff;
                data_byte[1] = 0xff;
                data_byte[2] = 0xff;
                data_byte[3] = 0xff;
                data_byte[4] = 0x57;
            }

            return data_byte;
        }
    }
}
