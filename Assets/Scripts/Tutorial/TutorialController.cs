using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    void Start()
    {
        FindObjectOfType<DialogDisplay>().StartDialog(15);
    }

}
