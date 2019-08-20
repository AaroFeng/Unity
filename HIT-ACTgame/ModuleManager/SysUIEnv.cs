using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//观察者模式
public delegate void ButtonDown();

public class SysUIEnv : SysModule
{
    GameObject canvas; //当前UI
    DialogCanvas dialogCanvas = new DialogCanvas(); //对话与信息UI
    public DialogCanvas DialogCanvas
    {
        get
        {
            if (dialogCanvas == null || !dialogCanvas.gameObject.activeSelf) 
                dialogCanvas = SysModuleManager.Instance.GetSysModule<SysPool>().CreateObj("DialogCanvas").GetComponent<DialogCanvas>();
            return dialogCanvas;
        }
    }

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
        if (canvas != null)
        {
            //更新界面UI
            canvas.GetComponent<CanvasUIBase>().OnUpdate();
            //更新场景UI
            if (canvas.name == "GameCanvas")
                canvas.GetComponent<SceneUI>().OnUpdate();   
        }
    }

    //释放
    public override void Dispose()
    {

    }

    //按钮事件 执行事件中的方法
    public void RunBtnEvent(ButtonDown _event)
    {
        if (_event != null)
            _event();
    }

    //加载UI界面
    public void LoadCanvas(string canvasName)
    {
        canvas = SysModuleManager.Instance.GetSysModule<SysPool>().CreateObj(canvasName);

        if (canvas == null)
            Debug.LogError("不存在该UI界面！或UI界面名字有误！");
    }
}
