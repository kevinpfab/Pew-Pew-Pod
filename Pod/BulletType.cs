using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Pod
{
    public class BulletType
    {
        // Properties a bullet can apply
        public enum BulletProperties
        {
            Ice,
            Fire,
            Lightning,
            Bounce,
            None
        }

        // The properties this bullet can apply
        public BulletProperties Properties
        {
            get { return properties; }
        }
        private BulletProperties properties;

        // The Texture of the BulletType
        public Texture2D Texture
        {
            get { return texture; }
        }
        private Texture2D texture;

        // The Framerate of this bullet
        public float FrameRate;

        // The Frame Width of this bullet
        public float FrameWidth;

        // The damage this bullet does
        public float Damage
        {
            get { return damage; }
        }
        private float damage;

        // The default velocity this bullet travels at
        public float Velocity
        {
            get { return defaultVelocity; }
        }
        private float defaultVelocity;

        // The Amount of Ammo this bullet consumes
        public int AmmoCost
        {
            get { return ammoCost; }
        }
        private int ammoCost;

        public BulletType(Texture2D texture, float damage, int ammoCost, BulletProperties bProp, float defaultVelocity, float frameWidth, float frameRate)
        {
            this.texture = texture;
            this.damage = damage;
            this.properties = bProp;
            this.defaultVelocity = defaultVelocity;
            this.ammoCost = ammoCost;
            this.FrameRate = frameRate;
            this.FrameWidth = frameWidth;
        }
    }
}
