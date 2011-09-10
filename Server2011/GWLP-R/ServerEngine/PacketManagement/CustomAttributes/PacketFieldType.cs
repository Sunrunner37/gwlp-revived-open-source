namespace ServerEngine.PacketManagement.CustomAttributes
{
        [System.AttributeUsage(System.AttributeTargets.Field,
                       AllowMultiple = false)
        ]
        public class PacketFieldType : System.Attribute
        {
                public bool ConstSize { get; set; }
                public int MaxSize { get; set; }
        }
}