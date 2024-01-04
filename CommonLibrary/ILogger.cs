namespace CommonLibrary
{
    public interface ILogger
    {
        public void Log(object log);
        public void LogError(object log);
        public void LogError(Exception exception);
    }
}
