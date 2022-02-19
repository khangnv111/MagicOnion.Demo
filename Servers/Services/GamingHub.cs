using MagicOnion.Server.Hubs;
using Shared;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
//using UnityEngine;

namespace Servers.Services
{
    // Server implementation
    // implements : StreamingHubBase<THub, TReceiver>, THub
    public class GamingHub : StreamingHubBase<IGamingHub, IGamingHubReceiver>, IGamingHub
    {
        // this class is instantiated per connected so fields are cache area of connection.
        IGroup room;
        Player self;
        IInMemoryStorage<Player> storage;

        public async Task JoinAsync(Player player)
        {
            string roomName = "Room 1";
            self = player;

            // Group can bundle many connections and it has inmemory-storage so add any type per group. 
            //this.room = await this.Group.AddAsync(roomName);
            (room, storage) = await Group.AddAsync(roomName, self);

            // Typed Server->Client broadcast.
            Broadcast(room).OnJoin(self.Name);

            //return storage.AllValues.ToArray();
        }

        public async Task JoinAsync2(string name)
        {
            string roomName = "Room 1";
            self = new Player
            {
                Name = name,
                Position = new Vector3(1, 1, 1),
                Rotation = new Quaternion(1, 1, 1, 1)
            };

            // Group can bundle many connections and it has inmemory-storage so add any type per group. 
            //this.room = await this.Group.AddAsync(roomName);
            (room, storage) = await Group.AddAsync(roomName, self);

            // Typed Server->Client broadcast.
            Broadcast(room).OnJoin(self.Name);

            storage.AllValues.ToArray();
            //return storage.AllValues.ToArray();
        }

        public async Task LeaveAsync()
        {
            await room.RemoveAsync(this.Context);
            Broadcast(room).OnLeave(self.Name);
        }

        public async Task MoveAsync(Vector3 position)
        {
            self.Position = position;
            Broadcast(room).OnMove(self);
        }

        public async Task MoveAsync2(string name)
        {
            //self.Position = position;
            Broadcast(room).OnMove2(name);
        }

        public async Task SendMessageAsync(string message)
        {
            this.Broadcast(room).OnSendMessage(self.Name, message);
        }

        // You can hook OnConnecting/OnDisconnected by override.
        protected override ValueTask OnDisconnected()
        {
            // on disconnecting, if automatically removed this connection from group.
            return CompletedTask;
        }
    }
}
