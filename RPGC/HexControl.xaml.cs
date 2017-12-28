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

namespace RPGC
{
    /// <summary>
    /// Interaction logic for HexControl.xaml
    /// </summary>
    public partial class HexControl : UserControl
    {
        
        /*** constructor ***/

        public HexControl()
        {
            InitializeComponent();
        }

        /*** public ***/

        public void ClearHex()
        {
            this.Player1Move.Visibility = System.Windows.Visibility.Hidden;
            this.Player2Move.Visibility = System.Windows.Visibility.Hidden;
            this.Player3Move.Visibility = System.Windows.Visibility.Hidden;
            this.Player4Move.Visibility = System.Windows.Visibility.Hidden;

            this.Player1Target.Visibility = System.Windows.Visibility.Hidden;
            this.Player2Target.Visibility = System.Windows.Visibility.Hidden;
            this.Player3Target.Visibility = System.Windows.Visibility.Hidden;
            this.Player4Target.Visibility = System.Windows.Visibility.Hidden;

            this.EnemyMove.Visibility = System.Windows.Visibility.Hidden;
            this.EnemyTarget.Visibility = System.Windows.Visibility.Hidden;

            this.Piece.Fill = new SolidColorBrush( Colors.White );
        }
    }
}
