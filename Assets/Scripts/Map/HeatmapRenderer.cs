using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatmapRenderer : MonoBehaviour
{
    public int gridWidth = 34;
    public int gridHeight = 19;
    public float cellSize = 2f;
    public Gradient heatmapGradient;

    private Texture2D heatmapTexture;
    private Sprite heatmapSprite;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        // Create a new texture and sprite for the heatmap
        heatmapTexture = new Texture2D(gridWidth, gridHeight);
        heatmapTexture.filterMode = FilterMode.Point; // Important for clear visualization
        heatmapTexture.wrapMode = TextureWrapMode.Clamp; // Optional but can be helpful
        heatmapSprite = Sprite.Create(heatmapTexture, new Rect(0, 0, gridWidth, gridHeight), Vector2.one * 0.5f);
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = heatmapSprite;
        spriteRenderer.transform.localScale = new Vector3(cellSize, cellSize, 1f);

        float heatmapWidth = gridWidth * cellSize;
        float heatmapHeight = gridHeight * cellSize;
        spriteRenderer.transform.localScale = new Vector3(heatmapWidth, heatmapHeight, 1f) * 0.5f;
        spriteRenderer.transform.position = new Vector3(heatmapWidth / 2f, heatmapHeight / 2f, 0f);
    }

    public void UpdateHeatmap(float[,] spawnProbabilities)
    {
        // Iterate through each cell in the grid
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Get the spawn probability for the current cell
                float probability = spawnProbabilities[x, y];

                // Map the probability to a color using the heatmap gradient
                Color color = heatmapGradient.Evaluate(probability);

                // Set the color of the corresponding pixel in the heatmap texture
                heatmapTexture.SetPixel(x, y, color);
            }
        }

        // Apply the changes to the heatmap texture
        heatmapTexture.Apply();
    }
}