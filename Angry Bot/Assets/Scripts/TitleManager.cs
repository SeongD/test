using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    public InputField nameInput;
    public GameObject bestDate;
    public Text bestUserData;

    public void GoPlay() 
    {
        if (nameInput.text == "")
            return;

        PlayerPrefs.SetString("UserName", nameInput.text);
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainPlay");
    }

    public void BestScore() 
    {
        bestUserData.text = "";
        for (int i = 1; i <= 3; i++)
        {
            if (PlayerPrefs.HasKey("BestPlayer" + i))
            {
                bestUserData.text += string.Format("{0}. {1}:{2:N0}\n",
                    i, PlayerPrefs.GetString("BestPlayer" + i), PlayerPrefs.GetFloat("BestScore" + i));            
            }
        }
        bestDate.SetActive(true);
    }

    public void Quit() 
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
