using UnityEngine;


public class TempPlayer : MonoBehaviour
{
    Dikstra dikstra;
    Mouse mouse;
    public void Start()
    {

        mouse = FindFirstObjectByType<Mouse>();
    }

    private void OnMouseDown()
    {

    }

    private void OnMouseUp()
    {
        dikstra = BattleSystem.BattleSystem1.dikstra;
        if (BattleSystem.BattleSystem1.fsm._curState == BattleSystem.BattleSystem1.fsm._startState)
        {
            Vector3 mousePos = dikstra.GetNodeFromWorldPoint(mouse.hitPoint).worldPosition; //mouse.hitPoint
            transform.position = new Vector3(mousePos.x + dikstra.cellSize / 2, mousePos.y, mousePos.z + dikstra.cellSize / 2);
            BattleSystem.BattleSystem1.state = BattleSystem.State.playerTurn;
            BattleSystem.BattleSystem1.ChangeState(BattleSystem.BattleSystem1.state);
        }
    }

    private void OnMouseDrag()
    {

    }
}
