using UnityEngine;

public class Recoil : MonoBehaviour
{
    //Scripts
    [SerializeField] private Gun[] _gunScript;
    [SerializeField] private PlayerController _playerController;

    private int _index;

    void Update()
    {
        _index = _playerController.itemIndex;

        _gunScript[_index]._targetRotation = Vector3.Lerp(_gunScript[_index]._targetRotation, Vector3.zero, _gunScript[_index]._returnSpeed * Time.deltaTime);
        _gunScript[_index]._currentRotation = Vector3.Slerp(_gunScript[_index]._currentRotation, _gunScript[_index]._targetRotation, _gunScript[_index]._snappiness * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(_gunScript[_index]._currentRotation);
    }

    public void RecoilFire()
    {
        _gunScript[_index]._targetRotation += new Vector3(_gunScript[_index]._recoilX,
            Random.Range(-_gunScript[_index]._recoilY, _gunScript[_index]._recoilY),
            Random.Range(-_gunScript[_index]._recoilZ, _gunScript[_index]._recoilZ));
    }
}
