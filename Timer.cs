using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    public GameObject StartTimer;
    public GameObject StopTimer;
    public GameObject[] Checkpoints;
    public Text StopWatch;
    public Text CheckpointTracker;
    int checkpoints;
    bool isTimerStarted = false;
    public bool isGameFinished = false;
    float currentTime;
    

    // Use this for initialization
    void Start () {
        currentTime = 0;
        checkpoints = 0;
        isTimerStarted = false;
        isGameFinished = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (isTimerStarted && !isGameFinished)
        {
            currentTime += Time.deltaTime;
            StopWatch.text = currentTime.ToString("0.00");
        }
	}

    public void Restart()
    {
        currentTime = 0;
        checkpoints = 0;
        isTimerStarted = false;
        isGameFinished = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        

        if (other.gameObject.name == StartTimer.name)
        {
            isTimerStarted = true;
            Debug.Log("Timer Started");
        }

        if (other.gameObject.name == StopTimer.name && isTimerStarted && checkpoints == 8)
        {
            isTimerStarted = false;
            isGameFinished = true;
            Debug.Log("Timer Stopped");
        }

        #region Checkpoints

            if (other.gameObject.name == Checkpoints[0].name && checkpoints == 0)
            {
                checkpoints++;
                updateCheckpoints();
                this.gameObject.GetComponent<DriverAgent>().Target = Checkpoints[1].transform;
                Debug.Log(Checkpoints[1].name);
            this.gameObject.GetComponent<DriverAgent>().AddReward(0.5f);
            }

            if (other.gameObject.name == Checkpoints[1].name && checkpoints == 1)
            {
                checkpoints++;
                updateCheckpoints();
                this.gameObject.GetComponent<DriverAgent>().Target = Checkpoints[2].transform;
                Debug.Log(Checkpoints[2].name);
            this.gameObject.GetComponent<DriverAgent>().AddReward(0.5f);
        }

            if (other.gameObject.name == Checkpoints[2].name && checkpoints == 2)
            {
                checkpoints++;
                updateCheckpoints();
                this.gameObject.GetComponent<DriverAgent>().Target = Checkpoints[3].transform;
                Debug.Log(Checkpoints[3].name);
            this.gameObject.GetComponent<DriverAgent>().AddReward(0.5f);
        }

            if (other.gameObject.name == Checkpoints[3].name && checkpoints == 3)
            {
                checkpoints++;
                updateCheckpoints();
                this.gameObject.GetComponent<DriverAgent>().Target = Checkpoints[4].transform;
                Debug.Log(Checkpoints[4].name);
            this.gameObject.GetComponent<DriverAgent>().AddReward(0.5f);
        }

            if (other.gameObject.name == Checkpoints[4].name && checkpoints == 4)
            {
                checkpoints++;
                updateCheckpoints();
                this.gameObject.GetComponent<DriverAgent>().Target = Checkpoints[5].transform;
                Debug.Log(Checkpoints[5].name);
            this.gameObject.GetComponent<DriverAgent>().AddReward(0.5f);
        }

            if (other.gameObject.name == Checkpoints[5].name && checkpoints == 5)
            {
                checkpoints++;
                updateCheckpoints();
                this.gameObject.GetComponent<DriverAgent>().Target = Checkpoints[6].transform;
                Debug.Log(Checkpoints[6].name);
            this.gameObject.GetComponent<DriverAgent>().AddReward(0.5f);
        }

            if (other.gameObject.name == Checkpoints[6].name && checkpoints == 6)
            {
                checkpoints++;
                updateCheckpoints();
                this.gameObject.GetComponent<DriverAgent>().Target = Checkpoints[7].transform;
                Debug.Log(Checkpoints[7].name);
            this.gameObject.GetComponent<DriverAgent>().AddReward(0.5f);
        }

            if (other.gameObject.name == Checkpoints[7].name && checkpoints == 7)
            {
                checkpoints++;
                updateCheckpoints();
                this.gameObject.GetComponent<DriverAgent>().Target = StopTimer.transform;
            this.gameObject.GetComponent<DriverAgent>().AddReward(0.5f);
        }

        #endregion

    }
    void updateCheckpoints()
    {
        CheckpointTracker.text = (checkpoints + "/8");
    }
}
