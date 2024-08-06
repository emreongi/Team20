using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAgent : MonoBehaviour
{
    [SerializeField] private string playerName = "Player";
    [SerializeField] private Transform target;
    private Vector3 targetLocation;
    [SerializeField] private Color activeGizmosColor = Color.red;
    [SerializeField] private Color passiveGizmosColor = Color.green;

    [SerializeField] private float detectionRadius = 10f;
    private float playerDistance;

    private NavMeshAgent navMeshAgent;
    [SerializeField] private float activeSpeed = 3.5f;
    [SerializeField] private float passiveSpeed = 1.5f;

    [Tooltip("It determines the minimum and maximum value that the agent will randomly go to on the X-axis. X => -X & +X")]
    [SerializeField] private float randomPositionX = 5f;
    [Tooltip("It determines the minimum and maximum value that the agent will randomly move on the Z-axis. Z => -Z & +Z")]
    private Vector3 randomTargetPoint;

    private RaycastHit hitInfo;

    [SerializeField] private Transform planet; // Gezegenin merkezi

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        target = GameObject.Find(playerName).transform;

        RandomPointDetermination();
    }

    void Update()
    {
        playerDistance = Vector3.Distance(transform.position, target.position);

        if (playerDistance <= detectionRadius)
        {
            targetLocation = target.position;
            navMeshAgent.speed = activeSpeed;
        }
        else
        {
            targetLocation = randomTargetPoint;
            navMeshAgent.speed = passiveSpeed;

            if (Vector3.Distance(transform.position, randomTargetPoint) <= 0.5f)
            {
                RandomPointDetermination();
            }
        }

        navMeshAgent.SetDestination(targetLocation);
        AlignToSurfaceNormal();
    }

    void RandomPointDetermination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * randomPositionX;
        randomDirection += transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, randomPositionX, -1);
        randomTargetPoint = navHit.position;
    }

    void AlignToSurfaceNormal()
    {
        // Gezegenin merkezine olan yön vektörünü hesapla
        Vector3 directionToCenter = (transform.position - planet.position).normalized;

        // Ajanın yukarı vektörünü bu yönle hizala
        transform.up = directionToCenter;

        // Raycast downwards to find the surface normal
        if (Physics.Raycast(transform.position, -transform.up, out hitInfo, 10f))
        {
            Vector3 surfaceNormal = hitInfo.normal;

            // Rotate the agent to align with the surface normal
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, surfaceNormal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    void OnDrawGizmos()
    {
        if (target)
        {
            if (playerDistance <= detectionRadius)
            {
                Gizmos.color = activeGizmosColor;
            }
            else
            {
                Gizmos.color = passiveGizmosColor;
            }

            Gizmos.DrawLine(transform.position, targetLocation);
        }

        // Draw a ray to visualize the raycast direction
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, -transform.up * 5f); // Adjust the length as needed
    }
}