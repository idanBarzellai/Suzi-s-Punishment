using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : Monster
{
    [SerializeField] float timer_for_sideways_movement = 30f;
    float last_time_sideways_movement= 0;
    bool on_left_side = false;
    bool walking = false;

    [SerializeField] float speed_for_movement = 5f;
    [SerializeField] float x_position_left_side = -6.72f;
    float x_position_right_side;

    CapsuleCollider2D _collider;

    [SerializeField] int dying_time = 3;
    [SerializeField] int wave = 0;
    [SerializeField] float delay_for_summoing_wave = 5f;
    [SerializeField] float last_time_summoned = 0;
    
    [SerializeField] float attack_animation_time = 3f;

    bool is_summoning;
    [HideInInspector]
    public MonsterWave current_wave;

    protected override void Start()
    {
        player = FindObjectOfType<LevelLoader>().get_avatars()[PlayerPrefs.GetInt("Avatar")];
       /* player = GameObject.FindGameObjectsWithTag("Player")[0];*/
        player_collider = player.GetComponent<BoxCollider2D>();
        _collider = GetComponent<CapsuleCollider2D>();
        x_position_right_side = transform.position.x;

        player_controller = player.GetComponent<Controls>();
        
        lvl_mngr_ref = FindObjectOfType<LevelManager>();
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _hp_slider = GetComponentInChildren<Slider>();
        
        enemy_health = PlayerPrefs.GetFloat("AvatarDmg", 50f) * 30;
        Speed = 10f;
        last_time_touching_player = -3f;
        delay_time_attacking_player = 1.5f;
        last_time_chaging_facing_dir = -3f;
        last_time_summoned = Time.time;
        last_time_sideways_movement = Time.time;
    }

    protected override void FixedUpdate()
    {
        if(!is_summoning && walking || Time.time - last_time_sideways_movement > timer_for_sideways_movement)
        {
            last_time_sideways_movement = Time.time;
            float step = speed_for_movement * Time.deltaTime;
            walking = true;
            _animator.SetTrigger("Run");
            if (!clips[2].isPlaying)
                clips[2].Play();
            if (on_left_side)
            { 
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(x_position_right_side, transform.position.y), step);
                
                if (Vector3.Distance(transform.position, new Vector3(x_position_right_side, transform.position.y, 0)) < 0.1f)
                {
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                    walking = false;
                    on_left_side = false;
                }
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(x_position_left_side, transform.position.y), step);

                if (Vector3.Distance(transform.position, new Vector3(x_position_left_side, transform.position.y, 0)) < 0.1f)
                {
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                    walking = false;
                    on_left_side = true;
                }   
            }
        }

        if (!walking && !is_summoning)
            _animator.SetTrigger("Idel");

        /*if (Mathf.Abs(player.transform.position.y - transform.position.y) < 4f && Mathf.Abs(player.transform.position.x - transform.position.x) < 2.5f)
        {
            player_controller.playerCanAttackMe(this);
        }*/

        if(Time.time - last_time_summoned > delay_for_summoing_wave)
        {
            is_summoning = true;
            Invoke("tickNextWave" , attack_animation_time);
            _animator.SetTrigger("Attack");
            if (!clips[1].isPlaying)
                clips[1].Play();
            last_time_summoned = Time.time;
        }
        if(GameObject.FindGameObjectsWithTag("Enemy").Length <= 0)
        {
            Debug.Log("Going to credits");
            // End level go to credits the go back to login with back 
        }
    }

    public void tickNextWave()
    {
        if (lvl_mngr_ref.transform.childCount > wave)
        {
           
            current_wave = lvl_mngr_ref.transform.GetChild(wave++).GetComponent<MonsterWave>().spawnWave(lvl_mngr_ref);
        }
        else
        {
            // what happens when wavs end
        }
        is_summoning = false;
    }

    public void checkDone()
    {
        if (current_wave.checkDone())
            tickNextWave();
    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && (Time.time - last_time_touching_player > delay_time_attacking_player))
        {
            last_time_touching_player = Time.time;
            player_controller.gotAttacked((int)transform.localScale.x *-1, 24, 10f, 7.5f);
        }
    }

    override protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (walking)
        {
            if (collision.CompareTag("FireFloor"))
            {
                StartCoroutine(create_firetrail(collision.gameObject, fireTrail_lifetime));
            }
        }
    }

    public override void gotAttacked(int x_push_dir)
    {
        if (!is_hurt)
        {
            is_hurt = true;
            Invoke("set_is_hurt", .5f);
            getDmg(PlayerPrefs.GetFloat("AvatarDmg", 50f)); 
        }
    }
    public override void getDmg(float dmg)
    {
        enemy_health -= dmg;

        _hp_slider.value = enemy_health / EnemyHealth;
        if (enemy_health < 1)
        {
            is_dead = true;
            _animator.SetTrigger("Die");
            if (!clips[0].isPlaying)
                clips[0].Play();
            GameObject.FindObjectOfType<LevelLoader>().boss_is_dead();
            Destroy(gameObject, dying_time) ;
            lvl_mngr_ref.checkDone();

        }
    }

    void set_is_hurt()
    {
        is_hurt = false;
    }
}
