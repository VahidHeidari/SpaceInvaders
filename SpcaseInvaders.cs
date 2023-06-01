// please don't delete my name
//+++++++++++++++++++++++++++++++++++++++
//+                                     +
//+                                     +
//+     S P A C E    I N V A D E R S    +
//+              G A M E                +
//+          by vahid heydari           +
//+                                     +
//+           september 2011            +
//+     http://tjs87.wordpress.com      +
//+                                     +
//+                                     +
//+++++++++++++++++++++++++++++++++++++++
using System;
using System.Drawing;
using GamesObjects;

namespace SpaceInvaders_1
{
    class SpaceInvaders
    {
        #region Members
        // Game Graphics Members
        Bitmap spaceInvaders;                   // sprites image
        Bitmap buffer;                          // Back buffer
        Graphics frm_graph;                     // Form Graghics
        Graphics g;                             // Buffer Graphics
        const int G_Width = 500, G_Height = 400;// Game dimensions
        System.Windows.Forms.Form client;       // Refrence to client form

        // sprites & tiles
        // define your sprites Table here
        Sprite squid, octopus, crab, logo, myName, cannon, laserBeam, bunker, Explosion, die, Saucer;
        Sprite[] EnemiesLaser;
        Tile tile;                              //moon desert

        const int NumOfRows = 5, NumOfCols = 11, NumOfLaser = 3;
        int[] InvadersRow = new int[NumOfRows];
        int offset = 10, rowXLeft, rowXRight, rowYUp, rowYDown, rowXSpeed, rowYSpeed, hits;
        enum RowMoves { Left, Right, Down };
        RowMoves rowMove, cannonMoves;

        bool fireing, Exploded, saucer, paused;

        enum GameStates
        {
            splashScreen,
            Running,
            Paused,
            GameOver,
            Demo
        };
        [Flags]
        enum GameKeys : int
        {
            None = 0x00000000,
            Left = 0x00000001,
            Right = 0x00000002,
            Fire = 0x00000004,
            Escape = 0x00000008,
            Pause = 0x00000010,
        };

        GameStates gameState;

        // Keyboard buffer
        GameKeys lastKeypressed;

        // Frames Per Second
        // This controles Refresh Rate
        // minimum = 2
        const int fps = 16;
        long frames;
        long screenshot_num;
        // just for debug purposes
        //float fpsElapsed;
        private System.Windows.Forms.Timer mainTimer;
        // Just for debug purpuses
        //private System.Windows.Forms.Timer FraneCtr;

        int life, level, score;
        struct Location
        {
            public int X, Y, Delay, Frame;

            public Location(int x, int y, int delay)
            {
                X = x;
                Y = y;
                Delay = delay;
                Frame = 0;
            }
        }
        System.Collections.Generic.List<Location> ExplosionList = new System.Collections.Generic.List<Location>();
        System.Collections.Generic.List<Location> BunkerList = new System.Collections.Generic.List<Location>();
        string PlayerName = "Player";
        const int MaxChars = 6;
        int EnterdChars = 0;
        int[] DemoMoves;
        int moves;

        //System.Threading.Thread SoundThread;
        //System.Media.SoundPlayer SndExplosion = new System.Media.SoundPlayer(@"resources\beep6.wav");
        //System.Media.SoundPlayer SndSauser = new System.Media.SoundPlayer(@"resources\8.wav");

        // Create Font
        Font fontSB = new Font("arial", 10, FontStyle.Bold);
        Font fontSp = new Font("Arial Black", 10);
        Font fontBig = new Font("arial", 30);
        Font font = new Font("Arial", 15);
        #endregion

