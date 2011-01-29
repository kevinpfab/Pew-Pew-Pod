using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pod
{
    public class TravelPoint
    {

        // The texture of the travel point
        public Texture2D Texture;

        // The Position of the travel point
        public Vector2 Position;

        // The rectangle of the Travel Point
        public Rectangle Rectangle;

        // the level this point belongs to
        public Level level;

        // The rotation of the iniside
        public float Rotation;
        public float RotationOutside;


        // The origin
        public Vector2 Origin;

        // Random generator
        private Random r;

        public TravelPoint(Level level)
        {
            this.level = level;

            Origin = new Vector2(level.WhitePoint.Width / 2.0f, level.WhitePoint.Height / 2.0f);

            r = new Random();

            Position = Vector2.Zero;
        }

        // Sets the point to a random position onscreen
        public void RandomPosition()
        {
            int ranX = r.Next((int)(level.GameRectangle.Left + Origin.X), (int)(level.GameRectangle.Right - Origin.X));
            int ranY = r.Next((int)(level.GameRectangle.Top + Origin.Y + 100), (int)(level.GameRectangle.Bottom - Origin.Y));
            Position = new Vector2(ranX, ranY);

            level.SpawnExplosion(level.Particles, 100, Position, level.YellowBullet, 3, 5, 0.005f);
            
            Rectangle = new Rectangle((int)(Position.X - Origin.X), (int)(Position.Y - Origin.Y), (int)level.WhitePoint.Width, (int)level.WhitePoint.Height);
        }

        // Sets the point to a random position onscreen
        public void RandomPositionNoExplosion()
        {
            int ranX = r.Next((int)(level.GameRectangle.Left + Origin.X), (int)(level.GameRectangle.Right - Origin.X));
            int ranY = r.Next((int)(level.GameRectangle.Top + Origin.Y + 100), (int)(level.GameRectangle.Bottom - Origin.Y));
            Position = new Vector2(ranX, ranY);

            Rectangle = new Rectangle((int)(Position.X - Origin.X), (int)(Position.Y - Origin.Y), (int)level.WhitePoint.Width, (int)level.WhitePoint.Height);
        }

        // Draws the travel point
        public void Draw(SpriteBatch spriteBatch)
        {
            // Increase rotations
            if (!Game1.Global.gameState.Equals(Game1.GameState.Paused))
            {
                Rotation += MathHelper.Pi / 200;
                RotationOutside -= MathHelper.Pi / 200;
            }

            if (level.Pods.Count == 1)
            {
                if (level.Pods.ElementAt(0).Gun.Equals(level.Red))
                {
                    spriteBatch.Draw(level.RedPoint, Position, null, Color.White, Rotation, Origin, 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(level.RedPointOutside, Position, null, Color.White, RotationOutside, Origin, 1.0f, SpriteEffects.None, 0);
                }
                else if (level.Pods.ElementAt(0).Gun.Equals(level.Blue))
                {
                    spriteBatch.Draw(level.BluePoint, Position, null, Color.White, Rotation, Origin, 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(level.BluePointOutside, Position, null, Color.White, RotationOutside, Origin, 1.0f, SpriteEffects.None, 0);
                }
                else if (level.Pods.ElementAt(0).Gun.Equals(level.Yellow))
                {
                    spriteBatch.Draw(level.YellowPoint, Position, null, Color.White, Rotation, Origin, 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(level.YellowPointOutside, Position, null, Color.White, RotationOutside, Origin, 1.0f, SpriteEffects.None, 0);
                }
                else if (level.Pods.ElementAt(0).Gun.Equals(level.Green))
                {
                    spriteBatch.Draw(level.GreenPoint, Position, null, Color.White, Rotation, Origin, 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(level.GreenPointOutside, Position, null, Color.White, RotationOutside, Origin, 1.0f, SpriteEffects.None, 0);
                }
            }
            else
            {
                spriteBatch.Draw(level.WhitePoint, Position, null, Color.White, Rotation, Origin, 1.0f, SpriteEffects.None, 0);
                spriteBatch.Draw(level.WhitePointOutside, Position, null, Color.White, RotationOutside, Origin, 1.0f, SpriteEffects.None, 0);
            }
        }

    }
}
