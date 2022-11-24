using Domain;
using WebApiServerLogs.SearchCriterias;

namespace WebApiServerLogs.Repositories
{
    public class LogsRepository
    {
        private List<Log> logs;
        private object padlock;
        private static LogsRepository instance;

        private static object singletonPadlock = new object();
        public static LogsRepository GetInstance()
        {

            lock (singletonPadlock)
            {
                if (instance == null)
                    instance = new LogsRepository();
            }
            return instance;
        }

        private LogsRepository()
        {
            logs = new List<Log>();
            padlock = new object();
        }

        public void AddLog(Log log)
        {
            lock (padlock)
            {
                logs.Add(log);
            }
        }

        public Log[] GetLogs(LogSearchCriteria filters)
        {
            lock (padlock)
            {
                var filteredLogs = logs;

                if (filters.MinDate != null)
                {
                    filteredLogs = filteredLogs.FindAll(x => x.Date >= DateTime.Parse(filters.MinDate));
                }

                if (filters.MaxDate != null)
                {
                    filteredLogs = filteredLogs.FindAll(x => x.Date <= DateTime.Parse(filters.MaxDate));
                }

                if (filters.KeyWord != null)
                {
                    filteredLogs = filteredLogs.FindAll(x => x.Message.ToLower().Contains(filters.KeyWord.ToLower()));
                }

                return filteredLogs.ToArray();
            }
        }
    }
}
