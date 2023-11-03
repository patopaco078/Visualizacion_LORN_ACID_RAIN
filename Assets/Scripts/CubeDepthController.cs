using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeDepthController : MonoBehaviour
{
    [SerializeField] float speed;
    void Update()
    {
        transform.position += new Vector3(0,0, speed) * Time.deltaTime;
    }

    public void RestartPositionCube()
    {
        transform.position = new Vector3(0, 0, -5);
    }
}
