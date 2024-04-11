using chatPrueba.Net;
using chatPrueba.MVVM.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using chatPrueba.MVVM.Model;
using System.Collections.ObjectModel;
using System.Windows;

namespace chatPrueba.MVVM.ViewModel
{
    class MainViewModel
    {
        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }

        private Server _server;
        public String Username { get; set; }
        public String Message { get; set; }
        public ObservableCollection<UserModel> Users { get; set; } 
        public ObservableCollection<string> Messages { get; set; }
        
        public MainViewModel()
        {
            _server = new Server();
            _server.connectedEvent += UserConnected;
            _server.msgReceivedEvent += MessageReceived;
            _server.disconnectedEvent += UserConnected;
            Users = new ObservableCollection<UserModel>();
            Messages = new ObservableCollection<string>();
            ConnectToServerCommand = new RelayCommand(o => _server.ConnectToServer(Username), o => !string.IsNullOrEmpty(Username));
            SendMessageCommand = new RelayCommand(o => _server.SendMessageToServer(Message), o => !string.IsNullOrEmpty(Message));
        }

        private void MessageReceived()
        {
            var msg = _server.PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() => Messages.Add(msg));

        }

        //This will be used whenever we receive an '0' opcode
        private void UserConnected()
        {
            var user = new UserModel
            {
                UserName = _server.PacketReader.ReadMessage(),
                UID = _server.PacketReader.ReadMessage(),
            };

            if (!Users.Any(x => x.UID == user.UID))
            { 
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            }

        }

        private void RemoveUser()
        {
            var uid = _server.PacketReader.ReadMessage();
            var user = Users.Where(x => x.UID == uid).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
        }
    }
}
