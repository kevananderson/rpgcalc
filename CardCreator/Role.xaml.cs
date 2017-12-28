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
    /// Interaction logic for Role.xaml
    /// </summary>
    public partial class Role : UserControl
    {
        public Role()
        {
            InitializeComponent();
            this.Tradeoff.Content = CardExplorer.Role.tradeoff_table;
        }

        public int View()
        {
            int cid = 0;

            //add our bits and pass it on
            cid |= (((bool)this.Trade00.IsChecked ? 1 : 0) << 0);
            cid |= (((bool)this.Trade01.IsChecked ? 1 : 0) << 1);
            cid |= (((bool)this.Trade02.IsChecked ? 1 : 0) << 2);
            cid |= (((bool)this.Trade03.IsChecked ? 1 : 0) << 3);
            cid |= (((bool)this.Trade04.IsChecked ? 1 : 0) << 4);
            cid |= (((bool)this.Trade05.IsChecked ? 1 : 0) << 5);
            cid |= (((bool)this.Trade06.IsChecked ? 1 : 0) << 6);
            cid |= (((bool)this.Trade07.IsChecked ? 1 : 0) << 7);
            cid |= (((bool)this.Trade08.IsChecked ? 1 : 0) << 8);
            cid |= (((bool)this.Trade09.IsChecked ? 1 : 0) << 9);
            cid |= (((bool)this.Trade10.IsChecked ? 1 : 0) << 10);
            cid |= (((bool)this.Trade11.IsChecked ? 1 : 0) << 11);

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
    }
}
