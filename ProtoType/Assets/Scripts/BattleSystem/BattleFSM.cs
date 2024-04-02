using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class BattleFSM
{
   public BattleFSM(BattleSystem system)
   {
        _startState = new BattleStartState(system);
        _endState = new BattleEndState(system);
        _enemyTurnState = new BattleEnemyTurnState(system);
        _myTurnState = new BattleMyTurnState(system);
        _chooseState = new BattleChooseState(system);
        _curState = _startState;
        ChangeState(_curState);
   }

    public BattleBaseState _curState;
    public BattleStartState _startState;
    public BattleEndState _endState;
    public BattleEnemyTurnState _enemyTurnState;
    public BattleMyTurnState _myTurnState;
    public BattleChooseState _chooseState;

    public void ChangeState(BattleBaseState nextState)
    {
        if(nextState == _curState)
        {
            return;
        }

        if(_curState ==null)
        {
            _curState.OnStateExit();
        }
        _curState = nextState;
        _curState.OnStateEnter();
    }

    public void UpdateState()
    {
        if(_curState != null)
            _curState.OnStateUpdate();
    }
}
