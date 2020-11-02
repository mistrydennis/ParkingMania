using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wallDetect : MonoBehaviour
{
    public CarAgent agent;  

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("wall"))
        {
            agent.hitAWall();
        }
    }
}
