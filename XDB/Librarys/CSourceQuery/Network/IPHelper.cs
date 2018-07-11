using System;
using System.Net;
using System.Threading.Tasks;

namespace Phantom.CSourceQuery.Network
{
    public class IPHelper
    {
        public async Task<IPEndPoint> GetIPEndPointFromHostName(string hostName, int port, bool throwIfMoreThanOneIP)
        {
            var addresses = await Dns.GetHostAddressesAsync(hostName);

            if (addresses.Length == 0)
                throw new ArgumentException("Unable to retrieve address from specified host name.", "hostName");
            else if (throwIfMoreThanOneIP && addresses.Length > 1)
                throw new ArgumentException("There is more that one IP address to the specified host.", "hostName");


            return new IPEndPoint(addresses[0], port);
        }
    }
}
