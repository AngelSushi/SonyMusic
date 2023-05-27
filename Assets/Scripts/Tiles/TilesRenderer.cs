using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesRenderer : MonoBehaviour
{
    private List<Tiles> _allTiles;

    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;


    [ContextMenu("Render Tiles")]
    private void RenderTiles() {
        for (int i = 0; i < gridHeight; i++) {
            
        }
    }
}
