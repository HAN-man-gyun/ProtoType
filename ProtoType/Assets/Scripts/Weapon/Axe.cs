using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MonoBehaviour,Weapon
{
    public int skillRange { get; set; } = 1;
    public int intellect { get; set; } = 0;
    public int strength { get; set; } = 20;
    public int luck { get; set; } = 0;
    public int agility { get; set; } = 0;
    public GameObject rangeText { get; set; }
    public void Attack()
    {
        
    }
}
