using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Nez;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SpiderStrider
{
    public class WaveManager : Entity
    {
        public int currentWave = 1;
        private bool active = true;
        Text waveLabel;


        private TimerCallback spawnCallback;
        Timer spawnTimer;
        FlyCounter flyCounter;

        public List<Entity> Wave
        {
            get {return waveList[currentWave-1];}
        }

        List<List<Entity>> waveList = new List<List<Entity>>();

        public WaveManager()
        {
            SpriteFont font = GameManager.gameScene.content.Load<SpriteFont>("testFont");
            NezSpriteFont nezFont = new NezSpriteFont(font);

            Text text = new Text(nezFont, $"Wave {currentWave}", new Vector2(500, 0), Color.Yellow);
            addComponent(text);
            waveLabel = text;
            waveLabel.transform.scale = new Vector2(2, 2);


            waveList = new List<List<Entity>>();
            
            waveList.Add(GenerateWave(new EnemyWave[]
            {
                new EnemyWave(typeof(Fly),19),
                new EnemyWave(typeof(RedFly),1)

            }));

            waveList.Add(GenerateWave(new EnemyWave[]
            {
                new EnemyWave(typeof(Fly),15),
                new EnemyWave(typeof(BlueFly),1),
                new EnemyWave(typeof(RedFly),5)

            }));
            
            waveList.Add(GenerateWave(new EnemyWave[]
            {
                new EnemyWave(typeof(BlueFly),15),
                new EnemyWave(typeof(RedFly),5)

            }));
            
            waveList.Add(GenerateWave(new EnemyWave[]
            {
                new EnemyWave(typeof(Wasp),1)
            }));

            waveList.Add(GenerateWave(new EnemyWave[]
            {
                new EnemyWave(typeof(RedFly),25)
            }));

            waveList.Add(GenerateWave(new EnemyWave[]
            {
                new EnemyWave(typeof(Fly),10),
                new EnemyWave(typeof(BlueFly),10),
                new EnemyWave(typeof(RedFly),10)
            }));
            
            waveList.Add(GenerateWave(new EnemyWave[]
            {
                new EnemyWave(typeof(Wasp),2),
                new EnemyWave(typeof(Fly),15),
                new EnemyWave(typeof(RedFly),15)
            }));
            
            waveList.Add(GenerateWave(new EnemyWave[]
            {
                new EnemyWave(typeof(BlueFly),15),
                new EnemyWave(typeof(RedWasp),1),
                new EnemyWave(typeof(RedFly),15)
            }));
            
            waveList.Add(GenerateWave(new EnemyWave[]
            {
                new EnemyWave(typeof(Wasp),5),
                new EnemyWave(typeof(RedFly),20)
            }));

            waveList.Add(GenerateWave(new EnemyWave[]
            {
                new EnemyWave(typeof(Wasp),5),
                new EnemyWave(typeof(RedWasp),5),
            }));


            //define a few waves.
            updateOrder = 2;
        }
        
        private List<Entity> GenerateWave(EnemyWave[] enemyWaves)
        {
            List<Entity> wave = new List<Entity>();
            foreach(EnemyWave enemyWave in enemyWaves)
                wave.AddRange(enemyWave.GenerateEnemies());

            return wave;
        }

        struct EnemyWave
        {
            private Type Enemy;
            int Amount;

            public EnemyWave(Type enemy, int amount)
            {
                Enemy = enemy;
                Amount = amount;
            }

            public List<Entity> GenerateEnemies()
            {
                List<Entity> enemyList = new List<Entity>();
                for(int i = 0;i<Amount;i++)
                {
                    var enemy = Activator.CreateInstance(Enemy);
                    enemyList.Add((Entity)enemy);
                }
                return enemyList;
            }
        }

        private void AddEntitiesToWave(int amount, Action callback)
        {
            for (int i = 0; i < amount; i++)
                callback();
        }


        public void Start()
        {
            RegisterWave();

            spawnCallback = new TimerCallback(SpawnEnemy);
            spawnTimer = new Timer(spawnCallback);
            spawnTimer.Change(1000, Timeout.Infinite);
        }

        private void RegisterWave()
        {
            flyCounter = new FlyCounter();

            Wave.ForEach((entity) =>
            {
                ((Fly)entity).Register(flyCounter);
            });
        }

        private void NextWave()
        {
            GameManager.gameScene.content.Load<SoundEffect>("NextLevel").Play();
            currentWave++;

            if(currentWave<=waveList.Count)
            {
                RegisterWave();
                spawnTimer.Change(100, Timeout.Infinite);
            }
            else
                GameFinish();
        }

        public override void update()
        {
            base.update();

            UpdateCamera();

            if (flyCounter.Count <= 0 && active)
            {
                NextWave();
            }
        }

        private void UpdateCamera()
        {
            GameManager.gameScene.camera.transform.position = new Vector2(GameManager.Width / 2, GameManager.Height / 2);

            var bounds = GameManager.gameScene.camera.bounds;
            if(!WithinBounds(bounds.y,-0.02f,0.02f))
            {
                AdjustZoomByY();
            }

            //if (InputManager.IsDown(Buttons.LeftShoulder))
            //    GameManager.gameScene.camera.zoomIn(0.02f);
            //if (InputManager.IsDown(Buttons.RightShoulder))
            //    GameManager.gameScene.camera.zoomOut(0.02f);
            //if (InputManager.IsDown(Buttons.Start))
            //    Console.Write("");
        }

        private bool WithinBounds(float val, float lower, float upper)
        {
            return lower < val && val < upper;
        }

        private void AdjustZoomByX()
        {
            var camera = GameManager.gameScene.camera;
            if (camera.bounds.x<0)
            {
                while (camera.bounds.x < 0.01f) camera.zoomIn(0.001f);
            }
            else
            {
                while (camera.bounds.x > 0.01f) camera.zoomOut(0.001f);
            }
        }

        private void AdjustZoomByY()
        {
            var camera = GameManager.gameScene.camera;
            if (camera.bounds.y < 0)
            {
                while (camera.bounds.y < 0.01f) camera.zoomIn(0.001f);
            }
            else
            {
                while (camera.bounds.y > 0.01f) camera.zoomOut(0.001f);
            }
        }

        private void SpawnEnemy(Object o)
        {
            if (currentWave <= waveList.Count)
            {
                if(currentWave==waveList.Count)
                    waveLabel.text = $"FINAL WAVE";
                else
                    waveLabel.text = $"Wave {currentWave}";
                if (Wave.Count != 0)
                {
                    int i = Nez.Random.nextInt(Wave.Count);
                    GameManager.gameScene.addEntity(Wave[i]);
                    Wave.RemoveAt(i);

                    //TODO: Change static time 400, 1200 to customizable interval Wave class or similar.
                    spawnTimer.Change(Nez.Random.range(400, 1200), Timeout.Infinite);
                }
            }
        }

        public void GameFinish()
        {
            GameManager.gameScene.destroyAllEntities();
            active = false;
            currentWave = waveList.Count + 1;
            MediaPlayer.Stop();
            
            var gameOverScene = Scene.createWithDefaultRenderer(Color.DarkGray);
            Core.scene = gameOverScene;

            var nezFont = new NezSpriteFont(gameOverScene.content.Load<SpriteFont>("testFont"));

            gameOverScene.content.Load<SoundEffect>("GameOverJingle").Play(0.4f, 0.0f, 0.0f);

            HighScoreManager.highscores.Add(new Highscore(GameManager.playerName, ScoreCounter.Points));
            HighScoreManager.highscores.Sort((a, b) => b.score.CompareTo(a.score));
            HighScoreManager.SaveFile();

            gameOverScene.addEntity(GameManager.GetHighScoreBoard(nezFont));

            GameOverText gameOverText = new GameOverText("GAME OVER", nezFont, Color.Red);
            TitleText score = new TitleText($"SCORE: {ScoreCounter.Points}", nezFont, Color.Yellow);

            gameOverScene.addEntity(gameOverText);

            gameOverText.transform.scale = new Vector2(8, 8);
            gameOverText.position = new Vector2(GameManager.Width / 2, GameManager.Height / 5);
            gameOverText.position -= new Vector2(gameOverText.text.width / 2, gameOverText.text.height / 2);

            gameOverScene.addEntity(score);

            score.transform.scale = new Vector2(3, 3);
            score.position = new Vector2(GameManager.Width / 2, GameManager.Height / 3);
            score.position -= new Vector2(score.text.width / 2, score.text.height / 2);

        }
    }
}