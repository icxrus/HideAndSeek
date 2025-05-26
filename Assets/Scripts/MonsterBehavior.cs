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


    //External data
    [SerializeField] private GameObject currentPathingTarget;
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

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        GetMovementSpots(rooms);
        GetHidingSpots(rooms);
    }

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

}
