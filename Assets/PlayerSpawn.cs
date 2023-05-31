using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public GameObject menPlayer;
    public GameObject womenPlayer;
    void Start()
    {
        if(GameManager.instance.isMen == true)
        {
            menPlayer = Instantiate(menPlayer, gameObject.transform.position, Quaternion.identity) as GameObject;
            menPlayer.transform.parent = transform;
        }
        if(GameManager.instance.isWomen == true)
        {
            womenPlayer = Instantiate(womenPlayer, gameObject.transform.position, Quaternion.identity) as GameObject;
            womenPlayer.transform.parent = transform;
        }
    }

    
}
