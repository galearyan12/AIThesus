using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MLAgents;
using UnityStandardAssets.Vehicles.Car;

public class newCarAgent : Agent
{
    private float movingtime = 0;
    private float LapTime = 0; //measure laptime
    private float bestLapTime = 60f; //holds record time
    private bool Collided = false; //true if collision detected
    public bool StartLineFlag = false; //true if kart passes start line

    public float action_steer = 0; //steering value
    public float action_throttle = 0; //throttle value
    
    public Transform[] Waypoints = new Transform[9];
    public Text txtLapTime, txtLastLap, txtBestLap, checkpoint;
    int currentCheckpoint;
    float distanceLeft, distanceLeft2, distanceLeft3, distanceRight, distanceRight2, distanceRight3;

    public GameObject SensorLeft, SensorLeft2, SensorLeft3 ;
    public GameObject SensorRight, SensorRight2, SensorRight3;

    public bool checkpointscheck = false;
    bool checkpoint1 = false;
    bool checkpoint2 = false;
    bool checkpoint3 = false;
    bool checkpoint4 = false;
    bool checkpoint5 = false;
    bool checkpoint6 = false;
    bool checkpoint7 = false;
    bool checkpoint8 = false;

    private CarController m_Car;
    //private LapController lapcontroller;
    Vector3 nomovement = new Vector3(0f, 0f, 0f);

    void Start()
    {
        m_Car = GetComponent<CarController>();
       // lapcontroller = GetComponent<LapController>();
        currentCheckpoint = 0;
    }

    private float PointToSegment(Vector3 A, Vector3 B, Vector3 P)
    {
        //calculates distance between point P and segment AB
        float t;
        Vector3 n = (B - A);
        Vector3 m = (P - A);

        float sign = Mathf.Sign(Vector3.SignedAngle(m, n, Vector3.up));

        float l = n.magnitude;
        //check if segment is a point
        if (l < 0.001)
        {
            return (P - A).magnitude * sign;
        }

        t = Vector3.Dot(m, n) / (l * l);

        if (t < 0)
        {
            return (P - A).magnitude * sign;
        }
        else if (t > 1)
        {
            return (P - B).magnitude * sign;
        }
        else
        { //t between 0..1
            Vector3 Q = A + t * n;
            return (P - Q).magnitude * sign;
        }

    }

    public override void AgentReset()
    {
        checkpoint.text = "0/8";
        LapTime = 0;
            transform.SetPositionAndRotation(GameObject.Find("StartingPosition").transform.position, new Quaternion(0, 0, 0, 0));
        transform.LookAt(Waypoints[1].position);


        //reset kart speed to zero
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        Collided = false;
        action_steer = 0;
        action_throttle = 0;
        currentCheckpoint = 0;
        checkpoint1 = false;
        checkpoint2 = false;
        checkpoint3 = false;
        checkpoint4 = false;
        checkpoint5 = false;
        checkpoint6 = false;
        checkpoint7 = false;
        checkpoint8 = false;
    }

    public override void CollectObservations()
    {
        AddVectorObs(distanceLeft);
        AddVectorObs(distanceLeft2);
        AddVectorObs(distanceLeft3);
        AddVectorObs(distanceRight);
        AddVectorObs(distanceRight2);
        AddVectorObs(distanceRight3);
        //AddVectorObs(GetComponent<Rigidbody>().angularVelocity.y);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        //decode actions
        action_steer = Mathf.Clamp(vectorAction[0], -1f, 1f);
        action_throttle = 1;

        //actions are executed in CarUserControl script
        m_Car.Move(action_steer, action_throttle, action_throttle, 0);

        

        //collect reward
        if (Collided)
        { //car hit the walls
            AddReward(-1.0f);
            Done();
        }
        else if(LapTime < bestLapTime)
        { //still alive
            AddReward(0.05f);
        }
    }

    private void Update()
    {
        raycastLeft();
        raycastRight();
        raycastLeft2();
        raycastRight2();
        raycastLeft3();
        raycastRight3();
        //update lap time on UI
        txtLapTime.text = "Lap Time : " + LapTime.ToString("f1") + " s";

        //turn steering wheel and front wheels
        float angle = Mathf.Clamp(action_steer, -1, 1) * 90f;
        // Steer.localEulerAngles = new Vector3(20.325f, 0f, 108.235f - angle);
        //WheelFL.localEulerAngles = new Vector3(0f, -90f + angle / 3, 0f);
        //WheelFR.localEulerAngles = new Vector3(0f, -90f + angle / 3, 0f);

        //Debug.Log(GetComponent<Rigidbody>().velocity);
        //Debug.Log(movingtime.ToString("0.0"));

        
        if ((Mathf.Round(GetComponent<Rigidbody>().velocity.x)) == (Mathf.Round(nomovement.x)) && (Mathf.Round(GetComponent<Rigidbody>().velocity.y)) == (Mathf.Round(nomovement.y)) && (Mathf.Round(GetComponent<Rigidbody>().velocity.z)) == (Mathf.Round(nomovement.z)))
        {
            if(movingtime > 5)
            {
                movingtime = 0;
                Debug.Log("Stuck");
                AddReward(-1.0f);
                Done();
            }
            
        }
        else if (GetComponent<Rigidbody>().velocity != nomovement)
        {
            //Debug.Log("Is Moving");
            movingtime = 0;
        }

    }

