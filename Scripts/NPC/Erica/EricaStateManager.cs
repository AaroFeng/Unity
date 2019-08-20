using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EricaStateManager : NPCStateManagerBase
{
    protected override void SetStartState()
    {
        //设定状态属性值
        idleCDMin = 25.0f;
        idleCDMax = 35.0f;
        patrolSpeed = 1.2f;

        //添加状态
        AddState<NPCStateIdle>();
        AddState<NPCStatePatrol>();
        //初始状态
        ChangeState<NPCStateIdle>();
    }
}
