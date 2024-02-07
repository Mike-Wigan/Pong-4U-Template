/*
 * Description:     A basic PONG simulator
 * Author:           
 * Date:            
 */

#region libraries

using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

#endregion

namespace Pong
{
    public partial class Form1 : Form
    {
        #region global values

        //graphics objects for drawing
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        SolidBrush redBrush = new SolidBrush(Color.Red);
        Font drawFont = new Font("Courier New", 10);

        // Sounds for game
        SoundPlayer scoreSound = new SoundPlayer(Properties.Resources.score);
        SoundPlayer collisionSound = new SoundPlayer(Properties.Resources.collision);

        //determines whether a key is being pressed or not
        Boolean wKeyDown, sKeyDown, upKeyDown, downKeyDown;

        // check to see if a new game can be started
        Boolean newGameOk = true;

        //ball values
        Boolean ballMoveRight = true;
        Boolean ballMoveDown = true;
        double BALL_SPEED = 3;
        const int BALL_WIDTH = 20;
        const int BALL_HEIGHT = 20;
        int random;
        Rectangle ball;

        //player values
        const int PADDLE_SPEED = 3;
        const int PADDLE_EDGE = 20;  // buffer distance between screen edge and paddle            
        const int PADDLE_WIDTH = 10;
        const int PADDLE_HEIGHT = 40;
        Rectangle player1, player2, player1front, player2front;

        //player and game scores
        int player1Score = 0;
        int player2Score = 0;
        int gameWinScore = 3;  // number of points needed to win game

        //random generator
        Random rand = new Random();

        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //check to see if a key is pressed and set is KeyDown value to true if it has
            switch (e.KeyCode)
            {
                case Keys.W:
                    wKeyDown = true;
                    break;
                case Keys.S:
                    sKeyDown = true;
                    break;
                case Keys.Up:
                    upKeyDown = true;
                    break;
                case Keys.Down:
                    downKeyDown = true;
                    break;
                case Keys.Y:
                case Keys.Space:
                    if (newGameOk)
                    {
                        SetParameters();
                    }
                    break;
                case Keys.Escape:
                    if (newGameOk)
                    {
                        Close();
                    }
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //check to see if a key has been released and set its KeyDown value to false if it has
            switch (e.KeyCode)
            {
                case Keys.W:
                    wKeyDown = false;
                    break;
                case Keys.S:
                    sKeyDown = false;
                    break;
                case Keys.Up:
                    upKeyDown = false;
                    break;
                case Keys.Down:
                    downKeyDown = false;
                    break;
            }
        }

