using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class Client : MonoBehaviour
{

    public static Client instance;
    public static int dataBufferSize = 4096; //4096 = 4MB

    //Do not change the IP from here, it does not update when building or playing in Unity
    public string ip = "92.33.146.82";//Public IP
    public int port = 26950; //TODO: Get random free socket port.

    public int myId = 0;

    public TCP tcp;
    public UDP udp;

    private bool isConnected = false;
    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

    //Singleton pattern
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    //Disconnect when exiting the editors playmode
    private void OnApplicationQuit()
    {
        Disconnect();
    }

    //Open TCP and UDP communication channels, and attempt to connect the server.
    public void ConnectedToServer()
    {
        //Get IP user enetered in the UI field
        ip = UIManager.instance.IPField.text;

        tcp = new TCP();
        udp = new UDP();

        InitializeClientData();

        if (UIManager.instance.attemptingConnectIP != null)
        {
            UIManager.instance.attemptingConnectIP.text = "Attempting to connect to IP: " + Client.instance.ip;
        }
        isConnected = true;

        tcp.Connect();
    }


    //TCP is a network protocol that ensures all data arrives and in the right order.
    public class TCP
    {
        public TcpClient socket;

        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;

        //Open socket and attempt to connect to the server
        public void Connect()
        {
            socket = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };

            receiveBuffer = new byte[dataBufferSize];
            socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);
        }

        //Confirm that the socket is connected and communicate with the server
        private void ConnectCallback(IAsyncResult _result)
        {
            socket.EndConnect(_result);

            if (!socket.Connected)
            {
                return;
            }

            stream = socket.GetStream();

            receivedData = new Packet();

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, RecieveCallback, null);
        }

        //Send desired packet to the server
        public void SendData(Packet _packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to server via TCP: {_ex}");
            }
        }

        //Task asynchronously listens for incoming communication from the client
        private void RecieveCallback(IAsyncResult _result)
        {
            try
            {
                //Upon recieving a a packet, determine it's length and determine it's validity 
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    instance.Disconnect();
                    return;
                }

                //Copy the recieved data and determine if it's the complete packet 
                //before reseting what has been recieved and start listning for new packets
                byte[] _data = new byte[_byteLength];
                Array.Copy(receiveBuffer, _data, _byteLength);

                receivedData.Reset(HandleData(_data));
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, RecieveCallback, null);

            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Error receiving TCP data: {_ex}");
                Disconnect();
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
                //If the packet length is less than 1 reset the data
                _packetLength = receivedData.ReadInt();
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
                        packetHandlers[_packetId](_packet);
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

            // If true, there is still a partial packet to recieve
            if (_packetLength <= 1)
            {
                return true;
            }

            return false;

        }

        //Close the TCP socket and clean up for new potential connections.
        private void Disconnect()
        {
            instance.Disconnect();

            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }

    }

    //UDP is a network protocol that is very fast but does not ensure all data arrives, nor the order of said data.
    public class UDP 
    {
        public UdpClient socket;
        public IPEndPoint endPoint;

        public UDP()
        {
            endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
        }

        //Assign an endpoint and begin listning for UDP packets
        public void Connect(int _localPort)
        {
            socket = new UdpClient(_localPort);

            socket.Connect(endPoint);
            socket.BeginReceive(ReceiveCallback, null);

            using (Packet _packet = new Packet())
            {
                SendData(_packet);
            }
        }

        //Send a UDP packet
        public void SendData(Packet _packet)
        {
            try
            {
                _packet.InsertInt(instance.myId);
                if (socket != null)
                {
                    socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
                }

            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to server via UDP: {_ex}");
            }
        }

        //Task asynchronously listens for incoming communication
        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                byte[] _data = socket.EndReceive(_result, ref endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                //If a UDP packet with less than 4 bytes is recieved, it cannot be determined how large it is and thus won't be handled
                if (_data.Length < 4)
                {
                    instance.Disconnect();
                    return;
                }

                //Handle the packet
                HandleData(_data);
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error receiving callback: {_ex}");
                Disconnect();
            }
        }

        //Handle the recieved data.
        private void HandleData(byte[] _data)
        {
         
            using (Packet _packet = new Packet(_data))
            {
                int _packetLength = _packet.ReadInt();
                _data = _packet.ReadBytes(_packetLength);
            }

            //Packets may be recieved asynchronously, so ensure their data are handled on the main thread with
            //the appropriate PacketHandler
            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _packet = new Packet(_data))
                {
                    int _packetId = _packet.ReadInt();
                    packetHandlers[_packetId](_packet);
                }
            });

        }

        //Clean up and prephare for a new connecton.
        private void Disconnect()
        {
            instance.Disconnect();

            endPoint = null;
            socket = null;
        }
    
    }

    //Initialize packet handlers with appropriate function calls
    private void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            {(int)ServerPackets.welcome, ClientHandle.Welcome },
            {(int)ServerPackets.udpTest, ClientHandle.UDPTest }
        };
        Debug.Log("Initialized packets.");
    }

    //Cleanup on disconnection
    private void Disconnect()
    {
        if (isConnected)
        {
            isConnected = false;
            tcp.socket.Close();
            udp.socket.Close();

            Debug.Log("Disconnected from server");
        }
    }

}
