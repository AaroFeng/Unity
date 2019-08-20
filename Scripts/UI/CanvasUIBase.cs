using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasUIBase : MonoBehaviour
{
    protected SysSceneManager sceneManager; //场景管理器模块
    protected SysUIEnv uiEnv; //UI环境模块
    protected PlayerCharacter player; //玩家属性组件

    public virtual void OnEnable()
    {

        sceneManager = SysModuleManager.Instance.GetSysModule<SysSceneManager>();
        uiEnv = SysModuleManager.Instance.GetSysModule<SysUIEnv>();

        GameObject _player = GameObject.FindGameObjectWithTag("Player");
        if (_player != null)
            player = _player.GetComponent<PlayerCharacter>();

    }

    public virtual void OnUpdate()
    {

    }
}
