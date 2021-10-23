using UnityEngine;

public class Recoil : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;

    void Update()
    {
        Gun currentgun = _playerController.gun;
        
        transform.localRotation = Quaternion.Euler(currentgun._currentRotation);
    }

    public void RecoilFire()
    {
        Gun currentgun = _playerController.gun;
        
        currentgun._targetRotation += new Vector3(currentgun._recoilX,
            Random.Range(-currentgun._recoilY, currentgun._recoilY),
            Random.Range(-currentgun._recoilZ, currentgun._recoilZ));
    }
}
