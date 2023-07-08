using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameratargetController : MonoBehaviour
{
    public float CameraScrollmoveSpeed;

    public Vector2 MaxPosition = new Vector2(10,10);
    public Vector2 MinPosition = new Vector2(0,0);
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float vetical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");
        Vector3 MoveVector = new Vector3(horizontal, 0, vetical)*CameraScrollmoveSpeed* Time.deltaTime;
        Vector3 newPos = transform.position + MoveVector;
        
        transform.position = new Vector3(Mathf.Clamp(newPos.x, MinPosition.x, MaxPosition.x), 0f
            ,Mathf.Clamp(newPos.z,MinPosition.y, MaxPosition.y ));
    }
}
