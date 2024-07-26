namespace KomootTourAnalyzer;
public abstract class BaseLoggingService
{
    protected static string TimePrefix()
    {
        return $"[{DateTime.Now.ToString("HH:mm:ss.fff")}]: ";
    }
}