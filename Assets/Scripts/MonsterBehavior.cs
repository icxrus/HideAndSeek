using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/* 
Monster behavior
1. Go to new room or stay
	- if player in room higher chance to stay
2. Search room
    2b.check hiding spot or look around
        -> if player in check hiding spot 
            - roar and kill player
	    -> if not, go back to 1. 
*/

public class MonsterBehavior : MonoBehaviour
{
    //Monster components
    private NavMeshAgent agent;
    private Animator animator;

    //Pathing
    [SerializeField] private Transform currentPathingTarget;

    //External data
    [SerializeField] private GameObject player;
    [SerializeField] private Transform[] rooms;
    [SerializeField] private List<Transform> movementSpotsRoom0 = new List<Transform>();
    [SerializeField] private List<Transform> movementSpotsRoom1 = new List<Transform>();
    [SerializeField] private List<Transform> movementSpotsRoom2 = new List<Transform>();
    [SerializeField] private List<Transform> hidingSpotsRoom0 = new List<Transform>();
    [SerializeField] private List<Transform> hidingSpotsRoom1 = new List<Transform>();
    [SerializeField] private List<Transform> hidingSpotsRoom2 = new List<Transform>();

    //Variables
    [SerializeField] private bool canSeePlayer = false;

    private enum MonsterState { Idle, Moving, Searching, GameOver }
    private MonsterState currentState;

    [SerializeField] private float baseStayChance = 0.5f;
    private int currentRoomIndex;

    private Coroutine searchRoutine;
    private bool loopActive = true;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        currentState = MonsterState.Idle;

        GetMovementSpots(rooms);
        GetHidingSpots(rooms);
    }

    private void Start()
    {
        StartCoroutine(BehaviorLoop());
    }

    private void Update()
    {
        //If we have reached the room start searching
        if (currentState == MonsterState.Moving && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            currentState = MonsterState.Searching;
        }

        if (currentRoomIndex == player.GetComponent<PlayerRoomDeterminer>().roomIndex && !player.GetComponent<HidingController>().isHidden)
            canSeePlayer = true;
        else
            canSeePlayer = false;
    }

    #region Data Getters
    private void GetMovementSpots(Transform[] _rooms)
    {
        for (int i = 0; i < _rooms.Length; i++)
        {
            foreach (Transform child in _rooms[i].GetComponentsInChildren<Transform>())
            {
                if (child.CompareTag("MovementSpot"))
                {
                    switch (i)
                    {
                        case 0:
                            movementSpotsRoom0.Add(child);
                            break;
                        case 1:
                            movementSpotsRoom1.Add(child);
                            break;
                        case 2:
                            movementSpotsRoom2.Add(child);
                            break;
                    }
                }
            }
        }
    }

    private void GetHidingSpots(Transform[] _rooms)
    {
        for (int i = 0; i < _rooms.Length; i++)
        {
            foreach (Transform child in _rooms[i].GetComponentsInChildren<Transform>())
            {
                if (child.CompareTag("HidingSpot"))
                {
                    switch (i)
                    {
                        case 0:
                            hidingSpotsRoom0.Add(child);
                            break;
                        case 1:
                            hidingSpotsRoom1.Add(child);
                            break;
                        case 2:
                            hidingSpotsRoom2.Add(child);
                            break;
                    }
                }
            }
        }
    }
    

    //Check current room monster is in
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Room0"))
            currentRoomIndex = 0;
        else if (other.CompareTag("Room1"))
            currentRoomIndex = 1;
        else if (other.CompareTag("Room2"))
            currentRoomIndex = 2;
    }
    #endregion

    //BT loop for differing behavior
    private IEnumerator BehaviorLoop()
    {
        while (loopActive)
        {
            yield return new WaitForSeconds(1f);

            switch (currentState)
            {
                case MonsterState.Idle:
                    StayOrMove();
                    break;
                case MonsterState.Moving:
                    animator.Play("walk4");
                    break;
                case MonsterState.Searching:
                    if (searchRoutine == null)
                        searchRoutine = StartCoroutine(SearchRoom());
                    break;
            }
        }
    }

    private IEnumerator SearchRoom()
    {
        Debug.Log("Monster searching room...");

        // Decide whether to check hiding spots or just look around
        bool checkHidingSpot = UnityEngine.Random.value > 0.5f;

        if (checkHidingSpot)
        {
            List<Transform> hidingSpots = GetHidingSpotsForRoom(currentRoomIndex);

            foreach (var spot in hidingSpots)
            {
                currentState = MonsterState.Moving;
                agent.SetDestination(spot.position);
                currentPathingTarget = spot;
                yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);

                animator.Play("rage");
                currentState = MonsterState.Searching;
                Debug.Log("Monster searches spot at: " + spot.position);
                yield return new WaitForSeconds(3f);

                if (IsPlayerHidingHere(spot))
                {
                    Debug.Log("ROAR! Player found!");
                    animator.Play("attack4RSpike");
                    // game over
                    searchRoutine = null;
                    loopActive = false;
                    currentState = MonsterState.GameOver;
                    yield break;
                }
            }
        }

        // If we didn't find player, go back to decision phase
        searchRoutine = null;
        currentState = MonsterState.Idle;
    }

    private bool IsPlayerHidingHere(Transform spot)
    {
        float detectionRange = 1.5f;
        return Vector3.Distance(player.transform.position, spot.position) <= detectionRange;
    }

    private List<Transform> GetHidingSpotsForRoom(int currentRoomIndex)
    {
        switch (currentRoomIndex)
        {
            case 0: return hidingSpotsRoom0;
            case 1: return hidingSpotsRoom1;
            case 2: return hidingSpotsRoom2;
            default: return new List<Transform>();
        }
    }

    private void StayOrMove()
    {
        float stayChance = baseStayChance;

        //If player is in the room increase chance to start searching
        if (player.GetComponent<PlayerRoomDeterminer>().IsPlayerInRoom(currentRoomIndex))
            stayChance += 0.3f;

        if (UnityEngine.Random.value < stayChance)
        {
            Debug.Log("Monster stays in room.");
            currentState = MonsterState.Searching;
        }
        else
        {
            Debug.Log("Monster moves to a new room.");
            MoveToNewRoom();
        }
    }

    private void MoveToNewRoom()
    {
        //Randomly select room to move to
        currentRoomIndex = UnityEngine.Random.Range(0, rooms.Length);
        List<Transform> currentMovementSpots = new();

        if (currentRoomIndex == 0)
            currentMovementSpots = movementSpotsRoom0;
        else if (currentRoomIndex == 1)
            currentMovementSpots = movementSpotsRoom1;
        else if (currentRoomIndex == 2)
            currentMovementSpots = movementSpotsRoom2;

        if (currentMovementSpots.Count == 0)
        {
            Debug.LogWarning("No movement spots in room " + currentRoomIndex);
            return;
        }

        //Randomly select a movement spot
        int i = UnityEngine.Random.Range(0, currentMovementSpots.Count);

        currentState = MonsterState.Moving;
        agent.SetDestination(currentMovementSpots[i].position);
        currentPathingTarget = currentMovementSpots[i];
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (currentPathingTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(currentPathingTarget.position, 0.3f);
            Gizmos.DrawLine(transform.position, currentPathingTarget.position);
        }
    }
#endif
}
