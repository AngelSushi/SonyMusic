using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour {

    public float timeBetweenPlateform;
    public GameObject plateformPrefab;
    public int playerSize;

    private void Start() {
        StartCoroutine(GeneratePlateform());
    }

    private IEnumerator GeneratePlateform() {

        Vector3 zero = Camera.main.ViewportToWorldPoint(new Vector3(0,1,-Camera.main.transform.position.z));
        zero.x = 0;
        
        // new Vector3(0, -Screen.height / 2, -Camera.main.transform.position.z)
        GameObject plateform = Instantiate(plateformPrefab, zero, Quaternion.identity);
        
        GameObject firstPart = plateform.transform.GetChild(0).gameObject;
        GameObject secondPart = plateform.transform.GetChild(1).gameObject;
        
        double width = Camera.main.orthographicSize * 2.0 * Screen.width / Screen.height;

        float randomX = Random.Range(0, (float)width - playerSize);

        float secondRandomX = (float) width - (randomX + playerSize);
        
        firstPart.transform.localScale = new Vector3(randomX,0.5f,0);
        secondPart.transform.localScale = new Vector3(-secondRandomX, 0.5f, 0);

        yield return new WaitForSeconds(timeBetweenPlateform);
        
        StartCoroutine(GeneratePlateform());
    }
}
