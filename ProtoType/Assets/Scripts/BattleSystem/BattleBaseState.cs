using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleBaseState
{
    protected BattleSystem system;

    protected BattleBaseState(BattleSystem system)
    {
        this.system = system;
    }

    public abstract void OnStateEnter();
    public abstract void OnStateExit();
    public abstract void OnStateUpdate();
}
