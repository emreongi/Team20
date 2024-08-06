using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    private float range = 15f;
    private float speed = 2f;
    private Vector3 targetpos;
    private Transform parent;
    private Vector3 parentpos;
    // Start is called before the first frame update
    void Start()
    {
        
        parentpos = transform.parent.position;
        SetNewRandomTargetPosition();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetpos, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetpos) < 0.1f)
        {
            SetNewRandomTargetPosition();
        }
    }
    void SetNewRandomTargetPosition()
    {
        targetpos = parentpos + new Vector3(
            Random.Range(-range, range),
            Random.Range(-20, 20),
            Random.Range(-range, range));
        
    }
}
