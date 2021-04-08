using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace BattleMonstersServer
{
    class Server
    {
        public static int MaxPlayers { get; private set; }
        public static int Port { get; private set; }
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
        public delegate void PacketHandler(int _fromClient, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;

        private static TcpListener tcpListener;
        private static UdpClient udpListener;

        //Upon server startup, open TCP and UDP sockets and begin accepting clients to connect.
        public static void Start(int _maxPlayers, int _port)
        {
            MaxPlayers = _maxPlayers;
            Port = _port;

            Console.WriteLine("Starting server...");
            InitalizeServerData();

            //Open tcp listeners to allow incoming communication.
            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            //Open a udp client for incoming communication.
            udpListener = new UdpClient(Port);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            

            Console.WriteLine($"Server started on {Port}.");

            CardFactory.GetInstance();
        }

        //Ensure the server can recieve all TCP communications
        private static void TCPConnectCallback(IAsyncResult _result)
        {
            TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
            Console.WriteLine($"Incoming connection from {_client.Client.RemoteEndPoint}...");

            //Ensure all client tcps are connected to a socket.
            for (int i = 1; i <= MaxPlayers; i++)
            {
                if (clients[i].tcp.socket == null)
                {
                    clients[i].tcp.Connect(_client);
                    return;
                }
            }

            Console.WriteLine($"{_client.Client.RemoteEndPoint} failled to connect: Server full!");
        }

        //Ensure the server can recieve all UDP comminication
        private static void UDPReceiveCallback(IAsyncResult _result)
        {
            try
            {
                IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
                udpListener.BeginReceive(UDPReceiveCallback, null);

                //Exit if end of message
                if (_data.Length < 4)
                {
                    return;
                }

                //Ensure validity of incoming UDP packet
                using (Packet _packet = new Packet(_data))
                {
                    int _clientId = _packet.ReadInt();

                    //No client should have Id 0
                    if (_clientId == 0)
                    {
                        return;
                    }

                    //The client is required to have a endpoint, if they do not, assing them one and avoid processing the packet any further
                    if (clients[_clientId].udp.endPoint == null)
                    {
                        clients[_clientId].udp.Connect(_clientEndPoint);
                        return;
                    }

                    //Ensure the clients endpoint is the same as previously established
                    if (clients[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString())
                    {
                        clients[_clientId].udp.HandleData(_packet);
                    }
                    else
                    {
                        Console.WriteLine($"WARNING! UDP ID does not match previously established endpoint. Potential IP Spoof hack detected!");
                    }
                }

            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Error receiving UDP data: {_ex}");
            }
        }

        //Send a udp packet to the client
        public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
        {
            try
            {
                if (_clientEndPoint != null)
                {
                    udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
                }
            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Error sending UDP data to {_clientEndPoint} via UDP: {_ex}");
            }
        }

        //Create client instances for all possible users, and initialize packet handlers with appropriate function calls.
        private static void InitalizeServerData() 
        {
            for (int i = 1; i <= MaxPlayers; i++)
            {
                clients.Add(i, new Client(i));
            }

            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                {(int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived },
                {(int)ClientPackets.udpTestReceieve, ServerHandle.UDPTestReceived }
            };
            Console.WriteLine("Initialized packets.");
        }
    }
}
