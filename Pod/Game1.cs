using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Pod
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Level level;

        // This allows access to the game from anywhere in the game
        public static Game1 Global;

        // Which players have been chosen?
        private bool PlayerOneChosen;
        private bool PlayerTwoChosen;
        private bool PlayerThreeChosen;
        private bool PlayerFourChosen;

        // Planets
        private Texture2D Planet1;
        private Texture2D Planet2;
        private Texture2D Planet3;
        private Texture2D Planet4;
        private Texture2D Planet5;
        private Texture2D Planet6;

        // Stars
        public Texture2D Star1;
        public Texture2D Star2;
        public Texture2D Star3;
        public Texture2D Star4;
        public Texture2D Star5;
        public Texture2D Star6;

        // Game pad states
        public GamePadState gps1;
        public GamePadState gps2;
        public GamePadState gps3;
        public GamePadState gps4;

        private SpriteFont gameFont;
        private SpriteFont gameFontLarge;
        private SpriteFont gameFontExtraLarge;

        // The vector2 in the center
        public static Vector2 CenterVector;

        // The highscore array
        public Highscore[] data_;
        public Highscore[] localScores_;

        // Rumble option
        public bool IsRumbleOn;

        // The controller in control of the pause menu
        public PlayerIndex PausedController;

        public enum GameState
        {
            MainMenu,
            Loading,
            Level,
            StartLevel,
            PlayerSelect,
            BadEggStudios,
            GameSelect,
            Highscores,
            Options,
            Extras,
            Exit,
            GameModeTutorial,
            PlayerTutorial,
            EnemyTutorial,
            ControlsTutorial,
            Paused
        }
        public GameState gameState;

        private Level.MatchType gameType;

        // Menu song
        public Song MenuSong;

        // Menu Timer
        public float MenuTimer;
        public float ChangeElementTime;

        // Pause menu
        public string PauseElementSelected;
        public Texture2D PauseTexture;
        public Texture2D ArrowTexture;

        // Volumes
        public float MusicVolume;
        public float SoundEffectVolume;
        public int MusicVolumeInt;
        public int SoundEffectVolumeInt;

        // Menu Textures
        public string MenuElementSelected;
        public string GameElementSelected;
        public string HighscoresElementSelected;
        public string OptionsElementSelected;

        public MenuElement MenuBackground;
        public MenuElement MenuLines;
        public MenuElement MenuBox;

        public MenuElement MenuPlayerSelect;

        public MenuElement Extras;

        public MenuElement AButton;
        public MenuElement BButton;
        public MenuElement YButton;
        public MenuElement XButton;

        public MenuElement ArcadeBox;
        public MenuElement VersusBox;
        public MenuElement ThinkFastBox;
        public MenuElement WaypointBox;
        public MenuElement ZonesBox;
        public MenuElement SurvivalBox;

        public MenuElement BlockedVersusBox;
        public MenuElement BlockedThinkFastBox;
        public MenuElement BlockedZonesBox;
        public MenuElement BlockedSurvivalBox;

        public MenuElement HighscoresBox;

        public MenuElement GameModeBoxSelect;

        public MenuElement TitleLogo;
        public MenuElement TitleLogoBottom;

        public Vector2 MainMenuPosition;
        public Vector2 PlayerSelectPosition;
        public Vector2 GameSelectPosition;
        public Vector2 HighscoresPosition;
        public Vector2 OptionsPosition;
        public Vector2 ExtrasPosition;
        public Vector2 GameModeTutorialPosition;
        public Vector2 PlayerTutorialPosition;
        public Vector2 EnemyTutorialPosition;
        public Vector2 ControlsTutorialPosition;

        public List<Particle> Planets;
        public List<BackgroundStar> Layer1;

        public float ColorPulseAmount;
        public float ColorPulseSpeed;
        public Color MenuColor;
        public Color NewColor;
        public Color LastColor;

        public float SlideSpeed;
        public float SlideAcceleration;
        public float ColorTransitionSpeed;

        public bool AButtonReady;
        public bool StartButtonReady;
        public bool BackButtonReady;
        public bool BButtonReady;
        public bool YButtonReady;

        public MenuElement ArcadeTutorial;
        public MenuElement SurvivalTutorial;
        public MenuElement ZonesTutorial;
        public MenuElement WaypointTutorial;
        public MenuElement ThinkFastTutorial;
        public MenuElement VersusTutorial;

        public MenuElement GameModeTutorial;
        public MenuElement PlayerTutorial;
        public MenuElement EnemyTutorial;

        public MenuElement PlayerScreen;
        public MenuElement EnemyScreen;

        public MenuElement ControlsScreen;
        public MenuElement ControlsScreen2;

        // Element positions
        public Vector2 ArrowDrawPosition;
        public Vector2 SelectedPosition;

        // Bad Egg Studios
        public Texture2D BadEggStudios;
        public float BadEggStudiosTimer;

        // Menu sounds
        public SoundEffect MenuScrollSound;
        public SoundEffect MenuClickSound;

        // Player select button ready
        public bool SelectButtonOneReady;
        public bool SelectButtonTwoReady;
        public bool SelectButtonThreeReady;
        public bool SelectButtonFourReady;

        // Random
        public Random r;

        // General Menu
        public Texture2D BlueBox;

        public float GameStartFade;

        // Is this a trial game?
        public bool IsTrial;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";

            // Lets us access this game object anywhere
            Global = this;

            // Initialize
            Components.Add(new GamerServicesComponent(this));

            CenterVector = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);

            MediaPlayer.IsVisualizationEnabled = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            r = new Random();

            gameState = GameState.BadEggStudios;
            gameType = Level.MatchType.Arcade;

            ResetPlayerSelect();

            SelectButtonOneReady = false;
            SelectButtonTwoReady = false;
            SelectButtonThreeReady = false;
            SelectButtonFourReady = false;

            // Loads the pause menu textures
            MenuTimer = 0;

            MusicVolume = 0.5f;
            SoundEffectVolume = 0.9f;
            MusicVolumeInt = 50;
            SoundEffectVolumeInt = 90;

            // Highscores
            data_ = new Highscore[20];
            localScores_ = new Highscore[10];

            IsRumbleOn = true;

            GameStartFade = 0;

            base.Initialize();
        }

        /// <summary>
        /// Resets the player select screen.
        /// </summary>
        private void ResetPlayerSelect()
        {
            // We have not chosen any player yet
            PlayerOneChosen = false;
            PlayerTwoChosen = false;
            PlayerThreeChosen = false;
            PlayerFourChosen = false;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            gameFont = Content.Load<SpriteFont>("Fonts/Quartz");
            gameFontLarge = Content.Load<SpriteFont>("Fonts/QuartzLarge");
            gameFontExtraLarge = Content.Load<SpriteFont>("Fonts/QuartzExtraLarge");

            // Sounds
            MenuClickSound = Content.Load<SoundEffect>("Sound/MenuSelection");
            MenuScrollSound = Content.Load<SoundEffect>("Sound/MenuScroll");

            // BES
            BadEggStudios = Content.Load<Texture2D>("Graphics/Extras/bes_splash");
            BadEggStudiosTimer = 2500;

            PauseTexture = Content.Load<Texture2D>("Graphics/Menu/PauseMenu");
            ArrowTexture = Content.Load<Texture2D>("Graphics/Menu/SelectWhiteArrow");

            BlueBox = Content.Load<Texture2D>("Graphics/Menu/BlueMenuBox");

            // Stars
            Star1 = Content.Load<Texture2D>("Graphics/Backgrounds/Stars/Star1");
            Star2 = Content.Load<Texture2D>("Graphics/Backgrounds/Stars/Star2");
            Star3 = Content.Load<Texture2D>("Graphics/Backgrounds/Stars/Star3");
            Star4 = Content.Load<Texture2D>("Graphics/Backgrounds/Stars/Star4");
            Star5 = Content.Load<Texture2D>("Graphics/Backgrounds/Stars/Star5");
            Star6 = Content.Load<Texture2D>("Graphics/Backgrounds/Stars/Star6");

            // Planets
            Planet1 = Content.Load<Texture2D>("Graphics/Backgrounds/Planets/Planet1");
            Planet2 = Content.Load<Texture2D>("Graphics/Backgrounds/Planets/Planet2");
            Planet3 = Content.Load<Texture2D>("Graphics/Backgrounds/Planets/Planet3");
            Planet4 = Content.Load<Texture2D>("Graphics/Backgrounds/Planets/Planet4");
            Planet5 = Content.Load<Texture2D>("Graphics/Backgrounds/Planets/Planet5");
            Planet6 = Content.Load<Texture2D>("Graphics/Backgrounds/Planets/Planet6");

            LoadMenus();
        }

        public void LoadMenus()
        {
            MenuSong = Content.Load<Song>("Music/Edgy_Crowd_full_mix");
            MediaPlayer.Volume = 0.5f;
            MediaPlayer.IsRepeating = true;

            MenuBackground = new MenuElement(Content.Load<Texture2D>("Graphics/Menu/MenuBackground"));
            MenuLines = new MenuElement(Content.Load<Texture2D>("Graphics/Menu/MenuLines"));
            MenuBox = new MenuElement(Content.Load<Texture2D>("Graphics/Menu/WhiteMenuBox"));

            MenuPlayerSelect = new MenuElement(Content.Load<Texture2D>("Graphics/Menu/LinesPlayerSelect"));

            Extras = new MenuElement(Content.Load<Texture2D>("Graphics/Menu/Credits"));

            ArcadeBox = new MenuElement(Content.Load<Texture2D>("Graphics/Menu/ArcadeBox"));
            SurvivalBox = new MenuElement(Content.Load<Texture2D>("Graphics/Menu/SurvivalBox"));
            ThinkFastBox = new MenuElement(Content.Load<Texture2D>("Graphics/Menu/ThinkFastBox"));
            WaypointBox = new MenuElement(Content.Load<Texture2D>("Graphics/Menu/WaypointBox"));
            ZonesBox = new MenuElement(Content.Load<Texture2D>("Graphics/Menu/BoxZone"));
            VersusBox = new MenuElement(Content.Load<Texture2D>("Graphics/Menu/VersusBox"));

            BlockedSurvivalBox = new MenuElement(Content.Load<Texture2D>("Graphics/Menu/BlockedSurvivalBox"));
            BlockedThinkFastBox = new MenuElement(Content.Load<Texture2D>("Graphics/Menu/BlockedThinkFastBox"));
            BlockedZonesBox = new MenuElement(Content.Load<Texture2D>("Graphics/Menu/BlockedZoneBox"));
            BlockedVersusBox = new MenuElement(Content.Load<Texture2D>("Graphics/Menu/BlockedVersusBox"));

            AButton = new MenuElement(Content.Load<Texture2D>("Graphics/Menu/AButton"));
            BButton = new MenuElement(Content.Load<Texture2D>("Graphics/Menu/BButton"));
            XButton = new MenuElement(Content.Load<Texture2D>("Graphics/Menu/XButton"));
            YButton = new MenuElement(Content.Load<Texture2D>("Graphics/Menu/YButton"));

            GameModeBoxSelect = new MenuElement(Content.Load<Texture2D>("Graphics/Menu/GameModeBoxSelect"));

            HighscoresBox = new MenuElement(Content.Load<Texture2D>("Graphics/Menu/LinesHighscores"));

            TitleLogo = new MenuElement(Content.Load<Texture2D>("Graphics/Menu/LogoTop"));
            TitleLogoBottom = new MenuElement(Content.Load<Texture2D>("Graphics/Menu/LogoBot"));
            
            // Positions
            MainMenuPosition = Vector2.Zero;
            PlayerSelectPosition = new Vector2(2554, 0);
            GameSelectPosition = new Vector2(1277, 0);
            HighscoresPosition = new Vector2(-1277, 6);
            OptionsPosition = new Vector2(-2554, 0);
            ExtrasPosition = new Vector2(-3831, 0);
            GameModeTutorialPosition = new Vector2(3831, 0);
            PlayerTutorialPosition = new Vector2(5108, 0);
            EnemyTutorialPosition = new Vector2(6385, 0);
            ControlsTutorialPosition = new Vector2(7662, 0);

            ColorPulseAmount = 0;
            ColorPulseSpeed = 0.0025f;
            MenuColor = Color.Green;
            LastColor = Color.Green;
            NewColor = Color.Blue;

            SlideSpeed = 85.0f;
            SlideAcceleration = 0.5f;
            ColorTransitionSpeed = 0.09f;

            AButtonReady = true;
            BButtonReady = true;
            StartButtonReady = true;
            BackButtonReady = true;
            YButtonReady = true;

            Planets = new List<Particle>();
            Layer1 = new List<BackgroundStar>();

            SpawnPlanets(Planets, r.Next(3) + 1);
            SpawnBackgroundLayer(600, Layer1);

            MenuElementSelected = "Play";
            GameElementSelected = "Arcade";
            HighscoresElementSelected = "Arcade";
            OptionsElementSelected = "Music";
            ChangeElementTime = 200;


            // Tutorials
            ArcadeTutorial = new MenuElement(Content.Load<Texture2D>("Graphics/Tutorials/Arcade"));
            SurvivalTutorial = new MenuElement(Content.Load<Texture2D>("Graphics/Tutorials/Survival"));
            ThinkFastTutorial = new MenuElement(Content.Load<Texture2D>("Graphics/Tutorials/ThinkFast"));
            ZonesTutorial = new MenuElement(Content.Load<Texture2D>("Graphics/Tutorials/Zones"));
            VersusTutorial = new MenuElement(Content.Load<Texture2D>("Graphics/Tutorials/Versus"));
            WaypointTutorial = new MenuElement(Content.Load<Texture2D>("Graphics/Tutorials/Waypoint"));

            PlayerTutorial = new MenuElement(Content.Load<Texture2D>("Graphics/Tutorials/LinesPlayerEx"));
            GameModeTutorial = new MenuElement(Content.Load<Texture2D>("Graphics/Tutorials/LinesGameModeEx"));
            EnemyTutorial = new MenuElement(Content.Load<Texture2D>("Graphics/Tutorials/LinesEnemyEx"));

            PlayerScreen = new MenuElement(Content.Load<Texture2D>("Graphics/Tutorials/PlayerEx"));
            EnemyScreen = new MenuElement(Content.Load<Texture2D>("Graphics/Tutorials/EnemyEx"));
            ControlsScreen = new MenuElement(Content.Load<Texture2D>("Graphics/Tutorials/ControlsEx"));
            ControlsScreen2 = new MenuElement(Content.Load<Texture2D>("Graphics/Tutorials/ControlsEx2"));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Is this a trial?
            IsTrial = Guide.IsTrialMode;

            // The gamepad states of all the players
            gps1 = GamePad.GetState(PlayerIndex.One);
            gps2 = GamePad.GetState(PlayerIndex.Two);
            gps3 = GamePad.GetState(PlayerIndex.Three);
            gps4 = GamePad.GetState(PlayerIndex.Four);

            // Menu Timer
            MenuTimer += gameTime.ElapsedGameTime.Milliseconds;

            // Volume
            if (!MediaPlayer.Volume.Equals(MusicVolume))
            {
                MediaPlayer.Volume = MusicVolume;
            }

            bool AButtonHit = (gps1.IsButtonDown(Buttons.A) 
                || gps2.IsButtonDown(Buttons.A) || gps2.IsButtonDown(Buttons.Start) 
                || gps3.IsButtonDown(Buttons.A) || gps3.IsButtonDown(Buttons.Start) 
                || gps4.IsButtonDown(Buttons.A) || gps4.IsButtonDown(Buttons.Start));

            bool StartButtonHit = (gps1.IsButtonDown(Buttons.Start)
                 || gps2.IsButtonDown(Buttons.Start)
                 || gps3.IsButtonDown(Buttons.Start)
                 || gps4.IsButtonDown(Buttons.Start));

            bool BButtonHit = (gps1.IsButtonDown(Buttons.B)
                || gps2.IsButtonDown(Buttons.B)
                || gps3.IsButtonDown(Buttons.B) 
                || gps4.IsButtonDown(Buttons.B));

            bool BackButtonHit = (gps1.IsButtonDown(Buttons.Back)
                 || gps2.IsButtonDown(Buttons.Back)
                 || gps3.IsButtonDown(Buttons.Back)
                 || gps4.IsButtonDown(Buttons.Back));

            bool DownButtonHit = (gps1.ThumbSticks.Left.Y < -0.5f || gps1.IsButtonDown(Buttons.DPadDown)
                || gps2.ThumbSticks.Left.Y < -0.5f || gps2.IsButtonDown(Buttons.DPadDown)
                || gps3.ThumbSticks.Left.Y < -0.5f || gps3.IsButtonDown(Buttons.DPadDown)
                || gps4.ThumbSticks.Left.Y < -0.5f || gps4.IsButtonDown(Buttons.DPadDown));

            bool UpButtonHit = (gps1.ThumbSticks.Left.Y > 0.5f || gps1.IsButtonDown(Buttons.DPadUp)
                || gps2.ThumbSticks.Left.Y > 0.5f || gps2.IsButtonDown(Buttons.DPadUp)
                || gps3.ThumbSticks.Left.Y > 0.5f || gps3.IsButtonDown(Buttons.DPadUp)
                || gps4.ThumbSticks.Left.Y > 0.5f || gps4.IsButtonDown(Buttons.DPadUp));

            bool LeftButtonHit = (gps1.ThumbSticks.Left.X < -0.5f || gps1.IsButtonDown(Buttons.DPadLeft)
                || gps2.ThumbSticks.Left.X < -0.5f || gps2.IsButtonDown(Buttons.DPadLeft)
                || gps3.ThumbSticks.Left.X < -0.5f || gps3.IsButtonDown(Buttons.DPadLeft)
                || gps4.ThumbSticks.Left.X < -0.5f || gps4.IsButtonDown(Buttons.DPadLeft));

            bool RightButtonHit = (gps1.ThumbSticks.Left.X > 0.5f || gps1.IsButtonDown(Buttons.DPadRight)
                || gps2.ThumbSticks.Left.X > 0.5f || gps2.IsButtonDown(Buttons.DPadRight)
                || gps3.ThumbSticks.Left.X > 0.5f || gps3.IsButtonDown(Buttons.DPadRight)
                || gps4.ThumbSticks.Left.X > 0.5f || gps4.IsButtonDown(Buttons.DPadRight));

            bool YButtonHit = (gps1.IsButtonDown(Buttons.Y)
                 || gps2.IsButtonDown(Buttons.Y)
                 || gps3.IsButtonDown(Buttons.Y)
                 || gps4.IsButtonDown(Buttons.Y));

            // Player select readys
            if (!gps1.IsButtonDown(Buttons.Y))
            {
                SelectButtonOneReady = true;
            }
            if (!gps2.IsButtonDown(Buttons.Y))
            {
                SelectButtonTwoReady = true;
            }
            if (!gps3.IsButtonDown(Buttons.Y))
            {
                SelectButtonThreeReady = true;
            }
            if (!gps4.IsButtonDown(Buttons.Y))
            {
                SelectButtonFourReady = true;
            }


            if (!AButtonHit)
            {
                AButtonReady = true;
            }
            if (!BackButtonHit)
            {
                BackButtonReady = true;
            }
            if (!StartButtonHit)
            {
                StartButtonReady = true;
            }
            if (!BButtonHit)
            {
                BButtonReady = true;
            }
            if (!YButtonHit)
            {
                YButtonReady = true;
            }

            // Update highscores
            if (level != null)
            {
                // Update the highscore component
                level.hsc_.Update(gameTime);
            }

            switch (gameState)
            {
                case GameState.BadEggStudios:
                    {
                        if (BadEggStudiosTimer == 2500)
                        {
                            MediaPlayer.Play(MenuSong);
                        }

                        // Reduce the timer
                        if (!Guide.IsVisible)
                        {
                            BadEggStudiosTimer -= 16;
                        }

                        if (BadEggStudiosTimer < 0)
                        {
                            gameState = GameState.MainMenu;
                        }

                        // If there is no level loaded, load ALL the level data (AND HIGHSCORES)
                        if (level == null && BadEggStudiosTimer < 0)
                        {
                            // Initialize highscores
                            HighscoreComponent hsc = new HighscoreComponent(this, null, "Pew Pew Pod");
                            Components.Add(hsc);

                            level = new Level(Content);
                            level.LoadContent();
                            level.Initialize();
                        }
                        break;
                    }
                case GameState.Exit:
                    {
                        this.Exit();
                        break;
                    }
                case GameState.Extras:
                    {
                        // Color rotation
                        if (!MenuColor.Equals(Color.Yellow))
                        {
                            MenuColor = Color.Lerp(MenuColor, Color.Yellow, ColorTransitionSpeed);
                        }

                        if (ExtrasPosition.X < 0)
                        {
                            MoveMenus(SlideSpeed);
                        }
                        else if (ExtrasPosition.X > 0)
                        {
                            MoveMenus(-SlideSpeed);
                        }
                        else
                        {
                            if (BButtonHit && BButtonReady)
                            {
                                BButtonReady = false;

                                MenuClickSound.Play(Game1.Global.SoundEffectVolume, 0, 0);

                                gameState = GameState.MainMenu;
                            }
                        }

                        if (Math.Abs(ExtrasPosition.X) < SlideSpeed)
                        {
                            MoveMenus(-ExtrasPosition.X);
                        }

                        break;
                    }
                case GameState.Options:
                    {
                        // Color rotation
                        if (!MenuColor.Equals(Color.Purple))
                        {
                            MenuColor = Color.Lerp(MenuColor, Color.Purple, ColorTransitionSpeed);
                        }

                        if (OptionsPosition.X < 0)
                        {
                            MoveMenus(SlideSpeed);
                        }
                        else if (OptionsPosition.X > 0)
                        {
                            MoveMenus(-SlideSpeed);
                        }
                        else
                        {
                            if (BButtonHit && BButtonReady)
                            {
                                BButtonReady = false;

                                MenuClickSound.Play(Game1.Global.SoundEffectVolume, 0, 0);

                                gameState = GameState.MainMenu;
                            }

                            if (BackButtonHit && BackButtonReady)
                            {
                                BackButtonReady = false;
                                level.hsc_.userWantsToLoad_ = true;
                                level.hsc_.storage_ = null;

                                data_ = new Highscore[10];
                                localScores_ = new Highscore[10];
                            }

                            if (DownButtonHit && MenuTimer > ChangeElementTime)
                            {
                                switch (OptionsElementSelected)
                                {
                                    case "Music":
                                        {
                                            OptionsElementSelected = "SoundEffects";
                                            break;
                                        }
                                    case "SoundEffects":
                                        {
                                            OptionsElementSelected = "Rumble";
                                            break;
                                        }
                                    case "Rumble":
                                        {
                                            OptionsElementSelected = "Music";
                                            break;
                                        }
                                }

                                MenuTimer = 0;
                            }
                            else if (UpButtonHit && MenuTimer > ChangeElementTime)
                            {
                                switch (OptionsElementSelected)
                                {
                                    case "Music":
                                        {
                                            OptionsElementSelected = "Rumble";
                                            break;
                                        }
                                    case "SoundEffects":
                                        {
                                            OptionsElementSelected = "Music";
                                            break;
                                        }
                                    case "Rumble":
                                        {
                                            OptionsElementSelected = "SoundEffects";
                                            break;
                                        }
                                }

                                MenuTimer = 0;
                            }

                            if (RightButtonHit && MenuTimer > ChangeElementTime)
                            {
                                switch (OptionsElementSelected)
                                {
                                    case "Music":
                                        {
                                            MusicVolumeInt += 5;
                                            if (MusicVolumeInt > 100)
                                            {
                                                MusicVolumeInt = 100;
                                            }
                                            break;
                                        }
                                    case "SoundEffects":
                                        {
                                            SoundEffectVolumeInt += 5;
                                            if (SoundEffectVolumeInt > 100)
                                            {
                                                SoundEffectVolumeInt = 100;
                                            }
                                            break;
                                        }
                                    case "Rumble":
                                        {
                                            IsRumbleOn = !IsRumbleOn;
                                            break;
                                        }
                                }

                                MenuTimer = 0;
                            }
                            else if (LeftButtonHit && MenuTimer > ChangeElementTime)
                            {
                                switch (OptionsElementSelected)
                                {
                                    case "Music":
                                        {
                                            MusicVolumeInt -= 5;
                                            if (MusicVolumeInt < 0)
                                            {
                                                MusicVolumeInt = 0;
                                            }
                                            break;
                                        }
                                    case "SoundEffects":
                                        {
                                            SoundEffectVolumeInt -= 5;
                                            if (SoundEffectVolumeInt < 0)
                                            {
                                                SoundEffectVolumeInt = 0;
                                            }
                                            break;
                                        }
                                    case "Rumble":
                                        {
                                            IsRumbleOn = !IsRumbleOn;
                                            break;
                                        }
                                }

                                MenuTimer = 0;
                            }
                        }

                        if (Math.Abs(OptionsPosition.X) < SlideSpeed)
                        {
                            MoveMenus(-OptionsPosition.X);
                        }

                        // Set sounds
                        MusicVolume = ((float)MusicVolumeInt) / 100.0f;
                        SoundEffectVolume = ((float)SoundEffectVolumeInt) / 100.0f;

                        break;
                    }
                case GameState.Highscores:
                    {
                        // Color rotation
                        if (!MenuColor.Equals(Color.Green))
                        {
                            MenuColor = Color.Lerp(MenuColor, Color.Green, ColorTransitionSpeed);
                        }

                        if (HighscoresPosition.X < 0)
                        {
                            MoveMenus(SlideSpeed);
                        }
                        else if (HighscoresPosition.X > 0)
                        {
                            MoveMenus(-SlideSpeed);
                        }
                        else
                        {
                            if (BButtonHit && BButtonReady)
                            {
                                BButtonReady = false;

                                MenuClickSound.Play(Game1.Global.SoundEffectVolume, 0, 0);

                                gameState = GameState.MainMenu;
                            }

                            // Rotate highscores
                            if (AButtonHit && AButtonReady)
                            {
                                AButtonReady = false;

                                switch (HighscoresElementSelected)
                                {
                                    case "Arcade":
                                        {
                                            HighscoresElementSelected = "Survival";
                                            break;
                                        }
                                    case "Survival":
                                        {
                                            HighscoresElementSelected = "Zones";
                                            break;
                                        }
                                    case "Zones":
                                        {
                                            HighscoresElementSelected = "Waypoint";
                                            break;
                                        }
                                    case "Waypoint":
                                        {
                                            HighscoresElementSelected = "ThinkFast";
                                            break;
                                        }
                                    case "ThinkFast":
                                        {
                                            HighscoresElementSelected = "Arcade";
                                            break;
                                        }
                                }
                            }

                            // Choose storage
                            if (BackButtonHit && BackButtonReady)
                            {
                                level.hsc_.userWantsToLoad_ = true;
                                BackButtonReady = false;
                            }
                        }

                        if (Math.Abs(HighscoresPosition.X) < SlideSpeed)
                        {
                            MoveMenus(-HighscoresPosition.X);
                        }

                        if (!HighscoresElementSelected.Equals("Waypoint"))
                        {
                            RebuildLocalHighscores(HighscoresElementSelected);
                            RebuildHighscores(HighscoresElementSelected);
                        }
                        else
                        {
                            RebuildLocalHighscores("Travel");
                            RebuildHighscores("Travel");
                        }

                        break;
                    }
                case GameState.MainMenu:
                    {
                        // Color rotation
                        if (LastColor.Equals(Color.Green))
                        {
                            if (!MenuColor.Equals(Color.Turquoise))
                            {
                                MenuColor = Color.Lerp(Color.Green, Color.Turquoise, ColorPulseAmount);
                                ColorPulseAmount += ColorPulseSpeed;
                            }
                            else
                            {
                                LastColor = Color.Turquoise;
                                ColorPulseAmount = 0;
                            }
                        }
                        else if (LastColor.Equals(Color.Turquoise))
                        {
                            if (!MenuColor.Equals(Color.Red))
                            {
                                MenuColor = Color.Lerp(Color.Turquoise, Color.Red, ColorPulseAmount);
                                ColorPulseAmount += ColorPulseSpeed;
                            }
                            else
                            {
                                LastColor = Color.Red;
                                ColorPulseAmount = 0;
                            }
                        }
                        else if (LastColor.Equals(Color.Red))
                        {
                            if (!MenuColor.Equals(Color.Yellow))
                            {
                                MenuColor = Color.Lerp(Color.Red, Color.Yellow, ColorPulseAmount);
                                ColorPulseAmount += ColorPulseSpeed;
                            }
                            else
                            {
                                LastColor = Color.Yellow;
                                ColorPulseAmount = 0;
                            }
                        }
                        if (LastColor.Equals(Color.Yellow))
                        {
                            if (!MenuColor.Equals(Color.Green))
                            {
                                MenuColor = Color.Lerp(Color.Yellow, Color.Green, ColorPulseAmount);
                                ColorPulseAmount += ColorPulseSpeed;
                            }
                            else
                            {
                                LastColor = Color.Green;
                                ColorPulseAmount = 0;
                            }
                        }

                        if (MainMenuPosition.X < 0)
                        {
                            MoveMenus(SlideSpeed);
                        }
                        else if (MainMenuPosition.X > 0)
                        {
                            MoveMenus(-SlideSpeed);
                        }
                        else
                        {
                            if (AButtonHit && AButtonReady)
                            {
                                AButtonReady = false;

                                MenuClickSound.Play(Game1.Global.SoundEffectVolume, 0, 0);

                                switch (MenuElementSelected)
                                {
                                    case "Play":
                                        {
                                            gameState = GameState.GameSelect;

                                            // Change the selected element
                                            GameElementSelected = "Arcade";
                                            break;
                                        }
                                    case "Highscores":
                                        {
                                            gameState = GameState.Highscores;
                                            break;
                                        }
                                    case "Options":
                                        {
                                            gameState = GameState.Options;
                                            break;
                                        }
                                    case "Extras":
                                        {
                                            gameState = GameState.Extras;
                                            break;
                                        }
                                    case "Exit":
                                        {
                                            gameState = GameState.Exit;
                                            break;
                                        }
                                }
                            }

                            // Are we trying to buy the game?
                            if (gps1.IsButtonDown(Buttons.X))
                            {
                                if (CanPlayerBuyGame(PlayerIndex.One))
                                {
                                    Guide.ShowMarketplace(PlayerIndex.One);
                                }
                                else
                                {
                                    SimpleMessageBox.ShowMessageBox("Purchase Pew Pew Pod",
                                        "You must be logged into a profile that has purchasing priviledges in order to buy Pew Pew Pod. Please try again with a different profile.",
                                        new string[] { "Ok" }, 0, MessageBoxIcon.Warning);
                                }
                            }
                            else if (gps2.IsButtonDown(Buttons.X))
                            {
                                if (CanPlayerBuyGame(PlayerIndex.Two))
                                {
                                    Guide.ShowMarketplace(PlayerIndex.Two);
                                }
                                else
                                {
                                    SimpleMessageBox.ShowMessageBox("Purchase Pew Pew Pod",
                                        "You must be logged into a profile that has purchasing priviledges in order to buy Pew Pew Pod. Please try again with a different profile.",
                                        new string[] { "Ok" }, 0, MessageBoxIcon.Warning);
                                }
                            }
                            else if (gps3.IsButtonDown(Buttons.X))
                            {
                                if (CanPlayerBuyGame(PlayerIndex.Three))
                                {
                                    Guide.ShowMarketplace(PlayerIndex.Three);
                                }
                                else
                                {
                                    SimpleMessageBox.ShowMessageBox("Purchase Pew Pew Pod",
                                        "You must be logged into a profile that has purchasing priviledges in order to buy Pew Pew Pod. Please try again with a different profile.",
                                        new string[] { "Ok" }, 0, MessageBoxIcon.Warning);
                                }
                            }
                            else if (gps4.IsButtonDown(Buttons.X))
                            {
                                if (CanPlayerBuyGame(PlayerIndex.Four))
                                {
                                    Guide.ShowMarketplace(PlayerIndex.Four);
                                }
                                else
                                {
                                    SimpleMessageBox.ShowMessageBox("Purchase Pew Pew Pod",
                                        "You must be logged into a profile that has purchasing priviledges in order to buy Pew Pew Pod. Please try again with a different profile.",
                                        new string[] { "Ok" }, 0, MessageBoxIcon.Warning);
                                }
                            }

                            // Highscores
                            if (BackButtonHit && BackButtonReady)
                            {
                                level.hsc_.userWantsToLoad_ = true;
                                BackButtonReady = false;
                            }
                        }

                        if (Math.Abs(MainMenuPosition.X) < SlideSpeed)
                        {
                            MoveMenus(-MainMenuPosition.X);
                        }

                        // Menu selection
                        if (UpButtonHit && MenuTimer > ChangeElementTime)
                        {
                            switch (MenuElementSelected)
                            {
                                case "Play":
                                    {
                                        MenuElementSelected = "Exit";
                                        break;
                                    }
                                case "Highscores":
                                    {
                                        MenuElementSelected = "Play";
                                        break;
                                    }
                                case "Options":
                                    {
                                        MenuElementSelected = "Highscores";
                                        break;
                                    }
                                case "Extras":
                                    {
                                        MenuElementSelected = "Options";
                                        break;
                                    }
                                case "Exit":
                                    {
                                        MenuElementSelected = "Extras";
                                        break;
                                    }
                            }

                            MenuScrollSound.Play(Game1.Global.SoundEffectVolume, 0, 0);

                            MenuTimer = 0;
                        }
                        else if (DownButtonHit && MenuTimer > ChangeElementTime)
                        {
                            switch (MenuElementSelected)
                            {
                                case "Play":
                                    {
                                        MenuElementSelected = "Highscores";
                                        break;
                                    }
                                case "Highscores":
                                    {
                                        MenuElementSelected = "Options";
                                        break;
                                    }
                                case "Options":
                                    {
                                        MenuElementSelected = "Extras";
                                        break;
                                    }
                                case "Extras":
                                    {
                                        MenuElementSelected = "Exit";
                                        break;
                                    }
                                case "Exit":
                                    {
                                        MenuElementSelected = "Play";
                                        break;
                                    }
                            }

                            MenuScrollSound.Play(Game1.Global.SoundEffectVolume, 0, 0);

                            MenuTimer = 0;
                        }

                        break;
                    }
                case GameState.GameSelect:
                    {
                        if (!MenuColor.Equals(Color.Turquoise))
                        {
                            MenuColor = Color.Lerp(MenuColor, Color.Turquoise, ColorTransitionSpeed);
                        }

                        if (GameSelectPosition.X < 0)
                        {
                            MoveMenus(SlideSpeed);
                        }
                        else if (GameSelectPosition.X > 0)
                        {
                            MoveMenus(-SlideSpeed);
                        }
                        else
                        {
                            if (BButtonHit && BButtonReady)
                            {
                                gameState = GameState.MainMenu;
                                BButtonReady = false;

                                // Change selected Element
                                MenuElementSelected = "Play";
                            }

                            if (AButtonHit && AButtonReady)
                            {
                                gameState = GameState.PlayerSelect;
                                AButtonReady = false;

                                // Reset player selects
                                PlayerOneChosen = false;
                                PlayerTwoChosen = false;
                                PlayerThreeChosen = false;
                                PlayerFourChosen = false;

                                // Choose the matchtype
                                switch (GameElementSelected)
                                {
                                    case "Arcade":
                                        {
                                            gameType = Level.MatchType.Arcade;
                                            break;
                                        }
                                    case "Survival":
                                        {
                                            gameType = Level.MatchType.Survival;
                                            break;
                                        }
                                    case "Zones":
                                        {
                                            gameType = Level.MatchType.Zones;
                                            break;
                                        }
                                    case "Waypoint":
                                        {
                                            gameType = Level.MatchType.Travel;
                                            break;
                                        }
                                    case "ThinkFast":
                                        {
                                            gameType = Level.MatchType.ThinkFast;
                                            break;
                                        }
                                    case "Versus":
                                        {
                                            gameType = Level.MatchType.Versus;
                                            break;
                                        }
                                }

                                MenuClickSound.Play(Game1.Global.SoundEffectVolume, 0, 0);
                            }
                        }

                        if (Math.Abs(GameSelectPosition.X) < SlideSpeed)
                        {
                            MoveMenus(-GameSelectPosition.X);
                        }

                        // Menu Selection
                        if (!Guide.IsTrialMode)
                        {
                            if (LeftButtonHit && MenuTimer > ChangeElementTime)
                            {
                                switch (GameElementSelected)
                                {
                                    case "Arcade":
                                        {
                                            GameElementSelected = "Zones";
                                            break;
                                        }
                                    case "Survival":
                                        {
                                            GameElementSelected = "Arcade";
                                            break;
                                        }
                                    case "Zones":
                                        {
                                            GameElementSelected = "Survival";
                                            break;
                                        }
                                    case "Waypoint":
                                        {
                                            GameElementSelected = "Versus";
                                            break;
                                        }
                                    case "ThinkFast":
                                        {
                                            GameElementSelected = "Waypoint";
                                            break;
                                        }
                                    case "Versus":
                                        {
                                            GameElementSelected = "ThinkFast";
                                            break;
                                        }
                                }

                                MenuTimer = 0;

                                MenuScrollSound.Play(Game1.Global.SoundEffectVolume, 0, 0);
                            }
                            else if (RightButtonHit && MenuTimer > ChangeElementTime)
                            {
                                switch (GameElementSelected)
                                {
                                    case "Arcade":
                                        {
                                            GameElementSelected = "Survival";
                                            break;
                                        }
                                    case "Survival":
                                        {
                                            GameElementSelected = "Zones";
                                            break;
                                        }
                                    case "Zones":
                                        {
                                            GameElementSelected = "Arcade";
                                            break;
                                        }
                                    case "Waypoint":
                                        {
                                            GameElementSelected = "ThinkFast";
                                            break;
                                        }
                                    case "ThinkFast":
                                        {
                                            GameElementSelected = "Versus";
                                            break;
                                        }
                                    case "Versus":
                                        {
                                            GameElementSelected = "Waypoint";
                                            break;
                                        }
                                }

                                MenuTimer = 0;

                                MenuScrollSound.Play(Game1.Global.SoundEffectVolume, 0, 0);
                            }
                            else if (UpButtonHit && MenuTimer > ChangeElementTime)
                            {
                                switch (GameElementSelected)
                                {
                                    case "Arcade":
                                        {
                                            GameElementSelected = "Waypoint";
                                            break;
                                        }
                                    case "Survival":
                                        {
                                            GameElementSelected = "ThinkFast";
                                            break;
                                        }
                                    case "Zones":
                                        {
                                            GameElementSelected = "Versus";
                                            break;
                                        }
                                    case "Waypoint":
                                        {
                                            GameElementSelected = "Arcade";
                                            break;
                                        }
                                    case "ThinkFast":
                                        {
                                            GameElementSelected = "Survival";
                                            break;
                                        }
                                    case "Versus":
                                        {
                                            GameElementSelected = "Zones";
                                            break;
                                        }
                                }

                                MenuTimer = 0;

                                MenuScrollSound.Play(Game1.Global.SoundEffectVolume, 0, 0);
                            }
                            else if (DownButtonHit && MenuTimer > ChangeElementTime)
                            {
                                switch (GameElementSelected)
                                {
                                    case "Arcade":
                                        {
                                            GameElementSelected = "Waypoint";
                                            break;
                                        }
                                    case "Survival":
                                        {
                                            GameElementSelected = "ThinkFast";
                                            break;
                                        }
                                    case "Zones":
                                        {
                                            GameElementSelected = "Versus";
                                            break;
                                        }
                                    case "Waypoint":
                                        {
                                            GameElementSelected = "Arcade";
                                            break;
                                        }
                                    case "ThinkFast":
                                        {
                                            GameElementSelected = "Survival";
                                            break;
                                        }
                                    case "Versus":
                                        {
                                            GameElementSelected = "Zones";
                                            break;
                                        }
                                }

                                MenuTimer = 0;

                                MenuScrollSound.Play(Game1.Global.SoundEffectVolume, 0, 0);
                            }
                        }
                        else
                        {
                            if (DownButtonHit && MenuTimer > ChangeElementTime)
                            {
                                switch (GameElementSelected)
                                {
                                    case "Arcade":
                                        {
                                            GameElementSelected = "Waypoint";
                                            break;
                                        }
                                    case "Waypoint":
                                        {
                                            GameElementSelected = "Arcade";
                                            break;
                                        }
                                }

                                MenuTimer = 0;

                                MenuScrollSound.Play(Game1.Global.SoundEffectVolume, 0, 0);
                            }
                            else if (UpButtonHit && MenuTimer > ChangeElementTime)
                            {
                                switch (GameElementSelected)
                                {
                                    case "Arcade":
                                        {
                                            GameElementSelected = "Waypoint";
                                            break;
                                        }
                                    case "Waypoint":
                                        {
                                            GameElementSelected = "Arcade";
                                            break;
                                        }
                                }

                                MenuTimer = 0;

                                MenuScrollSound.Play(Game1.Global.SoundEffectVolume, 0, 0);
                            }
                        }

                        break;
                    }
                case GameState.PlayerSelect:
                    {
                        PlayerSelect(gameTime);
                        // level = null;

                        if (!MenuColor.Equals(Color.Red))
                        {
                            MenuColor = Color.Lerp(MenuColor, Color.Red, ColorTransitionSpeed);
                        }

                        if (PlayerSelectPosition.X < 0)
                        {
                            MoveMenus(SlideSpeed);
                        }
                        else if (PlayerSelectPosition.X > 0)
                        {
                            MoveMenus(-SlideSpeed);
                        }
                        else
                        {
                            if (BButtonHit && BButtonReady)
                            {
                                gameState = GameState.GameSelect;
                                BButtonReady = false;
                            }
                        }

                        if (Math.Abs(PlayerSelectPosition.X) < SlideSpeed)
                        {
                            MoveMenus(-PlayerSelectPosition.X);
                        }
                        break;
                    }

                case GameState.Loading:
                    {
                        // Make a new level
                        if (level == null)
                        {
                            level = new Level(Content);
                        }

                        if (GameStartFade < 1.0f)
                        {
                            GameStartFade += 0.05f;
                        }
                        else
                        {
                            gameState = GameState.Level;
                            GameStartFade = 0;
                        }
                        break;
                    }
                case GameState.StartLevel:
                    {
                        // Holds the player indexes ingame
                        List<PlayerIndex> pi = new List<PlayerIndex>();

                        // If we haven't started a game, start a single player one
                        if (PlayerOneChosen)
                        {
                            pi.Add(PlayerIndex.One);
                        }
                        if (PlayerTwoChosen)
                        {
                            pi.Add(PlayerIndex.Two);
                        }
                        if (PlayerThreeChosen)
                        {
                            pi.Add(PlayerIndex.Three);
                        }
                        if (PlayerFourChosen)
                        {
                            pi.Add(PlayerIndex.Four);
                        }

                        if (gameType.Equals(Level.MatchType.Versus))
                        {
                            if (pi.Count >= 2)
                            {
                                level.StartMatch(Level.MatchType.Versus, pi);
                            }
                            else
                            {
                                gameState = GameState.PlayerSelect;
                            }
                        }
                        else if (gameType.Equals(Level.MatchType.Arcade))
                        {
                            level.StartMatch(Level.MatchType.Arcade, pi);
                        }
                        else if (gameType.Equals(Level.MatchType.Zones))
                        {
                            level.StartMatch(Level.MatchType.Zones, pi);
                        }
                        else if (gameType.Equals(Level.MatchType.Travel))
                        {
                            level.StartMatch(Level.MatchType.Travel, pi);
                        }
                        else if (gameType.Equals(Level.MatchType.Survival))
                        {
                            level.StartMatch(Level.MatchType.Survival, pi);
                        }
                        else if (gameType.Equals(Level.MatchType.ThinkFast))
                        {
                            level.StartMatch(Level.MatchType.ThinkFast, pi);
                        }

                        gameState = GameState.Loading;
                        break;
                    }
                case GameState.GameModeTutorial:
                    {
                        if (!MenuColor.Equals(Color.Red))
                        {
                            MenuColor = Color.Lerp(MenuColor, Color.Red, ColorTransitionSpeed);
                        }

                        if (GameModeTutorialPosition.X < 0)
                        {
                            MoveMenus(SlideSpeed);
                        }
                        else if (GameModeTutorialPosition.X > 0)
                        {
                            MoveMenus(-SlideSpeed);
                        }
                        else
                        {
                            if (AButtonHit && AButtonReady)
                            {
                                gameState = GameState.StartLevel;
                                AButtonReady = false;
                            }

                            if (BButtonHit && BButtonReady)
                            {
                                gameState = GameState.PlayerSelect;
                                BButtonReady = false;
                            }

                            if (RightButtonHit && MenuTimer > ChangeElementTime)
                            {
                                gameState = GameState.PlayerTutorial;
                                MenuTimer = 0;
                            }
                        }

                        if (Math.Abs(GameModeTutorialPosition.X) < SlideSpeed)
                        {
                            MoveMenus(-GameModeTutorialPosition.X);
                        }
                        break;
                    }
                case GameState.PlayerTutorial:
                    {
                        if (!MenuColor.Equals(Color.Red))
                        {
                            MenuColor = Color.Lerp(MenuColor, Color.Red, ColorTransitionSpeed);
                        }

                        if (PlayerTutorialPosition.X < 0)
                        {
                            MoveMenus(SlideSpeed);
                        }
                        else if (PlayerTutorialPosition.X > 0)
                        {
                            MoveMenus(-SlideSpeed);
                        }
                        else
                        {
                            if (AButtonHit && AButtonReady)
                            {
                                gameState = GameState.StartLevel;
                                AButtonReady = false;
                            }

                            if (BButtonHit && BButtonReady)
                            {
                                gameState = GameState.PlayerSelect;
                                BButtonReady = false;
                            }

                            if (RightButtonHit && MenuTimer > ChangeElementTime)
                            {
                                gameState = GameState.EnemyTutorial;
                                MenuTimer = 0;
                            }
                            else if (LeftButtonHit && MenuTimer > ChangeElementTime)
                            {
                                gameState = GameState.GameModeTutorial;
                                MenuTimer = 0;
                            }
                        }

                        if (Math.Abs(PlayerTutorialPosition.X) < SlideSpeed)
                        {
                            MoveMenus(-PlayerTutorialPosition.X);
                        }
                        break;
                    }
                case GameState.EnemyTutorial:
                    {
                        if (!MenuColor.Equals(Color.Red))
                        {
                            MenuColor = Color.Lerp(MenuColor, Color.Red, ColorTransitionSpeed);
                        }

                        if (EnemyTutorialPosition.X < 0)
                        {
                            MoveMenus(SlideSpeed);
                        }
                        else if (EnemyTutorialPosition.X > 0)
                        {
                            MoveMenus(-SlideSpeed);
                        }
                        else
                        {
                            if (AButtonHit && AButtonReady)
                            {
                                gameState = GameState.StartLevel;
                                YButtonReady = false;
                            }

                            if (BButtonHit && BButtonReady)
                            {
                                gameState = GameState.PlayerSelect;
                                BButtonReady = false;
                            }

                            if (RightButtonHit && MenuTimer > ChangeElementTime)
                            {
                                gameState = GameState.ControlsTutorial;
                                MenuTimer = 0;
                            }
                            else if (LeftButtonHit && MenuTimer > ChangeElementTime)
                            {
                                gameState = GameState.PlayerTutorial;
                                MenuTimer = 0;
                            }
                        }

                        if (Math.Abs(EnemyTutorialPosition.X) < SlideSpeed)
                        {
                            MoveMenus(-EnemyTutorialPosition.X);
                        }
                        break;
                    }
                case GameState.ControlsTutorial:
                    {
                        if (!MenuColor.Equals(Color.Red))
                        {
                            MenuColor = Color.Lerp(MenuColor, Color.Red, ColorTransitionSpeed);
                        }

                        if (BButtonHit && BButtonReady)
                        {
                            gameState = GameState.PlayerSelect;
                            BButtonReady = false;
                        }

                        if (ControlsTutorialPosition.X < 0)
                        {
                            MoveMenus(SlideSpeed);
                        }
                        else if (ControlsTutorialPosition.X > 0)
                        {
                            MoveMenus(-SlideSpeed);
                        }
                        else
                        {
                            if (AButtonHit && AButtonReady)
                            {
                                gameState = GameState.StartLevel;
                                AButtonReady = false;
                            }

                            if (LeftButtonHit && MenuTimer > ChangeElementTime)
                            {
                                gameState = GameState.EnemyTutorial;
                                MenuTimer = 0;
                            }
                        }

                        if (Math.Abs(ControlsTutorialPosition.X) < SlideSpeed)
                        {
                            MoveMenus(-ControlsTutorialPosition.X);
                        }
                        break;
                    }
                case GameState.Level:
                    {
                        if (!Guide.IsVisible)
                        {
                            level.Update(gameTime);

                            if (level.ReturnToMenu && level.BackgroundFade > 0.9f)
                            {
                                gameState = GameState.MainMenu;
                                MediaPlayer.Play(MenuSong);
                            }

                            if (level.GameOver && (level.Particles.Count <= 0 || level.ScoreSubmitted))
                            {
                                // Are we trying to buy the game?
                                if (gps1.IsButtonDown(Buttons.X))
                                {
                                    if (CanPlayerBuyGame(PlayerIndex.One))
                                    {
                                        Guide.ShowMarketplace(PlayerIndex.One);
                                    }
                                    else
                                    {
                                        SimpleMessageBox.ShowMessageBox("Purchase Pew Pew Pod",
                                            "You must be logged into a profile that has purchasing priviledges in order to buy Pew Pew Pod. Please try again with a different profile.",
                                            new string[] { "Ok" }, 0, MessageBoxIcon.Warning);
                                    }
                                }
                                else if (gps2.IsButtonDown(Buttons.X))
                                {
                                    if (CanPlayerBuyGame(PlayerIndex.Two))
                                    {
                                        Guide.ShowMarketplace(PlayerIndex.Two);
                                    }
                                    else
                                    {
                                        SimpleMessageBox.ShowMessageBox("Purchase Pew Pew Pod",
                                            "You must be logged into a profile that has purchasing priviledges in order to buy Pew Pew Pod. Please try again with a different profile.",
                                            new string[] { "Ok" }, 0, MessageBoxIcon.Warning);
                                    }
                                }
                                else if (gps3.IsButtonDown(Buttons.X))
                                {
                                    if (CanPlayerBuyGame(PlayerIndex.Three))
                                    {
                                        Guide.ShowMarketplace(PlayerIndex.Three);
                                    }
                                    else
                                    {
                                        SimpleMessageBox.ShowMessageBox("Purchase Pew Pew Pod",
                                            "You must be logged into a profile that has purchasing priviledges in order to buy Pew Pew Pod. Please try again with a different profile.",
                                            new string[] { "Ok" }, 0, MessageBoxIcon.Warning);
                                    }
                                }
                                else if (gps4.IsButtonDown(Buttons.X))
                                {
                                    if (CanPlayerBuyGame(PlayerIndex.Four))
                                    {
                                        Guide.ShowMarketplace(PlayerIndex.Four);
                                    }
                                    else
                                    {
                                        SimpleMessageBox.ShowMessageBox("Purchase Pew Pew Pod",
                                            "You must be logged into a profile that has purchasing priviledges in order to buy Pew Pew Pod. Please try again with a different profile.",
                                            new string[] { "Ok" }, 0, MessageBoxIcon.Warning);
                                    }
                                }
                            }
                        }
                        break;
                    }
                case GameState.Paused:
                    {
                        UpdatePause(gameTime);
                        break;
                    }
            }

            // If we are in a menu, update the planets
            if (gameState.Equals(GameState.MainMenu)
                || gameState.Equals(GameState.PlayerSelect)
                || gameState.Equals(GameState.GameSelect)
                || gameState.Equals(GameState.ControlsTutorial)
                || gameState.Equals(GameState.EnemyTutorial)
                || gameState.Equals(GameState.Extras)
                || gameState.Equals(GameState.GameModeTutorial)
                || gameState.Equals(GameState.Highscores)
                || gameState.Equals(GameState.Options)
                || gameState.Equals(GameState.PlayerTutorial)
                || gameState.Equals(GameState.StartLevel)
                || gameState.Equals(GameState.Loading))
            {
                foreach (Particle p in Planets)
                {
                    p.Update(gameTime);
                }

                foreach (BackgroundStar s in Layer1)
                {
                    s.Update();
                }
            }

            // If the guide is open, pause the music
            if ((Guide.IsVisible || gameState.Equals(GameState.Paused)) && MediaPlayer.State.Equals(MediaState.Playing))
            {
                MediaPlayer.Pause();
            }
            else if ((!Guide.IsVisible && !gameState.Equals(GameState.Paused)) && MediaPlayer.State.Equals(MediaState.Paused))
            {
                MediaPlayer.Resume();
            }

            if (level != null && gameState.Equals(GameState.MainMenu) && HighscoreComponent.Global.Enabled == false)
            {
                HighscoreComponent.Global.Enabled = true;
                System.Diagnostics.Trace.WriteLine("Switched highscores to on, because they were off.");
            }

            // Do not rumble controllers if we aren't in a level, or we turned rumble off
            if (!gameState.Equals(GameState.Level))
            {
                GamePad.SetVibration(PlayerIndex.One, 0, 0);
                GamePad.SetVibration(PlayerIndex.Two, 0, 0);
                GamePad.SetVibration(PlayerIndex.Three, 0, 0);
                GamePad.SetVibration(PlayerIndex.Four, 0, 0);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // Drawing code
            spriteBatch.Begin();

            switch (gameState)
            {
                case GameState.BadEggStudios:
                    {
                        spriteBatch.Draw(BadEggStudios, Vector2.Zero, Color.White);
                        break;
                    }
                case GameState.MainMenu:
                    {
                        DrawMenus(spriteBatch);
                        break;
                    }
                case GameState.GameSelect:
                    {
                        DrawMenus(spriteBatch);
                        break;
                    }
                case GameState.PlayerTutorial:
                    {
                        DrawMenus(spriteBatch);
                        break;
                    }
                case GameState.GameModeTutorial:
                    {
                        DrawMenus(spriteBatch);
                        break;
                    }
                case GameState.EnemyTutorial:
                    {
                        DrawMenus(spriteBatch);
                        break;
                    }
                case GameState.Highscores:
                    {
                        DrawMenus(spriteBatch);
                        break;
                    }
                case GameState.Options:
                    {
                        DrawMenus(spriteBatch);
                        break;
                    }
                case GameState.ControlsTutorial:
                    {
                        DrawMenus(spriteBatch);
                        break;
                    }
                case GameState.Extras:
                    {
                        DrawMenus(spriteBatch);
                        break;
                    }
                case GameState.Level:
                    {
                        level.Draw(spriteBatch, gameTime);
                        break;
                    }
                case GameState.Loading:
                    {
                        // spriteBatch.DrawString(gameFontLarge, "Press A to Play", new Vector2(200, 340), Color.White);
                        DrawMenus(spriteBatch);
                        break;
                    }
                case GameState.Paused:
                    {
                        level.Draw(spriteBatch, gameTime);

                        spriteBatch.Draw(PauseTexture, Vector2.Zero, Color.White);

                        Vector2 arrowOrigin = new Vector2(ArrowTexture.Width / 2, ArrowTexture.Height / 2);

                        spriteBatch.Draw(BlueBox, new Vector2(515, 215), Color.White);
                        spriteBatch.Draw(BlueBox, new Vector2(515, 312), Color.White);
                        spriteBatch.Draw(BlueBox, new Vector2(515, 410), Color.White);
                        spriteBatch.DrawString(gameFontLarge, "Resume", new Vector2(595, 245), Color.White);
                        spriteBatch.DrawString(gameFontLarge, "Restart", new Vector2(592, 342), Color.White);
                        spriteBatch.DrawString(gameFontLarge, "Back To Menu", new Vector2(558, 440), Color.White);

                        switch (PauseElementSelected)
                        {
                            case "Resume":
                                {
                                    spriteBatch.Draw(ArrowTexture, new Vector2(490, 267), null, Color.Turquoise, MathHelper.Pi, arrowOrigin, 0.7f, SpriteEffects.None, 0);
                                    spriteBatch.Draw(ArrowTexture, new Vector2(794, 267), null, Color.Turquoise, 0, arrowOrigin, 0.7f, SpriteEffects.None, 0);
                                    break;
                                }
                            case "MainMenu":
                                {
                                    spriteBatch.Draw(ArrowTexture, new Vector2(490, 455), null, Color.Turquoise, MathHelper.Pi, arrowOrigin, 0.7f, SpriteEffects.None, 0);
                                    spriteBatch.Draw(ArrowTexture, new Vector2(794, 455), null, Color.Turquoise, 0, arrowOrigin, 0.7f, SpriteEffects.None, 0);
                                    break;
                                }
                            case "Restart":
                                {
                                    spriteBatch.Draw(ArrowTexture, new Vector2(490, 365), null, Color.Turquoise, MathHelper.Pi, arrowOrigin, 0.7f, SpriteEffects.None, 0);
                                    spriteBatch.Draw(ArrowTexture, new Vector2(794, 365), null, Color.Turquoise, 0, arrowOrigin, 0.7f, SpriteEffects.None, 0);
                                    break;
                                }
                        }

                        // Draw name of player
                        string name = HighscoreComponent.NameFromPlayer(PausedController);
                        if (name.Equals("Guest"))
                        {
                            name = "Player " + PausedController.ToString();
                        }
                        spriteBatch.DrawString(gameFontLarge, name, new Vector2(640, 175), Color.White, 0, gameFontLarge.MeasureString(name) / 2, 1.0f, SpriteEffects.None, 0);
                        break;
                    }
                case GameState.StartLevel:
                    {
                        DrawMenus(spriteBatch);
                        break;
                    }
                case GameState.PlayerSelect:
                    {
                        DrawMenus(spriteBatch);
                        break;
                    }
                default:
                    {
                        DrawMenus(spriteBatch);
                        break;
                    }
            }

            // Game start fade
            if (GameStartFade > 0)
            {
                spriteBatch.Draw(Content.Load<Texture2D>("Graphics/Extras/BlankDot"), Vector2.Zero, new Rectangle(0, 0, 1280, 720), new Color(Color.Black, GameStartFade));
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Draws all the menus onscreen
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawMenus(SpriteBatch spriteBatch)
        {
            // Background
            foreach (BackgroundStar s in Layer1)
            {
                s.Draw(spriteBatch);
            }
            foreach (Particle p in Planets)
            {
                p.Draw(spriteBatch);
            }
            MenuBackground.Draw(spriteBatch, new Color(MenuColor, 0.75f));

            #region Main Menu
            MenuLines.Draw(spriteBatch, MenuColor, MainMenuPosition);

            MenuBox.Draw(spriteBatch, MenuColor, new Vector2(MainMenuPosition.X + 260, MainMenuPosition.Y + 130));
            MenuBox.Draw(spriteBatch, MenuColor, new Vector2(MainMenuPosition.X + 260, MainMenuPosition.Y + 225));
            MenuBox.Draw(spriteBatch, MenuColor, new Vector2(MainMenuPosition.X + 260, MainMenuPosition.Y + 320));
            MenuBox.Draw(spriteBatch, MenuColor, new Vector2(MainMenuPosition.X + 260, MainMenuPosition.Y + 415));
            MenuBox.Draw(spriteBatch, MenuColor, new Vector2(MainMenuPosition.X + 260, MainMenuPosition.Y + 510));

            // Draw Text
            spriteBatch.DrawString(gameFontLarge, "Play", new Vector2(MainMenuPosition.X + 355, MainMenuPosition.Y + 160), Color.White);
            spriteBatch.DrawString(gameFontLarge, "Highscores", new Vector2(MainMenuPosition.X + 320, MainMenuPosition.Y + 255), Color.White);
            spriteBatch.DrawString(gameFontLarge, "Options", new Vector2(MainMenuPosition.X + 340, MainMenuPosition.Y + 350), Color.White);
            spriteBatch.DrawString(gameFontLarge, "Extras", new Vector2(MainMenuPosition.X + 340, MainMenuPosition.Y + 445), Color.White);
            spriteBatch.DrawString(gameFontLarge, "Exit", new Vector2(MainMenuPosition.X + 360, MainMenuPosition.Y + 540), Color.White);

            // Draw Arrow
            switch (MenuElementSelected)
            {
                case "Play":
                    {
                        ArrowDrawPosition = new Vector2(MainMenuPosition.X + 500, MainMenuPosition.Y + 140);
                        break;
                    }
                case "Highscores":
                    {
                        ArrowDrawPosition = new Vector2(MainMenuPosition.X + 500, MainMenuPosition.Y + 235);
                        break;
                    }
                case "Options":
                    {
                        ArrowDrawPosition = new Vector2(MainMenuPosition.X + 500, MainMenuPosition.Y + 330);
                        break;
                    }
                case "Extras":
                    {
                        ArrowDrawPosition = new Vector2(MainMenuPosition.X + 500, MainMenuPosition.Y + 425);
                        break;
                    }
                case "Exit":
                    {
                        ArrowDrawPosition = new Vector2(MainMenuPosition.X + 500, MainMenuPosition.Y + 520);
                        break;
                    }
            }
            spriteBatch.Draw(ArrowTexture, ArrowDrawPosition, null, MenuColor, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);

            TitleLogoBottom.Draw(spriteBatch, MenuColor, new Vector2(MainMenuPosition.X + 375, MainMenuPosition.Y - 80));
            TitleLogo.Draw(spriteBatch, Color.White, new Vector2(MainMenuPosition.X + 375, MainMenuPosition.Y - 80));

            if (IsTrial)
            {
                XButton.Draw(spriteBatch, Color.White, new Vector2(MainMenuPosition.X + 720, MainMenuPosition.Y + 500));
                spriteBatch.DrawString(gameFontLarge, "Buy Now", new Vector2(MainMenuPosition.X + 780, MainMenuPosition.Y + 510), Color.White);
            }

            if (!level.hsc_.userWantsToLoad_ && level.hsc_.storage_ == null)
            {
                spriteBatch.DrawString(gameFontLarge, "Press BACK to choose storage device", new Vector2(MainMenuPosition.X + 555, MainMenuPosition.Y + 560), Color.White);
            }
            #endregion

            #region Game Select Screen
            MenuLines.Draw(spriteBatch, MenuColor, GameSelectPosition);

            // Draw Instructions
            BButton.Draw(spriteBatch, Color.White, new Vector2(GameSelectPosition.X + 360, GameSelectPosition.Y + 115));
            spriteBatch.DrawString(gameFontLarge, "Go Back", new Vector2(GameSelectPosition.X + 260, GameSelectPosition.Y + 125), Color.White);

            AButton.Draw(spriteBatch, Color.White, new Vector2(GameSelectPosition.X + 845, GameSelectPosition.Y + 115));
            spriteBatch.DrawString(gameFontLarge, "Continue", new Vector2(GameSelectPosition.X + 910, GameSelectPosition.Y + 125), Color.White);

            Vector2 GameTypePosition = new Vector2(GameSelectPosition.X, GameSelectPosition.Y + 162);
            // Draw Strings
            spriteBatch.DrawString(gameFontExtraLarge, "Arcade", new Vector2(GameTypePosition.X + 325, GameTypePosition.Y + 25), Color.White);
            spriteBatch.DrawString(gameFontExtraLarge, "Waypoint", new Vector2(GameTypePosition.X + 305, GameTypePosition.Y + 235), Color.White);
            spriteBatch.DrawString(gameFontExtraLarge, "Survival", new Vector2(GameTypePosition.X + 575, GameTypePosition.Y + 25), Color.White);
            spriteBatch.DrawString(gameFontExtraLarge, "Think Fast", new Vector2(GameTypePosition.X + 555, GameTypePosition.Y + 235), Color.White);
            spriteBatch.DrawString(gameFontExtraLarge, "Zones", new Vector2(GameTypePosition.X + 860, GameTypePosition.Y + 25), Color.White);
            spriteBatch.DrawString(gameFontExtraLarge, "Versus", new Vector2(GameTypePosition.X + 850, GameTypePosition.Y + 235), Color.White);

            ArcadeBox.Draw(spriteBatch, Color.White, new Vector2(GameTypePosition.X + 235, GameTypePosition.Y));
            WaypointBox.Draw(spriteBatch, Color.White, new Vector2(GameTypePosition.X + 235, GameTypePosition.Y + 210));

            if (!Guide.IsTrialMode)
            {
                SurvivalBox.Draw(spriteBatch, Color.White, new Vector2(GameTypePosition.X + 500, GameTypePosition.Y));
                ThinkFastBox.Draw(spriteBatch, Color.White, new Vector2(GameTypePosition.X + 500, GameTypePosition.Y + 210));
                ZonesBox.Draw(spriteBatch, Color.White, new Vector2(GameTypePosition.X + 765, GameTypePosition.Y));
                VersusBox.Draw(spriteBatch, Color.White, new Vector2(GameTypePosition.X + 765, GameTypePosition.Y + 210));
            }
            else
            {
                BlockedSurvivalBox.Draw(spriteBatch, Color.White, new Vector2(GameTypePosition.X + 500, GameTypePosition.Y));
                BlockedThinkFastBox.Draw(spriteBatch, Color.White, new Vector2(GameTypePosition.X + 500, GameTypePosition.Y + 210));
                BlockedZonesBox.Draw(spriteBatch, Color.White, new Vector2(GameTypePosition.X + 765, GameTypePosition.Y));
                BlockedVersusBox.Draw(spriteBatch, Color.White, new Vector2(GameTypePosition.X + 765, GameTypePosition.Y + 210));
            }

            // Position of the selected element
            switch (GameElementSelected)
            {
                case "Arcade":
                    {
                        SelectedPosition = new Vector2(GameSelectPosition.X + 224, GameSelectPosition.Y + 147);
                        break;
                    }
                case "Waypoint":
                    {
                        SelectedPosition = new Vector2(GameSelectPosition.X + 224, GameSelectPosition.Y + 357);
                        break;
                    }
                case "Survival":
                    {
                        SelectedPosition = new Vector2(GameSelectPosition.X + 490, GameSelectPosition.Y + 147);
                        break;
                    }
                case "ThinkFast":
                    {
                        SelectedPosition = new Vector2(GameSelectPosition.X + 490, GameSelectPosition.Y + 357);
                        break;
                    }
                case "Zones":
                    {
                        SelectedPosition = new Vector2(GameSelectPosition.X + 755, GameSelectPosition.Y + 147);
                        break;
                    }
                case "Versus":
                    {
                        SelectedPosition = new Vector2(GameSelectPosition.X + 755, GameSelectPosition.Y + 357);
                        break;
                    }
            }

            GameModeBoxSelect.Draw(spriteBatch, Color.White, SelectedPosition);
            #endregion

            #region Player Select Screen
            MenuPlayerSelect.Draw(spriteBatch, MenuColor, PlayerSelectPosition);

            BButton.Draw(spriteBatch, Color.White, new Vector2(PlayerSelectPosition.X + 360, PlayerSelectPosition.Y + 115));
            spriteBatch.DrawString(gameFontLarge, "Go Back", new Vector2(PlayerSelectPosition.X + 260, PlayerSelectPosition.Y + 125), Color.White);

            YButton.Draw(spriteBatch, Color.White, new Vector2(PlayerSelectPosition.X + 520, PlayerSelectPosition.Y + 115));
            spriteBatch.DrawString(gameFontLarge, "Toggle Ready", new Vector2(PlayerSelectPosition.X + 580, PlayerSelectPosition.Y + 125), Color.White);

            AButton.Draw(spriteBatch, Color.White, new Vector2(PlayerSelectPosition.X + 845, PlayerSelectPosition.Y + 115));
            spriteBatch.DrawString(gameFontLarge, "Continue", new Vector2(PlayerSelectPosition.X + 910, PlayerSelectPosition.Y + 125), Color.White);

            if (PlayerOneChosen)
            {
                spriteBatch.DrawString(gameFontLarge, "Ready!", new Vector2(PlayerSelectPosition.X + 300, PlayerSelectPosition.Y + 265), Color.White);
            }
            else
            {
                spriteBatch.DrawString(gameFontLarge, "Press\n   Y", new Vector2(PlayerSelectPosition.X + 305, PlayerSelectPosition.Y + 250), Color.White);
            }

            if (PlayerTwoChosen)
            {
                spriteBatch.DrawString(gameFontLarge, "Ready!", new Vector2(PlayerSelectPosition.X + 500, PlayerSelectPosition.Y + 265), Color.White);
            }
            else
            {
                spriteBatch.DrawString(gameFontLarge, "Press\n   Y", new Vector2(PlayerSelectPosition.X + 505, PlayerSelectPosition.Y + 250), Color.White);
            }

            if (!IsTrial)
            {
                if (PlayerThreeChosen)
                {
                    spriteBatch.DrawString(gameFontLarge, "Ready!", new Vector2(PlayerSelectPosition.X + 700, PlayerSelectPosition.Y + 265), Color.White);
                }
                else
                {
                    spriteBatch.DrawString(gameFontLarge, "Press\n   Y", new Vector2(PlayerSelectPosition.X + 703, PlayerSelectPosition.Y + 250), Color.White);
                }
            }
            else
            {
                spriteBatch.DrawString(gameFontLarge, "Please \nBuy", new Vector2(PlayerSelectPosition.X + 703, PlayerSelectPosition.Y + 250), Color.White);
            }

            if (!IsTrial)
            {
                if (PlayerFourChosen)
                {
                    spriteBatch.DrawString(gameFontLarge, "Ready!", new Vector2(PlayerSelectPosition.X + 895, PlayerSelectPosition.Y + 265), Color.White);
                }
                else
                {
                    spriteBatch.DrawString(gameFontLarge, "Press\n   Y", new Vector2(PlayerSelectPosition.X + 900, PlayerSelectPosition.Y + 250), Color.White);
                }
            }
            else
            {
                spriteBatch.DrawString(gameFontLarge, "Please\n Buy", new Vector2(PlayerSelectPosition.X + 900, PlayerSelectPosition.Y + 250), Color.White);
            }

            spriteBatch.DrawString(gameFontLarge, "Player One", new Vector2(PlayerSelectPosition.X + 280, PlayerSelectPosition.Y + 210), Color.White);
            spriteBatch.DrawString(gameFontLarge, "Player Two", new Vector2(PlayerSelectPosition.X + 470, PlayerSelectPosition.Y + 210), Color.White);
            spriteBatch.DrawString(gameFontLarge, "Player Three", new Vector2(PlayerSelectPosition.X + 660, PlayerSelectPosition.Y + 210), Color.White);
            spriteBatch.DrawString(gameFontLarge, "Player Four", new Vector2(PlayerSelectPosition.X + 860, PlayerSelectPosition.Y + 210), Color.White);
            
            if ((gameType.Equals(Level.MatchType.Versus) &&
                   !((PlayerOneChosen && PlayerTwoChosen) || (PlayerOneChosen && PlayerThreeChosen) || (PlayerOneChosen && PlayerFourChosen) || (PlayerTwoChosen && PlayerThreeChosen)
                       || (PlayerTwoChosen && PlayerFourChosen) || (PlayerThreeChosen && PlayerFourChosen))))
            {
                spriteBatch.DrawString(gameFontLarge, "Minimum of 2 Players", new Vector2(PlayerSelectPosition.X + 640, PlayerSelectPosition.Y + 195), Color.Yellow, 0, gameFontLarge.MeasureString("Minimum of 2 Players") / 2, 1.0f, SpriteEffects.None, 0);
            }

            if (((PlayerOneChosen && PlayerTwoChosen) || (PlayerOneChosen && PlayerThreeChosen) || (PlayerOneChosen && PlayerFourChosen) || (PlayerTwoChosen && PlayerThreeChosen)
                       || (PlayerTwoChosen && PlayerFourChosen) || (PlayerThreeChosen && PlayerFourChosen)) && !gameType.Equals(Level.MatchType.Versus))
            {
                spriteBatch.DrawString(gameFontLarge, "Highscores Are Only Saved In Single Player", new Vector2(PlayerSelectPosition.X + 640, PlayerSelectPosition.Y + 195), Color.Yellow, 0, gameFontLarge.MeasureString("Highscores Are Only Saved In Single Player") / 2, 1.0f, SpriteEffects.None, 0);
            }

            ControlsScreen.Draw(spriteBatch, Color.White, new Vector2(PlayerSelectPosition.X + 640, PlayerSelectPosition.Y + 450), 0.575f);
            #endregion

            #region Highscores
            // MenuLines.Draw(spriteBatch, MenuColor, HighscoresPosition);
            HighscoresBox.Draw(spriteBatch, MenuColor, HighscoresPosition);

            AButton.Draw(spriteBatch, Color.White, new Vector2(HighscoresPosition.X + 260,HighscoresPosition.Y + 105));
            spriteBatch.DrawString(gameFontLarge, "Change Game Type", new Vector2(HighscoresPosition.X + 325, HighscoresPosition.Y + 115), Color.White);

            BButton.Draw(spriteBatch, Color.White, new Vector2(HighscoresPosition.X + 945, HighscoresPosition.Y + 105));
            spriteBatch.DrawString(gameFontLarge, "Go Back", new Vector2(HighscoresPosition.X + 850, HighscoresPosition.Y + 115), Color.White);

            // Draw gametype
            #region White Selected
            switch (HighscoresElementSelected)
            {
                case "Arcade":
                    {
                        spriteBatch.DrawString(gameFontLarge, HighscoresElementSelected, new Vector2(HighscoresPosition.X + 600, HighscoresPosition.Y + 160), Color.White);
                        break;
                    }
                case "Survival":
                    {
                        spriteBatch.DrawString(gameFontLarge, HighscoresElementSelected, new Vector2(HighscoresPosition.X + 590, HighscoresPosition.Y + 160), Color.White);
                        break;
                    }
                case "Zones":
                    {
                        spriteBatch.DrawString(gameFontLarge, HighscoresElementSelected, new Vector2(HighscoresPosition.X + 610, HighscoresPosition.Y + 160), Color.White);
                        break;
                    }
                case "Waypoint":
                    {
                        spriteBatch.DrawString(gameFontLarge, HighscoresElementSelected, new Vector2(HighscoresPosition.X + 585, HighscoresPosition.Y + 160), Color.White);
                        break;
                    }
                case "ThinkFast":
                    {
                        spriteBatch.DrawString(gameFontLarge, "Think Fast", new Vector2(HighscoresPosition.X + 575, HighscoresPosition.Y + 160), Color.White);
                        break;
                    }
            }
            #endregion
            

            // Draw Local Highscores
            spriteBatch.DrawString(gameFontLarge, "Local", new Vector2(HighscoresPosition.X + 410, HighscoresPosition.Y + 198), Color.White);
            float localY = 240;
            if (!IsTrial && level.hsc_.storage_ != null)
            {
                foreach (Highscore s in localScores_)
                {
                    if (s != null)
                    {
                        spriteBatch.DrawString(gameFont, s.Gamer, new Vector2(HighscoresPosition.X + 270, HighscoresPosition.Y + localY), Color.White);
                        spriteBatch.DrawString(gameFont, s.Score.ToString(), new Vector2(HighscoresPosition.X + 515, HighscoresPosition.Y + localY), Color.White);
                        localY += 33;
                    }
                }
            }
            else if(level.hsc_.storage_ != null)
            {
                for (int i = 0; i < 10; i++)
                {
                    spriteBatch.DrawString(gameFont, "Buy To Submit", new Vector2(HighscoresPosition.X + 270, HighscoresPosition.Y + localY), Color.White);
                    spriteBatch.DrawString(gameFont, "Your Score", new Vector2(HighscoresPosition.X + 490, HighscoresPosition.Y + localY), Color.White);
                    localY += 33;
                }
            }


            // Global highscores
            spriteBatch.DrawString(gameFontLarge, "Global", new Vector2(HighscoresPosition.X + 800, HighscoresPosition.Y + 198), Color.White);
            localY = 240;
            if (level.hsc_.storage_ != null)
            {
                foreach (Highscore s in data_)
                {
                    if (s != null)
                    {
                        spriteBatch.DrawString(gameFont, s.Gamer, new Vector2(HighscoresPosition.X + 660, HighscoresPosition.Y + localY), Color.White);
                        spriteBatch.DrawString(gameFont, s.Score.ToString(), new Vector2(HighscoresPosition.X + 900, HighscoresPosition.Y + localY), Color.White);
                        localY += 33;
                    }
                }
            }

            if (!level.hsc_.userWantsToLoad_ && level.hsc_.storage_ == null)
            {
                spriteBatch.DrawString(gameFont, "No storage device          is selected.", new Vector2(HighscoresPosition.X + 270, HighscoresPosition.Y + 240), Color.White);
                spriteBatch.DrawString(gameFont, "Press back to              select device.", new Vector2(HighscoresPosition.X + 270, HighscoresPosition.Y + 290), Color.White);
                spriteBatch.DrawString(gameFont, "No storage device            is selected.", new Vector2(HighscoresPosition.X + 660, HighscoresPosition.Y + 240), Color.White);
                spriteBatch.DrawString(gameFont, "Press back to                select device.", new Vector2(HighscoresPosition.X + 660, HighscoresPosition.Y + 290), Color.White);
            }

            #endregion

            #region Options
            MenuLines.Draw(spriteBatch, MenuColor, OptionsPosition);

            BButton.Draw(spriteBatch, Color.White, new Vector2(OptionsPosition.X + 845, OptionsPosition.Y + 115));
            spriteBatch.DrawString(gameFontLarge, "Go Back", new Vector2(OptionsPosition.X + 910, OptionsPosition.Y + 125), Color.White);

            // Music Volume
            spriteBatch.DrawString(gameFontExtraLarge, "Options", new Vector2(OptionsPosition.X + 300, OptionsPosition.Y + 225), Color.Yellow);

            Color mColor = Color.White;
            Color sColor = Color.White;
            Color rColor = Color.White;

            if (OptionsElementSelected.Equals("Music"))
            {
                mColor = Color.Turquoise;

                spriteBatch.Draw(ArrowTexture, new Vector2(OptionsPosition.X + 700, OptionsPosition.Y + 300 + (int)(ArrowTexture.Height / 2)), null, Color.White, MathHelper.Pi, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
                spriteBatch.Draw(ArrowTexture, new Vector2(OptionsPosition.X + 525, OptionsPosition.Y + 300), null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            }
            else if (OptionsElementSelected.Equals("SoundEffects"))
            {
                sColor = Color.Turquoise;

                spriteBatch.Draw(ArrowTexture, new Vector2(OptionsPosition.X + 700, OptionsPosition.Y + 350 + (int)(ArrowTexture.Height / 2)), null, Color.White, MathHelper.Pi, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
                spriteBatch.Draw(ArrowTexture, new Vector2(OptionsPosition.X + 525, OptionsPosition.Y + 350), null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            }
            else if (OptionsElementSelected.Equals("Rumble"))
            {
                rColor = Color.Turquoise;

                spriteBatch.Draw(ArrowTexture, new Vector2(OptionsPosition.X + 700, OptionsPosition.Y + 400 + (int)(ArrowTexture.Height / 2)), null, Color.White, MathHelper.Pi, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
                spriteBatch.Draw(ArrowTexture, new Vector2(OptionsPosition.X + 525, OptionsPosition.Y + 400), null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            }

            string rumble = "On";
            if(!IsRumbleOn)
            {
                rumble = "Off";
            }

            spriteBatch.DrawString(gameFontExtraLarge, "Music Volume:      " + MusicVolumeInt.ToString(), new Vector2(OptionsPosition.X + 300, OptionsPosition.Y + 300), mColor);
            spriteBatch.DrawString(gameFontExtraLarge, "Sound Effects:      " + SoundEffectVolumeInt.ToString(), new Vector2(OptionsPosition.X + 300, OptionsPosition.Y + 350), sColor);
            spriteBatch.DrawString(gameFontExtraLarge, "Rumble:               " + rumble, new Vector2(OptionsPosition.X + 300, OptionsPosition.Y + 400), rColor);
            spriteBatch.DrawString(gameFontLarge, "Press BACK to change storage device. \n(If only a harddrive is connected, it will be selected automatically.)", new Vector2(OptionsPosition.X + 300, OptionsPosition.Y + 450), Color.White);
            #endregion

            #region Extras
            MenuLines.Draw(spriteBatch, MenuColor, ExtrasPosition);

            Extras.Draw(spriteBatch, Color.White, ExtrasPosition);

            BButton.Draw(spriteBatch, Color.White, new Vector2(ExtrasPosition.X + 845, ExtrasPosition.Y + 115));
            spriteBatch.DrawString(gameFontLarge, "Go Back", new Vector2(ExtrasPosition.X + 910, ExtrasPosition.Y + 125), Color.White);
            #endregion

            #region GameModeTutorial
            GameModeTutorial.Draw(spriteBatch, MenuColor, GameModeTutorialPosition);

            AButton.Draw(spriteBatch, Color.White, new Vector2(GameModeTutorialPosition.X + 570, GameModeTutorialPosition.Y + 110));
            spriteBatch.DrawString(gameFontLarge, "Play", new Vector2(GameModeTutorialPosition.X + 630, GameModeTutorialPosition.Y + 120), Color.White);

            switch (gameType)
            {
                case Level.MatchType.Arcade:
                    {
                        ArcadeTutorial.Draw(spriteBatch, Color.White, GameModeTutorialPosition);
                        break;
                    }
                case Level.MatchType.Survival:
                    {
                        SurvivalTutorial.Draw(spriteBatch, Color.White, GameModeTutorialPosition);
                        break;
                    }
                case Level.MatchType.Versus:
                    {
                        VersusTutorial.Draw(spriteBatch, Color.White, GameModeTutorialPosition);
                        break;
                    }
                case Level.MatchType.ThinkFast:
                    {
                        ThinkFastTutorial.Draw(spriteBatch, Color.White, GameModeTutorialPosition);
                        break;
                    }
                case Level.MatchType.Travel:
                    {
                        WaypointTutorial.Draw(spriteBatch, Color.White, GameModeTutorialPosition);
                        break;
                    }
                case Level.MatchType.Zones:
                    {
                        ZonesTutorial.Draw(spriteBatch, Color.White, GameModeTutorialPosition);
                        break;
                    }
            }

            spriteBatch.Draw(ArrowTexture, new Vector2(GameModeTutorialPosition.X + 1085, GameModeTutorialPosition.Y + 360), null, Color.White, MathHelper.Pi, new Vector2(ArrowTexture.Width / 2, ArrowTexture.Height / 2), 1.0f, SpriteEffects.None, 0);
            // spriteBatch.Draw(ArrowTexture, new Vector2(GameModeTutorialPosition.X + 195, GameModeTutorialPosition.Y + 360), null, Color.White, 0, new Vector2(ArrowTexture.Width / 2, ArrowTexture.Height / 2), 1.0f, SpriteEffects.None, 0);
            #endregion

            #region Player Tutorial
            PlayerTutorial.Draw(spriteBatch, MenuColor, PlayerTutorialPosition);
            PlayerScreen.Draw(spriteBatch, Color.White, PlayerTutorialPosition);

            AButton.Draw(spriteBatch, Color.White, new Vector2(PlayerTutorialPosition.X + 570, PlayerTutorialPosition.Y + 110));
            spriteBatch.DrawString(gameFontLarge, "Play", new Vector2(PlayerTutorialPosition.X + 630, PlayerTutorialPosition.Y + 120), Color.White);

            spriteBatch.Draw(ArrowTexture, new Vector2(PlayerTutorialPosition.X + 1085, PlayerTutorialPosition.Y + 360), null, Color.White, MathHelper.Pi, new Vector2(ArrowTexture.Width / 2, ArrowTexture.Height / 2), 1.0f, SpriteEffects.None, 0);
            spriteBatch.Draw(ArrowTexture, new Vector2(PlayerTutorialPosition.X + 195, PlayerTutorialPosition.Y + 360), null, Color.White, 0, new Vector2(ArrowTexture.Width / 2, ArrowTexture.Height / 2), 1.0f, SpriteEffects.None, 0);
            #endregion

            #region Enemy Tutorial
            EnemyTutorial.Draw(spriteBatch, MenuColor, EnemyTutorialPosition);
            EnemyScreen.Draw(spriteBatch, Color.White, EnemyTutorialPosition);

            AButton.Draw(spriteBatch, Color.White, new Vector2(EnemyTutorialPosition.X + 570, EnemyTutorialPosition.Y + 110));
            spriteBatch.DrawString(gameFontLarge, "Play", new Vector2(EnemyTutorialPosition.X + 630, EnemyTutorialPosition.Y + 120), Color.White);

            spriteBatch.Draw(ArrowTexture, new Vector2(EnemyTutorialPosition.X + 1085, EnemyTutorialPosition.Y + 360), null, Color.White, MathHelper.Pi, new Vector2(ArrowTexture.Width / 2, ArrowTexture.Height / 2), 1.0f, SpriteEffects.None, 0);
            spriteBatch.Draw(ArrowTexture, new Vector2(EnemyTutorialPosition.X + 195, EnemyTutorialPosition.Y + 360), null, Color.White, 0, new Vector2(ArrowTexture.Width / 2, ArrowTexture.Height / 2), 1.0f, SpriteEffects.None, 0);
            #endregion

            #region Controls Tutorial
            EnemyTutorial.Draw(spriteBatch, MenuColor, ControlsTutorialPosition);
            ControlsScreen2.Draw(spriteBatch, Color.White, ControlsTutorialPosition);

            AButton.Draw(spriteBatch, Color.White, new Vector2(ControlsTutorialPosition.X + 570, ControlsTutorialPosition.Y + 110));
            spriteBatch.DrawString(gameFontLarge, "Play", new Vector2(ControlsTutorialPosition.X + 630, ControlsTutorialPosition.Y + 120), Color.White);

            // spriteBatch.Draw(ArrowTexture, new Vector2(ControlsTutorialPosition.X + 1085, ControlsTutorialPosition.Y + 360), null, Color.White, MathHelper.Pi, new Vector2(ArrowTexture.Width / 2, ArrowTexture.Height / 2), 1.0f, SpriteEffects.None, 0);
            spriteBatch.Draw(ArrowTexture, new Vector2(ControlsTutorialPosition.X + 195, ControlsTutorialPosition.Y + 360), null, Color.White, 0, new Vector2(ArrowTexture.Width / 2, ArrowTexture.Height / 2), 1.0f, SpriteEffects.None, 0);
            #endregion
        }


        /// <summary>
        /// Slides all the menus a given distance.
        /// </summary>
        /// <param name="distance"></param>
        public void MoveMenus(float distance)
        {
            MainMenuPosition.X += distance;
            GameSelectPosition.X += distance;
            PlayerSelectPosition.X += distance;
            HighscoresPosition.X += distance;
            OptionsPosition.X += distance;
            ExtrasPosition.X += distance;
            PlayerTutorialPosition.X += distance;
            EnemyTutorialPosition.X += distance;
            GameModeTutorialPosition.X += distance;
            ControlsTutorialPosition.X += distance;
        }

        // Spawns a few planets
        public void SpawnPlanets(List<Particle> List, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Texture2D text = null;
                int ran = r.Next(6);
                if (ran == 0)
                {
                    text = Planet1;
                }
                else if (ran == 1)
                {
                    text = Planet2;
                }
                else if (ran == 2)
                {
                    text = Planet3;
                }
                else if (ran == 3)
                {
                    text = Planet4;
                }
                else if (ran == 4)
                {
                    text = Planet5;
                }
                else if (ran == 5)
                {
                    text = Planet6;
                }

                Vector2 pos = new Vector2(r.Next(-100, 1380), r.Next(-100, 820));

                Particle p = new Particle(text, pos);
                float xVel = (float)(r.Next(-100, 100) / 3000.0f);
                float yVel = (float)(r.Next(-100, 100) / 3000.0f);
                p.Velocity = new Vector2(xVel, yVel);
                p.Rotation = (float)(r.Next(628) / 10.0f);
                p.Scale = (float)(r.Next(50, 125) / 100.0f);
                p.PlanetParticle = true;

                List.Add(p);
            }
        }

        // Spawns a layer of stars
        public void SpawnBackgroundLayer(int count, List<BackgroundStar> List)
        {
            for (int i = 0; i < count; i++)
            {
                float fade = (float)(r.Next(5) / 200.0f);
                float alpha = (float)(r.Next(100) / 100.0f);
                Vector2 pos = new Vector2((float)(r.Next(-100, 1380)), (float)(r.Next(-100, 820)));

                int ran = r.Next(100);
                Texture2D text = null;
                if (ran == 0)
                {
                    text = Star1;
                }
                else if (ran == 1)
                {
                    text = Star2;
                }
                else if (ran == 2)
                {
                    text = Star3;
                }
                else if (ran == 3)
                {
                    text = Star4;
                }
                else if (ran == 4)
                {
                    text = Star5;
                }
                else
                {
                    text = Star6;
                }

                BackgroundStar b = new BackgroundStar(text, pos, fade, alpha);

                List.Add(b);
            }
        }

        /// <summary>
        /// This handles selecting players.
        /// </summary>
        /// <param name="gameTime"></param>
        private void PlayerSelect(GameTime gameTime)
        {
            // Choose the players
            if (gps1.IsButtonDown(Buttons.Y) && SelectButtonOneReady)
            {
                PlayerOneChosen = !PlayerOneChosen;
                SelectButtonOneReady = false;
            }

            if (gps2.IsButtonDown(Buttons.Y) && SelectButtonTwoReady)
            {
                PlayerTwoChosen = !PlayerTwoChosen;
                SelectButtonTwoReady = false;
            }

            if (gps3.IsButtonDown(Buttons.Y) && SelectButtonThreeReady && !IsTrial)
            {
                PlayerThreeChosen = !PlayerThreeChosen;
                SelectButtonThreeReady = false;
            }

            if (gps4.IsButtonDown(Buttons.Y) && SelectButtonFourReady && !IsTrial)
            {
                PlayerFourChosen = !PlayerFourChosen;
                SelectButtonFourReady = false;
            }

            // Start the game
            if (((gps1.IsButtonDown(Buttons.A) && PlayerOneChosen) ||
                (gps2.IsButtonDown(Buttons.A) && PlayerTwoChosen) ||
                (gps3.IsButtonDown(Buttons.A) && PlayerThreeChosen) ||
                (gps4.IsButtonDown(Buttons.A) && PlayerFourChosen)) && AButtonReady)
            {
                AButtonReady = false;
                if (!(gameType.Equals(Level.MatchType.Versus) &&
                    !((PlayerOneChosen && PlayerTwoChosen) || (PlayerOneChosen && PlayerThreeChosen) || (PlayerOneChosen && PlayerFourChosen) || (PlayerTwoChosen && PlayerThreeChosen)
                        || (PlayerTwoChosen && PlayerFourChosen) || (PlayerThreeChosen && PlayerFourChosen))))
                {
                    gameState = GameState.GameModeTutorial;
                }
            }
        }

        // Pause menu stuff
        #region Pause Menu
        public void Pause(PlayerIndex pi)
        {
            gameState = GameState.Paused;
            PausedController = pi;
            PauseElementSelected = "Resume";
        }
        public void UnPause()
        {
            gameState = GameState.Level;

            // Start the pause timers for all the ships
            foreach (Pod p in level.Pods)
            {
                p.PauseReady = 1;
            }
        }

        public void UpdatePause(GameTime gameTime)
        {
            // Unrumble each controller
            foreach (Pod p in level.Pods)
            {
                GamePad.SetVibration(p.PlayerIndex, 0, 0);
            }

            // Get the states of all the controllers
            List<GamePadState> gpstates = new List<GamePadState>();
            gpstates.Add(GamePad.GetState(PausedController));

            foreach (GamePadState p in gpstates)
            {
                if (p.ThumbSticks.Left.Y < -0.5 || p.IsButtonDown(Buttons.DPadDown))
                {
                    if (MenuTimer > ChangeElementTime)
                    {
                        switch (PauseElementSelected)
                        {
                            case "Resume":
                                {
                                    PauseElementSelected = "Restart";
                                    break;
                                }
                            case "Restart":
                                {
                                    PauseElementSelected = "MainMenu";
                                    break;
                                }
                            case "MainMenu":
                                {
                                    PauseElementSelected = "Resume";
                                    break;
                                }
                        }
                        MenuTimer = 0;

                        MenuScrollSound.Play(Game1.Global.SoundEffectVolume, 0, 0);

                        // Don't fight the controllers
                        return;
                    }
                }
                else if (p.ThumbSticks.Left.Y > 0.5 || p.IsButtonDown(Buttons.DPadUp))
                {
                    if (MenuTimer > ChangeElementTime)
                    {
                        switch (PauseElementSelected)
                        {
                            case "Resume":
                                {
                                    PauseElementSelected = "MainMenu";
                                    break;
                                }
                            case "Restart":
                                {
                                    PauseElementSelected = "Resume";
                                    break;
                                }
                            case "MainMenu":
                                {
                                    PauseElementSelected = "Restart";
                                    break;
                                }
                        }
                        MenuTimer = 0;

                        MenuScrollSound.Play(Game1.Global.SoundEffectVolume, 0, 0);

                        //Don't fight the controllers
                        return;
                    }
                }
                else if (p.IsButtonDown(Buttons.A))
                {
                    switch (PauseElementSelected)
                    {
                        case "Resume":
                            {
                                UnPause();
                                break;
                            }
                        case "MainMenu":
                            {
                                level.StartReset();
                                level.ReturnToMenu = true;

                                gameState = GameState.Level;
                                
                                // Change back to menu song
                                MediaPlayer.Play(MenuSong);
                                break;
                            }
                        case "Restart":
                            {
                                level.StartReset();

                                gameState = GameState.Level;
                                break;
                            }
                    }

                    MenuClickSound.Play(Game1.Global.SoundEffectVolume, 0, 0);
                }
                else if (p.IsButtonDown(Buttons.Start) && StartButtonReady)
                {
                    UnPause();
                    StartButtonReady = false;
                    MenuTimer = 0;
                }
            }
        }

        #endregion

        // All highscores stuff

        public static bool FilterHighscore(IGameComponent gc)
        {
            return gc is HighscoreComponent;
        }

        public void hsc_HighscoresChanged(HighscoreComponent sender, Highscore highestScore)
        {
            RebuildHighscores();
        }

        public void RebuildHighscores()
        {
            data_ = new Highscore[20];
            Highscore[] toShow = new Highscore[20];
            int n;
            n = level.hsc_.GetHighscores(toShow);
            // string me = HighscoreComponent.NameFromPlayer(MainProgram.Global.ActiveController.Player);
            data_ = toShow;
        }

        public void RebuildHighscores(string matchType)
        {
            data_ = new Highscore[10];
            Highscore[] toShow = new Highscore[10];
            int n;
            n = level.hsc_.GetHighscores(toShow, matchType);
            data_ = toShow;
        }

        public void RebuildLocalHighscores(string matchType)
        {
            localScores_ = new Highscore[10];

            PlayerIndex one = PlayerIndex.One;
            PlayerIndex two = PlayerIndex.Two;
            PlayerIndex three = PlayerIndex.Three;
            PlayerIndex four = PlayerIndex.Four;

            Highscore[] oneData_ = new Highscore[10];
            Highscore[] twoData_ = new Highscore[10];
            Highscore[] threeData_ = new Highscore[10];
            Highscore[] fourData_ = new Highscore[10];

            if (!HighscoreComponent.NameFromPlayer(one).Equals("Guest"))
            {
                level.hsc_.GetHighscores(oneData_, one, matchType);
            }
            if (!HighscoreComponent.NameFromPlayer(two).Equals("Guest"))
            {
                level.hsc_.GetHighscores(twoData_, two, matchType);
            }
            if (!HighscoreComponent.NameFromPlayer(three).Equals("Guest"))
            {
                level.hsc_.GetHighscores(threeData_, three, matchType);
            }
            if (!HighscoreComponent.NameFromPlayer(four).Equals("Guest"))
            {
                level.hsc_.GetHighscores(fourData_, four, matchType);
            }

            List<Highscore> allScores = new List<Highscore>();

            for (int i = 0; i < oneData_.Length; i++)
            {
                if (oneData_[i] != null)
                {
                    allScores.Add(oneData_[i]);
                }
            }
            for (int i = 0; i < twoData_.Length; i++)
            {
                if (twoData_[i] != null)
                {
                    allScores.Add(twoData_[i]);
                }
            }
            for (int i = 0; i < threeData_.Length; i++)
            {
                if (threeData_[i] != null)
                {
                    allScores.Add(threeData_[i]);
                }
            }
            for (int i = 0; i < fourData_.Length; i++)
            {
                if (threeData_[i] != null)
                {
                    allScores.Add(fourData_[i]);
                }
            }

            allScores.Sort();

            for (int i = 0; i < 10; i++)
            {
                if (localScores_.Length > i && allScores.Count > i)
                {
                    if (allScores[i] != null)
                    {
                        localScores_[i] = allScores[i];
                    }
                }
            }

        }


        // Can buy the game?
        public static bool CanPlayerBuyGame(PlayerIndex player)
        {
            SignedInGamer gamer = Gamer.SignedInGamers[player];

            // if the player isn't signed in, they can't buy games
            if (gamer == null)
                return false;

            // lastly check to see if the account is allowed to buy games
            return gamer.Privileges.AllowPurchaseContent;
        }
    }
}
