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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Game game;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindowLoaded;
            Closing += MainWindowClosing;
        }

        /*** protected override events ***/

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if ( Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) )
            {
                game.ShowUndo();
            }
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
            {
                game.HideUndo();
            }
            base.OnKeyUp(e);
        }

        /*** private window events ***/

        private void MainWindowLoaded(Object sender, RoutedEventArgs e)
        {
            this.game = new Game();
            this.game.AddMainWindow(this);
        }

        private void MainWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Game.Log(Game.LogLevel.MINIMAL, "*** GAME OVER ***");

            Game.CloseLog();
        }

    }
}
