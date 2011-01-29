using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pod
{
    public class BackgroundStar
    {

        // The texture to draw the star with
        private Texture2D texture;

        // The position of the star
        public Vector2 Position;

        // The velocity of this background star
        public Vector2 Velocity;

        // The occillate value
        private float fade;

        // The current alpha of the star
        private float Alpha;

        // The current scale of the star
        public float Scale;

        // Origin
        public Vector2 Origin;

        public BackgroundStar(Texture2D texture, Vector2 Pos, float fade, float startAlpha)
        {
            this.texture = texture;
            this.Position = Pos;
            this.fade = fade;
            this.Alpha = startAlpha;

            Scale = 1.0f;

            Velocity = Vector2.Zero;
            Origin = new Vector2(texture.Width / 2, texture.Height / 2);
        }

        // Fades the star in and out (twinkle, twinkle, little star)
        public void Update()
        {
            if (Alpha <= 0 || Alpha >= 1.0f)
            {
                fade *= -1;
            }

            Alpha += fade;

            Position += Velocity;

            Velocity *= 0.98f;

            /*if (Scale > 1.0f)
            {
                Scale -= 0.1f;
            }*/
        }

        // Draws the star
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, new Color(Color.White, Alpha), 0, Origin, Scale, SpriteEffects.None, 0);
        }
    }
}
