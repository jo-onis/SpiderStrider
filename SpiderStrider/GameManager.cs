using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Nez;
using Nez.Sprites;
using Nez.UI;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SpiderStrider
{
    public class GameManager
    {
        public static string playerName = "Player";
        public static int Width = 1200;
        public static int Height = 800;

        public static GameWindow gameWindow;

        public static HighScoreManager highScoreManager;
        public static Scene gameScene;
        public static Scene titleScene;
        public static Spider player;
        
        static ScoreCounter pointText;
        static TitleText startText;
        static TitleText Title;
        static TitleText HighScores;

        static Song theme;

        public static NezSpriteFont nezFont;

        private static void AddStartText()
        {
            nezFont = new NezSpriteFont(titleScene.content.Load<SpriteFont>("testFont"));
            
            Title = new TitleText("SPIDER STRIDER", nezFont, Color.Yellow);
            titleScene.addEntity(Title);

            Title.transform.scale = new Vector2(8, 8);
            Title.position = new Vector2(GameManager.Width / 2, GameManager.Height / 8);
            Title.position -= new Vector2(Title.text.width / 2, Title.text.height / 2);

            startText = new StartText("START", nezFont, Color.Red);
            titleScene.addEntity(startText);

            startText.transform.scale = new Vector2(3, 3);
            startText.position = new Vector2(GameManager.Width / 2, GameManager.Height / 4);
            startText.position -= new Vector2(startText.text.width / 2, startText.text.height / 2);


            titleScene.addEntity(GetHighScoreBoard(nezFont));

            NameField nameField = new NameField(nezFont, Color.Black);
            titleScene.addEntity(nameField);

            nameField.transform.scale = new Vector2(3, 3);
            nameField.position = new Vector2(GameManager.Width / 2, GameManager.Height / 3f);
            nameField.position -= new Vector2(nameField.text.width / 2, nameField.text.height / 2);

        }

        public static TitleText GetHighScoreBoard(NezSpriteFont font)
        {
            string highscores = "Highscores\n";
            foreach (Highscore score in HighScoreManager.highscores)
                highscores += $"{score.name}: {score.score}\n";

            TitleText scores = new TitleText(highscores, font, Color.White);
            scores.position = new Vector2(GameManager.Width / 2, GameManager.Height / 2.5f);
            scores.position -= new Vector2(scores.text.width, 0);
            scores.transform.scale = new Vector2(2,2);
            return scores;
        }

        public static void Start()
        {
            gameScene = Scene.createWithDefaultRenderer(Color.DarkMagenta);
            Core.scene = gameScene;
            
            nezFont = new NezSpriteFont(gameScene.content.Load<SpriteFont>("testFont"));
            
            gameScene.content.Load<SoundEffect>("StartSound").Play(0.4f, 0.0f, 0.0f);
            startText.destroy();
            Title.destroy();

            Spider spider = new Spider();
            player = spider;
            WaveManager waveManager = new WaveManager();
            spider.waveManager = waveManager;

            gameScene.addEntity(spider);

            gameScene.addEntity(waveManager);

            InitializeLevel();
            InitializeGUI();

            waveManager.Start();
        }

        private static void InitializeGUI()
        {
            pointText = new ScoreCounter("", nezFont, Color.White);
            gameScene.addEntity(pointText);
            pointText.transform.scale = new Vector2(2, 2);
        }

        private static void InitializeLevel()
        {
            theme = gameScene.content.Load<Song>("Arcado");
            MediaPlayer.Play(theme);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.5f;


            Wall topWall = new Wall(GameManager.Width/2, -128);
            topWall.scale = new Vector2(20, 4);
            gameScene.addEntity(topWall);

            Wall bottomWall = new Wall(GameManager.Width / 2, GameManager.Height + 128);
            bottomWall.scale = new Vector2(20, 4);
            gameScene.addEntity(bottomWall);

            Wall leftWall = new Wall(-128, GameManager.Height/2);
            leftWall.scale = new Vector2(4, 14);
            gameScene.addEntity(leftWall);

            Wall rightWall = new Wall(GameManager.Width + 128, GameManager.Height/2);
            rightWall.scale = new Vector2(4, 20);
            gameScene.addEntity(rightWall);
        }

        public static void LoadTitleScene()
        {
            HighScoreManager.LoadFile();

            titleScene = Scene.createWithDefaultRenderer(Color.DarkBlue);
            Core.scene = titleScene;

            titleScene.content.Load<SoundEffect>("TitleJingle").Play(0.4f, 0.0f, 0.0f);
            AddStartText();
        }
    }

    public class NameField : Entity
    {
        public Text text;

        public NameField(NezSpriteFont font, Color color)
        {
            var textComponent = new Text(font, $"Name: {GameManager.playerName}", new Vector2(0, 0), color);
            addComponent(textComponent);
            this.text = textComponent;
        }

        public override void update()
        {
            base.update();
            text.setText($"Name: {GameManager.playerName}");

            if(InputManager.KeyPressed(Keys.Back))
            {
                if(GameManager.playerName.Length>0)
                    GameManager.playerName = GameManager.playerName.Remove(GameManager.playerName.Length - 1);
            }

            GameManager.playerName += InputManager.GetPressedLetter();
        }
    }

    public class ScoreCounter : TitleText
    {
        private static int points;
        public static int Points
        {
            get { return points; }
            set
            {
                points = value;
                if(powerUpList!=null) SpawnPowerUp();
            }
        }

        private static Stack<Powerup> powerUpList;

        public ScoreCounter(string text, NezSpriteFont font, Color color) : base(text,font,color)
        {
            Points = 0;

            powerUpList = new Stack<Powerup>();

            for (int i = 0; i < 10; i++)
                powerUpList.Push(new Powerup());
        }

        private static void SpawnPowerUp()
        {
            if (powerUpList.Count > 0)
            {
                if (Points > (350 * (10 - powerUpList.Count)))
                    GameManager.gameScene.addEntity(powerUpList.Pop());
            }
        }

        public override void update()
        {
            base.update();
            setText($"Points: {Points}");
        }
    }

    public class StartText : TitleText
    {
        public StartText(string text, NezSpriteFont font, Color color) : base(text, font, color)
        {
            Timer startButtonFlash = new Timer(new TimerCallback((o) =>
            {
                setEnabled(!enabled);
            }));
            startButtonFlash.Change(1000, 1000);
        }

        public override void update()
        {
            base.update();
            if (InputManager.Pressed(Buttons.Start) || InputManager.KeyPressed(Keys.Enter))
            {
                GameManager.Start();
            }
        }
    }

    public class GameOverText : TitleText
    {
        public GameOverText(string text, NezSpriteFont font, Color color) : base(text, font, color)
        {
        }

        public override void update()
        {
            base.update();
            if (InputManager.Pressed(Buttons.Start) || InputManager.KeyPressed(Keys.Enter))
            {
                GameManager.LoadTitleScene();
            }

        }
    }

    public class TitleText : Entity
    {
        public Text text;
        
        public void setText(string newText)
        {
            text.setText(newText);
        }

        public TitleText(string text, NezSpriteFont font, Color color)
        {
            var textComponent = new Text(font, text, new Vector2(0,0), color);
            addComponent(textComponent);
            this.text = textComponent;
        }
    }
}