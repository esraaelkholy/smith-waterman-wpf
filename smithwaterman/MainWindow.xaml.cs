using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace smithwaterman
{
    public partial class MainWindow : Window
    {
        private int gap = -1;   //sets gap penalty
        private int match = 2;  //sets match score
        int[,] scores;          //scoring matrix

        public MainWindow()
        {
            InitializeComponent();
            aTextBox.Text = "ACACACTA";
            bTextBox.Text = "AGCACACA";
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            string[] alignments = SmithWatermanAlignment(aTextBox.Text, bTextBox.Text);
            aresultTextBox.Text = alignments[0];
            bresultTextBox.Text = alignments[1];
            string[] malignments = modifiedSmithWatermanAlignment(aTextBox.Text, bTextBox.Text);
            maresultTextBox.Text = malignments[0];
            mbresultTextBox.Text = malignments[1];
        }

        private string[] SmithWatermanAlignment(string a, string b)
        {
            int m = a.Length;
            int n = b.Length;
            char[] achars = a.ToCharArray();
            char[] bchars = b.ToCharArray();
            scores = new int[m+1,n+1];
            string[] alignments;

            initScores(m, n);

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cellscore = 0;
                    int[] results = new int[4];
                    if (achars[i - 1] == bchars[j - 1])
                    {
                        results[0] = scores[i - 1, j - 1] + match;
                        results[1] = scores[i - 1, j];
                        results[2] = scores[i, j - 1];
                    }
                    else
                    {
                        results[0] = scores[i - 1, j - 1];
                        results[1] = scores[i - 1, j] + gap;
                        results[2] = scores[i, j - 1] + gap;
                    }
                    results[3] = 0;
                    cellscore = results.Max();
                    scores[i, j] = cellscore;
                }
            }
            
            alignments = traceBack(m, n, achars, bchars);
            return alignments;
        }

        private void initScores(int m, int n)
        {
            for (int i = 0; i <= m; i++)
            {
                //populate zeroes for rows
                scores[i, 0] = 0;
            }
            for (int i = 0; i <= n; i++)
            {
                //populate zeroes for cols
                scores[0, 1] = 0;
            }
        }

        private string[] traceBack(int m, int n, char[] achars, char[] bchars)
        {
            int[] startcoords = findMaxCoord(m,n);
            List<int[]> trace = traceScores(startcoords);
            string[] alignments = traceSequences(trace, achars, bchars);
            return alignments;
        }

        private int[] findMaxCoord(int m, int n)
        {
            int[] coords = new int[2];
            int score = 0;
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    if (scores[i, j] > score)
                    {
                        score = scores[i, j];
                        coords[0] = i;
                        coords[1] = j;
                    }   
                }
            }
            return coords;
        }

        private List<int[]> traceScores(int[] startcoords)
        {
            List<int[]> trace = new List<int[]>();

            int m = startcoords[0];
            int n = startcoords[1];

            trace.Add(startcoords);

            int result = 1;

            while (result != 0 && m != 1 && n != 1)
            {
                result = scores[n - 1, m - 1];
                int uresult = scores[n - 1, m];
                int lresult = scores[n, m - 1];
                if (uresult > result)
                {
                    result = uresult;
                    n--;
                }
                else if (lresult > result)
                {
                    result = lresult;
                    m--;
                }
                else
                {
                    m--; n--;
                }
                int[] coords = new int[2];
                coords[0] = n;
                coords[1] = m;
                trace.Add(coords);
            }

            return trace;
        }

        private string[] traceSequences(List<int[]> trace, char[] achars, char[] bchars)
        {
            string[] alignments = new string[2];
            List<char> alist = new List<char>(); //figure out how to use stringbuffers later
            List<char> blist = new List<char>();

            int prevn = 0;
            int prevm = 0;

            foreach (int[] coords in trace)
            {
                if (prevn == 0 && prevm == 0) //first case
                {
                    int n = coords[0]-1;
                    int m = coords[1]-1;
                    alist.Add(achars[n]);
                    blist.Add(bchars[m]);
                    prevn = n;
                    prevm = m;
                }
                else //following
                {                    
                    int n = coords[0]-1;
                    int m = coords[1]-1;
                    if (n != prevn)
                        alist.Add(achars[n]);
                    else
                        alist.Add('-');
                    if (m != prevm)
                        blist.Add(bchars[m]);
                    else
                        blist.Add('-');
                    prevn = n;
                    prevm = m;
                }
            }
            alist.Reverse();
            blist.Reverse();

            alignments[0] = string.Join("", alist.ToArray());
            alignments[1] = string.Join("", blist.ToArray());

            return alignments;
        }

        private string[] modifiedSmithWatermanAlignment(string a, string b)
        {
            int m = a.Length;
            int n = b.Length;
            char[] achars = a.ToCharArray();
            char[] bchars = b.ToCharArray();
            scores = new int[m + 1, n + 1];
            string[] alignments;
            int affinegap = gap;
            int gaplength = 0;
            int gapgrower = -1;

            initScores(m, n);

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cellscore = 0;
                    int[] results = new int[4];
                    if (achars[i - 1] == bchars[j - 1])
                    {
                        affinegap = gap;
                        gaplength = 0;
                        results[0] = scores[i - 1, j - 1] + match;
                        results[1] = scores[i - 1, j];
                        results[2] = scores[i, j - 1];
                    }
                    else
                    {
                        affinegap = gap + gapgrower * (gaplength - 1);
                        results[0] = scores[i - 1, j - 1];
                        results[1] = scores[i - 1, j] + affinegap;
                        results[2] = scores[i, j - 1] + affinegap;
                        gaplength++;
                    }
                    results[3] = 0;
                    cellscore = results.Max();
                    scores[i, j] = cellscore;
                }
            }

            alignments = traceBack(m, n, achars, bchars);
            return alignments;
        }
    }
}
