using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Controls : MonoBehaviour
{
    const int basic_hp_for_reset = 350;
    int MAX_HP = 350;

    float[] FramesDelayBeforeAttackStartsToCountAsDmg = { 0, 0, 0 };
    int AttacksTilSpecial = 6;
    int attack_counter = 0;
    float range_for_attack = 1.5f;

    public GameObject HealthBarParentObject;
    public Slider HealthBarSlider;
    public Text MaxHealthText;
    public Text CurrentHealthText;

    [SerializeField] GameObject revive_canvas;
    [SerializeField] GameObject reviveAvater;
    [SerializeField] GameObject coinsAdded;
    [SerializeField] GameObject basicAttackButtons;
    [SerializeField] GameObject totemAttackButtons;

    float JumpAngle = 60f;

    float last_time_got_hurt = 0;
    float delay_time_getting_hurt = 1f;

    bool finished_level_emmobilazied = false;
    [HideInInspector]
    public Collider2D ground = null;
    
    //for controls
    bool is_attacking;
    bool is_dragging = false;
    bool is_grounded = false;
    int attack_num = 1;
    Vector2 target_pos;
    //int target_sign = 0;

    Vector2 escapeButtonPos;

    //current hp
    [SerializeField] int hp;

    //components to register
    Rigidbody2D _rigidbody;
    Animator _animator;
    BoxCollider2D _collider;

    [SerializeField] AudioSource[] clips;

    void Start()
    {
        // grab necessary components
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<BoxCollider2D>();

        MAX_HP = PlayerPrefs.GetInt("MaxHp", basic_hp_for_reset);
        if (PlayerPrefs.GetInt("CurrentHp", basic_hp_for_reset) > PlayerPrefs.GetInt("MaxHp", basic_hp_for_reset))
            hp = MAX_HP;
        else
            hp = PlayerPrefs.GetInt("CurrentHp", basic_hp_for_reset);
        MaxHealthText.text = "" + PlayerPrefs.GetInt("MaxHp", MAX_HP);

        escapeButtonPos = new Vector2(7f, 3.5f);

        if (PlayerPrefs.GetInt("Avatar", 0) == 0)
        {
            
            FramesDelayBeforeAttackStartsToCountAsDmg = new float[3]{ 0.15f, 0.15f, 0.2f };
            basicAttackButtons.SetActive(true);
            totemAttackButtons.SetActive(false);
        }
        else
        {
            range_for_attack = 2f;
            FramesDelayBeforeAttackStartsToCountAsDmg = new float[3] { 0.15f, 0.15f, 0.23f };
            basicAttackButtons.SetActive(false);
            totemAttackButtons.SetActive(true);
        }

        foreach (AudioSource a in clips)
        {
            a.ignoreListenerPause = true;
        }
    }

    void Update()
    {
        HealthBarParentObject.transform.localScale = new Vector3(Mathf.Sign(transform.localScale.x), 1.0f, 1.0f);
        if (get_current_hp() > 0)
        {
            CurrentHealthText.text = get_current_hp().ToString();
            HealthBarSlider.value = ((float)get_current_hp() / (float)get_max_hp());
        }
        else
        {
            CurrentHealthText.text = "" +0;
            HealthBarSlider.value = 0;
        }
        if (!finished_level_emmobilazied)
        {
            if (Input.GetMouseButtonDown(0))
            {
                is_dragging = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                is_dragging = false;
            }

            if (is_dragging && is_grounded)
            {
                Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                if (Mathf.Abs(pos.x) < escapeButtonPos.x || pos.y < escapeButtonPos.y)
                {
                    if (Vector2.Distance(pos, transform.position) > 1f && pos.y > -2.8f)
                        jump_to_point(pos, (Mathf.Abs(pos.y - transform.position.y) >= 2f));
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (!is_grounded && Physics2D.GetIgnoreLayerCollision(7,3))
        {
            if (Vector2.Distance(target_pos, transform.position) < 1.3f)
                Physics2D.IgnoreLayerCollision(7, 3,false);
        }
    }

    // This ballistic formula is using the standard mathematical idea of ballistic shooting from a point to a point
    Vector3 calcBallisticVelocityVector(Vector3 source, Vector3 target, float angle)
    {
        Vector3 direction = target - source;
        float h = direction.y+_collider.bounds.extents.y;
        direction.y = 0;
        float distance = direction.magnitude;
        float a = angle * Mathf.Deg2Rad;
        direction.y = distance * Mathf.Tan(a);
        distance += h / Mathf.Tan(a);

        float velocity = Mathf.Sqrt(distance * (Physics.gravity.magnitude * _rigidbody.mass * _rigidbody.gravityScale) / Mathf.Sin(2 * a));
        return velocity * direction.normalized;
    }

    // this is an overly simplified formula used for when they are going straight down and up,
    //      standard formula fails because it relies on a correct angle and "a logical" way to reach the point from that angle
    //      this way allows for a jump that "feels right" when the player would try to go straight up or down instead
    //      of letting the code to try and create an arch when going straight would suffice
    //      also this makes it easier by just switching instead of calculating correct angle from every point

    Vector3 calcBallisticVelocityVector2(Vector3 source, Vector3 target)
    {
        Vector3 diff = target - source;
        diff.y += _collider.bounds.size.y*3f; // offset y to avoid moving the pivot
        float gravity = 1.33f; // (Physics.gravity.magnitude * _rigidbody.mass * _rigidbody.gravityScale);

        return gravity * (diff.y + Mathf.Sqrt(diff.y * diff.y + diff.x * diff.x)) * diff.normalized;
    }

    public void jump_to_point(Vector2 target , bool higher)
    {
        _animator.Play("Avatar_Jump");
        if(!clips[6].isPlaying)
            clips[6].Play();
        Vector2 dir = target - (Vector2)transform.position;
        transform.localScale = new Vector3(Mathf.Sign(dir.x), 1.0f, 1.0f);
        HealthBarParentObject.transform.localScale = new Vector3(Mathf.Sign(dir.x), 1.0f, 1.0f);
        target_pos = target;

        Physics2D.IgnoreLayerCollision(7, 3);

        if (Mathf.Abs(dir.x) < 1.2f && dir.y > 0) // from 1.35 to 1.2
        {
            _rigidbody.velocity = calcBallisticVelocityVector2(transform.position, target);
        }
        else if (Mathf.Abs(dir.x) < 1.2f && dir.y < 0) // from 1.35 to 1.2
        {
            if (ground.gameObject.layer == 3)
            {
                _rigidbody.velocity = calcBallisticVelocityVector2(transform.position, target);
            }
        }
        else if (higher)
        {
            float jump_added_val = 2.5f;
            Vector2 new_velocity = calcBallisticVelocityVector(transform.position, target, JumpAngle);
            if (Mathf.Abs(target.y - transform.position.y) >= 3f)
                jump_added_val = 4.5f;
            _rigidbody.velocity = new Vector2(new_velocity.x * 0.7f, new_velocity.y + jump_added_val);
        }
        else
        {
            _rigidbody.velocity = calcBallisticVelocityVector(transform.position, target, JumpAngle);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        foreach (ContactPoint2D cp in collision.contacts)
        {
            if (cp.normal.y >= 0)
            {
                _rigidbody.velocity = new Vector2();
            }

            if (cp.collider.CompareTag("Ground"))
            {
                
                if (cp.normal.y > 0)
                {
                    if (!is_grounded) _animator.SetTrigger("Land");
                    is_grounded = true;
                    ground = cp.collider;
                }
                
            }
        }
        if (collision.collider.CompareTag("Walls"))
        {
            collision.collider.GetComponent<Animator>().SetTrigger("Move");
            clips[8].Play();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            is_grounded = false;
    }

    /*public void playerCanAttackMe(Monster enemy)
    {
        if (!playing_any_attacks_already() && is_grounded &&
            verify_facing(enemy.gameObject) &&
            !enemy.get_is_dead() && !enemy.get_is_hurt() )
        {
            is_attacking = true;
            int picked_anim_idx = 0;
            
            if(attack_counter < AttacksTilSpecial)
            {
                picked_anim_idx = attack_num-1;
                _animator.SetTrigger("Attack" + (picked_anim_idx + 1));
                attack_num = attack_num == 1 ? 2 : 1;
                ++attack_counter;
            }
            else
            {
                _animator.SetTrigger("Attack3");
                picked_anim_idx = 2;
                attack_counter = 0;
            }
            if (!clips[attack_num].isPlaying)
                clips[attack_num].Play();
            StartCoroutine(process_attack(FramesDelayBeforeAttackStartsToCountAsDmg[picked_anim_idx], enemy)); // _animator.GetCurrentAnimatorStateInfo(0).length
        }
    }*/

    public void newAttackSystem(bool is_left)
    {
        RaycastHit2D[] arr1;
        RaycastHit2D[] arr2;
        RaycastHit2D[] hits;
        arr1 = Physics2D.RaycastAll(transform.position, Vector2.right);
        arr2 = Physics2D.RaycastAll(transform.position, Vector2.left);
        hits = new RaycastHit2D[arr1.Length + arr2.Length];
        arr1.CopyTo(hits, 0);
        arr2.CopyTo(hits, arr1.Length);

        int picked_anim_idx = 0;
        _animator = GetComponent<Animator>();
        if (attack_counter < AttacksTilSpecial)
        {
            picked_anim_idx = attack_num - 1;
            _animator.SetTrigger("Attack" + (picked_anim_idx + 1)); //_animator.Play("Avatar_Attack" + (picked_anim_idx + 1))
            attack_num = attack_num == 1 ? 2 : 1;
            ++attack_counter;
        }
        else
        {
            _animator.SetTrigger("Attack3");  //_animator.Play("Avatar_Attack3");
            picked_anim_idx = 2;
            attack_counter = 0;
        }
        verify_facing(is_left); //enemy.gameObject);
        foreach (RaycastHit2D ray in hits)
        {
            if (ray.collider.gameObject.CompareTag("Enemy"))
            {
                Monster enemy = ray.collider.gameObject.GetComponent<Monster>();
                float distance = Mathf.Abs(ray.point.x - transform.position.x);
                if (enemy &&
                    distance < range_for_attack &&
                    verify_facing_monster(enemy) &&
                    /*!playing_any_attacks_already() &&*/
                    !enemy.get_is_dead() &&
                    !enemy.get_is_hurt())
                {
                   
                    is_attacking = true;
                    
                    
                    if (!clips[attack_num].isPlaying)
                        clips[attack_num].Play();
                    StartCoroutine(process_attack(FramesDelayBeforeAttackStartsToCountAsDmg[picked_anim_idx], enemy)); // _animator.GetCurrentAnimatorStateInfo(0).length
                }
                return;
            }
        }
    }

    public bool playing_any_attacks_already()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).IsName("Avatar_Attack1") ||
            _animator.GetCurrentAnimatorStateInfo(0).IsName("Avatar_Attack2") ||
            _animator.GetCurrentAnimatorStateInfo(0).IsName("Avatar_Attack3");
    }

    public bool verify_facing_monster(Monster enemey)
    {
        float dir = enemey.transform.position.x - transform.position.x;
        if (dir > 0 && transform.localScale.x > 0)
            return true;
        if (dir < 0 && transform.localScale.x < 0)
            return true;
        return false;
    }
    public void verify_facing(bool left_button)//GameObject other)
    {
        float dir = 1;
        if (left_button)
            dir = -1;
        /*float dir = other.transform.position.x - transform.position.x;
        if (dir > 0 && transform.localScale.x > 0)
            return;
        if (dir < 0 && transform.localScale.x < 0)
            return;*/
        transform.localScale = new Vector3(Mathf.Sign(dir), 1.0f, 1.0f);
        HealthBarParentObject.transform.localScale = new Vector3(Mathf.Sign(dir), 1.0f, 1.0f);
    }
    
    private IEnumerator process_attack(float time,Monster enemy)
    {
        yield return new WaitForSecondsRealtime(time);

        if (enemy && !enemy.get_is_dead() && !enemy.get_is_hurt()) {

            verify_facing_monster(enemy);
            enemy.gotAttacked((int)Mathf.Sign(transform.localScale.x));
        }
        is_attacking = false;
    }

    public void gotAttacked(int x_push_dir , int dmg, float x_push, float y_push)
    {
        getDmg(dmg);
        if (Time.time - last_time_got_hurt > delay_time_getting_hurt)
        {
            last_time_got_hurt = Time.time;
            if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Avatar_Hurt"))
            {
                _animator.SetTrigger("Hurt");
            }
            _rigidbody.velocity = new Vector2(x_push_dir * x_push, y_push);
        }
    }

    public void getDmg(int dmg)
    {
        if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Avatar_Hurt"))
        {
            _animator.SetTrigger("Hurt");
        }
        hp -= dmg;
        if (hp < 1)
        {
            Time.timeScale = 0;
            clips[4].Play();

            if (PlayerPrefs.GetInt("Coins") < 5)
                revive_canvas.GetComponent<ReviveManager>().not_reviving();
            else
                revive_canvas.GetComponent<ReviveManager>().start_revive_counter();
        }
    }
    public bool is_player_attacking_now()
    {
        return is_attacking;
    }
    public bool is_player_grounded()
    {
        return is_grounded;
    }
    public int get_max_hp()
    {
        return MAX_HP;
    }
    public Animator get_anim()
    {
        return _animator;
    }
    /*public void set_max_hp(int hp_to_set)
    {
        MAX_HP = hp_to_set;
    }*/
    public void set_current_hp(int hp_to_set)
    {
        hp = hp_to_set;
    }
    public void active_revive_avatar()
    {
        reviveAvater.SetActive(!reviveAvater.active);
    }
    public void set_active_coins_added()
    {
        coinsAdded.SetActive(true);
    }
    public int get_current_hp()
    {
        return hp;
    }
    public int get_basic_hp_for_reset()
    {
        return basic_hp_for_reset;
    }
    public AudioSource[] get_clips()
    {
        return clips;
    }
    public void set_finished_level_emmobilazied(bool set)
    {
        finished_level_emmobilazied = set;
    }
}
