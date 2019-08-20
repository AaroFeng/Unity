using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyParticle : ParticleBase
{
    public void Play(EnemyState orcSaState)
    {
        switch (orcSaState)
        {
            case EnemyState.Attack:
                ParticlePlay(particleList[0]); //播放粒子效果组
                break;
            case EnemyState.Damage:
                RandomPositionDirection(particleList[1]);
                ParticlePlay(particleList[1]);
                break;
            default:
                //Debug.Log(playerState.ToString() + ":无此类型粒子效果组");
                break;
        }
    }

    public void Stop(EnemyState orcSaState)
    {
        switch (orcSaState)
        {
            case EnemyState.Attack:
                ParticleStop(particleList[0]); //停止粒子效果组
                break;
            case EnemyState.Damage:
                ParticleStop(particleList[1]);
                break;
            default:
                //Debug.Log(playerState.ToString() + ":无此类型粒子效果组");
                break;
        }
    }

    void RandomPositionDirection(GameObject particles) //受伤溅血 随机 位置与方向
    {
        Transform par = particles.transform;
        par.localPosition = new Vector3(0, Random.Range(0.5f, 2.0f), 0); //随机高度
        par.localRotation = Quaternion.Euler(Random.Range(-60, 60), Random.Range(0, 360), 0); //随机方向
    }
}
