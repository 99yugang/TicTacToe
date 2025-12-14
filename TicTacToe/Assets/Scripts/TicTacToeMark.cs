using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TicTacToeMark : MonoBehaviour
{
    public GameObject xMark;
    public GameObject oMark;
    private int markIndex_1;
    private int markIndex_2;
    private TicTacToeManager.markState markState;

    public void SetEmpty(int index_1, int index_2)
    {
        markState = TicTacToeManager.markState.None;
        markIndex_1 = index_1;
        markIndex_2 = index_2;
        xMark.SetActive(false);
        oMark.SetActive(false);
        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() => OnClick());
    }

    private void OnClick()
    {
        TicTacToeManager.Instance.Move(markIndex_1, markIndex_2);
    }

    public void RefreshUI(TicTacToeManager.markState newMakeState)
    {
        if (markState != newMakeState)
        {
            markState = newMakeState;
            switch (newMakeState)
            {
                case TicTacToeManager.markState.None:
                    xMark.SetActive(false);
                    oMark.SetActive(false);
                    break;
                case TicTacToeManager.markState.X:
                    xMark.SetActive(true);
                    oMark.SetActive(false);
                    break;
                case TicTacToeManager.markState.O:
                    xMark.SetActive(false);
                    oMark.SetActive(true);
                    break;
            }

        }
    }
}
