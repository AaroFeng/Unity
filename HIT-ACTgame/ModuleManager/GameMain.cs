using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//项目程序入口 负责游戏的主逻辑更新
public class GameMain : MonoBehaviour
{
    SysSceneManager sceneManager;
    SysPlayerManager playerManager;
    SysUIEnv uiManager;
    SysPool pool;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        SysModuleManager.Instance.Initialize(gameObject); //设置根物体

        //加载 初始化所有模块
        SysModuleManager.Instance.AddSysModule<SysSceneManager>();
        SysModuleManager.Instance.AddSysModule<SysPlayerManager>();
        SysModuleManager.Instance.AddSysModule<SysUIEnv>();
        SysModuleManager.Instance.AddSysModule<SysPool>();
        SysModuleManager.Instance.AddSysModule<SysFileManager>();

        //获取场景模块
        sceneManager = SysModuleManager.Instance.GetSysModule<SysSceneManager>();
        //获取玩家模块
        playerManager = SysModuleManager.Instance.GetSysModule<SysPlayerManager>();
        //获取UI模块
        uiManager = SysModuleManager.Instance.GetSysModule<SysUIEnv>();
        //获取缓存池模块
        pool = SysModuleManager.Instance.GetSysModule<SysPool>();

        //缓存池 添加缓存预制物列表
        pool.AddPrefab(Resources.Load("Prefab/Player/RoleAnnika") as GameObject, 1); //玩家角色
        pool.AddPrefab(Resources.Load("Prefab/Player/PlayerCamera") as GameObject, 1); //玩家摄像机
        pool.AddPrefab(Resources.Load("Prefab/UI/DialogCanvas") as GameObject, 1); //对话与提示UI界面
        pool.AddPrefab(Resources.Load("Prefab/UI/StartCanvas") as GameObject, 1); //开始场景UI界面
        pool.AddPrefab(Resources.Load("Prefab/UI/GameCanvas") as GameObject, 1); //游戏场景UI界面
        pool.AddPrefab(Resources.Load("Prefab/UI/OrcKingCanvas") as GameObject, 1); //兽人首领UI界面
        //敌人预制物缓存
        //蠕虫射手
        pool.AddPrefab(Resources.Load("Prefab/Enemy/Venom") as GameObject, 5); //毒液弹
        //兽人首领
        pool.AddPrefab(Resources.Load("Prefab/Enemy/OrcKing/Bullet") as GameObject, 2); //火焰弹
        pool.AddPrefab(Resources.Load("Prefab/Enemy/OrcKing/TrackBullet") as GameObject, 15); //追踪弹
        pool.AddPrefab(Resources.Load("Prefab/Enemy/OrcKing/RangeBullet") as GameObject, 3); //范围弹
        pool.AddPrefab(Resources.Load("Prefab/Enemy/OrcKing/RangeBulletTarget") as GameObject, 3); //范围弹目标
        pool.AddPrefab(Resources.Load("Prefab/Enemy/OrcKing/RangeBulletHit") as GameObject, 3); //范围弹击中

        //缓存池 实例化 缓存预制物列表
        pool.BufferPrefabs();

        //加载Start场景
        sceneManager.LoadScene("Start", null);
    }

    void Start ()
    {
        
    }
	
    //系统模块主逻辑更新
	void Update ()
    {
        SysModuleManager.Instance.OnUpdate();
	}

    //程序暂停
    private void OnApplicationPause(bool pause)
    {
        
    }

    //程序失去焦点
    private void OnApplicationFocus(bool focus)
    {
        
    }

    //程序退出
    private void OnApplicationQuit()
    {
        
    }
}
