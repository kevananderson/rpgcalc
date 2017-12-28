using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardExplorer;

namespace RPGC
{
    public class Board
    {
        public enum Move { UP, DOWN, LEFT, RIGHT };

        protected const int X = 0;
        protected const int Y = 1;

        //           1
        //        2     3
        //     4     5     6
        //        7     8     
        //     9    10    11 
        //       12    13
        //    14    15    16
        //       17    18
        //          19     

        protected static double[][] position = new double[19][]//2
        { //                    x,    y
            new double[] {      0,    2 }, // 1
            new double[] { -0.866,  1.5 }, // 2
            new double[] {  0.866,  1.5 }, // 3
            new double[] { -1.732,    1 }, // 4
            new double[] {      0,    1 }, // 5
            new double[] {  1.732,    1 }, // 6
            new double[] { -0.866,  0.5 }, // 7
            new double[] {  0.866,  0.5 }, // 8
            new double[] { -1.732,    0 }, // 9
            new double[] {      0,    0 }, // 10
            new double[] {  1.732,    0 }, // 11
            new double[] { -0.866, -0.5 }, // 12
            new double[] {  0.866, -0.5 }, // 13
            new double[] { -1.732,   -1 }, // 14
            new double[] {      0,   -1 }, // 15
            new double[] {  1.732,   -1 }, // 16
            new double[] { -0.866, -1.5 }, // 17
            new double[] {  0.866, -1.5 }, // 18
            new double[] {      0,   -2 }  // 19
        };

        protected static int[,] graph = new int[19, 4]
        { //  Up, Dwn, Lft, Rgt
            { -1,   5,   2,   3 }, // 1
            {  1,   7,   4,   1 }, // 2
            {  1,   8,   1,   6 }, // 3
            {  2,   9,  -1,   2 }, // 4
            {  1,  10,   2,   3 }, // 5
            {  3,  11,   3,  -1 }, // 6
            {  2,  12,   4,   5 }, // 7
            {  3,  13,   5,   6 }, // 8
            {  4,  14,  -1,   7 }, // 9
            {  5,  15,   7,   8 }, // 10
            {  6,  16,   8,  -1 }, // 11
            {  7,  17,   9,  10 }, // 12
            {  8,  18,  10,  11 }, // 13
            {  9,  17,  -1,  12 }, // 14
            { 10,  19,  12,  13 }, // 15
            { 11,  18,  13,  -1 }, // 16
            { 12,  19,  14,  15 }, // 17
            { 13,  19,  15,  16 }, // 18
            { 15,  -1,  17,  18 }, // 19
        };

