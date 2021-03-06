﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BattleMonstersServer
{
    class ServerSend
    {

        //Send a TCP packet to a client.
        private static void SendTCPData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.clients[_toClient].tcp.SendData(_packet);         
        }

        //Send a UDP packet to a client.
        private static void  SendUDPData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.clients[_toClient].udp.SendData(_packet);
        }

        //Send a TCP packet to all clients.
        private static void SendTCPDataToAll(Packet _packet)
        {
            _packet.WriteLength();

            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }

        //Send a TCP packet to all clients but one client.
        private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
        {
            _packet.WriteLength();

            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != _exceptClient)
                {
                    Server.clients[i].tcp.SendData(_packet);
                }            
            }
        }

        //Send a UDP packet to all clients.
        private static void SendUDPDataToAll(Packet _packet)
        {
            _packet.WriteLength();

            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.clients[i].udp.SendData(_packet);
            }
        }

        //Send a UDP packet to all clients but one client.
        private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
        {
            _packet.WriteLength();

            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != _exceptClient)
                {
                    Server.clients[i].udp.SendData(_packet);
                }

            }
        }


        #region Packets
        //(TCP) Welcoming packet upon connection
        public static void Welcome(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.welcome)) //"using" desposes the packet when done, otherwise would have to be done by hand
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }

        //(UDP) Testing UDP data deliery upon connecting
        public static void UDPTest(int _toClient)
        {
            using (Packet _packet = new Packet((int)ServerPackets.udpTest))
            {
                _packet.Write("A test packet for UDP.");

                SendUDPData(_toClient, _packet);
            }
        }
        #endregion

    }
}
