using System;
using Grains;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;
using Orleans;
using Orleans.Hosting;
using Orleans.Statistics;
using System.Threading.Tasks;
using Grains.src;

namespace SiloHost
{
    class Program
    {
        public static Task Main()
        {
            //https://github.com/OrleansContrib/Orleans.Providers.MongoDB
            //https://dotnet.github.io/orleans/docs/implementation/cluster_management.html


            var connectionString = "mongodb://***.***.***.***:27017/OrleansTestApp";
            var createShardKey = true;
            return new HostBuilder()
                .ConfigureLogging(logging => logging.AddConsole())
                //Registering a Configuration source for Feature Management.
                .ConfigureAppConfiguration(config => config.AddJsonFile("appsettings.json").AddEnvironmentVariables())
                .UseOrleans((hostBuilderContext, siloBuilder) =>
                {
                    var orleansSettings = hostBuilderContext.Configuration.GetSection(nameof(OrleansHostSettings)).Get<OrleansHostSettings>();
                    orleansSettings.Validate();

                    siloBuilder.UseLinuxEnvironmentStatistics();
                    siloBuilder.ConfigureDashboardOptions(
                        "piotr",
                        "orleans",
                        orleansSettings.DashboardPort);
                    siloBuilder.ConfigureMongoDbClusteringOptions(connectionString, createShardKey, "OrleansTestApp");

                    //siloBuilder.ConfigureDynamoDbClusteringOptions(
                    //    orleansSettings.MembershipTableName,
                    //    orleansSettings.AwsRegion);
                    siloBuilder.ConfigureClusterOptions(
                        "cluster-of-silos",
                        "hello-world-service");

                    var isLocal = orleansSettings.IsLocal;

                    if (isLocal.Value)
                    {
                        siloBuilder.ConfigureEndpointOptions(
                            orleansSettings.GatewayPort,
                            orleansSettings.SiloPort,
                            orleansSettings.AdvertisedIp);
                    }
                    else
                    {
                        siloBuilder.ConfigureEndpointOptions(
                            orleansSettings.EcsContainerMetadataUri);
                    }

                    siloBuilder.ConfigureApplicationParts(applicationPartManager =>
                    {
                        applicationPartManager.AddApplicationPart(typeof(HelloWorld).Assembly).WithReferences();
                        applicationPartManager.AddApplicationPart(typeof(CarGrain).Assembly).WithReferences();
                        applicationPartManager.AddApplicationPart(typeof(CreatureGrain).Assembly).WithReferences();
                        applicationPartManager.AddApplicationPart(typeof(HumanGrain).Assembly).WithReferences();

                    });

                    /*Registering Feature Management, to allow DI of IFeatureManagerSnapshot in HelloWorld grain.
                        Using built in Percentage filter to demonstrate a feature being on/off.*/
                    siloBuilder.ConfigureServices(serviceCollection =>
                    {
                        serviceCollection.AddFeatureManagement()
                            .AddFeatureFilter<PercentageFilter>();
                    });
                })
                .RunConsoleAsync();
        }
    }
}