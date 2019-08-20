using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public List<GameObject> particles;

    private void OnCollisionEnter(Collision collision)
    {
        //碰撞目标有角色控制器
        if(collision.collider.GetComponent<CharacterController>() != null)
            GetComponent<Collider>().isTrigger = true; //关闭碰撞器碰撞 防止产生碰撞挤压 卡移BUG

        //随机获取碰撞粒子效果
        ParticleSystem particle = particles[Random.Range(0, particles.Count - 1)].GetComponent<ParticleSystem>();
        //获取碰撞点 并赋值粒子效果position
        particle.transform.position = collision.contacts[0].point;
        //播放粒子效果
        particle.Play();
    }

    private void OnCollisionExit(Collision collision)
    {
        //停止粒子效果
        for (int i = 0; i < particles.Count; i++)
        {
            particles[i].GetComponent<ParticleSystem>().Stop();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //停止粒子效果
        for (int i = 0; i < particles.Count; i++)
        {
            particles[i].GetComponent<ParticleSystem>().Stop();
        }

        GetComponent<Collider>().isTrigger = false;
    }
}
