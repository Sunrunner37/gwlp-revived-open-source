namespace GameServer.Commands
{
        [System.AttributeUsage(System.AttributeTargets.Class,
                       AllowMultiple = false)  // multiuse attribute
        ]
        public class CommandAttribute : System.Attribute
        {
                public string Description;
        }
}
