using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SuccessManager : MonoBehaviour
{
    [SerializeField] private List<SuccessData> successDatas = new List<SuccessData>();
    [SerializeField] private GameObject successObject;
    private Success success;

    public int diedEnemy = 0;
    public int collectionAmount = 0;
    public float planetTime = 0;

    void Start()
    {
        success = successObject.GetComponent<Success>();

        successObject.SetActive(false);
    }

    void Update()
    {
        //UpdatePlanetTime();
    }

    public void UpdateDiedEnemy()
    {
        diedEnemy ++;

        if (diedEnemy == 1)
        {
            SuccessCreate(9);
        }
        if (diedEnemy == 25)
        {
            SuccessCreate(2);
        }
        if (diedEnemy == 50)
        {
            SuccessCreate(8);
        }
    }

    public void UpdateItemCollection()
    {
        collectionAmount ++;

        if (collectionAmount == 25)
        {
            SuccessCreate(3);
        }
        if (collectionAmount == 50)
        {
            SuccessCreate(4);
        }
    }

    public void UpdatePlanetTime()
    {
        planetTime += Time.deltaTime;
        
        // Mavi, yeşil ve kahverengi Gezegende ilk 10 dk 'da bitirilirse
    }

    public void UpdateRS1Robot()
    {
        SuccessCreate(7);
    }

    public void UpdateCoin()
    {
        SuccessCreate(0);
    }

    public void UpdateMoaiStatue()
    {
        SuccessCreate(5);
    }

    void SuccessCreate(int index)
    {
        success.EditSuccessTraits(successDatas[index].name, successDatas[index].successDescription, successDatas[index].successImage);
        successObject.SetActive(true);
        Invoke("SuccessActive", 4);
    }

    void SuccessActive()
    {
        successObject.SetActive(false);
    }
}
