using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [Header("UI")]
    [SerializeField] GameObject cameraHolder;
    [SerializeField] GameObject UI;
    [SerializeField] private Image scope;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private Image crosshair;
    [SerializeField] private Text healthText; 

    [Header("Items")]
    [SerializeField] private GameObject[] items;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip zoomSound;
    [SerializeField] private AudioClip hurt;

    [Header("Health")]
    [SerializeField] private Image BloodEffect;
    [SerializeField] private float hurtTimer = 0.1f;

    private const float maxHealth = 100f;
    [HideInInspector] public float currentHealth = maxHealth;

    [HideInInspector] public int itemIndex;
    private int previousItemIndex = -1;

    private Rigidbody rb;
    public Weapon gun { get; private set; }
    private PlayerLook playerLook;
    [HideInInspector] public PhotonView PV;  
    private PlayerManager playerManager;
    private PLayerMovement pLayerMovement;
    private WallRun wallRun;

    private bool isScopeOn;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        playerLook = GetComponent<PlayerLook>();
        pLayerMovement = GetComponent<PLayerMovement>();
        wallRun = GetComponent<WallRun>();

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
        TryScope();
        TryHideCrosshair();
        
        TryLook();
        TryMove();
        TryJump();
        GroundCheck();
        TryControlSpeed();
        TryControlDrag();
        TryUpdateSlopeMoveDirection();
        TryCheckWall();
        TryWallRunUpdate();
    }

    private void TryWallRunUpdate()
    {
        wallRun.WallRunUpdate();
    }

    private void TryCheckWall()
    {
        wallRun.CheckWall();
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

        pLayerMovement.MovePlayer();
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
            gun.RPC_Shoot(PV.Owner.NickName);
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
        pLayerMovement.MyInput();
    }

    private void TryLook()
    {
        playerLook.Look();
    }

    private void TryJump()
    {
        pLayerMovement.Jump();
    }

    private void TryControlSpeed()
    {
        pLayerMovement.ControlSpeed();
    }

    private void TryControlDrag()
    {
        pLayerMovement.ControlDrag();
    }

    private void TryUpdateSlopeMoveDirection()
    {
        pLayerMovement.UpdateSlopeMoveDirection();
    }

    private void GroundCheck()
    {
        pLayerMovement.IsGrounded();
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

    public void TakeDamage(float damage)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    private void RPC_TakeDamage(float damage)
    {
        if (!PV.IsMine)
        {
            return;
        }
        currentHealth -= damage;

        //HurtTimer();
        HealthUpdate();

        healthBarImage.fillAmount = currentHealth / maxHealth;
        healthText.text = $"{currentHealth}";

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        playerManager.Die();
    }

    private void TryScope()
    {
        if (Input.GetMouseButtonDown(1) && itemIndex == 2 && isScopeOn == false)
        {
            scope.gameObject.SetActive(true);
            isScopeOn = true;
            audioSource.PlayOneShot(zoomSound);
        }
        else if (Input.GetMouseButtonDown(1) && itemIndex == 2 && isScopeOn == true)
        {
            scope.gameObject.SetActive(false);
            isScopeOn = false;
        }
        else if (itemIndex != 2)
        {
            scope.gameObject.SetActive(false);
            isScopeOn = false;
        }
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

    private void HealthUpdate()
    {
        Color bloodEffectColor = BloodEffect.color;
        bloodEffectColor.a = 1 - (currentHealth / maxHealth);
        BloodEffect.color = bloodEffectColor;
    }

    private IEnumerator HurtTimer()
    {
        BloodEffect.gameObject.SetActive(true);
        audioSource.PlayOneShot(hurt);
        yield return new WaitForSeconds(hurtTimer);
        BloodEffect.gameObject.SetActive(false);
    }
}
