using PCTestCommon;
using PCTestCommon.Models;
using ServiceStack.Redis;
using System;
using System.Threading;
using NC = NATS.Client;

namespace PCTestClientB
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Client B");
            Console.WriteLine("Enter any string to exit.");

            NatsSubscribeMethod();

            Console.WriteLine("END");
        }

        private static void NatsSubscribeMethod()
        {
            using (var c = new NC.ConnectionFactory().CreateEncodedConnection($"http://{Config.DOCKER_MACHINE_IP}:4222"))
            {
                c.OnDeserialize = Serializer.JsonDeserializer;

                EventHandler<NC.EncodedMessageEventArgs> eh = (sender, args) =>
                {
                    var user = (User)args.ReceivedObject;
                    var redisUser = GetUserFromRedis(user.Id.ToString());

                    Console.WriteLine(new string('*', 10));
                    Console.WriteLine($"User from NATS: ID: {user.Id}, Name: {user.Name}, Age:{user.Age}");
                    Console.WriteLine($"User from REDIS: ID: {redisUser.Id}, Name: {redisUser.Name}, Age:{redisUser.Age}");
                    Console.WriteLine($"Are users equal: {user.Equals(redisUser)}");
                };

                using (var s = c.SubscribeAsync("users", eh))
                {
                    Console.WriteLine("Waiting for a message..");
                    Console.WriteLine();

                    while (!string.IsNullOrEmpty(Console.ReadLine()))
                    {
                        Thread.Sleep(100);
                    }
                }
            }
        }

        private static User GetUserFromRedis(string userId)
        {
            var manager = new RedisManagerPool($"{Config.DOCKER_MACHINE_IP}:6379");
            using (var client = manager.GetClient())
            {
                return client.Get<User>(userId);
            }
        }
    }
}
