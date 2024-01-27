
using UnityEngine;
using TMPro;
using System.Collections;

// Code by tosaaa_3
// 19/01/2024
// skamtebord

public class Shotgun : MonoBehaviour
{
    public int damagePerPellet = 10;
    public int pelletsPerShot = 8;
    public float spreadAngle = 10f;
    public float shootingRange = 10f;
    public string targetTag = "Enemy";
    public int maxAmmo = 30;
    public float reloadTime = 2f;
    public AudioClip reloadingSoundClip;
    public AudioClip shootingSoundClip;
    public Transform muzzleFlashTransform; // Reference to the muzzle flash transform
    private AudioSource audioSource;

    public Transform bulletExitPoint; // Specify the exit point of the bullets

    private int currentAmmo;
    private bool isReloading = false;

    public TextMeshProUGUI ammoText;

    public float recoilAmount = 5f; // Adjust the amount of recoil
    public float recoilDuration = 0.1f; // Adjust the duration of the recoil effect

    private Quaternion originalRotation;
    private GameObject muzzleFlash; // The instantiated muzzle flash

    void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoUI();

        // Adding AudioSource component and setting it up
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = shootingSoundClip;

        originalRotation = transform.localRotation;

        // Instantiate the muzzle flash
        if (muzzleFlashTransform != null)
        {
            muzzleFlash = Instantiate(Resources.Load<GameObject>("MuzzleFlashPrefab")); // Load muzzle flash prefab from Resources folder
            muzzleFlash.transform.parent = muzzleFlashTransform;
            muzzleFlash.SetActive(false); // Set it inactive initially
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !isReloading)
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            Reload();
        }
    }


    void Shoot()
    {
        if (currentAmmo > 0)
        {
            for (int i = 0; i < pelletsPerShot; i++)
            {
                // Calculate spread direction
                Vector3 spreadDirection = Quaternion.Euler(Random.Range(-spreadAngle, spreadAngle), Random.Range(-spreadAngle, spreadAngle), 0f) * transform.forward;

                // Raycast to detect hits
                RaycastHit[] hits = Physics.RaycastAll(bulletExitPoint.position, spreadDirection, shootingRange);

                foreach (var hit in hits)
                {
                    //hit enemy idk

                    // Apply force to non-enemy physics objects
                    Rigidbody hitRigidbody = hit.transform.GetComponent<Rigidbody>();
                    if (hitRigidbody != null)
                    {
                        hitRigidbody.AddForceAtPosition(spreadDirection * 700f, hit.point);
                    }
                }
            }

            // Play shooting sound
            PlayShootingSound();

            // Show muzzle flash
            ShowMuzzleFlash();

            // Apply recoil
            ApplyRecoil();

            currentAmmo--;
            UpdateAmmoUI();
        }
        else
        {
            // Out of ammo, reload
            Reload();
        }
    }

    void Reload()
    {
        isReloading = true;
        PlayReloadingSound();
        Invoke("FinishReload", reloadTime);
    }

    void FinishReload()
    {
        currentAmmo = maxAmmo;
        isReloading = false;
        UpdateAmmoUI();
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = currentAmmo + " / " + maxAmmo;
        }
    }

    void PlayShootingSound()
    {
        if (audioSource != null && shootingSoundClip != null)
        {
            audioSource.PlayOneShot(shootingSoundClip);
        }
    }

    void ApplyRecoil()
    {
        // Apply recoil effect by changing the local rotation's pitch (X rotation)
        Quaternion recoilRotation = Quaternion.Euler(-recoilAmount, 0f, 0f); // Note the negative recoilAmount
        transform.localRotation *= recoilRotation;

        // Start coroutine to reset rotation after recoil duration
        StartCoroutine(ResetRotation());
    }

    void PlayReloadingSound()
    {
        // Play reloading sound if available
        if (audioSource != null && reloadingSoundClip != null)
        {
            audioSource.PlayOneShot(reloadingSoundClip);
        }
    }

    void ShowMuzzleFlash()
    {
        // Show the muzzle flash
        if (muzzleFlash != null && muzzleFlashTransform != null)
        {
            muzzleFlash.transform.position = muzzleFlashTransform.position;
            muzzleFlash.transform.rotation = muzzleFlashTransform.rotation;
            muzzleFlash.SetActive(true);

            // Disable the muzzle flash after a short delay (adjust the duration as needed)
            StartCoroutine(DisableMuzzleFlash());
        }
    }

    IEnumerator DisableMuzzleFlash()
    {
        // Wait for a short duration
        yield return new WaitForSeconds(0.1f);

        // Disable the muzzle flash
        if (muzzleFlash != null)
        {
            muzzleFlash.SetActive(false);
        }
    }

    IEnumerator ResetRotation()
    {
        // Get the initial rotation
        Quaternion startRotation = transform.localRotation;

        // Get the target rotation (original rotation)
        Quaternion targetRotation = originalRotation;

        // Time elapsed
        float elapsed = 0f;

        while (elapsed < recoilDuration)
        {
            // Interpolate between start and target rotations
            transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / recoilDuration);

            // Increment elapsed time
            elapsed += Time.deltaTime;

            yield return null;
        }

        // Ensure the final rotation is exactly the target rotation
        transform.localRotation = targetRotation;
    }
}
