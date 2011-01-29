using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pod
{
    public class Gun
    {

        // The texture of this gun
        public Texture2D Texture
        {
            get { return texture; }
        }
        private Texture2D texture;
        // The origin of this texture
        public Vector2 Origin;

        // The gun texture
        public Texture2D GunTexture;
        // The origin of the gun texture
        public Vector2 GunOrigin;

        // The fire rate of this gun (in milliseconds)
        public float FireRate;

        // The type of bullet used in this gun
        public BulletType Bullet
        {
            get { return bullet; }
        }
        private BulletType bullet;

        // The Color related to this gun
        public Color Color;

        public Gun(Texture2D texture, float fireRate, BulletType bullet)
        {
            this.texture = texture;
            this.FireRate = fireRate;
            this.bullet = bullet;

            Origin = new Vector2(texture.Width / 2, texture.Height / 2);
            GunOrigin = new Vector2(15, 15);
        }
    }
}
