using Serilog;

public static class SerilogConfig
{
    public static void Configure()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()

            .WriteTo.Seq("http://localhost:5341")

            .CreateLogger();
    }
}