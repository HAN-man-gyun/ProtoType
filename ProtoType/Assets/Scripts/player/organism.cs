using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface organism
{
    int hp { get; set; }
    int movingCount { get; set; }
    int damage { get; set; }
    int shield { get; set; }
    string type { get; set; }
    Weapon weapon { get; set; }

    int attackRange { get; set; }
    void Move();
    void MoveStop();
    void NormalAttack();
    void SkillAttack1();
    void SkillAttack2();
    
}
