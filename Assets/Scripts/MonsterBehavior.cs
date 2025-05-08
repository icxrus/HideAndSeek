using UnityEngine;
using UnityEngine.AI;

public class MonsterBehavior : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private GameObject currentPathingTarget;
    [SerializeField] private bool canSeePlayer = false;
    [SerializeField] private GameObject player;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
}
