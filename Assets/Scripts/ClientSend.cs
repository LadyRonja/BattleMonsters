using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    //Send a TCP packet.
    private static void SendTCPData(Packet _packet) 
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    //Send a UDP packet
    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }



    #region Packets
    //(TCP) Welcome confirmation packet, upon recieving welcome packet from server on connection
    public static void WelcomeRecieved()  
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(UIManager.instance.usernameField.text);

            SendTCPData(_packet);
        }
    }

    //(UDP) UDP-test confirmation packet, upon recieving UDP-test packet from server on connection
    public static void UDPTestReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.udpTestReceieve))
        {
            _packet.Write("Received a UDP packet.");
            SendUDPData(_packet);
        }
    }
    #endregion
}
