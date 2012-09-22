using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace smithwaterman
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private int gap = -1;   //sets gap penalty
        private int match = 2;  //sets match score
        int[,] scores;          //scoring matrix


    }
}
