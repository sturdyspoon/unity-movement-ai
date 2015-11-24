using UnityEngine;
using System.Collections;

public class RandomizeTerrain : MonoBehaviour {

    public float perlinScale = 10.0f;
    public float minHeight = 0f;
    public float maxHeight = 10f;

    public void randomize()
    {
        generateHeights(GetComponent<Terrain>(), perlinScale);
    }

    public void generateHeights(Terrain terrain, float perlinScale)
    {
        float minHeightPercent = minHeight / terrain.terrainData.heightmapScale.y;
        float maxHeightPercent = maxHeight / terrain.terrainData.heightmapScale.y;

        PerlinHelper ph = new PerlinHelper(terrain.terrainData.heightmapWidth, terrain.terrainData.heightmapHeight, perlinScale);

        float[,] heights = new float[terrain.terrainData.heightmapWidth, terrain.terrainData.heightmapHeight];

        for (int i = 0; i < terrain.terrainData.heightmapWidth; i++)
        {
            for (int k = 0; k < terrain.terrainData.heightmapHeight; k++)
            {
                heights[i, k] = minHeightPercent + (ph[i, k] * (maxHeightPercent - minHeightPercent));
            }
        }

        terrain.terrainData.SetHeights(0, 0, heights);
    }
}
