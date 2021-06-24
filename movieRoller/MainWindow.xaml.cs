using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using TMDbLib.Objects.Search;
using System.Windows.Media.Animation;
namespace movieRoller
{
    public partial class MainWindow : Window
    {
        SearchMovie rolled_movie;
        Roller.MovieRoller roller; // сам подборщик фильмов
        Genres.Title chosenOtherGenre; // выбранный "другой" жанр
        List<int>GenresIDs = new List<int>();
        List<string> history = new List<string>();  //список для истории найденных фильмов

        int year_l_border, year_r_border;
        int checking_pages = 3;

        public MainWindow()
        {
            InitializeComponent();
            roller = new Roller.MovieRoller();
            fill_genres_list();

            years_slider.LowerValue = years_slider.Minimum;
            years_slider.HigherValue = years_slider.Maximum;
        }

        private void fill_genres_list()
        {
            foreach (Genres.RuTitle genre in Enum.GetValues(typeof(Genres.RuTitle)))
                genres_list.Items.Add(genre);
        }


        async public void roll_click()
        {
            year_l_border = int.Parse(year_lower_b.Content.ToString());
            year_r_border = int.Parse(year_higher_b.Content.ToString());

            loading_background.Visibility = Visibility.Visible;
            loading_animation.Visibility = Visibility.Visible;
            try
            {
                if(cb_age.IsChecked == true) await roller.ParseMovie(year_l_border, year_r_border, checking_pages, false); //вернет фильм с ограничением до 18, информацию о котром я выведу на экран
                else await roller.ParseMovie(year_l_border, year_r_border, checking_pages, true); //вернет фильм без ограничения по возрасту, информацию о котром я выведу на экран
                if (history.Count > 9) history.RemoveAt(0);    //храним только последние 10 найденных фильмов
                history.Add(Convert.ToString(roller.return_movie().Title));   //запоминаем найденный фильм
            }
            catch (Exception) { };

            if (!roller.check_internet())
            {
                MessageBox.Show("Нет доступа к интернету!", "Ошибка");
                loading_background.Visibility = Visibility.Hidden;
                loading_animation.Visibility = Visibility.Hidden;
                return;
            }
            else
            {
                rolled_movie = roller.return_movie();

                loading_background.Visibility = Visibility.Hidden;
                loading_animation.Visibility = Visibility.Hidden;

                if (rolled_movie == null)
                {
                    MessageBox.Show("Выберите хотя бы один жанр!");
                }
                else
                {
                    //string overview = "";
                    txt_rolled_movie.Text = rolled_movie.Title;
                    btn_movie_info.IsEnabled = true;
                    btn_history.IsEnabled = true;
                    btn_search.IsEnabled = true;
                    //overview = rolled_movie.Overview;
                    /*int words_in_str = 0;
                    if (overview.Length < 10)
                        overview = "Описание отсутствует";
                    else
                        for (int i = 0; i < overview.Length; ++i)
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
                    main_Window.txt_rolled_movie.ToolTip = overview;*/

                    if (txt_rolled_movie.Text.Length > 1)
                    {
                        btn_roll.Visibility = Visibility.Hidden; //если нажали кнопку рола фильма и рол был успешен, скроем кнопку
                        rolled_movie_canvas.Visibility = Visibility.Visible;
                    }
                }
            }

        }

        private void btn_roll_Click(object sender, RoutedEventArgs e)
        {
            roll_click();
        }


        private void cb_comedy_Checked(object sender, RoutedEventArgs e)
        {
            roller.AddRemoveGenre(Genres.Title.Comedy);
        }

        private void cb_drama_Checked(object sender, RoutedEventArgs e)
        {
            roller.AddRemoveGenre(Genres.Title.Drama);
        }

        private void cb_drama_Unchecked(object sender, RoutedEventArgs e)
        {
            roller.AddRemoveGenre(Genres.Title.Drama, true);
        }

