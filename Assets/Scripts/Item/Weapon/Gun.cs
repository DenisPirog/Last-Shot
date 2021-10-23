using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Gun : MonoBehaviour
{
    [Header("Scripts")]

    [SerializeField] private Recoil _recoilScript;

    [Header("UI")]
    public GameObject gunUI;

    [Header("Sounds")]

    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip reloadSound;
    [SerializeField] private AudioClip noAmmoSound;
    [SerializeField] private AudioSource audioSource;

    [Header("Weapon Settings")]

    [SerializeField] private Camera cam;
    [SerializeField] private float damage;
    [SerializeField] private float fireRate = 0.1f; // 1f = 1second
    public int amountOfAmmo;
    [SerializeField] private Text textOfAmountOfAmmo;

    //Rotations
    [HideInInspector] public Vector3 _currentRotation;
    [HideInInspector] public Vector3 _targetRotation;

    [Header("Recoil Values")]

    public float _recoilX;
    public float _recoilY;
    public float _recoilZ;


    [Header("Recoil Settings")]

    public float _snappiness;
    public float _returnSpeed;

    private float _nextTimeToFire = 0f;

    [HideInInspector] public int amountOfAmmoSave;

    private void Awake()
    {
        amountOfAmmoSave = amountOfAmmo;
    }
    private void Update()
    {
        textOfAmountOfAmmo.text = $"{amountOfAmmo} / {amountOfAmmoSave}";

        _targetRotation = Vector3.Lerp(_targetRotation, Vector3.zero, _returnSpeed * Time.deltaTime);
        _currentRotation = Vector3.Slerp(_currentRotation, _targetRotation, _snappiness * Time.deltaTime);
    }

    [PunRPC]
    public void RPC_Shoot()
    {
        if (Time.time >= _nextTimeToFire && amountOfAmmo != 0)
        {
            audioSource.PlayOneShot(shootSound);

            _nextTimeToFire = Time.time + fireRate;           

            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            ray.origin = cam.transform.position;

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                hit.collider.gameObject
                    .GetComponent<IDamageable>()?
                    .TakeDamage(damage);
            }
            amountOfAmmo -= 1;

            audioSource.Stop();
            audioSource.clip = shootSound;
            audioSource.Play();

            _recoilScript.RecoilFire();
        }
        else if (amountOfAmmo == 0 && audioSource != null)
        {
            audioSource.PlayOneShot(noAmmoSound);
        }
    }

    public bool TryReload()
    {
        if (amountOfAmmo == amountOfAmmoSave)
            return false;
        
        Reload();
        return true;
    }
    
    private void Reload()
    {
        amountOfAmmo = amountOfAmmoSave;
        audioSource.PlayOneShot(reloadSound);
    }
}
