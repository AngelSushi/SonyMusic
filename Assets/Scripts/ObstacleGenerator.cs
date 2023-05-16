using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour {

    
    
    private IEnumerator SpawnObstacle() {
        yield return new WaitForSeconds(5f);
    }
}
