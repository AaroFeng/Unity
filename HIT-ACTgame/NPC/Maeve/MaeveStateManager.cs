using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaeveStateManager : NPCStateManagerBase
{
    protected override void SetStartState()
    {
        //设定状态属性值
        idleCDMin = 40.0f;
        idleCDMax = 70.0f;
        patrolSpeed = 3.5f;

        //添加状态
        AddState<NPCStateIdle>();
        AddState<NPCStatePatrol>();
        //初始状态
        ChangeState<NPCStateIdle>();
    }
}
