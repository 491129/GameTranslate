using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Restart : MonoBehaviour
{
    public Text StText;
    float sr;
    private void Start()
    {
        StText.text=AlarmManager.ST.ToString("F2");
    }
    public void restart()
    {
        SceneManager.LoadScene("SampleScene");
        AlarmManager.ST = 0;

    }
}
