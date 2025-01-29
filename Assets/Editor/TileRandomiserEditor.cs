using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

[CustomEditor(typeof(TileRandomiser))]
public class TileRandomiserEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TileRandomiser tileRandomiser = (TileRandomiser)target;

        base.OnInspectorGUI();

        if (GUILayout.Button("Randomise Tiles"))
        {
            tileRandomiser.RandomiseTiles();
        }
    }
}
