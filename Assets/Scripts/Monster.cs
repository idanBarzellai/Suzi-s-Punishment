using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    private float EnemyHitPushForce = 5.0f;
    [SerializeField] protected float EnemyHealth = 800f;
    protected float enemy_health = -1f;
    private float JumpHeight = 1f;
    [SerializeField] float JumpForce = 20f;
    protected float Speed = 1.5f;
    float MinTimeForAttack = 5f;
    float MaxTimeForAttack = 10f;

    protected bool isAttacking = false;
    protected float last_time_touching_player;
    protected float delay_time_attacking_player = 1f;
    protected bool touched_player;
    /*protected float range_for_attack = 1.5f;*/
    protected float last_time_chaging_facing_dir;
    protected float delay_time_chaging_facing_dir = 2f;

    protected float timer_calculation = 0;
    protected float max_timer_calculation = 15;
    protected float distance_to_stop = 2f;
    protected float jump_timer = 0f;
    protected float JumpEveryHowManySeconds = 5f;
    protected int target_x = 0;
    protected float prev_x = 0f;
    protected bool is_grounded = false;
    /*public float StaticDistanceBetweenEnemeis = 0f;
    protected float move_away_dis = 3f;*/
    protected float arrived_to_scene = 0;

    [SerializeField] protected float FramesDelayBeforeAttackStartsToCountAsDmg = 0;
    [SerializeField] protected float FramesAfterAttack = 0;
    [SerializeField] protected float fireTrail_lifetime = 4;

    protected bool is_dead = false;
    protected bool is_hurt;

    protected const float MOVEMENT_COUNTER_BASE = 5f;
    protected float movement_counter;

    [HideInInspector]
    public LevelManager lvl_mngr_ref;

    /*List<Monster> other_monsters = new List<Monster>();
*/
    protected GameObject player;
    protected BoxCollider2D player_collider;
    protected Controls player_controller;
    //[SerializeField] protected CircleCollider2D detection_attack_area;

    protected Animator _animator;
    protected Rigidbody2D _rigidbody;
    protected CircleCollider2D _collider;
    protected Slider _hp_slider;

    protected Collider2D my_ground;

    [SerializeField] protected AudioSource[] clips;

    virtual protected void Start()
    {
        arrived_to_scene = Time.time;
        timer_calculation = Time.time;
        max_timer_calculation = Random.Range(MinTimeForAttack, MaxTimeForAttack);
        JumpEveryHowManySeconds = JumpEveryHowManySeconds * Random.Range(.5f, 1.5f);

        player = GameObject.FindGameObjectsWithTag("Player")[0];
        player_collider = player.GetComponent<BoxCollider2D>();
        _collider = GetComponent<CircleCollider2D>();
        Physics2D.IgnoreCollision(_collider, player_collider);

        player_controller = player.GetComponent<Controls>();
        //float rad = GetComponent<CircleCollider2D>().radius + 1.5f; // TODO: replace with distance to weapon reach attack
        distance_to_stop = GetComponent<CircleCollider2D>().radius;

        lvl_mngr_ref = FindObjectOfType<LevelManager>();
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _hp_slider = GetComponentInChildren<Slider>();

        enemy_health = EnemyHealth;
        last_time_touching_player = -3f;
        last_time_chaging_facing_dir = -3f;

        /*foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
            if (obj != gameObject)
                other_monsters.Add(obj.GetComponent<Monster>());*/

        /*if (StaticDistanceBetweenEnemeis > 0f)
            move_away_dis = StaticDistanceBetweenEnemeis;
        else
            move_away_dis = (_collider.bounds.size.x + _collider.bounds.size.y) / 2f;*/

        movement_counter = new_movement_counter();

        /*if (PlayerPrefs.GetInt("Avatar", 0) == 1)
            range_for_attack = 2f;
*/
        foreach (AudioSource a in clips)
        {
            a.ignoreListenerPause = true;
        }

    }

    virtual protected void FixedUpdate()
    {
        _hp_slider.transform.localScale = new Vector3(Mathf.Sign(transform.localScale.x), 1.0f, 1.0f);
        /*foreach (Monster other in other_monsters)
        {
            if (!other)
            {
                other_monsters.Remove(other);
                break;
            }

            float x = (other.gameObject.transform.position.x - transform.position.x);
            float y = (other.gameObject.transform.position.y - transform.position.y);

            *//*if (x * x + y * y < move_away_dis)
            {
                other.target_x = 0;
                target_x = 0;

                Vector3 dir = new Vector3(x, y, 0f);
                dir = dir.normalized * Time.fixedDeltaTime * 3f;
                other.gameObject.transform.position += dir;
                transform.position -= dir;
            }

            Physics2D.IgnoreCollision(_collider, other.GetComponent<Collider2D>());*//*
        }*/

        if (prev_x == transform.position.x) target_x = 0;
        prev_x = transform.position.x;

        /*if (detection_area.IsTouching(player_collider) && Mathf.Abs(player.transform.position.y - transform.position.y) < 1f)
        {
            timer_calculation += Time.fixedDeltaTime;

            StartCoroutine(is_being_attacked());  //player_controller.playerCanAttackMe(this);
            if (verify_facing(player.gameObject)) //&& !_animator.GetCurrentAnimatorStateInfo(0).IsName("MFH Hurt"))
            {
                if (timer_calculation > max_timer_calculation && !_animator.GetCurrentAnimatorStateInfo(0).IsName("MFH Hurt") && !_animator.GetCurrentAnimatorStateInfo(0).IsName("MFH Attack"))
                {
                    
                    timer_calculation = 0f;
                    //_animator.Play("MFH Attack");
                    isAttacking = true; // idan cahnges - for attacking anim
                    Debug.Log("attack loop" + this.gameObject.name);
                    _animator.SetTrigger("Attack");
                    StartCoroutine(process_attack(FramesDelayBeforeAttackStartsToCountAsDmg));
                }

            }
        }*/

        jump_timer += Time.fixedDeltaTime;
        bool jump = false;
        if (jump_timer > JumpEveryHowManySeconds)
        {
            jump = true;
        }

        if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("MFH Hurt") && !isAttacking)
        {

            if (player_controller.ground == my_ground)
            {

                if (!(Mathf.Abs(transform.position.x - player.transform.position.x) < distance_to_stop)) 
                {

                    target_x = 0;
                    if (transform.position.x < player.transform.position.x)
                        add_forces(1, false, is_landing_or_idling());
                    else
                        add_forces(-1, false, is_landing_or_idling());
                }
            }
            else
            {
                movement_counter -= Time.fixedDeltaTime;

                if (movement_counter < 0)
                {
                    if (is_grounded && _animator.GetCurrentAnimatorStateInfo(0).IsName("MFH Idel"));
                        _animator.Play("MFH Idel");// _animator.SetTrigger("Idel");
                    if (movement_counter < -MOVEMENT_COUNTER_BASE * .5f)
                        movement_counter = new_movement_counter();
                }
                else if (jump)
                {
                    add_forces((int)transform.localScale.x, jump, is_landing_or_idling()); // _animator.GetCurrentAnimatorStateInfo(0).IsName("MFH Idel"));

                }
                else
                {
                    if (my_ground)
                    {

                        if (is_cant_go_right())
                        {

                            target_x = -1;
                        }
                        else if (is_cant_go_left())
                        {

                            target_x = 1;
                        }
                        else if (target_x == 0)
                        {

                            target_x = (int)Mathf.Sign(Random.Range(-1f, 1f));
                        }

                        if (!is_grounded)
                            target_x = (int)transform.localScale.x;

                        add_forces(target_x, false, is_landing_or_idling()); // _animator.GetCurrentAnimatorStateInfo(0).IsName("MFH Idel"));
                    }
                }

            }

        }
        //isAttacking = _animator.GetCurrentAnimatorStateInfo(0).IsName("MFH Attack");
        is_hurt = _animator.GetCurrentAnimatorStateInfo(0).IsName("MFH Hurt");

        /*if (!player_controller.is_player_attacking_now() &&
            player_controller.is_player_grounded() &&
            player_controller.verify_facing(this.gameObject) &&
            !this.is_dead && !this.is_hurt())
        {
            if ((Time.time - arrived_to_scene > 1.5f) && Vector2.Distance(player.transform.localPosition, transform.localPosition) < range_for_attack)
            {
                player_controller.playerCanAttackMe(this);
            }
        }*/

    }

    virtual protected void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (!is_grounded || _animator.GetCurrentAnimatorStateInfo(0).IsName("MFH Jump"))
                _animator.SetTrigger("Land");
            my_ground = collision.collider;
            is_grounded = true;
        }

        if (collision.collider.CompareTag("Walls"))
        {
            collision.collider.GetComponent<Animator>().SetTrigger("Move");
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.collider == _collider) Debug.Break();
            else
            {
                Physics2D.IgnoreCollision(_collider, collision.collider, true);
            }

        }
        

    }

    protected void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.collider == _collider) Debug.Break();

        }
        
    }

    virtual protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && player_controller.is_player_grounded())
        {
            touched_player = true;
            InvokeRepeating("touching_player", 0, 1.5f);
        }
    }

    virtual protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            touched_player = false;
            CancelInvoke("touching_player");
        }
    }

    virtual protected void OnTriggerStay2D(Collider2D collision)
    {
        /*if(collision.CompareTag("Player") )
        {
            touching_player();
        }*/
    }

    void touching_player()
    {
        /*if (Time.time - last_time_touching_player > delay_time_attacking_player &&
            player_controller.is_player_grounded()) */{ 
            //last_time_touching_player = Time.time;
            player_controller.gotAttacked((int)transform.localScale.x, 4, 10f, 5f);
        }
    }


    protected IEnumerator process_attack(float time)
    {
        /*while (time-- > 0)
            yield return null;*/
        yield return new WaitForSecondsRealtime(time);

        if (verify_facing(player.gameObject))
        {
            player_controller.gotAttacked((int)transform.localScale.x, 24, 15f, 7.5f);
        }
        StartCoroutine(set_isAttacking_false(FramesAfterAttack));
    }

    protected float new_movement_counter() { return Random.Range(MOVEMENT_COUNTER_BASE * .5f, MOVEMENT_COUNTER_BASE * 1.5f); }
    
