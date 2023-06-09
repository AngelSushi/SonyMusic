using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObject : MonoBehaviour
{
    public DashDirection dashDirection;
    private bool _isCut;

    public bool IsCut
    {
        get => _isCut;
        set => _isCut = value;
    }
}
