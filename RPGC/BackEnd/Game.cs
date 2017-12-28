using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardExplorer;
using System.IO;

namespace RPGC
{
    public class Game
    {
        public enum LogLevel { TRACE, DEBUG, NORMAL, MINIMAL };
        protected static LogLevel loglevel = Game.LogLevel.MINIMAL;

        //createing a function pointer called AdvanceGameState
        protected delegate void AdvanceGameCall();
        protected AdvanceGameCall AdvanceGameState;


        protected Player[] players = new Player[]
            {
                new Player(1), new Player(2), new Player(3), new Player(4)
            };
        protected Enemy enemy;
        protected Timeline timeline;
        protected Board board;
        protected bool defeat;

        protected ViewControl view;

        protected static StreamWriter log;
        protected static int time;
        protected static int fight;

        /*** constructor ***/
        public Game()
        {
            Game.fight = 1;
            Game.time = 0;
            Game.log = File.AppendText("Game" /*+ DateTime.Now.ToString("yyyyMMddHHmmss")*/ + ".log");
            Game.Log(Game.LogLevel.MINIMAL, "*** Start Game ***");
            Game.Log(Game.LogLevel.TRACE, "% Game Constructor %");

            this.enemy = new Enemy();
            this.AdvanceGameState = WaitForAllInit;
            this.timeline = new Timeline(players, enemy);
            this.board = new Board();

            this.defeat = false;

            this.InitializePieces();
        }

        /*** public ***/

        public void AddMainWindow(MainWindow window)
        {
            Game.Log(Game.LogLevel.TRACE, "% Game.AddMainWindow %");
            this.view = new ViewControl(window);
            this.view.UpdateWindow(this);
        }

        public bool PlayerCardInput(int player, int cid)
        {
            Game.Log(Game.LogLevel.TRACE, "% Game.PlayerCardInput %");
            Card card;
            bool success = false;

            //ensure real player number
            if ((player < 0) || (player > 3)) return false;

            //create the card
            try
            {
                card = Card.Factory(cid);
            }
            catch
            {
                return false;
            }

            //log the card
            Game.Log(Game.LogLevel.NORMAL, this.players[player].ToString() + " plays card: " + card.ToString() );

            //see if we are initialized
            if (!this.players[player].IsInitialized())
            {
                //we are not initialized, so we can try to apply the card
                success = this.players[player].ApplyCard(card);
            }
            else if ( this.players[player].IsReady())
            {
                //we are ready, so we can try to change equipment
                success = this.players[player].ApplyCard(card);

                //keep going if we did not succeed yet
                if (!success && this.players[player].IsActing() )
                {
                    //try to play the action
                    success = this.PlayActionCard(this.players[player], card);
                }
            }

            this.AdvanceGameState();

            return success;
        }

        public bool EnemyCardInput(int cid)
        {
            Game.Log(Game.LogLevel.TRACE, "% Game.EnemyCardInput %");
            Card card;
            bool success = false;

            //create the card
            try
            {
                card = Card.Factory(cid);
            }
            catch
            {
                return success;
            }

            //log the card
            Game.Log(Game.LogLevel.NORMAL, this.enemy.ToString() + " plays card: " + card.ToString());

            //always try to apply the card - assuming it is creature card. will fail if initailzed already
            success = enemy.ApplyCard(card);

            //keep going if we did not succeed yet
            if (!success)
            {
                //see if the enemy can play 
                if (enemy.IsActing())
                {
                    //it is our turn to play
                    success = this.PlayActionCard(enemy, card);
                }
            }

            this.AdvanceGameState();

            return success;
        }

        public bool CursorInput(int pid, Board.Move move)
        {
            Game.Log(Game.LogLevel.TRACE, "% Game.CursorInput %");
            Piece actor = this.GetActor(pid);

            //check if we have an actor and
            //check if it is the right time to move the cursor
            if ( (actor == null) || !actor.IsUsingCursor() ) return false;

            //move the cursor
            return actor.MoveCursor(move);
        }