        #region Methodes
        public void init_game(System.Windows.Forms.Form form)
        {
            client = form;
            client.ClientSize = new Size(G_Width, G_Height);
            // Create Graphic Buffer
            buffer = new Bitmap(G_Width, G_Height);
            g = Graphics.FromImage(buffer);
            frm_graph = client.CreateGraphics();
            // Game Variables
            level = 1;
            hits = 0;
            life = 3;
            score = 0;

            // Loade spirets Image
            spaceInvaders = new Bitmap(System.Windows.Forms.Application.StartupPath + @"\resources\SI2562.png");

            // Initialize spirites & Tiles
            //
            // Logo    X:59  Y:0  Width:196 Heigth:89
            // Desert X:128 Y:123 Width:127 Height:132
            //
            // Cannon1 X:0   Y:171 Width:31  Height:17
            // Cannon2 X:32  Y:171 Width:31  Height:17
            // Die     X:64  Y:178 Width:29  Height:13
            //
            // Bunker  X:0   Y:0   Width:48  Height:36
            //
            // Saucer X:94  Y:186 Width:34 Height:14
            // 
            // Explosion X:0 Y:151 Width:24 Height:19
            //
            // UFOs    X:48  Y:89  Width:16  Height:16
            //
            // Laser   X:50  Y:130 Width:3   Height:12
            //
            logo = new Sprite(spaceInvaders, 59, 0, 197, 89, 0, 1);
            myName = new Sprite(spaceInvaders, 181, 94, 76, 20, 0, 1);

            bunker = new Sprite(spaceInvaders, 0, 0, 48, 36, -1, 4);

            Explosion = new Sprite(spaceInvaders, 0, 151, 24, 19, 2, 2);

            Saucer = new Sprite(spaceInvaders, 94, 186, 34, 14, -1, 4);
            Saucer.X = G_Width;
            Saucer.Y = 16;
            RandomSaucer();
            saucer = false;

            squid = new Sprite(spaceInvaders, 48, 89, 16, 16, -1, 2);
            squid.Delay = fps / 5;

            crab = new Sprite(spaceInvaders, 64, 89, 22, 16, -1, 2);
            crab.Delay = fps / 5;

            octopus = new Sprite(spaceInvaders, 86, 89, 24, 16, -1, 2);
            octopus.Delay = fps / 5;

            laserBeam = new Sprite(spaceInvaders, 50, 130, 3, 12, +1, 1);
            fireing = false;
            Exploded = false;
            EnemiesLaser = new Sprite[NumOfLaser];
            for (int i = 0; i < EnemiesLaser.Length; ++i)
            {
                EnemiesLaser[i] = new Sprite(spaceInvaders, 53, 130, 3, 12, 0, 1);
                EnemiesLaser[i].Y = G_Height + EnemiesLaser[i].Height;
            }

            die = new Sprite(spaceInvaders, 64, 178 + 3 * 13, 29, 13, -1, 3);
            cannon = new Sprite(spaceInvaders, 32, 171, 31, 17, -1, 4);
            cannon.Delay = fps / 5;
            cannon.X = G_Width / 2 - cannon.Width / 2;
            cannon.Y = G_Height - 50;

            BunkerList.Add(new Location((G_Width / 4) - (bunker.Width / 2), G_Height - bunker.Height - (G_Height - cannon.Y) - 10, 3));
            BunkerList.Add(new Location((G_Width / 2) - (bunker.Width / 2), G_Height - bunker.Height - (G_Height - cannon.Y) - 10, 3));
            BunkerList.Add(new Location((3 * G_Width / 4) - (bunker.Width / 2), G_Height - bunker.Height - (G_Height - cannon.Y) - 10, 3));

            tile = new Tile(spaceInvaders, 128, 123, 127, 132);

            // Initialize Row of Invaders
            int tmp = 0;
            for (int i = 0; i < NumOfCols; ++i) tmp |= 0x01 << i;// tmp = 0x07FF
            for (int i = 0; i < InvadersRow.Length; ++i)
                InvadersRow[i] = tmp;// 5 row and 11 col
            rowMove = RowMoves.Right;
            rowYUp = 30;
            rowYDown = rowYUp + (squid.Height + offset) * NumOfRows - offset;
            rowXLeft = +50;
            rowXRight = rowXLeft + (octopus.Width + offset) * NumOfCols - offset;
            rowXSpeed = 1;
            rowYSpeed = 5;

            mainTimer = new System.Windows.Forms.Timer();
            // just for debug
            //FraneCtr = new System.Windows.Forms.Timer();
            this.mainTimer.Tick += new EventHandler(this.mainTimer_Tick);
            client.KeyDown += new System.Windows.Forms.KeyEventHandler(this.clientKeyDown);
            client.KeyUp += new System.Windows.Forms.KeyEventHandler(this.clientKeyUp);
            // Initialize Game speed & Start Game Timer
            frames = 0;
            mainTimer.Interval = 1000 / fps;
            mainTimer.Start();

            // Average of Frames in two secondes
            // just for debug
            //FraneCtr.Interval = 2000;   // Two second
            //FraneCtr.Start();
            //this.FraneCtr.Tick += new EventHandler(this.FraneCtr_Tick);
            DemoMoves = new int[30];

            Random r = new Random();
            for (int i = 0; i < DemoMoves.Length; ++i)
            {
                DemoMoves[i] = r.Next(G_Width - cannon.Width);
                //if ((i % 2) == 0) DemoMoves[i] = 0;
                //else DemoMoves[i] = G_Width - cannon.Width;
            }
                
            if (DemoMoves[0] > cannon.X) cannonMoves = RowMoves.Right;
            else cannonMoves = RowMoves.Left;
            gameState = GameStates.splashScreen;
        }

