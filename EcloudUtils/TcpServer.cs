using System;
using System.Text;
using System.Collections;
using Crestron.SimplSharp;      // For Basic SIMPL# Classes
using Crestron.SimplSharp.CrestronSockets;

namespace EcloudUtils
{
    public class TcpServer
    {
        private ArrayList clients;
        private TCPServer server;
        private int bufferSize = 1024;
        private int maxClient = 10;

        public delegate void ConnectedHandler(SimplSharpString clientIndex);
        public ConnectedHandler OnConnect { set; get; }

        public delegate void DisconnectedHandler(SimplSharpString clientIndex);
        public DisconnectedHandler OnDisconnect { set; get; }

        public delegate void RxHandler(SimplSharpString data);
        public RxHandler OnRx { set; get; }

        /// <summary>
        /// SIMPL+ can only execute the default constructor. If you have variables that require initialization, please
        /// use an Initialize method
        /// </summary>

        public void listen(int port)
        {
            clients = new ArrayList();
            this.server = new TCPServer("0.0.0.0", port, bufferSize, EthernetAdapterType.EthernetLANAdapter, maxClient);
            SocketErrorCodes error = server.WaitForConnectionAsync(onConnect);

            server.SocketStatusChange += onStatuChange;
        }

        private void onStatuChange(TCPServer server, uint clientIndex, SocketStatus serverSocketStatus)
        {
            if (serverSocketStatus == SocketStatus.SOCKET_STATUS_CONNECTED)
            {
                this.clients.Add(clientIndex);
                if (OnConnect != null)
                {
                    OnConnect(Convert.ToInt32(clientIndex).ToString());
                }
            }
            else
            {
                this.clients.Remove(clientIndex);
                if (OnDisconnect != null)
                {
                    OnDisconnect(Convert.ToInt32(clientIndex).ToString());
                }
            }
        }

        private void onConnect(TCPServer server, uint clientIndex)
        {
            server.ReceiveDataAsync(clientIndex, onRead);
            server.WaitForConnectionAsync(onConnect);
        }
       

        private void onRead(TCPServer server,uint clientIndex,int numberOfBytesReceived)
        {
            if (numberOfBytesReceived > 0)
            {
                String data = System.Text.Encoding.BigEndianUnicode.GetString(server.GetIncomingDataBufferForSpecificClient(clientIndex), 0, numberOfBytesReceived);
                if (OnRx != null)
                {
                    OnRx((new SimplSharpString(data)).ToString());
                }
                server.ReceiveDataAsync(clientIndex, onRead);
            }
        }

        private byte[] trans(byte[] data)
        {
            ArrayList ret = new ArrayList();
            int i = 0;
            foreach (byte b in data)
            {
                if (i % 2 == 1)
                {
                    ret.Add(b);   
                }
                i++;
            }
            byte[] r = new byte[ret.Count];
            i = 0;
            foreach (byte b in ret)
            {
                r[i] = b;
                i++;
            }

            return r;
        }

        public void broadcast(SimplSharpString data)
        {
            foreach (uint clientIdex in this.clients)
            {
                byte[] db = this.trans(System.Text.Encoding.BigEndianUnicode.GetBytes(data.ToString()));
                this.server.SendData(clientIdex,db, db.Length);
            }
        }

    }
}