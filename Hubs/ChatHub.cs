using Microsoft.AspNetCore.SignalR;

namespace Socket.Hubs
{
    public class ChatHub : Hub
    {
        
        private readonly string _botUser;
        private readonly IDictionary<string, UserConnection> _connections;

        public ChatHub(IDictionary<string, UserConnection> connections)
        {
            _botUser = "MyChat Bot";
            _connections = connections;
        }
        public async Task SendMessage(String message)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                Console.WriteLine("{0}", message);
                Console.WriteLine("{0}", Context);
                Console.WriteLine("{0}", Clients.All);

                await Clients.All.SendAsync("ReceiveMessage", _botUser, "Someone has typed a message!");

                await Clients.Group(userConnection.Room)
                    .SendAsync("ReceiveMessage", userConnection.User, message);
            }
        }

        public async Task JoinRoom(UserConnection userConnection)
        {
            Console.WriteLine("{0}", _connections);
            Console.WriteLine("{0}", Context);
            Console.WriteLine("{0}", Clients.All);

            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.Room);

            _connections[Context.ConnectionId] = userConnection;

            await Clients.Group(userConnection.Room)
                .SendAsync("ReceiveMessage", _botUser, $"{userConnection.User} has joined {userConnection.Room}");
        }

    }
}
