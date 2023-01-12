using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Socket.Hubs
{
    public class GameInfo
    {
        public string name { get; set; }
        public string playerTurn { get; set; }
        public string[] board { get; set; }
        public string winner { get; set; }
        public string status { get; set; }
    }

    public class GameHub : Hub
    {

        private readonly IDictionary<string, User> _users;
        private readonly IDictionary<string, GameRoom> _rooms;

        public GameHub(IDictionary<string, User> connections, IDictionary<string, GameRoom> gameRooms)
        {
            _users = connections;
            _rooms = gameRooms;
        }

        public override async Task OnConnectedAsync()
        {
            User user = new User()
            {
                Connection = Context.ConnectionId
            };

            _users[Context.ConnectionId] = user;

            Console.WriteLine("user connected... " + Context.ConnectionId);

            foreach (string key in _users.Keys)
            {
                User tempUser = _users[key];
                Console.WriteLine("user: " + tempUser.UserName);
            }

            await base.OnConnectedAsync();
        }

        public async Task PlayerMove(int pos)
        {
            GameRoom room = _rooms[_users[Context.ConnectionId].Room];

            if (room.GetPlayerTurn() == Context.ConnectionId)
            {
                room.PlayerMove(pos);
                await SendGameInfo(room);
            } else
            {
                await Clients.Group(room.Id)
                    .SendAsync("ReceiveMessage", "Bot", "Do not try and hack...");
            }
        }

        public async Task SendGameInfo(GameRoom room)
        {
            var gameInfo = new GameInfo
            {
                name = room.Name,
                playerTurn = room.GetPlayerTurn(),
                board = room.GetBoard(),
                winner = room.GetWinner(),
                status = room.GetStatus()
            };

            var data = JsonSerializer.Serialize(gameInfo);

            await Clients.Group(room.Id)
                .SendAsync("GameInfo", data);
        }

        public async Task JoinGame(string username)
        {
            User user;
            string id = Guid.NewGuid().ToString();

            Console.WriteLine("user joined game... " + username);

            user = _users[Context.ConnectionId] ?? new User();
            user.UserName = username;
            user.Room = JoinGameRoom(id);

            _users[Context.ConnectionId] = user;
            await Groups.AddToGroupAsync(Context.ConnectionId, user.Room);

            Console.WriteLine("ConnectionId: " + user.Connection + ", User: " + user.UserName + ", Room: " + user.Room);

            await SendGameInfo(_rooms[user.Room]);
        }

        public string JoinGameRoom(string id)
        {
            GameRoom gameRoom;
            bool createNewRoom = true;
            string gameId = id;

            foreach (KeyValuePair<string, GameRoom> kvp in _rooms)
            {
                string key = kvp.Key;
                ICollection<string> users = kvp.Value.Users;
                if (users.Count < 2)
                {
                    Console.WriteLine("Joining an existing room: " + kvp.Value.Name);

                    users.Add(Context.ConnectionId);
                    createNewRoom = false;
                    gameId = kvp.Value.Id;
                    break;
                }
            }

            if (createNewRoom)
            {
                ICollection<string> users = new List<string>();
                users.Add(Context.ConnectionId);
                _rooms[id] = new GameRoom()
                {
                    Id = id,
                    Name = "Game Room " + new Random().Next(10000),
                    Users = users
                };
                Console.WriteLine("Creating a new room with name: " + _rooms[id].Name);
            }

            return gameId;
        }
    }
}
