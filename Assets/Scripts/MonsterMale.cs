using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMale : Monster
{
    [SerializeField] GameObject circleRadius;
    float rad_for_scream_x = 3.5f;
    float rad_for_scream_y = 3.5f;
    float current_rad_x;
    float current_rad_y;

    override protected void FixedUpdate()
    {
        base.FixedUpdate();
        current_rad_x = Mathf.Abs(player.transform.position.x - transform.position.x); 
        current_rad_y = Mathf.Abs(player.transform.position.y - transform.position.y);
        
        if (current_rad_x < rad_for_scream_x && current_rad_y < rad_for_scream_y)
        {          
            if (Time.time - timer_calculation > max_timer_calculation &&
                !_animator.GetCurrentAnimatorStateInfo(0).IsName("MFH Hurt") &&
                !_animator.GetCurrentAnimatorStateInfo(0).IsName("MFH Attack") && !is_dead && is_grounded)
            {
                isAttacking = true;
                _animator.SetTrigger("Attack");
                circleRadius.SetActive(true);
                timer_calculation = Time.time;
                StartCoroutine(scream_attack(FramesDelayBeforeAttackStartsToCountAsDmg));
            }
        }
        
        
        if (is_dead)
        {
            StopCoroutine("scream_attack");
        }
    }
    IEnumerator scream_attack(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        /*while (time-- > 0)
            yield return null;*/
        if (!clips[1].isPlaying)
            clips[1].Play();
        current_rad_x = Mathf.Abs(player.transform.position.x - transform.position.x);
        current_rad_y = Mathf.Abs(player.transform.position.y - transform.position.y);
        if (current_rad_x < rad_for_scream_x && current_rad_y < rad_for_scream_y)
            player_controller.gotAttacked((int)Mathf.Sign(player.transform.position.x - this.transform.position.x), 10, 15f, 7.5f);
        
        StartCoroutine(set_isAttacking_false(FramesAfterAttack));
        Invoke("stop_circle", 2f);
    }

    void stop_circle()
    {
        circleRadius.SetActive(false);
    }

}