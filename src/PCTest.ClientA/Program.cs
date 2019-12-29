using PCTestCommon;
using PCTestCommon.Models;
using ProtoBuf;
using ServiceStack.Redis;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AC = Aerospike.Client;
using NC = NATS.Client;

namespace PCTestClientA
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Client A");

            var user = new User() { Id = 1, Name = "Dywar", Age = 33 };

            await ClickhouseReadMethod();

            AerospikeWriteMethod(user);
            AerospikeReadMethod(user.Id);

            RedisWriteMethod(user);
            NatsPublishMethod(user);
        }

        private static async Task ClickhouseReadMethod()
        {
            using (var client = new HttpClient { BaseAddress = new Uri($"http://{Config.DOCKER_MACHINE_IP}:8123") })
            {
                var request = "SELECT * FROM test.User FORMAT Protobuf SETTINGS format_schema = 'schemafile:User'";
                var response = await client.PostAsync("", new StringContent(request));

                Console.WriteLine(await response.Content.ReadAsStringAsync());

                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    var users = ProtoBuf.Serializer.DeserializeItems<User>(responseStream, PrefixStyle.Base128, 0).ToList();

                    foreach (var user in users)
                    {
                        Console.WriteLine($"Clickhouse: ID: {user.Id}, Name: {user.Name}, Age:{user.Age}");
                    }
                }
            }
        }

        private static void AerospikeWriteMethod(User user)
        {
            using (var client = new AC.AerospikeClient(Config.DOCKER_MACHINE_IP, 3000))
            {
                if (!client.Connected)
                {
                    Console.WriteLine("Aerospike ERROR: Connection failed!");
                    return;
                }

                var wPolicy = new AC.WritePolicy
                {
                    recordExistsAction = AC.RecordExistsAction.UPDATE
                };

                var key = new AC.Key("test", "users", user.Id);
                var binId = new AC.Bin("id", user.Id);
                var binName = new AC.Bin("name", user.Name);
                var binAge = new AC.Bin("age", user.Age);

                client.Put(wPolicy, key, binId, binName, binAge);
            }
        }

        private static void AerospikeReadMethod(long userId)
        {
            using (var client = new AC.AerospikeClient(Config.DOCKER_MACHINE_IP, 3000))
            {
                if (!client.Connected)
                {
                    Console.WriteLine("Aerospike ERROR: Connection failed!");
                    return;
                }

                var key = new AC.Key("test", "users", userId);

                var user = client.Get(null, key);
                if (user != null)
                {
                    Console.WriteLine($"Aerospike: ID: {user.GetValue("id")}, Name: {user.GetValue("name")}, Age:{user.GetValue("age")}");
                }
                else
                {
                    Console.WriteLine("Aerospike ERROR: User record not found!");
                }
            }
        }

        private static void RedisWriteMethod(User user)
        {
            var manager = new RedisManagerPool($"{Config.DOCKER_MACHINE_IP}:6379");
            using (var client = manager.GetClient())
            {
                client.Set("foo", user);
            }
        }

        private static void NatsPublishMethod(User user)
        {
            using (var c = new NC.ConnectionFactory().CreateEncodedConnection($"http://{Config.DOCKER_MACHINE_IP}:4222"))
            {
                c.OnSerialize = PCTestCommon.Serializer.JsonSerializer;

                c.Publish("foo", user);
            }
        }
    }
}
