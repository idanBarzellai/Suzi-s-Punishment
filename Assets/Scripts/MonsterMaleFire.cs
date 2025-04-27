using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMaleFire : Monster
{
    [SerializeField] GameObject fireball;
    

    override protected void FixedUpdate()
    {
        base.FixedUpdate();
        
         if (Time.time - timer_calculation > max_timer_calculation &&
                !_animator.GetCurrentAnimatorStateInfo(0).IsName("MFH Hurt") &&
                !_animator.GetCurrentAnimatorStateInfo(0).IsName("MFH Attack") && !is_dead && is_grounded)
            {
                isAttacking = true;
                _animator.SetTrigger("Attack");
                
                timer_calculation = Time.time;
                StartCoroutine(fireball_speical_attack(FramesDelayBeforeAttackStartsToCountAsDmg));
            }

        if (is_dead)
        {
            StopCoroutine("fireball_speical_attack");
        }
    }
    IEnumerator fireball_speical_attack(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        /*while (time-- > 0)
            yield return null;*/
        if (!clips[1].isPlaying)
            clips[1].Play();
        Instantiate(fireball, transform.position, Quaternion.identity);
        StartCoroutine(set_isAttacking_false(FramesAfterAttack));
        
    }

    public void start_fireball_firetrail(GameObject ground)
    {
        StartCoroutine(create_firetrail(ground, fireTrail_lifetime));
    }
}