        /// <summary>
        /// sets the ball and paddle positions for game start
        /// </summary>
        private void SetParameters()
        {
            if (newGameOk)
            {
                BALL_SPEED = 3;
                player1Score = player2Score = 0;
                newGameOk = false;
                startLabel.Visible = false;
                gameUpdateLoop.Start();
            }

            //player start positions
            player1 = new Rectangle(PADDLE_EDGE, this.Height / 2 - PADDLE_HEIGHT / 2, PADDLE_WIDTH, PADDLE_HEIGHT);
            player1front = new Rectangle(PADDLE_EDGE + PADDLE_WIDTH, this.Height / 2 - PADDLE_HEIGHT / 2, 0, PADDLE_HEIGHT);
            player2 = new Rectangle(this.Width - PADDLE_EDGE - PADDLE_WIDTH, this.Height / 2 - PADDLE_HEIGHT / 2, PADDLE_WIDTH, PADDLE_HEIGHT);
            player2front = new Rectangle(this.Width - PADDLE_EDGE - PADDLE_WIDTH - 2, this.Height / 2 - PADDLE_HEIGHT / 2, 0, PADDLE_HEIGHT);

            //ball start position
            ball = new Rectangle(this.Width / 2 - 12, this.Height / 2 - 11, BALL_WIDTH, BALL_HEIGHT);
        }
        /// <summary>
        /// This method is the game engine loop that updates the position of all elements
        /// and checks for collisions.
        /// </summary>
        private void gameUpdateLoop_Tick(object sender, EventArgs e)
        {
            #region update ball position

            player1ScoreLabel.Text = Convert.ToString(player1Score);
            player2ScoreLabel.Text = Convert.ToString(player2Score);

            if (ballMoveRight == true)
            {
                ball.X += Convert.ToInt32(BALL_SPEED);
            }
            else
            {
                ball.X -= Convert.ToInt32(BALL_SPEED);
            }
            if (ballMoveDown == true)
            {
                ball.Y += Convert.ToInt32(BALL_SPEED);
            }
            else
            {
                ball.Y -= Convert.ToInt32(BALL_SPEED);
            }
            #endregion

            #region update paddle positions

            if (wKeyDown == true && player1.Y > 0)
            {
                player1.Y -= PADDLE_SPEED;
                player1front.Y -= PADDLE_SPEED;
            }
            if (sKeyDown == true && player1.Y < this.Height - PADDLE_HEIGHT)
            {
                player1.Y += PADDLE_SPEED;
                player1front.Y += PADDLE_SPEED;
            }
            if (upKeyDown == true && player2.Y > 0)
            {
                player2.Y -= PADDLE_SPEED;
                player2front.Y -= PADDLE_SPEED;
            }
            if (downKeyDown == true && player2.Y < this.Height - PADDLE_HEIGHT)
            {
                player2.Y += PADDLE_SPEED;
                player2front.Y += PADDLE_SPEED;
            }
            #endregion

            #region ball collision with top and bottom lines

            if (ball.Y < 0) // if ball hits top line
            {
                ballMoveDown = true;
            }
            else if (ball.Y > this.Height - BALL_HEIGHT)
            {
                ballMoveDown = false;
            }
            if (ball.IntersectsWith(player1front) || ball.IntersectsWith(player2front))
            {
                collisionSound.Play();
            }

            #endregion

            #region ball collision with paddles

            if (ball.IntersectsWith(player1front))
            {
                ball.X = player1front.X + 1;
                collisionSound.Play();
                ballMoveRight = true;
                BALL_SPEED += 0.1;
            }
            else if (ball.IntersectsWith(player2front))
            {
                ball.X = player2front.X - 1 - BALL_WIDTH;
                collisionSound.Play();
                ballMoveRight = false;
                BALL_SPEED += 0.1;
            }

            #endregion

            #region ball collision with side walls (point scored)

            //rand = new Random();
            //random = rand.Next(1, 1);
            if (ball.X < 0)  // ball hits left wall logic
            {
                BALL_SPEED = 3;
                scoreSound.Play();
                player2Score++;
                player2ScoreLabel.Text = Convert.ToString(player2Score);
                //if (random == 1)
                //{
                //    player1 = new Rectangle(this.Width - PADDLE_EDGE - PADDLE_WIDTH, this.Height / 2 - PADDLE_HEIGHT / 2, PADDLE_WIDTH, PADDLE_HEIGHT);
                //    player2 = new Rectangle(PADDLE_EDGE, this.Height / 2 - PADDLE_HEIGHT / 2, PADDLE_WIDTH, PADDLE_HEIGHT);
                //}
                if (player2Score == gameWinScore)
                {
                    GameOver("player 2");
                }
                else
                {
                    ballMoveRight = true;
                    SetParameters();
                }
            }
            else if (ball.X > this.Width)  // ball hits right wall logic
            {
                BALL_SPEED = 3;
                scoreSound.Play();
                player1Score++;
                player1ScoreLabel.Text = Convert.ToString(player1Score);
                if (player1Score == gameWinScore)
                {
                    GameOver("player 1");
                }
                else
                {
                    ballMoveRight = false;
                    SetParameters();
                }
            }

            #endregion

            //refresh the screen, which causes the Form1_Paint method to run
            this.Refresh();
        }

        /// <summary>
        /// Displays a message for the winner when the game is over and allows the user to either select
        /// to play again or end the program
        /// </summary>
        /// <param name="winner">The player name to be shown as the winner</param>
        private void GameOver(string winner)
        {
            gameUpdateLoop.Enabled = false;
            startLabel.Text = $"{winner} Wins \n To Play Again Press Space to Exit Press ESC";
            startLabel.Visible = true;
            newGameOk = true;

            // TODO create game over logic
            // --- stop the gameUpdateLoop
            // --- show a message on the startLabel to indicate a winner, (may need to Refresh).
            // --- use the startLabel to ask the user if they want to play again
            Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // TODO draw player2 using FillRectangle
            e.Graphics.FillRectangle(whiteBrush, player1);
            e.Graphics.FillRectangle(whiteBrush, player2);
            e.Graphics.FillRectangle(redBrush, player2front);
            e.Graphics.FillRectangle(redBrush, player1front);

            // TODO draw ball using FillRectangle
            e.Graphics.FillRectangle(whiteBrush, ball);
        }

    }
}
