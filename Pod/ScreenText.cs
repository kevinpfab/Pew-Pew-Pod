using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pod
{
    public class ScreenText
    {

        // The spritefont to draw the text with
        private SpriteFont font;

        // The location of the text to draw
        private Vector2 position;

        // The text to write
        private string text;

        // The current alpha of the text
        public float Alpha;

        // the current scale of the text
        public float Scale;

        public ScreenText(SpriteFont font, Vector2 position, string text)
        {
            this.font = font;
            this.position = position;
            this.text = text;

            Alpha = 1.0f;
            Scale = 2.0f;
        }

        // Updates the text
        public void Update()
        {
            // Alpha -= 0.04f;
            Scale -= 0.04f;
            if (Scale <= 0)
            {
                Alpha = 0;
            }
        }

        // Draws the text
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, text, position, new Color(Color.White, 0.5f), 0, font.MeasureString(text) / 2, Scale + 0.1f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, text, position, new Color(Color.Green, Alpha), 0, font.MeasureString(text) / 2, Scale, SpriteEffects.None, 0);
        }
    }
}
