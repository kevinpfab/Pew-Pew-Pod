using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Pod
{
    public class Pod
    {

        // The Position of the Pod
        public Vector2 Position;

        // The PlayerIndex of this Pod
        public PlayerIndex PlayerIndex;

        // The State of the Gamepad
        public GamePadState gps;

        // The current gun of the pod
        public Gun Gun;

        // The movement velocity of the pod
        public float velocity;

        // The total velocity of the pod
        public Vector2 TotalVelocity;

        // The bullets we have fired
        public List<Bullet> Bullets;
        public Stack<Bullet> BulletPool;
        private List<Bullet> RemoveBullets;
        
        // Waits until we are ready (not coming out of a pause or a redo) to change guns
        public float PauseReady;

        // Keeps a reference of the level we are playing in
        private Level level;

        // The rotation we are at
        private float rotation;
        private float leftRotation;

        // The Amount of Ammo
        public int Ammo;
        public int MaxAmmo;
        public bool Overheated;

        // The time left to rumble
        public float RumbleTime;
        public float RumbleAmount;

        // The amount of lives our pod has
        public int Lives;
        public int Energy;

        // The Score we current have
        public int Score;

        // The random number generator
        public Random r;

        // Timer variables
        private float TimeSinceLastShot;

        // Readyness variables (keys need to be up before down again)
        private bool RightTriggerReady;

        // Are we currently respawning?
        public bool IsRespawning;
        private float RespawnTime;

        // Powerup variables
        private float PowerupTime;
        private float ShotSpeedModifier;
        private double directions;

        // Shooting timer
        public float ShootingTimer;

        // Hit particles
        public List<Particle> HitParticles;

        private bool PulsingBlue;

        public Pod(PlayerIndex playerIndex, Level level)
        {
            this.PlayerIndex = playerIndex;
            this.level = level;

            Initialize();
        }

        // Sets up any variables we might need for this pod
        public void Initialize()
        {
            Position = Vector2.Zero;

            Reset();
        }

        // Resets all variables
        public void Reset()
        {
            RespawnTime = 0;

            ShootingTimer = 0;

            Gun = level.Green;
            Bullets = new List<Bullet>();
            BulletPool = new Stack<Bullet>();
            RemoveBullets = new List<Bullet>();
            HitParticles = new List<Particle>();

            TotalVelocity = Vector2.Zero;

            TimeSinceLastShot = 0;

            MaxAmmo = 1000;
            Ammo = MaxAmmo;

            Lives = 3;
            Energy = 100;
            Score = 0;

            RumbleTime = 0;
            RumbleAmount = 0;

            velocity = 8;

            leftRotation = 0;

            PauseReady = 0;

            // Powerups
            PowerupTime = 0;
            ShotSpeedModifier = 1.0f;
            directions = Math.PI / 2;

            IsRespawning = false;

            r = new Random();
        }

        // Update the pod
        public void Update(GameTime gameTime)
        {

            // Check for respawning
            if (IsRespawning)
            {
                RespawnTime += gameTime.ElapsedGameTime.Milliseconds;

                if (RespawnTime > 2000 && RespawnTime < 3000)
                {
                    IsRespawning = false;
                }
            }

            // Handles powerups
            HandlePowerups(gameTime);

            // Get the current gamepad state
            gps = GamePad.GetState(PlayerIndex);

            GetInput(gameTime);

            // Decrease shooting timer
            if (ShootingTimer > 0)
            {
                ShootingTimer -= gameTime.ElapsedGameTime.Milliseconds;
            }
            if (IsFiring())
            {
                ShootingTimer = 2000;
            }

            // Moves the pod
            if (!IsRespawning)
            {
                Move(gameTime);
            }
            
            // Cleans up bullets
            CleanUpBullets();

            // Shoot with our current gun
            if (!IsRespawning)
            {
                Shoot(gameTime);
            }

            // Updates the bullets
            UpdateBullets(gameTime);

            // Updates the hit particles
            UpdateHitParticles(gameTime);

            // Recharges Ammo
            RechargeAmmo();

            // Generates a particle pulse
            // GeneratePulse();

            // Spawns the particles trailing the pod
            if (!IsRespawning)
            {
                SpawnTrail();
            }

            // Cap energy
            if (Energy > 100)
            {
                Energy = 100;
            }
            else if (Energy <= 0)
            {
                Energy = 0;
                Die();
            }

            UpdateRumble(gameTime);
        }

        // Updates the pods in a game over state
        public void GameOverUpdate(GameTime gameTime)
        {
            gps = GamePad.GetState(PlayerIndex);

            UpdateBullets(gameTime);
            CleanUpBullets();
            UpdateHitParticles(gameTime);
            CleanUpHitParticles();
            UpdateRumble(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // If we are respawning do not draw
            if (IsRespawning)
                return;

            // Determine the rotation of the pod
            if (IsFiring())
            {
                rotation = GetRightRotation() + (MathHelper.Pi / 2);
            }

            if(IsMoving())
            {
                leftRotation = GetLeftRotation() + (MathHelper.Pi / 2);
            }

            // The overhead displat
            int num = 1;
            if (PlayerIndex.Equals(PlayerIndex.Two))
            {
                num = 2;
            }
            else if (PlayerIndex.Equals(PlayerIndex.Three))
            {
                num = 3;
            }
            else if (PlayerIndex.Equals(PlayerIndex.Four))
            {
                num = 4;
            }
            spriteBatch.DrawString(level.gameFont, "P" + num.ToString(), new Vector2(Position.X - 10, Position.Y - 40), Color.White);


            // The color overlay
            Color c = Color.White;
            if (Overheated)
            {
                // Determine the percent of the border (vertical) to draw
                float percent = ((float)Ammo) / ((float)MaxAmmo);

                // Change the color (from white all the way to full red)
                byte gb = (byte)(255 * percent);
                c = new Color((byte)255, gb, gb);
            }

            // Draw the pod
            spriteBatch.Draw(Gun.Texture, Position, null, c, leftRotation, GetOrigin(), 1.0f, SpriteEffects.None, 0);
            spriteBatch.Draw(Gun.GunTexture, new Vector2(Position.X + 0, Position.Y + 0), null, c, rotation, Gun.GunOrigin, 1.0f, SpriteEffects.None, 0);

        }

        // Draw the bullets onscreen
        public void DrawBullets(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Draw every bullet we have spawned
            foreach (Bullet b in Bullets)
            {
                b.Draw(spriteBatch, gameTime);
            }
        }

        // Draws the hit particles
        public void DrawHitParticles(SpriteBatch spriteBatch)
        {
            foreach (Particle p in HitParticles)
            {
                p.Draw(spriteBatch);
            }
        }


        public void GetInput(GameTime gameTime)
        {
            // If we are ready to change
            if (PauseReady == 0)
            {
                if (gps.IsButtonDown(Buttons.A) || gps.IsButtonDown(Buttons.RightShoulder))
                {
                    Gun = level.Green;
                }
                else if (gps.IsButtonDown(Buttons.X) || gps.IsButtonDown(Buttons.LeftShoulder))
                {
                    Gun = level.Blue;
                }
                else if (gps.IsButtonDown(Buttons.Y) || gps.IsButtonDown(Buttons.LeftTrigger))
                {
                    Gun = level.Yellow;
                }
                else if (gps.IsButtonDown(Buttons.B) || gps.IsButtonDown(Buttons.RightTrigger))
                {
                    Gun = level.Red;
                }
            }
            else if (PauseReady > 100)
            {
                PauseReady = 0;
            }
            else
            {
                PauseReady += gameTime.ElapsedGameTime.Milliseconds;
            }
        }

        // Are we currently firing?
        public bool IsFiring()
        {
            return (Math.Abs(gps.ThumbSticks.Right.X) > 0.2f || Math.Abs(gps.ThumbSticks.Right.Y) > 0.2f);
        }

        // Are we currently moving
        public bool IsMoving()
        {
            return (Math.Abs(gps.ThumbSticks.Left.X) > 0.2f || Math.Abs(gps.ThumbSticks.Left.Y) > 0.2f);
        }

        // Helper methods
        private float GetYAxis()
        {
            return gps.ThumbSticks.Right.Y;
        }
        private float GetXAxis()
        {
            return gps.ThumbSticks.Right.X;
        }

        // The center of the pod
        private Vector2 GetOrigin()
        {
            return Gun.Origin;
        }

        // Gets the rotation of the pod shooter
        public float GetRightRotation()
        {
            return (float)(Math.Atan2((double)(GetYAxis() * -1), (double)GetXAxis()));
        }

        // Gets the left stick rotation of the pod
        public float GetLeftRotation()
        {
            return (float)(Math.Atan2((double)(gps.ThumbSticks.Left.Y * -1), (double)(gps.ThumbSticks.Left.X)));
        }

        // Gets the rectangle of the Pod
        public Rectangle GetRectangle()
        {
            return new Rectangle((int)(Position.X - (Gun.Texture.Width / 4)), (int)(Position.Y - (Gun.Texture.Height / 4)), (int)(Gun.Texture.Width / 2), (int)(Gun.Texture.Height / 2));
        }

        // Gets the rotated rectangle of the pod
        public RotatedRectangle GetRotatedRectangle()
        {
            return new RotatedRectangle(GetRectangle(), leftRotation);
        }

        // Moves the pod
        public void Move(GameTime gameTime)
        {
            if (!IsMoving())
            {
                TotalVelocity *= 0.8f;
                Position += TotalVelocity;
                return;
            }

            float xDistance = (velocity * gps.ThumbSticks.Left.X);
            float yDistance = (velocity * gps.ThumbSticks.Left.Y * -1);

            TotalVelocity.X = xDistance;
            TotalVelocity.Y = yDistance;

            Vector2 newPosition = new Vector2(Position.X + xDistance, Position.Y + yDistance);

            Position.X += xDistance;
            Position.Y += yDistance;

            if (Position.Y > (level.GameRectangle.Bottom - Gun.Texture.Height / 2))
            {
                Position.Y = level.GameRectangle.Bottom - Gun.Texture.Height / 2;
                TotalVelocity.Y = 0;
                level.BorderShowing = true;
            }
            else if(Position.Y < (level.GameRectangle.Top + Gun.Texture.Height / 2))
            {
                Position.Y = level.GameRectangle.Top + Gun.Texture.Height / 2;
                TotalVelocity.Y = 0;
                level.BorderShowing = true;
            }
            if (Position.X > (level.GameRectangle.Right - Gun.Texture.Width / 2))
            {
                Position.X = level.GameRectangle.Right - Gun.Texture.Width / 2;
                TotalVelocity.X = 0;
                level.BorderShowing = true;
            }
            else if(Position.X < (level.GameRectangle.Left + Gun.Texture.Width / 2))
            {
                Position.X = level.GameRectangle.Left + Gun.Texture.Width / 2;
                TotalVelocity.X = 0;
                level.BorderShowing = true;
            }
        }


        // Shoots the bullets for us
        public void Shoot(GameTime gameTime)
        {
            // If we aren't aiming anywhere, don't shoot
            if (!IsFiring() || Overheated)
                return;

            // If we can shoot, shoot
            if (TimeSinceLastShot > (Gun.FireRate / ShotSpeedModifier) && Ammo >= Gun.Bullet.AmmoCost)
            {
                // If we dont' have enough ammo for a pulse of blue
                if (!PulsingBlue && Gun.Equals(level.Blue) && Ammo < (Gun.Bullet.AmmoCost * 10))
                    return;


                SpawnBullet();

                TimeSinceLastShot = 0;
                // Ammo -= Gun.Bullet.AmmoCost;
                PulsingBlue = true;
            }
            else
            {
                PulsingBlue = false;
            }
            TimeSinceLastShot += gameTime.ElapsedGameTime.Milliseconds;

            // Did we overheat?
            if (Ammo < Gun.Bullet.AmmoCost)
            {
                Overheated = true;
            }
        }

        // starts the rumble of the controller of this pod
        public void StartRumble(float amount, float time)
        {
            if (Game1.Global.IsRumbleOn == false)
                return;

            this.RumbleTime = time;
            this.RumbleAmount = amount;
        }

        public void UpdateRumble(GameTime gameTime)
        {
            // If our rumble time is at 0, we stop rumbling
            if (this.RumbleTime <= 0)
            {
                GamePad.SetVibration(this.PlayerIndex, 0, 0);
            }
            else
            {
                this.RumbleTime -= gameTime.ElapsedGameTime.Milliseconds;
                GamePad.SetVibration(this.PlayerIndex, RumbleAmount, RumbleAmount);
            }
        }

        // Spawns a bullet
        public void SpawnBullet()
        {

            for (double i = 0; i < directions; i += (Math.PI / 2))
            {
                // Determines the velocity of the bullet
                float rotation = GetRightRotation();
                float xVel = Gun.Bullet.Velocity * (float)Math.Cos((double)rotation + i);
                float yVel = Gun.Bullet.Velocity * (float)Math.Sin((double)rotation + i);
                Vector2 velocity = new Vector2(xVel, yVel);

                // Creates the bullet
                if (Gun.Equals(level.Green))
                {
                    Bullet b1 = null;
                    if (BulletPool.Count > 0)
                    {
                        b1 = BulletPool.Pop();
                        b1.Type = Gun.Bullet;
                        b1.Velocity = velocity;
                        b1.level = level;
                        b1.Reset();
                    }
                    else
                    {
                        b1 = new Bullet(Gun.Bullet, velocity, level);
                    }
                    Bullet b2 = null;
                    if (BulletPool.Count > 0)
                    {
                        b2 = BulletPool.Pop();
                        b2.Type = Gun.Bullet;
                        b2.Velocity = velocity;
                        b2.level = level;
                        b2.Reset();
                    }
                    else
                    {
                        b2 = new Bullet(Gun.Bullet, velocity, level);
                    }

                    Vector2 topShot = new Vector2(Position.X, Position.Y);
                    Vector2 botShot = new Vector2(Position.X, Position.Y);

                    topShot.X += (float)(Math.Cos(GetRightRotation() + (MathHelper.Pi / 2) + i)) * 10;
                    topShot.Y += (float)(Math.Sin(GetRightRotation() + (MathHelper.Pi / 2) + i)) * 10;
                    botShot.X -= (float)(Math.Cos(GetRightRotation() + (MathHelper.Pi / 2) + i)) * 10;
                    botShot.Y -= (float)(Math.Sin(GetRightRotation() + (MathHelper.Pi / 2) + i)) * 10;

                    /*
                    b1.Position.X = topShot.X + (float)(Math.Cos(GetRightRotation() + i)) * 15;
                    b1.Position.Y = topShot.Y + (float)(Math.Sin(GetRightRotation() + i)) * 15;

                    b2.Position.X = botShot.X + (float)(Math.Cos(GetRightRotation() + i)) * 15;
                    b2.Position.Y = botShot.Y + (float)(Math.Sin(GetRightRotation() + i)) * 15;
                     * */

                    b2.Position = b1.Position = Position;
                    b2.rotTime = MathHelper.Pi;

                    Bullets.Add(b1);
                    Bullets.Add(b2);

                    // Play the sound effect
                    if (i % (Math.PI) == 0)
                    {
                        level.GreenLaserSound.Play(Game1.Global.SoundEffectVolume, 0, 0);
                    }
                }
                else if (Gun.Equals(level.Red))
                {
                    for (float j = -(MathHelper.Pi / 6); j <= MathHelper.Pi / 6; j += (MathHelper.Pi / 3) / 5)
                    {
                        Bullet b = null;
                        if (BulletPool.Count > 0)
                        {
                            b = BulletPool.Pop();
                            b.Type = Gun.Bullet;
                            b.Velocity = velocity;
                            b.level = level;
                            b.Reset();
                        }
                        else
                        {
                            b = new Bullet(Gun.Bullet, velocity, level);
                        }

                        b.Position.X = Position.X + (float)(Math.Cos(GetRightRotation() + i)) * 20;
                        b.Position.Y = Position.Y + (float)(Math.Sin(GetRightRotation() + i)) * 20;

                        b.Velocity.X = b.Velocity.X + 5.0f * (float)(Math.Cos(GetRightRotation() + i + j));
                        b.Velocity.Y = b.Velocity.Y + 5.0f * (float)(Math.Sin(GetRightRotation() + i + j));

                        Bullets.Add(b);
                    }

                    // Sound effect
                    if (i % (Math.PI) == 0)
                    {
                        level.RedSound.Play(Game1.Global.SoundEffectVolume, 0, 0);
                    }
                }
                else if (Gun.Equals(level.Blue))
                {
                    Bullet b = null;
                    if (BulletPool.Count > 0)
                    {
                        b = BulletPool.Pop();
                        b.Type = Gun.Bullet;
                        b.Velocity = velocity;
                        b.level = level;
                        b.Reset();
                    }
                    else
                    {
                        b = new Bullet(Gun.Bullet, velocity, level);
                    }

                    b.Position.X = Position.X + (float)(Math.Cos(GetRightRotation() + i)) * 40;
                    b.Position.Y = Position.Y + (float)(Math.Sin(GetRightRotation() + i)) * 40;

                    Bullets.Add(b);

                    // Sound effect
                    if (i % (Math.PI) == 0)
                    {
                        level.BlueSound.Play(Game1.Global.SoundEffectVolume, 0, 0);
                    }
                }
                else if (Gun.Equals(level.Yellow))
                {
                    Bullet b = null;
                    if (BulletPool.Count > 0)
                    {
                        b = BulletPool.Pop();
                        b.Type = Gun.Bullet;
                        b.Velocity = velocity;
                        b.level = level;
                        b.Reset();
                    }
                    else
                    {
                        b = new Bullet(Gun.Bullet, velocity, level);
                    }

                    b.Position.X = Position.X + (float)(Math.Cos(GetRightRotation() + i)) * 50;
                    b.Position.Y = Position.Y + (float)(Math.Sin(GetRightRotation() + i)) * 50;

                    b.Acceleration = velocity / 10.0f;

                    Bullets.Add(b);

                    // Sound effect
                    if (i % (Math.PI) == 0)
                    {
                        level.YellowSound.Play(Game1.Global.SoundEffectVolume, 0, 0);
                    }
                }
                else
                {
                    Bullet b = null;
                    if (BulletPool.Count > 0)
                    {
                        b = BulletPool.Pop();
                        b.Type = Gun.Bullet;
                        b.Velocity = velocity;
                        b.level = level;
                        b.Reset();
                    }
                    else
                    {
                        b = new Bullet(Gun.Bullet, velocity, level);
                    }
                    b.Position = Position;

                    // Adds the bullet onscreen
                    Bullets.Add(b);
                }
            }
        }

        // Spawns a trail beyond the pod
        public void SpawnTrail()
        {
            if (!IsMoving())
                return;

            Texture2D texture = null;
            if (Gun.Equals(level.Yellow))
            {
                texture = level.YellowParticle;
            }
            else if (Gun.Equals(level.Red))
            {
                texture = level.RedParticle;
            }
            else if (Gun.Equals(level.Blue))
            {
                texture = level.BlueParticle;
            }
            else if (Gun.Equals(level.Green))
            {
                texture = level.GreenParticle;
            }

            if(level.Pods.Count == 2)
            {
                if(this.Equals(level.Pods.ElementAt(0)))
                {
                    texture = level.GreenParticle;
                }
                else if(this.Equals(level.Pods.ElementAt(1)))
                {
                    texture = level.BlueParticle;
                }
            }
            else if (level.Pods.Count == 3)
            {
                if (this.Equals(level.Pods.ElementAt(0)))
                {
                    texture = level.GreenParticle;
                }
                else if (this.Equals(level.Pods.ElementAt(1)))
                {
                    texture = level.BlueParticle;
                }
                else if (this.Equals(level.Pods.ElementAt(2)))
                {
                    texture = level.YellowParticle;
                }
            }
            else if (level.Pods.Count == 4)
            {
                if (this.Equals(level.Pods.ElementAt(0)))
                {
                    texture = level.GreenParticle;
                }
                else if (this.Equals(level.Pods.ElementAt(1)))
                {
                    texture = level.BlueParticle;
                }
                else if (this.Equals(level.Pods.ElementAt(2)))
                {
                    texture = level.YellowParticle;
                }
                else if (this.Equals(level.Pods.ElementAt(3)))
                {
                    texture = level.RedParticle;
                }
            }

            level.SpawnTrail(10, texture, Position, (float)(GetLeftRotation() + (Math.PI)), 0.015f);
        }

        // Generates a pulse, starting at this pod
        public void GeneratePulse()
        {
            // Are we trying to generate a pulse?
            if (gps.IsButtonDown(Buttons.RightTrigger) && RightTriggerReady)
            {
                level.SpawnPulse(100, Position);
                RightTriggerReady = false;
            }
            else if (gps.IsButtonUp(Buttons.RightTrigger))
            {
                RightTriggerReady = true;
            }
        }

        // Charges the player's ammo
        public void RechargeAmmo()
        {
            if (Ammo < MaxAmmo)
            {
                Ammo += 5;
            }

            // Are we no longer overheated?
            if (Ammo >= MaxAmmo)
            {
                Overheated = false;
            }
        }

        // Updates all the bullets
        public void UpdateBullets(GameTime gameTime)
        {
            foreach (Bullet b in Bullets)
            {
                b.Update(gameTime);
            }
        }


        // Cleans up the bullets that aren't onscreen
        public void CleanUpBullets()
        {
            // Check if we can get rid of bullets
            foreach (Bullet b in Bullets)
            {
                if (level.BeyondBorder(b.Position) || !level.IsOnScreen(b.Position) || (b.Alpha <= 0 && b.Type.Properties.Equals(BulletType.BulletProperties.Fire)))
                {
                    // Bounce the blue ball if the time isn't up
                    if (b.Type.Equals(level.BlueBullet) && b.Timer < 2000 && level.BeyondBorder(b.Position))
                    {
                        if (b.Position.X >= level.GameRectangle.Right || b.Position.X <= level.GameRectangle.Left)
                        {
                            b.Velocity.X *= -1;
                            if (b.Position.X >= level.GameRectangle.Right)
                            {
                                b.Position.X = level.GameRectangle.Right + b.Velocity.X;
                            }
                            else if (b.Position.X <= level.GameRectangle.Left)
                            {
                                b.Position.X = level.GameRectangle.Left + b.Velocity.X;
                            }
                        }
                        if (b.Position.Y >= level.GameRectangle.Bottom || b.Position.Y <= level.GameRectangle.Top)
                        {
                            b.Velocity.Y *= -1;
                            if (b.Position.Y >= level.GameRectangle.Bottom)
                            {
                                b.Position.Y = level.GameRectangle.Bottom + b.Velocity.Y;
                            }
                            else if (b.Position.Y <= level.GameRectangle.Top)
                            {
                                b.Position.Y = level.GameRectangle.Top + b.Velocity.Y;
                            }
                        }
                    }
                    else
                    {
                        RemoveBullets.Add(b);
                        if (level.BeyondBorder(b.Position))
                        {
                            if (b.Type.Equals(level.YellowBullet))
                            {
                                level.SpawnExplosion(HitParticles, 10, b.Position, level.YellowBulletReplaced, 2, 2, 0);
                            }
                            else
                            {
                                level.SpawnExplosion(level.Particles, 10, b.Position, b.Type, 2, 3, 0.02f);
                            }
                        }
                    }
                }
            }
            
            // Remove offscreen bullets
            foreach (Bullet b in RemoveBullets)
            {
                Bullets.Remove(b);
                BulletPool.Push(b);
            }
            RemoveBullets.Clear();
        }

        // Updates the hit particles
        public void UpdateHitParticles(GameTime gameTime)
        {
            foreach (Particle p in HitParticles)
            {
                p.Update(gameTime);
            }

            CleanUpHitParticles();
        }

        // Removes unwanted hit particles
        public void CleanUpHitParticles()
        {
            for (int i = HitParticles.Count - 1; i >= 0; i--)
            {
                Particle p = HitParticles.ElementAt(i);
                if (!level.IsOnScreen(p.Position) || p.Alpha <= 0 || p.Scale <= 0)
                {
                    HitParticles.RemoveAt(i);
                }
            }
        }

        // Moves all the elements related to this pod
        public void MoveElements(Vector2 distance)
        {
            Position += distance;

            foreach (Bullet b in Bullets)
            {
                b.Position += distance;
            }

            foreach (Particle p in HitParticles)
            {
                p.Position += distance;
            }

        }

        // Draws the HUD onscreen
        public void DrawHUD(SpriteBatch spriteBatch, Vector2 pos)
        {
            string name = HighscoreComponent.NameFromPlayer(PlayerIndex);

            if (name.Length > 12)
            {
                name = name.Substring(0, 11) + "...";
            }

            if (name.Equals("Guest"))
                name = "Player " + PlayerIndex.ToString();

            spriteBatch.DrawString(level.gameFont, name, pos, Color.White);
            pos.Y += 18;
            if (!level.matchType.Equals(Level.MatchType.Versus))
            {
                spriteBatch.DrawString(level.gameFont, "Lives: " + Lives + "", pos, Color.White);
                pos.Y += 18;
            }
            spriteBatch.DrawString(level.gameFont, "" + Score, pos, Color.White);
        }

        // Draws the HUD onscreen
        public void DrawHUD(SpriteBatch spriteBatch, Vector2 pos, int xIncrement)
        {
            string name = HighscoreComponent.NameFromPlayer(PlayerIndex);

            if (name.Length > 12)
            {
                name = name.Substring(0, 11) + "...";
            }

            if (name.Equals("Guest"))
            {
                if(level.Pods.ElementAt(0).Equals(this))
                {
                    name = "Player One";
                }
                else if(level.Pods.ElementAt(1).Equals(this))
                {
                    name = "Player Two";
                }
                else if(level.Pods.ElementAt(2).Equals(this))
                {
                    name = "Player Three";
                }
                else if(level.Pods.ElementAt(3).Equals(this))
                {
                    name = "Player Four";
                }
            }

            spriteBatch.DrawString(level.gameFont, name, pos, Color.White);
            pos.Y += 18;
            pos.X += xIncrement;
            if (!level.matchType.Equals(Level.MatchType.Versus))
            {
                spriteBatch.DrawString(level.gameFont, "Lives: " + Lives + "", pos, Color.White);
                pos.Y += 18;
                pos.X += xIncrement;
            }
            spriteBatch.DrawString(level.gameFont, "" + Score, pos, Color.White);
        }

        // Applies the powerup to our player
        public void ApplyPowerup(Powerup.PowerupType type)
        {
            if (type == Powerup.PowerupType.FasterShots)
            {
                ShotSpeedModifier = 2.0f;
                PowerupTime = 10000.0f;
            }
            else if (type == Powerup.PowerupType.FourWayShooter)
            {
                directions = Math.PI * 2;
                PowerupTime = 10000.0f;
            }

            StartRumble(1.0f, 200.0f);
        }

        // Handle powerups
        public void HandlePowerups(GameTime gameTime)
        {
            if (PowerupTime > 0)
            {
                PowerupTime -= gameTime.ElapsedGameTime.Milliseconds;
            }
            else
            {
                // Reset variables
                PowerupTime = 0;
                ShotSpeedModifier = 1.0f;
                directions = Math.PI / 2;
            }
        }

        // Die
        public void Die()
        {
            // If we are already dying, do nothing
            if (IsRespawning)
                return;

            // explode all enemies onscreen
            foreach (Enemy e in level.Enemies)
            {
                level.SpawnExplosion(level.Particles, e.Type.ParticleNumber / 5, e.Position, Gun.Bullet, 4, 4, 0.01f);
                level.SpawnLightFade(e.Position, Gun.Bullet, 0.06f);
                e.Health = -1;
            }

            IsRespawning = true;
            Lives--;
            if (level.matchType.Equals(Level.MatchType.Versus))
            {
                Position = level.RandomSpawnPosition(1);
                Energy = 100;
            }
            else
            {
                Position = new Vector2(640, 360);
            }
            if (Lives > 0)
            {
                RespawnTime = 0;
            }
            else
            {
                // If we are dead we can no longer respawn
                RespawnTime = 3000;
            }
            PowerupTime = 0;
            TotalVelocity = Vector2.Zero;

            level.ShakeScreen(500.0f);
            StartRumble(1.0f, 500.0f);

            level.DeathSound.Play(Game1.Global.SoundEffectVolume, 0, 0);
        }
    }
}
