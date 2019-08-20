using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCanvasUI : CanvasUIBase
{
    //HUD UI组件
    public Slider hpBar;
    public Slider spBar;
    public Slider epBar;
    public Slider epPointBar;
    //动态属性值 实现动态增减动画
    float hpMotion;
    float spMotion;
    float epMotion;

    //UI界面
    public GameObject EscMenu; //Esc菜单
    public GameObject GameOver; //游戏结束菜单

    public override void OnUpdate()
    {
        if (player == null)
            return;

        RefreshHUD(); //刷新HUD显示
        OnEscMenu(); //Esc菜单
        OnGameOver(); //GameOver菜单
    }

    public override void OnEnable()
    {
        base.OnEnable();

        EscMenu.SetActive(false); //隐藏Esc菜单
        GameOver.SetActive(false); //隐藏Gameover菜单
        UiMenuOff();

        if (player == null)
            return;

        //关联开启小地图
        GameObject KGFMap = Instantiate(Resources.Load("Prefab/UI/KGFMapSystem")) as GameObject; //游戏场景UI界面 的小地图
        KGFMap.GetComponent<KGFMapSystem>().SetTarget(player.gameObject); //设定玩家目标物体
    }

    void RefreshHUD()
    {
        //动态属性值
        if (Mathf.Abs(player.Hp - hpMotion) > 1.0f)
        {
            hpMotion += (player.Hp - hpMotion) * 0.1f;
        }
        else
            hpMotion = player.Hp;
        if (Mathf.Abs(player.Sp - spMotion) > 1.0f)
        {
            spMotion += (player.Sp - spMotion) * 0.1f;
        }
        else
            spMotion = player.Sp;
        if (Mathf.Abs(player.Ep - epMotion) > 1.0f)
        {
            epMotion += (player.Ep - epMotion) * 0.1f;
        }
        else
            epMotion = player.Ep;

        //刷新体力条
        hpBar.value = hpMotion / player.HpMax;
        //刷新耐力条
        spBar.value = spMotion / player.SpMax;
        //刷新能量条
        epBar.value = epMotion / player.EpMax * 0.2f;

        //刷新能量点槽
        if (player.EpPoint == 0)
            epPointBar.value = 0f;
        else if (player.EpPoint == 1)
            epPointBar.value = 0.08f;
        else if (player.EpPoint == 2)
            epPointBar.value = 0.14f;
        else if (player.EpPoint == 3)
            epPointBar.value = 0.2f;
        else
            epPointBar.value = 0f;

        //耐力条颜色
        if (player.SpUseup) //耐力耗尽
            spBar.transform.Find("Fill Area").GetComponentInChildren<Image>().color = Color.gray;
        else
            spBar.transform.Find("Fill Area").GetComponentInChildren<Image>().color = Color.white;
    }

    void OnEscMenu() //系统菜单
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !EscMenu.activeSelf) //当Esc菜单非激活状态时
        {
            EscMenu.SetActive(true); //显示Esc菜单
            UiMenuOn();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && EscMenu.activeSelf) //当Esc菜单是激活状态时
        {
            EscMenu.SetActive(false); //隐藏Esc菜单
            UiMenuOff();
        }

    }

    void OnGameOver() //游戏结束菜单
    {
        if (player.GetComponent<PlayerCharacter>().Death)
        {
            Cursor.visible = true; // 显示鼠标
            Cursor.lockState = CursorLockMode.Confined; //区域锁定鼠标

            GameOver.SetActive(true); //显示Gameover菜单
        }
    }

    void UiMenuOn() //UI菜单打开设置
    {
        Cursor.visible = true; // 显示鼠标
        Cursor.lockState = CursorLockMode.Confined; //区域锁定鼠标
        Time.timeScale = 0; //暂停游戏

        if (player != null) 
            player.GetComponent<Animator>().enabled = true; //打开动画控制器
    }
    void UiMenuOff() //UI菜单关闭设置
    {
        Cursor.visible = false; //隐藏鼠标
        Cursor.lockState = CursorLockMode.Locked; //定点锁定鼠标
        Time.timeScale = 1; //继续游戏

        if (player != null)
            player.GetComponent<Animator>().enabled = true; //打开动画控制器
    }

    public void ButtonRenew() //重新开始
    {
        //加载场景
        sceneManager.LoadScene("Village", "SpawnPoint");
    }

    public void ButtonGameSave() //保存进度
    {
        SysModuleManager.Instance.GetSysModule<SysFileManager>().SaveGameFile();
    }

    public void ButtonGameLoad() //载入存档
    {
        SysModuleManager.Instance.GetSysModule<SysFileManager>().LoadGameFile();
    }

    public void ButtonGameSetUp() //游戏设置
    {
        Debug.Log("游戏设置");
    }

    public void ButtonBackMainMenu() //回到主界面按钮
    {
        //加载Start场景
        sceneManager.LoadScene("Start", null);
        //加载StartUI界面
        SysModuleManager.Instance.GetSysModule<SysUIEnv>().LoadCanvas("StartCanvas");
    }
}
