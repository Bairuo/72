using UnityEngine;
using System.Collections;

public class JoyStick : MonoBehaviour {
    public RectTransform rectT_Viewport;
    public RectTransform rectT_Joy;//将获取坐标作为摇杆键值
    public int r_;
    void Start()
    {
        r_ = (int)rectT_Viewport.sizeDelta.x / 2;
    }
    public void On_Move(RectTransform rect_)
    {
        if (rect_.anchoredPosition.magnitude > r_)
        {//将摇杆限制在 半径 r_ 以内
            rect_.anchoredPosition = rect_.anchoredPosition.normalized * r_;
        }
    }
}
