using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SuccesAnimationController : MonoBehaviour
{
    public Image succesItem;
    public Transform canvas;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.W)) 
        {
            Instantiate(succesItem, canvas);
        }
    }
}
