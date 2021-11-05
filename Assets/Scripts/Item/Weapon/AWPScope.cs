using System.Collections;
using UnityEngine;

public class AWPScope : MonoBehaviour
{
    public Animator animator;
    public GameObject weaponCamera;
    public GameObject scopeOverlay;
    public Camera mainCamera;
    [HideInInspector] public bool isScoped = false;
    public float scopedFOV = 15f;
    private float normalFOV;
    public PlayerController player;
    public float scopeSensitivity;

    private void Start()
    {
        normalFOV = mainCamera.fieldOfView;
    }

    public void TryToScope()
    {
        isScoped = !isScoped;
        animator.SetBool("IsScoped", isScoped);

        if (isScoped)
            StartCoroutine(OnScoped());
        else
            OnUnscoped();
    }

    public void OnUnscoped()
    {
        scopeOverlay.SetActive(false);
        weaponCamera.SetActive(true);

        mainCamera.fieldOfView = normalFOV;
        player.mouseSensitivity = 2f;
        isScoped = false;
    }
    private IEnumerator OnScoped()
    {
        yield return new WaitForSeconds(.15f);
        scopeOverlay.SetActive(true);
        weaponCamera.SetActive(false);

        mainCamera.fieldOfView = scopedFOV;
        player.mouseSensitivity = scopeSensitivity;
    }
}
