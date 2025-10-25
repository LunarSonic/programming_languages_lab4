namespace CSConsoleApp.Logic
{
    public class MovieAnalysisReporter
    {
        private readonly MovieCreditsService _service;
        private readonly string _resultsDirectory;

        public MovieAnalysisReporter(MovieCreditsService service, string resultsDirectory)
        {
            _service = service;
            _resultsDirectory = resultsDirectory;

            if (!Directory.Exists(_resultsDirectory))
            {
                Directory.CreateDirectory(_resultsDirectory);
            }
        }

        public void RunAllTasks()
        {
            var res1 = _service.FindMoviesByDirector("Steven Spielberg");
            WriteLinesToFile("Найти все фильмы, снятые режиссером 'Steven Spielberg':", "task-1.txt", res1);

            var res2 = _service.FindCharactersByActor("Tom Hanks");
            WriteLinesToFile("Получить список всех персонажей, которых сыграл актер 'Tom Hanks':", "task-2.txt", res2);

            var res3 = _service.GetTopMoviesByCastSize(5).Select(r => $"{r.Title} (Кол-во актеров: {r.CastCount})");
            WriteLinesToFile("Найти 5 фильмов с самым большим количеством актеров в составе:", "task-3.txt", res3);

            var res4 = _service.GetTopActorsByMovieCount(10).Select(r => $"{r.Actor} - {r.MovieCount} фильмов");
            WriteLinesToFile("Найти топ-10 самых востребованных актеров (по количеству фильмов):", "task-4.txt", res4);

            var res5 = _service.GetUniqueCrewDepartments();
            WriteLinesToFile("Получить список всех уникальных департаментов (department) съемочной группы:", "task-5.txt", res5);

            var res6 = _service.FindMoviesByComposer("Hans Zimmer");
            WriteLinesToFile("Найти все фильмы, где 'Hans Zimmer' был композитором (Original Music Composer):", "task-6.txt", res6);

            var res7 = _service.GetMovieDirectorMap().Select(kv => $"{kv.Key} -> {kv.Value}");
            WriteLinesToFile("Создать словарь, где ключ - ID фильма, а значение - имя режиссера:", "task-7.txt", res7);

            var res8 = _service.FindMoviesWithDuo("Brad Pitt", "George Clooney");
            WriteLinesToFile("Найти фильмы, где в актерском составе есть и 'Brad Pitt', и 'George Clooney':", "task-8.txt", res8);

            var res9 = _service.CountTotalCrewPositionsInDepartment("Camera").ToString();
            File.WriteAllText(Path.Combine(_resultsDirectory, "task-9.txt"), "Посчитать, сколько всего человек работает в департаменте 'Camera' по всем фильмам:\n" + res9, System.Text.Encoding.UTF8);

            var res10 = _service.FindDualRolePeopleInMovie("Titanic");
            WriteLinesToFile("Найти всех людей, которые в фильме 'Titanic' были одновременно и в съемочной группе, и в списке актеров:", "task-10.txt", res10);

            var res11 = _service.FindTopCollaboratorsWithDirector("Quentin Tarantino", 5).Select(r => $"{r.CrewMember} - {r.MovieCount} фильмов");
            WriteLinesToFile("Найти 'внутренний круг' режиссера: Для режиссера 'Quentin Tarantino' найти топ-5 членов съемочной группы:", "task-11.txt", res11);

            var res12 = _service.FindTopScreenDuos(10).Select(r => $"{r.Actor1} и {r.Actor2} - {r.Count} фильмов");
            WriteLinesToFile("Определить экранные 'дуэты': Найти 10 пар актеров, которые чаще всего снимались вместе:", "task-12.txt", res12);

            var res13 = _service.GetTopCrewDiversityIndex(5).Select(r => $"{r.Name} - {r.UniqueDepartments} департаментов");
            WriteLinesToFile("Вычислить 'индекс разнообразия' для карьеры: Найти 5 членов съемочной группы, которые поработали в наибольшем количестве различных департаментов:", "task-13.txt", res13);

            var res14 = _service.FindCreativeTrios();
            WriteLinesToFile("Фильмы с 'творческим трио' (Director, Writer, Producer):", "task-14.txt", res14);

            var res15 = _service.FindTwoDegreesOfSeparation("Kevin Bacon");
            WriteLinesToFile("Два шага до Кевина Бейкона:", "task-15.txt", res15);

            var res16 = _service.AnalyzeTeamwork().Select(r => $"{r.Director}: Cast = {r.AvgCast:F2}, Crew = {r.AvgCrew:F2}");
            WriteLinesToFile("Проанализировать 'командную работу': средний размер Cast и Crew по режиссеру:", "task-16.txt", res16);

            var res17 = _service.GetDualRoleCareerPath().Select(r => $"{r.Name}: {r.MostFrequentDepartment} ({r.Count} раз)"); 
            WriteLinesToFile("Определить карьерный путь 'универсалов': департамент, в котором чаще всего работал человек:", "task-17.txt", res17);
            
            var res18 = _service.FindCollaboratorsOfTwoDirectors("Martin Scorsese", "Christopher Nolan");
            WriteLinesToFile("Найти пересечение 'элитных клубов': люди, которые работали и с Мартином Скорсезе, и с Кристофером Ноланом:", "task-18.txt", res18);

            var res19 = _service.AnalyzeDepartmentInfluence().Select(r => $"{r.Department}: Avg Cast Size = {r.AvgCastSize:F2}");
            WriteLinesToFile("Выявить 'скрытое влияние': Ранжировать все департаменты по среднему количеству актеров:", "task-19.txt", res19);

            var res20 = _service.AnalyzeCharacterArchetypes("Johnny Depp").Select(r => $"{r.Archetype} - {r.Count} раз");
            WriteLinesToFile("Проанализировать 'архетипы' персонажей Джонни Деппа:", "task-20.txt", res20);
        }

        private void WriteLinesToFile(string header, string fileName, IEnumerable<string> lines)
        {
            var filePath = Path.Combine(_resultsDirectory, fileName);
            try
            {
                var content = new List<string> { header, new string('-', header.Length) };
                content.AddRange(lines); 
                File.WriteAllLines(filePath, content, System.Text.Encoding.UTF8);
            } 
            catch (Exception exc)
            {
                Console.WriteLine($"Ошибка при записи в файл {filePath}: " + exc.Message);
            }
        }
    }
}