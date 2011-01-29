using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pod
{
    public class Bullet
    {
        // The Position of this bullet
        public Vector2 Position;

        // The BulletType this bullet is
        public BulletType Type;

        // The velocity of this bullet
        public Vector2 Velocity;

        // The acceleration of the bullet
        public Vector2 Acceleration;

        // The origin of this bullet
        private Vector2 origin;

        // The alpha value of this bullet
        public float Alpha;

        // The rotation of the bullet
        public float Rotation;

        // The Animation of this bullet
        public Animation Animation;

        // A reference to the level the bullet is in
        public Level level;

        // A timer for this bullet
        public float Timer;

        // The trailing particles
        public float ParticleSpawnTime;
        public float MaxSpawnTime;

        // The scale of the bullet
        public float Scale;

        // the color of the Bullet
        public Color BulletColor;

        // The current timed rotation
        public float rotTime;

        public Bullet(BulletType bType, Vector2 velocity, Level level)
        {
            this.Type = bType;
            this.Velocity = velocity;
            this.level = level;

            Reset();
        }

        // Resets the bullet
        public void Reset()
        {
            Acceleration = Vector2.Zero;

            origin = new Vector2(Type.Texture.Width / 2, Type.Texture.Height / 2);
            Alpha = 1.0f;

            Random r = new Random();
            ParticleSpawnTime = 0;
            MaxSpawnTime = (float)(r.Next(50) + 50);

            Timer = 0;
            rotTime = 0;

            Scale = 1.0f;

            BulletColor = Color.White;

            // Creates the animation
            Animation = new Animation(Type.Texture, Type.FrameWidth, Type.FrameRate);
        }

        // Updates this bullet
        public void Update(GameTime gameTime)
        {
            // Moves the bullet
            Position += Velocity;

            // Accelerates the bullet
            Velocity += Acceleration;

            // Increases the timer
            Timer += gameTime.ElapsedGameTime.Milliseconds;

            // Determines the rotation
            Rotation = (float)(Math.Atan2(Velocity.Y, Velocity.X));

            if (Type.Properties.Equals(BulletType.BulletProperties.Fire))
            {
                Alpha -= 0.03f;
            }

            if (Type.Equals(level.YellowBullet))
            {
                float newX = (float)Math.Cos((double)Rotation) * 60;
                float newY = (float)Math.Sin((double)Rotation) * 60;
                Texture2D text = level.YellowParticle;
                int r = level.r.Next(3);
                if (r == 0)
                {
                    text = level.RedParticle;
                }

                level.SpawnTrail(5, text, new Vector2(Position.X - newX, Position.Y - newY), Rotation, 0.06f);
            }
            else if (Type.Equals(level.GreenBullet))
            {
                rotTime += MathHelper.Pi / 15.0f;

                /*Imprint im = new Imprint(Position, Type.Texture, new Rectangle(0, 0, Type.Texture.Width, Type.Texture.Height), 1.0f, Rotation, origin);
                level.Imprints.Add(im);*/

                float newX = (float)Math.Cos((double)Rotation) * 20;
                float newY = (float)Math.Sin((double)Rotation) * 20;
                level.SpawnTrail(1, level.GreenParticle, new Vector2(Position.X - newX, Position.Y - newY), Rotation, 0.09f);

                // Helix the bullets
                float xChange = (float)(Math.Cos(Rotation + MathHelper.Pi / 2)) * (float)Math.Sin(rotTime) * 1.5f;
                float yChange = (float)(Math.Sin(Rotation + MathHelper.Pi / 2)) * (float)Math.Sin(rotTime) * 1.5f;

                Position.X += xChange;
                Position.Y += yChange;
            }
            else if (Type.Equals(level.RedBullet))
            {
                float newX = (float)Math.Cos((double)Rotation) * 0;
                float newY = (float)Math.Sin((double)Rotation) * 0;
                level.SpawnTrail(1, level.RedParticle, new Vector2(Position.X - newX, Position.Y - newY), Rotation, 0.06f);
            }
            else if (Type.Equals(level.BlueBullet))
            {
                Imprint im = new Imprint(Position, Type.Texture, new Rectangle(0, 0, Type.Texture.Width, Type.Texture.Height), 1.0f, Rotation, origin);
                level.Imprints.Add(im);
            }

            // Spawn a particle
            ParticleSpawnTime += gameTime.ElapsedGameTime.Milliseconds;
            if(ParticleSpawnTime > MaxSpawnTime && Type.Equals(level.GreenBullet))
            {
                level.SpawnTrailingParticle(this);
                ParticleSpawnTime = 0;
            }
        }

        // Draws the bullet onscreen
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Animation.Draw(spriteBatch, gameTime, Position, new Color(BulletColor, Alpha), Rotation, origin, Scale);
        }

        // Gets the rectangle surrounding this bullet
        public Rectangle GetRectangle()
        {
            return new Rectangle((int)(Position.X - (Type.Texture.Width / 2)), (int)(Position.Y - (Type.Texture.Height / 2)), (int)Type.Texture.Width, (int)Type.Texture.Height);
        }

        // Get rotated rectangle
        public RotatedRectangle GetRotatedRectangle()
        {
            return new RotatedRectangle(GetRectangle(), Rotation);
        }
    }
}
