using System;

namespace ServerEngine.PacketManagement.CustomAttributes
{
        [System.AttributeUsage(System.AttributeTargets.Class,
                       AllowMultiple = false)  // multiuse attribute
        ]
        public class PacketAttributes : System.Attribute
        {
                public bool IsIncoming { get; set; }
                public UInt16 Header { get; set; }
        }
}