using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{

    public EventHandler<OnReleaseObstacleArgs> OnReleaseObstacle; // Event when a obstacle dissapear

    public class OnReleaseObstacleArgs : EventArgs
    {
        public GameObject obstacle;
        public PlayerDash player;
        public bool isCut;
    }
}
