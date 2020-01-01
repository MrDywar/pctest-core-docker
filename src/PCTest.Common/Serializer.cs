﻿using System.IO;
using System.Runtime.Serialization.Json;

namespace PCTestCommon
{
    public class Serializer
    {
        public static byte[] JsonSerializer(object obj)
        {
            if (obj == null)
                return null;

            var serializer = new DataContractJsonSerializer(typeof(PCTestCommon.Models.User));

            using (MemoryStream stream = new MemoryStream())
            {
                serializer.WriteObject(stream, obj);
                return stream.ToArray();
            }
        }

        public static object JsonDeserializer(byte[] buffer)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(PCTestCommon.Models.User));
                stream.Write(buffer, 0, buffer.Length);
                stream.Position = 0;
                return serializer.ReadObject(stream);
            }
        }

        public static byte[] ProtobufSerializer(object obj)
        {
            using (var memStream = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(memStream, obj);
                return memStream.ToArray();
            }
        }

        public static T ProtobufDeserializer<T>(byte[] buffer)
        {
            using (var memStream = new MemoryStream(buffer))
            {
                return ProtoBuf.Serializer.Deserialize<T>(memStream);
            }
        }
    }
}
