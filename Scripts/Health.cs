using Photon.Pun;
using UnityEngine;

public class Health : MonoBehaviourPunCallbacks
{
    public HealthData healthData;

    void Start()
    {
        transform.localScale = new Vector3(7, 7, 7);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            if (collider.gameObject.GetComponent<BarController>().health < 1)
            {
                collider.gameObject.GetComponent<BarController>().HealthBoost(healthData.healthValue, healthData.duration);
            }
            
            Destroy(this.gameObject);
        }
    }
}
