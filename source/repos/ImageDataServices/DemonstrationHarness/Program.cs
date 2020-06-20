using System;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace DemonstrationHarness
{
    class Program
    {
		static void Main()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Literate)
                .CreateLogger();
            Log.Information("Starting host.");
			try
			{
                var harness = new Harness {RunLocal = true};
				harness.RunQ4LineOutputService();
                harness.RunFoundDefectService();
				harness.RunTaggedDefectService();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
				WriteInnerException(e);
				Console.WriteLine("Press any key to continue");
				Console.ReadKey();
			}        
		}

		static void WriteInnerException(Exception e)
		{
			if (e.InnerException != null)
			{
				Console.WriteLine(e.InnerException.Message);
				Console.WriteLine(e.InnerException.StackTrace);
				WriteInnerException(e.InnerException);// yes, this is supposed to be recursive.
			} 
		}
    }
}
