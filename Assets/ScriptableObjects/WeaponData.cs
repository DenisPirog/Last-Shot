using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("UI Elements")]
    public GameObject gunUI;
    public Text textOfAmmo;

    [Header("Sound Elements")]
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip noAmmoSound;
    public AudioSource audioSource;

    [Header("Game Objects")]
    public GameObject weaponModel;
    public Camera playerCamera;

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