using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<Yoshi> yoshis;
    [Header("UI objects")]
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject GameUI;
    [SerializeField] private GameObject OOT;
    [SerializeField] private GameObject DKH;
    [SerializeField] private TMPro.TextMeshProUGUI TimeText;
    [SerializeField] private TMPro.TextMeshProUGUI ScoreText;
    public static GameManager instance;

    private float startingTime = 100f;

    private float timeRemaining;
    private HashSet<Yoshi> currentYoshis = new HashSet<Yoshi>();
    private int score;
    private bool playing = false;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    public void StartGame()
    {
        startButton.SetActive(false);
        OOT.SetActive(false);
        DKH.SetActive(false);
        GameUI.SetActive(true);
        for(int i = 0; i < yoshis.Count; i++)
        {
            yoshis[i].Hide();
            yoshis[i].SetIndex(i);
        }
        currentYoshis.Clear();
        timeRemaining = startingTime;
        score = 0;
        ScoreText.text = "0";
        playing = true;
    }

    public void GameOver(int type)
    {
        if (type == 0)
        {
            OOT.SetActive(true);
        }
        else
        {
            DKH.SetActive(true);
        }
        foreach(Yoshi yoshi in yoshis)
        {
            yoshi.StopGame();
        }
        playing = false;
        startButton.SetActive(true);
    }
    void Update()
    {
        if (playing)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                GameOver(0);
            }
            TimeText.text = $"{(int)timeRemaining / 60}:{(int)timeRemaining % 60:D2}";
            if (currentYoshis.Count <= (score / 10))
            {
                int index = Random.Range(0, yoshis.Count);

                Debug.Log("YOSHI QUE APARECE: " + index+" TOTAL DE YOSHIS: "+currentYoshis.Count);

                if (!currentYoshis.Contains(yoshis[index]))
                {
                    Debug.Log("ACTIVO EL YOSHI");
                    currentYoshis.Add(yoshis[index]);
                    yoshis[index].Activate(score / 10);
                }
            }
        }
    }
    public void AddScore(int yoshiIndex)
    {
        score += 1;
        ScoreText.text = $"{score}";
        timeRemaining += 1;
        Debug.Log("MATO YOSHI " + yoshiIndex);
        currentYoshis.Remove(yoshis[yoshiIndex]);
    }
    public void Missed(int yoshiIndex,bool isYoshi)
    {
        if (isYoshi)
        {
            timeRemaining -= 2;
        }
        currentYoshis.Remove(yoshis[yoshiIndex]);
        Debug.Log("SE VA YOSHI " + yoshiIndex);
    }
}