    void FixedUpdate()
    {
        //update lap time
        LapTime += Time.fixedDeltaTime;
        movingtime += Time.fixedDeltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("checkpoints"))
        {
            if (checkpointscheck == false)
            {
                Debug.Log("Reached Checkpoint " + other.gameObject.name);
                currentCheckpoint++;
                checkpoint.text = currentCheckpoint.ToString() + "/8";
                checkpointscheck = true;
            }

        }

        //reset lap time when crossing start line
        if (other.CompareTag("StartLine"))
        {
            if (!StartLineFlag)
            {
                Debug.Log("Finished Lap");

                txtLastLap.text = "Last Lap : " + LapTime.ToString("f1") + " s";
                if (LapTime < bestLapTime)
                {
                    bestLapTime = LapTime;
                    txtBestLap.text = "Best Lap : " + bestLapTime.ToString("f1") + " s";
                    AddReward(1f);
                }
                AddReward(1f);
                LapTime = 0;
                StartLineFlag = true;
                Done();
            }
        }
        else if(other.gameObject.name == "Checkpoint1" && !checkpoint1)
        {
            AddReward(0.5f);
            checkpoint1 = true;
            //Debug.Log("Reward 1 Added");
        }
        else if (other.gameObject.name == "Checkpoint2" && !checkpoint2)
        {
            AddReward(0.5f);
            checkpoint2 = true;
           // Debug.Log("Reward 2 Added");
        }
        else if (other.gameObject.name == "Checkpoint3" && !checkpoint3)
        {
            AddReward(0.5f);
            checkpoint3 = true;
           // Debug.Log("Reward 3 Added");
        }
        else if (other.gameObject.name == "Checkpoint4" && !checkpoint4)
        {
            AddReward(0.5f);
            checkpoint4 = true;
            //Debug.Log("Reward 4 Added");
        }
        else if (other.gameObject.name == "Checkpoint5" && !checkpoint5)
        {
            AddReward(0.5f);
            checkpoint5 = true;
            //Debug.Log("Reward 5 Added");
        }
        else if (other.gameObject.name == "Checkpoint6" && !checkpoint6)
        {
            AddReward(0.5f);
            checkpoint6 = true;
            //Debug.Log("Reward 6 Added");
        }
        else if (other.gameObject.name == "Checkpoint7" && !checkpoint7)
        {
            AddReward(0.5f);
            checkpoint7 = true;
           // Debug.Log("Reward 7 Added");
        }
        else if (other.gameObject.name == "Checkpoint8" && !checkpoint8)
        {
            AddReward(0.5f);
            checkpoint8 = true;
            //Debug.Log("Reward 8 Added");
        }
        else if (other.CompareTag("sidewall"))//we hit a wall -> collision detected -> end of episode
        {
            Collided = true;
            Debug.Log("Crashed");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("StartLine"))
            StartLineFlag = false;

        if (other.CompareTag("checkpoints"))
        {
            checkpointscheck = false;
        }
    }

    public void raycastLeft()
    {
        RaycastHit hit;
        if(Physics.Raycast(SensorLeft.transform.position,SensorLeft.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            Debug.DrawRay(SensorLeft.transform.position, SensorLeft.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            if (hit.transform.tag == "sidewall")
            {
               // Debug.Log(hit.transform.tag);
                distanceLeft = hit.distance;
            }
            
        }
    }

    public void raycastRight()
    {
        RaycastHit hit;
        if (Physics.Raycast(SensorRight.transform.position, SensorRight.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            Debug.DrawRay(SensorRight.transform.position, SensorRight.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            if (hit.transform.tag == "sidewall")
            {
                //Debug.Log(hit.transform.tag);
                distanceRight = hit.distance;
            }
        }
    }

    public void raycastLeft2()
    {
        RaycastHit hit;
        if (Physics.Raycast(SensorLeft2.transform.position, SensorLeft2.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            Debug.DrawRay(SensorLeft2.transform.position, SensorLeft2.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            if (hit.transform.tag == "sidewall")
            {
                //Debug.Log(hit.transform.tag);
                distanceLeft = hit.distance;
            }

        }
    }

    public void raycastRight2()
    {
        RaycastHit hit;
        if (Physics.Raycast(SensorRight2.transform.position, SensorRight2.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            Debug.DrawRay(SensorRight2.transform.position, SensorRight2.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            if (hit.transform.tag == "sidewall")
            {
                //Debug.Log(hit.transform.tag);
                distanceRight = hit.distance;
            }
        }
    }

    public void raycastLeft3()
    {
        RaycastHit hit;
        if (Physics.Raycast(SensorLeft3.transform.position, SensorLeft3.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            Debug.DrawRay(SensorLeft3.transform.position, SensorLeft3.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            if (hit.transform.tag == "sidewall")
            {
                //Debug.Log(hit.transform.tag);
                distanceLeft = hit.distance;
            }

        }
    }

    public void raycastRight3()
    {
        RaycastHit hit;
        if (Physics.Raycast(SensorRight3.transform.position, SensorRight3.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            Debug.DrawRay(SensorRight3.transform.position, SensorRight3.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            if (hit.transform.tag == "sidewall")
            {
                //Debug.Log(hit.transform.tag);
                distanceRight = hit.distance;
            }
        }
    }

}
