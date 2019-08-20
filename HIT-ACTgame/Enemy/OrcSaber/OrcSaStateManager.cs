using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OrcSaStateManager : EnemyStateManagerBase
{
    protected override void SetStartState()
    {
        //添加状态
        AddState<EnemyStateIdle>();
        AddState<EnemyStatePatrol>();
        AddState<EnemyStateChase>();
        AddState<EnemyStateAttack>();
        AddState<EnemyStateDamage>();
        AddState<EnemyStateDeath>();
        //初始状态
        ChangeState<EnemyStateIdle>();
    }
}
