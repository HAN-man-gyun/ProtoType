using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeWarrior : MonoBehaviour, organism
{
    public int hp { get; set; }
    public int movingCount { get; set; }
    public int damage { get; set; }
    public int shield { get; set; }

    public Weapon weapon { get; set; }
    public string type { get; set; }

    public Animator axeAnimator;

    private void Start()
    {
        axeAnimator = GetComponent<Animator>();
        weapon = GetComponent<Axe>();
        movingCount = 10;
    }

    public void Move()
    {
        axeAnimator.SetBool("Move",true);
    }
    public void MoveStop()
    {
        axeAnimator.SetBool("Move",false);
    }

    public void NormalAttack()
    {
        axeAnimator.SetTrigger("NormalAttack");
    }
    public void SkillAttack1()
    {
        axeAnimator.SetTrigger("Skill1");
    }
    public void SkillAttack2()
    {
        axeAnimator.SetTrigger("Skill2");
    }

}
