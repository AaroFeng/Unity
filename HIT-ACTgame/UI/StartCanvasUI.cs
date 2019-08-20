using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCanvasUI : CanvasUIBase
{
    public override void OnEnable()
    {
        base.OnEnable();

        Cursor.visible = true; // 显示鼠标
        Cursor.lockState = CursorLockMode.Confined; //区域锁定鼠标
        Time.timeScale = 1; //继续游戏
    }

    public override void OnUpdate()
    {

    }

    public void GameStart() //开始游戏
    {
        //加载场景
        sceneManager.LoadScene("Village", "SpawnPoint");
    }

    public void GameLoad() //载入存档
    {
        SysModuleManager.Instance.GetSysModule<SysFileManager>().LoadGameFile();
    }

    public void GameSetUp() //游戏设置
    {
        Debug.Log("游戏设置");
    }

    public void GameExit() //退出游戏
    {
        Application.Quit();
    }

    private void OnGUI()
    {
        if(GUILayout.Button("测试场景"))
        {
            //加载场景
            sceneManager.LoadScene("Test", "SpawnPoint");
        }
    }
}
