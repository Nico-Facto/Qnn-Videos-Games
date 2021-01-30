using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

public class GridMultiPrism : Agent
{
    /// <summary>
    /// Commented lines open agent to a third branch, setup for an action or not (2) 
    /// he is ready to shoot with countdown or not horever i dont use it for Mr.Navigator
    /// And dont really work the shoot himself
    /// </summary>

    [Header("Env Features")]
    [SerializeField] AreaPrisme m_MyArea = null;
    Rigidbody m_AgentRb;
    private Transform targetpos;
    //EnvironmentParameters m_ResetParams;
    StatsRecorder m_stats;

    [Header("Agent specs")]
    [Range(1,3f)]
    public float moveSpeed = 2; // Speed of agent movement.
    [Range(150, 400f)]
    public float turnSpeed = 300; // Speed of agent rotation.
    public static bool isEpisodeStart = false; // A state set to True at each onepisodeBegin Allow to manage LetterManagers for example, shoud not be used Here


    /*[SerializeField] GameObject MainEffect = null;
    [SerializeField] Transform firePoint = null;
    private float Effect_DestroyTime = 10;
    const float timeOutSpells = 2f;
    private float countdownSpells = timeOutSpells;
    private bool HaveSendSpells = false;*/


    [Header("Agent Commutator")]
    [HideInInspector] public int stepReach = 0; // give us possibility to act on Env in relation of step reached by agent (+ for inference env).
    private Vector3 forward;
    private Vector3 toOther;
    private float angleToTarget;
    private bool isFirstStep = true;
    private bool secure_step = true;


    [Header("Agent Tag's reachable")]
    [Tooltip("Those tag must match with the grid setup and your checkpoints gameObjects")]
    [SerializeField] string[] stepTagName = {"symbol_O", "symbol_X", "symbol_O_Goal", "symbol_X_Goal"};
    [Tooltip("Those tag must match with the grid setup and your constraints gameObjects")]
    [SerializeField] string[] constraintTagName = { "wall", "block"};

    public override void Initialize()
    {
        m_AgentRb = GetComponent<Rigidbody>();
        m_stats = Academy.Instance.StatsRecorder;
        //m_ResetParams = Academy.Instance.EnvironmentParameters;
    }

    public Color32 ToColor(int hexVal)
    {
        var r = (byte)((hexVal >> 16) & 0xFF);
        var g = (byte)((hexVal >> 8) & 0xFF);
        var b = (byte)(hexVal & 0xFF);
        return new Color32(r, g, b, 255);
    }

    private void Update()
    {
        //CheckShoot();
        LookTarget();
        if (stepReach == 0)
        {
            GameObject temp = GameObject.FindGameObjectWithTag(stepTagName[0]);
            targetpos = temp.transform;
        }
        if (stepReach == 4)
        {
            AddReward(1 - (StepCount / MaxStep));
            EndEpisode();
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        var localVelocity = transform.InverseTransformDirection(m_AgentRb.velocity);
        sensor.AddObservation(localVelocity.x); //1
        sensor.AddObservation(localVelocity.z); //1
        sensor.AddObservation(stepReach); //1
        sensor.AddObservation(transform.position.normalized); //3
        sensor.AddObservation(forward); //3
        if (targetpos == null)
        {
            sensor.AddObservation(new float[4]);
            return;
        }
        sensor.AddObservation(targetpos.position.normalized); //3
        sensor.AddObservation(angleToTarget); //1
    }

    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var forwardAxis = (int)act[0];
        var rotateAxis = (int)act[1];
        //var actionButton = (int)act[2];

        switch (forwardAxis)
        {
            case 1:
                dirToGo = transform.forward;
                break;
            case 2:
                dirToGo = -transform.forward;
                break;
        }
        switch (rotateAxis)
        {
            case 1:
                rotateDir = -transform.up;
                break;
            case 2:
                rotateDir = transform.up;
                break;
        }
        /*switch (actionButton)
        {
            case 1 :
                Action();
                break;
        }*/
        m_AgentRb.AddForce(dirToGo * moveSpeed, ForceMode.VelocityChange);
        transform.Rotate(rotateDir, Time.fixedDeltaTime * turnSpeed);
        

        if (m_AgentRb.velocity.sqrMagnitude > 25f) // slow it down
        {
            m_AgentRb.velocity *= 0.95f;
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)

    {
        MoveAgent(actionBuffers.DiscreteActions);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = 0;
        discreteActionsOut[1] = 0;
        //discreteActionsOut[2] = 0;
        if (Input.GetKey(KeyCode.Z))
        {
            discreteActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }

        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[1] = 2;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            discreteActionsOut[1] = 1;
        }

        /*if (Input.GetKey(KeyCode.Space))
        {
            discreteActionsOut[2] = 1;
        }*/
    }

