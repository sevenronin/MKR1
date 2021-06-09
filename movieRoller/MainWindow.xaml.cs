﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TMDbLib;
using TMDbLib.Client;
using TMDbLib.Objects.Discover;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Genres;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using TMDbLib.Utilities.Converters;

namespace movieRoller
{
    public enum GenreName
    {
        Action = 28,
        Adventure = 12,
        Animation = 16,
        Comedy = 35,
        Crime = 80,
        Documentary = 99,
        Drama = 18,
        Family = 10751,
        Fantasy = 14,
        History = 36,
        Horror = 27,
        Music = 10402,
        Mystery = 9648,
        Romance = 10749,
        ScienceFiction = 878,
        TVMovie = 10770,
        Thriller = 53,
        War = 10752,
        Western = 37
    }

    public enum ReleaseYear
    {
        Old,
        New,
        User
    }


    public class MovieRoller
    {
        MainWindow main_Window;
        TMDbClient client; //клиент для работы с api
        string api_key = "ed7d16aeb09ffce311534f878ece4fb3";
        SearchMovie rolled_movie; //подобранный фильм
        int primary_year = 2020;
        List<int> chosenGanresIDs = new List<int>(); //добавление/удаление жанра = select/unselect чекбокса у жанра
        Dictionary<GenreName, int> GenresIDs = new Dictionary<GenreName, int>(); //список жанров с их ID   

        List<GenreName> genres = new List<GenreName>();

        public MovieRoller(MainWindow mainW)
        {
            main_Window = mainW;
            client = new TMDbClient(api_key);
            client.DefaultLanguage = "ru";

            ParseGanresIDs();
        }

