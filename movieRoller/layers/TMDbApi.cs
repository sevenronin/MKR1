using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMDbLib.Client;
using TMDbLib.Objects.Discover;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;

namespace TMDbApi
{
    public class TMDbApi
    {
        TMDbClient client; //клиент для работы с api
        string api_key = "ed7d16aeb09ffce311534f878ece4fb3";
        SearchMovie rolled_movie; //подобранный фильм
        SearchContainer<SearchMovie> rolled_movies = new SearchContainer<SearchMovie>();
        
        public void Connect()
        {
            client = new TMDbClient(api_key);
            client.DefaultLanguage = "ru";
        }
        
        async public Task api_ParseMovie(List<int> genresIDs, int primary_year, int amnt_pages, bool age_flag)
        {
            try
            {
                rolled_movies = await client.
                       DiscoverMoviesAsync().
                       IncludeAdultMovies(age_flag).
                       IncludeWithAnyOfGenre(genresIDs).
                       OrderBy(DiscoverMovieSortBy.PopularityDesc).
                       WherePrimaryReleaseIsInYear(primary_year).
                       Query(new Random().Next(1, amnt_pages));
            }
            catch (Exception) { }
        }

        public SearchMovie return_movie(List<int> genresIDs)
        {
            if (genresIDs.Count == 0)
                return null;

            rolled_movie = rolled_movies.Results[new Random().Next(0, rolled_movies.Results.Count() - 1)];
            return rolled_movie;
        }
    }
}
