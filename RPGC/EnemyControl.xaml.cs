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
using System.Globalization;
using CardExplorer;

namespace RPGC
{
    /// <summary>
    /// Interaction logic for EnemyControl.xaml
    /// </summary>
    public partial class EnemyControl : UserControl
    {
        protected Game game;

        /*** constructor ***/

        public EnemyControl()
        {
            InitializeComponent();
        }

        /*** public ***/

        public void UpdateView(Enemy enemy, int time)
        {
            //set the content for state
            this.State.Content = enemy.GetState();

            //set the content for HP/MP
            this.HP.Content = enemy.GetHp();
            this.HP_Max.Content = enemy.GetMaxHp(time);
            this.MP.Content = enemy.GetMp();
            this.MP_Max.Content = enemy.GetMaxMp(time);

            //get the content for the level and exp
            this.Level.Content = enemy.GetLevel(time);

            //set the background color
            if( enemy.IsReady() || !enemy.IsInitialized() )
            {
                this.Background = Brushes.Peru;
            }
            else
            {
                this.Background = Brushes.LightGray;
            }
        }

        public void EnterCard()
        {
            //check the input value
            String value = this.CardReader.Text;
            int num = -1;
            Int32.TryParse(value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out num);

            //check if the string is the correct format then try it
            if ((value.Length == 4) && (num >= 0) &&
                (this.Game().EnemyCardInput(num)))
            {
                this.GoodInput(value);
                this.Game().UpdateWindow();
            }//if
            else
            {
                this.FailedInput(value);
            }//else
        }

        /*** helper functions ***/

        private Game Game()
        {
            if (this.game == null)
            {
                MainWindow window = (MainWindow)Window.GetWindow(this);
                this.game = window.game;
            }
            return this.game;
        }

        private void FailedInput(String display)
        {
            this.CardReader.FontWeight = FontWeights.Normal;
        }

        private void GoodInput(String display)
        {
            this.CardReader.FontWeight = FontWeights.Bold;
            this.CardReader.Text = display.ToUpper();
        }

        /*** event handling ***/

        private void Up_Click(object sender, RoutedEventArgs e)
        {
            if (this.Game().CursorInput(-1, Board.Move.UP))
            {
                this.Game().UpdateWindow();
            }
        }

        private void Right_Click(object sender, RoutedEventArgs e)
        {
            if (this.Game().CursorInput(-1, Board.Move.RIGHT))
            {
                this.Game().UpdateWindow();
            }
        }

        private void Down_Click(object sender, RoutedEventArgs e)
        {
            if (this.Game().CursorInput(-1, Board.Move.DOWN))
            {
                this.Game().UpdateWindow();
            }
        }

        private void Left_Click(object sender, RoutedEventArgs e)
        {
            if (this.Game().CursorInput(-1, Board.Move.LEFT))
            {
                this.Game().UpdateWindow();
            }
        }

        private void ReadCard_Click(object sender, RoutedEventArgs e)
        {
            Enemy enemy = this.Game().GetEnemy();
            if( enemy.IsMoving() )
            {
                if( this.Game().MoveInput(-1) ) // -1 is for the enemy
                {
                    this.Game().UpdateWindow();
                }
            }//if moving
            else
            {
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    if ( this.Game().Undo(-1) ) // -1 is for the enemy
                    {
                        this.Game().UpdateWindow();
                    }
                }
                else
                {

                    this.EnterCard();
                }
            }//else not moving
        }

    }
}
