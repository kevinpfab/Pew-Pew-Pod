using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Pod
{
    public class Level
    {

        // Global
        public static Level Global;

        // The type of match we are playing
        public enum MatchType
        {
            SinglePlayer,
            MultiPlayer,
            Versus,
            Travel,
            Arcade,
            Survival,
            Cooperative,
            Zones,
            ThinkFast,
            None
        }
        public MatchType matchType;

        // The List of Pods playing
        public List<Pod> Pods;

        // The number of players playing
        public int PlayerCount;

        // The content manager for the level
        private ContentManager Content;

        // The game's rectangle
        public Rectangle GameRectangle;

        // The four public guns that we can use
        // public Gun Red;
        public Gun Blue;
        public Gun Yellow;
        public Gun Green;
        public Gun Red;

        public BulletType GreenBullet;
        public BulletType BlueBullet;
        public BulletType YellowBullet;
        public BulletType YellowBulletReplaced;
        public BulletType RedBullet;

        // The sound effects of the level
        public SoundEffect GreenLaserSound;
        public SoundEffect RedSound;
        public SoundEffect BlueSound;
        public SoundEffect YellowSound;
        public SoundEffect ExplosionSound;
        public SoundEffect PowerUpSound;
        public SoundEffect ThinkFastWarning;
        public SoundEffect DeathSound;

        // Total distance shooken by screen
        private Vector2 shook;
        private float shakeTime;
        private float TimeToShake;
        public bool IsShaking;

        // The 4 background boxes onscreen
        private Texture2D backgroundBoxTexture;
        private Color topLeft;
        private Color topRight;
        private Color bottomLeft;
        private Color bottomRight;

        // Score submission
        public bool ScoreSubmitted;

        // The little point dots you can pick up
        public List<PointDot> PointDots;
        public Texture2D PointDotTexture;

        // The enemies in the game
        public EnemyType FollowEnemy;
        public EnemyType DashEnemy;
        public EnemyType FloatEnemy;
        public EnemyType ClusterEnemy;
        public EnemyType SnakeEnemy;
        public Texture2D EnemySpawnTexture;
        public Vector2 EnemySpawnOrigin;

        // The enemy objects in the game
        public List<Enemy> Enemies;

        public Song gameSong;

        // Wave information
        public int wave;
        public bool spawningEnemies;

        // The time left before the game ends
        public float timer;

        // Particle information
        public List<Particle> Particles;
        public Stack<Particle> ParticlePool;
        public List<Particle> PulseParticles;
        public List<Particle> TrailParticles;
        public List<Particle> LiquidParticles;
        public Texture2D RegularParticle;
        public Texture2D RedParticle;
        public Texture2D BlueParticle;
        public Texture2D YellowParticle;
        public Texture2D GreenParticle;
        public Texture2D PulseTexture;
        public Texture2D RedExplosionParticle;
        public Texture2D BlueExplosionParticle;
        public Texture2D YellowExplosionParticle;
        public Texture2D YellowGunExplosionParticle;
        public Texture2D GreenExplosionParticle;
        public Texture2D RedLight;
        public Texture2D BlueLight;
        public Texture2D YellowLight;
        public Texture2D GreenLight;

        public SpriteFont gameFont;
        public SpriteFont gameFontLarge;
        public SpriteFont gameFontExtraLarge;

        // Background texture
        private Texture2D BackgroundTexure;
        private float BackgroundRotation;
        public float BackgroundFade;
        private bool BackgroundFadingIn;
        private List<BackgroundStar> BackgroundStars;
        private List<BackgroundStar> Layer1;
        private List<BackgroundStar> Layer2;
        private List<BackgroundStar> Layer3;

        // The enemy bullets
        public BulletType EnemyBullet;
        public List<Bullet> EnemyBullets;

        // The Think Fast variables
        public string CurrentColor;
        public float TimeToSwitch;
        public float CurrentTime;
        public float ColorFlashScale;
        public float ColorFlashAlpha;
        public Texture2D CurrentThinkFastTexture;
        public Texture2D CurrentThinkFastFlashTexture;
        public Vector2 ThinkFastIconOrigin;

        // Border texture
        private Vector2 BorderPosition;
        public bool BorderShowing;
        private float BorderAlpha;

        // Letters that appear onscreen
        public List<ScreenText> Text;

        // The imprints onscreen that fade away
        public List<Imprint> Imprints;

        // Powerups currently onscreen
        public List<Powerup> Powerups;
        public Texture2D FastShoot;
        public Texture2D FastShootSpin;
        public Texture2D FourWayShoot;
        public Texture2D FourWayShootSpin;
        public Texture2D OneUp;
        public Texture2D OneUpSpin;

        // Helper lists (for removing objects)
        private List<Bullet> RemoveBulletList;
        private List<Particle> RemoveParticleList;
        private List<Enemy> RemoveEnemyList;

        // Random number generator
        public Random r;

        // Has our explosion sound played?
        public bool ExplosionSoundHasPlayed;

        // Game Level variables
        public float TotalSeconds;
        public int EnemyCount;
        public int MaxEnemyCount;

        #region Zone Variables
        // the zone onscreen
        public Zone zone;

        // The zone circles
        public Texture2D WhiteZoneLit;
        public Texture2D WhiteZoneUnlit;
        public Texture2D WhiteZoneLitOutside;
        public Texture2D WhiteZoneUnlitOutside;
        public Texture2D RedZoneUnlit;
        public Texture2D RedZoneUnlitOutside;
        public Texture2D RedZoneLit;
        public Texture2D RedZoneLitOutside;
        public Texture2D BlueZoneUnlit;
        public Texture2D BlueZoneUnlitOutside;
        public Texture2D BlueZoneLit;
        public Texture2D BlueZoneLitOutside;
        public Texture2D YellowZoneLit;
        public Texture2D YellowZoneLitOutside;
        public Texture2D YellowZoneUnlit;
        public Texture2D YellowZoneUnlitOutside;
        public Texture2D GreenZoneLit;
        public Texture2D GreenZoneLitOutside;
        public Texture2D GreenZoneUnlit;
        public Texture2D GreenZoneUnlitOutside;
        #endregion

        #region Travel Point Variables
        // the point we try and reach
        public TravelPoint tPoint;
        // The texture of the travel point
        public Texture2D WhitePoint;
        public Texture2D WhitePointOutside;
        public Texture2D RedPoint;
        public Texture2D RedPointOutside;
        public Texture2D BluePoint;
        public Texture2D BluePointOutside;
        public Texture2D YellowPoint;
        public Texture2D YellowPointOutside;
        public Texture2D GreenPoint;
        public Texture2D GreenPointOutside;
        #endregion

        #region Survival Variables
        public float SurvivalTimer;
        #endregion

        // Is there a game over?
        public bool GameOver;
        public bool MatchEnded;
        public bool ReturnToMenu;

        // Fps testing
        int _total_frames = 0;
        float _elapsed_time = 0.0f;
        int _fps = 0;

        // Positions of other elements on the screen
        public Vector2 TimePosition;
        public Vector2 HUDPosition;
        public Vector2 PodHUDPosition;

        // Mediaplayer visualization data
        VisualizationData MusicData;
        float[] LastMusicData;
        Texture2D BlankPixel;

        // HUD Textures
        public Texture2D RedHudTime;
        public Texture2D RedHudNoTime;
        public Texture2D YellowHudTime;
        public Texture2D YellowHudNoTime;
        public Texture2D BlueHudTime;
        public Texture2D BlueHudNoTime;
        public Texture2D GreenHudTime;
        public Texture2D GreenHudNoTime;
        public Texture2D TwoPlayerHudTime;
        public Texture2D TwoPlayerHudNoTime;
        public Texture2D ThreePlayerHudTime;
        public Texture2D ThreePlayerHudNoTime;
        public Texture2D FourPlayerHudTime;
        public Texture2D FourPlayerHudNoTime;
        
        // Think Fast
        public Texture2D BlueIcon;
        public Texture2D BlueMode;
        public Texture2D DontMoveMode;
        public Texture2D DontShootMode;
        public Texture2D GreenIcon;
        public Texture2D GreenMode;
        public Texture2D RedIcon;
        public Texture2D RedMode;
        public Texture2D StopMIcon;
        public Texture2D StopSIcon;
        public Texture2D YellowIcon;
        public Texture2D YellowMode;
        public Vector2 ThinkFastOrigin;
        public Vector2 ThinkFastIconPoint;

        // Highscores
        public Texture2D LinesHighscoresLevel;

        // Highscore screen
        public HighscoreComponent hsc_;

        public Level(ContentManager content)
        {
            this.Content = content;

            matchType = MatchType.None;

            // Set this level equal to this level
            Global = this;
        }

        // Initializes variables for the level
        // Loads textures used in the level
        public void Initialize()
        {
            StartReset();

            // Still playing
            GameOver = false;
            MatchEnded = false;
            ReturnToMenu = false;

            // Creates the list for the player pods
            Pods = new List<Pod>();

            // Random generator
            r = new Random();

            // The game rectangle
            GameRectangle = new Rectangle(128, 72, 1024, 576);

            // Highscores
            hsc_ = HighscoreComponent.Find<IGameComponent>(Game1.Global.Components, Game1.FilterHighscore) as HighscoreComponent;
            if (hsc_ == null)
                throw new InvalidOperationException("You must have initialized the highscore component to use HighscoreScreen.");
            // hsc_.HighscoresChanged += new HighscoresChanged(Game1.hsc_HighscoresChanged);

            MusicData = new VisualizationData();
            LastMusicData = new float[256];

            // Waves
            wave = 0;
            spawningEnemies = false;


            Particles = new List<Particle>();
            ParticlePool = new Stack<Particle>();
            PulseParticles = new List<Particle>();
            TrailParticles = new List<Particle>();
            LiquidParticles = new List<Particle>();

            // Border texture
            BorderPosition = Vector2.Zero;
            BorderAlpha = 1.0f;
            BorderShowing = false;

            // Helpers
            RemoveBulletList = new List<Bullet>();
            RemoveEnemyList = new List<Enemy>();
            RemoveParticleList = new List<Particle>();

            // SpawnLiquidParticles(25);

            // The text that is onscreen
            Text = new List<ScreenText>();

            // Imprints
            Imprints = new List<Imprint>();

            // Keeps track of global time variables
            TotalSeconds = 0;
            EnemyCount = 2;

            // Random element positions
            TimePosition = new Vector2(622, 72);
            HUDPosition = Vector2.Zero;
            PodHUDPosition = new Vector2(135, 72);

            BackgroundFade = 1.0f;
        }

        // Resets the background
        public void ResetBackground()
        {
            BackgroundTexure = Content.Load<Texture2D>("Graphics/Backgrounds/BG" + (r.Next(3) + 1).ToString());
            BackgroundRotation = 0;

            BackgroundStars = new List<BackgroundStar>();
            LiquidParticles.Clear();
            Layer1 = new List<BackgroundStar>();
            Layer2 = new List<BackgroundStar>();
            Layer3 = new List<BackgroundStar>();
            SpawnBackground(200);
            Game1.Global.SpawnBackgroundLayer(200, Layer1);
            Game1.Global.SpawnBackgroundLayer(200, Layer2);
            Game1.Global.SpawnBackgroundLayer(200, Layer3);

            Game1.Global.SpawnPlanets(LiquidParticles, r.Next(2) + 1);
            Game1.Global.SpawnPlanets(LiquidParticles, r.Next(2));
        }

        // Resets all the variables that need resetting
        public void Reset()
        {
            TotalSeconds = 0;
            EnemyCount = 2;
            MaxEnemyCount = 30;
            GameOver = false;
            MatchEnded = false;
            ReturnToMenu = false;

            timer = 180000;

            if (matchType.Equals(MatchType.Versus))
            {
                timer = 60000;
            }

            ScoreSubmitted = false;

            // Think fast
            CurrentTime = 0;
            TimeToSwitch = 0;
            ColorFlashScale = 0;
            ColorFlashAlpha = 0;
            int ran = r.Next(6);
            if (ran == 0)
            {
                CurrentColor = "Green";
                CurrentThinkFastTexture = GreenIcon;
                CurrentThinkFastFlashTexture = GreenMode;
            }
            else if (ran == 1)
            {
                CurrentColor = "Yellow";
                CurrentThinkFastTexture = YellowIcon;
                CurrentThinkFastFlashTexture = YellowMode;
            }
            else if (ran == 2)
            {
                CurrentColor = "Red";
                CurrentThinkFastTexture = RedIcon;
                CurrentThinkFastFlashTexture = RedMode;
            }
            else if (ran == 3)
            {
                CurrentColor = "Blue";
                CurrentThinkFastTexture = BlueIcon;
                CurrentThinkFastFlashTexture = BlueMode;
            }
            else if (ran == 4)
            {
                CurrentColor = "Don't Shoot!";
                CurrentThinkFastTexture = StopSIcon;
                CurrentThinkFastFlashTexture = DontShootMode;
            }
            else if (ran == 5)
            {
                CurrentColor = "Don't Move!";
                CurrentThinkFastTexture = StopMIcon;
                CurrentThinkFastFlashTexture = DontMoveMode;
            }

            // Choose a random song
            int ranSong = r.Next(5);
            if (ranSong == 0)
            {
                gameSong = Content.Load<Song>("Music/Adventures_in_Explosions_full_mix");
            }
            else if (ranSong == 1)
            {
                gameSong = Content.Load<Song>("Music/Crash_Test_full_mix");
            }
            else if (ranSong == 2)
            {
                gameSong = Content.Load<Song>("Music/Edged_Out_120_sec");
            }
            else if (ranSong == 3)
            {
                gameSong = Content.Load<Song>("Music/Head_Up_120_sec");
            }
            else if (ranSong == 4)
            {
                gameSong = Content.Load<Song>("Music/Modern_Times_120_sec");
            }

            // Survival timers
            SurvivalTimer = 0;

            // Clear all lists
            Particles.Clear();
            TrailParticles.Clear();
            ParticlePool.Clear();

            Enemies = new List<Enemy>();
            Powerups = new List<Powerup>();
            PointDots = new List<PointDot>();
            
            // Explosion sound
            ExplosionSoundHasPlayed = false;

            foreach(Pod p in Pods)
            {
                p.Reset();
            }

            SpawnZone();

            // The travel point gametype
            tPoint = new TravelPoint(this);
            tPoint.RandomPositionNoExplosion();

            // Screen shake
            EndShaking();

            // Reset positions
            int PlayerCount = Pods.Count;
            int i = 0;
            foreach (Pod Player in Pods)
            {
                if (i == 0)
                {
                    if (PlayerCount == 1)
                    {
                        Player.Position = new Vector2(640, 360);
                    }
                    else
                        if (PlayerCount == 2)
                        {
                            Player.Position = new Vector2(640, 310);
                        }
                        else
                        {
                            Player.Position = new Vector2(640, 210);
                        }
                }
                else if (i == 1)
                {
                    if (PlayerCount == 2)
                    {
                        Player.Position = new Vector2(640, 410);
                    }
                    else
                    {
                        Player.Position = new Vector2(640, 310);
                    }
                }
                else if (i == 2)
                {
                    Player.Position = new Vector2(640, 410);
                }
                else if (i == 3)
                {
                    Player.Position = new Vector2(640, 510);
                }

                i++;
            }
        }

        // Start the reset process
        public void StartReset()
        {
            BackgroundFadingIn = true;
            BackgroundFade = 0;
        }

        // Loads all the content for the level
        public void LoadContent()
        {
            // Loads the background boxes
            backgroundBoxTexture = Content.Load<Texture2D>("Graphics/BackgroundBox");
            topRight = topLeft = bottomRight = bottomLeft = Color.LightGreen;

            LinesHighscoresLevel = Content.Load<Texture2D>("Graphics/Menu/LinesHighscoresLevel");

            // Loads each gun into existance
            // Red Gun
            RedBullet = new BulletType(Content.Load<Texture2D>("Graphics/Bullets/RedBullet"), 5.0f, 15, BulletType.BulletProperties.Fire, 6.0f, 25, 0.01f);
            Red = new Gun(Content.Load<Texture2D>("Graphics/Pods/RedShip"), 350.0f, RedBullet);
            Red.GunTexture = Content.Load<Texture2D>("Graphics/Pods/RedWeapon");
            Red.Color = Color.Orange;
            // Yellow Gun
            YellowBullet = new BulletType(Content.Load<Texture2D>("Graphics/Bullets/YellowBullet"), 100.0f, 50, BulletType.BulletProperties.Bounce, 5.0f, 36, 0.02f);
            YellowBulletReplaced = new BulletType(Content.Load<Texture2D>("Graphics/Bullets/YellowBullet"), 100.0f, 50, BulletType.BulletProperties.Bounce, 5.0f, 40, 0);
            Yellow = new Gun(Content.Load<Texture2D>("Graphics/Pods/YellowShip"), 350.0f, YellowBullet);
            Yellow.GunTexture = Content.Load<Texture2D>("Graphics/Pods/YellowWeapon");
            Yellow.Color = Color.LightYellow;
            // Green Gun
            GreenBullet = new BulletType(Content.Load<Texture2D>("Graphics/Bullets/GreenBullet"), 50.0f, 25, BulletType.BulletProperties.None, 10.0f, 38, 0);
            Green = new Gun(Content.Load<Texture2D>("Graphics/Pods/GreenShip"), 125.0f, GreenBullet);
            Green.GunTexture = Content.Load<Texture2D>("Graphics/Pods/GreenWeapon");
            Green.Color = Color.LightGreen;
            // Blue Gun
            BlueBullet = new BulletType(Content.Load<Texture2D>("Graphics/Bullets/BlueBullet"), 20.0f, 12, BulletType.BulletProperties.Ice, 10.0f, 45, 0);
            Blue = new Gun(Content.Load<Texture2D>("Graphics/Pods/BlueShip"), 190.0f, BlueBullet);
            Blue.GunTexture = Content.Load<Texture2D>("Graphics/Pods/BlueWeapon");
            Blue.Color = Color.LightBlue;

            // Loads the various enemies
            FollowEnemy = new EnemyType(Content.Load<Texture2D>("Graphics/Enemies/Follow"), 6.0f, 15, 50, EnemyType.EnemyBehavior.MoveTowardsPlayer);
            DashEnemy = new EnemyType(Content.Load<Texture2D>("Graphics/Enemies/Dash"), 9.0f, 20, 50, EnemyType.EnemyBehavior.DashTowardsPlayer);
            FloatEnemy = new EnemyType(Content.Load<Texture2D>("Graphics/Enemies/Random"), 4.0f, 10, 50, EnemyType.EnemyBehavior.Float);
            ClusterEnemy = new EnemyType(Content.Load<Texture2D>("Graphics/Enemies/Cluster"), 4.0f, 5, 16, EnemyType.EnemyBehavior.MoveTowardsPlayer);
            SnakeEnemy = new EnemyType(Content.Load<Texture2D>("Graphics/Enemies/Snake"), 3.0f, 15, 30, EnemyType.EnemyBehavior.Snake);
            EnemySpawnTexture = Content.Load<Texture2D>("Graphics/Enemies/Spawn");
            EnemySpawnOrigin = new Vector2(100, EnemySpawnTexture.Height / 2);

            // ThinkFast
            BlueIcon = Content.Load<Texture2D>("Graphics/ThinkFast/BlueIcon");
            BlueMode = Content.Load<Texture2D>("Graphics/ThinkFast/BlueMode");
            DontMoveMode = Content.Load<Texture2D>("Graphics/ThinkFast/DontMoveMode");
            DontShootMode = Content.Load<Texture2D>("Graphics/ThinkFast/DontShootMode");
            GreenIcon = Content.Load<Texture2D>("Graphics/ThinkFast/GreenIcon");
            GreenMode = Content.Load<Texture2D>("Graphics/ThinkFast/GreenMode");
            RedIcon = Content.Load<Texture2D>("Graphics/ThinkFast/RedIcon");
            RedMode = Content.Load<Texture2D>("Graphics/ThinkFast/RedMode");
            StopMIcon = Content.Load<Texture2D>("Graphics/ThinkFast/StopSIcon");
            StopSIcon = Content.Load<Texture2D>("Graphics/ThinkFast/StopMIcon");
            YellowIcon = Content.Load<Texture2D>("Graphics/ThinkFast/YellowIcon");
            YellowMode = Content.Load<Texture2D>("Graphics/ThinkFast/YellowMode");
            ThinkFastOrigin = new Vector2(BlueMode.Width / 2, BlueMode.Height / 2);
            ThinkFastIconPoint = new Vector2(640, 85);
            ThinkFastIconOrigin = new Vector2(BlueIcon.Width / 2, BlueIcon.Height / 2);

            // The point dots
            PointDotTexture = Content.Load<Texture2D>("Graphics/Extras/PointDot");

            // The travel points
            WhitePoint = Content.Load<Texture2D>("Graphics/Waypoints/MPTravel2");
            WhitePointOutside = Content.Load<Texture2D>("Graphics/Waypoints/MPTravel1");
            RedPoint = Content.Load<Texture2D>("Graphics/Waypoints/RedTravel2");
            RedPointOutside = Content.Load<Texture2D>("Graphics/Waypoints/RedTravel1");
            BluePoint = Content.Load<Texture2D>("Graphics/Waypoints/BlueTravel2");
            BluePointOutside = Content.Load<Texture2D>("Graphics/Waypoints/BlueTravel1");
            YellowPoint = Content.Load<Texture2D>("Graphics/Waypoints/YellowTravel2");
            YellowPointOutside = Content.Load<Texture2D>("Graphics/Waypoints/YellowTravel1");
            GreenPoint = Content.Load<Texture2D>("Graphics/Waypoints/GreenTravel2");
            GreenPointOutside = Content.Load<Texture2D>("Graphics/Waypoints/GreenTravel1");

            gameFont = Content.Load<SpriteFont>("Fonts/Quartz");
            gameFontLarge = Content.Load<SpriteFont>("Fonts/QuartzLarge");
            gameFontExtraLarge = Content.Load<SpriteFont>("Fonts/QuartzExtraLarge");

            // HUDS
            RedHudTime = Content.Load<Texture2D>("Graphics/HUD/SPTHudRed");
            RedHudNoTime = Content.Load<Texture2D>("Graphics/HUD/SPNTHudRed");
            GreenHudTime = Content.Load<Texture2D>("Graphics/HUD/SPTHudGreen");
            GreenHudNoTime = Content.Load<Texture2D>("Graphics/HUD/SPNTHudGreen");
            BlueHudTime = Content.Load<Texture2D>("Graphics/HUD/SPTHudBlue");
            BlueHudNoTime = Content.Load<Texture2D>("Graphics/HUD/SPNTHudBlue");
            YellowHudTime = Content.Load<Texture2D>("Graphics/HUD/SPTHudYellow");
            YellowHudNoTime = Content.Load<Texture2D>("Graphics/HUD/SPNTHudYellow");
            TwoPlayerHudTime = Content.Load<Texture2D>("Graphics/HUD/MPTHud2");
            TwoPlayerHudNoTime = Content.Load<Texture2D>("Graphics/HUD/MPNTHud2");
            ThreePlayerHudTime = Content.Load<Texture2D>("Graphics/HUD/MPTHud3");
            ThreePlayerHudNoTime = Content.Load<Texture2D>("Graphics/HUD/MPNTHud3");
            FourPlayerHudTime = Content.Load<Texture2D>("Graphics/HUD/MPTHud4");
            FourPlayerHudNoTime = Content.Load<Texture2D>("Graphics/HUD/MPNTHud4");

            // Powerups
            FourWayShoot = Content.Load<Texture2D>("Graphics/Extras/FourWayPU2");
            FourWayShootSpin = Content.Load<Texture2D>("Graphics/Extras/FourWayPU1");
            FastShoot = Content.Load<Texture2D>("Graphics/Extras/SpeedPU2");
            FastShootSpin = Content.Load<Texture2D>("Graphics/Extras/SpeedPU1");
            OneUp = Content.Load<Texture2D>("Graphics/Extras/OneUpPU2");
            OneUpSpin = Content.Load<Texture2D>("Graphics/Extras/OneUpPU1");

            // The zone textures
            WhiteZoneLit = Content.Load<Texture2D>("Graphics/Zones/MPZoneA1");
            WhiteZoneLitOutside = Content.Load<Texture2D>("Graphics/Zones/MPZoneA2");
            WhiteZoneUnlit = Content.Load<Texture2D>("Graphics/Zones/MPZoneIA1");
            WhiteZoneUnlitOutside = Content.Load<Texture2D>("Graphics/Zones/MPZoneIA2");
            BlueZoneLit = Content.Load<Texture2D>("Graphics/Zones/BlueZoneA1");
            BlueZoneLitOutside = Content.Load<Texture2D>("Graphics/Zones/BlueZoneA2");
            BlueZoneUnlit = Content.Load<Texture2D>("Graphics/Zones/BlueZoneIA1");
            BlueZoneUnlitOutside = Content.Load<Texture2D>("Graphics/Zones/BlueZoneIA2");
            RedZoneLit = Content.Load<Texture2D>("Graphics/Zones/RedZoneA1");
            RedZoneLitOutside = Content.Load<Texture2D>("Graphics/Zones/RedZoneA2");
            RedZoneUnlit = Content.Load<Texture2D>("Graphics/Zones/RedZoneIA1");
            RedZoneUnlitOutside = Content.Load<Texture2D>("Graphics/Zones/RedZoneIA2");
            YellowZoneLit = Content.Load<Texture2D>("Graphics/Zones/YellowZoneA1");
            YellowZoneLitOutside = Content.Load<Texture2D>("Graphics/Zones/YellowZoneA2");
            YellowZoneUnlit = Content.Load<Texture2D>("Graphics/Zones/YellowZoneIA1");
            YellowZoneUnlitOutside = Content.Load<Texture2D>("Graphics/Zones/YellowZoneIA2");
            GreenZoneLit = Content.Load<Texture2D>("Graphics/Zones/GreenZoneA1");
            GreenZoneLitOutside = Content.Load<Texture2D>("Graphics/Zones/GreenZoneA2");
            GreenZoneUnlit = Content.Load<Texture2D>("Graphics/Zones/GreenZoneIA1");
            GreenZoneUnlitOutside = Content.Load<Texture2D>("Graphics/Zones/GreenZoneIA2");

            // Sound effects
            GreenLaserSound = Content.Load<SoundEffect>("Sound/GreenLaser");
            ExplosionSound = Content.Load<SoundEffect>("Sound/Explosion");
            RedSound = Content.Load<SoundEffect>("Sound/Red");
            YellowSound = Content.Load<SoundEffect>("Sound/Yellow");
            BlueSound = Content.Load<SoundEffect>("Sound/Blue");
            PowerUpSound = Content.Load<SoundEffect>("Sound/PowerUpSound");
            ThinkFastWarning = Content.Load<SoundEffect>("Sound/ThinkFastFinal");
            DeathSound = Content.Load<SoundEffect>("Sound/DeathSoundEffect3");

            // Particles
            RegularParticle = Content.Load<Texture2D>("Graphics/Particles/Particle");
            BlueParticle = Content.Load<Texture2D>("Graphics/Particles/BParticle");
            YellowParticle = Content.Load<Texture2D>("Graphics/Particles/YParticle");
            RedParticle = Content.Load<Texture2D>("Graphics/Particles/RParticle");
            GreenParticle = Content.Load<Texture2D>("Graphics/Particles/GParticle");
            RedExplosionParticle = Content.Load<Texture2D>("Graphics/Particles/REParticle");
            BlueExplosionParticle = Content.Load<Texture2D>("Graphics/Particles/BEParticle");
            YellowExplosionParticle = Content.Load<Texture2D>("Graphics/Particles/YEParticle");
            YellowGunExplosionParticle = Content.Load<Texture2D>("Graphics/Particles/YCParticle");
            GreenExplosionParticle = Content.Load<Texture2D>("Graphics/Particles/GEParticle");
            RedLight = Content.Load<Texture2D>("Graphics/Particles/RELight");
            BlueLight = Content.Load<Texture2D>("Graphics/Particles/BELight");
            YellowLight = Content.Load<Texture2D>("Graphics/Particles/YELight");
            GreenLight = Content.Load<Texture2D>("Graphics/Particles/GELight");
            PulseTexture = RegularParticle;

            BlankPixel = Content.Load<Texture2D>("Graphics/Extras/BlankDot");
        }

        // Starts a match with the players and type specified
        public void StartMatch(MatchType mt, List<PlayerIndex> index)
        {
            this.matchType = mt;

            // Reset
            Initialize();
            Reset();
            ResetBackground();

            if (matchType.Equals(MatchType.Arcade) || matchType.Equals(MatchType.Zones) || matchType.Equals(MatchType.Travel) || matchType.Equals(MatchType.Survival) || matchType.Equals(MatchType.ThinkFast))
            {
                int i = 0;
                PlayerCount = index.Count;
                
                // Add each player
                foreach (PlayerIndex pi in index)
                {
                    Pod Player = new Pod(pi, this);
                    if (i == 0)
                    {
                        if (PlayerCount == 1)
                        {
                            Player.Position = new Vector2(640, 360);
                        }
                        else
                        if (PlayerCount == 2)
                        {
                            Player.Position = new Vector2(640, 310);
                        }
                        else
                        {
                            Player.Position = new Vector2(640, 210);
                        }
                    }
                    else if (i == 1)
                    {
                        if (PlayerCount == 2)
                        {
                            Player.Position = new Vector2(640, 410);
                        }
                        else
                        {
                            Player.Position = new Vector2(640, 310);
                        }
                    }
                    else if (i == 2)
                    {
                        Player.Position = new Vector2(640, 410);
                    }
                    else if (i == 3)
                    {
                        Player.Position = new Vector2(640, 510);
                    }
                    Pods.Add(Player);
                    i++;
                }
            }
            else if (matchType.Equals(MatchType.Versus))
            {
                int i = 0;
                PlayerCount = index.Count;
                // Add each player
                foreach (PlayerIndex pi in index)
                {
                    Pod Player = new Pod(pi, this);
                    if (i == 0)
                    {
                        if (PlayerCount == 2)
                        {
                            Player.Position = new Vector2(640, 310);
                        }
                        else
                        {
                            Player.Position = new Vector2(640, 210);
                        }
                    }
                    else if (i == 1)
                    {
                        if (PlayerCount == 2)
                        {
                            Player.Position = new Vector2(640, 410);
                        }
                        else
                        {
                            Player.Position = new Vector2(640, 310);
                        }
                    }
                    else if (i == 2)
                    {
                        Player.Position = new Vector2(640, 410);
                    }
                    else if (i == 3)
                    {
                        Player.Position = new Vector2(640, 510);
                    }
                    Pods.Add(Player);
                    i++;

                    Player.Lives = 10;
                    Player.Energy = 100;
                }
            }

            StartEnemyWaves();

            // Start music
            MediaPlayer.Play(gameSong);

            // Temporarily disable high score connectivity (to avoid lag in the middle of a match)
            HighscoreComponent.Global.Enabled = false;
        }

        
        // Updates the match
        public void Update(GameTime gameTime)
        {
            // Music
            MediaPlayer.GetVisualizationData(MusicData);

            /*int backRan = r.Next(500);
            if (backRan == 1)
            {
                SpawnNebulaGroup(30);
            }*/

            // Explosion sound
            ExplosionSoundHasPlayed = false;

            // If we are resetting, reset it
            if (BackgroundFadingIn)
            {
                BackgroundFade += 0.025f;
                if (BackgroundFade >= 1.0f)
                {
                    Reset();
                    ResetBackground();

                    // Play the new song
                    MediaPlayer.Play(gameSong);

                    BackgroundFadingIn = false;
                }
            }
            else if (BackgroundFade > 0)
            {
                BackgroundFade -= 0.025f;
            }

            // If this is a game over, handle the game over
            if (GameOver)
            {
                if (!MatchEnded)
                {
                    EndMatch();
                }
                else if(Particles.Count == 0 || ScoreSubmitted)
                {
                    if (hsc_.userWantsToLoad_ || hsc_.storage_ != null)
                    {
                        Game1.Global.RebuildHighscores(matchType.ToString());
                        Game1.Global.RebuildLocalHighscores(matchType.ToString());
                    }

                    if (Pods.Count == 1 && !ScoreSubmitted)
                    {
                        if (!HighscoreComponent.NameFromPlayer(Pods.ElementAt(0).PlayerIndex).Equals("Guest") && !Game1.Global.IsTrial)
                        {
                            if (hsc_.userWantsToLoad_ || hsc_.storage_ != null)
                            {
                                HighscoreComponent.Global.SetNewScore(Pods.ElementAt(0).PlayerIndex, Pods.ElementAt(0).Score, matchType.ToString());
                                ScoreSubmitted = true;
                            }
                        }
                    }
                    else if (Pods.Count > 1)
                    {
                        ScoreSubmitted = true;
                    }

                    foreach (Pod p in Pods)
                    {
                        if (p.gps.IsButtonDown(Buttons.B))
                        {
                            ReturnToMenu = true;
                            StartReset();
                        }
                        else if (p.gps.IsButtonDown(Buttons.A) && !BackgroundFadingIn)
                        {
                            StartReset();

                            // Temporarily disable high score connectivity (to avoid lag in the middle of a match)
                            HighscoreComponent.Global.Enabled = false;
                        }
                        else if (p.gps.IsButtonDown(Buttons.Back))
                        {
                            hsc_.userWantsToLoad_ = true;
                        }
                    }
                }

                foreach (Pod p in Pods)
                {
                    p.GameOverUpdate(gameTime);

                    // Destroy all bullets
                    if (p.Bullets.Count > 0)
                    {
                        for (int j = p.Bullets.Count - 1; j >= 0; j--)
                        {
                            SpawnExplosion(Particles, 10, p.Bullets.ElementAt(j).Position, p.Bullets.ElementAt(j).Type, 4, 4, 0.01f);
                            p.Bullets.RemoveAt(j);
                        }
                    }

                }

                UpdateBackground(gameTime);

                // Destroy all enemies
                if (Enemies.Count > 0)
                {
                    foreach (Enemy e in Enemies)
                    {
                        int ran = r.Next(4);
                        BulletType bt = null;
                        if (ran == 0)
                        {
                            bt = RedBullet;
                        }
                        else if (ran == 1)
                        {
                            bt = BlueBullet;
                        }
                        else if (ran == 2)
                        {
                            bt = YellowBullet;
                        }
                        else if (ran == 3)
                        {
                            bt = GreenBullet;
                        }
                        SpawnExplosion(Particles, 10, e.Position, bt, 4, 4, 0.005f);

                        e.Health = -1;
                    }
                }
                UpdateParticles(gameTime);
                UpdatePointDots(gameTime);
                RemoveParticles();
                UpdateScreenText(gameTime);
                UpdateImprints(gameTime);
                RemoveDeadEnemies();

                // Spawn some random explosions
                if (ScoreSubmitted && Pods.Count > 1)
                {
                    int ranEx = r.Next(20);
                    int ranT = r.Next(4);

                    if (ranEx == 0)
                    {
                        BulletType bT = null;
                        if (ranT == 0)
                        {
                            bT = YellowBullet;
                        }
                        else if (ranT == 1)
                        {
                            bT = RedBullet;
                        }
                        else if (ranT == 2)
                        {
                            bT = BlueBullet;
                        }
                        else
                        {
                            bT = GreenBullet;
                        }

                        SpawnExplosion(Particles, 50, RandomSpawnPosition(1), bT, 4, 8, 0.005f);
                    }
                }
                return;
            }


            // Check for game over
            bool gov = true;
            foreach (Pod p in Pods)
            {
                if (p.Lives > 0)
                {
                    gov = false;
                }
            }
            if (gov)
            {
                GameOver = true;
            }
            

            #region FPS
             // Update
            _elapsed_time += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
 
            // 1 Second has passed
            if (_elapsed_time >= 1000.0f)
            {
                _fps = _total_frames;
                _total_frames = 0;
                _elapsed_time = 0;
            }

            #endregion

            // Decrease timer if we are using one
            if (matchType.Equals(Level.MatchType.Arcade) || matchType.Equals(Level.MatchType.Versus))
            {
                timer -= gameTime.ElapsedGameTime.Milliseconds;

                // Check for game over
                if (timer <= 0)
                {
                    GameOver = true;
                }
            }

            // This spawns enemies
            if (!matchType.Equals(MatchType.Versus))
            {
                GenerateEnemies(gameTime);
            }

            #region Zone Gametype Updates
            if (matchType.Equals(Level.MatchType.Zones))
            {
                UpdateZones(gameTime);
            }
            #endregion

            // Draws the travel point if we need to
            #region Travel Point Gametype Updates
            if (matchType.Equals(Level.MatchType.Travel))
            {
                UpdateTravelPoint();
            }
            #endregion

            // Increases points if this is survival mode
            #region Survival Updates
            if (matchType.Equals(MatchType.Survival))
            {
                foreach (Pod p in Pods)
                {
                    if (!p.IsRespawning)
                    {
                        p.Score += 1;
                    }
                }
            }
            #endregion

            // Increases the surival timer if we are playing survival
            SurvivalTimer += gameTime.ElapsedGameTime.Milliseconds;

            /*if (Enemies.Count < 30)
            {
                SpawnEnemy(FloatEnemy, RandomSpawnPosition(200.0f));
            }*/

            // Updates each of the players
            foreach (Pod p in Pods)
            {
                p.Update(gameTime);
            }

            // Updates each of the background layers
            foreach (Pod p in Pods)
            {
                foreach (BackgroundStar b in Layer1)
                {
                    b.Velocity -= p.TotalVelocity / 400;
                }
                foreach (BackgroundStar b in Layer2)
                {
                    b.Velocity -= p.TotalVelocity / 600;
                }
                foreach (BackgroundStar b in Layer3)
                {
                    b.Velocity -= p.TotalVelocity / 800;
                }

                break;
            }

            UpdateBackground(gameTime);

            foreach (Pod p in Pods)
            {
                // Check for pause
                if ((p.gps.IsButtonDown(Buttons.Start) && Game1.Global.StartButtonReady) || !p.gps.IsConnected)
                {
                    Game1.Global.Pause(p.PlayerIndex);
                    Game1.Global.StartButtonReady = false;
                }
            }

            // Update the border flash
            if (BorderShowing)
            {
                BorderAlpha -= 0.01f;
                if (BorderAlpha <= 0)
                {
                    BorderShowing = false;
                }
            }
            else
            {
                BorderAlpha = 1.0f;
            }

            // Updates each of the enemies
            /*foreach (Enemy e in Enemies)
            {
                e.Update(gameTime);
            }*/

            // Updates each of the background stars
            foreach (BackgroundStar b in BackgroundStars)
            {
                b.Update();
            }

            // Bullet collision, pod collision, updates enemies
            if (!matchType.Equals(MatchType.Versus))
            {
                HandleEnemyCollision(gameTime);
            }
            else
            {
                //Check bullets against other pods
                HandleEnemyPodCollisions(gameTime);
            }

            // Enemy collision with pods
            // HandlePodCollision();

            // Particle hit detection
            // HandleParticleCollisions();

            HandleShaking(gameTime);

            // Updates the backgrounds
            UpdateBackgrounds();

            // Updates the particles
            UpdateParticles(gameTime);

            // Removes particles
            RemoveParticles();

            // Updates the point dots
            UpdatePointDots(gameTime);

            // Removes Dead Enemies
            RemoveDeadEnemies();

            #region Think Fast
            if (matchType.Equals(Level.MatchType.ThinkFast))
            {
                UpdateThinkFast(gameTime);
            }
            #endregion

            // Updates the screen text
            UpdateScreenText(gameTime);

            // Updates the imprints
            UpdateImprints(gameTime);

            // Updates the powerups
            UpdatePowerups(gameTime);
        }

        // Draws the match
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _total_frames++;

            // Draws the background
            spriteBatch.Draw(backgroundBoxTexture, Vector2.Zero, new Rectangle(0, 0, 640, 360), Color.Black);
            spriteBatch.Draw(backgroundBoxTexture, new Vector2(640, 0), new Rectangle(0, 0, 640, 360), Color.Black);
            spriteBatch.Draw(backgroundBoxTexture, new Vector2(0, 360), new Rectangle(0, 0, 640, 360), Color.Black);
            spriteBatch.Draw(backgroundBoxTexture, new Vector2(640, 360), new Rectangle(0, 0, 640, 360), Color.Black);

            Vector2 tempPos = new Vector2(BackgroundTexure.Width / 2, BackgroundTexure.Height / 2);
            spriteBatch.Draw(BackgroundTexure, tempPos, null, Color.White, BackgroundRotation, tempPos, 1.0f, SpriteEffects.None, 0);

            // Draws all the background stars
            /*foreach (BackgroundStar b in BackgroundStars)
            {
                b.Draw(spriteBatch);
            }*/

            // Draws the background layers

            float avg = 0;
            for (int f = 0; f < 85; f++)
            {
                avg += MusicData.Frequencies[f];
            }
            avg = avg / 85;

            foreach (BackgroundStar b in Layer3)
            {
                // b.Scale = 2.0f * avg + 0.5f;
                b.Draw(spriteBatch);
            }


            /*drawBig = false;
            for (int f = 85; f < 170; f++)
            {
                if ((MusicData.Frequencies[f] - LastMusicData[f]) > 0.04f)
                {
                    drawBig = true;
                    break;
                }
            }*/

            avg = 0;
            for (int f = 85; f < 170; f++)
            {
                avg += MusicData.Frequencies[f];
            }
            avg = avg / 85;


            foreach (BackgroundStar b in Layer2)
            {
                // b.Scale = 2.0f * avg + 0.5f;
                b.Draw(spriteBatch);
            }


            avg = 0;
            for (int f = 170; f < 256; f++)
            {
                avg += MusicData.Frequencies[f];
            }
            avg = avg / 85;

            foreach (BackgroundStar b in Layer1)
            {
                // b.Scale = 2.0f * avg + 0.5f;
                b.Draw(spriteBatch);
            }

            // Draws the imprints
            foreach (Imprint im in Imprints)
            {
                im.Draw(spriteBatch);
            }




            foreach (Particle p in LiquidParticles)
            {
                p.Draw(spriteBatch);
            }

            // Draws the border texture
            /*if (BorderShowing)
            {
                spriteBatch.Draw(BorderTexture, Vector2.Zero, new Color(Color.White, BorderAlpha));
            }*/

            // Draws each of the screen text
            foreach (ScreenText text in Text)
            {
                text.Draw(spriteBatch);
            }

            // Draws each of the trail particles
            foreach (Particle p in TrailParticles)
            {
                p.Draw(spriteBatch);
            }

            // Draws the point dots
            foreach (PointDot pd in PointDots)
            {
                pd.Draw(spriteBatch);
            }

            // Draws each of the pods
            foreach (Pod p in Pods)
            {
                p.Draw(spriteBatch);
            }

            // Draws each of the enemies
            foreach (Enemy e in Enemies)
            {
                e.Draw(spriteBatch, gameTime);
            }

            // Draws the zones if we need to
            #region Zone Gametype Drawings
            if (matchType.Equals(Level.MatchType.Zones) && !GameOver)
            {
                zone.Draw(spriteBatch);
            }
            #endregion

            // Draws the travel point if we need to
            #region Travel Point Gametype Updates
            if (matchType.Equals(Level.MatchType.Travel) && !GameOver)
            {
                tPoint.Draw(spriteBatch);
            }
            #endregion


            // Draws all the particles
            foreach (Particle p in Particles)
            {
                p.Draw(spriteBatch);
            }

            foreach (Powerup p in Powerups)
            {
                p.Draw(spriteBatch);
            }

            // Draws each bullet of the pods
            foreach (Pod p in Pods)
            {
                p.DrawBullets(spriteBatch, gameTime);
                p.DrawHitParticles(spriteBatch);
            }

            foreach (Particle p in PulseParticles)
            {
                p.Draw(spriteBatch);
            }

            // spriteBatch.Draw(HUDTexture, HUDPosition, Color.White);

            // spriteBatch.DrawString(gameFont, _fps.ToString(), new Vector2(1100, 80), Color.White);

            // The timer in arcade gamemode
            if (matchType.Equals(MatchType.Arcade) || matchType.Equals(MatchType.Versus))
            {
                float tempTimer = timer;
                int minutes = (int)(tempTimer / 60000.0f);
                int seconds = (int)(tempTimer / 1000.0f) % 60;
                string second = seconds.ToString();
                if(seconds < 10)
                {
                    second = "0" + seconds.ToString();
                }
                string stime = minutes.ToString() + ":" + second;
                spriteBatch.DrawString(gameFont, stime, TimePosition, Color.White);
            }
            else if (matchType.Equals(MatchType.Survival))
            {
                // The survival timer
                int minutes = (int)(SurvivalTimer / 60000.0f);
                int seconds = (int)(SurvivalTimer / 1000.0f) % 60;
                string second = seconds.ToString();
                if (seconds < 10)
                {
                    second = "0" + seconds.ToString();
                }
                string stime = minutes.ToString() + ":" + second;
                spriteBatch.DrawString(gameFont, stime, TimePosition, Color.White);
            }
            else if (matchType.Equals(MatchType.ThinkFast))
            {
                Color TimeColor = Color.White;
                if (CurrentColor.Equals("Green"))
                {
                    TimeColor = Color.Green;
                }
                else if (CurrentColor.Equals("Blue"))
                {
                    TimeColor = Color.Turquoise;
                }
                else if (CurrentColor.Equals("Red"))
                {
                    TimeColor = Color.Red;
                }
                else if (CurrentColor.Equals("Yellow"))
                {
                    TimeColor = Color.Yellow;
                }
                // Draw the icon
                spriteBatch.Draw(CurrentThinkFastTexture, ThinkFastIconPoint, null, Color.White, 0, ThinkFastIconOrigin, 0.8f, SpriteEffects.None, 0);

                if (ColorFlashScale > 0 && !GameOver)
                {
                    spriteBatch.Draw(CurrentThinkFastFlashTexture, Game1.CenterVector, null, new Color(Color.White, ColorFlashAlpha), 0, 
                       ThinkFastOrigin, ColorFlashScale, SpriteEffects.None, 0);
                }
            }

            // List out high scores
            if (GameOver)
            {
                // Draw highscore textutre
                if (Particles.Count == 0 || ScoreSubmitted)
                {
                    // Draw instructions
                    Game1.Global.AButton.Draw(spriteBatch, Color.White, new Vector2(250, 585));
                    spriteBatch.DrawString(gameFontLarge, "Replay", new Vector2(310, 595), Color.White);

                    Game1.Global.BButton.Draw(spriteBatch, Color.White, new Vector2(970, 585));
                    spriteBatch.DrawString(gameFontLarge, "Back To Menu", new Vector2(800, 595), Color.White);

                    if (Game1.Global.IsTrial)
                    {
                        Game1.Global.XButton.Draw(spriteBatch, Color.White, new Vector2(540, 585));
                        spriteBatch.DrawString(gameFontLarge, "Buy Now", new Vector2(600, 595), Color.White);
                    }

                    if(Pods.Count > 1)
                    {
                        string win = "";
                        int topScore = 0;
                        int cn = 0;
                        foreach(Pod p in Pods)
                        {
                            if(p.Score > topScore)
                            {
                                topScore = p.Score;
                            }
                        }

                        foreach(Pod p in Pods)
                        {
                            if(p.Score == topScore)
                            {
                                if(HighscoreComponent.NameFromPlayer(p.PlayerIndex).Equals("Guest"))
                                {
                                    win += "Player " + p.PlayerIndex.ToString() + "\n";
                                }
                                else
                                {
                                    win += HighscoreComponent.NameFromPlayer(p.PlayerIndex) + "\n";
                                }

                                cn++;
                            }
                        }

                        if (cn == 1)
                        {
                            win += "IS THE WINNER!";
                        }
                        else
                        {
                            win += "TIED!";
                        }

                        spriteBatch.DrawString(gameFontExtraLarge, win, new Vector2(640, 360), Color.White, 0, gameFontExtraLarge.MeasureString(win) / 2, 1.0f, SpriteEffects.None, 0);
                     }

                    Color HighscoreColor = Color.White;
                    if (Pods.Count == 1)
                    {
                        if (Pods.ElementAt(0).Gun.Equals(Green))
                        {
                            HighscoreColor = Color.Green;
                        }
                        else if (Pods.ElementAt(0).Gun.Equals(Blue))
                        {
                            HighscoreColor = Color.Turquoise;
                        }
                        else if (Pods.ElementAt(0).Gun.Equals(Red))
                        {
                            HighscoreColor = Color.Red;
                        }
                        else if (Pods.ElementAt(0).Gun.Equals(Yellow))
                        {
                            HighscoreColor = Color.Yellow;
                        }
                    }

                    if (Pods.Count == 1)
                    {
                        spriteBatch.Draw(LinesHighscoresLevel, Vector2.Zero, HighscoreColor);

                        switch (matchType)
                        {
                            case MatchType.Arcade:
                                {
                                    spriteBatch.DrawString(gameFontLarge, "Arcade", new Vector2(600, 160), Color.White);
                                    break;
                                }
                            case MatchType.Survival:
                                {
                                    spriteBatch.DrawString(gameFontLarge, "Survival", new Vector2(590, 160), Color.White);
                                    break;
                                }
                            case MatchType.Zones:
                                {
                                    spriteBatch.DrawString(gameFontLarge, "Zones", new Vector2(610, 160), Color.White);
                                    break;
                                }
                            case MatchType.Travel:
                                {
                                    spriteBatch.DrawString(gameFontLarge, "Waypoint", new Vector2(585, 160), Color.White);
                                    break;
                                }
                            case MatchType.ThinkFast:
                                {
                                    spriteBatch.DrawString(gameFontLarge, "Think Fast", new Vector2(575, 160), Color.White);
                                    break;
                                }
                            case MatchType.Versus:
                                {
                                    spriteBatch.DrawString(gameFontLarge, "Versus", new Vector2(600, 160), Color.White);
                                    break;
                                }
                        }

                        // Draw Local Highscores
                        spriteBatch.DrawString(gameFontLarge, "Local", new Vector2(410, 198), Color.White);
                        spriteBatch.DrawString(gameFontLarge, "Global", new Vector2(800, 198), Color.White);

                        if (!matchType.Equals(MatchType.Versus))
                        {

                            float localY = 240;
                            if (!Game1.Global.IsTrial && hsc_.storage_ != null)
                            {
                                // Local scores
                                foreach (Highscore s in Game1.Global.localScores_)
                                {
                                    if (s != null)
                                    {
                                        Color tempC = Color.White;

                                        spriteBatch.DrawString(gameFont, s.Gamer, new Vector2(270, localY), tempC);
                                        spriteBatch.DrawString(gameFont, s.Score.ToString(), new Vector2(515, localY), tempC);
                                        localY += 33;
                                    }
                                }
                            }
                            else if (hsc_.storage_ != null)
                            {
                                for (int i = 0; i < 10; i++)
                                {
                                    spriteBatch.DrawString(gameFont, "Buy To Submit", new Vector2(270, localY), Color.White);
                                    spriteBatch.DrawString(gameFont, "Your Score", new Vector2(490, localY), Color.White);
                                    localY += 33;
                                }
                            }

                            // Global highscores
                            localY = 240;
                            if (hsc_.storage_ != null)
                            {
                                foreach (Highscore s in Game1.Global.data_)
                                {
                                    if (s != null)
                                    {
                                        Color tempC = Color.White;

                                        spriteBatch.DrawString(gameFont, s.Gamer, new Vector2(660, localY), tempC);
                                        spriteBatch.DrawString(gameFont, s.Score.ToString(), new Vector2(900, localY), tempC);
                                        localY += 33;
                                    }
                                }
                            }
                        }
                        else
                        {
                            spriteBatch.DrawString(gameFont, "Highscores are not     available in versus.", new Vector2(270, 240), Color.White);
                            spriteBatch.DrawString(gameFont, "Highscores are not     available in versus.", new Vector2(660, 240), Color.White);
                        }

                        if (!hsc_.userWantsToLoad_ && hsc_.storage_ == null)
                        {
                            spriteBatch.DrawString(gameFont, "No storage device          is selected.", new Vector2(270, 240), Color.White);
                            spriteBatch.DrawString(gameFont, "Press back to              select device.", new Vector2(270, 290), Color.White);
                            spriteBatch.DrawString(gameFont, "No storage device            is selected.", new Vector2(660, 240), Color.White);
                            spriteBatch.DrawString(gameFont, "Press back to                select device.", new Vector2(660, 290), Color.White);
                        }
                    }
                }
            }

            // random mediaplayer texture
            Viewport viewport = Game1.Global.graphics.GraphicsDevice.Viewport;
            int x1, y1, width, height;


            #region Music Visualization
            Color MusicColor = Color.White;
            if (Pods.Count == 1)
            {
                if (Pods.ElementAt(0).Gun.Equals(Red))
                {
                    MusicColor = Color.Red;
                }
                else if (Pods.ElementAt(0).Gun.Equals(Blue))
                {
                    MusicColor = Color.Turquoise;
                }
                else if (Pods.ElementAt(0).Gun.Equals(Yellow))
                {
                    MusicColor = Color.Yellow;
                }
                else if (Pods.ElementAt(0).Gun.Equals(Green))
                {
                    MusicColor = Color.Green;
                }
            }
            for (int f = 0; f < MusicData.Frequencies.Count; f++)
            {
                x1 = 300 * f / MusicData.Frequencies.Count + 640;
                y1 = (int)(viewport.Height - MusicData.Frequencies[f] * 72);
                width = 1;
                height = (int)(MusicData.Frequencies[f] * 72);
                if (f % 2 == 0)
                {
                    spriteBatch.Draw(BlankPixel, new Rectangle(x1, y1, width, height), MusicColor);
                }
            }
            for (int f = MusicData.Frequencies.Count - 1; f >= 0; f--)
            {
                x1 = 300 * (256 - f) / MusicData.Frequencies.Count + 340;
                y1 = (int)(viewport.Height - MusicData.Frequencies[f] * 72);
                width = 1;
                height = (int)(MusicData.Frequencies[f] * 72);
                if (f % 2 == 0)
                {
                    spriteBatch.Draw(BlankPixel, new Rectangle(x1, y1, width, height), MusicColor);
                }
            }

            for (int f = 0; f < MusicData.Frequencies.Count; f++)
            {
                x1 = 300 * f / MusicData.Frequencies.Count + 640;
                y1 = (int)(MusicData.Frequencies[f] * 0);
                width = 1;
                height = (int)(MusicData.Frequencies[f] * 72);
                if (f % 2 == 0)
                {
                    spriteBatch.Draw(BlankPixel, new Rectangle(x1, y1, width, height), MusicColor);
                }
            }
            for (int f = MusicData.Frequencies.Count - 1; f >= 0; f--)
            {
                x1 = 300 * (256 - f) / MusicData.Frequencies.Count + 340;
                y1 = (int)(MusicData.Frequencies[f] * 0);
                width = 1;
                height = (int)(MusicData.Frequencies[f] * 72);
                if (f % 2 == 0)
                {
                    spriteBatch.Draw(BlankPixel, new Rectangle(x1, y1, width, height), MusicColor);
                }
            }
            #endregion


            #region HUD
            if (matchType.Equals(Level.MatchType.Arcade) || matchType.Equals(Level.MatchType.Survival) || matchType.Equals(Level.MatchType.ThinkFast) || matchType.Equals(MatchType.Versus))
            {
                if (Pods.Count == 1)
                {
                    if (Pods.ElementAt(0).Gun.Equals(Red))
                    {
                        spriteBatch.Draw(RedHudTime, HUDPosition, Color.White);
                        Pods.ElementAt(0).DrawHUD(spriteBatch, PodHUDPosition);
                    }
                    else if (Pods.ElementAt(0).Gun.Equals(Blue))
                    {
                        spriteBatch.Draw(BlueHudTime, HUDPosition, Color.White);
                        Pods.ElementAt(0).DrawHUD(spriteBatch, PodHUDPosition);
                    }
                    else if (Pods.ElementAt(0).Gun.Equals(Yellow))
                    {
                        spriteBatch.Draw(YellowHudTime, HUDPosition, Color.White);
                        Pods.ElementAt(0).DrawHUD(spriteBatch, PodHUDPosition);
                    }
                    else if (Pods.ElementAt(0).Gun.Equals(Green))
                    {
                        spriteBatch.Draw(GreenHudTime, HUDPosition, Color.White);
                        Pods.ElementAt(0).DrawHUD(spriteBatch, PodHUDPosition);
                    }
                }
                else if (Pods.Count == 2)
                {
                    spriteBatch.Draw(TwoPlayerHudTime, HUDPosition, Color.White);
                    Pods.ElementAt(0).DrawHUD(spriteBatch, PodHUDPosition);
                    Pods.ElementAt(1).DrawHUD(spriteBatch, new Vector2(1028, PodHUDPosition.Y), 18);
                }
                else if (Pods.Count == 3)
                {
                    spriteBatch.Draw(ThreePlayerHudTime, HUDPosition, Color.White);
                    Pods.ElementAt(0).DrawHUD(spriteBatch, PodHUDPosition);
                    Pods.ElementAt(1).DrawHUD(spriteBatch, new Vector2(PodHUDPosition.X + 210, PodHUDPosition.Y));
                    Pods.ElementAt(2).DrawHUD(spriteBatch, new Vector2(PodHUDPosition.X + 697, PodHUDPosition.Y));
                }
                else if (Pods.Count == 4)
                {
                    spriteBatch.Draw(FourPlayerHudTime, HUDPosition, Color.White);
                    Pods.ElementAt(0).DrawHUD(spriteBatch, PodHUDPosition);
                    Pods.ElementAt(1).DrawHUD(spriteBatch, new Vector2(PodHUDPosition.X + 210, PodHUDPosition.Y));
                    Pods.ElementAt(2).DrawHUD(spriteBatch, new Vector2(PodHUDPosition.X + 697, PodHUDPosition.Y));
                    Pods.ElementAt(3).DrawHUD(spriteBatch, new Vector2(1028, PodHUDPosition.Y), 18);
                }
            }
            else
            {
                if (Pods.Count == 1)
                {
                    if (Pods.ElementAt(0).Gun.Equals(Red))
                    {
                        spriteBatch.Draw(RedHudNoTime, HUDPosition, Color.White);
                        Pods.ElementAt(0).DrawHUD(spriteBatch, PodHUDPosition);
                    }
                    else if (Pods.ElementAt(0).Gun.Equals(Blue))
                    {
                        spriteBatch.Draw(BlueHudNoTime, HUDPosition, Color.White);
                        Pods.ElementAt(0).DrawHUD(spriteBatch, PodHUDPosition);
                    }
                    else if (Pods.ElementAt(0).Gun.Equals(Yellow))
                    {
                        spriteBatch.Draw(YellowHudNoTime, HUDPosition, Color.White);
                        Pods.ElementAt(0).DrawHUD(spriteBatch, PodHUDPosition);
                    }
                    else if (Pods.ElementAt(0).Gun.Equals(Green))
                    {
                        spriteBatch.Draw(GreenHudNoTime, HUDPosition, Color.White);
                        Pods.ElementAt(0).DrawHUD(spriteBatch, PodHUDPosition);
                    }
                }
                else if (Pods.Count == 2)
                {
                    spriteBatch.Draw(TwoPlayerHudNoTime, HUDPosition, Color.White);
                    Pods.ElementAt(0).DrawHUD(spriteBatch, PodHUDPosition);
                    Pods.ElementAt(1).DrawHUD(spriteBatch, new Vector2(1028, PodHUDPosition.Y), 18);
                }
                else if (Pods.Count == 3)
                {
                    spriteBatch.Draw(ThreePlayerHudNoTime, HUDPosition, Color.White);
                    Pods.ElementAt(0).DrawHUD(spriteBatch, PodHUDPosition);
                    Pods.ElementAt(1).DrawHUD(spriteBatch, new Vector2(PodHUDPosition.X + 210, PodHUDPosition.Y));
                    Pods.ElementAt(2).DrawHUD(spriteBatch, new Vector2(PodHUDPosition.X + 697, PodHUDPosition.Y));
                }
                else if (Pods.Count == 4)
                {
                    spriteBatch.Draw(FourPlayerHudNoTime, HUDPosition, Color.White);
                    Pods.ElementAt(0).DrawHUD(spriteBatch, PodHUDPosition);
                    Pods.ElementAt(1).DrawHUD(spriteBatch, new Vector2(PodHUDPosition.X + 210, PodHUDPosition.Y));
                    Pods.ElementAt(2).DrawHUD(spriteBatch, new Vector2(PodHUDPosition.X + 697, PodHUDPosition.Y));
                    Pods.ElementAt(3).DrawHUD(spriteBatch, new Vector2(1028, PodHUDPosition.Y), 18);
                }
            }

            #endregion

            // top
            /*
            for (int f = 0; f < MusicData.Frequencies.Count; f++)
            {
                x1 = 512 * f / MusicData.Frequencies.Count + 638 - 6;
                y1 = (int)(MusicData.Frequencies[f] * 100 - 6);
                width = 14;
                height = (int)(MusicData.Frequencies[f] * 10);
                spriteBatch.Draw(MusicGlow, new Vector2(x1, y1), Color.White);
            }
            for (int f = MusicData.Frequencies.Count - 1; f >= 0; f--)
            {
                x1 = 512 * (256 - f) / MusicData.Frequencies.Count + 126 - 6;
                y1 = (int)(MusicData.Frequencies[f] * 100 - 6);
                width = 14;
                height = (int)(MusicData.Frequencies[f] * 10);
                spriteBatch.Draw(MusicGlow, new Vector2(x1, y1), Color.White);
            }

            for (int f = 0; f < MusicData.Frequencies.Count; f++)
            {
                x1 = 512 * f / MusicData.Frequencies.Count + 638;
                y1 = (int)(MusicData.Frequencies[f] * 100);
                width = 2;
                height = (int)(MusicData.Frequencies[f] * 100);
                height = 2;
                spriteBatch.Draw(BlankPixel, new Rectangle(x1, y1, width, height), Color.White);
            }
            for (int f = MusicData.Frequencies.Count - 1; f >= 0; f--)
            {
                x1 = 512 * (256 - f) / MusicData.Frequencies.Count + 126;
                y1 = (int)(MusicData.Frequencies[f] * 100);
                width = 2;
                height = (int)(MusicData.Frequencies[f] * 100);
                height = 2;
                spriteBatch.Draw(BlankPixel, new Rectangle(x1, y1, width, height), Color.White);
            }*/

            /*
            for (int s = 0; s < MusicData.Samples.Count; s++)
            {
                x1 = viewport.Width * s / MusicData.Samples.Count;
                width = 2;
                if (MusicData.Samples[s] > 0.0f)
                {
                    y1 = (int)(0.75f * viewport.Height - MusicData.Samples[s] * viewport.Height / 4);
                    height = (int)(MusicData.Samples[s] * viewport.Height / 4);
                }
                else
                {
                    y1 = (int)(0.75f * viewport.Height);
                    height = (int)(-1.0f * MusicData.Samples[s] * viewport.Height / 4);
                }
                spriteBatch.Draw(BlankPixel, new Rectangle(x1, y1, width, height), Color.White);
            }*/


            // The background fading
            spriteBatch.Draw(backgroundBoxTexture, Vector2.Zero, new Rectangle(0, 0, 1280, 720), new Color(Color.Black, BackgroundFade), 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0);

        }

        // Generates enemies when we are in regular gamemode
        public void GenerateEnemies(GameTime gameTime)
        {
            // If we are respawning do not spawn enemies
            foreach (Pod p in Pods)
            {
                if (p.IsRespawning && p.Lives > 0)
                {
                    return;
                }
            }

            TotalSeconds += gameTime.ElapsedGameTime.Milliseconds;

            if (TotalSeconds % 1000 == 0)
            {
                int ran = r.Next(EnemyCount) + 1;
                int ran2 = r.Next(3);
                if (ran2 == 0)
                {
                    SpawnEnemyGroup(ran);
                }
                else if(ran2 == 1)
                {
                    int ran3 = r.Next(4);
                    EnemyType et = null;
                    if (ran3 == 0)
                    {
                        et = ClusterEnemy;
                    }
                    else if (ran3 == 1)
                    {
                        et = FollowEnemy;
                    }
                    else if (ran3 == 2)
                    {
                        et = DashEnemy;
                    }
                    else if (ran3 == 3)
                    {
                        et = FloatEnemy;
                    }
                    SpawnCornerGroup(ran, et);
                }
                else if (ran2 == 2)
                {
                    int ranSnakeNum = r.Next(5 + (int)(EnemyCount / 7)) + 1;
                    for (int i = 0; i < ranSnakeNum; i++)
                    {
                        int ranNumToSpawn = r.Next((int)(EnemyCount / 5) + 1);
                        SpawnSnakeEnemy(ranNumToSpawn + (EnemyCount / 10) + 2);
                    }
                }

                if (TotalSeconds % 2000 == 0)
                {
                    EnemyCount += r.Next(2);
                }

                // Max enemy count
                if (EnemyCount > MaxEnemyCount)
                {
                    EnemyCount = MaxEnemyCount;
                }
            }
        }

        // spawns a snake enemy
        public void SpawnSnakeEnemy(int count)
        {
            Vector2 pos = RandomSpawnPosition(300 - ((Pods.Count) * 50));
            float OriginalRotation = (float)(r.Next(628) / 10.0f);
            for (int i = 0; i < count; i++)
            {
                Enemy e = new Enemy(SnakeEnemy, this);
                e.Position = pos;
                e.InitializeTimer = 250 * i;
                e.OriginalRotation = OriginalRotation;
                e.r = r;

                Enemies.Add(e);
            }
        }

        public void DrawString(SpriteBatch spriteBatch, string s)
        {
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/SpriteFont1"), s, new Vector2(100, 100), Color.Black);
        }

        // Handles bullet collision
        public void HandleEnemyCollision(GameTime gameTime)
        {
            // Check each bullet in each pod
            foreach (Enemy e in Enemies)
            {
                foreach (Pod pod in Pods)
                {
                    for(int i = (pod.Bullets.Count - 1); i >= 0; i--)
                    {
                        Bullet b = pod.Bullets.ElementAt(i);

                        if (e.Initialized && Vector2.Distance(b.Position, e.Position) < 30)
                        {
                            if (e.GetRotatedRectangle().Intersects(b.GetRotatedRectangle()))
                            {
                                e.Health -= b.Type.Damage;
                                if (!b.Type.Equals(RedBullet) && !b.Type.Equals(BlueBullet))
                                {
                                    pod.Bullets.RemoveAt(i);
                                }

                                if (b.Type.Equals(YellowBullet))
                                {
                                    SpawnExplosion(pod.HitParticles, 20, e.Position, YellowBulletReplaced, 2, 4, 0);
                                }

                                if (e.Health < 0)
                                {

                                    if (!ExplosionSoundHasPlayed)
                                    {
                                        ExplosionSound.Play(Game1.Global.SoundEffectVolume, 0, 0);
                                        ExplosionSoundHasPlayed = true;
                                    }

                                    if (!b.Type.Equals(YellowBullet))
                                    {
                                        SpawnExplosion(Particles, e.Type.ParticleNumber / Pods.Count, e.Position, b.Type, 2, 8, 0.005f);
                                    }
                                    SpawnLightFade(e.Position, b.Type, 0.03f);
                                    if (matchType.Equals(MatchType.Arcade))
                                    {
                                        pod.Score += e.Type.Score;
                                    }

                                    if (matchType.Equals(MatchType.ThinkFast))
                                    {
                                        if (CurrentColor.Equals("Green") && b.Type.Equals(GreenBullet))
                                        {
                                            pod.Score += e.Type.Score;
                                        }
                                        else if (CurrentColor.Equals("Blue") && b.Type.Equals(BlueBullet))
                                        {
                                            pod.Score += e.Type.Score;
                                        }
                                        else if (CurrentColor.Equals("Yellow") && b.Type.Equals(YellowBullet))
                                        {
                                            pod.Score += e.Type.Score;
                                        }
                                        else if (CurrentColor.Equals("Red") && b.Type.Equals(RedBullet))
                                        {
                                            pod.Score += e.Type.Score;
                                        }
                                    }

                                    if (matchType.Equals(MatchType.Arcade) || matchType.Equals(MatchType.Survival))
                                    {
                                        SpawnPointDots(r.Next(4), e.Position);
                                    }

                                    // Write the number of points the enemy is worth onscreen
                                    if (matchType.Equals(MatchType.Arcade))
                                    {
                                        Text.Add(new ScreenText(gameFont, e.Position, e.Type.Score.ToString()));
                                    }

                                    // Spawn a powerup if there is enough points
                                    if (r.Next(EnemyCount * 20) == 1)
                                    {
                                        SpawnPowerUp(e.Position);
                                    }
                                }
                            }
                        }
                    }

                    // Also check hit particles
                    for (int i = pod.HitParticles.Count - 1; i >= 0; i--)
                    {
                        Particle p = pod.HitParticles.ElementAt(i);
                        if (e.Initialized && Vector2.Distance(p.Position, e.Position) < 30)
                        {
                            if (e.GetRotatedRectangle().Intersects(p.GetRectangle()))
                            {
                                // pod.HitParticles.RemoveAt(i);
                                e.Health -= 2;
                            }

                            if (e.Health < 0)
                            {
                                if (!ExplosionSoundHasPlayed)
                                {
                                    ExplosionSound.Play(Game1.Global.SoundEffectVolume, 0, 0);
                                    ExplosionSoundHasPlayed = true;
                                }

                                SpawnExplosion(Particles, e.Type.ParticleNumber / Pods.Count, e.Position, YellowBullet, 2, 8, 0.005f);
                                SpawnLightFade(e.Position, YellowBullet, 0.03f);
                                if (matchType.Equals(MatchType.Arcade) || (matchType.Equals(MatchType.ThinkFast) && CurrentColor.Equals("Yellow")))
                                {
                                    pod.Score += e.Type.Score;
                                }

                                if (matchType.Equals(MatchType.Arcade) || matchType.Equals(MatchType.Survival))
                                {
                                    SpawnPointDots(r.Next(4), e.Position);
                                }

                                // Write the number of points the enemy is worth onscreen
                                if (matchType.Equals(MatchType.Arcade))
                                {
                                    Text.Add(new ScreenText(gameFont, e.Position, e.Type.Score.ToString()));
                                }

                                // Spawn a powerup if there is enough points
                                if (r.Next(EnemyCount * 20) == 1)
                                {
                                    SpawnPowerUp(e.Position);
                                }
                                break;
                            }
                        }
                    }

                    // Also check for pod collisions
                    if (pod.Lives > 0)
                    {
                        if (e.Initialized && Vector2.Distance(pod.Position, e.Position) < 30)
                        {
                            if (pod.GetRectangle().Intersects(e.GetRectangle()))
                            {
                                SpawnExplosion(Particles, e.Type.ParticleNumber / Pods.Count, e.Position, pod.Gun.Bullet, 4, 4, 0.01f);
                                SpawnLightFade(e.Position, pod.Gun.Bullet, 0.03f);
                                ShakeScreen(500.0f);
                                pod.Die();
                                e.Health = -1;
                            }
                        }
                    }
                }
                
                // Also update enemies while we are looping through them
                e.Update(gameTime);
            }
        }

        // Handles collisions with other pods
        public void HandleEnemyPodCollisions(GameTime gameTime)
        {
            // Check each pod with all bullets
            foreach (Pod p in Pods)
            {
                // Check bullets against these pods
                foreach (Pod ePod in Pods)
                {
                    if (!ePod.Equals(p))
                    {
                        for (int i = ePod.Bullets.Count - 1; i >= 0; i--)
                        {
                            Bullet b = ePod.Bullets.ElementAt(i);
                            if (!p.IsRespawning && Vector2.Distance(b.Position, p.Position) < 50)
                            {
                                if (b.GetRotatedRectangle().Intersects(p.GetRotatedRectangle()))
                                {
                                    if (b.Type.Equals(YellowBullet))
                                    {
                                        SpawnExplosion(ePod.HitParticles, 4, p.Position, YellowBulletReplaced, 2, 4, 0);
                                    }

                                    ePod.Bullets.RemoveAt(i);
                                    SpawnExplosion(Particles, 50, p.Position, b.Type, 3, 5, 0.02f);
                                    ePod.Score++;

                                    p.StartRumble(1.0f, 100);
                                }
                            }
                        }

                        for (int j = ePod.HitParticles.Count - 1; j >= 0; j--)
                        {
                            Particle part = ePod.HitParticles.ElementAt(j);
                            if (!p.IsRespawning && Vector2.Distance(p.Position, part.Position) < 50)
                            {
                                if (part.GetRectangle().Intersects(p.GetRectangle()))
                                {
                                    ePod.HitParticles.RemoveAt(j);
                                    SpawnExplosion(Particles, 5, p.Position, YellowBullet, 3, 5, 0.02f);
                                    ePod.Score++;

                                    p.StartRumble(1.0f, 100);
                                }
                            }
                        }
                    }
                }
            }
        }

        // Handles enemy collision with the pods
        public void HandlePodCollision()
        {
            // Check each enemy to each pod
            foreach (Pod p in Pods)
            {
                RemoveEnemyList.Clear();
                foreach (Enemy e in Enemies)
                {
                    if (e.Initialized)
                    {
                        if (p.GetRectangle().Intersects(e.GetRectangle()))
                        {
                            p.Die();
                            RemoveEnemyList.Add(e);
                        }
                    }
                }

                // Remove dead enemies
                foreach (Enemy e in RemoveEnemyList)
                {
                    Enemies.Remove(e);
                }
            }
        }

        // Gets the closest pod
        public Pod GetClosestPod(Vector2 pos)
        {
            Pod pod = null;
            float distance = 0;
            foreach (Pod p in Pods)
            {
                if (p.Lives > 0)
                {
                    if (distance == 0)
                    {
                        pod = p;
                        distance = Vector2.Distance(pos, pod.Position);
                    }

                    float testDistance = Vector2.Distance(pos, p.Position);

                    if (testDistance > distance)
                    {
                        pod = p;
                        distance = testDistance;
                    }
                }
            }

            return pod;
        }

        // Handles the particle collisions with the enemies
        public void HandleParticleCollisions()
        {
            // Check each enemy to each particle
            foreach (Pod pod in Pods)
            {
                foreach (Enemy e in Enemies)
                {
                    for (int i = pod.HitParticles.Count - 1; i >= 0; i--)
                    {
                        Particle p = pod.HitParticles.ElementAt(i);
                        if (e.Initialized && Vector2.Distance(p.Position, e.Position) < 30)
                        {
                            if (e.GetRotatedRectangle().Intersects(p.GetRectangle()))
                            {
                                pod.HitParticles.RemoveAt(i);
                                e.Health -= 2;
                            }

                            if (e.Health < 0)
                            {
                                ShakeScreen(100.0f);

                                SpawnExplosion(Particles, e.Type.ParticleNumber, e.Position, YellowBullet, 4, 4, 0.01f);
                                SpawnLightFade(e.Position, YellowBullet, 0.03f);
                                pod.Score += e.Type.Score;
                                pod.Energy++;

                                // Write the number of points the enemy is worth onscreen
                                Text.Add(new ScreenText(gameFont, e.Position, e.Type.Score.ToString()));

                                // Spawn a powerup if there is enough points
                                if (pod.Score % 10000 <= 5)
                                {
                                    SpawnPowerUp(e.Position);
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        // Helper method for the game
        public bool IsOnScreen(Vector2 position)
        {
            return (position.X > -100 && position.X < 1380 && position.Y < 820 && position.Y > -100);
        }
        // Helper method
        public bool BeyondBorder(Vector2 position)
        {
            return (position.X > GameRectangle.Right || position.X < GameRectangle.Left || position.Y > GameRectangle.Bottom || position.Y < GameRectangle.Top);
        }

        // Moves all the elements in the level
        public void MoveElements(Vector2 distance)
        {
            foreach (Pod p in Pods)
            {
                p.MoveElements(distance);
            }

            foreach (Enemy e in Enemies)
            {
                e.Position += distance;
            }

            foreach (Particle p in Particles)
            {
                p.Position += distance;
            }

            foreach (Particle p in TrailParticles)
            {
                p.Position += distance;
            }

            foreach (PointDot pd in PointDots)
            {
                pd.Position += distance;
            }

            TimePosition += distance;
            HUDPosition += distance;
            PodHUDPosition += distance;

            BorderPosition += distance;
        }

        // Shake Screen
        public void ShakeScreen(float timeToShake)
        {
            IsShaking = true;
            TimeToShake = timeToShake;
        }

        // Do the shaking of the screen
        public void Shake()
        {
            if (shook.Equals(Vector2.Zero))
            {
                float xDistance = (float)r.Next(-5, 5);
                float yDistance = (float)r.Next(-5, 5);

                Vector2 distance = new Vector2(xDistance, yDistance);
                MoveElements(distance);

                shook += distance;
            }
            else
            {
                MoveElements(-shook);
                shook = Vector2.Zero;
            }

        }

        // End shaking
        public void EndShaking()
        {
            MoveElements(-shook);
            shook = Vector2.Zero;
            IsShaking = false;
            shakeTime = 0;
            TimeToShake = 0;
        }

        // Handles all shaking methods
        public void HandleShaking(GameTime gameTime)
        {
            if (IsShaking)
            {
                shakeTime += gameTime.ElapsedGameTime.Milliseconds;

                Shake();
            }

            if (shakeTime > TimeToShake)
            {
                EndShaking();
            }
        }

        /// <summary>
        /// Updates the backgrounds of the level.
        /// </summary>
        private void UpdateBackgrounds()
        {
            foreach (Pod p in Pods)
            {
                if (matchType.Equals(MatchType.SinglePlayer))
                {
                    topLeft = topRight = bottomLeft = bottomRight = p.Gun.Color;
                }
                else if(matchType.Equals(MatchType.MultiPlayer))
                {
                    if (PlayerCount == 2)
                    {
                        if (p.Position.Y < 360)
                        {
                            topLeft = topRight = p.Gun.Color;
                        }
                        else
                        {
                            bottomLeft = bottomRight = p.Gun.Color;
                        }
                    }
                }
            }
        }

        // Updates all the text onscreen
        public void UpdateScreenText(GameTime gameTime)
        {
            for (int i = Text.Count - 1; i >= 0; i--)
            {
                ScreenText t = Text.ElementAt(i);
                t.Update();

                // Removes text
                if (t.Alpha <= 0)
                {
                    Text.RemoveAt(i);
                }
            }
        }

        // Updates every particle onscreen
        public void UpdateParticles(GameTime gameTime)
        {
            foreach (Particle p in Particles)
            {
                p.Update(gameTime);
            }

            foreach (Particle p in TrailParticles)
            {
                p.Update(gameTime);
            }

            UpdatePulse(gameTime);

            UpdateLiquidParticles(gameTime);
        }

        // Updates liquid particles
        public void UpdateLiquidParticles(GameTime gameTime)
        {
            // Loops through each particle
            foreach (Particle p in LiquidParticles)
            {
                /*
                // Loops through each pod
                foreach (Pod pod in Pods)
                {
                    if (Vector2.Distance(pod.Position, p.Position) < 50)
                    {
                        Vector2 angleVector = pod.Position - p.Position;
                        angleVector.Normalize();

                        float angle = (float)(Math.Atan2(Math.Abs((double)angleVector.Y), Math.Abs((double)angleVector.X)));

                        float percent = 0.05f;

                        p.Velocity.X += (float)(pod.TotalVelocity.X * Math.Cos(angle) * percent);
                        p.Velocity.Y += (float)(pod.TotalVelocity.Y * Math.Sin(angle) * percent);
                    }
                }*/

                p.Update(gameTime);
            }
        }

        /// <summary>
        /// Starts the waves.
        /// </summary>
        public void StartEnemyWaves()
        {
            spawningEnemies = true;
        }

        // Spawns an enemy in a random position
        public void SpawnEnemy(EnemyType type, Vector2 pos)
        {
            // Make a new enemy
            Enemy e = new Enemy(type, this);

            // Let the enemy use our random number generator (for randomness amongst enemies)
            e.r = this.r;

            // Choose closest pod if we behave that way
            if (type.Behavior.Equals(EnemyType.EnemyBehavior.DashTowardsPlayer) || type.Behavior.Equals(EnemyType.EnemyBehavior.MoveTowardsPlayer))
            {
                Pod pod = GetClosestPod(pos);
                e.DestinationPod = pod;
            }

            // float in random direction
            e.Position = pos;

            // Start by fading in
            e.Alpha = 0;

            Enemies.Add(e);
        }

        // Spawns x amount of enemies at the same time
        public void SpawnEnemyGroup(int count)
        {

            for (int i = 0; i < count; i++)
            {
                int ran = r.Next(4);
                if (ran == 0)
                {
                    SpawnEnemy(FollowEnemy, RandomSpawnPosition(300.0f - ((Pods.Count) * 50)));
                }
                else if (ran == 1)
                {
                    SpawnEnemy(FloatEnemy, RandomSpawnPosition(300.0f - ((Pods.Count) * 50)));
                }
                else if(ran == 2)
                {
                    SpawnEnemy(DashEnemy, RandomSpawnPosition(300.0f - ((Pods.Count) * 50)));
                }
                else if (ran == 3)
                {
                    Vector2 spawn = RandomSpawnPosition(300.0f - ((Pods.Count) * 50));
                    int ran2 = r.Next(3) + 4;
                    for (int j = 0; j < ran2; j++)
                    {
                        Vector2 spawn2 = new Vector2(spawn.X + r.Next(-30, 30), spawn.Y + r.Next(-30, 30));
                        SpawnEnemy(ClusterEnemy, spawn2);
                    }
                }
            }
        }

        // Spawns a group of one enemy type in a corner
        public void SpawnCornerGroup(int count, EnemyType type)
        {

            // Choose a corner
            int ranCorner = r.Next(4);
            Vector2 cornerPos;
            if (ranCorner == 0)
            {
                cornerPos = new Vector2(GameRectangle.Left + 50, GameRectangle.Top + 50);
            }
            else if (ranCorner == 1)
            {
                cornerPos = new Vector2(GameRectangle.Right - 50, GameRectangle.Top + 50);
            }
            else if (ranCorner == 2)
            {
                cornerPos = new Vector2(GameRectangle.Left + 50, GameRectangle.Bottom - 50);
            }
            else
            {
                cornerPos = new Vector2(GameRectangle.Right - 50, GameRectangle.Bottom - 50);
            }

            // Spawn enemies
            for (int i = 0; i < count; i++)
            {
                if (type.Equals(ClusterEnemy))
                {
                    int ran2 = r.Next(3) + 1;
                    for (int j = 0; j < ran2; j++)
                    {
                        Vector2 spawn2 = new Vector2(cornerPos.X + r.Next(-30, 30), cornerPos.Y + r.Next(-30, 30));
                        SpawnEnemy(ClusterEnemy, spawn2);
                    }
                }
                else
                {
                    Vector2 ranPos = new Vector2(cornerPos.X + r.Next(-50, 50), cornerPos.Y + r.Next(-50, 50));
                    SpawnEnemy(type, ranPos);
                }
            }
        }


        // Gets a random position on the board
        public Vector2 RandomSpawnPosition(float range)
        {
            float distance = 0;
            Vector2 pos = new Vector2();
            while (distance < range)
            {
                // reset distance
                distance = 2000;
                pos.X = (float)(r.Next(GameRectangle.Left + 25, GameRectangle.Right - 25));
                pos.Y = (float)(r.Next(GameRectangle.Top + 15, GameRectangle.Bottom - 15));
                foreach (Pod p in Pods)
                {
                    float tempDistance = Vector2.Distance(p.Position, pos);
                    if (tempDistance < distance)
                    {
                        distance = tempDistance;
                    }
                }
            }

            return pos;
        }

        // Removes dead enemies
        public void RemoveDeadEnemies()
        {
            RemoveEnemyList.Clear();

            foreach (Enemy e in Enemies)
            {
                if (e.Health < 0)
                {
                    RemoveEnemyList.Add(e);
                }
            }

            foreach (Enemy e in RemoveEnemyList)
            {
                Enemies.Remove(e);
            }
        }

        // Updates the powerups
        public void UpdatePowerups(GameTime gameTime)
        {
            for (int i = Powerups.Count - 1; i >= 0; i--)
            {
                Powerup p = Powerups.ElementAt(i);
                p.Update(gameTime);

                foreach (Pod pod in Pods)
                {
                    if (p.GetRectangle().Intersects(pod.GetRectangle()))
                    {
                        pod.ApplyPowerup(p.Type);
                        PowerUpSound.Play(Game1.Global.SoundEffectVolume, 0, 0);
                        Powerups.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        // Spawns a powerup onscreen
        public void SpawnPowerUp(Vector2 pos)
        {
            Powerup.PowerupType type = Powerup.PowerupType.FasterShots;
            Texture2D texture = null;
            Texture2D spinTexture = null;
            int ran = r.Next(2);
            if (ran == 0)
            {
                type = Powerup.PowerupType.FasterShots;
                texture = FastShoot;
                spinTexture = FastShootSpin;
            }
            else if (ran == 1)
            {
                type = Powerup.PowerupType.FourWayShooter;
                texture = FourWayShoot;
                spinTexture = FourWayShootSpin;
            }
            else if (ran == 2)
            {
                type = Powerup.PowerupType.Invincible;
            }

            Powerup p = new Powerup(pos, texture, type, this);
            p.SpinTexture = spinTexture;
            
            // Give a random starting rotation
            p.Rotation = (float)(r.Next(628) / 10.0f);

            Powerups.Add(p);
        }

        // Spawn explosion
        public void SpawnExplosion(List<Particle> ParticleList, int count, Vector2 Pos, BulletType bt, int minVel, int range, float fadePerFrame)
        {
            for(int i = 0; i < count; i++)
            {
                float rotation = ((float)r.Next(628)) / 10.0f;
                float vel = ((float)r.Next(range * 10)) / 10.0f;
                vel += (float)minVel;

                Vector2 velocity = new Vector2(vel * (float)Math.Cos(rotation), vel * (float)Math.Sin(rotation));

                // Choose the right texture
                Texture2D texture;
                Color c = Color.White;
                if (bt.Equals(RedBullet))
                {
                    texture = RedExplosionParticle;
                }
                else if (bt.Equals(BlueBullet))
                {
                    texture = BlueExplosionParticle;
                }
                else if (bt.Equals(GreenBullet))
                {
                    texture = GreenExplosionParticle;
                }
                else if(bt.Equals(YellowBullet))
                {
                    texture = YellowExplosionParticle;
                }
                else
                {
                    texture = YellowGunExplosionParticle;
                }
                Particle p = null;
                if (ParticlePool.Count > 0)
                {
                    p = ParticlePool.Pop();
                    p.Reset();
                    p.Texture = texture;
                    p.Position = Pos;
                }
                else
                {
                    p = new Particle(texture, Pos);
                }
                p.color = c;
                p.Velocity = velocity;
                p.Acceleration = -(p.Velocity / 200.0f);
                p.ExplosionParticle = true;
                if (fadePerFrame == 0)
                {
                    p.RocketParticle = true;
                }
                p.FadeDecrease = fadePerFrame;

                ParticleList.Add(p);
            }
        }

        // Spawn a light fade particle
        public void SpawnLightFade(Vector2 Pos, BulletType bt, float fadePerFrame)
        {
            // Choose the right texture
            Texture2D texture;
            if (bt.Equals(RedBullet))
            {
                texture = RedLight;
            }
            else if (bt.Equals(BlueBullet))
            {
                texture = BlueLight;
            }
            else if (bt.Equals(GreenBullet))
            {
                texture = GreenLight;
            }
            else
            {
                texture = YellowLight;
            }
            Particle p = null;
            if (ParticlePool.Count > 0)
            {
                p = ParticlePool.Pop();
                p.Reset();
                p.Texture = texture;
                p.Position = Pos;
            }
            else
            {
                p = new Particle(texture, Pos);
            }
            p.Velocity = Vector2.Zero;
            p.LightFadeParticle = true;
            p.FadeDecrease = fadePerFrame;

            Particles.Add(p);
        }

        // Spawns a light fade with set scale
        public void SpawnLightFade(Vector2 Pos, BulletType bt, float fadePerFrame, float newScale)
        {
            // Choose the right texture
            Texture2D texture;
            if (bt.Equals(RedBullet))
            {
                texture = RedLight;
            }
            else if (bt.Equals(BlueBullet))
            {
                texture = BlueLight;
            }
            else if (bt.Equals(GreenBullet))
            {
                texture = GreenLight;
            }
            else
            {
                texture = YellowLight;
            }
            Particle p = null;
            if (ParticlePool.Count > 0)
            {
                p = ParticlePool.Pop();
                p.Reset();
                p.Texture = texture;
                p.Position = Pos;
            }
            else
            {
                p = new Particle(texture, Pos);
            }
            p.Velocity = Vector2.Zero;
            p.LightFadeParticle = true;
            p.FadeDecrease = fadePerFrame;
            p.Scale = newScale;

            Particles.Add(p);
        }
        // Spawns a pulse
        public void SpawnPulse(int count, Vector2 pos)
        {
            float vel = 15.0f;

            // Spawns particles in a circle
            for (float i = 0; i < (MathHelper.Pi * 2); i += ((MathHelper.Pi * 2) / count))
            {
                Vector2 velocity = new Vector2(vel * (float)Math.Cos(i), vel * (float)Math.Sin(i));

                Particle p = new Particle(PulseTexture, pos);
                p.Velocity = velocity;

                PulseParticles.Add(p);
            }
        }

        // Handle Pulse
        public void UpdatePulse(GameTime gameTime)
        {
            foreach (Particle p in PulseParticles)
            {
                p.Update(gameTime);

                foreach (Enemy e in Enemies)
                {
                    if (Vector2.Distance(p.Position, e.Position) < 20)
                    {
                        e.Position += (p.Velocity / 2.0f);
                    }
                }
            }
        }

        // Updates imprints onscreen
        public void UpdateImprints(GameTime gameTime)
        {
            for (int i = Imprints.Count - 1; i >= 0; i--)
            {
                Imprint im = Imprints.ElementAt(i);
                im.Update();

                if (im.Alpha <= 0)
                {
                    Imprints.RemoveAt(i);
                }
            }
        }

        // Removes offscreen particles
        public void RemoveParticles()
        {

            for(int i = Particles.Count - 1; i >= 0; i--)
            {
                Particle p = Particles.ElementAt(i);
                if (!IsOnScreen(p.Position) || p.Alpha <= 0)
                {
                    Particles.RemoveAt(i);
                    ParticlePool.Push(p);
                }
            }

            for (int i = PulseParticles.Count - 1; i >= 0; i--)
            {
                Particle p = PulseParticles.ElementAt(i);
                if (!IsOnScreen(p.Position))
                {
                    PulseParticles.RemoveAt(i);
                    ParticlePool.Push(p);
                }
            }

            for (int i = TrailParticles.Count - 1; i >= 0; i--)
            {
                Particle p = TrailParticles.ElementAt(i);
                if (!IsOnScreen(p.Position) || p.Alpha <= 0)
                {
                    TrailParticles.RemoveAt(i);
                    ParticlePool.Push(p);
                }
            }

            for (int i = LiquidParticles.Count - 1; i >= 0; i--)
            {
                Particle p = LiquidParticles.ElementAt(i);
                if (p.NebulaParticle && p.LifeTime >= 600000)
                {
                    LiquidParticles.RemoveAt(i);
                    ParticlePool.Push(p);
                }
            }
        }

        // Spawns a trailing particle (for bullets)
        public void SpawnTrailingParticle(Bullet b)
        {/*
            Particle p = null;
            if (b.Type.Equals(GreenBullet))
            {
                p = new Particle(RegularParticle, b.Position);
                p.Velocity = Vector2.Zero;
                p.Position -= b.Velocity;
                Vector2 offset = new Vector2((float)(r.Next(-5, 5)), (float)(r.Next(-5, 5)));
                p.Position += offset;
                p.TrailParticle = true;
            }

            Particles.Add(p);
          * */
        }

        // Spawn a trail behind a pod
        public void SpawnTrail(int count, Texture2D texture, Vector2 Pos, float rotation, float fadePerFrame)
        {
            for (int i = 0; i < count; i++)
            {
                Particle p = null;
                if (ParticlePool.Count > 0)
                {
                    p = ParticlePool.Pop();
                    p.Reset();
                    p.Texture = texture;
                    p.Position = Pos;
                }
                else
                {
                    p = new Particle(texture, Pos);
                }
                float vel = (float)(r.Next(20) + 10) / 10.0f;
                float rotVar = (float)(r.Next(-30, 30) / 100.0f);
                p.Velocity.X = (float)(vel * Math.Cos((double)(rotation + rotVar)));
                p.Velocity.Y = (float)(vel * Math.Sin((double)(rotation + rotVar)));
                p.FadeDecrease = fadePerFrame;
                p.LightFadeParticle = true;

                TrailParticles.Add(p);
            }
        }

        // Updates the enemy bullets
        public void UpdateEnemyBullets(GameTime gameTime)
        {
            for(int i = EnemyBullets.Count - 1; i >= 0; i--)
            {
                Bullet b = EnemyBullets.ElementAt(i);

                b.Update(gameTime);

                // collisions
                foreach (Pod p in Pods)
                {
                    if (Vector2.Distance(b.Position, p.Position) < 30)
                    {
                        if (b.GetRotatedRectangle().Intersects(p.GetRectangle()))
                        {
                            p.Lives--;
                            EnemyBullets.RemoveAt(i);
                        }
                    }
                }
            }

            CleanEnemyBullets();
        }

        // Clean up enemy bullets
        public void CleanEnemyBullets()
        {
            for (int i = EnemyBullets.Count - 1; i >= 0; i--)
            {
                Bullet b = EnemyBullets.ElementAt(i);
                if (!IsOnScreen(b.Position))
                {
                    EnemyBullets.RemoveAt(i);
                }
            }
        }

        // Spawns the background liquid particles
        /*public void SpawnLiquidParticles(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Vector2 pos = RandomSpawnPosition(1);
                int ran1 = r.Next(4);
                Texture2D tex = null;
                if (ran1 == 0)
                {
                    tex = Gas1;
                }
                else if (ran1 == 1)
                {
                    tex = Gas2;
                }
                else if (ran1 == 2)
                {
                    tex = Gas3;
                }
                else if(ran1 == 3)
                {
                    tex = Gas4;
                }

                Particle p = new Particle(tex, pos);

                p.Velocity = Vector2.Zero;

                float rotation = (float)(r.Next(628)) / 100.0f;
                float ran = (float)r.Next(100) / 400.0f;

                p.ConstantVelocity = new Vector2(ran * (float)Math.Cos((double)rotation), ran * (float)Math.Sin((double)rotation));
                p.Rotation = rotation;
                p.LiquidParticle = true;

                // Get a random color
                int red = r.Next(255);
                int blue = r.Next(255);
                int green = r.Next(255);

                p.color = new Color((byte)red, (byte)green, (byte)blue);

                LiquidParticles.Add(p);
            }
        }*/

        // Spawns a group of nebula particles
        /*public void SpawnNebulaGroup(int count)
        {
            // direction choice
            int ranp = r.Next(2);
            Vector2 pos = Vector2.Zero;
            Vector2 vel = Vector2.Zero;
            if(ranp == 0)
            {
                pos = new Vector2(-600, r.Next(720));
                vel = new Vector2((float)(r.Next(5, 10) / 10.0f), (float)(r.Next(-2, 2) / 10.0f));
            }
            else if (ranp == 1)
            {
                pos = new Vector2(1880, r.Next(720));
                vel = new Vector2((float)(r.Next(5, 10) / 10.0f * -1), (float)(r.Next(-2, 2) / 10.0f));
            }

            for (int i = 0; i < count; i++)
            {
                Vector2 pos1 = new Vector2(pos.X + r.Next(-10, 10), pos.Y + r.Next(-10, 10));
                int ran1 = r.Next(8);
                Texture2D tex = null;
                if (ran1 == 0)
                {
                    tex = Nebula1;
                }
                else if (ran1 == 1)
                {
                    tex = Nebula2;
                }
                else if (ran1 == 2)
                {
                    tex = Nebula3;
                }
                else if (ran1 == 3)
                {
                    tex = Nebula4;
                }
                else if (ran1 == 4)
                {
                    tex = Nebula5;
                }
                else if (ran1 == 5)
                {
                    tex = Nebula6;
                }
                else if (ran1 == 6)
                {
                    tex = Nebula7;
                }
                else if (ran1 == 7)
                {
                    tex = Nebula8;
                }
                Particle p = new Particle(tex, pos1);

                p.Velocity = new Vector2(vel.X, vel.Y);

                float rotation = (float)(r.Next(628)) / 100.0f;

                p.Rotation = rotation;
                p.NebulaParticle = true;

                // Get a random color
                int red = r.Next(255);
                int blue = r.Next(255);
                int green = r.Next(255);

                p.color = new Color((byte)red, (byte)green, (byte)blue);

                LiquidParticles.Add(p);
            }
        }*/


        // Spawns the backgroung stars
        public void SpawnBackground(int count)
        {
            for (int i = 0; i < count; i++)
            {
                int ran = r.Next(15);
                Texture2D texture;
                if (ran == 1)
                {
                    texture = Game1.Global.Star1;
                }
                else if (ran == 2)
                {
                    texture = Game1.Global.Star3;
                }
                else if (ran == 3)
                {
                    texture = Game1.Global.Star4;
                }
                else if (ran == 4)
                {
                    texture = Game1.Global.Star5;
                }
                else
                {
                    ran = r.Next(2);
                    if (ran == 1)
                    {
                        texture = Game1.Global.Star6;
                    }
                    else
                    {
                        texture = Game1.Global.Star2;
                    }
                }

                float fade = (float)(r.Next(5) / 200.0f);
                float alpha = (float)(r.Next(100) / 100.0f);
                Vector2 pos = new Vector2((float)(r.Next(1280)), (float)(r.Next(720)));

                BackgroundStar b = new BackgroundStar(texture, pos, fade, alpha);

                BackgroundStars.Add(b);
            }
        }

        // Updates the backgrounds of the level
        public void UpdateBackground(GameTime gameTime)
        {
            // Background
            BackgroundRotation += MathHelper.Pi / 25000;

            foreach (BackgroundStar s in Layer1)
            {
                s.Update();
            }
            foreach (BackgroundStar s in Layer2)
            {
                s.Update();
            }
            foreach (BackgroundStar s in Layer3)
            {
                s.Update();
            }

            UpdateLiquidParticles(gameTime);
        }

        // The point dots you pick up ingame
        public void SpawnPointDots(int count, Vector2 pos)
        {
            for (int i = 0; i < count; i++)
            {
                PointDot pd = new PointDot(PointDotTexture, pos, 2);

                float velX = (float)(r.Next(-10, 10) / 50.0f);
                float velY = (float)(r.Next(-10, 10) / 50.0f);

                pd.Velocity = new Vector2(velX, velY);
                pd.Position += pd.Velocity;

                PointDots.Add(pd);
            }
        }

        // Updates the point dots
        public void UpdatePointDots(GameTime gameTime)
        {
            // Checks for collisions
            for (int i = PointDots.Count - 1; i >= 0; i--)
            {
                PointDot pd = PointDots.ElementAt(i);
                pd.Update(gameTime);

                foreach (Pod p in Pods)
                {
                    if (pd.Alpha > 0 && !p.IsRespawning && Vector2.Distance(p.Position, pd.Position) < 50)
                    {
                        // Get drawn towards the pod
                        if (pd.AttractingPod == null)
                        {
                            pd.AttractingPod = p;
                        }

                        if (p.GetRectangle().Intersects(pd.GetRectangle()))
                        {
                            p.Score += pd.Points;
                            // SpawnLightFade(p.Position, p.Gun.Bullet, 0.02f);

                            PointDots.RemoveAt(i);

                            break;
                        }
                    }
                }

                if (pd.Alpha <= 0)
                {
                    PointDots.RemoveAt(i);
                }
            }
        }

        #region Zone Gamemode

        // Spawns a zone if it is time to change
        public void SpawnZone()
        {
            // Gets a random position for the zone
            int ranX = r.Next(GameRectangle.Left + WhiteZoneLit.Width / 2, GameRectangle.Right - WhiteZoneLit.Width / 2);
            int ranY = r.Next(GameRectangle.Top + WhiteZoneLit.Height / 2, GameRectangle.Bottom - WhiteZoneLit.Height / 2);

            // The position of the zone
            Vector2 pos = new Vector2(ranX, ranY);

            zone = new Zone(pos);
        }

        // Updates the zones (and spawns if we need to)
        public void UpdateZones(GameTime gameTime)
        {
            // If we need to spawn a zone, do so
            if (zone == null || (zone.Scale <= 0 && zone.Shrinking))
            {
                SpawnZone();
            }

            // Are we lit?
            bool lit = false;
            foreach(Pod p  in Pods)
            {
                if (!p.IsRespawning)
                {
                    if (p.GetRectangle().Intersects(zone.GetRectangle()))
                    {
                        lit = true;

                        // Handle point increases
                        if (!p.IsRespawning)
                        {
                            p.Score++;
                        }
                    }
                }
            }
            zone.IsLit = lit;

            zone.Update(gameTime);
        }

        #endregion

        #region Travel Gamemode
        // Travel point reposition
        public void SpawnTravelPoint()
        {
            tPoint.RandomPosition();
        }
        // Updates the travel point
        public void UpdateTravelPoint()
        {
            foreach (Pod p in Pods)
            {
                if (!p.IsRespawning)
                {
                    if (p.GetRectangle().Intersects(tPoint.Rectangle))
                    {
                        // Add a point to the player
                        p.Score++;

                        // Rumble slightly
                        p.StartRumble(1.0f, 100.0f);

                        // Spawn an explosion
                        ShakeScreen(300);
                        SpawnTravelPoint();
                    }
                }
            }
        }
        #endregion

        #region Think Fast Gamemode
        public void ChangeColor()
        {
            string LastColor = CurrentColor;

            while (LastColor.Equals(CurrentColor))
            {
                int ran = r.Next(6);
                if (ran == 0)
                {
                    CurrentColor = "Green";
                    CurrentThinkFastTexture = GreenIcon;
                    CurrentThinkFastFlashTexture = GreenMode;
                }
                else if (ran == 1)
                {
                    CurrentColor = "Yellow";
                    CurrentThinkFastTexture = YellowIcon;
                    CurrentThinkFastFlashTexture = YellowMode;
                }
                else if (ran == 2)
                {
                    CurrentColor = "Red";
                    CurrentThinkFastTexture = RedIcon;
                    CurrentThinkFastFlashTexture = RedMode;
                }
                else if (ran == 3)
                {
                    CurrentColor = "Blue";
                    CurrentThinkFastTexture = BlueIcon;
                    CurrentThinkFastFlashTexture = BlueMode;
                }
                else if (ran == 4)
                {
                    CurrentColor = "Don't Shoot!";
                    CurrentThinkFastTexture = StopSIcon;
                    CurrentThinkFastFlashTexture = DontShootMode;
                }
                else if (ran == 5)
                {
                    CurrentColor = "Don't Move!";
                    CurrentThinkFastTexture = StopMIcon;
                    CurrentThinkFastFlashTexture = DontMoveMode;
                }
            }

            TimeToSwitch = r.Next(5000, 10000);

            ColorFlashScale = 0;
            ColorFlashAlpha = 1.0f;

            ThinkFastWarning.Play(Game1.Global.SoundEffectVolume, 0, 0);

            foreach (Pod p in Pods)
            {
                if (!p.IsRespawning)
                {
                    p.StartRumble(0.5f, 300.0f);
                }
            }
        }
        public void UpdateThinkFast(GameTime gameTime)
        {
            CurrentTime += gameTime.ElapsedGameTime.Milliseconds;

            if (CurrentTime > TimeToSwitch)
            {
                CurrentTime = 0;
                ChangeColor();
            }

            if (CurrentColor.Equals("Don't Shoot!"))
            {
                foreach (Pod p in Pods)
                {
                    if (!p.IsRespawning && p.ShootingTimer <= 0)
                    {
                        p.Score++;
                    }
                }
            }
            else if (CurrentColor.Equals("Don't Move!"))
            {
                foreach (Pod p in Pods)
                {
                    if (!p.IsRespawning && !p.IsMoving())
                    {
                        p.Score++;
                    }
                }
            }

            if (ColorFlashScale < 1.0f)
            {
                ColorFlashScale += 0.05f;
            }

            if (ColorFlashAlpha > 0 && ColorFlashScale >= 1.0f)
            {
                ColorFlashAlpha -= 0.02f;
            }
        }
        #endregion

        // this gets the string version of matchtype

        // This ends the match (sets highscores, etc)
        public void EndMatch()
        {
            HighscoreComponent.Global.Enabled = true;

            Game1.Global.data_ = new Highscore[20];

            string matchTypeString = matchType.ToString();

            foreach (Pod p in Pods)
            {
                if (matchType.Equals(MatchType.Arcade))
                {
                    p.Score += (p.Lives * 1000);
                }

                if (p.Lives > 0)
                {
                    p.Die();
                }
            }

            MatchEnded = true;
        }
    }
}
