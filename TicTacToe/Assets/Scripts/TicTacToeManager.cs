using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TicTacToeManager : MonoBehaviour
{

    private enum Mode
    {
        TowPlayer,
        AI,
    }

    private enum State
    {
        Playing,
        XWin,
        OWin,
        NoWin,
    }

    private enum Turn { 
        X,
        O
    }

    public struct Mark
    {
        public TicTacToeMark _mark;
        public markState markState;
    }

    public enum markState
    {
        None,
        X,
        O
    }
    public string markPrefabPath;
    public TextMeshProUGUI stateText;
    public TextMeshProUGUI aiModeTipsText;
    public GameObject setPanel;
    public Button setAIModeBtn;
    public Button setPlayerModeBtn;
    public Button setPlayerABtn;
    public Button setPlayerBBtn;
    public Button resetBtn;
    public Transform ticTacToeBoard;
    private static TicTacToeManager _instance;
    private bool hasInitBoard;
    private Mark[,] board = new Mark[3, 3];
    public static TicTacToeManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(TicTacToeManager)) as TicTacToeManager;
            }
            return _instance;
        }
    }
    //当前模式
    private Mode currntMode;
    //当前轮次
    private State currenState;
    //初始先手
    private Turn initTurn;
    //当前手
    private Turn currentTurn;


    void Start()
    {
        hasInitBoard = false;
        resetBtn.onClick.RemoveAllListeners();
        resetBtn.onClick.AddListener(() => {
            ResetGame();
        });
        ResetGame();

    }

    private void ResetGame()
    {
        ResetMode();
        ResetBoard();
    }

    private void ResetMode()
    {
        setPanel.SetActive(true);
        setAIModeBtn.onClick.RemoveAllListeners();
        setPlayerModeBtn.onClick.RemoveAllListeners();
        setPlayerABtn.onClick.RemoveAllListeners();
        setPlayerBBtn.onClick.RemoveAllListeners();
        
        setAIModeBtn.onClick.AddListener(() => {
            SetAIMode();
        });

        setPlayerModeBtn.onClick.AddListener(() => {
            SetTwoPlayMode();
        });

        setPlayerABtn.onClick.AddListener(() => {
            SetATurn();
        });

        setPlayerBBtn.onClick.AddListener(() => {
            SetBTurn();
        });

        setAIModeBtn.gameObject.SetActive(true);
        setPlayerModeBtn.gameObject.SetActive(true);
        setPlayerABtn.gameObject.SetActive(false);
        setPlayerBBtn.gameObject.SetActive(false);
    }

    private void ResetBoard()
    {
        if(hasInitBoard)
        {
            for (int temp1 = 0; temp1 < 3; temp1++)
            {
                for (int temp2 = 0; temp2 < 3; temp2++)
                {
                    board[temp1, temp2].markState = markState.None;
                    board[temp1, temp2]._mark.RefreshUI(markState.None);
                   

                }
            }
        }
        else
        {
            GameObject prefab = Resources.Load<GameObject>(markPrefabPath);
            if (prefab == null)
            {
                Debug.LogError($"Prefab not found at path: {markPrefabPath}");
                return;
            }
            for (int temp1 = 0; temp1 < 3; temp1++)
            {
                for (int temp2 = 0; temp2 < 3; temp2++)
                {
                    GameObject mark = Instantiate(prefab, ticTacToeBoard);
                    mark.GetComponent<TicTacToeMark>().SetEmpty(temp1, temp2);
                    board[temp1, temp2] = new Mark { _mark = mark.GetComponent<TicTacToeMark>(),
                    markState = markState.None } ;

                }
            }
            hasInitBoard = true;
        }
        currentTurn = Turn.X;
        SetState();
    }

    public void Move(int markIndex_1 = -1, int markIndex_2 = -1)
    {
        if(board[markIndex_1,markIndex_2].markState == markState.None && CheckState() == State.Playing)
        {
            switch (currntMode)
            {
                case Mode.TowPlayer:
                    if (currentTurn == Turn.X)
                    {
                        currentTurn = Turn.O;
                        board[markIndex_1, markIndex_2].markState = markState.X;
                        board[markIndex_1, markIndex_2]._mark.RefreshUI(markState.X);
                        CheckState();
                        SetState();
                    }
                    else
                    {
                        currentTurn = Turn.X;
                        board[markIndex_1, markIndex_2].markState = markState.O;
                        board[markIndex_1, markIndex_2]._mark.RefreshUI(markState.O);
                        CheckState();
                        SetState();
                    }
                    break;

                case Mode.AI:

                    if (currentTurn == initTurn)
                    {
                        currentTurn = initTurn == Turn.X ? Turn.O : Turn.X;
                        board[markIndex_1, markIndex_2].markState = initTurn == Turn.X ? markState.X : markState.O;
                        board[markIndex_1, markIndex_2]._mark.RefreshUI(initTurn == Turn.X ? markState.X : markState.O);
                        CheckState();
                        SetState();
                        if (currenState == State.Playing)
                            MoveAI();

                    }
                    break;
                    
            }

        }
        return ;
    }

    private void MoveAI()
    {
        if (currentTurn != initTurn && CheckState() == State.Playing)
        {
            currentTurn = initTurn;
            int tarLoseX, tarLoseY, cnt;
            int[] mp = new int[9];
            cnt = 0;
            tarLoseX = tarLoseY = -1;

            //遍历棋盘，计算下一次落子位置
            for (int temp1 = 0; temp1 < 3; temp1++)
            {
                for (int temp2 = 0; temp2 < 3; temp2++)
                {
                    if (board[temp1, temp2].markState == markState.None)
                    {
                        //判断AI落子此处是否会胜利，若胜利，则落子
                        board[temp1, temp2].markState = initTurn == Turn.X ? markState.O : markState.X;
                        if (CheckState() == (initTurn == Turn.X ? State.OWin : State.XWin))
                        {
                            board[temp1, temp2]._mark.RefreshUI(initTurn == Turn.X ? markState.O : markState.X);
                            //获胜
                            CheckState();
                            SetState();
                            return;

                        }

                        //判断玩家落子此处是否会胜利，若胜利，则记下当前位置(玩家已将军)
                        board[temp1, temp2].markState = initTurn == Turn.X ? markState.X : markState.O;
                        if (CheckState() == (initTurn == Turn.X ? State.XWin : State.OWin))
                        {
                            tarLoseX = temp1;
                            tarLoseY = temp2;
                        }

                        //恢复棋盘，记下当前空白格位置
                        board[temp1, temp2].markState = markState.None;
                        mp[cnt++] = temp1 * 3 + temp2;
                    }
                }
            }

            //若存在玩家将军，则落子
            if (tarLoseX != -1)
            {
                board[tarLoseX, tarLoseY].markState = initTurn == Turn.X ? markState.O : markState.X;
                board[tarLoseX, tarLoseY]._mark.RefreshUI(initTurn == Turn.X ? markState.O : markState.X);
                SetState();
                return;
            }

            //AI落子后既不会胜利，也不存在玩家将军，则随机选择一个空白格落子
            int rd = (int)Random.Range(0, cnt);
            board[mp[rd] / 3, mp[rd] % 3].markState = initTurn == Turn.X ? markState.O : markState.X;
            board[mp[rd] / 3, mp[rd] % 3]._mark.RefreshUI(initTurn == Turn.X ? markState.O : markState.X);
            SetState();
        }
    }
    
    private State CheckState()
    {
       
        //判断交叉线是否符合获胜条件
        if (board[0, 0].markState != markState.None && board[0, 0].markState == board[1, 1].markState && board[0, 0].markState == board[2, 2].markState)
            return (State)board[0, 0].markState;
        if (board[2, 0].markState != markState.None && board[2, 0].markState == board[1, 1].markState && board[2, 0].markState == board[0, 2].markState)
            return (State)board[2, 0].markState;

        int cnt = 0;
        for (int temp1 = 0; temp1 < 3; temp1++)
        {
            //判断temp1行是否符合获胜条件
            if (board[temp1, 0].markState == board[temp1, 1].markState && board[temp1, 0].markState == board[temp1, 2].markState && board[temp1, 0].markState != 0)
                return (State)board[temp1, 0].markState;
            //判断temp1列是否符合获胜条件
            if (board[0, temp1].markState == board[1, temp1].markState && board[0, temp1].markState == board[2, temp1].markState && board[0, temp1].markState != 0)
                return (State)board[0, temp1].markState;
            //统计temo1行的空白格数量
            for (int temp2 = 0; temp2 < 3; temp2++)
            {
                if (board[temp1, temp2].markState == 0)
                    cnt++;
            }
        }
        //若空白格已下完，则平局，否则游戏继续进行
        return (State)(cnt == 0 ? 3 : 0);
        
    }

    public void SetState()
    {
        currenState = CheckState();
        switch (currenState)
        {
            case State.Playing:
                switch (currentTurn)
                {
                    case Turn.X:
                        stateText.text = "X回合";
                        break;
                    case Turn.O:
                        stateText.text = "O回合";
                        break;
                }
                break;
            case State.XWin:
                stateText.text = "X获胜";

                break;
            case State.OWin:
                stateText.text = "O获胜";
                break;
            case State.NoWin:
                stateText.text = "平局";
                break;
        }
    }


    private void SetAIMode()
    {
        currntMode = Mode.AI;
        setAIModeBtn.gameObject.SetActive(false);
        setPlayerModeBtn.gameObject.SetActive(false);
        setPlayerABtn.gameObject.SetActive(true);
        setPlayerBBtn.gameObject.SetActive(true);
    }

    private void SetTwoPlayMode()
    {
        currntMode = Mode.TowPlayer;
        aiModeTipsText.gameObject.SetActive(false);
        setPanel.SetActive(false);
    }

    private void SetATurn()
    {
        initTurn = Turn.X;
        setPanel.SetActive(false);
        aiModeTipsText.gameObject.SetActive(true);
        aiModeTipsText.text = "玩家 : X    AI : O";
    }

    private void SetBTurn()
    {
        initTurn = Turn.O;
        aiModeTipsText.gameObject.SetActive(true);
        aiModeTipsText.text = "玩家 : O    AI : X";
        setPanel.SetActive(false);
        MoveAI();
    }

}
