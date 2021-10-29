using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Sound Elements")]
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip noAmmoSound;

    [Header("Game Objects")]
    public GameObject bulletImpact;

    [Header("Main Settings")]
    public string weaponName;
    public float damage;
    public float fireRate;
    public int amountOfAmmo;

    [Header("Recoil Values")]

    public float recoilX;
    public float recoilY;
    public float recoilZ;

    [Header("Recoil Settings")]

    public float snappiness;
    public float returnSpeed;
}