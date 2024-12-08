namespace IO.Modules.Communication;

public interface IReporting
{
    public string CreateReport();


    public void archiveReport(string todaysReport);

}