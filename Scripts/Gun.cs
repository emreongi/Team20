using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

[RequireComponent(typeof(AudioSource))]

public class Gun : MonoBehaviourPunCallbacks
{
    [SerializeField] private GunData gunData;
    [SerializeField] private Transform muzzle;
    private GameObject bulletPrefab;
    [SerializeField] private ParticleSystem muzzleEffect;
    private AudioSource audioSource;
    private AudioClip audioClip;
    private float fireRate;
    private int currentAmmo;
    private int initialAmmo;
    private int magSize;
    private float reloadTime;
    private float nextTimeToFire;
    private bool isReloading;
    private bool isAutomatic;
    private WaitManager waitManager;

    private CinemachineImpulseSource cinemachineImpulseSource;
    private Camera mainCamera;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioClip = gunData.fireClip;
        waitManager = GameObject.Find("Crosshair").GetComponent<WaitManager>();
        mainCamera = GameObject.Find("Camera").GetComponent<Camera>();
        InitializeGun();
        transform.localScale = gunData.gunScale;
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        HandleShooting();
        HandleReloading();
        AimAtTarget();
    }

    private void InitializeGun()
    {
        fireRate = gunData.fireRate;
        isAutomatic = gunData.isAutomotic;
        bulletPrefab = gunData.bulletPrefab;
        currentAmmo = gunData.currentAmmo;
        initialAmmo = currentAmmo;
        magSize = gunData.magSize;
        reloadTime = gunData.reloadTime;
        isReloading = false;

        Debug.Log($"{gunData.name} selected.");

        cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
    }

private void HandleShooting()
{
    if (currentAmmo > 0)
    {
        if (isAutomatic)
        {
            if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
                waitManager.TriggerWait(1f, 1f / fireRate);

                PhotonNetwork.LocalPlayer.AddScore(1);
            }

            if (Input.GetButtonUp("Fire1"))
            {
                StopMuzzleEffect();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
                waitManager.TriggerWait(1f, 1f / fireRate);

                PhotonNetwork.LocalPlayer.AddScore(1);
            }


            if (Input.GetButtonUp("Fire1"))
            {
                StopMuzzleEffect();
            }
        }
    }
    else if (!isReloading && magSize > 0)
    {
        // Cephane bittiğinde ve şarjör doluyken otomatik olarak yeniden yükle
        StartReloading();
    }
}

    private void HandleReloading()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!isReloading && magSize > 0)
            {
                StartReloading();
            }
            else
            {
                Debug.LogWarning("Not enough ammo to reload.");
            }
        }
    }

    private void StartReloading()
    {
        isReloading = true;
        waitManager.TriggerWait(1f, reloadTime); // WaitManager'a Reload işlemi başlatma
        StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(reloadTime);
        Reload();
    }

    private void AimAtTarget()
    {
        // Ekranın ortasından bir ray gönderin
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        // Ray'in çarptığı noktayı bulun
        if (Physics.Raycast(ray, out hit))
        {
            transform.LookAt(hit.point);
        }
        else
        {
            // Ray bir şeye çarpmadıysa, ileriye doğru bir yön kullanın
            transform.LookAt(ray.GetPoint(1000)); // 1000 birim uzağa bakın
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || muzzle == null)
        {
            Debug.LogError("Bullet prefab or muzzle is not assigned.");
            return;
        }

        GameObject newBullet = PhotonNetwork.Instantiate(bulletPrefab.name, muzzle.position, muzzle.rotation);
        PhotonView bulletPhotonView = newBullet.GetComponent<PhotonView>();
        audioSource.PlayOneShot(audioClip);

        if (isAutomatic)
        {
            if (!muzzleEffect.isPlaying)
            {
                muzzleEffect.Play();
            }
        }
        else
        {
            muzzleEffect.Play();
        }

        if (bulletPhotonView != null)
        {
            currentAmmo--;

            BulletMovetment bulletMovement = newBullet.GetComponent<BulletMovetment>();
            if (bulletMovement != null)
            {
                bulletMovement.SetDirection(newBullet.transform.forward);
            }
            else
            {
                Debug.LogError("BulletMovetment component not found on the instantiated bullet.");
            }

            ApplyRecoil();
        }
        else
        {
            Debug.LogError("PhotonView not found on the instantiated bullet.");
        }
    }

    private void StopMuzzleEffect()
    {
        if (muzzleEffect.isPlaying)
        {
            muzzleEffect.Stop();
        }
    }

    private void ApplyRecoil()
    {
        cinemachineImpulseSource.GenerateImpulse();
    }

    private void Reload()
    {
        int ammoToReload = Mathf.Min(initialAmmo, magSize);
        magSize -= ammoToReload;
        currentAmmo = ammoToReload;
        isReloading = false;
    }
}