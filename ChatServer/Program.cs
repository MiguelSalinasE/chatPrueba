using ChatServer.Net.IO;
using System;
using System.Net;
using System.Net.Sockets;

namespace ChatServer
{
    class Program
    {
        static List<Client> _users;
        static TcpListener _listener;
        static void Main(string[] args)
        {
            _users = new List<Client>();
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7891);
            _listener.Start();
            while(true)
            {
                var client = new Client(_listener.AcceptTcpClient());
                _users.Add(client);

                /* Broadcast the conection to everyone on the server */
                BroadcastConnection();
            }
            
            
        }

        static void BroadcastConnection()
        {
            foreach(var user in _users)
            {
                foreach(var usr in _users)
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(1);
                    broadcastPacket.WriteMessage(usr.Username);
                    broadcastPacket.WriteMessage(usr.UID.ToString());
                    user.ClientSocket.Client.Send(broadcastPacket.getPacketBytes());
                }
            }
        }

        public static void BroadcastMessage(String message)
        {
            foreach (var user in _users) { 
                var msgPacket = new PacketBuilder();
                msgPacket.WriteOpCode(2);
                msgPacket.WriteMessage(message);
                user.ClientSocket.Client.Send(msgPacket.getPacketBytes());
            }
        }
        public static void BroadcastDisconnect(String uid)
        {
            var disconnectedUser = _users.Where(x => x.UID.ToString() == uid).FirstOrDefault();
            _users.Remove(disconnectedUser);
            foreach (var user in _users)
            {
                var broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOpCode(3);
                broadcastPacket.WriteMessage(uid);
                user.ClientSocket.Client.Send(broadcastPacket.getPacketBytes());
            }
            BroadcastMessage($"[{disconnectedUser.Username}] Disconnected");
        }

    }
}
