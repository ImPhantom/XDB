//
//      Code is mostly from:
//      https://code.google.com/archive/p/ssqlib/
//

using System.Net;
using System.Net.Sockets;

namespace Phantom.CSourceQuery.Network
{
    internal class SocketHelper
    {
        internal static byte[] getInfo(EndPoint ipe, Packet packet)
        {
            return getInfo(ipe, packet.outputAsBytes());
        }

        internal static byte[] getInfo(EndPoint ipe, byte[] request)
        {
            //Create the socket
            Socket srvSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            //Save the max packet size
            int packetSize = 12288;

            //Send/Receive timeouts
            srvSocket.SendTimeout = 3000;
            srvSocket.ReceiveTimeout = 3000;

            try
            {
                //Send the request to the server
                srvSocket.SendTo(request, ipe);
            }
            catch (SocketException se)
            {
                throw new SourceQueryException("Could not send packet to server {" + se.Message + "}");
            }

            //Create a new receive buffer
            byte[] rcvPacketInfo = new byte[packetSize];

            try
            {
                //Receive the data from the server
                srvSocket.ReceiveFrom(rcvPacketInfo, ref ipe);
            }
            catch (SocketException se)
            {
                throw new SourceQueryException("Could not receive packet from server {" + se.Message + "}");
            }

            //Send the information back
            return rcvPacketInfo;
        }
    }
}
