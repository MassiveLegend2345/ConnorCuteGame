using UnityEngine;

public class HandAttackUI : MonoBehaviour
{
    public Animator handAnimator;
    private bool isAttacking = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            isAttacking = true;
            handAnimator.Play("HandAttack");
        }
    }

    // You can call this from an Animation Event at the end of the animation
    public void EndAttack()
    {
        isAttacking = false;
    }
}