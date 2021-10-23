using UnityEngine;

public class Recoil : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;

    void Update()
    {
        Gun currentgun = _playerController.gun;
        currentgun._targetRotation = Vector3.Lerp(currentgun._targetRotation, Vector3.zero, currentgun._returnSpeed * Time.deltaTime);
        currentgun._currentRotation = Vector3.Slerp(currentgun._currentRotation, currentgun._targetRotation, currentgun._snappiness * Time.deltaTime);
        
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
