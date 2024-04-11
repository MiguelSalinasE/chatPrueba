using chatPrueba.Net.IO;
using System.Net.Sockets;

namespace chatPrueba.Net
{
    class Server
    {
        TcpClient _client;
        public PacketReader PacketReader;

        public event Action connectedEvent;
        public event Action msgReceivedEvent;
        public event Action disconnectedEvent;

        public Server()
        {
            _client = new TcpClient();
        }

        public void ConnectToServer(String username)
        {

            if (!_client.Connected)
            {
                _client.Connect("127.0.0.1", 7891);
                PacketReader = new PacketReader(_client.GetStream());
                if (!string.IsNullOrEmpty(username))
                {
                    var connectPacket = new PacketBuilder();
                    connectPacket.WriteOpCode(0);
                    connectPacket.WriteString(username);
                    _client.Client.Send(connectPacket.GetPacketBytes());
                }
                ReadPackets();

            }
        }

        private void ReadPackets()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    // TODO : differ between opcodes, by doing that function we could send files, images and all of that

                    var opcode = PacketReader.ReadByte();
                    switch (opcode)
                    {
                        case 1:
                            connectedEvent?.Invoke();
                            break;
                        case 2:
                            msgReceivedEvent?.Invoke();
                            break;
                        case 3:
                            disconnectedEvent?.Invoke();
                            break;
                        default:
                            break;
                    }
                }
            });
        }

        public void SendMessageToServer(String messsage)
        {
            var messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode(2);
            messagePacket.WriteString(messsage);
            _client.Client.Send(messagePacket.GetPacketBytes());
        }
    }
}
