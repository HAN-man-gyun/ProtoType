using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Weapon
{
    int skillRange{ get; set; }
    int intellect { get; set; }
    int strength { get; set; }
    int luck { get; set; }
    int agility { get; set; }

    GameObject rangeText { get; set; }
    void Attack();
}
