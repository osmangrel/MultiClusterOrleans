# readme

> Multi Cluster Example with ORLEANS

> Using MongoDb for Membership Table

> N Silo - N Client

[Orleans Multi Cluster , Membership Table doc](https://dotnet.github.io/orleans/docs/implementation/cluster_management.html)

Multi Clustering yapabilmek için ** Membership ** Table kullanılır. Membership Table ayarlamasında kullanılan 5 yöntem vardır.
Bunlar: Azure Table Storage, SQL Server, Apache ZooKeeper, Consul IO, AWS DynamoDB, Memory Emulation (Development ortamı için).
Projede Sql server yöntemi seçilip MongoDb Provider projeye eklenmiştir. [MongoDb Provider Github Repo](https://github.com/OrleansContrib/Orleans.Providers.MongoDB) 
SiloHost -> Program.cs -> .UseOrleans() -> özelliklerinin içerisinde
                    siloBuilder.ConfigureMongoDbClusteringOptions(connectionString, createShardKey, dbName);  methodu ile mongoDB ayarlamaları yapılacak methodu çağırıyoruz.
```
public static void ConfigureMongoDbClusteringOptions(this ISiloBuilder siloBuilder, string connectionString, bool createShardKey, string dbName)
        {
            siloBuilder.UseMongoDBClient(connectionString);
            siloBuilder.UseMongoDBClustering(o =>
            {
                o.DatabaseName = dbName;
                o.CreateShardKeyForCosmos = createShardKey;
            });
            siloBuilder.UseMongoDBReminders(o =>
            {
                o.DatabaseName = dbName;
                o.CreateShardKeyForCosmos = createShardKey;
            });
            siloBuilder.AddMongoDBGrainStorage("PubSubStore", options =>
            {
                options.DatabaseName = "OrleansTestAppPubSubStore";
                options.CreateShardKeyForCosmos = createShardKey;
                options.ConfigureJsonSerializerSettings = settings =>
                {
                    settings.NullValueHandling = NullValueHandling.Include;
                    settings.ObjectCreationHandling = ObjectCreationHandling.Replace;
                    settings.DefaultValueHandling = DefaultValueHandling.Populate;
                };
            });
            siloBuilder.AddMongoDBGrainStorage("MongoDBStore", options =>
             {
                 options.DatabaseName = "OrleansTestApp";
                 options.CreateShardKeyForCosmos = createShardKey;

                 options.ConfigureJsonSerializerSettings = settings =>
                 {
                     settings.NullValueHandling = NullValueHandling.Include;
                     settings.ObjectCreationHandling = ObjectCreationHandling.Replace;
                     settings.DefaultValueHandling = DefaultValueHandling.Populate;
                 };

             });
        }
```

Orleans ismini vermiş olduğumuz db’yi MongoDb’de yaratacaktır ve Provider bizim için gerekli collectionları vs kendisi oluşturup yönetecektir.
##Client tarafında
```

ClientBuilder ile Client startup yapılandırmasında;
  .UseMongoDBClient(connectionString)
  .UseMongoDBClustering(options =>
{
options.DatabaseName = "OrleansTestApp";
             options.CreateShardKeyForCosmos = createShardKey;
       });
```

Özelliğini vermemiz yeterlidir. Silo yapılandırmasında verdiğimiz connectionString ve database ismini belirtmemiz yeterli olacaktır.



