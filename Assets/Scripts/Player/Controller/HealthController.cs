using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class HealthController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float hurtTimer = 0.1f;
    [HideInInspector] public float currentHealth = maxHealth;
    private const float maxHealth = 100f;

    [Header("Effects")]
    [SerializeField] private Image BloodEffect;
    [SerializeField] private Image Vingette;

    [Header("Audio")]
    [SerializeField] private AudioSource healthSource;
    [SerializeField] private AudioClip hurt;

    [Header("UI")]
    [SerializeField] private Image healthBarImage;
    [SerializeField] private Text healthText;

    [PunRPC]
    private void RPC_TakeDamage(float damage, PhotonView PV, PlayerManager playerManager)
    {
        if (!PV.IsMine)
        {
            return;
        }

        currentHealth -= damage;

        if (currentHealth >= 0f - 0.01f)
        {
            HealthUpdate();
            StartCoroutine(HurtFlash());
        }

        if (currentHealth <= 0f)
        {
            playerManager.Die();
        }
    }

    private void HealthUpdate()
    {
        Color bloodEffectColor = BloodEffect.color;
        bloodEffectColor.a = 1 - (currentHealth / maxHealth);
        BloodEffect.color = bloodEffectColor;

        healthText.text = $"{currentHealth}";
        healthBarImage.fillAmount = currentHealth / maxHealth;
    }

    private IEnumerator HurtFlash()
    {
        Vingette.enabled = true;
        healthSource.PlayOneShot(hurt);
        yield return new WaitForSeconds(hurtTimer);
        Vingette.enabled = false;
    }
}
