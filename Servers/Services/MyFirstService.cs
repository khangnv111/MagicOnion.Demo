using Grpc.Core;
using MagicOnion;
using MagicOnion.Server;
using Microsoft.Extensions.Logging;
using Shared;
using System.Threading.Tasks;

namespace Servers.Services
{
    // implement RPC service.
    // inehrit ServiceBase<interface>, interface
    public class MyFirstService : ServiceBase<IMyFirstService>, IMyFirstService
    {
        ILogger<MyFirstService> logger;

        public MyFirstService(ILogger<MyFirstService> logger)
        {
            this.logger = logger;
        }

        public async UnaryResult<int> SumAsync(int x, int y)
        {
            //Console.WriteLine($"Received:{x}, {y}");
            return x + y;
        }

        #region Streaming
        public async Task<UnaryResult<string>> SumLegacyTaskAsync(int x, int y)
        {
            logger.LogDebug($"Called SumAsync - x:{x} y:{y}");

            // use UnaryResult method.
            return UnaryResult((x + y).ToString());
        }

        public async Task<ClientStreamingResult<int, string>> ClientStreamingSampleAsync()
        {
            logger.LogDebug($"Called ClientStreamingSampleAsync");

            // If ClientStreaming, use GetClientStreamingContext.
            var stream = GetClientStreamingContext<int, string>();

            // receive from client asynchronously
            await foreach (var x in stream.ReadAllAsync())
            {
                logger.LogDebug("Client Stream Received:" + x);
            }

            // StreamingContext.Result() for result value.
            return stream.Result("finished");
        }

        public async Task<ServerStreamingResult<string>> ServerStreamingSampleAsync(int x, int y, int z)
        {
            logger.LogDebug($"Called ServertSreamingSampleAsync - x:{x} y:{y} z:{z}");

            var stream = GetServerStreamingContext<string>();

            var acc = 0;
            for (int i = 0; i < z; i++)
            {
                acc = acc + x + y;
                await stream.WriteAsync(acc.ToString());
            }

            return stream.Result();
        }

        public async Task<DuplexStreamingResult<int, string>> DuplexStreamingSampleAsync()
        {
            logger.LogDebug($"Called DuplexStreamingSampleAync");

            // DuplexStreamingContext represents both server and client streaming.
            var stream = GetDuplexStreamingContext<int, string>();

            var waitTask = Task.Run(async () =>
            {
                // ForEachAsync(MoveNext, Current) can receive client streaming.
                await foreach (var x in stream.ReadAllAsync())
                {
                    logger.LogDebug($"Duplex Streaming Received:" + x);
                }
            });

            // WriteAsync is ServerStreaming.
            await stream.WriteAsync("test1");
            await stream.WriteAsync("test2");
            await stream.WriteAsync("finish");

            await waitTask;

            return stream.Result();
        }
        #endregion

    }
}
