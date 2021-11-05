using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Weapon : MonoBehaviour
{ 
    [Header("UI")]
    public GameObject gunUI;
    public Text textOfAmountOfAmmo;

    [Header("Game Objects")]

    public GameObject CameraHolder;
    public Camera cam;

    [Header("Particle System")]
    public ParticleSystem muzzleFlash;

    [Header("DATA")]
    public WeaponData weaponData;

    private AudioClip _shootSound;
    private AudioClip _reloadSound;
    private AudioClip _noAmmoSound;

    private GameObject bulletImpact;

    private float _damage;
    private float _fireRate = 0.1f; // 1f = 1second
    private int _amountOfAmmo;

    private Vector3 _recoilValues;

    private float _snappiness;
    private float _returnSpeed;

    //Rotations
    private Vector3 _currentRotation;
    private Vector3 _targetRotation;

    private float _nextTimeToFire = 0f;

    private int amountOfAmmoSave;

    private AudioSource audioSource;

    private void Awake()
    {
        LoadWeaponData();
    }

    private void Start()
    {
        amountOfAmmoSave = _amountOfAmmo;
        audioSource = GetComponent<AudioSource>();
    }

    public void Shoot(PhotonView PV)
    {
        if (Time.time >= _nextTimeToFire && _amountOfAmmo != 0)
        {
            PV.RPC("PlaySound", RpcTarget.All);

            if(muzzleFlash != null)
                muzzleFlash.Play();

            _nextTimeToFire = Time.time + _fireRate;           

            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            ray.origin = cam.transform.position;

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                hit.collider.gameObject.GetComponent<PlayerController>()?.TakeDamage(_damage);
                _amountOfAmmo -= 1;
                RecoilFire();
                if (hit.collider.gameObject.GetComponent<PlayerController>() == null)
                {
                    GameObject impact = Instantiate(bulletImpact, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(impact, 3f);
                }              
            }
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
        _targetRotation += new Vector3(_recoilValues.x,
            Random.Range(-_recoilValues.y, _recoilValues.y),
            Random.Range(-_recoilValues.z, _recoilValues.z));
    }

    private void LoadWeaponData()
    {
        _shootSound = weaponData.shootSound;
        _reloadSound = weaponData.reloadSound;
        _noAmmoSound = weaponData.noAmmoSound;

        bulletImpact = weaponData.bulletImpact;

        _damage = weaponData.damage;
        _fireRate = weaponData.fireRate;
        _amountOfAmmo = weaponData.amountOfAmmo;

        _recoilValues = weaponData.recoilValues;

        _snappiness = weaponData.snappiness;
        _returnSpeed = weaponData.returnSpeed;
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
