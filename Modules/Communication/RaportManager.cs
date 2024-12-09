namespace IO.Modules.Communication;

public class RaportManager : IReporting
{
    public string activeReport = "";
    public Server usedServer;

    public string CreateReport()
    {
        string raport = activeReport + "\n" + DateTime.Now.ToShortTimeString();
        return raport;
    }

    public void archiveReport(string todaysReport)
    {
        string raport = CreateReport();
        usedServer.Reports.Add(raport);
        activeReport = "";
    }
    
}