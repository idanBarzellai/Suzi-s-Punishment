using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemControls : MonoBehaviour
{
    [SerializeField] GameObject[] body_parts;
    [SerializeField] GameObject[] spear_hair;
    [SerializeField] GameObject[] hairs;
    private void Start()
    {
        body_parts = GameObject.FindGameObjectsWithTag("Body");
        spear_hair = GameObject.FindGameObjectsWithTag("Spear");
    }
    public GameObject[] get_body_parts()
    {
        return body_parts;
    }
    public GameObject[] get_spear_hair()
    {
        return spear_hair;
    }
    public GameObject[] get_hairs()
    {
        return hairs;
    }
}
