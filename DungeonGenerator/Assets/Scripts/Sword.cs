using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    PlayerController player;

    bool isAttacking;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.Instance;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.GetComponent<Enemy>() != null && isAttacking)
        //{
        //    collision.gameObject.GetComponent<Enemy>().ApplyKnockback(25, player.transform.position);
        //}

        //if(player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("1st Attack"))
        //{
        //    collision.gameObject.GetComponent<Enemy>().TakeDamage(player.GetLightAttackDamages()[0]);
        //}
        //else if (player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("2nd Attack"))
        //{
        //    collision.gameObject.GetComponent<Enemy>().TakeDamage(player.GetLightAttackDamages()[1]);
        //}
        //else if (player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("3rd Attack"))
        //{
        //    collision.gameObject.GetComponent<Enemy>().TakeDamage(player.GetLightAttackDamages()[2]);
        //}
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (!player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Idle"))
    //    {
    //        isAttacking = true;
    //    }
    //    else
    //    {
    //        isAttacking = false;
    //    }
    //}
}
