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
        Alignment alignment;

        public MainWindow()
        {
            InitializeComponent();
            aTextBox.Text = "ACACACTA";
            bTextBox.Text = "AGCACACA";
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //new instance of class Alignment with parameters
            //sequence A, B, the gap penalty, the match score
            //converts to upper case because of stylistic choices
            alignment = new Alignment(aTextBox.Text.ToUpper(), bTextBox.Text.ToUpper(), -1, 2);

            //run the alignments
            alignment.alignSWA();
            alignment.alignAGSWA();

            //set the result textboxes for the alignments
            aresultTextBox.Text = alignment.getAlignmentA();
            bresultTextBox.Text = alignment.getAlignmentB();

            //set the result textboxes for the affine gap alignments
            maresultTextBox.Text = alignment.getAGAlignmentA();
            mbresultTextBox.Text = alignment.getAGAlignmentB();
        }
    }
}
