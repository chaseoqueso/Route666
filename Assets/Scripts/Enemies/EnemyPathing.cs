using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyPathing : MonoBehaviour
{
    [SerializeField] private NavMeshAgent enemyAgent;
    private Transform playerLoc;

    void Start()
    {
        playerLoc = GameManager.instance.player.transform;
    }

    void Update()
    {
        enemyAgent.SetDestination(playerLoc.position);
    }
}
