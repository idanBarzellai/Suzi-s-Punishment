using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TotemEntities;
using enums;
using consts;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TotemCharacter : MonoBehaviour
{
    TotemUsersDB usersDB;
    TotemEntitiesDB entitiesDB;
    [SerializeField] GameObject player;
    TotemUser user;

    Material material1;
    Material material2;
/*    Material material3;
    Material material4;
    Material material5;
    Material material6;
    Material material7;
*/
    TotemAvatar avatar;
    TotemSpear spear;

    Color[] elements_colors;

    public static GameObject totem_instance;

    void Awake()
    {
        if (totem_instance != null)
            Destroy(this.gameObject);
        else
        {
            totem_instance = this.gameObject;
            DontDestroyOnLoad(totem_instance);
        }
    }

    void Start()
    {
        if (GameObject.FindObjectOfType<TotemControls>())
            player = GameObject.FindObjectOfType<TotemControls>().gameObject;
        TotemMockDB mockDB = new TotemMockDB();
        entitiesDB = mockDB.EntitiesDB;
        usersDB = mockDB.UsersDB;

        elements_colors = new Color[]{
            new Color(0.87f, 0.87f, 0.87f, 1f), // Air
            new Color(0.69f, 0.356f, 0.247f, 1f), // Earth
            new Color(0.231f, 0.886f, 0.867f, 1f), // Water
            new Color(0.984f, 0f, 0.0745f, 1f) }; // Fire

        /*material1 = new Material(Shader.Find("ColorChangeTotem"));
        material2 = new Material(Shader.Find("ColorChangeHairSpear"));
        material3 = new Material(Shader.Find("ColorChangeF"));
        material4 = new Material(Shader.Find("ColorChangeM"));
        material5 = new Material(Shader.Find("ColorChangeMF"));
        material6 = new Material(Shader.Find("ColorChangeMFF"));
        material7 = new Material(Shader.Find("ColorChangeBoss"));*/

    }


    void set_materials()
    {
        material1 = new Material(Shader.Find("ColorChangeTotem"));
        material2 = new Material(Shader.Find("ColorChangeHairSpear"));
        /*material3 = new Material(Shader.Find("ColorChangeF"));
        material4 = new Material(Shader.Find("ColorChangeM"));
        material5 = new Material(Shader.Find("ColorChangeMF"));
        material6 = new Material(Shader.Find("ColorChangeMFF"));
        material7 = new Material(Shader.Find("ColorChangeBoss"));*/

        Color darker = Color.Lerp(avatar.skinColor, Color.black, .5f);
        Color dress_color = new Color(1 - avatar.hairColor.r, 1 - avatar.hairColor.g, 1 - avatar.hairColor.b, avatar.hairColor.a);
        Color dress_color_dark = new Color(0.686f - avatar.hairColor.r, 0.686f - avatar.hairColor.g, 0.686f - avatar.hairColor.b, avatar.hairColor.a);
        Color hair_color_dark = new Color(avatar.hairColor.r - 0.3137f, avatar.hairColor.g - 0.3137f, avatar.hairColor.b - 0.3137f, avatar.hairColor.a);
        Color spear_shadow = new Color(pick_color(spear.element).r - 0.3137f, pick_color(spear.element).g - 0.3137f, pick_color(spear.element).b - 0.3137f, pick_color(spear.element).a);

        material1.SetColor("_C_cfcfcf", avatar.skinColor); // skin
        material1.SetColor("_C_a1a1a1_b2b2b2", darker);  // skin dark
        material1.SetColor("_C_858585", dress_color); // dress
        material1.SetColor("_C_595959", dress_color_dark); // dress shadow 
        material1.SetColor("_C_606060", avatar.hairColor); // eyebrows

        material2.SetColor("_C_a1a1a1_b2b2b2", avatar.hairColor); // hair
        material2.SetColor("_C_000000", hair_color_dark); // hair shadow
        material2.SetColor("_C_ededed", spear.shaftColor); // spear
        material2.SetColor("_C_7f7f7f", pick_color(spear.element)); // spear element pick
        material2.SetColor("_C_606060", spear_shadow);

        /*material3.SetColor("_C_ffffff", pick_color(spear.element)); // monster F
        material4.SetColor("_C_ffffff", pick_color(spear.element)); // monster M
        material5.SetColor("_C_ffffff", pick_color(spear.element)); // monster MF
        material6.SetColor("_C_ffffff", pick_color(spear.element)); // monster FF
        
        material7.SetColor("_C_7f7f7f", pick_color(spear.element)); // boss*/
        if (player)
        {
            PlayerPrefs.SetFloat("AvatarDmg", spear.damage + 50f);
            foreach (GameObject obj in player.GetComponent<TotemControls>().get_body_parts())
            {
                foreach (SpriteRenderer sprite in obj.GetComponentsInChildren<SpriteRenderer>())
                    sprite.material = material1;
            }
            foreach (GameObject obj in player.GetComponent<TotemControls>().get_spear_hair())
            {
                foreach (SpriteRenderer sprite in obj.GetComponentsInChildren<SpriteRenderer>())
                    sprite.material = material2;
            }
            foreach (GameObject obj in player.GetComponent<TotemControls>().get_hairs())
            {
                if (obj.name.Equals(avatar.hairStyle.ToString()))
                {

                    obj.SetActive(true);
                    foreach (SpriteRenderer sprite in obj.GetComponentsInChildren<SpriteRenderer>())
                        sprite.material = material2;

                }
            }
        }

    }

    void set_monster_materials()
    {
        Monster[] monsters_in_scene = FindObjectsOfType<Monster>();
        foreach (Monster mob in monsters_in_scene)
        {
            if (mob is MonsterFemale)
                set_metarial_for_monster(mob, false);
            else if(mob is MonsterMale)
                set_metarial_for_monster(mob, false);
            else if(mob is MonsterMaleFire)
                set_metarial_for_monster(mob, false);
            else if (mob is MonsterFemaleFire)
                set_metarial_for_monster(mob, false);
            else if (mob is Boss)
                set_metarial_for_monster(mob, true);

        }
    }

    void set_metarial_for_monster(Monster mob, bool boss)
    {
        for (int i = 0; i < mob.transform.GetChild(0).childCount; i++)
        {
            if (boss)
            {
                if (mob.transform.GetChild(0).GetChild(i).CompareTag("Monster_head"))
                {

                    SpriteRenderer sprite = mob.transform.GetChild(0).GetChild(i).GetComponentInChildren<SpriteRenderer>();
                    sprite.material.SetColor("_C_7f7f7f", pick_color(spear.element));
                    //sprite.material = mat;
                }
            }
            else
            {
                if (mob.transform.GetChild(0).GetChild(i).CompareTag("Monster_head"))
                {

                    SpriteRenderer sprite = mob.transform.GetChild(0).GetChild(i).GetComponentInChildren<SpriteRenderer>();
                    sprite.material.SetColor("_C_ffffff", pick_color(spear.element));
                    //sprite.material = mat;
                }
            }
        }
    }

    private void Update()
    {
        /*if (Input.GetKey(KeyCode.Space))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);*/
        if (GameObject.FindObjectOfType<TotemControls>())
            player = GameObject.FindObjectOfType<TotemControls>().gameObject;

        if (user != null)
        {
            avatar = user.GetOwnedAvatars()[0];
            spear = user.GetOwnedSpears()[0];
            set_materials();
        }
        
        if (!SceneManager.GetActiveScene().name.Equals("Login"))
        {
            if (player)
            {
                set_monster_materials();
            }
        }
        
    }

    private Color pick_color(ElementEnum elem)
    {
        switch (elem)
        {
            case ElementEnum.Air:
                return elements_colors[0];
            case ElementEnum.Earth:
                return elements_colors[1];
            case ElementEnum.Water:
                return elements_colors[2];
            case ElementEnum.Fire:
                return elements_colors[3];
            default:
                return new Color(1, 1, 1, 1);
        }
    }

    public string get_hair_style()
    {
        return avatar.hairStyle.ToString();
    }
    public Color get_hair_color()
    {
        return avatar.hairColor;
    }
    public TotemUsersDB get_totem_userdb()
    {
        return usersDB;
    }

    public void user_logged_in(string name)
    {
        user = usersDB.GetUser(name);
    }
}
