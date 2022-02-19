using MagicOnion;
using System;
using System.Threading.Tasks;

namespace Shared
{
    //internal class IMyFirstService
    //{
    //}

    // Defines .NET interface as a Server/Client IDL.
    // The interface is shared between server and client.
    public interface IMyFirstService : IService<IMyFirstService>
    {
        // The return type must be `UnaryResult<T>`.
        UnaryResult<int> SumAsync(int x, int y);

        #region Streaming
        Task<UnaryResult<string>> SumLegacyTaskAsync(int x, int y);
        Task<ClientStreamingResult<int, string>> ClientStreamingSampleAsync();
        Task<ServerStreamingResult<string>> ServerStreamingSampleAsync(int x, int y, int z);
        Task<DuplexStreamingResult<int, string>> DuplexStreamingSampleAsync();
        #endregion

    }
}
