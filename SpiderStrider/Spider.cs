using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Sprites;
using Nez.TextureAtlases;

namespace SpiderStrider
{
    public class Spider : Entity
    {
        private float speed = 6;
        private float netDurability = 1;
        private float netSize = 1;
        private float netCuantity = 4;

        SoundEffect makeWebSound;
        SoundEffect eat;
        public WaveManager waveManager;

        private enum Animations
        {
            walk
        }

        public Spider()
        {
            TextureAtlas spiderAtlas = GameManager.gameScene.content.Load<TextureAtlas>("spiderAtlas");
            SpriteAnimation spiderWalk = spiderAtlas.getSpriteAnimation("walk");
            addComponent(new Sprite<Animations>(Animations.walk, spiderWalk));

            transform.setLocalPosition(new Vector2(GameManager.Width/2, GameManager.Height / 2));

            getComponent<Sprite<Animations>>().play(Animations.walk);

            var collider = addComponent<CircleCollider>();
            collider.radius = 32;

            collider.physicsLayer = (int)Layers.player;

            makeWebSound = GameManager.gameScene.content.Load<SoundEffect>("MakeWeb");
            eat = GameManager.gameScene.content.Load<SoundEffect>("Eat");
        }

        public override void update()
        {
            base.update();
            
            Vector2 movementDir = InputManager.GetMovementVector();

            movementDir.Y *= -1;
            movementDir *= speed;


            if (movementDir != Vector2.Zero)
            {
                getComponent<Sprite<Animations>>().unPause();
                transform.rotation = Helpers.VectorAngle(movementDir);
            }
            else
                getComponent<Sprite<Animations>>().pause();

            bool raiseSpeed = false;
            CollisionResult collisionResult;
            var thisCollider = getComponent<CircleCollider>();
            var colliders = Physics.getAllColliders();
            foreach (Collider collider in colliders)
            {
                if (thisCollider == collider) continue;

                if (collider.physicsLayer == (int)Layers.wall)
                {
                    if (thisCollider.collidesWith(collider, out collisionResult))
                        movementDir -= collisionResult.minimumTranslationVector;
                }
                else if (collider.physicsLayer == (int)Layers.web)
                {
                    if (thisCollider.collidesWith(collider, out collisionResult))
                    {
                        raiseSpeed = true;
                        WebCollision((Web)collisionResult.collider.entity);
                    }
                }
                else if (collider.physicsLayer == (int)Layers.enemy)
                {
                    if (thisCollider.collidesWith(collider, out collisionResult))
                    {
                        if (((Fly)collider.entity).stamina <= 0)
                        {
                            collider.entity.destroy();
                            eat.Play();
                            ScoreCounter.Points += ((Fly)collider.entity).points;
                        }
                        else if (collider.entity is Wasp)
                        {
                            waveManager.GameFinish();
                        }
                    }
                }
                else if (collider.physicsLayer == (int)Layers.powerups)
                {
                    if (thisCollider.collidesWith(collider, out collisionResult))
                    {
                        GameManager.gameScene.content.Load<SoundEffect>("powerups/powerupSound").Play(0.4f, 0.0f, 0.0f);

                        switch (((Powerup)collider.entity).powerupEffect)
                        {
                            case PowerEffect.Durability:
                                netDurability += 0.2f;
                                break;
                            case PowerEffect.Size:
                                netSize += 0.2f;
                                break;
                            case PowerEffect.Speed:
                                speed += 0.2f;
                                break;
                            default:
                                netCuantity++;
                                break;

                        }
                        collider.entity.destroy();
                    }
                }
            }

            if (raiseSpeed) movementDir *= 1.6f;


            transform.position += movementDir;
            
            MakeWeb();
        }

        public static int NetCount = 0;

        private void MakeWeb()
        {
            if (NetCount >= netCuantity) return;

            if (InputManager.Pressed(Buttons.A) || InputManager.KeyPressed(Keys.Space) || InputManager.KeyPressed(Keys.E))
            {
                makeWebSound.Play();

                Web web = new Web(10 * netDurability);

                web.transform.position = transform.position;
                web.transform.rotation = transform.rotation;

                web.transform.scale *= netSize;

                GameManager.gameScene.addEntity(web);
            }
        }

        private void WebCollision(Web collidedWeb)
        {
            collidedWeb.Damage(0.1f);
            collidedWeb.shake();
        }

        private void ControlCamera()
        {
            Vector2 movementDir = GamePad.GetState(PlayerIndex.One).ThumbSticks.Right;
            movementDir.Y *= -1;
            GameManager.gameScene.camera.position += movementDir * 10;

            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftShoulder)) GameManager.gameScene.camera.zoomOut(0.1f);

            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.RightShoulder)) GameManager.gameScene.camera.zoomIn(0.1f);
        }
    }
}