        private void cb_comedy_Unchecked(object sender, RoutedEventArgs e)
        {
            roller.AddRemoveGenre(Genres.Title.Comedy, true);
        }

        private void cb_action_Checked(object sender, RoutedEventArgs e)
        {
            roller.AddRemoveGenre(Genres.Title.Action);
        }

        private void cb_action_Unchecked(object sender, RoutedEventArgs e)
        {
            roller.AddRemoveGenre(Genres.Title.Action, true);
        }

        private void cb_horror_Unchecked(object sender, RoutedEventArgs e)
        {
            roller.AddRemoveGenre(Genres.Title.Horror, true);
        }

        private void cb_horror_Checked(object sender, RoutedEventArgs e)
        {
            roller.AddRemoveGenre(Genres.Title.Horror);
        }

        private string fromRuToEnGenre(string ru_genre)
        {
            //нужно найти название жанра на английском по айдишнику жанра на русском
            string en_genre = "";
            int id=0;

            if (ru_genre.Length > 2)
            {
                var ru_genres = Enum.GetValues(typeof(Genres.RuTitle));
                for(int i=0; i<ru_genres.Length; ++i)
                {
                    string a = ru_genres.GetValue(i).ToString();
                    if (ru_genres.GetValue(i).ToString() == ru_genre)
                    {
                        id = i;
                        break;
                    }
                }
                
            }
            en_genre = Enum.GetNames(typeof(Genres.Title))[id].ToString(); //получаем жанр на английском языке
            return en_genre;
        }

        private void cb_other_Checked(object sender, RoutedEventArgs e)
        {
            genres_list.IsEnabled = true;
            chosenOtherGenre = (Genres.Title)Enum.Parse(typeof(Genres.Title), fromRuToEnGenre(genres_list.Text));
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
                    chosenOtherGenre = (Genres.Title)Enum.Parse(typeof(Genres.Title), fromRuToEnGenre(e.AddedItems[0].ToString()));
                    roller.AddRemoveGenre(chosenOtherGenre);
                }
                catch (Exception) { }
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

        private void info_close_Click(object sender, RoutedEventArgs e)
        {
            canvas_info.Visibility = Visibility.Hidden;
            loading_background.Visibility = Visibility.Hidden;
        }

        private void btn_movie_info_Click(object sender, RoutedEventArgs e)
        {
            info_genres.Clear();
            canvas_info.Visibility = Visibility.Visible;
            loading_background.Visibility = Visibility.Visible;
            info_title.Content = rolled_movie.Title;
            info_amnt_votes.Content = rolled_movie.VoteCount;
            info_rating.Content = rolled_movie.VoteAverage;
            for(int genre = 0; genre < rolled_movie.GenreIds.Count; ++genre)
            {
                info_genres.Text += Enum.GetName(typeof(Genres.RuTitle), rolled_movie.GenreIds[genre]);
                if (genre != rolled_movie.GenreIds.Count - 1) info_genres.Text += ", ";
            }
        }

        private void btn_history_Click(object sender, RoutedEventArgs e)
        {
            canvas_history.Visibility = Visibility.Visible;
            loading_background.Visibility = Visibility.Visible;
            tb_history.Clear();
            int i = 1;
            foreach(string film in history)     //построчный вывод подобранных фильмов
            {
                if (i != history.Count) tb_history.Text += Convert.ToString(i) + ". " + Convert.ToString(film) + Environment.NewLine;
                else tb_history.Text += Convert.ToString(i) + ". " + Convert.ToString(film);
                i++;
            }
        }

        private void btn_close_history_Click(object sender, RoutedEventArgs e)
        {
            canvas_history.Visibility = Visibility.Hidden;
            loading_background.Visibility = Visibility.Hidden;
        }

        private void btn_search_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://yandex.ru/search/?lr=213&text=фильм " + Uri.EscapeUriString(roller.rolledMovie));
        }
    }
}
