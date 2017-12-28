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
using CardExplorer;

namespace CardCreator
{
    /// <summary>
    /// Interaction logic for Actor.xaml
    /// </summary>
    public partial class Glamour : UserControl
    {
        public Glamour()
        {
            InitializeComponent();

            this.Affect.ItemsSource = CardExplorer.Glamour.affect_string;
            this.Affect.SelectedIndex = 0;

            this.Potency.ItemsSource = CardExplorer.Glamour.potency_string;
            this.Potency.SelectedIndex = 1;

            this.Duration.ItemsSource = CardExplorer.Glamour.duration_string;
            this.Duration.SelectedIndex = 0;
        }

        public int View()
        {
            int cid = 0;

            int affect = this.Affect.SelectedIndex;
            int potency = this.Potency.SelectedIndex;
            int duration = this.Duration.SelectedIndex;

            //check if we are a max, with a multiple
            if( CardExplorer.Glamour.affect_string[affect].Contains("Maximum") )
            {
                //use maximum range
                this.Potency.ItemsSource = CardExplorer.Glamour.max_string;
            }
            //check if we use the correction, this overloads these glamours to have more functionality
            else if (CardExplorer.Glamour.affect_string[affect] == "Range" )
            {
                //make them visible
                this.CorrectionLabel.Visibility = Visibility.Visible;
                this.Correction.Visibility = Visibility.Visible;

                //change the range of the potency drop to match
                if( (bool)this.Correction.IsChecked )
                {
                    //use correction range
                    this.Potency.ItemsSource = CardExplorer.Glamour.correct_string;
                }
                else
                {
                    //use modification range
                    this.Potency.ItemsSource = CardExplorer.Glamour.range_string;
                }
            }
            else if(CardExplorer.Glamour.affect_string[affect] == "Position")
            {
                //make them visible
                this.CorrectionLabel.Visibility = Visibility.Visible;
                this.Correction.Visibility = Visibility.Visible;

                //change the range of the potency drop to match
                if ((bool)this.Correction.IsChecked)
                {
                    //use correction range
                    this.Potency.ItemsSource = CardExplorer.Glamour.correct_string;
                }
                else
                {
                    //use modification range
                    this.Potency.ItemsSource = CardExplorer.Glamour.position_string;
                }
            }
            else
            {
                //make them hidden
                this.CorrectionLabel.Visibility = Visibility.Hidden;
                this.Correction.Visibility = Visibility.Hidden;

                //use the normal source
                this.Potency.ItemsSource = CardExplorer.Glamour.potency_string;
            }

            //add our bits and pass it on
            cid |= ((affect << CardExplorer.Glamour.affect_shift) & CardExplorer.Glamour.affect_mask);
            cid |= ((potency << CardExplorer.Glamour.potency_shift) & CardExplorer.Glamour.potency_mask);
            cid |= ((duration << CardExplorer.Glamour.duration_shift) & CardExplorer.Glamour.duration_mask);

            return cid;
        }

        /*** private window events ***/

        private void UpdateView(object sender, RoutedEventArgs e)
        {
            MainWindow window = (MainWindow)Window.GetWindow(this);
            if (window != null)
            {
                window.View();
            }
        }

        private void UpdateView(object sender, SelectionChangedEventArgs e)
        {
            MainWindow window = (MainWindow)Window.GetWindow(this);
            if (window != null)
            {
                window.View();
            }
        }

    }
}