        void splashScreen()
        {
            g.Clear(Color.Black);//WhiteBlue

            // Diplay space Invaders Logo evry 500ms
            if (((frames / (fps / 2)) % 2) == 0)
            {
                logo.Render((G_Width / 2) - (logo.Width / 2), 20, g);
                myName.Render((G_Width / 2) - (myName.Width / 2), 30 + logo.Height, g);
                g.DrawString("Press Any Key To Start", fontSp, Brushes.Yellow, (G_Width / 2) - 90, 60 + logo.Height);
                g.DrawString("HighScore : " + ScoreRecorder.HighScores.HighScore(), fontSp, Brushes.Yellow, (G_Width / 2) - 70, 80 + logo.Height);
            }
            // Display < Help >
            g.DrawString("Space : Fire\nLeft & Right Arrow Keys : Move\nP : Pause\\Play\nEsc : Exit", fontSp, Brushes.White, 20, G_Height - tile.Height - 4 * 20);
            tile.Render(-(tile.Width / 2), G_Height - tile.Height, G_Width / tile.Width + 2, g);
            // Start Demo evry 10 secondes
            if (frames / fps == 10)
            {
                frames = 0;
                level = 0;
                life = 0;
                score = 0;
                NextLevel();
                gameState = GameStates.Demo;
            }
        }
        void GameOverScreen()
        {
            g.Clear(Color.Blue);
            g.DrawString("Game Over", fontBig, Brushes.White, G_Width / 2 - 100, 100);
            g.DrawString("Score : " + score * 10 + "\nLevel : " + level + "\nEnter Your Name : " + PlayerName + "_", font, Brushes.White, G_Width / 2 - 100, 200);
        }
        void Paused()
        {
            GetInput();
            DrawAll();
            // Diplay "-- Paused --" evry 500ms
            if ((frames / (fps / 2) % 2) == 1)
                g.DrawString("-- Paused --", font, Brushes.White, G_Width / 2 - 50, 15);
        }
        void Running()
        {
            GetInput();
            LaserMove();
            SaucerMove();
            RowMove();
            Shot();
            DrawAll();
        }
        void Demo()
        {
            GetInput();
            CannonMove();
            LaserMove();
            SaucerMove();
            RowMove();
            Shot();
            DrawAll();
            // Display "-- Demo --" evry 500ms
            if ((frames / (fps / 2) % 2) == 1)
                g.DrawString("-- Demo --", font, Brushes.White, G_Width / 2 - 50, 15);
            if (gameState == GameStates.GameOver /*|| frames / fps == 20*/)
            {
                frames = 0;
                gameState = GameStates.splashScreen;
            }
            //SaveScreenshot();     // Uncomment to capture demo frames.
        }

        void SaveScreenshot()
        {
            var base_dir = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "Screenshots");
            if (!System.IO.Directory.Exists(base_dir))
                System.IO.Directory.CreateDirectory(base_dir);
            var img_name = String.Format("{0}.png", screenshot_num);
            var out_path = System.IO.Path.Combine(base_dir, img_name);
            buffer.Save(out_path);
            ++screenshot_num;
        }
        
