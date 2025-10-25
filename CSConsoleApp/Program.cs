using System; 
using System.Collections.Immutable;
using CSConsoleApp.Models;
using CSConsoleApp.Logic;

namespace CSConsoleApp
{
    public static class Program
    {
        public static void Main()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var filePath = Directory.GetFiles(currentDirectory, "*.csv").FirstOrDefault();

            IReadOnlyList<MovieCredit> movieCredits = ImmutableList<MovieCredit>.Empty;
            try
            {
                var parser = new MovieCreditsParser(filePath);
                movieCredits = parser.Parse();
            }
            catch (Exception exc)
            {
                Console.WriteLine("Не удалось распарсить csv: " + exc.Message);
                Environment.Exit(1);
            }

            var creditsService = new MovieCreditsService(movieCredits);
            var resultsDirectory = Path.Combine(currentDirectory, "Results");
            var reporter = new MovieAnalysisReporter(creditsService, resultsDirectory);
            reporter.RunAllTasks();
        }
    }
}