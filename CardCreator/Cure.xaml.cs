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
    public partial class Cure : UserControl
    {
        public Cure()
        {
            InitializeComponent();
            this.Effect.ItemsSource = CardExplorer.Cure.effect_string;
            this.Effect.SelectedIndex = 0;

            this.Num.ItemsSource = CardExplorer.Cure.num_string;
            this.Num.SelectedIndex = 0;

            this.Speed.ItemsSource = CardExplorer.Skill.speed_string;
            this.Speed.SelectedIndex = 0;

            this.Area.ItemsSource = CardExplorer.Skill.area_string;
            this.Area.SelectedIndex = 0;
        }

        public int View()
        {
            int cid = 0;

            int effect = this.Effect.SelectedIndex;
            int num = this.Num.SelectedIndex;
            int speed = this.Speed.SelectedIndex;
            int area = this.Area.SelectedIndex;

            //add our bits and pass it on

            //num is used to set bits in both the "range" and the "position" position
            cid |= ((num << (CardExplorer.Skill.range_shift - CardExplorer.Cure.level_shift)) & CardExplorer.Skill.range_mask);
            cid |= ((num << CardExplorer.Skill.position_shift) & CardExplorer.Skill.position_mask);

            cid |= ((speed << CardExplorer.Skill.speed_shift) & CardExplorer.Skill.speed_mask);
            cid |= ((area << CardExplorer.Skill.area_shift) & CardExplorer.Skill.area_mask);

            //effect replaces the max value
            cid |= ((effect << CardExplorer.Skill.max_shift) & CardExplorer.Skill.max_mask);

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