        void DrawScene()
        {
            switch (gameState)
            {
                case GameStates.splashScreen:
                    splashScreen();
                    break;

                case GameStates.Running:
                    Running();
                    break;

                case GameStates.Paused:
                    Paused();
                    break;

                case GameStates.GameOver:
                    GameOverScreen();
                    break;

                case GameStates.Demo:
                    Demo();
                    break;
            }

            // Middle of image
            // just for debuging
            //g.DrawLine(Pens.Yellow, G_Width / 2, 0, G_Width / 2, G_Height);

            // Draw buffer on client
            frm_graph.DrawImage(buffer, 0, 0);
            frames++;

            // Sleeps for 1ms and wating
            System.Threading.Thread.Sleep(1);
        }
        void DrawStatusBar()
        {
            g.DrawString("Score : " + score * 10, fontSB, Brushes.White, 20, 0);
            g.DrawString("Lives : " + life, fontSB, Brushes.White, G_Width / 2 - 20, 0);
            g.DrawString("Level : " + level, fontSB, Brushes.White, G_Width - 80, 0);
            g.DrawLine(Pens.White, 20, 15, G_Width - 20, 15);

            // Draw frames
            // just for debug
            //g.DrawString(fpsElapsed + "fps", font, Brushes.Green, 0, 15);
        }
        void DrawAll()
        {
            g.Clear(Color.Black);

            RowOfInvadersRender();
            for (int i = 0; i < NumOfLaser; ++i)
                EnemiesLaser[i].Render(g);
            if (!Exploded)
            {
                cannon.Render(g);
                cannon.NextFrame();
            }
            else
            {
                die.Delay--;
                if (die.Delay <= 0)
                {
                    die.Delay = fps / 5;
                    die.SetFrame(die.Frame + 1);
                    if (die.Frame == 2)
                        Exploded = false;
                }
                die.Render(g);
            }
            if (saucer)
            {
                Saucer.Render(g);
                Saucer.NextFrame();
            }
            if (fireing) laserBeam.Render(g);
            DrawStatusBar();
        }
        void RowOfInvadersRender()
        {
            for (int j = 0; j < NumOfCols; ++j)
                if ((InvadersRow[0] & (0x01 << j)) != 0)
                    squid.Render(rowXLeft + j * (octopus.Width + offset), rowYUp + 0 * (squid.Height + offset), g);

            for (int i = 1; i < 3; ++i)
            {
                for (int j = 0; j < NumOfCols; ++j)
                    if ((InvadersRow[i] & (0x01 << j)) != 0)
                        crab.Render(rowXLeft + j * (octopus.Width + offset), rowYUp + i * (squid.Height + offset), g);
            }

            for (int i = 3; i < 5; ++i)
            {
                for (int j = 0; j < NumOfCols; ++j)
                    if ((InvadersRow[i] & (0x01 << j)) != 0)
                        octopus.Render(rowXLeft + j * (octopus.Width + offset), rowYUp + i * (squid.Height + offset), g);
            }
            squid.NextFrame();
            crab.NextFrame();
            octopus.NextFrame();
            if (ExplosionList.Count != 0)
            {
                Location tmp;
                for (int i = 0; i < ExplosionList.Count; ++i)
                {
                    tmp = ExplosionList[i];
                    if (--tmp.Delay >= 0)
                    {
                        Explosion.SetFrame(tmp.Frame);
                        Explosion.Render(tmp.X, tmp.Y, g);
                        ExplosionList[i] = tmp;
                    }
                    else
                    {
                        if (tmp.Frame == 0)
                        {
                            tmp.Frame++;
                            tmp.Delay = 1;
                            ExplosionList[i] = tmp;
                        }
                        else
                        {
                            ExplosionList.Remove(tmp);
                        }
                    }
                }
            }
            if (BunkerList.Count != 0)
            {
                Location tmp;
                for (int i = 0; i < BunkerList.Count; ++i)
                {
                    tmp = BunkerList[i];
                    if (tmp.Frame < 4)
                    {
                        bunker.SetFrame(tmp.Frame);
                        bunker.Render(tmp.X, tmp.Y, g);
                    }
                }
            }
        }
        
