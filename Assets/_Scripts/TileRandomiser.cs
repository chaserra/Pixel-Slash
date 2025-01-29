using System;
using UnityEngine;

public class TileRandomiser : MonoBehaviour
{
    // Tile Key-Value paired object
    [Serializable]
    public class TileWithWeights 
    {
        public Sprite sprite;
        public float weight;
    }

    [SerializeField] private TileWithWeights[] tiles;
    [SerializeField] private int canvasSize = 5;

    private GameObject targetBackground;
    private Vector2 currentPosition;

    public void RandomiseTiles()
    {
        // Assign this object as the parent GameObject
        targetBackground = this.gameObject;

        // Reset all currently existing sprites
        if (targetBackground.transform.childCount > 0)
        {
            for (int i = targetBackground.transform.childCount; i > 0; --i)
            {
                DestroyImmediate(targetBackground.transform.GetChild(0).gameObject);
            }
        }
        
        // Columns
        for (int x = -canvasSize; x < canvasSize; x++)
        {
            // Rows
            for (int y = -canvasSize; y < canvasSize; y++)
            {
                // Create a tile
                GameObject tile = GetRandomTile();
                tile.transform.parent = targetBackground.transform;
                currentPosition.x = x;
                currentPosition.y = y;
                tile.transform.position = currentPosition;
            }
        }
    }

    private GameObject GetRandomTile()
    {
        // Create a new GameObject with a SpriteRenderer component
        GameObject tile = new GameObject("Background Tile", typeof(SpriteRenderer));
        SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();

        // Base
        float randDiff = Mathf.Infinity;
        TileWithWeights currentTile = null;

        // Loop through Key Value sprite pairs
        for (int i = 0; i < tiles.Length; i++)
        {
            // Randomise a number
            float random = UnityEngine.Random.Range(0, 100f);
            // Get difference from tile's weight value
            float diff = random - tiles[i].weight;

            // If difference is smaller or same as previous difference value
            if (diff <= randDiff)
            {
                // If no difference, select the higher weighted sprite
                if (diff == randDiff && currentTile != null)
                {
                    if (currentTile.weight > tiles[i].weight) { continue; }
                }

                // Assign current tile iteration as the selected tile
                currentTile = tiles[i];
                // Update difference comparison reference
                randDiff = diff;
            }
        }

        // Change the GameObject's sprite to the randomised sprite
        sr.sprite = currentTile.sprite;

        return tile;
    }
}
