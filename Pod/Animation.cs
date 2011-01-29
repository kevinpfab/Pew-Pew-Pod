using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pod
{
    public class Animation
    {
        // The texture that holds the animation
        public Texture2D Texture;

        // The width of each frame in the animation
        public float FrameWidth;

        // The time between each frame (milliseconds)
        public float FrameTime;

        // The time we are currently at
        public float Time;

        // The current frame of this animation
        public int Frame;

        // The number of frames
        public int NumFrames;

        // Are we paused?
        public bool Paused;

        public Animation(Texture2D texture, float frameWidth, float frameTime)
        {
            this.Texture = texture;
            this.FrameWidth = frameWidth;
            this.FrameTime = frameTime;

            NumFrames = (int)(Texture.Width / FrameWidth) - 1;

            Frame = 0;
            Time = 0;

            Paused = false;
        }

        // Draws the animation
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 Position, Color color, float Rotation, Vector2 Origin, float scale)
        {
            // Don't update if we are paused
            if (!Paused && !Game1.Global.gameState.Equals(Game1.GameState.Paused))
            {
                Time += gameTime.ElapsedGameTime.Milliseconds;

                if (Time >= FrameTime)
                {
                    Frame--;
                    Time = 0;
                }

                if (Frame < 0)
                {
                    Frame = NumFrames;
                }
            }

            Rectangle drawRectangle = new Rectangle((int)(Frame * FrameWidth), 0, (int)FrameWidth, (int)Texture.Height);

            spriteBatch.Draw(Texture, Position, drawRectangle, color, Rotation, Origin, scale, SpriteEffects.None, 0);
        }

        // Gets the origin of the texture frame
        public Vector2 GetOrigin()
        {
            return new Vector2(FrameWidth / 2, Texture.Height / 2);
        }

        // Gets the rectangle of this animation
        public Rectangle GetRectangle()
        {
            return new Rectangle((int)(Frame * FrameWidth), 0, (int)FrameWidth, (int)Texture.Height);
        }
    }
}
