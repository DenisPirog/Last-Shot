using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animations : MonoBehaviour
{
   
  

   
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControll))
        {
            Crouch();
        
        }
        if (Input.GetKeyUp(KeyCode.LeftControll))
        {
            UnCrouch();

        }
    }
    private void UnCrouch()
    {
        animator.SetBoll(name: "isCrouching", value: false)



    }
    private void Crouch()
    {
        if (Physics.Raycast(origin: groundCheckerTransform.position, direction: Vector3.down, maxDistance: 0.2f, (int)notPlayerMask))
        { 
         animator.SetBoll(name:"isCrouching",value:true)
        
        
        }

    }
}
