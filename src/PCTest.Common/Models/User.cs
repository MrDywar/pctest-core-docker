using ProtoBuf;
using System;

namespace PCTestCommon.Models
{
    [Serializable]
    [ProtoContract]
    public class User
    {
        [ProtoMember(1)]
        public long Id { get; set; }

        [ProtoMember(2)]
        public string Name { get; set; }

        [ProtoMember(3)]
        public int Age { get; set; }
    }
}