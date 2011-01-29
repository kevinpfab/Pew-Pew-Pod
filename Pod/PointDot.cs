using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pod
{
    public class PointDot
    {

        // The texture of the point dot
        public Texture2D Texture;

        // The position of the point dot
        public Vector2 Position;

        // The velocity of the point dot
        public Vector2 Velocity;

        // The amount of points this dot is worth
        public int Points;

        // The rotation of the point dot
        public float Rotation;

        // The alpha of the point dot
        public float Alpha;

        // The origin of the point dot
        public Vector2 Origin;

        // The time alive for this dot
        public float TimeAlive;

        // The attracting pod
        public Pod AttractingPod;

        public PointDot(Texture2D texture, Vector2 position, int points)
        {
            this.Texture = texture;
            this.Position = position;
            this.Points = points;

            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

            // No velocity
            Velocity = Vector2.Zero;
            TimeAlive = 0;
            Alpha = 1.0f;
        }

        // Updates the point dot
        public void Update(GameTime gameTime)
        {
            // Moves the point dot
            if (AttractingPod == null)
            {
                Position += Velocity;
            }
            else
            {
                Position += (AttractingPod.Position - Position) / 4.0f;

                if (AttractingPod.IsRespawning || Vector2.Distance(AttractingPod.Position, Position) > 50)
                {
                    AttractingPod = null;
                }
            }

            // Rotates the point dot
            Rotation += (MathHelper.Pi / 100) * Velocity.X;

            TimeAlive += gameTime.ElapsedGameTime.Milliseconds;

            if (TimeAlive > 4000)
            {
                Alpha -= 0.05f;
            }

        }

        // Draws the point dot
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, Rotation, Origin, Alpha, SpriteEffects.None, 0);
        }

        //  Gets the rectangle for this point dot
        public Rectangle GetRectangle()
        {
            return new Rectangle((int)(Position.X - Origin.X + 8), (int)(Position.Y - Origin.Y + 8), (int)(Texture.Width - 8), (int)(Texture.Height - 8));
        }
    }
}
