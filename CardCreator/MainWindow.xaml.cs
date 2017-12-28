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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.CardGroup.ItemsSource = CardExplorer.Card.group_string;
            this.CardGroup.SelectedIndex = 0;

            Loaded += MainWindowLoaded;
        }

        public void View()
        {
            this.MakeAllHidden();
            int group = this.CardGroup.SelectedIndex;
            int cid = 0;
            switch((CardExplorer.Card.Group)group)
            {
                case CardExplorer.Card.Group.ACTOR:
                    this.Actor.Visibility = Visibility.Visible;
                    cid = this.Actor.View();
                    break;
                case CardExplorer.Card.Group.EQUIPMENT:
                    this.Equipment.Visibility = Visibility.Visible;
                    cid = this.Equipment.View();
                    break;
                case CardExplorer.Card.Group.SKILL:
                    this.Skill.Visibility = Visibility.Visible;
                    cid = this.Skill.View();
                    break;
                case CardExplorer.Card.Group.GLAMOR:
                    this.Glamour.Visibility = Visibility.Visible;
                    cid = this.Glamour.View();
                    break;
                default:
                    cid = 0;
                    break;
            }

            //add our bits and pass it on
            cid |= ((group << CardExplorer.Card.group_shift) & CardExplorer.Card.group_mask);

            this.CIDx.Text = cid.ToString("X4");
            this.Card.Content = CardExplorer.Card.Factory(cid).ToString();
        }

        /*** private window events ***/

        private void MainWindowLoaded(Object sender, RoutedEventArgs e)
        {
            this.View();
        }

        private void MakeAllHidden()
        {
            this.Actor.Visibility = Visibility.Hidden;
            this.Equipment.Visibility = Visibility.Hidden;
            this.Skill.Visibility = Visibility.Hidden;
            this.Glamour.Visibility = Visibility.Hidden;
        }

        private void UpdateView(object sender, SelectionChangedEventArgs e)
        {
            this.View();
        }
    }
}
