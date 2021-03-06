using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMDbLib.Objects.Search;
using System.Net.Sockets;

namespace Roller
{

    public class MovieRoller
    {
        SearchMovie rolled_movie; //подобранный фильм
        int primary_year = 2020;
        List<int> chosenGenresIDs = new List<int>(); //добавление/удаление жанра = select/unselect чекбокса у жанра
        Dictionary<Genres.Title, int> GenresIDs = new Dictionary<Genres.Title, int>(); //список жанров с их ID   
        List<Genres.Title> genres = new List<Genres.Title>();
        TMDbApi.TMDbApi client = new TMDbApi.TMDbApi();
        
        bool have_internet = true;

        public MovieRoller()
        {
            client.Connect();
            ParseGenresIDs();
        }
        public int GenresCount
        {
            get { return chosenGenresIDs.Count; }
        }
        public bool check_internet()
        {
            have_internet = true;
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                have_internet = false;
            }

            return have_internet;
        }

        public SearchMovie return_movie()
        {
            return rolled_movie;
        }
  
        public async Task ParseMovie(int year_l_border, int year_r_border, int amnt_pages, bool all_genres, bool age_flag)
        {
            if (chosenGenresIDs.Count == 0)
            {
                rolled_movie = null;
                return;
            }
            check_internet();
            if (!have_internet)
                return;
            primary_year = new Random().Next(year_l_border, year_r_border);
            await client.api_ParseMovie(chosenGenresIDs, primary_year, amnt_pages, all_genres, age_flag);
            rolled_movie = client.return_movie(chosenGenresIDs);
        }

        public string rolledMovie
        {
            get { return rolled_movie.Title; }
        }

        void ParseGenresIDs()
        {
            foreach (Genres.Title genre in Enum.GetValues(typeof(Genres.Title)))
                GenresIDs[genre] = (int)genre;
        }

        public void AddRemoveGenre(Genres.Title genre, bool remove = false)
        {
            try
            {
                if (!remove) //если не убираем, тогда посмотрим, есть у нас уже этот жанр или нет, если нет, то добавим в список
                {
                    if (!chosenGenresIDs.Contains(GenresIDs[genre]))
                        chosenGenresIDs.Add(GenresIDs[genre]);
                }
                else if (remove && chosenGenresIDs.Contains(GenresIDs[genre])) //если убираем жанр, то проверим, есть у нас он вообще или нет, если есть, то уберем
                    chosenGenresIDs.Remove(GenresIDs[genre]);

            }
            catch (Exception) { }
        }
    }
}
