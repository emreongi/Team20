using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HerbEnemyController : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    private Animator animator;
    private GameObject target;
    [SerializeField] private float health = 100f;
    private bool isAttack = false;
    [SerializeField] private float detectionRadius = 10f;

    [SerializeField] private Image healthBar;

    private Coroutine attackCoroutine;

    float targetDistance;
    public Color activeGizmosColor = Color.red;
    public Color passiveGizmosColor = Color.green;

    void Start()
    {
        animator = GetComponent<Animator>();
        target = GameObject.FindWithTag(playerTag);
    }

    void Update()
    {
        if (target != null && health > 0)
        {
            targetDistance = Vector3.Distance(transform.position, target.transform.position);
            if (detectionRadius >= targetDistance)
            {
                TargetLookAt();
            }

            if (!isAttack)
            {
                animator.SetBool("isIdle", true);
            }
        }
    }

    void TargetLookAt()
    {
        Vector3 targetPosition = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        transform.LookAt(targetPosition);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == playerTag && health > 0)
        {
            Debug.Log(playerTag + " nesnesine çarpıyor.");
            attackCoroutine = StartCoroutine(AttackPlayer());
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == playerTag)
        {
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
            animator.SetBool("isAttack", false);
            isAttack = false;
        }
    }

    private IEnumerator AttackPlayer()
    {
        while (true)
        {
            animator.SetBool("isAttack", true);
            isAttack = true;
            yield return new WaitForSeconds(2f);
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;

        healthBar.fillAmount = health / 100;

        if (health <= 0)
        {
            health = 0;
            animator.SetBool("isDeath", true);
            animator.SetBool("isIdle", false);
            animator.SetBool("isAttack", false);
        }
    }

    void OnDrawGizmos()
    {
        if (target)
        {
            if (detectionRadius <= targetDistance)
            {
                Gizmos.color = activeGizmosColor;
            }
            else
            {
                Gizmos.color = passiveGizmosColor;
            }

            Vector3 previousPoint = transform.position + new Vector3(detectionRadius, 0, 0);

            for (int i = 1; i <= 360; i++)
            {
                float angle = i * Mathf.PI * 2 / 360;
                Vector3 newPoint = transform.position + new Vector3(Mathf.Cos(angle) * detectionRadius, 0, Mathf.Sin(angle) * detectionRadius);

                Gizmos.DrawLine(previousPoint, newPoint);
                previousPoint = newPoint;
            }
        }
    }
}
