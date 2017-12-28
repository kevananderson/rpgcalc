using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Colors = System.Windows.Media.Colors;

namespace RPGC
{
    public class ViewControl
    {
        protected MainWindow window;

        protected PlayerControl player1;
        protected PlayerControl player2;
        protected PlayerControl player3;
        protected PlayerControl player4;

        protected EnemyControl enemy;

        protected BoardControl board;

        protected HexControl hex1;
        protected HexControl hex2;
        protected HexControl hex3;
        protected HexControl hex4;
        protected HexControl hex5;
        protected HexControl hex6;
        protected HexControl hex7;
        protected HexControl hex8;
        protected HexControl hex9;
        protected HexControl hex10;
        protected HexControl hex11;
        protected HexControl hex12;
        protected HexControl hex13;
        protected HexControl hex14;
        protected HexControl hex15;
        protected HexControl hex16;
        protected HexControl hex17;
        protected HexControl hex18;
        protected HexControl hex19;

        protected HexControl[] hexes;
        protected PlayerControl[] players;

        protected static SolidColorBrush[] colors = new SolidColorBrush[] { new SolidColorBrush(Colors.Peru),
                                    new SolidColorBrush(Colors.Red), new SolidColorBrush(Colors.Green),
                                    new SolidColorBrush(Colors.Blue),new SolidColorBrush(Colors.Yellow) };

        /*** constructor ***/

        public ViewControl(MainWindow window)
        {
            this.window = window;

            this.player1 = window.Player1;
            this.player2 = window.Player2;
            this.player3 = window.Player3;
            this.player4 = window.Player4;

            this.enemy = window.Enemy;

            this.board = window.Board;

            this.hex1 = window.Board.Hex1;
            this.hex2 = window.Board.Hex2;
            this.hex3 = window.Board.Hex3;
            this.hex4 = window.Board.Hex4;
            this.hex5 = window.Board.Hex5;
            this.hex6 = window.Board.Hex6;
            this.hex7 = window.Board.Hex7;
            this.hex8 = window.Board.Hex8;
            this.hex9 = window.Board.Hex9;
            this.hex10 = window.Board.Hex10;
            this.hex11 = window.Board.Hex11;
            this.hex12 = window.Board.Hex12;
            this.hex13 = window.Board.Hex13;
            this.hex14 = window.Board.Hex14;
            this.hex15 = window.Board.Hex15;
            this.hex16 = window.Board.Hex16;
            this.hex17 = window.Board.Hex17;
            this.hex18 = window.Board.Hex18;
            this.hex19 = window.Board.Hex19;

            this.hexes = new HexControl[] {  this.hex1, this.hex2, this.hex3, this.hex4, this.hex5, this.hex6,  this.hex7,
                                             this.hex8, this.hex9, this.hex10, this.hex11, this.hex12, this.hex13, this.hex14,
                                             this.hex15,  this.hex16, this.hex17, this.hex18, this.hex19 };

            this.players = new PlayerControl[] { this.player1, this.player2, this.player3, this.player4 };

            //set the numbers on the hexes
            for( int i = 0; i < this.hexes.Length; i++ )
            {
                this.hexes[i].Text.Content = i + 1;
                this.hexes[i].ClearHex();
            }
        }

        /*** public ***/

        public void UpdateWindow(Game game)
        {
            this.ClearBoard();

            this.player1.UpdateView(game.GetPlayer(0), game.GetTime());
            this.UpdateBoard(game.GetPlayer(0));

            this.player2.UpdateView(game.GetPlayer(1), game.GetTime());
            this.UpdateBoard(game.GetPlayer(1));

            this.player3.UpdateView(game.GetPlayer(2), game.GetTime());
            this.UpdateBoard(game.GetPlayer(2));

            this.player4.UpdateView(game.GetPlayer(3), game.GetTime());
            this.UpdateBoard(game.GetPlayer(3));

            this.enemy.UpdateView(game.GetEnemy(), game.GetTime());
            this.UpdateBoard(game.GetEnemy());

            this.SetTime(game.GetTime());
        }

        public void ShowUndo(Player[] players, Enemy enemy)
        {
            for(int i = 0; i < this.players.Length; i++)
            {
                if( players[i].CanUndo() ) this.players[i].ReadCard.Content = "Undo";
            }

            if (enemy.CanUndo()) this.enemy.ReadCard.Content = "Undo";
        }

        public void HideUndo()
        {
            for (int i = 0; i < this.players.Length; i++)
            {
                this.players[i].ReadCard.Content = "Enter";
            }

            this.enemy.ReadCard.Content = "Enter";
        }

        /*** protected ***/

        protected void ClearBoard()
        {
            foreach(HexControl hex in this.hexes)
            {
                hex.ClearHex();
            }
        }

        protected void SetTime( int time )
        {
            this.board.Time.Content = time;
        }

        protected void UpdateBoard(Piece piece)
        {
            //get player/enemy id
            int pieceIdx = ViewControl.GetPieceIdx(piece);

            //piece
            //get our location
            int idx = piece.GetLocation().GetLocation() - 1;
            this.hexes[idx].Piece.Fill = ViewControl.colors[pieceIdx];

            //move
            string state = piece.GetState();
            if(state  == "MOVE")
            {
                //get our cursor position
                idx = piece.GetCursor().GetLocation() - 1;
                switch(pieceIdx)
                {
                    case 0:
                        this.hexes[idx].EnemyMove.Visibility = System.Windows.Visibility.Visible;

                        idx = piece.GetTarget().GetLocation().GetLocation() - 1;
                        this.hexes[idx].EnemyTarget.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case 1:
                        this.hexes[idx].Player1Move.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case 2:
                        this.hexes[idx].Player2Move.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case 3:
                        this.hexes[idx].Player3Move.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case 4:
                        this.hexes[idx].Player4Move.Visibility = System.Windows.Visibility.Visible;
                        break;
                    default:
                        break;
                }//switch
            }//if
            else if(state == "TARGET")
            {
                //get our cursor position
                idx = piece.GetCursor().GetLocation() - 1;
                switch (pieceIdx)
                {
                    case 1:
                        this.hexes[idx].Player1Target.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case 2:
                        this.hexes[idx].Player2Target.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case 3:
                        this.hexes[idx].Player3Target.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case 4:
                        this.hexes[idx].Player4Target.Visibility = System.Windows.Visibility.Visible;
                        break;
                    default:
                        break;
                }//case
            }//else if
            else
            {
                //get our targets position
                idx = piece.GetTarget().GetLocation().GetLocation() - 1;
                switch (pieceIdx)
                {
                    case 0:
                        this.hexes[idx].EnemyTarget.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case 1:
                        this.hexes[idx].Player1Target.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case 2:
                        this.hexes[idx].Player2Target.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case 3:
                        this.hexes[idx].Player3Target.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case 4:
                        this.hexes[idx].Player4Target.Visibility = System.Windows.Visibility.Visible;
                        break;
                    default:
                        break;
                }//switch
            }//else
        }

        protected static int GetPieceIdx(Piece piece)
        {
            if( piece is Enemy)
            {
                return 0;
            }
            if( piece is Player)
            {
                return ((Player)piece).GetNumber();
            }
            return 0;
        }

    }
}
