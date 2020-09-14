using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
    public void adminScene()
    {
        //kullanıcı admin i seçerse admin ekranına gidiyor
        SceneManager.LoadScene("admin");
    }
    public void userScene()
    {
        //kullanıcı user ı seçerse user ekranına gidiyor
        SceneManager.LoadScene("user");
    }

}