        public bool MoveInput(int pid)
        {
            Game.Log(Game.LogLevel.TRACE, "% Game.MoveInput %");
            Piece actor = this.GetActor(pid);

            //check if we have an actor and
            //check if it is the right time to move the cursor
            if ((actor == null) || !actor.IsMoving()) return false;

            Hex cursor = actor.GetCursor();
            bool success = this.board.SetPieceAt(actor, cursor.GetLocation());

            //if we succeeded then we need to update the piece as having moved
            if (success)
            {
                actor.SetHavingMoved();
            }

            this.AdvanceGameState();

            return success;
        }

        public bool TargetInput(int pid)
        {
            Game.Log(Game.LogLevel.TRACE, "% Game.TargetInput %");
            Piece actor = this.GetActor(pid);

            //check if we have an actor and
            //check if it is the right time to move the cursor
            if ((actor == null) || !actor.IsTargeting()) return false;

            Hex cursor = actor.GetCursor();

            Piece target = cursor.GetPieceAtHex();

            //check that we have a target then we need to update the piece as having targeted
            bool success = (target != null);
            if (success)
            {
                actor.SetTarget(target);
                actor.SetHavingTargeted();
            }

            this.AdvanceGameState();

            return success;
        }

        public bool Undo(int pid)
        {
            Game.Log(Game.LogLevel.TRACE, "% Game.Undo %");
            Piece piece = this.GetActor(pid);

            //check if we have an actor and
            //check if it is the right time to move the cursor
            if ((piece == null) || !piece.CanUndo()) return false;

            if (piece.IsTargeting() )
            {
                piece.SetResolvedAction(true);
            }
            else if( piece.IsActing() )
            {
                piece.SetHavingMoved(true);
            }
         
            return true;
        }

        public void UpdateWindow()
        {
            Game.Log(Game.LogLevel.TRACE, "% Game.UpdateWindow %");
            view.UpdateWindow(this);
        }

        public void ShowUndo()
        {
            view.ShowUndo(this.players, this.enemy);
        }

        public void HideUndo()
        {
            view.HideUndo();
        }

        public Player GetPlayer(int pid)
        {
            return this.players[pid];
        }
        
        public Enemy GetEnemy()
        {
            return this.enemy;
        }

        public int GetTime()
        {
            return Game.time;
        }

        public static void Log( LogLevel level, string logMessage )
        {
            if (!(level >= Game.loglevel)) return;
            try
            {
                Game.log.WriteLine(Game.fight.ToString("D3")+":"+ Game.time.ToString("D5") + "  " + logMessage);
                Game.log.Flush();
            }
            catch
            {
            }
        }

        public static void CloseLog()
        {
            Game.log.Close();
        }

        /*** game states ***/

        protected void WaitForAllInit()
        {
            Game.Log(Game.LogLevel.TRACE, "% Game.WaitForAllInit %");
            bool initialized = true;
            foreach( Player player in this.players )
            {
                initialized &= player.IsInitialized();
            }

            initialized &= enemy.IsInitialized();

            //check for a transition
            if (initialized)
            {
                Game.time = this.timeline.GetTime();
                Game.Log(Game.LogLevel.DEBUG, "All players and enemys initialized.");

                //find out if the creature attacks first
                Battle.Side side = Battle.FirstStrike(this.enemy);
                Game.Log(Game.LogLevel.NORMAL, Battle.GetSideText(side) + " will attack first.");

                //make first strike side ready
                this.MakeSideReady(side);

                this.ResolveDeath();
                this.AdvanceGameState = WaitForFirstStrike;
            }
        }

        protected void WaitForFirstStrike()
        {
            Game.Log(Game.LogLevel.TRACE, "% Game.WaitForFirstStrike %");
            if( this.timeline.ResolveActions() )
            {
                Game.time = this.timeline.GetTime();
                Game.Log(Game.LogLevel.MINIMAL, "~ Switch sides after first strike ~");

                //make counter strike side ready
                Battle.Side side = Battle.Opposite(this.timeline.GetFirstSide());
                this.MakeSideReady(side);

                this.ResolveDeath();
                this.AdvanceGameState = WaitForCounterStrike;
            }
        }

