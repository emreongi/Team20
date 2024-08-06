using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonAnimatorView))]
[RequireComponent(typeof(PhotonTransformView))]
[RequireComponent(typeof(planetGravity))]
[RequireComponent(typeof(NavMeshAgent))]

public class FollowerFriend : MonoBehaviour
{
    [SerializeField] private string playerName = "Player";
    [SerializeField] private string planetName = "Planet";
    [SerializeField] private Transform planetCenter;
    [SerializeField] private Color activeGizmosColor = Color.red;
    [SerializeField] private Color passiveGizmosColor = Color.green;
    private float speed;

    [SerializeField] private float detectionRadius = 10f;
    private NavMeshAgent navMeshAgent;
    private float randomDistance = 8f;

    private Vector3 destination;
    private bool playerSeen = false;

    [SerializeField] private GameObject heartPrefab;
    private bool isHeartCreated = false;

    [SerializeField] private float waitingTime = 3f;
    [SerializeField] private float heartLifeTime = 4f;
    [SerializeField] private float resetHeartTime = 5f;
    [SerializeField] private float heartMinDistance = 50;

    private Animator animator;
    public Transform target;

    private RaycastHit hitInfo;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        target = GameObject.FindWithTag(playerName).transform;
        if (!target)
        {
            Debug.LogError("Player name not found");
        }

        planetCenter = GameObject.FindWithTag(planetName).transform;
        if (!planetCenter)
        {
            Debug.LogError("Planet name not found");
        }

        speed = navMeshAgent.speed;

        SetRandomDestination();
    }

    void Update()
    {
        AlignToSurfaceNormal();
        if (!target) return;

        transform.LookAt(target);

        float playerDistance = Vector3.Distance(transform.position, target.position);

        if (playerDistance <= detectionRadius)
        {
            playerSeen = true;
            navMeshAgent.enabled = true;
            destination = target.position;
            FollowTarget(target.position);

            if (!isHeartCreated && playerDistance <= heartMinDistance)
            {
                StartCoroutine(CreateHeart());
            }
        }
        else
        {
            playerSeen = false;
            navMeshAgent.speed = speed;
            FollowTarget(destination);

            if (Vector3.Distance(transform.position, destination) <= 0.5f && !isWaiting)
            {
                StartCoroutine(WaitBeforeNextMove());
            }
        }
        UpdateAnimator();
    }

    void SetRandomDestination()
    {
        Vector3 randomDirection = Random.onUnitSphere * randomDistance;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, randomDistance, NavMesh.AllAreas))
        {
            destination = hit.position;
            navMeshAgent.SetDestination(destination);
        }
        else
        {
            SetRandomDestination();
        }
    }

    void FollowTarget(Vector3 target)
    {
        navMeshAgent.SetDestination(target);
    }

    void AlignToSurfaceNormal()
    {
        Vector3 directionToCenter = (transform.position - planetCenter.position).normalized;

        transform.up = directionToCenter;
        if (Physics.Raycast(transform.position + transform.up * 0.5f, -transform.up, out hitInfo, 10f))
        {
            Vector3 surfaceNormal = hitInfo.normal;

            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, surfaceNormal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    private void UpdateAnimator()
    {
        if (navMeshAgent.velocity.magnitude > 0.1f)
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isIdle", false);
        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isIdle", true);
        }
    }

    void OnDrawGizmos()
    {
        if (target)
        {
            Gizmos.color = playerSeen ? activeGizmosColor : passiveGizmosColor;
            Gizmos.DrawLine(transform.position, destination);
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, -transform.up * 5f);
    }

    private IEnumerator CreateHeart()
    {
        isHeartCreated = true;
        GameObject newHeart = Instantiate(heartPrefab, transform.position, Quaternion.identity, transform);
        animator.SetBool("isHello", true);

        float elapsedTime = 0f;
        float duration = 1f;

        Vector3 initialScale = newHeart.transform.localScale;
        Vector3 targetScale = new Vector3(35f, 35f, 35f);
        Vector3 initialPosition = newHeart.transform.localPosition;
        Vector3 targetPosition = new Vector3(0f, 250, 0f);
        Quaternion initialRotation = newHeart.transform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(0f, 360f, 0);

        while (elapsedTime < duration)
        {
            newHeart.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / duration);
            newHeart.transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / duration);
            newHeart.transform.localRotation = Quaternion.Lerp(initialRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        newHeart.transform.localScale = targetScale;
        newHeart.transform.localPosition = targetPosition;
        newHeart.transform.localRotation = targetRotation;

        yield return new WaitForSeconds(heartLifeTime);
        Destroy(newHeart);
        animator.SetBool("isHello", false);
        isHeartCreated = false;
        StartCoroutine(IdleBeforeNextHeart());
    }

    private bool isWaiting = false;

    private IEnumerator WaitBeforeNextMove()
    {
        isWaiting = true;
        animator.SetBool("isWalking", false);
        animator.SetBool("isIdle", true);
        yield return new WaitForSeconds(waitingTime);
        SetRandomDestination();
        animator.SetBool("isWalking", true);
        isWaiting = false;
    }

    private IEnumerator IdleBeforeNextHeart()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isIdle", true);
        navMeshAgent.speed = 0;
        yield return new WaitForSeconds(resetHeartTime);
        navMeshAgent.speed = speed;
        animator.SetBool("isIdle", false);
    }
}