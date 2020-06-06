using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Enemy_FSM : MonoBehaviour
{
    public enum Enemy_State { PATROL, CHASE, ATTACK };

    [SerializeField]
    private Enemy_State currentState;

    public Enemy_State CurrentState
    {
        get { return currentState; }
        set 
        { 
            currentState = value;

            StopAllCoroutines();

            switch (currentState)
            {
                case Enemy_State.PATROL:
                    StartCoroutine(EnemyPatrol());
                    break;
                case Enemy_State.CHASE:
                    StartCoroutine(EnemyChase());
                    break;
                case Enemy_State.ATTACK:
                    StartCoroutine(EnemyAttack());
                    break;
            }
        }
    }

    private void StartCoroutine(IEnumerable enumerable)
    {
        throw new NotImplementedException();
    }

    private CheckMyVision checkMyVision;

    private NavMeshAgent agent = null;

    private Transform playerTransform = null;

    private Transform patrolDestiantion = null;

    private Health playerHealth = null;

    public float maxDamage = 10f;

    private void Awake()
    {
        checkMyVision = GetComponent<CheckMyVision>();
        agent = GetComponent<NavMeshAgent>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        // Do something about player tansform to
        playerTransform = playerHealth.GetComponent<Transform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] destination = GameObject.FindGameObjectsWithTag("Dest");
        patrolDestiantion = destination[Random.Range(0, destination.Length)].GetComponent<Transform>();
        
        currentState = Enemy_State.PATROL;
    }

    public IEnumerable EnemyPatrol()
    {
        while (currentState == Enemy_State.PATROL)
        {
            checkMyVision.sensitivity = CheckMyVision.Sensitivity.HIGH;
            agent.isStopped = false;
            agent.SetDestination(patrolDestiantion.position);

            while (agent.pathPending)
                yield return null;

            if (checkMyVision.TargetInSight)
            {
                agent.isStopped = true;
                currentState = Enemy_State.CHASE;
                yield break;
            }
            yield break;
        }
        
    }
    
    public IEnumerable EnemyChase()
    {
        while(currentState == Enemy_State.CHASE)
        {
            checkMyVision.sensitivity = CheckMyVision.Sensitivity.LOW;
            agent.isStopped = false;
            agent.SetDestination(checkMyVision.lastKnownSighting);
            while (agent.pathPending)
                yield return null;
            if(agent.remainingDistance <= agent.stoppingDistance)
            {
                agent.isStopped = true;

                if (!checkMyVision.TargetInSight)
                    currentState = Enemy_State.PATROL;
                else
                    currentState = Enemy_State.ATTACK;
                yield break;
            }
            yield return null;
        }
    }
    
    public IEnumerable EnemyAttack()
    {
        while(currentState == Enemy_State.ATTACK)
        {
            agent.isStopped = false;
            agent.SetDestination(playerTransform.position);
            while(agent.pathPending)
                yield return null;

            if(agent.remainingDistance > agent.stoppingDistance)
            {
                currentState = Enemy_State.CHASE;
            }
            else
            {
                //Do later
                playerHealth.healthPoints -= maxDamage * Time.deltaTime;
            }
            yield return null;
        }
        yield break;
        
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
