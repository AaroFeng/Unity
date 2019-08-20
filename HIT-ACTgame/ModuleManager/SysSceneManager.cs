using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SysSceneManager : SysModule
{
    string nowScene; //场景加载完成后 记录当前场景
    public string NowScene { get { return nowScene; } }

    string lastScene; //上个场景名
    string nextScene; //下个场景名

    string accessPointName; //出入口
    string uiCanvas; //UI界面

    Slider LoadingBar; //进度条
    public AsyncOperation asyncOp;

    //初始化
    public override bool Initialize()
    {
        return true;
    }

    //开始执行
    public override void Run(object userData)
    {

    }

    //更新
    public override void OnUpdate()
    {

    }

    //释放
    public override void Dispose()
    {

    }

    public void LoadScene(string sceneName,string accessPointName)
    {
        nowScene = "LoadingScene"; //当前场景名
        lastScene = SceneManager.GetActiveScene().name; //记录上个场景名
        nextScene = sceneName; //记录下个场景名
        this.accessPointName = accessPointName; //记录出入口
        
        //判断场景 设定UI界面
        switch (sceneName)
        {
            case "Start": //开始界面
                uiCanvas = "StartCanvas"; //开始UI界面
                break;
            default:
                uiCanvas = "GameCanvas"; //设定UI界面
                break;
        }

        StartCoroutine("LoadingScene"); //进入loading场景 加载下个场景
    }

    IEnumerator LoadingScene()
    {
        asyncOp = SceneManager.LoadSceneAsync("Loading");

        //loading场景加载 未完成
        while (!asyncOp.isDone)
            yield return null;

        //Loading场景加载 完成后
        SysModuleManager.Instance.GetSysModule<SysPool>().RemoveAllObj(); //移除所有 已激活 缓存池物体
        LoadingBar = FindObjectOfType<Slider>(); //获取进度条
        StartCoroutine("LoadNextScene"); //开启加载下个场景携程
    }

    IEnumerator LoadNextScene()
    {
        asyncOp = SceneManager.LoadSceneAsync(nextScene);

        //场景加载 未完成
        while (!asyncOp.isDone) 
        {
            //刷新加载进度条
            if (asyncOp.progress < 0.9f)
                LoadingBar.value = asyncOp.progress;
            else
                LoadingBar.value = 1.0f;
            yield return null;
        }

        yield return null;

        //场景加载完成后 加载玩家角色
        if (accessPointName != null)
        {
            if (accessPointName.Equals("Load")) //载入存档
                SysModuleManager.Instance.GetSysModule<SysPlayerManager>().LoadRole(transform);
            else if(accessPointName.Equals("SpawnPoint")) //出生点
                SysModuleManager.Instance.GetSysModule<SysPlayerManager>().LoadRole(GameObject.FindGameObjectWithTag("SpawnPoint").transform);
            else
            {
                //寻找场景中的 出入口位置
                GameObject[] accessPoints = GameObject.FindGameObjectsWithTag("AccessPoint");
                for (int i = 0; i <= accessPoints.Length; i++)
                {
                    //遍历完毕 没有找到匹配的地图出入点
                    if (i >= accessPoints.Length)
                    {
                        if (accessPoints.Length < 1) //地图不存在出入点
                        {
                            SysModuleManager.Instance.GetSysModule<SysPlayerManager>().LoadRole(transform);
                            break;
                        }

                        //加载玩家角色到第一个出入点
                        SysModuleManager.Instance.GetSysModule<SysPlayerManager>().LoadRole(accessPoints[0].transform);
                        break;
                    }

                    //找到匹配的 地图出入点
                    if (accessPoints[i].name == accessPointName)
                    {
                        //加载玩家角色
                        SysModuleManager.Instance.GetSysModule<SysPlayerManager>().LoadRole(accessPoints[i].transform);
                        break;
                    }
                }
            }      
        }

        yield return null;

        //加载UI界面
        SysModuleManager.Instance.GetSysModule<SysUIEnv>().LoadCanvas(uiCanvas);

        //加载完成后 记录当前场景名
        nowScene = nextScene; 
    }
}
