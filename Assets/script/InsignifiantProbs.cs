using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsignifiantProbs : MonoBehaviour
{
    
    public GameObject prefabsDebrie;
    public GameObject prefabsFX;
   
    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Zombi")) {
            Destroy();
        }
    }
    
    public void Destroy() {
        if (prefabsDebrie) Instantiate(prefabsDebrie, transform.position, transform.rotation);
        if (prefabsFX) Instantiate(prefabsFX, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
