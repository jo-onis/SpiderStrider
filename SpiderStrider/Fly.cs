using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Nez;
using Nez.Sprites;
using Nez.TextureAtlases;
using System;
using System.Threading;

namespace SpiderStrider
{
    public class Fly : Entity
    {
        FlyCounter flyCounter;

        protected int wallBumps = 5;
        protected float baseSpeed = 4;
        public int points = 12;
        public float stamina = 2;

        protected Vector2 movement;

        protected enum Animations
        {
            flap
        }

        public Fly()
        {
            InitializeFly();
        }

        //Gets random spawnpoint at edge
        protected Vector2 randomSpawnPoint()
        {
            if (Nez.Random.chance(0.5f))
                return new Vector2(Nez.Random.chance(0.5f) ? 0 : GameManager.Width, Nez.Random.nextInt(GameManager.Height));
            else
                return new Vector2(Nez.Random.nextInt(GameManager.Width), Nez.Random.chance(0.5f) ? 0 : GameManager.Height);
        }

        public virtual void SetStats()
        {
            wallBumps = 5;
            baseSpeed = 4;
            points = 12;
            stamina = 2;
        }

        public virtual void InitializeFly()
        {
            SetStats();

            var angle = Nez.Random.nextAngle();
            var speed = baseSpeed + baseSpeed * Nez.Random.nextFloat();

            movement = new Vector2(Mathf.cos(angle) * speed, Mathf.sin(angle) * speed);
            rotation = Helpers.VectorAngle(movement);

            TextureAtlas atlas = GameManager.gameScene.content.Load<TextureAtlas>("flyAtlas");
            SpriteAnimation flap = atlas.getSpriteAnimation("flying");

            addComponent(new Sprite<Animations>(Animations.flap, flap));

            getComponent<Sprite<Animations>>().play(Animations.flap);

            var collider = addComponent<BoxCollider>();
            collider.setSize(atlas.subtextures[0].texture2D.Width / 2.5f, atlas.subtextures[0].texture2D.Height / 2.5f);

            collider.physicsLayer = (int)Layers.enemy;

            transform.position = randomSpawnPoint();
        }

        public void Drain(float amount)
        {
            stamina -= amount;

            if (stamina <= 0)
            {
                destroy();
            }
        }

        public override void update()
        {
            base.update();

            if (stamina > 0)
            {
                bool move = true;

                CollisionResult collisionResult;
                var thisCollider = getComponent<BoxCollider>();
                var colliders = Physics.getAllColliders();
                foreach (Collider collider in colliders)
                {
                    if (thisCollider == collider) continue;

                    if (collider.physicsLayer == (int)Layers.web)
                    {
                        if (thisCollider.collidesWith(collider, out collisionResult))
                        {
                            move = false;
                            stamina -= 0.1f;
                            ((Web)collider.entity).Damage(0.1f);
                        }
                    }
                }

                if (move)
                    transform.position += movement;

                if (transform.position.X < 0)
                {
                    movement.X = Math.Abs(movement.X);
                    wallBumps--;
                }
                if (transform.position.Y < 0)
                {
                    movement.Y = Math.Abs(movement.Y);
                    wallBumps--;
                }
                if (transform.position.X > GameManager.Width)
                {
                    movement.X = -Math.Abs(movement.X);
                    wallBumps--;
                }
                if (transform.position.Y > GameManager.Height)
                {
                    movement.Y = -Math.Abs(movement.Y);
                    wallBumps--;
                }

                if (wallBumps <= 0)
                    destroy();

                rotation = Helpers.VectorAngle(movement);
            }
            else
            {
                getComponent<Sprite<Animations>>().stop();
                getComponent<Sprite>().setColor(Color.LightGreen);
            }
        }

        internal void Register(FlyCounter flyCounter)
        {
            this.flyCounter = flyCounter;
            this.flyCounter.Count++;
        }

        public override void onRemovedFromScene()
        {
            if(flyCounter!=null) flyCounter.Count--;
            base.onRemovedFromScene();
        }
    }

    public class RedFly : Fly
    {
        public RedFly() : base()
        {
        }