        protected void WaitForCounterStrike()
        {
            Game.Log(Game.LogLevel.TRACE, "% Game.WaitForCounterStrike %");
            if (this.timeline.HasCountered())
            {
                Game.time = this.timeline.GetTime();
                Game.Log(Game.LogLevel.MINIMAL, "~ Switch sides after counter strikes ~");

                //make first side ready again, so that everyone can act
                Battle.Side side = this.timeline.GetFirstSide();
                this.MakeSideReady(side);

                this.ResolveDeath();
                this.AdvanceGameState = WaitForSide;
            }
        }

        protected void WaitForSide()
        {
            Game.Log(Game.LogLevel.TRACE, "% Game.WaitForSide %");
            if ( this.timeline.ResolveActions() )
            {
                Game.time = this.timeline.GetTime();
                Game.Log(Game.LogLevel.MINIMAL, "~ Switch sides ~");

                this.enemy.SelectTarget(this.players, Game.time);

                this.ResolveDeath();
            }
        }

        protected void Defeat()
        {
            Game.Log(Game.LogLevel.MINIMAL, "Players have lost the game.");
        }

        /*** helpers ***/

        protected void MakeSideReady(Battle.Side side)
        {
            Game.Log(Game.LogLevel.TRACE, "% Game.MakeSideReady %");
            //this depends on the side that is going first
            foreach (Player player in this.players)
            {
                if( side == Battle.Side.PLAYERS)
                {
                    //get the player ready to act as if they had just finished an action
                    player.SetResolvedAction(true);
                }
                else
                {
                    //set the players as if they had just acted
                    player.SetTookAction(true);
                }
            }//foreach

            if (side == Battle.Side.ENEMY)
            {
                //get the enemy ready to act as if it had just finished an action
                this.enemy.SetResolvedAction(true);
                this.enemy.SelectTarget(this.players, 0);
            }
            else
            {
                //set the enemy as if it had just acted
                enemy.SetTookAction(true);
            }
        }

        protected bool PlayActionCard( Piece piece, Card card)
        {
            Game.Log(Game.LogLevel.TRACE, "% Game.PlayActionCard %");
            CardEffect effect = null;
            bool success = false;

            //create a skill
            if ( card is Skill)
            {
                effect =  new SkillEffect( (Skill)card, piece, piece.GetTarget(),this.enemy);
            }

            if (card is Glamour)
            {
                effect = new GlamourEffect((Glamour)card, piece, piece.GetTarget(),this.enemy);
            }

            success = this.timeline.AddEffect(effect);

            if( success )
            {
                //update the piece as having moved and equiped
                //this also sets the state to busy
                piece.SetTookAction();
                Game.Log(Game.LogLevel.MINIMAL, effect.ToString());
            }

            return success;
        }

        protected void ResolveDeath()
        {
            Game.Log(Game.LogLevel.TRACE, "% Game.UpdateState %");
            //check for all players death
            bool allDead = true;
            foreach( Player player in this.players )
            {
                allDead &= player.IsDead();
            }
            if( allDead )
            {
                this.AdvanceGameState = Defeat;
                this.defeat = true; // <= NOOOOO, WE HAVE LOST!!!!
                return;
            }

            //check for battle victory
            if( this.enemy.IsDead() )
            {
                Game.Log(Game.LogLevel.MINIMAL, "The players have defeated the enemy.");

                //get new characters
                for ( int i = 0; i < players.Length; i++)
                {
                    if (this.players[i].IsDead())
                    {
                        this.ClearDeadPiece(this.players[i]);
                        this.players[i] = new Player(i +1);
                        this.board.SetPieceRandomly(this.players[i]);
                        this.players[i].SetTarget(this.enemy);
                        Game.Log(Game.LogLevel.MINIMAL, this.players[i].ToString() + " died and a new player is recruited to the battle field.");
                    }//if player dead
                    else
                    {
                        //add some experience and check for new level
                        this.players[i].AddExperience(this.enemy);
                        Game.Log(Game.LogLevel.NORMAL, this.players[i].ToString() + " has gained " + this.players[i].GetExperience() + " experience.");
                        if( this.players[i].CheckForNewLevel() )
                        {
                            Game.Log(Game.LogLevel.MINIMAL, this.players[i].ToString() + " has advanced to level " + this.players[i].GetBaseLevel() + ".");
                        }

                        //clear the last action time for the players
                        players[i].SetLastActionTime(0);

                        //set the player states to waiting
                        this.players[i].SetHavingDefeatedEnemy();
                    }//else player lived
                }//for

                //get a new enemy
                this.ClearDeadPiece(this.enemy);
                this.enemy = new Enemy();
                this.board.SetPieceRandomly(this.enemy);

                //set the targets of the players
                for (int i = 0; i < players.Length; i++)
                {
                    this.players[i].SetTarget(this.enemy);
                }//for targets

                //reset what is needed for a new battle
                this.RemoveAllGlamours();
                this.timeline = new Timeline(players,enemy);
                Game.time = this.timeline.GetTime();
                Game.fight++;
                Game.Log(Game.LogLevel.MINIMAL, "* New Fight *");

                //wait for enemy/characters to be initialized
                this.AdvanceGameState = WaitForAllInit;
                return;
            }// if enemy is dead
        }