    public override void OnEpisodeBegin()
    {
        isEpisodeStart = true;
        if (m_MyArea.isTrainingMl)
        {
            if (!isFirstStep)
            {
                m_MyArea.cleanAllCollectible();
            }
            else
            {
                isFirstStep = false;
            }
            m_MyArea.SpawmAllPrism();
            m_MyArea.SpawmBadPrisme();
            transform.position = new Vector3(Random.Range(-m_MyArea.rangeX, m_MyArea.rangeX), 1f, Random.Range(-m_MyArea.rangeZ, m_MyArea.rangeZ)) + m_MyArea.transform.position;
            transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
        }
        else
        {
            foreach (GameObject item in m_MyArea.goTemp)
            {
                item.SetActive(true);
            }
        }
        stepReach = 0;
        GameObject temp = GameObject.FindGameObjectWithTag(stepTagName[0]);
        targetpos = temp.transform;
    }


    private void OnCollisionEnter(Collision other)
    {
        
        if (other.gameObject.CompareTag(constraintTagName[0]) || other.gameObject.CompareTag(constraintTagName[1])) 
        {
            AddReward(-1f);
            EndEpisode();
        }

        // i start coroutine a the end to return to true after little delay 0.5f to prevent double collision bug, 
        // cause of unity delay of destroy(go) and kick of biais of data during training 
        if (!secure_step) { return; }

        secure_step = false;

        if (other.gameObject.CompareTag(stepTagName[0]))
        {
            CheckSteps(other, 0);
        }

        if (other.gameObject.CompareTag(stepTagName[1]))
        {
            CheckSteps(other, 1);
        }

        if (other.gameObject.CompareTag(stepTagName[2]))
        {
            CheckSteps(other, 2);
        }

        if (other.gameObject.CompareTag(stepTagName[3]))
        {
            CheckSteps(other, 3, true);
        }
        StartCoroutine(SecureStepValue());
    }

    void CheckSteps(Collision other, int thisStep, bool islastStep = false)
    {
        if (!islastStep)
        {
            if (stepReach == thisStep)
            {
                other.collider.enabled = false;
                AddReward(thisStep+1);
                AddReward(1 - (StepCount / MaxStep));
                stepReach = thisStep+1;

                if (m_MyArea.isTrainingMl)
                {
                    Debug.Log($"Step {thisStep+1} ");
                    Destroy(other.gameObject);
                }
                else
                {
                    m_MyArea.goTemp[thisStep].SetActive(false);
                }

                GameObject temp = GameObject.FindGameObjectWithTag(stepTagName[thisStep+1]);
                targetpos = temp.transform;
            }
            else
            {
                EndEpisode();
            }
        }
        else
        {
            if (stepReach == thisStep)
            {
                AddReward(thisStep + 1);
                if (m_MyArea.isTrainingMl)
                {
                    Debug.Log("Step Final ");
                    Destroy(other.gameObject);
                }
                else
                {
                    m_MyArea.goTemp[thisStep].SetActive(false);
                }
                stepReach = thisStep + 1;
            }
            else
            {
                EndEpisode();
            }
        }
    } 
    
    IEnumerator SecureStepValue() 
    {
        yield return new WaitForSeconds(0.5f);
        secure_step = true;
    }

    void LookTarget()
    {
        forward = transform.forward;
        if(targetpos == null)
        {
            return;
        }
        toOther = (targetpos.transform.position - transform.position).normalized;
        angleToTarget = Vector3.Dot(forward, toOther);
        if (angleToTarget > 0.95f)
        {
            AddReward(0.001f);
            if (m_MyArea.isTrainingMl)
            {
                Debug.Log("Facing Target");
                m_stats.Add("Facing", 0.001f);
            }
        }
    }

    /*void CheckShoot()
    {
        //cooldown for Spell
        if (!HaveSendSpells)
        {
            countdownSpells -= Time.deltaTime;
            if (countdownSpells <= 0)
            {
                countdownSpells = timeOutSpells;
                HaveSendSpells = true;
            }
        }
    }

    void Action()
    {
        if (!HaveSendSpells)
        {
            return;
        }
        HaveSendSpells = false;
        if (MainEffect == null) return;
        GameObject instance;
        instance = Instantiate(MainEffect, firePoint.transform.position, firePoint.transform.rotation);
        if (Effect_DestroyTime > 0.01f) Destroy(instance, Effect_DestroyTime);  /// !!!
    }*/
}

