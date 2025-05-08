using System.Threading;
using UnityEditor.Searcher;
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

    //Variables
    [SerializeField] private bool canSeePlayer = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

   

}