        void CannonMove()
        {
            if (!fireing)
            {
                if (!((cannon.X + laserBeam.Width + cannon.Width / 2 > BunkerList[0].X && cannon.X + cannon.Width / 2 < BunkerList[0].X + bunker.Width) ||
                    (cannon.X + laserBeam.Width + cannon.Width / 2 > BunkerList[1].X && cannon.X + cannon.Width / 2 < BunkerList[1].X + bunker.Width) ||
                    (cannon.X + laserBeam.Width + cannon.Width / 2 > BunkerList[2].X && cannon.X + cannon.Width / 2 < BunkerList[2].X + bunker.Width )))
                {
                    laserBeam.X = cannon.X + cannon.Width / 2;
                    laserBeam.Y = cannon.Y;
                    fireing = true;
                }
            }
            if (cannonMoves == RowMoves.Right)
            {
                if (cannon.X < DemoMoves[moves])
                {
                    if (!((/*EnemiesLaser[0].Y > BunkerList[0].Y &&*/ EnemiesLaser[0].X > cannon.X + 10 && EnemiesLaser[0].X < cannon.X + cannon.Width + 10 && EnemiesLaser[0].Y < G_Height-10)
                        || (/*EnemiesLaser[1].Y > BunkerList[0].Y &&*/ EnemiesLaser[1].X > cannon.X + 10 && EnemiesLaser[1].X < cannon.X + cannon.Width + 10 && EnemiesLaser[1].Y < G_Height-10)
                        || (/*EnemiesLaser[2].Y > BunkerList[0].Y &&*/ EnemiesLaser[2].X > cannon.X + 10 && EnemiesLaser[2].X < cannon.X + cannon.Width + 10 && EnemiesLaser[2].Y < G_Height-10)))
                        cannon.X += 5;
                }
                else
                {
                    cannonMoves = RowMoves.Left;
                    moves++;
                    moves %= DemoMoves.Length;
                }
            }
            else
            {

                if (cannon.X > DemoMoves[moves])
                {
                    if (!((/*EnemiesLaser[0].Y > BunkerList[0].Y &&*/ EnemiesLaser[0].X > cannon.X - 10 && EnemiesLaser[0].X < cannon.X + cannon.Width - 10 && EnemiesLaser[0].Y < G_Height - 10)
                        || (/*EnemiesLaser[1].Y > BunkerList[0].Y &&*/ EnemiesLaser[1].X > cannon.X - 10 && EnemiesLaser[1].X < cannon.X + cannon.Width - 10 && EnemiesLaser[0].Y < G_Height - 10)
                        || (/*EnemiesLaser[2].Y > BunkerList[0].Y &&*/ EnemiesLaser[2].X > cannon.X - 10 && EnemiesLaser[2].X < cannon.X + cannon.Width - 10 && EnemiesLaser[0].Y < G_Height - 10)))
                        cannon.X -= 5;
                }
                else
                {
                    cannonMoves = RowMoves.Right;
                    moves++;
                    moves %= DemoMoves.Length;
                }
            }
        }
        void GetInput()
        {
            if ((lastKeypressed & GameKeys.Escape) == GameKeys.Escape)
                System.Windows.Forms.Application.Exit();
            if (paused)
            {
                if ((lastKeypressed & GameKeys.Pause) == GameKeys.Pause)
                {
                    if (gameState == GameStates.Running)
                        gameState = GameStates.Paused;
                    else if (gameState == GameStates.Paused)
                        gameState = GameStates.Running;
                    lastKeypressed &= ~GameKeys.Pause;
                }
                paused = false;
                lastKeypressed &= ~GameKeys.Pause;
            }
            if (!Exploded && gameState != GameStates.Paused && gameState != GameStates.Demo)
            {
                if ((lastKeypressed & GameKeys.Left) == GameKeys.Left &&
                   cannon.X > 1)
                    cannon.X -= 5;

                if ((lastKeypressed & GameKeys.Right) == GameKeys.Right &&
                    cannon.X + cannon.Width < G_Width - 5)
                    cannon.X += 5;

                if ((lastKeypressed & GameKeys.Fire) == GameKeys.Fire &&
                    !fireing)
                {
                    laserBeam.X = cannon.X + cannon.Width / 2;
                    laserBeam.Y = cannon.Y;
                    fireing = true;
                }
            }
        }
        void LaserMove()
        {
            if (fireing)
            {
                if (laserBeam.Y + laserBeam.Height > 0)
                {
                    laserBeam.Y -= 10;

                    if (laserBeam.Y > G_Height - bunker.Height - (G_Height - cannon.Y) - 10)
                    {
                        int nearX = ((laserBeam.X - (bunker.Width / 2)) / (G_Width / 4));
                        int nearXPixel = ((nearX + 1) * G_Width / 4) - (bunker.Width / 2);
                        int nearYPixel = G_Height - bunker.Height - (G_Height - cannon.Y) - 10;

                        if (laserBeam.Collision(nearXPixel, nearYPixel, bunker.Width, bunker.Height) && nearX < 3)
                        {
                            Location tmp = BunkerList[nearX];
                            if (tmp.Frame < 4)
                            {
                                if (tmp.Delay < 0)
                                {
                                    tmp.Frame++;
                                    tmp.Delay = 3;
                                }
                                else
                                    tmp.Delay--;
                                fireing = false;
                                BunkerList[nearX] = tmp;
                            }
                        }
                    }
                    if (laserBeam.Y < rowYDown)
                    {
                        if (laserBeam.X > rowXLeft && laserBeam.X + laserBeam.Width < rowXRight && laserBeam.Y > rowYUp && laserBeam.Y < rowYDown)
                        {
                            int nearY = (laserBeam.Y - rowYUp) / (squid.Height + offset);
                            int nearX = (laserBeam.X - rowXLeft) / (octopus.Width + offset);
                            int nearXPixel = rowXLeft + (nearX) * (octopus.Width + offset);
                            int nearYPixel = rowYUp + (nearY) * (squid.Width + offset);
                            int nearWidth = (nearY == 0) ? squid.Width : (nearY == 1 || nearY == 2) ? crab.Width : octopus.Width;

                            if (laserBeam.Collision(nearXPixel, nearYPixel, nearWidth, squid.Height) && (InvadersRow[nearY] & (0x01 << nearX)) != 0)
                            {
                                //SoundThread = new System.Threading.Thread(SndExplosion.Play);
                                //SoundThread.Start();
                                InvadersRow[nearY] &= ~(0x01 << nearX);
                                score++;
                                ExplosionList.Add(new Location(nearXPixel, nearYPixel, 1));
                                if (AllHit()) NextLevel();
                                if (++hits == 10)
                                {
                                    rowXSpeed++;
                                    hits = 0;
                                }
                                fireing = false;
                            }
                        }
                    }
                }
                else
                {
                    laserBeam.Y = G_Height;
                    fireing = false;
                }
                if (saucer)
                {
                    if (laserBeam.Y < 15 + Saucer.Height)
                        if (laserBeam.Collision(Saucer))
                        {
                            fireing = false;
                            score += 30;
                            ExplosionList.Add(new Location(Saucer.X - 10, Saucer.Y, 10));
                            saucer = false;
                            RandomSaucer();
                        }
                }
            }
        }
        void RowMove()
        {
            if (rowMove == RowMoves.Right)
            {
                if (rowXRight < G_Width + ((NumOfCols - GetRightOfRow() - 1) * (octopus.Width + offset) - offset))
                {
                    rowXLeft += rowXSpeed;
                    rowXRight += rowXSpeed;
                }
                else
                {
                    rowMove = RowMoves.Left;
                    rowYDown += rowYSpeed;
                    rowYUp += rowYSpeed;
                    if (Landed()) gameState = GameStates.GameOver;
                }
            }
            else
            {

                if (rowXLeft >= -(GetLeftOfRow() * ((octopus.Width + offset)) - offset))
                {
                    rowXRight -= rowXSpeed;
                    rowXLeft -= rowXSpeed;
                }
                else
                {
                    rowMove = RowMoves.Right;
                    rowYDown += rowYSpeed;
                    rowYUp += rowYSpeed;
                    if (Landed()) gameState = GameStates.GameOver;
                }
            }
        }
        void NextLevel()
        {
            level++;
            // Initialize Row of Invaders
            int tmp = 0;
            Location temporary;

            // Reset Enemies lasers
            for (int i = 0; i < NumOfLaser; ++i)
                EnemiesLaser[i].Y = G_Height + EnemiesLaser[i].Height;
            // Reset Bunkers
            for (int i = 0; i < BunkerList.Count; ++i)
            {
                temporary = BunkerList[i];
                temporary.Frame = 0;
                BunkerList[i] = temporary;
            }
            // Reset cannon position
            cannon.X = G_Width / 2 - cannon.Width / 2;
            cannon.Y = G_Height - 50;
            // Reset row of invaders
            for (int i = 0; i < NumOfCols; ++i) tmp |= 0x01 << i;// tmp = 0x07FF
            for (int i = 0; i < InvadersRow.Length; ++i)
                InvadersRow[i] = tmp;// 5 row and 11 col
            rowMove = RowMoves.Right;
            rowYUp = 30;
            rowYDown = rowYUp + (squid.Height + offset) * NumOfRows - offset;
            rowXLeft = +50;
            rowXRight = rowXLeft + (octopus.Width + offset) * NumOfCols - offset;
            rowXSpeed = level;  // Increase game speed
            rowYSpeed = 5;
            Exploded = false;
            fireing = false;
            life++;             // Increse life
            hits = 0;
            saucer = false;
            RandomSaucer();
            EnterdChars = 0;
            gameState = GameStates.Running;
        }
        void Shot()
        {
            Random r = new Random();
            int x, y;
            for (int i = 0; i < NumOfLaser; ++i)
            {
                if (EnemiesLaser[i].Y < G_Height)
                {
                    EnemiesLaser[i].Y += 10;
                    if (EnemiesLaser[i].Y > BunkerList[0].Y)
                    {
                        int nearX = ((EnemiesLaser[i].X - (bunker.Width / 2)) / (G_Width / 4));
                        int nearXPixel = ((nearX + 1) * G_Width / 4) - (bunker.Width / 2);
                        int nearYPixel = G_Height - bunker.Height - (G_Height - cannon.Y) - 10;

                        if (EnemiesLaser[i].Collision(nearXPixel, nearYPixel, bunker.Width, bunker.Height) && nearX < 3)
                        {
                            Location tmp = BunkerList[nearX];
                            if (tmp.Frame < 4)
                            {
                                if (tmp.Delay < 0)
                                {
                                    tmp.Frame++;
                                    tmp.Delay = 3;
                                }
                                else
                                    tmp.Delay--;
                                BunkerList[nearX] = tmp;
                                EnemiesLaser[i].Delay = r.Next(100);
                                RandomShot(out x, out y);
                                EnemiesLaser[i].Y = y;
                                EnemiesLaser[i].X = x;
                            }
                        }
                        if (EnemiesLaser[i].Collision(cannon) && !Exploded)
                        {
                            die.X = cannon.X;
                            die.Y = cannon.Y + 4;
                            die.Delay = fps / 5;
                            die.SetFrame(0);
                            Exploded = true;
                            if (--life == 0)
                                gameState = GameStates.GameOver;
                            EnemiesLaser[i].Delay = r.Next(100);
                            RandomShot(out x, out y);
                            EnemiesLaser[i].Y = y;
                            EnemiesLaser[i].X = x;
                        }
                    }
                }
                else
                {
                    if (--EnemiesLaser[i].Delay < 0)
                    {
                        EnemiesLaser[i].Delay = r.Next(100);
                        RandomShot(out x, out y);
                        EnemiesLaser[i].Y = y;
                        EnemiesLaser[i].X = x;
                    }
                }
            }
        }
        void RandomShot(out int xPx, out int yPx)
        {
            Random r = new Random();
            int random = r.Next(NumOfCols);
            int tmp = 0;
            for (int i = 0; i < NumOfRows; ++i)
                tmp |= InvadersRow[i];
            while ((tmp & (0x01 << random)) == 0) random = r.Next(NumOfCols);
            for (tmp = NumOfRows - 1; tmp >= 0; --tmp)
            {
                if ((InvadersRow[tmp] & (0x01 << random)) != 0)
                {

                    break;
                }
            }
            yPx = octopus.Height + rowYUp + (tmp * (octopus.Height + offset));
            xPx = (octopus.Width / 2) + rowXLeft + (random * (octopus.Width + offset));
        }
        void SaucerMove()
        {
            if (!saucer)
            {
                Saucer.Delay--;
                if (Saucer.Delay <= 0)
                {
                    Saucer.Delay = fps / 32;
                    saucer = true;
                    //SoundThread = new System.Threading.Thread(SndSauser.PlayLooping);
                    //SoundThread.Start();
                }
            }
            else
            {
                if (Saucer.X > -Saucer.Width)
                    Saucer.X -= 10;
                else
                {
                    RandomSaucer();
                    saucer = false;
                    //SoundThread.Abort();
                }
            }
        }
        void RandomSaucer()
        {
            Random r = new Random();
            Saucer.X = G_Width;
            Saucer.Delay = r.Next(500, 1000);
        }

