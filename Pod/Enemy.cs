using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pod
{
    public class Enemy
    {

        // The position of the enemy
        public Vector2 Position;

        // The velocity of the enemy
        public Vector2 Velocity;

        // The animation of the enemy
        public Animation Animation;

        // The enemy type this is
        public EnemyType Type;

        // The destination point of the enemy
        public Pod DestinationPod;

        // Velocity modifier
        public float VelocityModifier;

        // Keeps track of our rotation for us
        public float rotation;

        // The health this enemy has left
        public float Health;

        // The normalized move vector
        public Vector2 move;

        // The alpha of the enemy (for fading in)
        public float Alpha;
        public float alphaIncrease;

        // The level this enemy belongs to
        private Level level;

        // The random number generator we will use
        public Random r;

        // Dash cooldown
        public float DashCooldown;

        // Twirling
        public float TwirlRotation;

        // Slow Rotation
        public float SlowRotation;
        public float SlowRotationDirection;

        // The time to spawn imprints
        public float ImprintSpawn;

        // Spawn animation
        public Animation SpawnAnimation;
        public float SpawnAlpha;

        // Snaking
        public float OriginalRotation;
        public bool RotatingUp;

        // Are we initialized?
        public bool Initialized;
        public float InitializeTimer;

        // The origin for the enemy
        public Vector2 Origin;

        public Enemy(EnemyType type, Level l)
        {
            this.Type = type;
            Health = 1;
            VelocityModifier = 1.0f;

            InitializeTimer = 0;

            Alpha = 1.0f;
            alphaIncrease = 0.03f;
            level = l;

            DashCooldown = 0;

            Initialized = false;

            TwirlRotation = 0;
            SlowRotation = 0;

            ImprintSpawn = 0;

            Origin = new Vector2(Type.Texture.Width / 2, Type.Texture.Height / 2);

            if (Type.Equals(level.DashEnemy))
            {
                Animation = new Animation(Type.Texture, 40, 0.01f);
                Origin = new Vector2(Animation.FrameWidth / 2, Animation.Texture.Height / 2);
            }

            SpawnAnimation = new Animation(l.EnemySpawnTexture, 200.0f, 0.1f);
            SpawnAlpha = 0.01f;
        }

        public void Initialize()
        {
            // We are now initialized
            Initialized = true;

            // Only get a random velocity modifier if we aren't a cluster enemy
            if (Type.Equals(level.ClusterEnemy))
            {
                TwirlRotation = (float)r.Next(628) / 10.0f;
            }
            else if(!Type.Equals(level.SnakeEnemy))
            {
                VelocityModifier = (float)(r.Next(80, 120)) / 100.0f;
            }

            // depending on how we move, initialize differently
            switch(Type.Behavior)
            {
                case EnemyType.EnemyBehavior.DashTowardsPlayer:
                    {
                        move = DestinationPod.Position - Position;
                        move.Normalize();

                        Velocity = move * Type.Velocity;
                        break;
                    }
                case EnemyType.EnemyBehavior.Snake:
                    {
                        rotation = OriginalRotation;

                        // The direction we are rotating
                        RotatingUp = true;
                        break;
                    }
                case EnemyType.EnemyBehavior.Float:
                    {
                        int xran = 0;
                        int yran = 0;
                        while (xran == 0 && yran == 0)
                        {
                            xran = r.Next(-1, 1);
                            yran = r.Next(-1, 1);
                        }

                        Velocity.X = Type.Velocity * xran;
                        Velocity.Y = Type.Velocity * yran;

                        SlowRotation = (float)r.Next(628) / 10.0f;
                        int ran = r.Next(2);
                        if (ran == 0)
                        {
                            SlowRotationDirection = 1;
                        }
                        else
                        {
                            SlowRotationDirection = -1;
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        public void Update(GameTime gameTime)
        {
            // Move if we are faded in, otherwise fade in
            if (Alpha >= 1.0f && InitializeTimer <= 0)
            {
                // If we are not initialized, do so
                if (!Initialized)
                    Initialize();

                Move(gameTime);

                SpawnImprint(gameTime);

                if (SpawnAlpha > 0)
                {
                    SpawnAlpha -= 0.05f;
                }

                // Shoot(gameTime);
            }
            else
            {
                if (Type.Equals(level.DashEnemy))
                {
                    Animation.Paused = true;
                }
                Alpha += alphaIncrease;

                if (SpawnAlpha < 1.0f)
                {
                    SpawnAlpha += 0.1f;
                }

                InitializeTimer -= gameTime.ElapsedGameTime.Milliseconds;
            }
        }

        // Draws the enemy onscreen
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // spawning
            if (SpawnAlpha > 0)
            {
                float tempScale = 1.0f;
                if (Type.Equals(level.ClusterEnemy))
                {
                    tempScale = 0.75f;
                }
                SpawnAnimation.Draw(spriteBatch, gameTime, Position, new Color(Color.White, SpawnAlpha), 0, level.EnemySpawnOrigin, tempScale);
            }

            if (Type.Equals(level.DashEnemy))
            {
                Animation.Draw(spriteBatch, gameTime, Position, new Color(Color.White, Alpha), rotation, Animation.GetOrigin(), 1.0f);
                return;
            }
            spriteBatch.Draw(Type.Texture, new Vector2((int)Position.X, (int)Position.Y), null, new Color(Color.White, Alpha), rotation, Type.GetOrigin(), 1.0f, SpriteEffects.None, 0);
        }

        // Moves the enemy according to its behavior
        public void Move(GameTime gameTime)
        {
            // depending on how we behave, move differently
            switch (Type.Behavior)
            {
                case EnemyType.EnemyBehavior.MoveTowardsPlayer:
                    {
                        // Move the enemy
                        move = DestinationPod.Position - Position;
                        move.Normalize();

                        Position += move * Type.Velocity * VelocityModifier;

                        // Keeps track of our rotation
                        rotation = (float)Math.Atan2((double)move.Y, (double)move.X) + MathHelper.Pi / 2;

                        if (Type.Equals(level.ClusterEnemy))
                        {
                            // If we are twirling, twirl
                            Position.X += (float)Math.Cos((double)TwirlRotation) * 1.0f;
                            Position.Y += (float)Math.Sin((double)TwirlRotation) * 1.0f;
                            TwirlRotation += (628 / 10.0f);
                        }

                        break;
                    }
                case EnemyType.EnemyBehavior.Snake:
                    {

                        // Snakes the enemy
                        if (RotatingUp)
                        {
                            rotation += (628 / 10000.0f);
                        }
                        else
                        {
                            rotation -= (628 / 10000.0f);
                        }

                        // Move via rotation (snaking)
                        Position.X += (float)(Math.Cos((double)rotation) * Type.Velocity);
                        Position.Y += (float)(Math.Sin((double)rotation) * Type.Velocity);

                        if (Math.Abs(rotation - OriginalRotation) > Math.PI / 3)
                        {
                            if (RotatingUp)
                            {
                                RotatingUp = false;
                            }
                            else
                            {
                                RotatingUp = true;
                            }
                        }
                        // System.Diagnostics.Trace.WriteLine(Math.Abs(rotation - OriginalRotation).ToString());

                        BorderCheck();
                        break;
                    }
                       
                case EnemyType.EnemyBehavior.DashTowardsPlayer:
                    {
                        // Move
                        if (DashCooldown == 0)
                        {
                            Position += Velocity * VelocityModifier;

                            Animation.Paused = false;

                            // Keeps track of our rotation
                            rotation = (float)Math.Atan2((double)Velocity.Y, (double)Velocity.X) + MathHelper.Pi / 2;
                        }
                        else
                        {
                            DashCooldown += gameTime.ElapsedGameTime.Milliseconds;
                            Animation.Paused = true;
                            if (DashCooldown > 750)
                            {
                                DashCooldown = 0;
                            }
                        }

                        BorderCheck();

                        break;
                    }
                case EnemyType.EnemyBehavior.Float:
                    {
                        // Move
                        Position += Velocity * VelocityModifier;

                        BorderCheck();

                        // Keeps track of our rotation
                        rotation = (float)Math.Atan2((double)Velocity.Y, (double)Velocity.X) + MathHelper.Pi / 2;

                        SlowRotation += (6.28f) / (200.0f) * SlowRotationDirection;

                        rotation = rotation + SlowRotation;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        // Spawns an imprint of this enemy
        public void SpawnImprint(GameTime gameTime)
        {
            ImprintSpawn += gameTime.ElapsedGameTime.Milliseconds;

            if (ImprintSpawn >= 25)
            {
                ImprintSpawn = 0;

                Imprint im;
                if (Type.Equals(level.DashEnemy))
                {
                    im = new Imprint(Position, Type.Texture, Animation.GetRectangle(), 1.0f, rotation, Animation.GetOrigin());
                }
                else
                {
                    im = new Imprint(Position, Type.Texture, new Rectangle(0, 0, (int)Type.Texture.Width, (int)Type.Texture.Height), 1.0f, rotation, Type.GetOrigin());
                }

                level.Imprints.Add(im);
            }
        }

        // Switches the velocity if we hit a border
        public void BorderCheck()
        {
            if (Position.X - Origin.X < level.GameRectangle.Left)
            {
                Position.X = level.GameRectangle.Left + Origin.X;

                if (Type.Behavior.Equals(EnemyType.EnemyBehavior.Float) || Type.Behavior.Equals(EnemyType.EnemyBehavior.Snake))
                {
                    Velocity.X *= -1;
                    Position.X += Velocity.X;

                    OriginalRotation += MathHelper.Pi;
                    rotation += MathHelper.Pi;
                }
                else if (Type.Behavior.Equals(EnemyType.EnemyBehavior.DashTowardsPlayer))
                {
                    RecalculateDirection();
                }
            }
            else if (Position.X + Origin.X > level.GameRectangle.Right)
            {
                Position.X = level.GameRectangle.Right - Origin.X;

                if (Type.Behavior.Equals(EnemyType.EnemyBehavior.Float) || Type.Behavior.Equals(EnemyType.EnemyBehavior.Snake))
                {
                    Velocity.X *= -1;
                    Position.X += Velocity.X;

                    OriginalRotation += MathHelper.Pi;
                    rotation += MathHelper.Pi;
                }
                else if (Type.Behavior.Equals(EnemyType.EnemyBehavior.DashTowardsPlayer))
                {
                    RecalculateDirection();
                }
            }

            if (Position.Y + Origin.Y > level.GameRectangle.Bottom)
            {
                Position.Y = level.GameRectangle.Bottom - Origin.Y;

                if (Type.Behavior.Equals(EnemyType.EnemyBehavior.Float) || Type.Behavior.Equals(EnemyType.EnemyBehavior.Snake))
                {
                    Velocity.Y *= -1;
                    Position.Y += Velocity.Y;

                    OriginalRotation += MathHelper.Pi;
                    rotation += MathHelper.Pi;
                }
                else if (Type.Behavior.Equals(EnemyType.EnemyBehavior.DashTowardsPlayer))
                {
                    RecalculateDirection();
                }
            }
            else if (Position.Y - Origin.Y < level.GameRectangle.Top)
            {
                Position.Y = level.GameRectangle.Top + Origin.Y;

                if (Type.Behavior.Equals(EnemyType.EnemyBehavior.Float) || Type.Behavior.Equals(EnemyType.EnemyBehavior.Snake))
                {
                    Velocity.Y *= -1;
                    Position.Y += Velocity.Y;

                    OriginalRotation += MathHelper.Pi;
                    rotation += MathHelper.Pi;
                }
                else if (Type.Behavior.Equals(EnemyType.EnemyBehavior.DashTowardsPlayer))
                {
                    RecalculateDirection();
                }
            }
        }
        
        // Recalculates the direction
        public void RecalculateDirection()
        {
            move = DestinationPod.Position - Position;
            move.Normalize();

            Velocity = move * Type.Velocity;

            DashCooldown = 1;
        }

        // Gets the enemy rectangle
        public Rectangle GetRectangle()
        {
            if(Type.Equals(level.DashEnemy))
            {
                return new Rectangle((int)(Position.X - Origin.X), (int)(Position.Y - Origin.Y), (int)Animation.FrameWidth, (int)Type.Texture.Height);
            }
            return new Rectangle((int)(Position.X - Origin.X), (int)(Position.Y - Origin.Y), (int)Type.Texture.Width, (int)Type.Texture.Height);
        }

        // Gets the enemy rotated rectangle
        public RotatedRectangle GetRotatedRectangle()
        {
            return new RotatedRectangle(GetRectangle(), rotation);
        }
    }
}
