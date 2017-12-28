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

namespace CardCreator
{
    /// <summary>
    /// Interaction logic for Character.xaml
    /// </summary>
    public partial class Creature : UserControl
    {
        public Creature()
        {
            InitializeComponent();
            this.Tradeoff.Content = CardExplorer.Creature.tradeoff_table;

            this.Level.ItemsSource = CardExplorer.Creature.level_string;
            this.Level.SelectedIndex = 0;
        }

        public int View()
        {
            int cid = 0;

            int level = this.Level.SelectedIndex;
            cid |= ((level << CardExplorer.Creature.level_shift) & CardExplorer.Creature.level_mask);

            //add our bits and pass it on
            cid |= (((bool)this.Trade00.IsChecked ? 1 : 0) << 0);
            cid |= (((bool)this.Trade01.IsChecked ? 1 : 0) << 1);
            cid |= (((bool)this.Trade02.IsChecked ? 1 : 0) << 2);
            cid |= (((bool)this.Trade03.IsChecked ? 1 : 0) << 3);
            cid |= (((bool)this.Trade04.IsChecked ? 1 : 0) << 4);
            cid |= (((bool)this.Trade05.IsChecked ? 1 : 0) << 5);
            cid |= (((bool)this.Trade06.IsChecked ? 1 : 0) << 6);

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
