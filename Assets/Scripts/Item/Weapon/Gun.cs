using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Gun : MonoBehaviour
{ 
    [Header("UI")]
    public GameObject gunUI;
    public Text textOfAmountOfAmmo;

    [Header("Game Objects")]

    public GameObject CameraHolder;
    public Camera cam;

    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("DATA")]
    public WeaponData weaponData;


    private AudioClip _shootSound;
    private AudioClip _reloadSound;
    private AudioClip _noAmmoSound;

    private float _damage;
    private float _fireRate = 0.1f; // 1f = 1second
    private int _amountOfAmmo;

    private float _recoilX;
    private float _recoilY;
    private float _recoilZ;

    private float _snappiness;
    private float _returnSpeed;


    //Rotations
    private Vector3 _currentRotation;
    private Vector3 _targetRotation;

    private float _nextTimeToFire = 0f;

    private int amountOfAmmoSave;

    private void Awake()
    {
        _shootSound = weaponData.shootSound;
        _reloadSound = weaponData.reloadSound;
        _noAmmoSound = weaponData.noAmmoSound;

        _damage = weaponData.damage;
        _fireRate = weaponData.fireRate;
        _amountOfAmmo = weaponData.amountOfAmmo;

        _recoilX = weaponData.recoilX;
        _recoilY = weaponData.recoilY;
        _recoilZ = weaponData.recoilZ;

        _snappiness = weaponData.snappiness;
        _returnSpeed = weaponData.returnSpeed;
    }
    private void Start()
    {
        amountOfAmmoSave = _amountOfAmmo;
    }

    [PunRPC]
    public void RPC_Shoot(string actor)
    {
        if (Time.time >= _nextTimeToFire && _amountOfAmmo != 0)
        {
            audioSource.PlayOneShot(_shootSound);

            _nextTimeToFire = Time.time + _fireRate;           

            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            ray.origin = cam.transform.position;

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                hit.collider.gameObject
                    .GetComponent<IDamageable>()?
                    .TakeDamage(_damage, actor);

            }
            _amountOfAmmo -= 1;

            audioSource.Stop();
            audioSource.clip = _shootSound;
            audioSource.Play();

            RecoilFire();
        }
        else if (_amountOfAmmo == 0 && audioSource != null)
        {
            audioSource.PlayOneShot(_noAmmoSound);
        }
    }

    public bool TryReload()
    {
        if (_amountOfAmmo == amountOfAmmoSave)
            return false;
        
        Reload();
        return true;
    }
    
    private void Reload()
    {
        _amountOfAmmo = amountOfAmmoSave;
        audioSource.PlayOneShot(_reloadSound);
    }

    public void RecoilFire()
    {
        _targetRotation += new Vector3(_recoilX,
            Random.Range(-_recoilY, _recoilY),
            Random.Range(-_recoilZ, _recoilZ));
    }

    public void UpdateText()
    {
        textOfAmountOfAmmo.text = $"{_amountOfAmmo} / {amountOfAmmoSave}";
    }

    public void UpdateRecoil()
    {
        CameraHolder.transform.localRotation = Quaternion.Euler(_currentRotation);

        _targetRotation = Vector3.Lerp(_targetRotation, Vector3.zero, _returnSpeed * Time.deltaTime);
        _currentRotation = Vector3.Slerp(_currentRotation, _targetRotation, _snappiness * Time.deltaTime);
    }

}
