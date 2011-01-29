using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pod
{
    public class EnemyType
    {
        // The texture of this enemy type
        public Texture2D Texture
        {
            get { return texture; }
        }
        private Texture2D texture;

        // The Base Velocity of the enemy
        public float Velocity
        {
            get { return velocity; }
        }
        private float velocity;

        // The score this enemytype is worth
        public int Score
        {
            get { return score; }
        }
        private int score;

        // The number of particles that this enemy will explode
        public int ParticleNumber;

        public enum EnemyColor
        {
            Blue,
            Red,
            Green,
            Yellow,
            None
        }

        public enum EnemyBehavior
        {
            MoveTowardsPlayer,
            StraightLine,
            Float,
            DashTowardsPlayer,
            Snake
        }
        public EnemyBehavior Behavior;

        public EnemyType(Texture2D texture, float velocity, int score, int pn, EnemyBehavior beh)
        {
            this.texture = texture;
            this.velocity = velocity;
            this.score = score;
            this.ParticleNumber = pn;
            this.Behavior = beh;
        }

        // Gets the origin of the enemy texture
        public Vector2 GetOrigin()
        {
            return new Vector2(Texture.Width / 2, Texture.Height / 2);
        }
    }
}
