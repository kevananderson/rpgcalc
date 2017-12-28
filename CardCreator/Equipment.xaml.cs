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
    public partial class Equipment : UserControl
    {
        public Equipment()
        {
            InitializeComponent();
            this.EquipmentType.ItemsSource = CardExplorer.Equipment.type_string;
            this.EquipmentType.SelectedIndex = 0;

            this.Level.ItemsSource = CardExplorer.Equipment.level_string;
            this.Level.SelectedIndex = 0;

            this.Affect.ItemsSource = CardExplorer.Equipment.affect_string;
            this.Affect.SelectedIndex = 0;

            this.Push.ItemsSource = CardExplorer.Equipment.push_string;
            this.Push.SelectedIndex = 0;

            this.Stats.ItemsSource = CardExplorer.Equipment.stats_string;
            this.Stats.SelectedIndex = 0;
        }


        public int View()
        {
            int cid = 0;

            int type = this.EquipmentType.SelectedIndex;
            int level = this.Level.SelectedIndex;
            int affect = this.Affect.SelectedIndex;
            int push = this.Push.SelectedIndex;
            int stats = this.Stats.SelectedIndex;

            //add our bits and pass it on
            cid |= ((type << CardExplorer.Equipment.type_shift) & CardExplorer.Equipment.type_mask);
            cid |= ((level << CardExplorer.Equipment.level_shift) & CardExplorer.Equipment.level_mask);
            cid |= ((affect << CardExplorer.Equipment.affect_shift) & CardExplorer.Equipment.affect_mask);
            cid |= ((push << CardExplorer.Equipment.push_shift) & CardExplorer.Equipment.push_mask);
            cid |= ((stats << CardExplorer.Equipment.stats_shift) & CardExplorer.Equipment.stats_mask);

            return cid;
        }


        /*** private window events ***/

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
