using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTemp : MonoBehaviour
{
    [SerializeField] float health = 3;

    [Header("Combat")]
    [SerializeField] float attackCD = -1f;
    [SerializeField] float attackRange = 30f;
    [SerializeField] float aggroRange = 60f;

    GameObject player;
    NavMeshAgent agent;
    Animator animator;
    float timePassed;
    float newDestinationCD = 0.5f;
    [SerializeField] private Transform planetCenter;
    private RaycastHit hitInfo;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        //planetCenter = GameObject.FindWithTag("Planet").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        animator.SetFloat("speed",agent.velocity.magnitude / agent.speed);
        
        if (timePassed >= attackCD)
        {
            if (Vector3.Distance(player.transform.position,transform.position) <= attackRange)
            {
                animator.SetTrigger("attack");
                timePassed = 0;
            }
        }
        timePassed += Time.deltaTime;

        if (newDestinationCD <= 0 && Vector3.Distance(player.transform.position,transform.position) <= aggroRange)
        {
            newDestinationCD = 0.5f;
            agent.SetDestination(player.transform.position);
        }

        newDestinationCD -= Time.deltaTime;
        transform.LookAt(player.transform);
        AlignToSurfaceNormal();
    }
    void Die()
    {
        Destroy(this.gameObject);
        animator.SetBool("isDeath",true);
    }
    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        animator.SetTrigger("damage");
        if (health <= 0)
        {
            Die();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,attackRange);
        Gizmos.color= Color.yellow;
        Gizmos.DrawWireSphere(transform.position,aggroRange);
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
}
