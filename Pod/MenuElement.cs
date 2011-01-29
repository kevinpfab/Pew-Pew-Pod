using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pod
{
    public class MenuElement
    {
        // The texture of the element
        public Texture2D Texture;

        // The Position of the element
        public Vector2 Position;

        // The origin of the elemnt
        public Vector2 Origin;

        public MenuElement(Texture2D Texture)
        {
            this.Texture = Texture;

            Position = Vector2.Zero;

            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
        }

        // Draw the element
        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(Texture, Position, color);
        }

        // Draws the elemtn in a new position
        public void Draw(SpriteBatch spriteBatch, Color color, Vector2 p)
        {
            spriteBatch.Draw(Texture, p, color);
        }

        // Draws the element in a new position and new scale
        public void Draw(SpriteBatch spriteBatch, Color color, Vector2 p, float scale)
        {
            spriteBatch.Draw(Texture, p, null, color, 0, Origin, scale, SpriteEffects.None, 0);
        }
    }
}
