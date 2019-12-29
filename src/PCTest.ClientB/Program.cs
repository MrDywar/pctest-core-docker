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

            RedisReadMethod();
            NatsReadMethod();

            Console.WriteLine("END");
        }

        private static void RedisReadMethod()
        {
            var manager = new RedisManagerPool($"{Config.DOCKER_MACHINE_IP}:6379");
            using (var client = manager.GetClient())
            {
                var user = client.Get<User>("foo");
                Console.WriteLine($"Redis: ID: {user.Id}, Name: {user.Name}, Age:{user.Age}");
            }
        }

        private static void NatsReadMethod()
        {
            using (var c = new NC.ConnectionFactory().CreateEncodedConnection($"http://{Config.DOCKER_MACHINE_IP}:4222"))
            {
                c.OnDeserialize = Serializer.JsonDeserializer;

                EventHandler<NC.EncodedMessageEventArgs> eh = (sender, args) =>
                {
                    var user = (User)args.ReceivedObject;
                    Console.WriteLine($"Nats: ID: {user.Id}, Name: {user.Name}, Age:{user.Age}");
                };

                using (var s = c.SubscribeAsync("foo", eh))
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
    }
}
