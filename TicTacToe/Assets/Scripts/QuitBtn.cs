using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuitBtn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(() => QuitGame());
    }

    // Update is called once per frame
    public void QuitGame()
    {
        // 在编辑器中会打印日志（不会真正退出），在构建的游戏中会退出
        Debug.Log("退出游戏");
        Application.Quit();
    }
}
