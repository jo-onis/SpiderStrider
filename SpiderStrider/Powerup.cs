using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;

namespace SpiderStrider
{
    public class Powerup : Entity
    {
        public PowerEffect powerupEffect; 

        public Powerup()
        {
            powerupEffect = (PowerEffect)Random.range(0, 4);

            transform.position = RandomSpawnPoint();
            
            Texture2D texture;

            switch (powerupEffect)
            {
                case PowerEffect.Durability:
                    texture = GameManager.gameScene.content.Load<Texture2D>("powerups/durability");
                    break;
                case PowerEffect.Size:
                    texture = GameManager.gameScene.content.Load<Texture2D>("powerups/size");
                    break;
                case PowerEffect.Speed:
                    texture = GameManager.gameScene.content.Load<Texture2D>("powerups/speed");
                    break;
                default:
                    texture = GameManager.gameScene.content.Load<Texture2D>("powerups/cuantity");
                    break;
            }

            addComponent(new Sprite(texture));

            var collider = addComponent<BoxCollider>();
            collider.setSize(texture.Width, texture.Height);

            collider.physicsLayer = (int)Layers.powerups;
        }

        private Vector2 RandomSpawnPoint()
        {
            return new Vector2(Random.range(100, GameManager.Width - 100), Random.range(100, GameManager.Height - 100));
        }
    }
}