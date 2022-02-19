using Grpc.Core;
using Grpc.Net.Client;
using MagicOnion.Client;
using Shared;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Channels;
using System.Threading.Tasks;
//using UnityEngine;

namespace App.Client
{
    class Program : IGamingHubReceiver
    {
        IGamingHub gamingHub;
        static Task Main(string[] args)
        {
            return new Program().MainCore(args);
        }

        private async Task MainCore(string[] args)
        {
            try
            {
                // Connect to the server using gRPC channel.
                var channel = GrpcChannel.ForAddress("https://localhost:5001");
                //var channel = GrpcChannelx.ForTarget(new GrpcChannelTarget("localhost", 12345, ChannelCredentials.Insecure));

                // Create a proxy to call the server transparently.
                //var client = MagicOnionClient.Create<IMyFirstService>(channel);

                this.gamingHub = await StreamingHubClient.ConnectAsync<IGamingHub, IGamingHubReceiver>(channel, this);

                //var result = await client.SumAsync(123, 456);
                //Console.WriteLine(string.Format("result: {0}", result.ToString()));

                //await UnaryRun(client);
                //await ServerStreamRun(client);
                //await DuplexStreamRun(client);
                //await ClientStreamRun(client);
                this.GameHubTest();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ex: ", ex.ToString());
            }

            Console.ReadLine();
        }

        #region FirstService
        static async Task UnaryRun(IMyFirstService client)
        {
            // await(C# 7.0, Unity 2018.3+)
            var vvvvv = await client.SumAsync(10, 20);
            Console.WriteLine("UnaryRun SumAsync:" + vvvvv);

            // if use Task<UnaryResult>(Unity 2018.2), use await await
            //var vvvv2 = await await client.SumLegacyTaskAsync(10, 20);
        }

        static async Task ClientStreamRun(IMyFirstService client)
        {
            Console.WriteLine("ClientStreamRun===========");
            var stream = await client.ClientStreamingSampleAsync();

            for (int i = 0; i < 3; i++)
            {
                await stream.RequestStream.WriteAsync(i);
            }
            await stream.RequestStream.CompleteAsync();

            var response = await stream.ResponseAsync;

            Console.WriteLine("Response:" + response);
        }

        static async Task ServerStreamRun(IMyFirstService client)
        {
            Console.WriteLine("ServerStreamRun===========");
            var stream = await client.ServerStreamingSampleAsync(10, 20, 3);

            await foreach (var x in stream.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine("ServerStream Response:" + x);
            }
        }

        static async Task DuplexStreamRun(IMyFirstService client)
        {
            Console.WriteLine("DuplexStreamRun===========");
            var stream = await client.DuplexStreamingSampleAsync();

            var count = 0;
            await foreach (var x in stream.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine("DuplexStream Response:" + x);

                await stream.RequestStream.WriteAsync(count++);
                if (x == "finish")
                {
                    await stream.RequestStream.CompleteAsync();
                }
            }
        }
        #endregion

        #region Streaming
        public void DisposeAsync()
        {
            this.gamingHub.DisposeAsync();
        }

        async void GameHubTest()
        {
            // set player
            var player = new Player
            {
                Name = "Player1",
                Position = new Vector3(1, 1, 1),
                Rotation = new Quaternion(1, 1, 1, 1)
            };

            await this.gamingHub.JoinAsync2(player.Name);
            //await this.gamingHub.JoinAsync(player);

            // send mess
            await this.gamingHub.SendMessageAsync(String.Format("Welcome {0}!!!!!", player.Name));

            // move
            //player.Position = new Vector3(1, 2, 0);
            //await this.gamingHub.MoveAsync(player.Position);
            await this.gamingHub.MoveAsync2(player.Name);

            // leave game
            await this.gamingHub.LeaveAsync();
        }

        void IGamingHubReceiver.OnJoin(string name)
        {
            //Debug.Log($"{name} join game");
            Console.WriteLine($"{name} join game");
        }

        void IGamingHubReceiver.OnLeave(string name)
        {
            //Debug.Log($"{name} leave game");
            Console.WriteLine($"{name} leave game");
        }

        void IGamingHubReceiver.OnSendMessage(string name, string message)
        {
            Console.WriteLine($"{name}: {message}");
        }

        void IGamingHubReceiver.OnMove(Player player)
        {
            //Debug.Log("Move Player:" + player.Name);
            Console.WriteLine($"{player.Name} move to {player.Position.X} {player.Position.Y} {player.Position.Z}");
        }

        void IGamingHubReceiver.OnMove2(string name)
        {
            //Debug.Log("Move Player:" + player.Name);
            Console.WriteLine($"{name} moved");
        }
        #endregion
    }
}
