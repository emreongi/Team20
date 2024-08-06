using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BulletMovetment : MonoBehaviourPunCallbacks
{
    public Rigidbody rb;
    [SerializeField] private BulletData bulletData;

    private float damage;

    private float speed;
    public Vector3 direction;

    void Start()
    {
        InitializeBullet();

        damage = bulletData.damage;
    }

    void InitializeBullet()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody is null in BulletMovement.");
            return;
        }

        rb.useGravity = bulletData.isGravity;
        speed = bulletData.speed;

        Collider bulletCollider = GetComponent<Collider>();
        if (bulletCollider != null)
        {
            bulletCollider.material = bulletData.physicMaterial;
        }

        if (direction == Vector3.zero)
        {
            direction = transform.forward;
        }

        rb.velocity = direction * speed;

        StartCoroutine(DestroyAfterLifespan());
    }

    IEnumerator DestroyAfterLifespan()
    {
        yield return new WaitForSeconds(bulletData.maxLifeSpan);
        DestroyBullet();
    }

    public void SetDirection(Vector3 newDirection)
    {
        if (rb == null)
        {
            Debug.LogError("Rigidbody is null in BulletMovement.");
            return;
        }

        direction = newDirection;
        rb.velocity = direction * speed;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            if (photonView.IsMine)
            {
                photonView.RPC("HitObject", RpcTarget.AllBuffered);
            }
        }

        if (collision.gameObject.GetComponent<Enemy>())
        {
            if (photonView.IsMine)
            {
                Debug.Log("Tüfek vurdu");
                collision.gameObject.GetComponent<Enemy>().TakeDamage(damage);
                photonView.RPC("DestroyBullet", RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    void HitObject()
    {
        if (bulletData.explosionObject)
        {
            Instantiate(bulletData.explosionObject, transform.position, Quaternion.identity);
        }
        DestroyBullet();
    }

    [PunRPC]
    void DestroyBullet()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}