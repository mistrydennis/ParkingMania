﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class CarAgent : Agent
{
    /// <summary>
    /// List of spawning areas where the agent can spawn
    /// </summary>
    public List<GameObject> agentSpawnPositions;

    [HideInInspector]
    public Bounds areaBounds;


    /// <summary>
    /// The parking 
    /// </summary>
    public GameObject parking;
    /// <summary>
    /// reference to the Parking lot area
    /// </summary>
    public GameObject area;
    public float maxStep = 50000;

    [HideInInspector]
    public ParkingArea areaSettings;

    
    public bool useVectorObs;
    
    //reference to carSetting object
    CarSetting carSetting;

    Rigidbody m_CarRb;  //cached on initialization

    //list of wheels and meshes
    public List<WheelCollider> throttleWheels;
    public List<GameObject> steeringWheels;
    public List<GameObject> wheelMeshes;

    public float maxTurnDegree = 20f;
    

    //force applied to the driving wheels
    public float strengthCoefficient = 20000f;

    private GameObject ground;
    /// <summary>
    /// get the variables from external scripts on awake
    /// </summary>
    void Awake()
    {
        carSetting = FindObjectOfType<CarSetting>();
        areaSettings = area.GetComponent<ParkingArea>();
    }

    /// <summary>
    /// Initialize to get hte rigid body and a randomswpawn position
    /// </summary>
    public override void Initialize()
    {
        m_CarRb = GetComponent<Rigidbody>();

        GetRandomSpawnPos();
        //SetResetParameters();
    }
    /// <summary>
    /// Get a random Spawn position for the agent car depending upon the spawn area margin multiplier
    /// </summary>
    /// <returns> a vector3 position</returns>
    public Vector3 GetRandomSpawnPos()
    {
        ground = agentSpawnPositions[Mathf.FloorToInt(Random.Range(0, agentSpawnPositions.Count))];

        areaBounds = ground.GetComponent<Collider>().bounds;
        var foundNewSpawnLocation = false;
        var randomSpawnPos = Vector3.zero;
        while (foundNewSpawnLocation == false)
        {
            var randomPosX = Random.Range(-areaBounds.extents.x * carSetting.spawnAreaMarginMultiplier,
                areaBounds.extents.x * carSetting.spawnAreaMarginMultiplier);

            var randomPosZ = Random.Range(-areaBounds.extents.z * carSetting.spawnAreaMarginMultiplier,
                areaBounds.extents.z * carSetting.spawnAreaMarginMultiplier);
            randomSpawnPos = ground.transform.position + new Vector3(randomPosX, 1f, randomPosZ);
            if (Physics.CheckBox(randomSpawnPos, new Vector3(2.5f, 0.01f, 2.5f)) == false)
            {
                foundNewSpawnLocation = true;
            }
        }
        return randomSpawnPos;
    }

    /// <summary>
    /// Punishing the agent for hitting another parked car
    /// </summary>
    public void hitACar()
    {
        Debug.Log("Hit Car");
        // punish the agent
        AddReward(-0.1f);
        //EndEpisode();
    }
    /// <summary>
    /// Punish the agent for hitting a wall
    /// </summary>
    public void hitATree()
    {
        // punish the agent
        Debug.Log("Hit tree");
        AddReward(-0.1f);
    }

    public void hitAWall()
    {
        // punish the agent
        Debug.Log("Hit Wall");
        AddReward(-0.1f);
    }
    /// <summary>
    /// Reward the agent on parking properly
    /// </summary>
    public void parked()
    {
        // reward the agent
        Debug.Log("Parked");
        AddReward(5f);
        EndEpisode();
    }

    /// <summary>
    /// MoveAgent function for continuous actions where we get values in a float called act, the first value belongs to steering
    /// and the second value belongs to the throttle
    /// </summary>
    /// <param name="act"></param>
    public void MoveAgent(float[] act)
    {
        var steer = act[0];
        var throttle = act[1];

        foreach (WheelCollider wheel in throttleWheels)
        {
            wheel.motorTorque = strengthCoefficient * Time.fixedDeltaTime * throttle;
        }
        foreach (GameObject wheel in steeringWheels)
        {
            wheel.GetComponent<WheelCollider>().steerAngle = maxTurnDegree * steer;
            wheel.transform.localEulerAngles = new Vector3(0f, steer * maxTurnDegree, 0f);
        }
    }

    //Move agent for discrete actions
    /*
     public void MoveAgent(float[] act)
    {
        
        //act array of floats has forward/backward in position 0, turning in position 1, handbrake in position 2
        var throttle= act[0];
        var steer = act[1];
        //var handBrake = act[2];
        // when throttle is 1 move forward and when it is two move back
        if(throttle == 1f)
        {
            foreach (WheelCollider wheel in throttleWheels)
            {
                wheel.motorTorque = strengthCoefficient * Time.fixedDeltaTime * 1f;
            }
        }
        if(throttle == 2f)
        {
            foreach (WheelCollider wheel in throttleWheels)
            {
                wheel.motorTorque = strengthCoefficient * Time.fixedDeltaTime * -1;
            }
        }
        //when steer is one turn wheels left and when it is 2 turn right
        if(steer == 0f)
        {
            foreach (GameObject wheel in steeringWheels)
            {
                wheel.GetComponent<WheelCollider>().steerAngle = 0;
                wheel.transform.localEulerAngles = new Vector3(0f, 0f * maxTurnDegree, 0f);
            }
        }
        if(steer == 1f)
        {
            foreach (GameObject wheel in steeringWheels)
            {
                wheel.GetComponent<WheelCollider>().steerAngle = maxTurnDegree * 1;
                wheel.transform.localEulerAngles = new Vector3(0f, 1 * maxTurnDegree, 0f);
            }
        }
        if (steer == 2f)
        {
            foreach (GameObject wheel in steeringWheels)
            {
                wheel.GetComponent<WheelCollider>().steerAngle = maxTurnDegree * -1;
                wheel.transform.localEulerAngles = new Vector3(0f, -1 * maxTurnDegree, 0f);
            }
        }
        //when handbrake is 1 brake and when it is 2 release
        /*if(handBrake == 1f)
        {
            foreach (WheelCollider wheel in brakingWheels)
            {
                wheel.brakeTorque = brakingTorque * 1f;
            }
        }
        if(handBrake == 2f)
        {
            foreach (WheelCollider wheel in brakingWheels)
            {
                wheel.brakeTorque = brakingTorque * 0;
            }
        }
    }
*/
    private void FixedUpdate()
    {
        //visual turning of the wheels
        foreach (GameObject mesh in wheelMeshes)
        {
            mesh.transform.Rotate(m_CarRb.velocity.magnitude * (transform.InverseTransformDirection(m_CarRb.velocity).z >= 0 ? 1 : -1) / (2 * Mathf.PI * 0.17f), 0f, 0f);
        }
        
    }

    /// <summary>
    /// Called every step of the engine. Here the agent takes an action.
    /// </summary>
    public override void OnActionReceived(float[] vectorAction)
    {
        // Move the agent using the action.
        MoveAgent(vectorAction);
        // Penalty given each step to encourage agent to finish task quickly.
        AddReward(-1f / maxStep);
    }

    /// <summary>
    /// Heuristic function for the user to be able to drive the car himself
    /// </summary>
    /// <returns></returns>
    public float[] Heuristic()
    {
        var action = new float[2];
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        //return action;

        /*var action = new float[3];

        // left right
        if (Input.GetKeyDown(KeyCode.D))
        {
            action[1] =  1f ; 
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            action[1] = 0f;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            action[1] = 2f;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            action[1] = 0f;
        }
        //forward backward
        if (Input.GetKey(KeyCode.W))
        {
            action[0] = 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            action[0] = 2f;
        }*/
        // handbrake pressed or released
        /* if (Input.GetKeyDown(KeyCode.Space))
         {
             action[2] = 1f;
         }
         if (Input.GetKeyUp(KeyCode.Space))
         {
             action[2] = 2f;
         }*/
        return action;
    }
    
    /// <summary>
    /// In the editor, if "OnEpisodeBegin" is checked then AgentReset() will be
    /// called automatically anytime we mark done = true in an agent script.
    /// </summary>
    public override void OnEpisodeBegin()
    {
        var rotation = Random.Range(0, 4);
        var rotationAngle = rotation * 90f;
        area.transform.Rotate(new Vector3(0f, rotationAngle, 0f));

        areaSettings.resetArea();


        transform.position = GetRandomSpawnPos();
        transform.Rotate(new Vector3(0f, 0f, 0f));

        m_CarRb.velocity = Vector3.zero;
        
        m_CarRb.angularVelocity = Vector3.zero;

        //removing torque from the wheels
        foreach (WheelCollider wheel in throttleWheels)
        {
            wheel.motorTorque = 0f;
        }
        //removing steer angle from the wheels
        foreach (GameObject wheel in steeringWheels)
        {
            wheel.GetComponent<WheelCollider>().steerAngle  = 0f;
            wheel.transform.localEulerAngles = new Vector3(0f, 0f * maxTurnDegree, 0f);
        }
        //removing handbrake
        /*foreach (WheelCollider wheel in brakingWheels)
        {
            wheel.brakeTorque = brakingTorque * 0;
        }*/
    }

}
