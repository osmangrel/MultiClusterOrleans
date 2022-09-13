using Interfaces;
using Orleans;
using Orleans.Configuration;
using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
namespace ClientConsole
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //https://github.com/OrleansContrib/Orleans.Providers.MongoDB
            //https://dotnet.github.io/orleans/docs/implementation/cluster_management.html
            var connectionString = "mongodb://2.12.100.62:27017/OrleansTestApp";
            var createShardKey = true;

            var client = new ClientBuilder()
                .ConfigureApplicationParts(options =>
                {
                    options.AddApplicationPart(typeof(IHelloWorld).Assembly);
                })
                .UseMongoDBClient(connectionString)
                .UseMongoDBClustering(options =>
                {
                    options.DatabaseName = "OrleansTestApp";
                    options.CreateShardKeyForCosmos = createShardKey;
                })
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "cluster-of-silos";
                    options.ServiceId = "hello-world-service";
                })
                //.ConfigureLogging(log => log.AddConsole())
                .Build();

            await client.Connect();

            await TestBasic(client, "100.62 first opening message");
            while (true)
            {
                Console.Write("1-HelloWorld Grain \n2-Car Grain \n3-Creature Grain \n4-Human Grain\n   ");
                string grainType = Console.ReadLine();
                await SetGrainWithParameter(grainType, client);
                await Task.Delay(2000);
            }
        }

        private static async Task SetGrainWithParameter(string grainType, IClusterClient client)
        {
            Guid n = Guid.NewGuid();
            string milliSec = DateTime.Now.ToString();
            switch (grainType)
            {
                case "1":
                    var helloWorldGrain = client.GetGrain<IHelloWorld>("35abc123");
                    await helloWorldGrain.SayHello($"svrnk - {n}: HelloWorld Grain {milliSec}", new GrainCancellationTokenSource().Token);
                    break;
                case "2":
                    var carGrain = client.GetGrain<ICarGrain>(n);
                    await carGrain.GetCarAbilities($"svrnk - {n}: Car Grain {milliSec}", new GrainCancellationTokenSource().Token);
                    break;
                case "3":
                    var creatureGrain = client.GetGrain<ICreatureGrain>(n);
                    await creatureGrain.GetCreatureAbilities($"svrnk - {n}: Creature Grain {milliSec}", new GrainCancellationTokenSource().Token);
                    break;
                case "4":
                    var humanGrain = client.GetGrain<IHumanGrain>(n);
                    await humanGrain.GetHumanAbilities($"svrnk - {n}: Human Grain {milliSec}", new GrainCancellationTokenSource().Token);
                    break;
                default:
                    break;
            }
            Console.WriteLine("-*-*- Mesajınız Silo'ya gönderildi. *-*-*  ");
        }
        private static async Task TestBasic(IClusterClient client, string mesaj)
        {
            Guid n = Guid.NewGuid();
            var helloWorldGrain = client.GetGrain<IHelloWorld>("35abc123");
            string milliSec = DateTime.Now.ToString() + " - " + DateTime.Now.Millisecond.ToString();
            await helloWorldGrain.SayHello($"svrnk.62:  {mesaj}  -  {milliSec}", new GrainCancellationTokenSource().Token);
            Console.WriteLine("-*-*- Mesajınız Silo'ya gönderildi. *-*-*  ");
        }
    }
}
