namespace Game.Core.Logger
{
    public class UnityLogger : ILogger
    {        
        public void LogI(object obj)
        {
            UnityEngine.Debug.Log(obj);
        }

        public void LogW(object obj)
        {
            UnityEngine.Debug.LogWarning(obj);
        }

        public void LogE(object obj)
        {
            UnityEngine.Debug.LogError(obj);
        }

        public void LogIFormat(string format, params object[] args)
        {
            UnityEngine.Debug.LogFormat(format, args);
        }

        public void LogWFormat(string format, params object[] args)
        {
            UnityEngine.Debug.LogWarningFormat(format, args);
        }

        public void LogEFormat(string format, params object[] args)
        {
            UnityEngine.Debug.LogErrorFormat(format, args);
        }
    }
}
