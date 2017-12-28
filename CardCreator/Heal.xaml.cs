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
    public partial class Heal : UserControl
    {
        public Heal()
        {
            InitializeComponent();
            this.Level.ItemsSource = CardExplorer.Heal.level_string;
            this.Level.SelectedIndex = 0;

            this.Speed.ItemsSource = CardExplorer.Skill.speed_string;
            this.Speed.SelectedIndex = 0;

            this.Area.ItemsSource = CardExplorer.Skill.area_string;
            this.Area.SelectedIndex = 0;

            this.Max.ItemsSource = CardExplorer.Skill.max_string;
            this.Max.SelectedIndex = 0;
        }

        public int View()
        {
            int cid = 0;

            int level = this.Level.SelectedIndex;
            int speed = this.Speed.SelectedIndex;
            int area = this.Area.SelectedIndex;
            int max = this.Max.SelectedIndex;

            //add our bits and pass it on

            //level is used to set bits in both the "range" and "position" position
            cid |= ((level << (CardExplorer.Skill.range_shift - CardExplorer.Heal.level_shift)) & CardExplorer.Skill.range_mask);
            cid |= ((level << CardExplorer.Skill.position_shift) & CardExplorer.Skill.position_mask);

            cid |= ((speed << CardExplorer.Skill.speed_shift) & CardExplorer.Skill.speed_mask);
            cid |= ((area << CardExplorer.Skill.area_shift) & CardExplorer.Skill.area_mask);
            cid |= ((max << CardExplorer.Skill.max_shift) & CardExplorer.Skill.max_mask);

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
