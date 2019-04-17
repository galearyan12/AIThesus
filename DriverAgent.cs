using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using UnityStandardAssets.Vehicles.Car;

public class DriverAgent : Agent {
    [SerializeField] LayerMask SensorMask; // Defines the layer of the walls ("Wall")
    Rigidbody rBody;
    float currentTime;
    private CarController m_Car; // the car controller we want to use
    double distanceRight;
    double distanceLeft;
    double distanceFront;
    double distanceBack;
    GameObject LineBlock;
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        m_Car = GetComponent<CarController>();
        currentTime = 0;
        LineBlock = GameObject.Find("LineBlock");
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        if(currentTime > 30)
        {
            AddReward(-0.1f);
            Done();
            Debug.Log("Out of Time");
        }

        if(this.gameObject.transform.position.y > 3)
        {
            AddReward(-0.01f);
            Done();
            Debug.Log("Above track height");
        }

        castRayRight();
        castRayLeft();
        castRayForward();
        castRayBack();
        //Debug.Log("Distance right : " + distanceRight.ToString());
    }
    public Transform Target;
    public override void AgentReset()
    {

            // If the Agent fell, zero its momentum
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.position = new Vector3(63.44f, 2.52f, -145.75f);
            this.transform.rotation = new Quaternion(0, 80.93f, 0, 90f);
            this.gameObject.GetComponent<Timer>().Restart();
            Target = GameObject.Find("Checkpoint1").transform;
            currentTime = 0;
    }

    public override void CollectObservations()
    {
        // Target and Agent positions
        //AddVectorObs(Target.position);
        //AddVectorObs(this.transform.position);

        // Agent velocity
        AddVectorObs((float)distanceRight);
        AddVectorObs((float)distanceLeft);
        AddVectorObs((float)distanceFront);
        AddVectorObs((float)distanceBack);
    }

    //public float speed = 10;
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        
        float h = vectorAction[0];
        float v = vectorAction[1];
        
        m_Car.Move(h, v, v, 0);

        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.position,
                                                  Target.position);

        // Reached target
        if (distanceToTarget < 0.01f)
        {
            AddReward(0.5f);
            currentTime = 0;
        }

        // Fell off platform
        if (this.transform.position.y < 0)
        {
            Done();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "wall")
        {
            Debug.Log("Left Wall");
            AddReward(-0.01f);
            Done();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "sidewall")
        {
            Debug.Log("Hit Wall");
            AddReward(-0.01f);
            Done();
        }

        if (other.gameObject.tag == "wall")
        {
            Debug.Log("Left Track");
            AddReward(-0.01f);
            Done();
        }

        if (other.gameObject.name == GameObject.Find("TimerStop").name && this.gameObject.GetComponent<Timer>().isGameFinished)
        {
            Done();
        }
    }

    void castRayRight()
    {
        RaycastHit hit;
        Ray rightRay = new Ray(LineBlock.transform.position, LineBlock.transform.right);

        Debug.DrawRay(LineBlock.transform.position, LineBlock.transform.right, Color.green);

        if (Physics.Raycast(rightRay, out hit))
        {
            distanceRight = hit.distance;
            Debug.Log(hit.transform.name);
        }
    }

    void castRayLeft()
    {
        RaycastHit hit;
        Ray leftRay = new Ray(LineBlock.transform.position, -LineBlock.transform.right);

        Debug.DrawRay(LineBlock.transform.position, -LineBlock.transform.right, Color.blue);

        if (Physics.Raycast(leftRay, out hit))
        {
            distanceLeft = hit.distance;
            Debug.Log(hit.transform.name);
        }
    }

    void castRayForward()
    {
        RaycastHit hit;
        Ray forwardRay = new Ray(LineBlock.transform.position, LineBlock.transform.forward);

        Debug.DrawRay(LineBlock.transform.position, LineBlock.transform.forward, Color.green);

        if (Physics.Raycast(forwardRay, out hit))
        {
            distanceFront = hit.distance;
            Debug.Log(hit.transform.name);
        }
    }

    void castRayBack()
    {
        RaycastHit hit;
        Ray backRay = new Ray(LineBlock.transform.position, -LineBlock.transform.forward);

        Debug.DrawRay(LineBlock.transform.position, -LineBlock.transform.forward, Color.blue);

        if (Physics.Raycast(backRay, out hit))
        {
            distanceBack = hit.distance;
            Debug.Log(hit.transform.name);
        }
    }
}