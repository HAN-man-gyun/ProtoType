using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BattleChooseState : BattleBaseState
{
    public BattleChooseState(BattleSystem system) : base(system)
    {

    }
    public override void OnStateEnter()
    {

    }

    public override void OnStateExit()
    {

    }

    public override void OnStateUpdate()
    {
        Debug.Log("chooseturn");
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            Vector3 hitPoint = new Vector3(hit.point.x, 0, hit.point.z);
            if (!(hitPoint.x < 0 || hitPoint.z < 0 || hitPoint.x>BattleSystem.BattleSystem1.dikstra.grid.GetLength(0)* BattleSystem.BattleSystem1.dikstra.cellSize ||
                hitPoint.z > BattleSystem.BattleSystem1.dikstra.grid.GetLength(1) * BattleSystem.BattleSystem1.dikstra.cellSize))
            {
                BattleSystem.BattleSystem1.dikstra.MakeTargetTextureRed(hitPoint);
            }
            
        }
    }

   
}
