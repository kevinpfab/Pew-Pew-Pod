using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pod
{
    public class Powerup
    {

        // The type of powerup this is
        public enum PowerupType
        {
            FourWayShooter,
            Invincible,
            FasterShots
        }

        // The type of powerup this specific powerup is
        public PowerupType Type;

        // The position of the powerup
        public Vector2 Position;

        // The texture of this powerup
        public Texture2D Texture;
        public Texture2D SpinTexture;

        // The level this powerup is in
        private Level level;

        // The rotation of the spinning texture
        public float Rotation;

        // The origin of this powerup
        public Vector2 Origin;

        // Random generator for this powerup
        private Random r;

        // the scale of the powerup
        public float Scale;

        public Powerup(Vector2 Pos, Texture2D texture, PowerupType type, Level l)
        {
            this.Position = Pos;
            this.Texture = texture;
            this.Type = type;

            this.level = l;

            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

            Scale = 0.75f;

            r = new Random();
        }

        // Updates the powerup
        public void Update(GameTime gameTime)
        {
            /*Particle p = null;
            if (level.ParticlePool.Count > 0)
            {
                p = level.ParticlePool.Pop();
                p.Reset();
                p.Texture = level.BlueParticle;
                p.Position = Position;
            }
            else
            {
                p = new Particle(level.BlueParticle, Position);
            }
            p.FadeDecrease = 0.03f;
            p.LightFadeParticle = true;

            float rot = (float)(r.Next(628)) / 10.0f;

            p.Velocity.X = 1.0f * (float)Math.Cos(rot);
            p.Velocity.Y = 1.0f * (float)Math.Sin(rot);

            level.Particles.Add(p);*/

            Rotation += MathHelper.Pi / 120;
        }

        // Draws the powerup
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(SpinTexture, Position, null, Color.White, Rotation, Origin, Scale, SpriteEffects.None, 0);
            spriteBatch.Draw(Texture, Position, null, Color.White, 0, Origin, Scale, SpriteEffects.None, 0);
        }

        // Gets the hit rectangle of the powerup
        public Rectangle GetRectangle()
        {
            return new Rectangle((int)(Position.X - (Origin.X * Scale)), (int)(Position.Y - (Origin.Y * Scale)), (int)(Texture.Width * Scale), (int)(Texture.Height * Scale));
        }
    }
}
