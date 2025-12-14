using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScensBtn: MonoBehaviour
{
    public string sceneName;
       
    void Start()
    {
        
        
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(() => LoadScene(sceneName));
        
    }


    // Update is called once per frame
    void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