        int GetLeftOfRow()
        {
            int left = 0, tmp = 0;
            for (left = 0; left < NumOfRows; ++left)
                tmp |= InvadersRow[left];
            left = 0;
            while ((tmp & 0x01 << left) == 0 && left < NumOfCols) ++left;
            return left;
        }
        int GetRightOfRow()
        {
            int right = 0, tmp = 0;
            for (right = 0; right < NumOfRows; ++right)
                tmp |= InvadersRow[right];
            right = NumOfCols - 1;
            while ((tmp & (0x01 << right)) == 0 && right >= 0) --right;
            return right;
        }
        int GetDownOfRow()
        {
            int down = NumOfRows - 1;
            while (InvadersRow[down] == 0 && down >= 0) --down;
            return down;
        }
        int GetTopOfRow()
        {
            int top = 0;
            while (InvadersRow[top] == 0 && top < NumOfRows) ++top;
            return top;
        }
        
        bool AllHit()
        {
            for (int i = 0; i < NumOfRows; ++i)
                if (InvadersRow[i] != 0)
                    return false;
            return true;
        }
        bool Landed()
        {
            int o = ((GetDownOfRow() + 1) * (octopus.Height + offset) - offset) + rowYUp;
            if (G_Height - 20 > o)
                return false;
            return true;
        }
        #endregion

