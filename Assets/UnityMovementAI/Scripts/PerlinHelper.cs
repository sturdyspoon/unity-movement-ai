using UnityEngine;

namespace UnityMovementAI
{
    public class PerlinHelper
    {
        public int width;
        public int height;
        public Vector2 offset;
        public Vector2 scale;

        public float this[int x, int y]
        {
            get
            {
                float i = offset.x + (float)x / width * scale.x;
                float j = offset.y + (float)y / height * scale.y;

                return Mathf.PerlinNoise(i, j);
            }
        }

        public PerlinHelper(int width, int height, float perlinScale)
        {
            this.width = width;
            this.height = height;

            offset = new Vector2(Random.value * 100000, Random.value * 100000);

            float rAspect = (float)width / height;
            scale = new Vector2(perlinScale * rAspect, perlinScale);
        }
    }
}