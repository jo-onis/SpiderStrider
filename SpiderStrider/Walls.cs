using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;

namespace SpiderStrider
{
    public class Wall : Entity
    {
        public Wall(float x, float y)
        {
            Texture2D wallTexture = GameManager.gameScene.content.Load<Texture2D>("wall");
            addComponent(new Sprite(wallTexture));
            transform.position = new Vector2(x, y);

            var collider = addComponent<BoxCollider>();
            collider.setSize(wallTexture.Width, wallTexture.Height);
            collider.setShouldColliderScaleAndRotateWithTransform(true);

            collider.physicsLayer = (int)Layers.wall;
        }
    }

    public class TriangleWall : Entity
    {
        public TriangleWall(float x, float y)
        {
            Texture2D texture = GameManager.gameScene.content.Load<Texture2D>("triangleWall");
            var sprite = addComponent(new Sprite(texture));


            transform.position = new Vector2(x, y);

            Vector2 v1 = new Vector2(-texture.Width / 2, texture.Height / 2);
            Vector2 v2 = new Vector2(0, -texture.Height / 2);
            Vector2 v3 = new Vector2(texture.Width / 2, texture.Height / 2);
            Vector2[] vertices = new Vector2[] { v1, v2, v3 };


            PolygonCollider collider = new PolygonCollider(vertices);
            addComponent(collider);
            collider.setShouldColliderScaleAndRotateWithTransform(true);

            collider.physicsLayer = (int)Layers.wall;
        }
    }

    public class RotatingTriangle : TriangleWall
    {
        public RotatingTriangle(float x, float y) : base(x, y)
        {

        }

        public override void update()
        {
            base.update();
            rotation += 0.015f;
        }
    }

    public class RotatingWall : Wall
    {
        public RotatingWall(float x, float y) : base(x, y)
        {

        }

        public override void update()
        {
            base.update();
            rotation += 0.01f;
        }
    }
}