using System;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using System.IO;

namespace RokuTools
{
    public class RokuTools
    {
        public class RokuDevice
        {
            public string IpAddress { get; set; }
            public string FriendlyName { get; set; }

            public RokuDevice(string Address, string Name)
            {
                IpAddress = Address;
                FriendlyName = Name;
            }
        }

        public static string[] GetDevices()
        {
            int b = 0;
            List<string> rokulist = new List<string>();
            //do
            while (rokulist.Count < 1 && b < 5)
            {
                System.Net.Sockets.Socket s = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 2400);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("239.255.255.250"), 1900);
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                EndPoint senderRemote = (EndPoint)RemoteIpEndPoint;
                Byte[] sendBytes = Encoding.ASCII.GetBytes("M-SEARCH * HTTP/1.1\nHost: 239.255.255.250:1900\nMan: \"ssdp:discover\"\nMX: 2\nST: roku:ecp\n");
                var recvBytes = new byte[1028];
                s.SendTo(sendBytes, endPoint);

                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        int recv = s.ReceiveFrom(recvBytes, ref senderRemote);
                        rokulist.Add(senderRemote.ToString());
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("{0} Devices Found:", rokulist.Count);
                        s.Close();
                        goto EndUDP;
                    }

                }
               
            EndUDP:
                b++;
            }
            //while (rokulist.Count < 1 && b < 5);

            if (rokulist.Count == 0)
            {
                rokulist.Add ("0.0.0.0");
            }

            int c = 0;
            string[] tmp = new string[2];
            string[] DeviceList = new string[rokulist.Count];
            foreach (string a in rokulist)
            {
                tmp = a.Split(':');
                DeviceList[c] = tmp[0];
                c++;
            }

            return DeviceList;
        }

        public static string[] DeviceNames(string[] DeviceList)
        {
            int c = 0;
            string[] DeviceNameArray = new string[DeviceList.Length];

            foreach (string a in DeviceList)
            {
                string Dev1Info = QueryDevice(DeviceList[c]);
                DeviceNameArray[c] = (DeviceList[c] + "\t<" + Dev1Info + ">");
                c++;
            }
            return DeviceNameArray;
        }

        public static RokuDevice[] GetDeviceArray(string[] DeviceList)
        {
            int i = 0;
            RokuDevice[] DeviceArray = new RokuDevice[DeviceList.Length];
            foreach (string a in DeviceList)
            {
            string FriendlyName = QueryDevice(a);
                RokuDevice b = new RokuDevice(a, FriendlyName);
                DeviceArray[i] = b;
                i++;
            }
            return DeviceArray;
        }

     

        public static string QueryDevice(string server)
        {
            try
            {
                Int32 port = 8060;
                TcpClient client1 = new TcpClient(server, port);

                client1.ReceiveTimeout = 5000;

                Byte[] data = System.Text.Encoding.ASCII.GetBytes("GET /query/device-info HTTP/1.1\r\n\r\n");

                NetworkStream stream = client1.GetStream();

                stream.Write(data, 0, data.Length);

                data = new Byte[2048];

                var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                data = memoryStream.ToArray();
                string StrArray = System.Text.Encoding.ASCII.GetString(data);

                int sourceindexbegin = StrArray.IndexOf("<friendly-device-name>");
                int sourceindexend = StrArray.IndexOf("</friendly-device-name>");
                string devicename = StrArray.Substring(sourceindexbegin + 22, sourceindexend - (sourceindexbegin + 22));

                stream.Close();
                client1.Close();
                return devicename;
            }

            catch (Exception)
            {
                return "Connection Error";
                //return "Error Receiving Name";
            }

        }

        public static void SendKey(string server, string keypress)
        {
            try
            {
                TcpClient client1 = new TcpClient(server, 8060);
                Stream stream = client1.GetStream();
                byte[] data = System.Text.Encoding.ASCII.GetBytes($"POST /keypress/{keypress} HTTP/1.1\r\nHost: {server}:8060\r\nAccept: */*\r\nContent-Length: 0\r\nContent-Type: application/x-www-form-urlencoded\r\n\r\n");
                stream.Write(data, 0, data.Length);
                client1.Close();
            }
            catch
            { }
        }

        public static void SendLaunch(string server, string launchid)
        {
            try
            {
                TcpClient client1 = new TcpClient(server, 8060);
                Stream stream = client1.GetStream();
                byte[] data = System.Text.Encoding.ASCII.GetBytes($"POST /launch/{launchid} HTTP/1.1\r\nHost: {server}:8060\r\nAccept: */*\r\nContent-Length: 0\r\nContent-Type: application/x-www-form-urlencoded\r\n\r\n");
                stream.Write(data, 0, data.Length);
                client1.Close();
            }
            catch { }
        }

        public static void ManualEntry(string server, string ManualCommand)
        {
            try
            {

                TcpClient client1 = new TcpClient(server, 8060);
                Stream stream = client1.GetStream();
                byte[] data = System.Text.Encoding.ASCII.GetBytes($"POST /{ManualCommand} HTTP/1.1\r\nHost: {server}:8060\r\nAccept: */*\r\nContent-Length: 0\r\nContent-Type: application/x-www-form-urlencoded\r\n\r\n");
                stream.Write(data, 0, data.Length);
                client1.Close();
            }
            catch
            {

            }
        }

        public static string[][] Disco()
        {
            string[] servers = GetDevices();
            string[] rokuName = DeviceNames(servers);
            string[][] ServerMatrix = new string[2][];
            ServerMatrix[0] = servers;
            ServerMatrix[1] = rokuName;

            return ServerMatrix;
        }

        public static RokuDevice[] Disco2()
        {
            string[] servers = GetDevices();
            RokuDevice[] DeviceArray = GetDeviceArray(servers);
            return DeviceArray;
        }

        public static int checkServNum(int servNum)
        {
            if (servNum < 0)
            { servNum = 0; }
            return servNum;
        }

    }
}