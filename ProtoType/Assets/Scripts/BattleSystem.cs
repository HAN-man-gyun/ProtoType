using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    BattleFSM fsm;
    State _curState;
    Dikstra dikstra;
    public enum State
    {
        start, playerTurn, enemyTurn, end
    }

    public State state;
    // Start is called before the first frame update
    void Start()
    {
        fsm = new BattleFSM(this);
        state = State.start;
        ChangeState(state);

        //dikstra
    }

    private void ChangeState(State nextState)
    {
        if(_curState == nextState)
        {
            return;
        }
        _curState = nextState;

        switch(_curState)
        {
            case State.start:
                fsm.ChangeState(fsm._startState);
                break;
            case State.playerTurn:
                fsm.ChangeState(fsm._myTurnState);
                break;
            case State.enemyTurn:
                fsm.ChangeState(fsm._enemyTurnState);
                break;
            case State.end:
                fsm.ChangeState(fsm._endState);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
