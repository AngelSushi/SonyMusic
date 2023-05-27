using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DashDirection : MonoBehaviour
{
    
    public Texture2D dashDirectionRef;
    private int[,] dashDirection;
    private void OnEnable()
    {
        if (dashDirectionRef != null)
        {
            dashDirection = new int[dashDirectionRef.height,dashDirectionRef.width];
            UpdateValues();
        }
    }
    
    [ContextMenu("Update Values")]
    public void UpdateValues()
    {
        for (int i = 0; i < dashDirectionRef.height; i++)
        {
            for (int j = 0; j < dashDirectionRef.width; j++)
            {
                Color color = dashDirectionRef.GetPixel(i, j);

                dashDirection[i, j] = color == Color.white ? 0 : 1;
                Debug.Log("color " + dashDirection[i, j]);
                
            }
        }
    }
}
