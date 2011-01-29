using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pod
{
    public class Imprint
    {

        // The position of the imprint
        public Vector2 Position;

        // The texture of the imprint
        public Texture2D Texture;

        // the source rectangle of the imprint
        public Rectangle SourceRectangle;

        // The Alpha of the imprint
        public float Alpha;

        // The rotation of the imprint
        public float Rotation;

        // The origin of the imprint
        public Vector2 Origin;

        public Imprint(Vector2 Position, Texture2D Texture, Rectangle SourceRectangle, float Alpha, float Rotation, Vector2 Origin)
        {
            this.Position = Position;
            this.Texture = Texture;
            this.SourceRectangle = SourceRectangle;
            this.Alpha = Alpha;
            this.Rotation = Rotation;
            this.Origin = Origin;
        }

        // Updates the imprint
        public void Update()
        {
            Alpha -= 0.15f;
        }

        // Draws the imprint
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, SourceRectangle, new Color(Color.White, Alpha), Rotation, Origin, 1.0f, SpriteEffects.None, 0);
        }
    }
}
