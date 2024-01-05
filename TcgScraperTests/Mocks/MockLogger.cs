using CommonLibrary;

namespace TcgScraperTests.Mocks
{
    internal class MockLogger : IApiLogger
    {
        private LinkedList<string> Logs { get; set; } = new();
        private LinkedList<string> ErrorLogs { get; set; } = new();

        private Task<string>? TaskAwaitingLog { get; set; }
        private Task<string>? TaskAwaitingErrorLog { get; set; }

        public void Log(object message)
        {
            Console.WriteLine(message);
            Logs.AddLast(message.ToString());
        }

        public void LogError(object message)
        {
            Console.WriteLine(message);
            ErrorLogs.AddLast(message.ToString());
            TaskAwaitingLog?.Start();
        }

        public void LogError(Exception exception)
        {
            Console.WriteLine(exception.Message);
            ErrorLogs.AddLast(exception.Message);
            TaskAwaitingErrorLog?.Start();
        }

        public async Task<string> AwaitNextLog()
        {
            TaskAwaitingLog = new Task<string>(ConsumeLog);
            if (Logs.First is not null)
            {
                TaskAwaitingLog.Start();
            }
            return await TaskAwaitingLog;
        }

        public async Task<string> AwaitNextErrorLog()
        {
            TaskAwaitingErrorLog = new Task<string>(ConsumeErrorLog);
            if (ErrorLogs.First is not null)
            {
                TaskAwaitingErrorLog.Start();
            }
            return await TaskAwaitingErrorLog;
        }

        private string ConsumeLog()
        {
            var log = Logs.First?.Value;
            Logs.RemoveFirst();
            return log;
        }

        private string ConsumeErrorLog()
        {
            var log = ErrorLogs.First?.Value;
            ErrorLogs.RemoveFirst();
            return log;
        }
    }
}
