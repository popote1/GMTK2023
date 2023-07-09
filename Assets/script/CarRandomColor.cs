using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarRandomColor : MonoBehaviour
{
    public MeshRenderer MeshRenderer;
    
    void Start()
    {
        if (MeshRenderer) MeshRenderer.material.color = new Color(Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f),1);
    }
}
