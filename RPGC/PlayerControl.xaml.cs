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
    /// Interaction logic for PlayerControl.xaml
    /// </summary>
    public partial class PlayerControl : UserControl
    {
        protected Game game;
        protected SolidColorBrush[] colors = new SolidColorBrush[]
                        { Brushes.Red, Brushes.Green, Brushes.Blue, Brushes.Yellow };

        /*** constructor ***/

        public PlayerControl()
        {
            InitializeComponent();
        }

        /*** public ***/

        public void UpdateView(Player player, int time)
        {
            //set the content for state
            this.State.Content = player.GetState();

            //set the content for HP/MP
            this.HP.Content = player.GetHp();
            this.HP_Max.Content = player.GetMaxHp(time);
            this.MP.Content = player.GetMp();
            this.MP_Max.Content = player.GetMaxMp(time);

            //get the content for the level and exp
            this.Level.Content = player.GetLevel(time);
            this.Exp.Content = player.GetExperience();

            //update the stats
            this.Strength.Content = player.GetStat(Card.Stat.STRENGTH, time);
            this.Grit.Content = player.GetStat(Card.Stat.GRIT, time);
            this.Speed.Content = player.GetStat(Card.Stat.SPEED, time);
            this.Balance.Content = player.GetStat(Card.Stat.BALANCE, time);
            this.Faith.Content = player.GetStat(Card.Stat.FAITH, time);
            this.Focuss.Content = player.GetStat(Card.Stat.FOCUS, time);
            this.Luck.Content = player.GetStat(Card.Stat.LUCK, time);
            this.Allure.Content = player.GetStat(Card.Stat.ALLURE, time);

            //set the background color
            this.Background = Brushes.White;
            this.Colorboard.Fill = colors[ player.GetNumber() - 1 ];
            if (player.IsReady() || !player.IsInitialized())
            {
                this.Baseboard.Fill = colors[ player.GetNumber() - 1 ];
                this.Baseboard.Opacity = .5;
                this.Backboard.Fill = colors[ player.GetNumber() - 1 ];
            }
            else
            {
                this.Baseboard.Fill = Brushes.LightGray;
                this.Baseboard.Opacity = 1;
                this.Backboard.Fill = Brushes.LightGray;
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
                (this.Game().PlayerCardInput(this.PlayerId(), num)))
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

        private int PlayerId()
        {
            int numidx = this.Name.IndexOf('r') + 1;
            String num = this.Name.Substring(numidx);
            int pid = 0;
            Int32.TryParse(num, out pid);
            pid--;
            return pid;
        }

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
            if (this.Game().CursorInput(this.PlayerId(), Board.Move.UP))
            {
                this.Game().UpdateWindow();
            }
        }

        private void Right_Click(object sender, RoutedEventArgs e)
        {
            if (this.Game().CursorInput(this.PlayerId(), Board.Move.RIGHT))
            {
                this.Game().UpdateWindow();
            }
        }

        private void Down_Click(object sender, RoutedEventArgs e)
        {
            if (this.Game().CursorInput(this.PlayerId(), Board.Move.DOWN))
            {
                this.Game().UpdateWindow();
            }
        }

        private void Left_Click(object sender, RoutedEventArgs e)
        {
            if( this.Game().CursorInput(this.PlayerId(), Board.Move.LEFT) )
            {
                this.Game().UpdateWindow();
            }
        }

        private void ReadCard_Click(object sender, RoutedEventArgs e)
        {
            Player player = this.Game().GetPlayer(this.PlayerId());
            if( player.IsMoving() )
            {
                if( this.Game().MoveInput(this.PlayerId()) )
                {
                    this.Game().UpdateWindow();
                }//if move input succes
            }//if is moving
            else if( player.IsTargeting() )
            {
                bool success = false;

                if( Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) )
                {
                    success = this.Game().Undo( this.PlayerId());
                }//if undo from targeting
                else
                {
                    success = this.Game().TargetInput(this.PlayerId());
                }//else targeting

                if(success)
                {
                    this.Game().UpdateWindow();
                }//if success
            }//else if targeting
            else
            {
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    if( this.Game().Undo( this.PlayerId() ) )
                    {
                        this.Game().UpdateWindow();
                    }//if undo was succesful
                }//if shift pressed
                else
                {
                    this.EnterCard();
                }//else enter card

            }//else everything else
        }

    }
}
