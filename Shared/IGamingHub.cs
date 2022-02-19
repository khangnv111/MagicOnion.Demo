using MagicOnion;
using MessagePack;
using System.Numerics;
using System.Threading.Tasks;

namespace Shared
{
    // Client -> Server definition
    // implements `IStreamingHub<TSelf, TReceiver>`  and share this type between server and client.
    public interface IGamingHub : IStreamingHub<IGamingHub, IGamingHubReceiver>
    {
        // return type should be `Task` or `Task<T>`, parameters are free.
        Task JoinAsync(Player player);
        Task JoinAsync2(string name);
        Task LeaveAsync();
        Task MoveAsync(Vector3 position);
        Task MoveAsync2(string name);

        Task SendMessageAsync(string message);
    }

    // Server -> Client definition
    public interface IGamingHubReceiver
    {
        // return type should be `void` or `Task`, parameters are free.
        void OnJoin(string name);
        void OnLeave(string name);
        void OnMove(Player player);
        void OnMove2(string name);

        void OnSendMessage(string name, string message);
    }

    // for example, request object by MessagePack.
    [MessagePackObject]
    public class Player
    {
        [Key(0)]
        public string Name { get; set; }
        [Key(1)]
        public Vector3 Position { get; set; }
        [Key(2)]
        public Quaternion Rotation { get; set; }
    }
}