        async public void ParseMovie(ReleaseYear release = ReleaseYear.Old, int year = 9999)
        {
            int amnt_checking_pages = 5; //кол-во обратываеых страниц с фильмами

            if (chosenGanresIDs.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы один жанр!");
                return;
            }
            else
            {
                //await client.GetConfigAsync(); //до этого без этой функции ничего не работало, а щас работает, почему - хз
                //chosenGanresIDs.Add(28); //для теста добавим жанр - "Action"
                if (year != 9999)
                {
                    primary_year = year;
                }
                else if (release == ReleaseYear.Old)
                {
                    primary_year = new Random().Next(1950, 2000);
                }
                else primary_year = new Random().Next(2001, 2021);
                SearchContainer<SearchMovie> rolled_movies = null;
                bool rolled = false;
                while (!rolled)
                {
                    try
                    {
                        rolled_movies = await client.
                               DiscoverMoviesAsync().
                               IncludeWithAnyOfGenre(chosenGanresIDs).
                               OrderBy(DiscoverMovieSortBy.PopularityDesc).
                               WherePrimaryReleaseIsInYear(primary_year).
                               Query(new Random().Next(1, amnt_checking_pages));
                        rolled = true;
                    }
                    catch (Exception) { }
                }

                if (rolled)
                {
                    rolled_movie = rolled_movies.Results[new Random().Next(0, rolled_movies.Results.Count() - 1)];
                    main_Window.txt_rolled_movie.Text = rolled_movie.Title;

                    if (main_Window.txt_rolled_movie.Text.Length > 1)
                    {
                        main_Window.btn_roll.Visibility = Visibility.Hidden; //если нажали кнопку рола фильма и рол был успешен, скроем кнопку
                        main_Window.rolled_movie_canvas.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        void ParseGanresIDs()
        {
            foreach (GenreName genre in Enum.GetValues(typeof(GenreName)))
                GenresIDs[genre] = (int)genre;

            /*var genres_list = await client.GetMovieGenresAsync();
            for (GenreName genre = 0; (int)genre < genres_list.Count; ++genre)
                GenresIDs[genre] = genres_list[(int)genre].Id;*/
        }

        public void AddRemoveGenre(GenreName genre, bool remove = false)
        {
            try
            {
                if (!remove) //если не убираем, тогда посмотрим, есть у нас уже этот жанр или нет, если нет, то добавим в список
                {
                    if (!chosenGanresIDs.Contains(GenresIDs[genre]))
                        chosenGanresIDs.Add(GenresIDs[genre]);
                }
                else if (remove && chosenGanresIDs.Contains(GenresIDs[genre])) //если убираем жанр, то проверим, есть у нас он вообще или нет, если есть, то уберем
                    chosenGanresIDs.Remove(GenresIDs[genre]);

            }
            catch (Exception) { }
        }
    }
    public partial class MainWindow : Window
    {
        MovieRoller roller; // сам подборщик фильмов
        GenreName chosenOtherGenre; // выбранный "другой" жанр
        ReleaseYear release = ReleaseYear.Old; // по дефолту старое кино
        int release_year = 0000; // год релиза if release == Release.User
        public MainWindow()
        {
            InitializeComponent();
            roller = new MovieRoller(this);

            foreach (GenreName genre in Enum.GetValues(typeof(GenreName)))
                genres_list.Items.Add(genre);

            /*for (GenreName genre = 0; (int)genre < 18; ++genre)
                genres_list.Items.Add(genre);*/

            for (int i = 2021; i > 1949; --i)
                years_list.Items.Add(i);
        }

        private void btn_roll_Click(object sender, RoutedEventArgs e)
        {
            if (release == ReleaseYear.Old || release == ReleaseYear.New)
                roller.ParseMovie(release);
            else roller.ParseMovie(0, release_year);

        }

        private void cb_comedy_Checked(object sender, RoutedEventArgs e)
        {
            roller.AddRemoveGenre(GenreName.Comedy);
        }

        private void cb_drama_Checked(object sender, RoutedEventArgs e)
        {
            roller.AddRemoveGenre(GenreName.Drama);
        }

        private void cb_drama_Unchecked(object sender, RoutedEventArgs e)
        {
            roller.AddRemoveGenre(GenreName.Drama, true);
        }

        private void cb_comedy_Unchecked(object sender, RoutedEventArgs e)
        {
            roller.AddRemoveGenre(GenreName.Comedy, true);
        }

        private void cb_action_Checked(object sender, RoutedEventArgs e)
        {
            roller.AddRemoveGenre(GenreName.Action);
        }

        private void cb_action_Unchecked(object sender, RoutedEventArgs e)
        {
            roller.AddRemoveGenre(GenreName.Action, true);
        }

        private void cb_horror_Unchecked(object sender, RoutedEventArgs e)
        {
            roller.AddRemoveGenre(GenreName.Horror, true);
        }

        private void cb_horror_Checked(object sender, RoutedEventArgs e)
        {
            roller.AddRemoveGenre(GenreName.Horror);
        }

        private void cb_other_Checked(object sender, RoutedEventArgs e)
        {
            chosenOtherGenre = (GenreName)Enum.Parse(typeof(GenreName), genres_list.Text);
            roller.AddRemoveGenre(chosenOtherGenre);
        }

        private void cb_other_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                roller.AddRemoveGenre(chosenOtherGenre, true);
            }
            catch (Exception) { }
        }

        private void genres_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //будем измегнять спиоск жанров на подбор только если "другое" активировано
                if (cb_other.IsChecked == true)
                {
                    roller.AddRemoveGenre(chosenOtherGenre, true);
                    chosenOtherGenre = (GenreName)Enum.Parse(typeof(GenreName), genres_list.Text);
                    roller.AddRemoveGenre(chosenOtherGenre);
                }
            }
            catch (Exception) { }
        }

        private void rb_old_movies_Checked(object sender, RoutedEventArgs e)
        {
            release = ReleaseYear.Old;
        }

        private void rb_new_movies_Checked(object sender, RoutedEventArgs e)
        {
            release = ReleaseYear.New;
        }

        private void rb_user_movies_Checked(object sender, RoutedEventArgs e)
        {
            release = ReleaseYear.User;
            release_year = int.Parse(years_list.SelectedItem.ToString());
        }

        private void years_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            release_year = int.Parse(years_list.SelectedItem.ToString());
        }

        private void btn_reroll_Click(object sender, RoutedEventArgs e)
        {
            if (release == ReleaseYear.Old || release == ReleaseYear.New)
                roller.ParseMovie(release);
            else roller.ParseMovie(0, release_year);
        }
    }
}