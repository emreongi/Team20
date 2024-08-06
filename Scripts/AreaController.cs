using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaController : MonoBehaviour
{
    private float waitTime = 30f;
    private List<GameObject> children;
    // Start is called before the first frame update
    void Start()
    {
        children = new List<GameObject>(GameObject.FindGameObjectsWithTag("Oxygen"));
        InvokeRepeating("ActivateChildrenRoutine",  0.1f, waitTime);
    }

    // Update is called once per frame
    void ActivateChildrenRoutine()
    {

        foreach (GameObject child in children)
        {
            child.SetActive(false);
        }

        for (int i = 0; i < 3; i++)
        {
            int randomNum = Random.Range(0, children.Count);
            children[randomNum].SetActive(true); 
        }

    }
}
