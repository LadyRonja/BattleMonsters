using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace BattleMonstersServer
{
    class Client
    {
        public static int dataBufferSize = 4096; //4096 == 4 MB

        public int id;
        public TCP tcp;
        public UDP udp;

        public Client(int _clientId)
        {
            id = _clientId;
            tcp = new TCP(id);
            udp = new UDP(id);
        }

        //TCP is a network protocol that ensures all data arrives and in the right order. 
        public class TCP
        {
            public TcpClient socket;
            
            private readonly int id;
            private NetworkStream stream;
            private Packet receivedData;
            private byte[] receiveBuffer;


            public TCP(int _id) 
            {
                id = _id;
            }

            //Open sockets and being accepting TCP communication.
            public void Connect(TcpClient _socket)
            {
                socket = _socket;
                socket.ReceiveBufferSize = dataBufferSize;
                socket.SendBufferSize = dataBufferSize;

                stream = socket.GetStream();

                receivedData = new Packet();
                receiveBuffer = new byte[dataBufferSize];

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null); //Start running RecieveCallback on a seperate thread, running asynchronously to the main application. 

                ServerSend.Welcome(id, "Welcome to the server!");
            }

            //Send desired packet to this client from the server.
            public void SendData(Packet _packet)
            {
                try
                {
                    if(socket != null)
                    {
                        stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                    }

                }
                catch (Exception _ex)
                {

                    Console.WriteLine($"Error sending data to player {id} via TCP: {_ex}");
                }

            }

            //Task asynchronously listens for incoming communication from the client
            private void ReceiveCallback(IAsyncResult _result)
            {
                try
                {
                    //Upon recieving a a packet, determine it's length and determine it's validity 
                    int _byteLength = stream.EndRead(_result);
                    if (_byteLength <= 0)
                    {
                        Server.clients[id].Disconnect();
                        return;
                    }

                    //Copy the recieved data and determine if it's the complete packet 
                    //before reseting what has been recieved and start listning for new packets
                    byte[] _data = new byte[_byteLength];
                    Array.Copy(receiveBuffer, _data, _byteLength);

                    receivedData.Reset(HandleData(_data));
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

                }
                catch (Exception _ex)
                {
                    Console.WriteLine($"Error receiving TCP data: {_ex}");
                    Server.clients[id].Disconnect();
                }
            }

            //Determine if a complete packet has been recieved, and if so begin handling it with an apporpriate PacketHandler.
            //Return true if a complete package has been recieved
            private bool HandleData(byte[] _data)
            {

                //Determine validity of packet
                int _packetLength = 0;
                receivedData.SetBytes(_data);

                //The first 4 bytes of a packet contains the length of the message
                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLength = receivedData.ReadInt();

                    //If the packet length is less than 1 reset the data
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }

                //As long as the loop is running, the data contains a complete packet that can be handled
                while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
                {
                    byte[] _packetBytes = receivedData.ReadBytes(_packetLength);

                    //HandleData may not be ran on the mainThread, but the data should be handled on it.
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        //Have the appropriate PacketHandlet handle the incoming data from the packet
                        using (Packet _packet = new Packet(_packetBytes))
                        {
                            int _packetId = _packet.ReadInt();
                            Server.packetHandlers[_packetId](id,_packet);
                        }
                    });

                    //Re-assess if there are still more packets to in the recieved data.
                    _packetLength = 0;
                    if (receivedData.UnreadLength() >= 4)
                    {
                        _packetLength = receivedData.ReadInt();
                        if (_packetLength <= 0)
                        {
                            return true;
                        }
                    }
                }//End of while-loop

                //If true, there is still a partial packet to recieve
                if (_packetLength <= 1)
                {
                    return true;
                }

                return false;

            }

            //Close the TCP socket and clean up for a new potential client to connect.
            public void Disconnect()
            {
                socket.Close();
                stream = null;
                receivedData = null;
                receiveBuffer = null;
                socket = null;
            }
        }

        //UDP is a network protocol that is very fast but does not ensure all data arrives, nor the order of said data.
        public class UDP
        {
            public IPEndPoint endPoint;

            private int id;

            public UDP(int _id)
            {
                id = _id;
            }

            //Assign an endpoint and send a UDP test packet.
            public void Connect(IPEndPoint _endpoint)
            {
                endPoint = _endpoint;
                ServerSend.UDPTest(id);
            }

            //Send a packet to this client.
            public void SendData(Packet _packet)
            {
                Server.SendUDPData(endPoint, _packet);
            }

            //Handle the recieved packet.
            public void HandleData(Packet _packetData)
            {
                int _packetLength = _packetData.ReadInt();
                byte[] _packetBytes = _packetData.ReadBytes(_packetLength);

                //Packets may be recieved asynchronously, so ensure their data are handled on the main thread with
                //the appropriate PacketHandler
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetID = _packet.ReadInt();
                        Server.packetHandlers[_packetID](id, _packet);
                    }
                });
            }

            //Clean up and prephare for a new client to connect.
            public void Disconnect()
            {
                endPoint = null;

            }

        }

        //When a client disconnects, ensure cleanup.
        private void Disconnect()
        {
            Console.WriteLine($"{tcp.socket.Client.RemoteEndPoint} has disconnected.");

            //Set any refrences to client specific things such as players to null here
            //player = null; 

            tcp.Disconnect();
            udp.Disconnect();
        }
    }


}