        protected static int[,] distance = new int[19, 19]
        { //  1  2  3  4  5  6  7  8  9 10 11 12 13 14 15 16 17 18 19
            { 0, 1, 1, 2, 1, 2, 2, 2, 3, 2, 3, 3, 3, 4, 3, 4, 4, 4, 4}, // 1
            { 0, 0, 2, 1, 1, 3, 1, 2, 2, 2, 3, 2, 3, 3, 3, 4, 3, 4, 4}, // 2
            { 0, 0, 0, 3, 1, 1, 2, 1, 3, 2, 2, 3, 2, 4, 3, 3, 4, 3, 4}, // 3
            { 0, 0, 0, 0, 2, 4, 1, 3, 1, 2, 4, 2, 3, 2, 3, 4, 3, 4, 4}, // 4
            { 0, 0, 0, 0, 0, 2, 1, 1, 2, 1, 2, 2, 2, 3, 2, 3, 3, 3, 3}, // 5
            { 0, 0, 0, 0, 0, 0, 3, 1, 4, 2, 1, 3, 2, 4, 3, 2, 4, 3, 4}, // 6
            { 0, 0, 0, 0, 0, 0, 0, 2, 1, 1, 3, 1, 2, 2, 2, 3, 2, 3, 3}, // 7
            { 0, 0, 0, 0, 0, 0, 0, 0, 3, 1, 1, 2, 1, 3, 2, 2, 3, 2, 3}, // 8
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 4, 1, 3, 1, 2, 4, 2, 3, 3}, // 9
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 1, 1, 2, 1, 2, 2, 2, 2}, // 10
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 1, 4, 2, 1, 3, 2, 3}, // 11
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 1, 1, 3, 1, 2, 2}, // 12
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 1, 1, 2, 1, 2}, // 13
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 4, 1, 3, 2}, // 14
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 1, 1, 1}, // 15
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 1, 2}, // 16
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 1}, // 17
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1}, // 18
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}  // 19
        };


        protected Hex[] board;

        /*** conctructor ***/

        public Board()
        {
            Game.Log(Game.LogLevel.TRACE, "% Board Constructor %");
            this.board = new Hex[]
            {
                new Hex(1, this), new Hex(2, this), new Hex(3, this), new Hex(4, this), new Hex(5, this),
                new Hex(6, this), new Hex(7, this), new Hex(8, this), new Hex(9, this), new Hex(10, this),
                new Hex(11, this), new Hex(12, this), new Hex(13, this), new Hex(14, this), new Hex(15, this),
                new Hex(16, this), new Hex(17, this), new Hex(18, this), new Hex(19, this)
            };
        }

        /*** public ***/

        public static int Distance( Hex a, Hex b )
        {
            Game.Log(Game.LogLevel.TRACE, "% Board.Distance %");
            //get the row and column
            int row, col;
            if( a < b )
            {
                row = a.GetLocation() - 1;
                col = b.GetLocation() - 1;
            }
            else
            {
                row = b.GetLocation() - 1;
                col = a.GetLocation() - 1;
            }

            return Board.distance[row, col];
        }

        public static Skill.Position Position( Hex enemy, Hex target, Hex actor )
        {
            Game.Log(Game.LogLevel.TRACE, "% Board.Position %");
            //calculate our angle
            double baseAngle = Board.GetAngle(enemy, target);
            double actorAngle = Board.GetAngle(enemy, actor);
            double positionAngle = baseAngle - actorAngle;

            //normalize it
            while (positionAngle > 180) positionAngle -= 360;
            while (positionAngle <= -180) positionAngle += 360;
            positionAngle = Math.Abs(positionAngle);

            if (positionAngle < 30)
                return Skill.Position.FORWARD;
            else if (positionAngle <= 90)
                return Skill.Position.FRONT_FLANK;
            else if (positionAngle <= 150)
                return Skill.Position.BACK_FLANK;
            else
                return Skill.Position.REAR;
        }

        public Hex GetHex(int number)
        {
            Game.Log(Game.LogLevel.TRACE, "% Board.GetHex %");
            number--;
            if (number < 0 || number >= 19) return null;

            return board[number];
        }

        public bool SetPieceAt(Piece piece, int place)
        {
            Game.Log(Game.LogLevel.TRACE, "% Board.SetPieceAt %");
            Hex hex = this.GetHex(place);
            if( (hex == null) || hex.IsOccupied(piece) ) return false;

            //remove the piece from the board
            foreach( Hex clearHex in this.board )
            {
                if (clearHex.GetPieceAtHex() == piece)
                    clearHex.ClearPiece();
            }

            piece.SetLocation(hex); // this will cause the player to add some movement time
            hex.SetPiece(piece); 
            return true;
        }

        public Hex GetHexByMove( int start, Board.Move move )
        {
            Game.Log(Game.LogLevel.TRACE, "% Board.GetHexByMove %");
            start--;
            if ((start < 0) || (start >= 19)) return null;

            //we have a valid start
            int next = Board.graph[start, (int)move];
            next--;

            //it was not a valid move from this starting position
            if (next < 0) return null;

            return this.board[next];
        }

        public void ClearPieceFromBoard( Piece piece )
        {
            Game.Log(Game.LogLevel.TRACE, "% Board.ClearPieceFromBoard %");
            foreach ( Hex hex in this.board )
            {
                if (hex.GetPieceAtHex() == piece)
                    hex.ClearPiece();                 
            }
        }

        public void SetPieceRandomly(Piece piece)
        {
            Game.Log(Game.LogLevel.TRACE, "% Board.SetPieceRandomly %");
            Random random = new Random();
            bool set = false;
            while( !set )
            {
                int location = random.Next(1, 20);
                set = this.SetPieceAt(piece, location);
            }
        }

        /*** protected ***/

        protected static double GetAngle( Hex a, Hex b)
        {
            double x = Board.position[b.GetLocation() - 1][X] - Board.position[a.GetLocation() - 1][X];
            double y = Board.position[b.GetLocation() - 1][Y] - Board.position[a.GetLocation() - 1][Y];
            return 180 * Math.Atan2(y, x) / Math.PI;
        }
    }

    public class Hex : IComparable
    {
        protected int location = 0;
        protected Piece piece = null;
        protected Board board = null;

        /*** conctructor ***/

        public Hex( int location, Board board )
        {
            this.location = location;
            this.board = board;
        }

        /*** public ***/

        public int GetLocation()
        {
            return this.location;
        }

        public bool IsOccupied()
        {
            return (this.piece != null);
        }

        public bool IsOccupied(Piece piece)
        {
            return ((this.piece != null) && (this.piece != piece));
        }

        //only called by the board
        public void SetPiece(Piece piece)
        {
            this.piece = piece;
        }

        //only called by board
        public void ClearPiece()
        {
            this.piece = null;
        }

        public Hex GetHexByMove( Board.Move move )
        {
            Game.Log(Game.LogLevel.TRACE, "% Hex.GetHexByMove %");
            return this.board.GetHexByMove(this.location, move);
        }

        public Piece GetPieceAtHex()
        {
            return this.piece;
        }

        public override string ToString()
        {
            return "Hex " + this.location;
        }

        /*** interface ***/

        public int CompareTo(Object other)
        {
            if (other == null) return -1;
            if (!(other is Hex)) return -1;
            return this.location - ((Hex)other).location;
        }

        public int CompareTo(Hex other)
        {
            if (other == null) return -1;
            return this.location - other.location;
        }

        public static int Compare(Hex left, Hex right)
        {
            if (left == null) return 1;
            return left.CompareTo(right);
        }

        public override bool Equals( Object other )
        {
            if (other == null) return false;
            return ((other is Hex) && (this.location == ((Hex)other).location));
        }

        public override int GetHashCode()
        {
            return this.location;
        }

        public static bool operator ==(Hex left, Hex right)
        {
            if( Object.ReferenceEquals(null, left) )
            {
                if (Object.ReferenceEquals(null, right)) return true;
                else return false;
            }
            return left.Equals(right);
        }
        public static bool operator !=(Hex left, Hex right)
        {
            return !(left == right);
        }
        public static bool operator <(Hex left, Hex right)
        {
            return (Compare(left, right) < 0);
        }
        public static bool operator >(Hex left, Hex right)
        {
            return (Compare(left, right) > 0);
        }

    }
}
