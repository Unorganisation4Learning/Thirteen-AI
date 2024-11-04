namespace Game.Core.Logger
{
    public class BELogger // : Resolver.Resolvable<BELogger>
    {
        private static ILogger _logger;
        static BELogger()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD || ENABLE_LOG
            _logger = new UnityLogger();
            UnityEngine.Debug.unityLogger.logEnabled = true;
#else
            _logger = new DisabledLogger();
		    UnityEngine.Debug.unityLogger.logEnabled = false;		
#endif
        }

        //protected override void OnDispose()
        //{
        //    _logger = null;
        //}

        [System.Diagnostics.Conditional(BEDefines.UnityEditor), System.Diagnostics.Conditional(BEDefines.DevelopmentBuild), System.Diagnostics.Conditional(BEDefines.EnableLog)]
        public static void LogI(object obj)
        {
            _logger.LogI(obj);
        }

        [System.Diagnostics.Conditional(BEDefines.UnityEditor), System.Diagnostics.Conditional(BEDefines.DevelopmentBuild), System.Diagnostics.Conditional(BEDefines.EnableLog)]
        public static void LogW(object obj)
        {
            _logger.LogW(obj);
        }

        [System.Diagnostics.Conditional(BEDefines.UnityEditor), System.Diagnostics.Conditional(BEDefines.DevelopmentBuild), System.Diagnostics.Conditional(BEDefines.EnableLog)]
        public static void LogE(object obj)
        {
            _logger.LogE(obj);
        }

        [System.Diagnostics.Conditional(BEDefines.UnityEditor), System.Diagnostics.Conditional(BEDefines.DevelopmentBuild), System.Diagnostics.Conditional(BEDefines.EnableLog)]
        public static void LogIFormat(string format, params object[] args)
        {
            _logger.LogIFormat(format, args);
        }

        [System.Diagnostics.Conditional(BEDefines.UnityEditor), System.Diagnostics.Conditional(BEDefines.DevelopmentBuild), System.Diagnostics.Conditional(BEDefines.EnableLog)]
        public static void LogWFormat(string format, params object[] args)
        {
            _logger.LogWFormat(format, args);
        }

        [System.Diagnostics.Conditional(BEDefines.UnityEditor), System.Diagnostics.Conditional(BEDefines.DevelopmentBuild), System.Diagnostics.Conditional(BEDefines.EnableLog)]
        public static void LogEFormat(string format, params object[] args)
        {
            _logger.LogEFormat(format, args);
        }

        // Extra
        [System.Diagnostics.Conditional(BEDefines.UnityEditor), System.Diagnostics.Conditional(BEDefines.DevelopmentBuild), System.Diagnostics.Conditional(BEDefines.EnableLog)]
        public static void LogI(string tag, object obj)
        {
            LogI($"[{tag}] {obj}");
        }

        [System.Diagnostics.Conditional(BEDefines.UnityEditor), System.Diagnostics.Conditional(BEDefines.DevelopmentBuild), System.Diagnostics.Conditional(BEDefines.EnableLog)]
        public static void LogW(string tag, object obj)
        {
            LogW($"[{tag}] {obj}");
        }

        [System.Diagnostics.Conditional(BEDefines.UnityEditor), System.Diagnostics.Conditional(BEDefines.DevelopmentBuild), System.Diagnostics.Conditional(BEDefines.EnableLog)]
        public static void LogE(string tag, object obj)
        {
            LogE($"[{tag}] {obj}");
        }

        [System.Diagnostics.Conditional(BEDefines.UnityEditor), System.Diagnostics.Conditional(BEDefines.DevelopmentBuild), System.Diagnostics.Conditional(BEDefines.EnableLog)]
        public static void LogIFormat(string tag, string format, params object[] args)
        {
            LogIFormat($"[{tag}] {format}", args);
        }

        [System.Diagnostics.Conditional(BEDefines.UnityEditor), System.Diagnostics.Conditional(BEDefines.DevelopmentBuild), System.Diagnostics.Conditional(BEDefines.EnableLog)]
        public static void LogWFormat(string tag, string format, params object[] args)
        {
            LogWFormat($"[{tag}] {format}", args);
        }

        [System.Diagnostics.Conditional(BEDefines.UnityEditor), System.Diagnostics.Conditional(BEDefines.DevelopmentBuild), System.Diagnostics.Conditional(BEDefines.EnableLog)]
        public static void LogEFormat(string tag, string format, params object[] args)
        {
            LogEFormat($"[{tag}] {format}", args);
        }

        // -- Extra
    }

    public static class LogExtension
    {
        [System.Diagnostics.Conditional(BEDefines.UnityEditor), System.Diagnostics.Conditional(BEDefines.DevelopmentBuild), System.Diagnostics.Conditional(BEDefines.EnableLog)]
        public static void LogI(this object any, object obj)
        {
            BELogger.LogI($"{any.GetType()}", obj);
        }

        [System.Diagnostics.Conditional(BEDefines.UnityEditor), System.Diagnostics.Conditional(BEDefines.DevelopmentBuild), System.Diagnostics.Conditional(BEDefines.EnableLog)]
        public static void LogW(this object any, object obj)
        {
            BELogger.LogE($"{any.GetType()}", obj);
        }

        [System.Diagnostics.Conditional(BEDefines.UnityEditor), System.Diagnostics.Conditional(BEDefines.DevelopmentBuild), System.Diagnostics.Conditional(BEDefines.EnableLog)]
        public static void LogE(this object any, object obj)
        {
            BELogger.LogE($"{any.GetType()}", obj);
        }
    }
}
