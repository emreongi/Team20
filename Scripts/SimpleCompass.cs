using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SimpleCompass : MonoBehaviour
{
    
    public Transform targetObject;
    public Transform[] collectibles;
    public TMPro.TextMeshProUGUI text;
    public TMPro.TextMeshProUGUI distance; 
    public Transform baseArea;
    void Start()
    {
            
    }

    // Update is called once per frame
    void Update()
    {
        if (collectibles != null)
        {
            targetObject = FindNearestObject();
        }
        else { targetObject = baseArea; }
        Vector3 target = targetObject.position;
        Vector3 relativeTarget = transform.InverseTransformDirection(target);
        float needleRotation = Mathf.Atan2(relativeTarget.x, relativeTarget.z) * Mathf.Rad2Deg;


        text.text = ((int)needleRotation).ToString() + "°";
        float distancevalue = Vector3.Distance(transform.position, targetObject.position);
        distance.text = ((int)distancevalue).ToString() + "m";
    }
    private Transform FindNearestObject()
    {
        Transform nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (Transform collectible in collectibles)
        {
            if (collectible.gameObject.activeInHierarchy)
            {
                float distance = Vector3.Distance(transform.localPosition, collectible.localPosition);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = collectible;
                }
            }
        }

        return nearest;
    }

}
