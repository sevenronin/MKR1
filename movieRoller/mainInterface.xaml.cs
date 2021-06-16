using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TMDbLib.Objects.Search;

namespace movieRoller
{

   
    public partial class MainWindow : Window
    {
        SearchMovie rolled_movie;
        Roller.MovieRoller roller; // сам подборщик фильмов
        Genres.Title chosenOtherGenre; // выбранный "другой" жанр
        List<int>GenresIDs = new List<int>();

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
            foreach (Genres.Title genre in Enum.GetValues(typeof(Genres.Title)))
                genres_list.Items.Add(genre);
        }


        async public void roll_click()
        {
            year_l_border = int.Parse(year_lower_b.Content.ToString());
            year_r_border = int.Parse(year_higher_b.Content.ToString());

            loading_background.Visibility = Visibility.Visible;
            loading_animation.Visibility = Visibility.Visible;

            await roller.ParseMovie(year_l_border, year_r_border, checking_pages); //вернет фильм, информацию о котром я выведу на экран
            rolled_movie = roller.return_movie();
            
            if (rolled_movie == null)
            {
                loading_background.Visibility = Visibility.Hidden;
                loading_animation.Visibility = Visibility.Hidden;
                

                MessageBox.Show("Выберите хотя бы один жанр!");
            }
            else
            {
                loading_background.Visibility = Visibility.Hidden;
                loading_animation.Visibility = Visibility.Hidden;
                //string overview = "";
                txt_rolled_movie.Text = rolled_movie.Title;
                
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

        private void cb_other_Checked(object sender, RoutedEventArgs e)
        {
            genres_list.IsEnabled = true;
            chosenOtherGenre = (Genres.Title)Enum.Parse(typeof(Genres.Title), genres_list.Text);
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
                    chosenOtherGenre = (Genres.Title)Enum.Parse(typeof(Genres.Title), e.AddedItems[0].ToString());
                    roller.AddRemoveGenre(chosenOtherGenre);
                }
                catch (Exception) { }
        }


        private void btn_reroll_Click(object sender, RoutedEventArgs e)
        {
            roll_click();
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
