using Microsoft.Xna.Framework;
using Nez;
using System;

namespace SpiderStrider
{
    public static class Helpers
    {
        public static float VectorAngle(Vector2 vector)
        {
            return Mathf.atan2(vector.Y, vector.X) - MathHelper.Pi / 2;
        }

        public static Vector2 RotateVector(Vector2 origin, float angle)
        {
            double x2 = Mathf.cos(angle) * origin.X - Mathf.sin(angle) * origin.Y;
            double y2 = Mathf.sin(angle) * origin.X + Mathf.cos(angle) * origin.Y;
            return new Vector2((float)x2, (float)y2);
        }

        public static float AngleTowards(Vector2 origin, Vector2 target)
        {
            return VectorAngle(target-origin) + (float)(Math.PI/2);
        }
    }

    public enum PowerEffect
    {
        Durability = 0,
        Speed = 1,
        Cuantity = 2,
        Size = 3
    }

    public enum Layers
    {
        wall = 5,
        web = 4,
        enemy = 3,
        player = 2,
        powerups = 6
    }
}