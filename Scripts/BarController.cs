using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class BarController : MonoBehaviourPunCallbacks, IPunObservable
{
    public Slider oxygenBar;
    public Slider healthBar;
    private WaitManager waitManager;
    public PlayerController playerController;
    public float health = 1f;
    private float decreaseRate = 0.002f;
    private float increaseRate = 0.1f;

    public float oxygen = 1f;
    Animator animator;

    private Vector3 initialpos;
    private InventoryManager inventoryManager;
    private Vector3 offset = new Vector3(0.1f, 0.1f, 0.1f);

    private void Awake()
    {
        inventoryManager = GetComponent<InventoryManager>();
    }

    void Start()
    {
        initialpos = transform.position;
        oxygenBar.value = oxygen;
        StartCoroutine(DecreaseSliderValueOverTime());
        healthBar.value = health;

        waitManager = GameObject.Find("Crosshair").GetComponent<WaitManager>();

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        if (healthBar.value <= 0)
        {
            StartCoroutine(Death());
        }
        else if (oxygenBar.value <= 0)
        {
            StartCoroutine(Death());
        }
        else
        {
            animator.SetBool("isDeath", false);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Oxygen") && oxygen < 1)
        {
            StopAllCoroutines(); // Oksijen artışını durdurup yeniden başlatmak için
            StartCoroutine(OxygenBoostCoroutine(1f, 2f)); // Hedef değer ve süreyi ayarlayın
        }
    }

    IEnumerator DecreaseSliderValueOverTime()
    {
        while (oxygenBar.value > 0)
        {
            oxygen -= decreaseRate * Time.deltaTime;
            oxygenBar.value = oxygen;
            photonView.RPC("RPC_UpdateOxygen", RpcTarget.AllBuffered, oxygen); // AllBuffered, oyuna yeni katılanların da güncellenmiş değeri almasını sağlar.
            yield return null;
        }
    }

    public void Damage(float damage)
    {
        if (!photonView.IsMine) return;

        health -= damage;
        healthBar.value = health;
        photonView.RPC("RPC_UpdateHealth", RpcTarget.AllBuffered, health); // AllBuffered, oyuna yeni katılanların da güncellenmiş değeri almasını sağlar.
    }

    [PunRPC]
    void RPC_UpdateOxygen(float newOxygenValue, PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal) // Bu kontrol, sadece yerel oyuncunun değerlerini güncellemesini sağlar
        {
            oxygenBar.value = newOxygenValue;
            oxygen = newOxygenValue;
        }
    }

    [PunRPC]
    void RPC_UpdateHealth(float newHealthValue, PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal) // Bu kontrol, sadece yerel oyuncunun değerlerini güncellemesini sağlar
        {
            healthBar.value = newHealthValue;
            health = newHealthValue;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
            stream.SendNext(oxygenBar.value);
        }
        else
        {
            health = (float)stream.ReceiveNext();
            oxygenBar.value = (float)stream.ReceiveNext();
            healthBar.value = health;
        }
    }

    IEnumerator Death()
    {
        animator.SetBool("isDeath", true);
        transform.position = new Vector3(transform.position.x - 0.03f, transform.position.y-0.03f, transform.position.z - 0.03f);
        for (int i = 0; i < 11; i++)
        {
            inventoryManager.MultiDropItem(i, transform.position + offset);
            offset += offset;
        }
        transform.position = new Vector3(transform.position.x - 0.03f, transform.position.y-0.03f, transform.position.z - 0.03f);
        transform.position = new Vector3(transform.position.x - 0.03f, transform.position.y - 0.03f, transform.position.z - 0.03f);
        yield return new WaitForSecondsRealtime(5f);
        animator.SetBool("isDeath", false);
        transform.position = initialpos;
        health = 1f;
        healthBar.value = 1f;
        oxygenBar.value = 1f;
        offset = new Vector3(0.1f, 0.1f, 0.1f);
    }

    public void HealthBoost(float value, float duration)
    {
        StartCoroutine(HealthBoostCoroutine(value, duration));
    }

    public void OxygenBoost(float value, float duration)
    {
        StartCoroutine(OxygenBoostCoroutine(value, duration));
    }

    IEnumerator HealthBoostCoroutine(float targetValue, float duration)
    {
        float startValue = healthBar.value;
        float elapsedTime = 0f;

        waitManager.TriggerWait(targetValue, duration);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            healthBar.value = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
            health = healthBar.value;
            photonView.RPC("RPC_UpdateHealth", RpcTarget.AllBuffered, healthBar.value); // AllBuffered, oyuna yeni katılanların da güncellenmiş değeri almasını sağlar.
            yield return null;
        }

        healthBar.value = targetValue;
        health = targetValue;
    }

    IEnumerator OxygenBoostCoroutine(float targetValue, float duration)
    {
        float startValue = oxygenBar.value;
        float elapsedTime = 0f;

        waitManager.TriggerWait(targetValue, duration);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            oxygenBar.value = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
            oxygen = oxygenBar.value;
            photonView.RPC("RPC_UpdateOxygen", RpcTarget.AllBuffered, oxygenBar.value); // AllBuffered, oyuna yeni katılanların da güncellenmiş değeri almasını sağlar.
            yield return null;
        }

        oxygenBar.value = targetValue;
        oxygen = targetValue;
    }
}