        public override void SetStats()
        {
            baseSpeed = 8;
            wallBumps = 3;
            points = 51;
            stamina = 4;
        }

        public override void onAddedToScene()
        {
            base.onAddedToScene();
            GameManager.gameScene.content.Load<SoundEffect>("RedBzz").Play();
        }
        
        public override void InitializeFly()
        {
            base.InitializeFly();
            getComponent<Sprite>().color = Color.Red;
        }
    }


    public class BlueFly : Fly
    {
        private float rotationChanger = 0;
        private Timer rotationChangeTimer;

        public BlueFly() : base()
        {
        }

        public override void SetStats()
        {
            baseSpeed = 6.5f;
            wallBumps = 8;
            points = 21;
            stamina = 2.5f;
        }

        public override void InitializeFly()
        {
            base.InitializeFly();

            rotationChangeTimer = new Timer(new TimerCallback(ChangeRotation));
            rotationChangeTimer.Change(1000, Timeout.Infinite);

            getComponent<Sprite>().color = Color.Blue;
        }

        private void ChangeRotation(object o)
        {
            rotationChanger = Nez.Random.nextFloat() * (Nez.Random.chance(0.5f) ? 0.05f : -0.05f);
            rotationChangeTimer.Change(Nez.Random.range(500, 1500), Timeout.Infinite);
        }

        public override void update()
        {
            movement = Helpers.RotateVector(movement, rotationChanger);
            base.update();
        }
    }

    public class Wasp : Fly
    {
        private float rotationDelta = 0.02f;
        private float rotationChanger = -0.05f;
        private Timer rotationChangeTimer;

        public Wasp() : base()
        {
        }

        public override void SetStats()
        {
            baseSpeed = 3f;
            wallBumps = 50;
            points = 200;
            stamina = 20f;
        }


        public override void onAddedToScene()
        {
            base.onAddedToScene();
            GameManager.gameScene.content.Load<SoundEffect>("WaspBzz").Play();
        }

        public override void InitializeFly()
        {
            SetStats();

            rotationChangeTimer = new Timer(new TimerCallback(ChangeRotation));
            rotationChangeTimer.Change(1000, 500);


            transform.position = randomSpawnPoint();
            var angle = Helpers.AngleTowards(position, GameManager.player.transform.position);
            var speed = baseSpeed + baseSpeed * Nez.Random.nextFloat();
            movement = new Vector2(Mathf.cos(angle) * speed, Mathf.sin(angle) * speed);
            rotation = Helpers.VectorAngle(movement);

            TextureAtlas atlas = GameManager.gameScene.content.Load<TextureAtlas>("waspAtlas");
            SpriteAnimation flap = atlas.getSpriteAnimation("flap");

            addComponent(new Sprite<Animations>(Animations.flap, flap));
            getComponent<Sprite<Animations>>().play(Animations.flap);

            var collider = addComponent<BoxCollider>();
            collider.setSize(atlas.subtextures[0].texture2D.Width / 3f, atlas.subtextures[0].texture2D.Height / 3f);

            collider.physicsLayer = (int)Layers.enemy;
        }

        private void ChangeRotation(object o)
        {
            rotationDelta *= -1;
        }
        
        public override void update()
        {
            rotationChanger += rotationDelta;

            var angle = Helpers.AngleTowards(position, GameManager.player.position) + rotationChanger;
            movement = new Vector2(Mathf.cos(angle) * baseSpeed, Mathf.sin(angle) * baseSpeed);

            base.update();
        }
    }

    public class RedWasp : Wasp
    {
        public RedWasp() : base()
        {
        }

        public override void SetStats()
        {
            baseSpeed = 7;
            wallBumps = 50;
            points = 500;
            stamina = 40;
        }

        public override void onAddedToScene()
        {
            base.onAddedToScene();
            GameManager.gameScene.content.Load<SoundEffect>("WaspBzz").Play(1,1,0);
        }

        public override void InitializeFly()
        {
            base.InitializeFly();
            getComponent<Sprite>().color = Color.Red;
        }
    }
}