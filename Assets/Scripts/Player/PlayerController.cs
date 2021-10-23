using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [Header("UI")]
    [SerializeField] GameObject cameraHolder;
    [SerializeField] GameObject UI;
    [SerializeField] private Image scope;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private Image crosshair;
    [SerializeField] private Text healthText;

    [Header("Settings")]
    [SerializeField] float mouseSensitivity;
    [SerializeField] float sprintSpeed;
    [SerializeField] float walkSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] float smoothTime;

    [Header("Items")]
    [SerializeField] private GameObject[] items;

    [HideInInspector] public int itemIndex;
    private int previousItemIndex = -1;

    private float verticalLookRotation;
    private bool grounded;
    private Vector3 smoothMoveVelocity;
    private Vector3 moveAmount;

    private Rigidbody rb;
    public Gun gun { get; private set; }

    private PhotonView PV;

    private const float maxHealth = 100f;
    private float currentHealth = maxHealth;

    private PlayerManager playerManager;

    private bool isScopeOn;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();

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

        if (transform.position.y < -10f)
        {
            Die();
        }

        ItemImageSwicth();
        Shoot();
        HideCrosshair();
        Reload();
        Look();
        Move();
        Jump();
        SwitchItem();
        Scope();
    }

    void Shoot()
    {
        if (Input.GetMouseButton(0))
        {
            gun.RPC_Shoot();
        }
    }

    void HideCrosshair()
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

    void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R) )
        {
            gun.TryReload();
        }
    }

    void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
    }

    void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }

    void EquipItem(int _index)
    {
        if (_index == previousItemIndex)
        {
            return;
        }
        
        itemIndex = _index;
        gun = items[itemIndex].GetComponent<Gun>();

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

    void SwitchItem()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);               
                break;
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

    private void FixedUpdate()
    {
        if (!PV.IsMine)
        {
            return;
        }

        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    public void TakeDamage(float damage)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage)
    {
        if (!PV.IsMine)
        {
            return;
        }

        currentHealth -= damage;

        healthBarImage.fillAmount = currentHealth / maxHealth;
        healthText.text = $"{currentHealth}";

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        playerManager.Die();
    }

    private void Scope()
    {
        if (Input.GetMouseButtonDown(1) && itemIndex == 2 && isScopeOn == false)
        {
            scope.gameObject.SetActive(true);
            isScopeOn = true;
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

    void ItemImageSwicth()
    {
        if (itemIndex == 0)
        {
            items[0].GetComponent<Gun>().gunUI.SetActive(true);
            items[1].GetComponent<Gun>().gunUI.SetActive(false);
            items[2].GetComponent<Gun>().gunUI.SetActive(false);
        }
        else if (itemIndex == 1)
        {
            items[0].GetComponent<Gun>().gunUI.SetActive(false);
            items[1].GetComponent<Gun>().gunUI.SetActive(true);
            items[2].GetComponent<Gun>().gunUI.SetActive(false);
        }
        else if (itemIndex == 2)
        {
            items[0].GetComponent<Gun>().gunUI.SetActive(false);
            items[1].GetComponent<Gun>().gunUI.SetActive(false);
            items[2].GetComponent<Gun>().gunUI.SetActive(true);
        }
    }
}
