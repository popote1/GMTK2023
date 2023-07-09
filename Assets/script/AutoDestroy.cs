using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float Delay =2;
    void Start() {
        Destroy(gameObject, Delay);
    }

   
   
}
