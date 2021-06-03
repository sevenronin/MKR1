using System;
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
using TMDbLib.Objects.Genres;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using TMDbLib.Utilities.Converters;

namespace MovieFinder
{
    public enum GenreName
    {
        Action,
        Adventure,
        Animation,
        Comedy,
        Crime,
        Documentary,
        Drama,
        Family,
        Fantasy,
        History,
        Horror,
        Music,
        Mystery,
        Romance,
        ScienceFiction,
        TVMovie,
        Thriller,
        War,
        Western
    }


    public class MovieRoller
    {
        TMDbClient client; //клиент для работы с api
        string api_key = "ed7d16aeb09ffce311534f878ece4fb3";
        Movie rolled_movie; //подобранный фильм
        int primary_year = 2020;
        List<int> chosenGanresIDs = new List<int>(); //добавление/удаление жанра = select/unselect чекбокса у жанра
        Dictionary<GenreName , int> GenresIDs = new Dictionary<GenreName, int>(); //список жанров с их ID   

        public MovieRoller()
        {
            Init(); 
            ParseGanresIDs();
        }

        async void Init()
        {
            client = new TMDbClient(api_key); 
            client.DefaultLanguage = "ru"; 
        }

        async public void ParseMovie()
        {
            await client.GetConfigAsync();
            chosenGanresIDs.Add(28); //для теста добавим жанр - "Action"
            if (chosenGanresIDs.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы один жанр!");
                return;
            }
            else
            {
                var rolled_movies = await client.
                    DiscoverMoviesAsync().
                    IncludeWithAnyOfGenre(chosenGanresIDs).
                    OrderBy(DiscoverMovieSortBy.PopularityDesc).
                    WherePrimaryReleaseIsInYear(primary_year).
                    Query();
            }
        }

        async void ParseGanresIDs()
        {
            var genres_list = await client.GetMovieGenresAsync();
            for (GenreName genre = 0; (int)genre < genres_list.Count; ++genre)
                GenresIDs[genre] = genres_list[(int)genre].Id;
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MovieRoller roller = new MovieRoller();
            roller.ParseMovie();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            for (GenreName genre = 0; (int)genre < 18; ++genre)
                genres_list.Items.Add(genre);
            for (int i = 1950; i < 2022; ++i)
                years_list.Items.Add(i);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
