using UnityEngine;
using Photon.Pun;
using System.Collections;
using Cinemachine;

public class ElectricCurrent : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private string mineTagName = "Crystal";
    [SerializeField] private string aimPointName = "AimPoint";
    [SerializeField] private Transform aimPoint;

    private CinemachineImpulseSource cinemachineImpulseSource;
    [SerializeField] private Transform electricCurrentTransform;

    void Start()
    {   
        aimPoint = GameObject.Find(aimPointName).transform;
        cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
        transform.localScale = new Vector3(3, 3, 3);
    }

    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            RaycastProduction(true);
        }
        else
        {
            RaycastProduction(false);
        }

        electricCurrentTransform.LookAt(aimPoint);
    }

    void RaycastProduction(bool isBreaking)
    {
        RaycastHit hit;
        if (Physics.Raycast(muzzlePoint.position, muzzlePoint.forward, out hit))
        {
            if (hit.collider.CompareTag(mineTagName))
            {
                AdjustCylinder(muzzlePoint.position, hit.point);
                ApplyRecoil();
                RockShattering rockShattering = hit.transform.GetComponent<RockShattering>();
                if (rockShattering != null)
                {
                    if (isBreaking)
                    {
                        rockShattering.StartBreaking();
                    }
                    else
                    {
                        rockShattering.StopBreaking();
                    }
                }
            }
            else
            {
                AdjustCylinder(muzzlePoint.position, muzzlePoint.position); // Silindiri sıfırla
            }
        }
        else
        {
            AdjustCylinder(muzzlePoint.position, muzzlePoint.position); // Silindiri sıfırla
        }
    }

    private void AdjustCylinder(Vector3 start, Vector3 end)
    {
        // Silindirin uzunluğunu hesapla
        float distance = Vector3.Distance(start, end);
        electricCurrentTransform.localScale = new Vector3(electricCurrentTransform.localScale.x, distance / 2f, electricCurrentTransform.localScale.z);

        // Silindirin konumunu ve yönünü ayarla
        electricCurrentTransform.position = start + (end - start) / 2f;
        electricCurrentTransform.LookAt(end);
    }

    private void ApplyRecoil()
    {
        cinemachineImpulseSource.GenerateImpulse();
    }
}