        protected void RemoveAllGlamours()
        {
            Game.Log(Game.LogLevel.TRACE, "% Game.RemoveAllGlamours %");
            foreach (Player player in this.players)
            {
                player.RemoveGlamours();
            }
        }

        protected Piece GetActor(int pid)
        {
            Game.Log(Game.LogLevel.TRACE, "% Game.GetActor %");
            //pid -1 = enemy, the players are 0-3
            if (pid == -1)
            {
                return this.enemy;
            }
            else if ((pid >= 0) && (pid <= 3))
            {
                return this.players[pid];
            }
            else
            {
                return null;
            }
        }

        protected void ClearDeadPiece(Piece piece)
        {
            Game.Log(Game.LogLevel.TRACE, "% Game.ClearDeadPiece %");
            //clear the enemy from the players targets
            foreach (Player player in this.players)
            {
                if (player.GetTarget() == piece)
                    player.SetTarget(null);
            }

            //clear the piece from the board
            this.board.ClearPieceFromBoard(piece);
        }

        //todo put this back and delete the other one
        protected void InitializePieces()
        {
            int[] starting = new int[] { 14, 17, 18, 16 };
            for (int i = 0; i < players.Length; i++)
            {
                board.SetPieceAt(players[i], starting[i]);
                players[i].SetTarget(this.enemy);
            }
            board.SetPieceAt(enemy, 5);
        }

        //protected void InitializePieces()
        //{
        //    int[] starting = new int[] { 10, 19, 1, 16 };
        //    for (int i = 0; i < players.Length; i++)
        //    {
        //        board.SetPieceAt(players[i], starting[i]);
        //        players[i].SetTarget(this.enemy);
        //    }
        //    board.SetPieceAt(enemy, 5);

        //    Player player;
        //    //initialize player 1
        //    player = this.players[0];
        //    player.ApplyCard(Card.Factory("1145"));//betty
        //    player.ApplyCard(Card.Factory("0611"));//warrior

        //    //initialize player 2
        //    player = this.players[1];
        //    player.ApplyCard(Card.Factory("1092"));//roland
        //    player.ApplyCard(Card.Factory("0aac"));//healer
        //    player.SetTarget(this.players[0]);

        //    //initialize player 3
        //    player = this.players[2];
        //    player.ApplyCard(Card.Factory("1007"));//marg
        //    player.ApplyCard(Card.Factory("00d0"));//armsman

        //    //initialize player 4
        //    player = this.players[3];
        //    player.ApplyCard(Card.Factory("10a2"));//eunice
        //    player.ApplyCard(Card.Factory("08c2"));//mage

        //    //initialze the enemy
        //    this.enemy.ApplyCard(Card.Factory("210c"));

        //    //call state check
        //    this.AdvanceGameState();

        //    //move and target players
        //    this.MoveInput(0);
        //    this.TargetInput(0);

        //    this.MoveInput(1);
        //    this.TargetInput(1);

        //    this.MoveInput(2);
        //    this.TargetInput(2);

        //    this.MoveInput(3);
        //    this.TargetInput(3);

        //}


    }
}
