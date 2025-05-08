using System.Collections.Generic;
using System.Reflection;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.iOS;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BirdScript : Agent
{
    public Rigidbody2D myRigidbody;
    public float flapStrength;
    public bool birdIsAlive = true;

    //logic
    public int playerScore;
    public Text scoreText;
    public GameObject gameOverScreen;

    private List<GameObject> spawnedPipes = new List<GameObject>();

    public GameObject pipe;
    public float spanRate = 2;
    private float timer = -2f;
    public int heightOffset = 10;

    [HideInInspector] public int CurrentEpisode = 0;
    [HideInInspector] public float CumulativeReward = 0f;
    void Start()
    {
        spanPipe();
    }

    void Update()
    {
        AddReward(0.01f);
        if (timer < spanRate) timer += Time.deltaTime;
        else
        {
            spanPipe();
            timer = -2f;
        }
        if (Input.GetKeyUp(KeyCode.Space) && birdIsAlive)
        {
            myRigidbody.linearVelocity = Vector2.up * flapStrength;
        }

        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        bool isOffScreen = viewportPos.x < 0 || viewportPos.x > 1
                        || viewportPos.y < 0 || viewportPos.y > 1;
        
        MiddleScript.middlePart.RemoveAll(m => m == null || m.transform.position.x < (transform.position.x-3));
        if (isOffScreen)
        {
            gameOver();
            return;
        }
    }
    void spanPipe()
    {
        //return;
        float lowerOffset = 0 - heightOffset + 4;
        float highOffset = 0 + heightOffset;
        GameObject newPipe = Instantiate(pipe, new Vector3(0 + 40, Random.Range(lowerOffset, highOffset), 0), Quaternion.identity);
        spawnedPipes.Add(newPipe);
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = 0;
        if (Input.GetKeyUp(KeyCode.Space) && birdIsAlive)
        {
            discreteActionsOut[0] = 1;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (birdIsAlive)
        {
            gameOver();
        }
    }

    public override void OnEpisodeBegin()
    {
        //Debug.Log("Episode Started");
        CurrentEpisode++;
        CumulativeReward = 0f;
        transform.position = new Vector3(0, 0, 0);
        transform.rotation = new Quaternion(0, 0, 0, 0);       
        birdIsAlive = true; 
        myRigidbody.linearVelocity = Vector2.zero;
        myRigidbody.angularVelocity = 0f;
        clearAllPipe();
        MiddleScript.middlePart.Clear();
        playerScore = 0;
        scoreText.text = playerScore.ToString();
    }
    public void clearAllPipe()
    {
        foreach (GameObject p in spawnedPipes)
        {
            if (p != null)
            {
                Destroy(p);
            }
        }
        spawnedPipes.Clear();
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        float BirdPosX_normalized = transform.position.x / 5f;
        float BirdPosY_normalized = transform.position.y / 5f;
        sensor.AddObservation(BirdPosX_normalized);
        sensor.AddObservation(BirdPosY_normalized);
        float nearestPipeX = 10000000;
        float nearestPipeY = 10000000;
        float height = 0;
        foreach (var middle in MiddleScript.middlePart)
        {
            if (middle != null && middle.transform.position.x > transform.position.x)
            {
                if (nearestPipeX > middle.transform.position.x)
                {
                    height = middle.GetComponent<Collider2D>().bounds.size.y;
                    nearestPipeX = middle.transform.position.x;
                    nearestPipeY = middle.transform.position.y;
                }
            }
        }
        if (nearestPipeX != 10000000)
        {
            //Debug.Log($"nearestPipeX {nearestPipeX}");
            //Debug.Log($"nearestPipeY {(nearestPipeY - (height / 2))}");
            //Debug.Log($"nearestPipeY {(nearestPipeY + (height / 2))}");
            sensor.AddObservation(nearestPipeX / 5f);
            sensor.AddObservation((nearestPipeY - (height/2)) / 5f);
            sensor.AddObservation((nearestPipeY + (height/2)) / 5f);
        }
        else
        {
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
        }
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        MoveBird(actions.DiscreteActions);
        CumulativeReward = GetCumulativeReward();
    }
    public void MoveBird(ActionSegment<int> act)
    {
        var action = act[0];
        //Debug.Log($"getting action {action}");
        switch (action)
        {
            case 0:
                // eat 5 start do nothing
                break;
            case 1:
                myRigidbody.linearVelocity = Vector2.up * flapStrength;
                break;
        }
    }
    public void addScore(int score)
    {
        AddReward(1f);
        CumulativeReward = GetCumulativeReward();
        playerScore += score;
        scoreText.text = playerScore.ToString();
    }
    public void gameOver()
    {
        //Debug.Log("Game Over");
        AddReward(-1f);
        CumulativeReward = GetCumulativeReward();
        EndEpisode();
        //gameOverScreen.SetActive(true);
    }
    public void restartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
