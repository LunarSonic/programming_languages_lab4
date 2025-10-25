using CSConsoleApp.Models;

namespace CSConsoleApp.Logic
{
    public class MovieCreditsService
    {
        private readonly IReadOnlyList<MovieCredit> _movies;

        public MovieCreditsService(IReadOnlyList<MovieCredit> movies)
        {
            _movies = movies;
        }

        // 1) Найти все фильмы, снятые указанным режиссером 
        public IEnumerable<string> FindMoviesByDirector(string directorName)
        {
            return _movies
               .Where(m => m.Crew.Any(c => c.Job == "Director" && c.Name == directorName))
               .Select(m => m.Title);
        }

        // 2) Получить список всех персонажей, которых сыграл актер 
        public IEnumerable<string> FindCharactersByActor(string actorName)
        {
            return _movies
               .SelectMany(m => m.Cast)
               .Where(c => c.Name == actorName)
               .Select(c => c.Character)
               .Distinct();
        }

        // 3) Найти N фильмов с самым большим количеством актеров в составе
        public IEnumerable<(string Title, int CastCount)> GetTopMoviesByCastSize(int count)
        {
            return _movies
               .OrderByDescending(m => m.Cast.Count)
               .Take(count)
               .Select(m => (m.Title, m.Cast.Count));
        }

        // 4) Найти топ-10 самых востребованных актеров (по количеству фильмов)
        public IEnumerable<(string Actor, int MovieCount)> GetTopActorsByMovieCount(int count = 10)
        {
            return _movies
               .SelectMany(movie => movie.Cast)
               .GroupBy(castMember => castMember.Name)
               .Select(group => (Actor: group.Key, MovieCount: group.Count()))
               .OrderByDescending(actor => actor.MovieCount)
               .Take(count);
        }

        // 5) Получить список всех уникальных департаментов (department) съемочной группы
        public IEnumerable<string> GetUniqueCrewDepartments()
        {
            return _movies
                .SelectMany(m => m.Crew)
                .Select(c => c.Department)
                .Where(d => !string.IsNullOrEmpty(d))
                .Distinct()
                .OrderBy(d => d);
        }

        // 6) Найти все фильмы, где человек с указанными именем и фамилией был композитором (Original Music Composer)
        public IEnumerable<string> FindMoviesByComposer(string composerName)
        {
            return _movies
                .Where(m => m.Crew.Any(c => c.Job == "Original Music Composer" && c.Name == composerName))
                .Select(m => m.Title);
        }

        // 7) Создать словарь, где ключ — ID фильма, а значение — имя режиссера
        public IReadOnlyDictionary<int, string> GetMovieDirectorMap()
        {
            return _movies
               .ToDictionary(
                   m => m.MovieId,
                   m => m.Crew.FirstOrDefault(c => c.Job == "Director")?.Name ?? "Неизвестен"
               );
        }

        // 8) Найти фильмы, где в актерском составе есть два указанных актера
        public IEnumerable<string> FindMoviesWithDuo(string actor1, string actor2)
        {
            return _movies
                .Where(m => m.Cast.Any(c => c.Name == actor1) && m.Cast.Any(c => c.Name == actor2))
                .Select(m => m.Title);
        }

        // 9) Посчитать, сколько всего человек работает в департаменте "Camera" по всем фильмам
        public int CountTotalCrewPositionsInDepartment(string departmentName)
        {
            return _movies
                .SelectMany(m => m.Crew)
                .Where(c => c.Department == departmentName)
                .GroupBy(c => c.Id)
                .Count();
        }

        // 10) Найти всех людей, которые в указанном фильме были одновременно и в съемочной группе, и в списке актеров
        public IEnumerable<string> FindDualRolePeopleInMovie(string movieTitle)
        {
            var movie = _movies.FirstOrDefault(m => m.Title == movieTitle);
            if (movie == null)
            {
                return Enumerable.Empty<string>();
            }
            var castIds = movie.Cast.Select(c => c.Id).ToHashSet();

            return movie.Crew
                .Where(c => castIds.Contains(c.Id))
                .Select(c => c.Name)
                .Distinct();
        }

