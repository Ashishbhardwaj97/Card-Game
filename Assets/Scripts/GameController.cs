using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private GameObject btn;

    [SerializeField]
    private Sprite bgImage;

    [SerializeField]
    private TextMeshProUGUI Turns;

    [SerializeField]
    private TextMeshProUGUI Matches;

    public Sprite[] puzzles;

    public List<Sprite> gamePuzzles = new List<Sprite>();

    public List<Button> btns = new List<Button>();
    public List<Animator> animators = new List<Animator>();

    private bool firstGuess, secondGuess;

    private int countGuesses;
    private int countCorrectGuesses;
    private int gameGuesses;
    private int firstGuessIndex, secondGuessIndex;

    private string firstGuessPuzzle, secondGuessPuzzle;

    private AudioSource audioSource;
    public AudioClip gameOverClip;
    public AudioClip cardMatchClip;
    public AudioClip cardMismatchClip;

    private void Awake()
    {
        puzzles = Resources.LoadAll<Sprite>("Character");
    }

    private void Start()
    {
        Matches.text = PlayerPrefs.GetInt("MatchesScore", 0).ToString();
        Turns.text = PlayerPrefs.GetInt("TurnsScore", 0).ToString();

        audioSource = GetComponent<AudioSource>();

        GetButtons();
        AddListeners();
        AddGamePuzzles();
        Shuffle(gamePuzzles);
        gameGuesses = gamePuzzles.Count / 2;
    }

    void GetButtons()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("PuzzleButton");

        for (int i = 0; i < objects.Length; i++)
        {
            btns.Add(objects[i].GetComponent<Button>());
            btns[i].image.sprite = bgImage;
        }

        for (int i = 0; i < objects.Length; i++)
        {
            animators.Add(objects[i].GetComponent<Animator>());
        }
    }

    void AddGamePuzzles()
    {
        int looper = btns.Count;
        int index = 0;

        for (int i = 0; i < looper; i++)
        {
            if (index == looper / 2)
            {
                index = 0;
            }

            gamePuzzles.Add(puzzles[index]);
            index++;
        }
    }

    void AddListeners()
    {
        foreach(Button btn in btns)
        {
            btn.onClick.AddListener(() => PickAPuzzle());
        }
    }

    public void PickAPuzzle()
    {
        string currentSelectedButtonName = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name;

        if(!firstGuess)
        {
            firstGuess = true;
            firstGuessIndex = int.Parse(currentSelectedButtonName);
            firstGuessPuzzle = gamePuzzles[firstGuessIndex].name;

            btns[firstGuessIndex].image.sprite = gamePuzzles[firstGuessIndex];
        }
        else if(!secondGuess)
        {
            secondGuess = true;
            secondGuessIndex = int.Parse(currentSelectedButtonName);
            secondGuessPuzzle = gamePuzzles[secondGuessIndex].name;

            btns[secondGuessIndex].image.sprite = gamePuzzles[secondGuessIndex];
            countGuesses++;
            Turns.text = countGuesses.ToString();

            StartCoroutine(CheckIfPuzzleMatch());
        }
    }

    IEnumerator CheckIfPuzzleMatch()
    {
        yield return new WaitForSeconds(1f);

        if(firstGuessPuzzle == secondGuessPuzzle)
        {
            yield return new WaitForSeconds(0.5f);

            btns[firstGuessIndex].interactable = false; 
            btns[secondGuessIndex].interactable = false;
            btns[firstGuessIndex].image.color = new Color(0, 0, 0, 0);
            btns[secondGuessIndex].image.color = new Color(0, 0, 0, 0);
            if (audioSource != null && cardMatchClip != null)
            {
                audioSource.clip = cardMatchClip;
                audioSource.Play();
            }
            CheckIfGameIsFinished();
        }
        else
        {
            yield return new WaitForSeconds(0.5f);

            if (audioSource != null && cardMismatchClip != null)
            {
                audioSource.clip = cardMismatchClip;
                audioSource.Play();
            }
            btns[firstGuessIndex].image.sprite = bgImage;
            btns[secondGuessIndex].image.sprite = bgImage;
        }
        yield return new WaitForSeconds(0.5f);
        firstGuess = secondGuess = false;
    }

    void CheckIfGameIsFinished()
    {
        countCorrectGuesses++;
        Matches.text = countCorrectGuesses.ToString();

        if (countCorrectGuesses == gameGuesses)
        {
            print("Done");
            Matches.text = "0";
            Turns.text = "0";

            if (audioSource != null && gameOverClip != null)
            {
                // Play the AudioClip
                audioSource.clip = gameOverClip;
                audioSource.Play();
            }
        }
    }

    void Shuffle(List<Sprite> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Sprite temp = list[i];
            int randomIndex = Random.Range(0, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("TurnsScore", int.Parse(Turns.text));
        PlayerPrefs.SetInt("MatchesScore", int.Parse(Matches.text));
        PlayerPrefs.Save();
    }
}
