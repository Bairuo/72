using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurtainController : MonoBehaviour
{

    public void EnterNetStart()
    {
        SceneManager.LoadScene("NetStart");
    }
}