        // 11) Найти "внутренний круг" режиссера (топ N членов съемочной группы, которые работали с ним над наибольшим количеством фильмов)
        public IEnumerable<(string CrewMember, int MovieCount)> FindTopCollaboratorsWithDirector(string directorName, int count)
        {
            var directorMovies = _movies
                .Where(m => m.Crew.Any(c => c.Job == "Director" && c.Name == directorName))
                .ToList();

            return directorMovies
                .SelectMany(m => m.Crew)
                .Where(c => c.Name != directorName)
                .GroupBy(c => c.Name)
                .Select(g => (CrewMember: g.Key, MovieCount: g.Count()))
                .OrderByDescending(x => x.MovieCount)
                .Take(count);
        }

        // 12) Определить экранные "дуэты": Найти 10 пар актеров, которые чаще всего снимались вместе в одних и тех же фильмах
        public IEnumerable<(string Actor1, string Actor2, int Count)> FindTopScreenDuos(int count)
        {
            return _movies
                .SelectMany(movie =>
                    movie.Cast.SelectMany(actor1 =>
                        movie.Cast.Select(actor2 => new { actor1, actor2 })
                    )
                )
                .Where(pair => pair.actor1.Id < pair.actor2.Id)
                .GroupBy(pair => (Id1: pair.actor1.Id, Id2: pair.actor2.Id))
                .Select(g => new
                {
                    Actor1 = g.First().actor1.Name,
                    Actor2 = g.First().actor2.Name,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(count)
                .Select(x => (x.Actor1, x.Actor2, x.Count));
        }

        // 13) Вычислить "индекс разнообразия" для карьеры: Найти N членов съемочной группы, которые поработали в наибольшем количестве различных департаментов
        public IEnumerable<(string Name, int UniqueDepartments)> GetTopCrewDiversityIndex(int count)
        {
            return _movies
                .SelectMany(m => m.Crew)
                .GroupBy(c => c.Name)
                .Select(g => new
                {
                    Name = g.Key,
                    UniqueDepartments = g.Select(c => c.Department).Distinct().Count()
                })
                .OrderByDescending(x => x.UniqueDepartments)
                .Take(count)
                .Select(x => (x.Name, x.UniqueDepartments));
        }

        // 14) Найти "творческие трио": Найти фильмы, где один и тот же человек выполнял роли режиссера (Director), сценариста (Writer) и продюсера (Producer)
        public IEnumerable<string> FindCreativeTrios()
        {
            return _movies
                .Where(m =>
                    m.Crew.GroupBy(c => c.Id)
                    .Any(g =>
                        g.Any(c => c.Job == "Director") &&
                        g.Any(c => c.Job.Contains("Writer")) &&
                        g.Any(c => c.Job.Contains("Producer"))
                    )
                )
                .Select(m => m.Title);
        }

        // 15) Два шага до Кевина Бейкона: Найти всех актеров, которые снимались в одном фильме с актером, который, в свою очередь, снимался в одном фильме с "Kevin Bacon"
        public IEnumerable<string> FindTwoDegreesOfSeparation(string targetActor)
        {
            var firstDegreeActors = _movies
                .Where(m => m.Cast.Any(c => c.Name == targetActor))
                .SelectMany(m => m.Cast)
                .Where(c => c.Name != targetActor)
                .Select(c => c.Name)
                .ToHashSet();

            return _movies
                .Where(m => m.Cast.Any(c => firstDegreeActors.Contains(c.Name)))
                .SelectMany(m => m.Cast)
                .Select(c => c.Name)
                .Where(name => name != targetActor && !firstDegreeActors.Contains(name))
                .Distinct();
        }

        // 16) Проанализировать "командную работу": Сгруппировать фильмы по режиссеру и для каждого из них найти средний размер как актерского состава (Cast), так и съемочной группы (Crew)
        public IEnumerable<(string Director, double AvgCast, double AvgCrew)> AnalyzeTeamwork()
        {
            return _movies
                .SelectMany(m => m.Crew
                                  .Where(c => c.Job == "Director")
                                  .Select(d => new { DirectorName = d.Name, Movie = m }))
                .GroupBy(x => x.DirectorName)
                .Select(g => (
                    Director: g.Key,
                    AvgCast: g.Average(x => x.Movie.Cast.Count),
                    AvgCrew: g.Average(x => x.Movie.Crew.Count)
                ))
                .OrderByDescending(x => x.AvgCast);
        }
        
        // 17) Определить карьерный путь "универсалов": Для каждого человека, который был и актером, и членом съемочной группы (в целом по датасету), определить департамент, в котором он работал чаще всего
        public IEnumerable<(string Name, string MostFrequentDepartment, int Count)> GetDualRoleCareerPath()
        {
            var allCastIds = _movies.SelectMany(m => m.Cast).Select(c => c.Id).ToHashSet();

            return _movies
                .SelectMany(m => m.Crew)
                .Where(c => allCastIds.Contains(c.Id))
                .GroupBy(c => c.Id)
                .Select(g =>
                {
                    var topDepartmentGroup = g.GroupBy(c => c.Department)
                                              .OrderByDescending(dg => dg.Count())
                                              .First();
                    return (
                        Name: g.First().Name,
                        MostFrequentDepartment: topDepartmentGroup.Key,
                        Count: topDepartmentGroup.Count()
                    );
                });
        }

        // 18) Найти пересечение "элитных клубов": Найти людей, которые работали с двумя указанными режиссерами
        public IEnumerable<string> FindCollaboratorsOfTwoDirectors(string director1, string director2)
        {
            var crew1 = _movies
                .Where(m => m.Crew.Any(c => c.Job == "Director" && c.Name == director1))
                .SelectMany(m => m.Crew)
                .Select(c => c.Name)
                .ToHashSet();
            var crew2 = _movies
                .Where(m => m.Crew.Any(c => c.Job == "Director" && c.Name == director2))
                .SelectMany(m => m.Crew)
                .Select(c => c.Name)
                .ToHashSet();

            return crew1.Intersect(crew2)
                        .Where(name => name != director1 && name != director2)
                        .Distinct();
        }

        // 19) Выявить "скрытое влияние": Ранжировать все департаменты по среднему количеству актеров в тех фильмах, над которыми они работали (чтобы проверить, коррелирует ли работа определенного департамента с масштабом актерского состава)
        public IEnumerable<(string Department, double AvgCastSize)> AnalyzeDepartmentInfluence()
        {
            return _movies
                .SelectMany(m => m.Crew.Select(c => new
                {
                    c.Department,
                    CastCount = m.Cast.Count
                }))
                .Where(x => !string.IsNullOrEmpty(x.Department))
                .GroupBy(x => x.Department)
                .Select(g => (
                    Department: g.Key,
                    AvgCastSize: g.Average(x => x.CastCount)
                ))
                .OrderByDescending(x => x.AvgCastSize);
        }

        // 20) Проанализировать "архетипы" персонажей: Для актера сгруппировать его роли по первому слову в имени персонажа и посчитать частоту
        public IEnumerable<(string Archetype, int Count)> AnalyzeCharacterArchetypes(string actorName)
        {
            return _movies
                .SelectMany(m => m.Cast)
                .Where(c => c.Name == actorName)
                .Select(c => c.Character.Split(' ').FirstOrDefault()?.Trim() ?? string.Empty)
                .Where(archetype => !string.IsNullOrEmpty(archetype))
                .GroupBy(archetype => archetype)
                .Select(g => (Archetype: g.Key, Count: g.Count()))
                .OrderByDescending(x => x.Count);
        }
    }
}