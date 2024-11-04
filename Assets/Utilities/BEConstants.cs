namespace Game.Core
{
    public static class BEConstants
    {
        public const string ScreenDataLocation = "Assets/Game/Shared/UIs/BEScreenReferences.asset"; 
        public const string DialogDataLocation = "Assets/Game/Shared/UIs/BEDialogReferences.asset";
    }

    public static class BEDefines
    {
        /// <summary>
        /// Used to enable log in the app
        /// </summary>
        public const string EnableLog = "ENABLE_LOG";
        /// <summary>
        /// Unity editor flag
        /// </summary>
        public const string UnityEditor = "UNITY_EDITOR";
        /// <summary>
        /// Used to let the app the release features should be enabled
        /// </summary>
        public const string ReleaseBuild = "RELEASE_BUILD";
        /// <summary>
        /// Used to let the app the staging features should be enabled
        /// </summary>
        public const string StagingBuild = "STAGING_BUILD";
        /// <summary>
        /// Used to enable Unity development features belong with the build
        /// </summary>
        public const string DevelopmentBuild = "DEVELOPMENT_BUILD";
        /// <summary>
        /// Used to enable internal features such as the cheating
        /// </summary>
        public const string InternalBuild = "INTERNAL_BUILD";
        /// <summary>
        /// Used to let the app know its server
        /// </summary>
        public const string DedicatedServerBuild = "DEDICATED_SERVER";
        /// <summary>
        /// Used to eanble PlayFab for multiplayer server, belong with EnableHostingAgent
        /// </summary>
        public const string PlayFabAPIServer = "ENABLE_PLAYFABSERVER_API";
        /// <summary>
        /// To enable IL2cpp
        /// </summary>
        public const string EnableIL2CppScriptBackend = "ENABLE_IL2CPP_SCRIPT_BACKEND";
        /// <summary>
        /// Used to let the app know it should not use the networking methods
        /// </summary>
        public const string OfflineGameMode = "OFFLINE_MODE";
        /// <summary>
        /// Use to activate hosting agent such as PlayFabMPSAgentListener/ UMutiplayAgent which depend on your chosen
        /// </summary>
        public const string EnableHostingAgent = "ENABLE_HOSTING_AGENT";
        /// <summary>
        /// Use to enable UMutiplayAgent, should go belong with EnableHostingAgent
        /// </summary>
        public const string EnableUnityMultiplayAgent = "ENABLE_UNITY_MULTIPLAY_AGENT";
    }
}
