
namespace Game.Core.Utilities
{
    using System.Collections.Generic;
    
    public static class EnvironmentArguments
    {
        public static class Key
        {
            public const string ApplicationMode = "-app-mode";
            public const string NetworkMode = "-net-mode";
            /// <summary>
            /// The port at which the specific server/client instance is accessible.
            /// </summary>
            public const string Port = "-port";
            /// <summary>
            /// The IP address of the server instance.
            /// </summary>
            public const string Ipv4 = "-ipv4";
            /// <summary>
            /// The IPv6 address of the server instance, if available.
            /// </summary>
            public const string Ipv6 = "-ipv6";

            // Additional for Unity Game Server Hosting (Multiplay)
            /// <summary>
            /// The unique ID of the allocation
            /// </summary>
            public const string AllocatedID = "-allocatedID";
            /// <summary>
            /// The IP address of the server instance. (Ipv4)
            /// </summary>
            public const string Ip = "-ip";
            /// <summary>
            /// The unique identifier of the server instance.
            /// </summary>
            public const string ServerID = "-serverID";
            /// <summary>
            /// The query protocol the server instance uses.
            /// </summary>
            public const string QueryType = "-queryType";
            /// <summary>
            /// The port at which you can access the query protocol data.
            /// </summary>
            public const string QueryPort = "-queryPort"; 
        }

        public static Dictionary<string, string> StartupArguments { get; private set; }

        static EnvironmentArguments()
        {
            if (StartupArguments == null)
            {
                StartupArguments = GetCommandlineArgs();
                Logger.BELogger.LogI($"EnvironmentArguments");
                foreach (var item in StartupArguments)
                {
                    Logger.BELogger.LogI($"{item.Key}:{item.Value}");
                }
            }
        }

        public static Dictionary<string, string> GetCommandlineArgs()
        {
            Dictionary<string, string> argDictionary = new Dictionary<string, string>();

            string[] args = System.Environment.GetCommandLineArgs();

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i].ToLower();
                if (arg.StartsWith("-"))
                {
                    string value = i < args.Length - 1 ? args[i + 1].ToLower() : null;
                    if (!string.IsNullOrEmpty(value))
                    {
                        value = value.StartsWith("-") ? null : value;
                    }
                    argDictionary.Add(arg, value);
                }
            }
            return argDictionary;
        }   
    }

    public interface IEnvironmentOptional
    {
        public string Key { get; }
    }
}
