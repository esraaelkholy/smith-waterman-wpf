using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace smithwaterman
{
    public class Alignment
    {
        //sequences
        private string seqA;
        private string seqB;
        //normal returns
        private string resA;
        private string resB;
        //affine returns
        private string agaA;
        private string agaB;
        //constants
        private int gap;
        private int match;
        //2d array for scoring matrix
        private int[,] scores;

        public Alignment(string seqA, string seqB, int gap, int match)
        {
            this.gap = gap;
            this.match = match;
            this.seqA = seqA;
            this.seqB = seqB;
        }

        public void alignSWA()
        {
            string[] alignments = SmithWatermanAlignment(seqA, seqB);
            resA = alignments[0];
            resB = alignments[1];
        }

        public void alignAGSWA()
        {
            string[] alignments = modifiedSmithWatermanAlignment(seqA, seqB);
            agaA = alignments[0];
            agaB = alignments[1];
        }

        public string getAlignmentA()
        {
            return resA;
        }

        public string getAlignmentB()
        {
            return resB;
        }

        public string getAGAlignmentA()
        {
            return agaA;
        }

        public string getAGAlignmentB()
        {
            return agaB;
        }

        private string[] SmithWatermanAlignment(string a, string b)
        {
            int m = a.Length;
            int n = b.Length;
            char[] achars = a.ToCharArray();
            char[] bchars = b.ToCharArray();
            scores = new int[m + 1, n + 1];
            string[] alignments;

            initScores(m, n);

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    //what is greatest? 0
                    //diag + match case
                    //sides + gap case
                    //assign greatest value to cell
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
            int[] startcoords = findMaxCoord(m, n);
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
            
            //stack prevents editors from doing funny business with traceSequences
            Stack<char> astack = new Stack<char>();
            Stack<char> bstack = new Stack<char>();

            int prevn = 0;
            int prevm = 0;

            foreach (int[] coords in trace)
            {
                if (prevn == 0 && prevm == 0) //first case
                {
                    int n = coords[0] - 1;
                    int m = coords[1] - 1;
                    astack.Push(achars[n]);
                    bstack.Push(bchars[m]);
                    prevn = n;
                    prevm = m;
                }
                else //following
                {
                    int n = coords[0] - 1;
                    int m = coords[1] - 1;
                    if (n != prevn)
                        astack.Push(achars[n]);
                    else
                        astack.Push('-');
                    if (m != prevm)
                        bstack.Push(bchars[m]);
                    else
                        bstack.Push('-');
                    prevn = n;
                    prevm = m;
                }
            }

            alignments[0] = stackchartoString(astack);
            alignments[1] = stackchartoString(bstack);

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

        private string stackchartoString(Stack<char> stack)
        {
            string outstring="";
            foreach (char ch in stack)
            {
                outstring += ch;
            }

            return outstring;
        }
    }
}
