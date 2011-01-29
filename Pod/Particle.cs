using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pod
{
    public class Particle
    {
        // The texture for a particle
        public Texture2D Texture;

        // The Position of the particle
        public Vector2 Position;

        // The Velocity of the particle
        public Vector2 Velocity;
        public Vector2 ConstantVelocity;

        // The acceleration of the particle
        public Vector2 Acceleration;

        // The color of the particle
        public Color color;

        // The rotation of the particle
        public float Rotation;

        // The alpha of the particle
        public float Alpha;

        // The scale of the particle
        public float Scale;

        // How much this particle will fade
        public float FadeDecrease;

        // Is this a trail particle?
        public bool TrailParticle;
        // Is this a light fade particle?
        public bool LightFadeParticle;
        // Is this an explosion particle?
        public bool ExplosionParticle;
        // Is this a rocket particle?
        public bool RocketParticle;
        // Is this a background particle?
        public bool LiquidParticle;
        // Is this a nebula?
        public bool NebulaParticle;
        // Is this a planet?
        public bool PlanetParticle;
        public float LifeTime;
        public float colorChangeVelocity;

        public Particle(Texture2D texture, Vector2 position)
        {
            this.Texture = texture;
            this.Position = position;

            Reset();
        }

        // Resets the particle
        public void Reset()
        {
            Velocity = Vector2.Zero;
            Acceleration = Vector2.Zero;

            color = Color.White;

            Rotation = 0;
            Alpha = 1.0f;
            Scale = 1.0f;
            FadeDecrease = 0.01f;

            colorChangeVelocity = 1;
        }

        // Get a random color
        private Color GetRandomColor()
        {
            Random ran = new Random();

            byte b = (byte)ran.Next(255);
            byte r = (byte)ran.Next(255);
            byte g = (byte)ran.Next(255);

            return new Color(r, g, b);
        }

        // Updates the particle
        public void Update(GameTime gameTime)
        {

            // Change velocity
            Velocity += Acceleration;

            //Change position
            Position += Velocity;

            // Rotate the particle
            if (!LiquidParticle && !NebulaParticle)
            {
                Rotation = (float)Math.Atan2((double)Velocity.Y, (double)Velocity.X);
            }
            else
            {
                Rotation += MathHelper.Pi / 2000;
            }

            // If we don't have velocity, reduce alpha
            if (TrailParticle)
            {
                Alpha -= 0.1f;
            }

            // If we are an explosion particle, reduce alpha and slow down and shrink over time
            if (ExplosionParticle && !RocketParticle)
            {
                Alpha -= FadeDecrease;
                Scale -= FadeDecrease;
            }

            if (RocketParticle)
            {
                Scale -= 0.02f;
            }

            // If this is a light fade particle, reduce alpha and shrink
            if (LightFadeParticle)
            {
                Alpha -= FadeDecrease;
                Scale -= FadeDecrease;
            }

            // If this is a nebula particle
            if (NebulaParticle)
            {
                LifeTime += gameTime.ElapsedGameTime.Milliseconds;
            }

            // If this is a liquid particle, slow it down
            if (LiquidParticle)
            {

                Velocity *= 0.98f;

                //Velocity.X = MathHelper.Clamp(Velocity.X, -3, 3);
                //Velocity.Y = MathHelper.Clamp(Velocity.Y, -3, 3);

                
                /*if (Math.Abs((double)Velocity.X) < 0.02 && Math.Abs((double)Velocity.Y) < 0.02)
                {
                    Velocity = ConstantVelocity;
                }*/
                Velocity = ConstantVelocity;

                // vary color
                /*color.R = (byte)(color.R + (byte)(normalized.X * colorChangeVelocity));
                color.G = (byte)(color.G + (byte)(normalized.Y * colorChangeVelocity));

                if ((color.R == 168 && color.G == 249) || (color.R == 255 && color.G == 153))
                {
                    colorChangeVelocity *= -1;
                }*/
            }

            if(ExplosionParticle || LiquidParticle)
            {
                if (Position.X < 128)
                {
                    Velocity.X = Math.Abs(Velocity.X);
                    ConstantVelocity.X = Math.Abs(ConstantVelocity.X);
                    Acceleration.X = Math.Abs(Acceleration.X) * -1;
                    Position.X += Velocity.X * 2;
                }
                else if(Position.X > 1152)
                {
                    Velocity.X = Math.Abs(Velocity.X) * -1;
                    ConstantVelocity.X = Math.Abs(ConstantVelocity.X) * -1;
                    Acceleration.X = Math.Abs(Acceleration.X);
                    Position.X += Velocity.X * 2;
                }

                if (Position.Y < 72)
                {
                    Velocity.Y = Math.Abs(Velocity.Y);
                    ConstantVelocity.Y = Math.Abs(ConstantVelocity.Y);
                    Acceleration.Y = Math.Abs(Acceleration.Y) * -1;
                    Position.Y += Velocity.Y * 2;
                }
                else if (Position.Y > 648)
                {
                    Velocity.Y = Math.Abs(Velocity.Y) * -1;
                    ConstantVelocity.Y = Math.Abs(ConstantVelocity.Y) * -1;
                    Acceleration.Y = Math.Abs(Acceleration.Y);
                    Position.Y += Velocity.Y * 2;
                }
            }


            if (PlanetParticle)
            {
                if (Position.X < -128)
                {
                    Velocity.X = Math.Abs(Velocity.X);
                }
                else if (Position.X > 1408)
                {
                    Velocity.X = Math.Abs(Velocity.X) * -1;
                }

                if (Position.Y < -72)
                {
                    Velocity.Y = Math.Abs(Velocity.Y);
                }
                else if (Position.Y > 792)
                {
                    Velocity.Y = Math.Abs(Velocity.Y) * -1;
                }


                // Always face the center
                Vector2 normal = Position - Game1.CenterVector;
                normal.Normalize();
                Rotation = (float)(Math.Atan2((double)normal.Y, (double)normal.X)) + MathHelper.Pi;
            }
        }

        // Draws the particle
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, new Color(color, Alpha), Rotation, new Vector2(Texture.Width / 2, Texture.Height / 2), Scale, SpriteEffects.None, 0);
        }

        // Gets the particle rectangle
        public Rectangle GetRectangle()
        {
            return new Rectangle((int)(Position.X + (Texture.Width / 2)), (int)(Position.Y + (Texture.Height / 2)), (int)5, (int)5);
        }


    }
}
