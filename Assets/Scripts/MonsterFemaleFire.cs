using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterFemaleFire : Monster
{
    [SerializeField] float radius_to_dash = 5f;

    override protected void FixedUpdate()
    {
        base.FixedUpdate();
        float current_dist = Mathf.Abs(player.transform.position.x - transform.position.x);

        if (current_dist < radius_to_dash && Mathf.Abs(player.transform.position.y - transform.position.y) < 1f)
        {
            if (Time.time - timer_calculation > max_timer_calculation &&
                !_animator.GetCurrentAnimatorStateInfo(0).IsName("MFH Hurt") &&
                !_animator.GetCurrentAnimatorStateInfo(0).IsName("MFH Attack") && !is_dead && is_grounded)
            {
                isAttacking = true;
                _animator.SetTrigger("Attack");
                
                timer_calculation = Time.time;

                StartCoroutine(dash_towards_player(FramesDelayBeforeAttackStartsToCountAsDmg));
            }
        }
        if (is_dead)
        {
            StopCoroutine("dash_towards_player");
        }
    }

    IEnumerator dash_towards_player(float time)
    {
        yield return new WaitForSecondsRealtime(time);

        if (!clips[1].isPlaying)
            clips[1].Play();
        _rigidbody.velocity = new Vector2((int)transform.localScale.x * 15f, 0);

        StartCoroutine(set_isAttacking_false(FramesAfterAttack));
    }


    override protected void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (isAttacking)
        {
            if (collision.CompareTag("FireFloor"))
            {
                StartCoroutine(create_firetrail(collision.gameObject, fireTrail_lifetime));
            }
        }
        if (isAttacking)
        {
            if (collision.CompareTag("Player"))
                StartCoroutine(process_attack(0));

        }
    }
}
