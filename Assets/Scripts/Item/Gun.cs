using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float damage;
    [SerializeField] private float fireRate = 0.1f; // 1f = 1second
    [SerializeField] private float amountOfAmmo = 5f;
    [SerializeField] private Text textOfAmountOfAmmo;

    private float _nextTimeToFire = 0f;
    private float _amountOfAmmoSave;

    private void Awake()
    {
        _amountOfAmmoSave = amountOfAmmo;
    }
    private void Update()
    {
        textOfAmountOfAmmo.text = $"{amountOfAmmo} / {_amountOfAmmoSave}";

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }
    public void Shoot()
    {
        if(Time.time >= _nextTimeToFire && amountOfAmmo != 0)
        {
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
        }     
    }

    private void Reload()
    {
        amountOfAmmo = _amountOfAmmoSave;
    }
}
