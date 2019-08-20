using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SysPlayerManager : SysModule
{
    GameObject playerRole; //玩家当前角色
    public GameObject PlayerRole { get { return playerRole; } }

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
        if (playerRole == null || playerRole.activeSelf == false) 
            return;

        //获取状态机管理器
        PlayerStateManager stateManager = playerRole.GetComponent<PlayerStateManager>();
        //更新状态机
        if (stateManager != null)
            stateManager.OnUpdate();
        else
            Debug.LogError("当前角色状态机管理器为空！");
    }

    //释放
    public override void Dispose()
    {

    }

    //加载角色
    public void LoadRole(Transform TransPoint) //场景出入口 或 出生点
    {
        if (SysModuleManager.Instance.GetSysModule<SysSceneManager>().asyncOp.isDone)
        //从缓存池生成 角色
        playerRole = SysModuleManager.Instance.GetSysModule<SysPool>().CreateObj("RoleAnnika");
        //设定 角色位置
        playerRole.transform.position = TransPoint.position;
        playerRole.transform.rotation = TransPoint.rotation;
        //加载主摄像机
        SysModuleManager.Instance.GetSysModule<SysPool>().CreateObj("PlayerCamera");
    }
}
