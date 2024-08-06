using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class Enemy : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private float health = 1;
    [SerializeField] private float attackCD = 3f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float aggroRange = 4f;
    [SerializeField] private Transform planetCenter;

    private List<GameObject> players = new List<GameObject>();
    private NavMeshAgent agent;
    private Animator animator;
    private float newDestinationCD = 0.5f;
    private bool canAttack = true;
    private GameObject targetPlayer;
    private SuccessManager successManager;

    void Start()
    {
        planetCenter = GameObject.FindWithTag("Planet").transform;
        successManager = GameObject.Find("GameManager").GetComponent<SuccessManager>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (!photonView.IsMine)
        {
            agent.enabled = false;
        }

        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            players.Add(player);
        }

        if (players.Count == 0)
        {
            Debug.LogWarning("No players found!");
        }

        health = 1;
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        animator.SetFloat("speed", agent.velocity.magnitude / agent.speed);

        newDestinationCD -= Time.deltaTime;

        if (players.Count > 0)
        {
            targetPlayer = GetClosestPlayer();
            if (targetPlayer != null && Vector3.Distance(targetPlayer.transform.position, transform.position) <= aggroRange)
            {
                if (newDestinationCD <= 0)
                {
                    newDestinationCD = 0.5f;
                    agent.SetDestination(targetPlayer.transform.position);
                }
            }
        }

        AlignToSurfaceNormal();
    }

    private GameObject GetClosestPlayer()
    {
        GameObject closestPlayer = null;
        float minDistance = Mathf.Infinity;

        foreach (var player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestPlayer = player;
            }
        }

        return closestPlayer;
    }

    private void AlignToSurfaceNormal()
    {
        Vector3 directionToCenter = (transform.position - planetCenter.position).normalized;
        transform.up = directionToCenter;

        if (Physics.Raycast(transform.position + transform.up * 0.5f, -transform.up, out RaycastHit hitInfo, 10f))
        {
            Vector3 surfaceNormal = hitInfo.normal;
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, surfaceNormal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (!photonView.IsMine) return;

        health -= damageAmount;
        animator.SetTrigger("damage");
        if (health <= 0)
        {
            photonView.RPC("DieRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    private void Attack()
    {
        if (targetPlayer != null)
        {
            BarController barController = targetPlayer.GetComponent<BarController>();
            if (barController != null)
            {
                barController.Damage(0.1f);
            }
        }
    }

    [PunRPC]
    private void DieRPC()
    {
        successManager.UpdateDiedEnemy();
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(animator.GetFloat("speed"));
        }
        else
        {
            health = (float)stream.ReceiveNext();
            Vector3 position = (Vector3)stream.ReceiveNext();
            Quaternion rotation = (Quaternion)stream.ReceiveNext();
            float speed = (float)stream.ReceiveNext();

            transform.position = position;
            transform.rotation = rotation;
            animator.SetFloat("speed", speed);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && photonView.IsMine && canAttack)
        {
            if (Vector3.Distance(transform.position, collision.transform.position) <= attackRange)
            {
                animator.SetTrigger("attack");
                photonView.RPC("Attack", RpcTarget.All);
                StartCoroutine(AttackCooldown());
            }
        }

        if (collision.gameObject.CompareTag("ElectromagneticBomb") && photonView.IsMine)
        {
            TakeDamage(0.3f);
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCD);
        canAttack = true;
    }
}