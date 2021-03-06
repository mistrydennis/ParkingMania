﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class treeDetect : MonoBehaviour
{
    /// <summary>
    /// The associated agent.
    /// This will be set by the agent script on Initialization.
    /// Don't need to manually set.
    /// </summary>
    public CarAgent agent;  //

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("tree"))
        {
            agent.hitATree();
        }
    }
    /*
    private void OnTriggerEnter(Collider col)
    {
        // Touched goal.
        if (col.gameObject.CompareTag("wall"))
        {
            agent.hitAWall();
        }
    }*/
}
