using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [Header("UI")]

    [SerializeField] GameObject cameraHolder;
    [SerializeField] GameObject UI;
    [SerializeField] private Image scope;
    [SerializeField] private Image crosshair;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private Text healthText;

    [Header("Settings")]

    public float mouseSensitivity;
    [SerializeField] float sprintSpeed;
    [SerializeField] float walkSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] float smoothTime;

    [Header("Items")]

    [SerializeField] private GameObject[] items;

    [Header("Audio")]

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip zoomSound;
    public AudioClip shootSound;
    [SerializeField] private AudioSource healthSource;
    [SerializeField] private AudioClip hurt;

    [Header("Health")]

    [SerializeField] private Image BloodEffect;
    [SerializeField] private Image Vingette;
    [SerializeField] private float hurtTimer = 0.1f;

    private const float maxHealth = 100f;

    [HideInInspector] public float currentHealth = maxHealth;

    [HideInInspector] public int itemIndex;

    private int previousItemIndex = -1;

    private float verticalLookRotation;
    private bool grounded;

    private Vector3 smoothMoveVelocity;
    private Vector3 moveAmount;

    private Rigidbody rb;
    public Weapon gun { get; private set; }

    [HideInInspector] public PhotonView PV;  

    private PlayerManager playerManager;

    private Lantern lantern;

    [SerializeField] private AWPScope awpScope;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        lantern = GetComponent<Lantern>();
        awpScope.normalFOV = GetComponentInChildren<Camera>().fieldOfView;

        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    private void Start()
    {
        if (PV.IsMine)
        {
            EquipItem(0);
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("EnemyLayer");

            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
            Destroy(UI);
        }
    }

    private void Update()
    {
        if (!PV.IsMine)
        {
            return;
        }

        TryDieInVoid();

        TryUpdateRecoil();
        TryUpdateAmmoText();

        TrySwitchItem();
        TrySwicthItemImage();

        TryShoot();
        TryReload();
        TryHideCrosshair();
        TryLook();
        TryMove();
        TryJump();

        TryOnLatern();
        TryToScope();
    }

    private void TryToScope()
    {
        if (Input.GetButtonDown("Fire2") && itemIndex == 2)
        {
            awpScope.TryToScope();
        }
        else if(awpScope.isScoped && itemIndex != 2)
        {
            awpScope.OnUnscoped();
        }
    }

    private void TryOnLatern()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            lantern.LaternOn();
        }
    }

    private void TryDieInVoid()
    {
        if (transform.position.y < -10f)
        {
            Die();
        }
    }

    private void FixedUpdate()
    {
        if (!PV.IsMine)
        {
            return;
        }

        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    private void TryUpdateRecoil()
    {
        gun.UpdateRecoil();
    }

    private void TryUpdateAmmoText()
    {
        gun.UpdateText();
    }

    private void TryShoot()
    {
        if (Input.GetMouseButton(0))
        {
            gun.Shoot();
        }
    }

    private void TryHideCrosshair()
    {
        if (itemIndex == 2)
        {
            crosshair.gameObject.SetActive(false);
        }
        else
        {
            crosshair.gameObject.SetActive(true);
        }
    }

    private void TryReload()
    {
        if (Input.GetKeyDown(KeyCode.R) )
        {
            gun.TryReload();
        }
    }

    private void TryMove()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
    }

    private void TryLook()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }

    private void EquipItem(int _index)
    {
        if (_index == previousItemIndex)
        {
            return;
        }
        
        itemIndex = _index;
        gun = items[itemIndex].GetComponent<Weapon>();

        items[itemIndex].SetActive(true);

        if (previousItemIndex != -1)
        {
            items[previousItemIndex].SetActive(false);
        }

        previousItemIndex = itemIndex;

        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    private void TrySwitchItem()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            if (itemIndex >= items.Length - 1)
            {
                EquipItem(0);
            }
            else
            {
                EquipItem(itemIndex + 1);
            }
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            if (itemIndex <= 0)
            {
                EquipItem(items.Length - 1);
            }
            else
            {
                EquipItem(itemIndex - 1);
            }
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!PV.IsMine && targetPlayer == PV.Owner)
        {
            EquipItem((int)changedProps["itemIndex"]);
        }
    }

    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }

    private void Die()
    {
        playerManager.Die();
    }
    private IEnumerator IsScoped()
    {
        yield return new WaitForSeconds(.15f);
    }

    private void TrySwicthItemImage()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (i != itemIndex)
            {
                items[i].GetComponent<Weapon>().gunUI.SetActive(false);
            }
            else if (i == itemIndex)
            {
                items[i].GetComponent<Weapon>().gunUI.SetActive(true);
            }
        }
    }


    public void TakeDamage(float damage)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    public void PlaySound()
    {
        audioSource.PlayOneShot(shootSound);
    }

    [PunRPC]
    private void RPC_TakeDamage(float damage)
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
            Die();
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