/*    protected IEnumerator is_being_attacked(int wait_time)
    {
        player_controller.playerCanAttackMe(this);
        yield return new WaitForSecondsRealtime(wait_time);
        
    }*/

    protected bool is_cant_go_right() {
        if (Time.time - last_time_chaging_facing_dir > delay_time_chaging_facing_dir)
        {
            last_time_chaging_facing_dir = Time.time;
            return true;
        }
        return transform.position.x > my_ground.transform.position.x + my_ground.bounds.extents.x; }

    protected bool is_cant_go_left() {
        if (Time.time - last_time_chaging_facing_dir > delay_time_chaging_facing_dir)
        {
            last_time_chaging_facing_dir = Time.time;
            return true;
        }
        return transform.position.x < my_ground.transform.position.x - my_ground.bounds.extents.x; }

    protected void add_forces(int x, bool jump,bool on_floor_normally)
    {
        if (!isAttacking && !is_dead)// && on_floor_normally) // on floor normall check
        {  
            transform.localScale = new Vector3(x, 1, 1);

            if (jump)
            {
                _animator.SetTrigger("Jump");//_animator.Play("MFH Jump");
                Vector2 vec2 = new Vector2(x * Mathf.Abs(JumpHeight - 1.0f) * .5f, JumpHeight);
                vec2.Normalize();
                _rigidbody.velocity = vec2 * JumpForce;
                jump_timer = 0f;
                is_grounded = false;
            }
            else
            {
                if (is_grounded && !_animator.GetCurrentAnimatorStateInfo(0).IsName("MFH Run"))
                    _animator.SetTrigger("Run");//_animator.Play("MFH Run");
                transform.position = new Vector2(transform.position.x + (x * Speed * Time.fixedDeltaTime), transform.position.y);
            }
        }
    }

    protected bool verify_facing(GameObject other)
    {
        if (other.transform.position.x < transform.position.x && transform.localScale.x < 0)
            return true;
        else if (other.transform.position.x > transform.position.x && transform.localScale.x > 0)
            return true;
        else if (Vector2.Distance(transform.position, other.transform.position) < 1f)
            return true;
        return false;
    }

    public virtual void gotAttacked(int x_push_dir)
    {
        if (!get_is_hurt() && player_controller.is_player_attacking_now())
        {
            is_hurt = true;
            
            _animator.SetTrigger("Hurt");
            _rigidbody.velocity = new Vector2(x_push_dir * EnemyHitPushForce, EnemyHitPushForce * .5f);
            getDmg(PlayerPrefs.GetFloat("AvatarDmg", 50f));
        }

        /*getDmg(PlayerPrefs.GetFloat("AvatarDmg", 50f)); */
    }

    public virtual void getDmg(float dmg)
    {
        enemy_health -= dmg;

        _hp_slider.value = enemy_health / EnemyHealth;
        if (enemy_health < 1)
        {
            is_dead = true;
            _animator.SetTrigger("Die");
            if(!clips[0].isPlaying)
                clips[0].Play();
            Destroy(gameObject, 2f);
            lvl_mngr_ref.checkDone();
            
        }
    }

    protected IEnumerator set_isAttacking_false(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        /*while (time-- > 0)
            yield return null;*/
        isAttacking = false;
    }

    public IEnumerator create_firetrail(GameObject obj, float time)
    {
        obj.GetComponent<SpriteRenderer>().enabled = true;
        obj.GetComponent<FireTrail>().enabled = true;
        obj.GetComponent<FireTrail>().set_active(true);
        yield return new WaitForSecondsRealtime(time);
        obj.GetComponent<SpriteRenderer>().enabled = false;
        obj.GetComponent<FireTrail>().enabled = false;
        obj.GetComponent<FireTrail>().set_active(false);
    }

    public bool get_is_hurt()
    {
        return is_hurt;
    }
    public bool get_isAttacking()
    {
        return isAttacking;
    }

    public bool is_landing_or_idling()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).IsName("MFH Idel") || _animator.GetCurrentAnimatorStateInfo(0).IsName("MFH Land");
    }

    public bool get_is_dead()
    {
        return is_dead;
    }
}
