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
    public partial class SkillType : UserControl
    {
        public SkillType()
        {
            InitializeComponent();
            this.SkillTypeCombo.ItemsSource = CardExplorer.Skill.type_string;
            this.SkillTypeCombo.SelectedIndex = 0;
        }

        public int View()
        {
            this.MakeAllHidden();
            int type = this.SkillTypeCombo.SelectedIndex;
            int cid = 0;
            switch ((CardExplorer.Skill.Type)type)
            {
                case CardExplorer.Skill.Type.EDGE:
                case CardExplorer.Skill.Type.BLUNT:
                case CardExplorer.Skill.Type.PIERCING:
                case CardExplorer.Skill.Type.RANGED:
                case CardExplorer.Skill.Type.SPELL:
                    this.Skill.Visibility = Visibility.Visible;
                    cid = this.Skill.View();
                    break;
                case CardExplorer.Skill.Type.HEAL:
                    this.Heal.Visibility = Visibility.Visible;
                    cid = this.Heal.View();
                    break;
                case CardExplorer.Skill.Type.CURE:
                    this.Cure.Visibility = Visibility.Visible;
                    cid = this.Cure.View();
                    break;
                default:
                    cid = 0;
                    break;
            }

            //add our bits and pass it on
            cid |= ((type << CardExplorer.Skill.type_shift) & CardExplorer.Skill.type_mask);

            return cid;
        }

        /*** private window events ***/

        private void UpdateView(object sender, SelectionChangedEventArgs e)
        {
            MainWindow window = (MainWindow)Window.GetWindow(this);
            if( window != null )
            {
                window.View();
            }
        }

        private void MakeAllHidden()
        {
            this.Skill.Visibility = Visibility.Hidden;
            this.Heal.Visibility = Visibility.Hidden;
            this.Cure.Visibility = Visibility.Hidden;

        }


    }
}
