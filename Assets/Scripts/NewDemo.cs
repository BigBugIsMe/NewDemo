using UnityEngine;
using System.Collections;

public class NewDemo : MonoBehaviour {
    /// <summary>
    /// 目标
    /// </summary>
    [SerializeField]
    GameObject m_target;
    /// <summary>
    /// 位置
    /// </summary>
    [SerializeField]
    Vector3 m_TargetPos;
    float m_Velocity;
    int V_ID;
    // Use this for initialization
	void Start () {
        Debug.Log("This is just a test script");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnGUI()
    {
        if(GUI.Button(new Rect(20,20,50,30),"ok"))
        {
            
        }
    }
    void OnRenderImage()
    {
        RenderTexture s = RenderTexture.GetTemporary(1280, 640, 0);
    }
}
