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
            // ���� �ν��Ͻ��� ���ٸ� ���� ����
            if (battleSystem == null)
            {
                // ���ο� ���� ������Ʈ�� �����Ͽ� �̱��� �ν��Ͻ��� ����
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
        // �ν��Ͻ��� �������� Ȯ��
        if (battleSystem != null && battleSystem != this)
        {
            // �̹� �ٸ� �̱��� �ν��Ͻ��� �����ϸ� ���� �ν��Ͻ��� �ı�
            Destroy(this.gameObject);
        }
        else
        {
            // ������ �̱��� �ν��Ͻ��� ����
            battleSystem = this;
            // �ٸ� ������ �Ѿ���� �ı����� �ʵ��� ����
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
