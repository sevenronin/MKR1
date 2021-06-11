using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TMDbLib.Client;
using TMDbLib.Objects.Discover;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;

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


    public class MovieRoller
    {
        MainWindow main_Window; //aaa
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

        async public void ParseMovie()
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
                primary_year = new Random().Next((int)main_Window.years_slider.LowerValue, (int)main_Window.years_slider.HigherValue);
                SearchContainer<SearchMovie> rolled_movies = null;
                bool rolled = false;
                int amnt_tries = 0;
                main_Window.btn_roll.IsEnabled = false;
                main_Window.loading_background.Visibility = Visibility.Visible;
                main_Window.loading_animation.Visibility = Visibility.Visible;
                while (!rolled && amnt_tries < 31)
                {
                    amnt_tries++;
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
                if (amnt_tries == 30 && !rolled)
                    MessageBox.Show("Произошла ошибка!\nПопробуйте нажать на кнопку снова.", "Ошибка");

                if (rolled)
                {
                    main_Window.loading_background.Visibility = Visibility.Hidden;
                    main_Window.loading_animation.Visibility = Visibility.Hidden;
                    string overview = "";
                    rolled_movie = rolled_movies.Results[new Random().Next(0, rolled_movies.Results.Count() - 1)];
                    main_Window.txt_rolled_movie.Text = rolled_movie.Title;
                    overview = rolled_movie.Overview;
                    int words_in_str = 0;
                    if (overview.Length < 10)
                        overview = "Описание отсутствует";
                    else
                        for(int i=0; i<overview.Length; ++i)
                        {
                            if (overview[i] == ' ')
                            {
                                if (words_in_str == 10)
                                {
                                    overview = overview.Insert(i, "\n");
                                    words_in_str = 0;
                                }
                                else words_in_str++;
                            }
                        
                        }
                    main_Window.txt_rolled_movie.ToolTip = overview;

                    if (main_Window.txt_rolled_movie.Text.Length > 1)
                    {
                        main_Window.btn_roll.Visibility = Visibility.Hidden; //если нажали кнопку рола фильма и рол был успешен, скроем кнопку
                        main_Window.rolled_movie_canvas.Visibility = Visibility.Visible;
                    }
                }
            }
        }
        public string rolledMovie
        {
            get { return rolled_movie.Title; }
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
        public MainWindow()
        {
            InitializeComponent();
            roller = new MovieRoller(this);
            years_slider.LowerValue = years_slider.Minimum;
            years_slider.HigherValue = years_slider.Maximum;
            foreach (GenreName genre in Enum.GetValues(typeof(GenreName)))
                genres_list.Items.Add(genre);

            /*for (GenreName genre = 0; (int)genre < 18; ++genre)
                genres_list.Items.Add(genre);*/

            // years_slider.SetBinding(Xceed.Wpf.Toolkit.RangeSlider.MaximumProperty, "year_lower_b");

        }

        private void btn_roll_Click(object sender, RoutedEventArgs e)
        {
            roller.ParseMovie();
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
            genres_list.IsEnabled = true;
            chosenOtherGenre = (GenreName)Enum.Parse(typeof(GenreName), genres_list.Text);
            roller.AddRemoveGenre(chosenOtherGenre);
        }

        private void cb_other_Unchecked(object sender, RoutedEventArgs e)
        {
            genres_list.IsEnabled = false;
            try
            {
                roller.AddRemoveGenre(chosenOtherGenre, true);
            }
            catch (Exception) { }
        }

        private void genres_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cb_other.IsChecked == true)
                try
                {
                    roller.AddRemoveGenre(chosenOtherGenre, true);
                    chosenOtherGenre = (GenreName)Enum.Parse(typeof(GenreName), e.AddedItems[0].ToString());
                    roller.AddRemoveGenre(chosenOtherGenre);
                }
                catch (Exception) { }
        }


        private void btn_reroll_Click(object sender, RoutedEventArgs e)
        {
            roller.ParseMovie();
        }

        private void years_slider_LowerValueChanged(object sender, RoutedEventArgs e)
        {
            year_lower_b.Content = ((int)years_slider.LowerValue).ToString();
        }

        private void years_slider_HigherValueChanged(object sender, RoutedEventArgs e)
        {
            year_higher_b.Content = ((int)years_slider.HigherValue).ToString();
        }

        private void reroll_image_MouseEnter(object sender, MouseEventArgs e)
        {
            btn_reroll.Focus();
        }

        private void reroll_image_MouseLeave(object sender, MouseEventArgs e)
        {
            btn_reroll.Focus();
        }

        private void btn_search_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://yandex.ru/search/?lr=213&text=" + Uri.EscapeUriString(roller.rolledMovie));
        }
    }
}
