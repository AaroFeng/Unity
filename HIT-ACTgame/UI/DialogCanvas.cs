using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogCanvas : MonoBehaviour
{
    public GameObject tips; //提示信息

	void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}

    public void Tips(string tipsText)
    {
        tips.SetActive(true); //打开提示UI
        tips.GetComponentInChildren<Text>().text = tipsText; //设置提示文字内容
        StartCoroutine("TipsClickClose");
    }

    IEnumerator TipsClickClose()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        while(true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                tips.SetActive(false); //关闭提示UI
                break;
            }
            else
                yield return null;
        }

        yield break;
    }
}
