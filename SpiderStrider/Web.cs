using Microsoft.Xna.Framework.Audio;
using Nez;
using Nez.Sprites;
using Nez.TextureAtlases;

namespace SpiderStrider
{
    public class Web : Entity
    {
        SoundEffect webRip;

        public float hitpoints;


        private enum Animations
        {
            shake
        }

        public Web(float hitpoints)
        {
            this.hitpoints = hitpoints;

            Spider.NetCount++;

            webRip = GameManager.gameScene.content.Load<SoundEffect>("WebRip");

            TextureAtlas atlas = GameManager.gameScene.content.Load<TextureAtlas>("webAtlas");
            SpriteAnimation webShake = atlas.getSpriteAnimation("shake");
            

            addComponent(new Sprite<Animations>(Animations.shake, webShake));

            getComponent<Sprite<Animations>>().play(Animations.shake);

            var collider = addComponent<CircleCollider>();
            collider.radius = 64;

            collider.physicsLayer = (int)Layers.web;

            getComponent<Sprite<Animations>>().play(Animations.shake);
            getComponent<Sprite<Animations>>().pause();

        }

        public void Damage(float damage)
        {
            hitpoints -= damage;

            if (hitpoints <= 0)
            {
                webRip.Play();
                destroy();
            }
        }

        float shookTime = Time.time;

        public void shake()
        {
            shookTime = Time.time;
            getComponent<Sprite<Animations>>().unPause();
        }

        public override void update()
        {
            base.update();
            if (Time.time - shookTime > 0.5f)
            {
                shookTime = Time.time;
                getComponent<Sprite<Animations>>().pause();
            }
        }

        public override void onRemovedFromScene()
        {
            Spider.NetCount--;
            base.onRemovedFromScene();
        }
    }
}