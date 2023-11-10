using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class PlayManager : MonoBehaviour
{
    public bool playEnd;
    public float limitTime;
    public int enemyCount;

    public Text timeLabel;
    public Text enemyLabel;
    public GameObject finalGUI;
    public Text finalMessage;
    public Text finalScoreLabel;

    public Text PlayerName;

    private void Start()
    {
        enemyLabel.text = string.Format("Enemy : {0}", enemyCount);
        timeLabel.text = string.Format("Time : {0:N2}", limitTime);

        PlayerName.text = PlayerPrefs.GetString("UserName");
    }

    private void Update()
    {
        if (limitTime > 0)
        {
            limitTime -= Time.deltaTime;
            timeLabel.text = string.Format("Time : {0:N2}", limitTime);
        }
        else
            GameOver();
    }

    public void EnemyDie() 
    {
        enemyCount--;
        enemyLabel.text = string.Format("Enemy : {0}", enemyCount);

        limitTime += 5f;

        if (enemyCount <= 0)
            Clear();
    }

    public void Clear() 
    {
        if (!playEnd) 
        {
            Time.timeScale = 0;
            playEnd = true;
            finalMessage.text = "Clear!!";

            PlayerController pc = GameObject.Find("Player").GetComponent<PlayerController>();
            float score = 12345f + limitTime * 123f + pc.hp * 123f;
            finalScoreLabel.text = string.Format("{0:N0}", score);

            finalGUI.SetActive(true);
            pc.playerState = PlayerState.Dead;

            BestCheck(score);
        }
    }

    public void GameOver() 
    {
        if (!playEnd) 
        {
            Time.timeScale = 0;
            playEnd = true;
            finalMessage.text = "Fail...";
            float score = 1234f - enemyCount * 123f;
            finalScoreLabel.text = string.Format("{0:N0}", score);
            finalGUI.SetActive(true);

            PlayerController pc = GameObject.Find("Player").GetComponent< PlayerController>();
            pc.playerState = PlayerState.Dead;

            BestCheck(score);
        }
    }

    public void Replay() 
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainPlay");
    }

    public void Quit() 
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Title");
    }

    private void BestCheck(float score) 
    {
        UserNameScore[] userScores = new UserNameScore[4];
        for (int i = 0; i < userScores.Length - 1; i++) 
        {
            int idx = i + 1;
            userScores[i] = new UserNameScore();
            userScores[i].score = PlayerPrefs.GetFloat("BestScore" + idx);
            userScores[i].name = PlayerPrefs.GetString("BestPlayer" + idx);
        }

        userScores[3] = new UserNameScore();
        userScores[3].score = score;
        userScores[3].name = PlayerPrefs.GetString("UserName");

        userScores = userScores.OrderByDescending(item => item.score).ToArray();

        for (int i = 0;i < userScores.Length - 1;i++) 
        {
            if (userScores[i].score > 0) 
            {
                int idx = i + 1;
                PlayerPrefs.SetFloat("BestScore" + idx, userScores[i].score);
                PlayerPrefs.SetString("BestPlayer" + idx, userScores[i].name);
            }
        }
            
        PlayerPrefs.Save();
    }
}
