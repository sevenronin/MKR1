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
using IMDbApiLib;
using JMovies.IMDb;
///////23423234
efwrewyhiouehif ew qtyretouoeoriu
namespace MovieFinder
{
    public class IMDb
    {
        async public void IMDb_call()
        {
            var apiLib = new ApiLib("k_wz4b3ciw");
            var data = await apiLib.KeywordAsync("music-video");

        }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            IMDb a = new IMDb();
            a.IMDb_call();
            //hello
        }
    }
}
