using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public static BattleSystem battleSystem;
    public static BattleSystem BattleSystem1
    {
        get
        {
            // 만약 인스턴스가 없다면 새로 생성
            if (battleSystem == null)
            {
                // 새로운 게임 오브젝트를 생성하여 싱글턴 인스턴스로 지정
                GameObject singletonObject = new GameObject("BattleSystem");
                battleSystem = singletonObject.AddComponent<BattleSystem>();
            }
            return battleSystem;
        }
    }

    public bool isNormalAttack;
    public bool isSkill1Attack;
    public bool isSkill2Attack;
    public int skillCount;
    public BattleFSM fsm;
    State _curState;
    public Dikstra dikstra;
    //public List<>
    public enum State
    {
        start, playerTurn, chooseTurn, enemyTurn, end
    }

    public State state;
    // Start is called before the first frame update
    private void Update()    
    {
        fsm.UpdateState();
    }

    private void Awake()
    {
        // 인스턴스가 유일한지 확인
        if (battleSystem != null && battleSystem != this)
        {
            // 이미 다른 싱글턴 인스턴스가 존재하면 현재 인스턴스를 파괴
            Destroy(this.gameObject);
        }
        else
        {
            // 유일한 싱글턴 인스턴스로 설정
            battleSystem = this;
            // 다른 씬으로 넘어가더라도 파괴되지 않도록 설정
            DontDestroyOnLoad(this.gameObject);
        }
    }
    void Start()
    {
        fsm = new BattleFSM(this);
        state = State.start;
        ChangeState(state);
        dikstra = dikstra = FindFirstObjectByType<Dikstra>();

        isNormalAttack = false;
        isSkill1Attack = false;
        isSkill2Attack = false;
    }

    public void ChangeState(State nextState)
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
            case State.chooseTurn:
                fsm.ChangeState(fsm._chooseState);
                break;
            case State.enemyTurn:
                fsm.ChangeState(fsm._enemyTurnState);
                break;
            case State.end:
                fsm.ChangeState(fsm._endState);
                break;
        }
    }
}
