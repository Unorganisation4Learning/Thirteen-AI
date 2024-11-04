public interface ILogger
{
    public void LogI(object obj);

    public void LogW(object obj);

    public void LogE(object obj);

    public void LogIFormat(string format, params object[] args);

    public void LogWFormat(string format, params object[] args);

    public void LogEFormat(string format, params object[] args);
}

public interface IIdentify
{
    public ulong GetId();
    public bool SetId(ulong id);
}

public interface IBEAgent { }

public interface INodeInformation { }