        #region Events
        // Game Timer
        private void mainTimer_Tick(object sender, EventArgs e)
        {
            DrawScene();
        }

        // Place Last key pressed in buffer
        private void clientKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (gameState == GameStates.splashScreen)
            {
                gameState = GameStates.Running;
                // Reset Level
                level = 0;
                life = 2;
                score = 0;
                NextLevel();
            }
            if (gameState == GameStates.Demo)
            {
                frames = 0;
                gameState = GameStates.splashScreen;
            }

            if (gameState != GameStates.GameOver)
            {
                switch (e.KeyData)
                {
                    case System.Windows.Forms.Keys.Left:
                        lastKeypressed |= GameKeys.Left;
                        break;

                    case System.Windows.Forms.Keys.Right:
                        lastKeypressed |= GameKeys.Right;
                        break;

                    case System.Windows.Forms.Keys.Escape:
                        lastKeypressed |= GameKeys.Escape;
                        break;

                    case System.Windows.Forms.Keys.Space:
                        lastKeypressed |= GameKeys.Fire;
                        break;
                    case System.Windows.Forms.Keys.P:
                        lastKeypressed |= GameKeys.Pause;
                        break;
                }
            }
            else
                if (e.KeyData == System.Windows.Forms.Keys.Enter)
                {
                    if (PlayerName.Length > 0)
                    {
                        ScoreRecorder.HighScores.Recorde(PlayerName, (10 * score).ToString(), level.ToString());
                        frames = 0;
                        gameState = GameStates.splashScreen;
                    }
                }
                else
                    if (e.KeyData == System.Windows.Forms.Keys.Back)
                    {
                        if (EnterdChars > 0)
                        {
                            EnterdChars--;
                            PlayerName = PlayerName.Remove(PlayerName.Length - 1);
                        }
                    }
                    else
                        if (EnterdChars < MaxChars)
                        {
                            if (e.KeyValue >= 'A' && e.KeyValue <= 'Z')
                            {
                                if (EnterdChars == 0)
                                {
                                    EnterdChars++;
                                    PlayerName = e.KeyCode.ToString();
                                }
                                else
                                {
                                    EnterdChars++;
                                    PlayerName += e.KeyCode;
                                }
                            }

                        }
        }


        // Clear relased key from buffer
        private void clientKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case System.Windows.Forms.Keys.Left:
                    lastKeypressed &= ~GameKeys.Left;
                    break;

                case System.Windows.Forms.Keys.Right:
                    lastKeypressed &= ~GameKeys.Right;
                    break;

                case System.Windows.Forms.Keys.Space:
                    lastKeypressed &= ~GameKeys.Fire;
                    break;

                case System.Windows.Forms.Keys.P:
                    paused = true;
                    break;

                case System.Windows.Forms.Keys.PrintScreen:
                    SaveScreenshot();
                    break;
            }
        }

        // Just for debug
        //private void FraneCtr_Tick(object sender, EventArgs e)
        //{
        //    fpsElapsed = frames / (float)2;
        //    frames = 0;
        //}
        #endregion
    }
}
