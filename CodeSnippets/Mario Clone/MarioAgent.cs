using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MarioAgent : Agent
{
    private GameObject mario;
    private Rigidbody2D rb;
    private movement move;

    public GameController controller;

    public GameObject flag;
    public Tilemap fg_tilemap;

    public bool useAdvancedJump = false;

    public bool hasEnemies = false;
    public List<GameObject> enemies = new List<GameObject>();

    
    private float episodeTimeLeft;

    public float numJumps;

    private float startingX = 0.0f;

    // Variables for adjusting reward function
    public float timeBetweenRewards = 1.0f;
    float prevJumps = 0.0f;
    float prevX = 0.0f;

    

    private void Start()
    {
        episodeTimeLeft = controller.episodeTimeLimit;

        CheckForEnemies();
        mario = gameObject;
        rb = mario.GetComponent<Rigidbody2D>();
        move = mario.GetComponent<movement>();

        Transform mvt = controller.marioVision.GetComponent<Transform>();
        mvt.localScale = new Vector3(controller.rangeBehind + controller.rangeAhead, controller.rangeAbove, 1f);

        StartCoroutine(AddRewards());
    }


    // Function called when mlagents EndEpisode() is called
    public override void OnEpisodeBegin()
    {

        episodeTimeLeft = controller.episodeTimeLimit;
        numJumps = 0f;
        // Strategy 1; Set mario back to the beggining of the level
        startingX = Random.Range(-15f, 0f);
        mario.GetComponent<Transform>().position = new Vector3(startingX, 0f, 0f);
        gameObject.GetComponent<movement>().isJumping = false;
        gameObject.GetComponent<movement>().jumpTime = 0.0f;

    }
    public float curReward = 0.0f;
    private void Update()
    {
        curReward = GetCumulativeReward();
  
        // Manual player control
        //if (Input.GetKey(KeyCode.A))
        //{
        //    move.SetHorzTarget(1);
        //}
        //else if (Input.GetKey(KeyCode.D))
        //{
        //    move.SetHorzTarget(2);
        //}
        //else
        //{
        //    move.SetHorzTarget(0);
        //}

        //if (Input.GetKey(KeyCode.Space))
        //{
        //    move.SetVertTarget(1, false);
        //}
        //else if (Input.GetKey(KeyCode.V))
        //{
        //    move.SetVertTarget(2, false);
        //}
        //else
        //{
        //    move.SetVertTarget(0, false);
        //}
    }

    // Initialize the hasEnemies bool. If there are no enemies on the level, this will prevent unecessary checks for enemies
    // If there are enemies, add them to the enemies list
    public void CheckForEnemies()
    {
        GameObject[] allGO = Object.FindObjectsOfType<GameObject>();

        foreach (GameObject go in allGO)
        {
            if(go.CompareTag("Enemy"))
            {
                hasEnemies = true;
                enemies.Add(go);
            }
        }

    }

    public Vector2 GetDistanceToNextEnemy()
    {
        Vector2 dis = new Vector2(999f, 999f);

        foreach (GameObject enemy in enemies)
        {
            Transform enemy_pos = enemy.GetComponent<Transform>();
            if (enemy_pos.position.magnitude < dis.magnitude)
            {
                dis.x = enemy_pos.position.x;
                dis.y = enemy_pos.position.y;
            }
        }
        
        return dis;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Flag"))
        {
            Debug.Log("Flag reached. Restarting Level. Final reward: " + GetCumulativeReward());
            AddReward(1f * controller.victoryFactor);
            EndEpisode();
        }
    }


    // Inifinite loop to continously add rewards
    private IEnumerator AddRewards()
    {
        prevJumps = numJumps;
        prevX = mario.GetComponent<Transform>().position.x;
        yield return new WaitForSeconds(timeBetweenRewards);
        AddReward(-timeBetweenRewards * controller.timeFactor); // penalalty based on time elapsed
        AddReward((numJumps - prevJumps) * controller.jumpingFactor);
        AddReward((mario.GetComponent<Transform>().position.x - prevX) * controller.disRightFactor); // reward for moving right (+) and penalty for moving left (-)

        StartCoroutine(AddRewards());
    }

    
    // Action space of the agent
    public override void OnActionReceived(ActionBuffers actions)
    {
        episodeTimeLeft -= Time.fixedDeltaTime;

        if (episodeTimeLeft < 0.0f)
        {
            Debug.Log("Time limit reached. Ending episode with " + GetCumulativeReward());
            EndEpisode();
        }
        Debug.Log("X: " + actions.DiscreteActions[0]);
        Debug.Log("Y: " + actions.DiscreteActions[1]);
        move.SetHorzTarget(actions.DiscreteActions[0]);
        move.SetVertTarget(actions.DiscreteActions[1], useAdvancedJump);
    }

    // Observations that AI needs in order to solve task - roughly the entire visible screen
    public override void CollectObservations(VectorSensor sensor)
    {
        // Total observations = 202

        // Relative player y position - (y)
        sensor.AddObservation(mario.GetComponent<Transform>().position.y);

        // Current velocity - (x_vel, y_vel)
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.y);

        // Distance to flag - (dis)
        float flag_dis = flag.GetComponent<Transform>().position.x - mario.GetComponent<Transform>().position.x;
        sensor.AddObservation(flag_dis);

        // Distance to next enemy (if any) - (xdis, ydis)
        if(hasEnemies == false)
        {
            sensor.AddObservation(999f);
            sensor.AddObservation(999f);
        }
        else
        {
            sensor.AddObservation(GetDistanceToNextEnemy());
        }

        // (121 ints, where 0=empty, 1=floor, 2=brick, 3=broken_brick, 4=questionbox, 5=death, 6=flag, 7=unknown

        Vector3Int curCellPos = fg_tilemap.WorldToCell(mario.GetComponent<Transform>().position);

        int count = 0;
        
        for (int x = curCellPos.x - controller.rangeBehind; x < curCellPos.x + controller.rangeAhead; x++)
        {
            for(int y = controller.marioVisionGroundLevel ; y < controller.marioVisionGroundLevel + controller.rangeAbove; y++)
            {
                count++;
                Vector3Int tilePos = new Vector3Int(x, y, 0);

                TileBase t = fg_tilemap.GetTile(tilePos);

                if(t == null) { sensor.AddObservation(0); }
                else if (t.name == "ground_1") { sensor.AddObservation(1); }
                else if (t.name == "brick_1") { sensor.AddObservation(2); }
                else if (t.name == "broken_brick") { sensor.AddObservation(3); }
                else if (t.name == "questionblock") { sensor.AddObservation(4); }
                else if (t.name == "death") { sensor.AddObservation(5); }
                else if (t.name == "pole" || t.name == "pole_bot" || t.name == "pole_top" || t.name == "flag") { sensor.AddObservation(6); }
                else { Debug.Log("UNKNOWN BLOCK"); sensor.AddObservation(7); }
            }
        }
    }
}
