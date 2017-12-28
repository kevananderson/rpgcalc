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
    public partial class Actor : UserControl
    {
        public Actor()
        {
            InitializeComponent();
            this.ActorType.ItemsSource = CardExplorer.Actor.type_string;
            this.ActorType.SelectedIndex = 0;
        }

        public int View()
        {
            this.MakeAllHidden();
            int type = this.ActorType.SelectedIndex;
            int cid = 0;
            switch ((CardExplorer.Actor.Type)type)
            {
                case CardExplorer.Actor.Type.ROLE:
                    this.Role.Visibility = Visibility.Visible;
                    cid = this.Role.View();
                    break;
                case CardExplorer.Actor.Type.CHARACTER:
                    this.Character.Visibility = Visibility.Visible;
                    cid = this.Character.View();
                    break;
                case CardExplorer.Actor.Type.CREATURE:
                    this.Creature.Visibility = Visibility.Visible;
                    cid = this.Creature.View();
                    break;
                default:
                    cid = 0;
                    break;
            }

            //add our bits and pass it on
            cid |= ((type << CardExplorer.Actor.type_shift) & CardExplorer.Actor.type_mask);

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
            this.Role.Visibility = Visibility.Hidden;
            this.Character.Visibility = Visibility.Hidden;
            this.Creature.Visibility = Visibility.Hidden;

        }


    }
}
