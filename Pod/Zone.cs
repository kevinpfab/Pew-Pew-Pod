using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pod
{
    public class Zone
    {
        // The unlit texture for this zone
        public Texture2D UnlitZone;

        // The lit texture for this zone
        public Texture2D LitZone;

        // The current scale of the zone
        public float Scale;

        // The Position of the lit texture
        public Vector2 Position;

        // Origin
        public Vector2 Origin;

        // The width of the texture
        public float Width;

        // The height of the texture
        public float Height;

        // Are we currently shrinking?
        public bool Shrinking;

        // Is this zone currently lit?
        public bool IsLit;

        // The current rotation of the zone
        public float Rotation;
        
        // The current outside rotation of the zone
        public float OutsideRotation;

        // The timers for the zone
        public float LifeTime;
        public float Timer;

        public Zone(Vector2 pos)
        {

            // The scale of the texture
            this.Scale = 0;

            // the position of the pos
            this.Position = pos;

            this.Width = Level.Global.WhiteZoneLit.Width;
            this.Height = Level.Global.WhiteZoneLit.Height;

            Origin = new Vector2(Level.Global.WhiteZoneLit.Width / 2, Level.Global.WhiteZoneLit.Height / 2);

            Rotation = 0;
            OutsideRotation = 0;

            Shrinking = false;
            IsLit = false;

            LifeTime = 5000;
        }

        // Updates this zone
        public void Update(GameTime gameTime)
        {
            if (Shrinking && Scale > 0)
            {
                Scale -= 0.01f;
            }
            else
            {
                if (Scale < 1.0f)
                {
                    Scale += 0.01f;
                }
            }

            if (IsLit)
            {
                Rotation += MathHelper.Pi / 200;
                OutsideRotation -= MathHelper.Pi / 200;
            }


            // Handle the timing of the zones
            Timer += gameTime.ElapsedGameTime.Milliseconds;

            if (Timer > LifeTime)
            {
                Shrinking = true;
            }
        }

        // Draws this zone
        public void Draw(SpriteBatch spriteBatch)
        {
            if (Level.Global.Pods.Count == 1)
            {
                if (Level.Global.Pods.ElementAt(0).Gun.Equals(Level.Global.Red))
                {
                    if (IsLit)
                    {
                        spriteBatch.Draw(Level.Global.RedZoneLit, Position, null, Color.White, Rotation, Origin, Scale, SpriteEffects.None, 0);
                        spriteBatch.Draw(Level.Global.RedZoneLitOutside, Position, null, Color.White, OutsideRotation, Origin, Scale, SpriteEffects.None, 0);
                    }
                    else
                    {
                        spriteBatch.Draw(Level.Global.RedZoneUnlit, Position, null, Color.White, Rotation, Origin, Scale, SpriteEffects.None, 0);
                        spriteBatch.Draw(Level.Global.RedZoneUnlitOutside, Position, null, Color.White, OutsideRotation, Origin, Scale, SpriteEffects.None, 0);
                    }
                }
                else if (Level.Global.Pods.ElementAt(0).Gun.Equals(Level.Global.Blue))
                {
                    if (IsLit)
                    {
                        spriteBatch.Draw(Level.Global.BlueZoneLit, Position, null, Color.White, Rotation, Origin, Scale, SpriteEffects.None, 0);
                        spriteBatch.Draw(Level.Global.BlueZoneLitOutside, Position, null, Color.White, OutsideRotation, Origin, Scale, SpriteEffects.None, 0);
                    }
                    else
                    {
                        spriteBatch.Draw(Level.Global.BlueZoneUnlit, Position, null, Color.White, Rotation, Origin, Scale, SpriteEffects.None, 0);
                        spriteBatch.Draw(Level.Global.BlueZoneUnlitOutside, Position, null, Color.White, OutsideRotation, Origin, Scale, SpriteEffects.None, 0);
                    }
                }
                else if (Level.Global.Pods.ElementAt(0).Gun.Equals(Level.Global.Yellow))
                {
                    if (IsLit)
                    {
                        spriteBatch.Draw(Level.Global.YellowZoneLit, Position, null, Color.White, Rotation, Origin, Scale, SpriteEffects.None, 0);
                        spriteBatch.Draw(Level.Global.YellowZoneLitOutside, Position, null, Color.White, OutsideRotation, Origin, Scale, SpriteEffects.None, 0);
                    }
                    else
                    {
                        spriteBatch.Draw(Level.Global.YellowZoneUnlit, Position, null, Color.White, Rotation, Origin, Scale, SpriteEffects.None, 0);
                        spriteBatch.Draw(Level.Global.YellowZoneUnlitOutside, Position, null, Color.White, OutsideRotation, Origin, Scale, SpriteEffects.None, 0);
                    }
                }
                else if (Level.Global.Pods.ElementAt(0).Gun.Equals(Level.Global.Green))
                {
                    if (IsLit)
                    {
                        spriteBatch.Draw(Level.Global.GreenZoneLit, Position, null, Color.White, Rotation, Origin, Scale, SpriteEffects.None, 0);
                        spriteBatch.Draw(Level.Global.GreenZoneLitOutside, Position, null, Color.White, OutsideRotation, Origin, Scale, SpriteEffects.None, 0);
                    }
                    else
                    {
                        spriteBatch.Draw(Level.Global.GreenZoneUnlit, Position, null, Color.White, Rotation, Origin, Scale, SpriteEffects.None, 0);
                        spriteBatch.Draw(Level.Global.GreenZoneUnlitOutside, Position, null, Color.White, OutsideRotation, Origin, Scale, SpriteEffects.None, 0);
                    }
                }
            }
            else
            {
                if (IsLit)
                {
                    spriteBatch.Draw(Level.Global.WhiteZoneLit, Position, null, Color.White, Rotation, Origin, Scale, SpriteEffects.None, 0);
                    spriteBatch.Draw(Level.Global.WhiteZoneLitOutside, Position, null, Color.White, OutsideRotation, Origin, Scale, SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.Draw(Level.Global.WhiteZoneUnlit, Position, null, Color.White, Rotation, Origin, Scale, SpriteEffects.None, 0);
                    spriteBatch.Draw(Level.Global.WhiteZoneUnlitOutside, Position, null, Color.White, OutsideRotation, Origin, Scale, SpriteEffects.None, 0);
                }
            }

        }

        // Gets the rectangle of the zone
        public Rectangle GetRectangle()
        {
            return new Rectangle((int)(Position.X + (85 * Scale) - (Level.Global.WhiteZoneLit.Width / 2 * Scale)), (int)(Position.Y + (85 * Scale) - (Level.Global.WhiteZoneLit.Height / 2 * Scale)), (int)(Width * Scale - (150 * Scale)), (int)(Height * Scale - (150 * Scale)));
        }

    }
}
