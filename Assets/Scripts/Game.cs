using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using UnityEngine.Networking;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    [Header("UX Fields")]
    public Text questionText;
    public Button optionButton1;
    public Button optionButton2;
    public Button optionButton3;
    public Button optionButton4;
    public Button optionButton5;
    public Text timeCounter;
    public Text currentLevel;
    public Text killCount;
    public Text madLife;
    public Image bg;

    [Header("Level Rules")]
    public string thisLevel = "1-1";
    public bool isready = false;
    public bool playQ = false;
    public bool useSum = true;
    public bool useSub = false;
    public bool useMul = false;
    public bool useDiv = false;
    public bool usePar = true; // group elements in parenthesis
    public bool useNeg = false; // allow negative results
    public int Decimals = 0; // returns decimal numbers (0 for ints)
    public float minBaseNumber = 0f; // bottom random number for elem (use negs if needed)
    public float maxBaseNumber = 9f; // top random number for elem
    public int opsNumElems = 3;  // number of elements (ie  1 + 4 + 5) is 3 elems
    public int levelNumElems = 10; // number of questions in level
    public int numberOptions = 3; // number of possible answers
    public int maxseconds = 15; // number of seconds allowed per question

    [Header("Catalogs")]
    public List<string> Questions; // container for all questions in level
    public List<string> Options; // container for all options in level (item[0] is the right one)

    [Header("Managers")]
    private TextAsset jsonFile; // file that contains level data
    private string filePath = "";
    private string result = "";
    private static Game instance = null;
    private GameObject top;
    private GameObject bottom;
    private GameObject timer;
    private GameObject play;

    [Header("Game Vars")]
    public int currentQ = 0;
    public int totalKills = 0;
    public int currentmadlife = 3;
    private bool countDown = false;
    private float currentTime = 15;

    [Header("Anims")]
    public GameObject laserPrefab;
    public AudioClip LaserBeamSound;
    public AudioClip BadHitSound;
    public AudioSource myAudioSource;
    public AudioClip clockTock;
    public AudioClip madJump;

    [Header("Positions")]
    private Vector3 MadStartPos;
    private Vector3 BadStartPos;
    private Vector3 BGStartPos;

    


    // Levels :https://docs.google.com/spreadsheets/d/19pmOPtS59Rp_mYpKgt2lWl72NJnmpq4cHgykQZM7GmY/edit#gid=0 

    // Create Singleton to maintain Vars
    public static Game Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (Game)FindObjectOfType(typeof(Game));
            }
            return instance;
        }
    }
    void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }


    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            Reset();
        }
        else
        {
            Debug.Log(SceneManager.GetActiveScene().name);
        }

    }

    private void Reset()
    {
        countDown = false;

        BadStartPos = Bad.instance.transform.position;
        MadStartPos = Mad.instance.transform.position;
        BGStartPos = bg.transform.position;

        top = GameObject.FindWithTag("top");
        bottom = GameObject.FindWithTag("bottom");
        timer = GameObject.FindWithTag("timer");
        play = GameObject.FindWithTag("play");
        LoadLevel(thisLevel);

    }


    private void Update()
    {
        if (countDown)
        {



                currentTime -= Time.deltaTime;
            timeCounter.text = Math.Round(currentTime,0).ToString();



                if (currentTime < 0)
                    {
                        if (myAudioSource.isPlaying)
                            myAudioSource.Stop();
                        countDown = false;
                        currentTime = 15;
                        optionButton1.GetComponentInChildren<Text>().text = "";
                        clickOption(optionButton1.GetComponentInChildren<Text>());
                    }


        }

    }


    public void clearGUI()
    {
        questionText.text = "";
        optionButton1.GetComponentInChildren<Text>().text = "";
        optionButton2.GetComponentInChildren<Text>().text = "";
        optionButton3.GetComponentInChildren<Text>().text = "";
        optionButton4.GetComponentInChildren<Text>().text = "";
        optionButton5.GetComponentInChildren<Text>().text = "";
    }

    // Reset all vars
    public void initLevel()
    {
        isready = false;
        Questions = new List<string>();
        Options = new List<string>();
        currentQ = 0;

        top.SetActive(false);
        bottom.SetActive(false);
        timer.SetActive(false);
        play.SetActive(true);

        clearGUI();
    }

    // Enable Screen to Play Current Question
    public void PlayLevel()
    {
        if (isready)
        {
            if (currentQ < Questions.Count)
            {
                top.SetActive(true);
                bottom.SetActive(true);
                timer.SetActive(true);
                play.SetActive(false);

                questionText.text = Questions[currentQ];
                countDown = true;

                myAudioSource.clip = clockTock;
                myAudioSource.loop = true;
                myAudioSource.volume = 0.10f;
                myAudioSource.Play();


                string CurrentOptions = Options[currentQ];
                string[] myOption = CurrentOptions.Split(new string[] { "," }, StringSplitOptions.None);
                myOption = reshuffle(myOption);

                switch (numberOptions)
                {
                    case 2:
                        optionButton1.gameObject.SetActive(false);
                        optionButton1.GetComponentInChildren<Text>().text = "";
                        optionButton2.gameObject.SetActive(true);
                        optionButton2.GetComponentInChildren<Text>().text = myOption[0];
                        optionButton3.gameObject.SetActive(false);
                        optionButton3.GetComponentInChildren<Text>().text = "";
                        optionButton4.gameObject.SetActive(true);
                        optionButton4.GetComponentInChildren<Text>().text = myOption[1];
                        optionButton5.gameObject.SetActive(false);
                        optionButton5.GetComponentInChildren<Text>().text = "";
                        break;
                    case 3:
                        optionButton1.gameObject.SetActive(false);
                        optionButton1.GetComponentInChildren<Text>().text = "";
                        optionButton2.gameObject.SetActive(true);
                        optionButton2.GetComponentInChildren<Text>().text = myOption[0];
                        optionButton3.gameObject.SetActive(true);
                        optionButton3.GetComponentInChildren<Text>().text = myOption[1];
                        optionButton4.gameObject.SetActive(true);
                        optionButton4.GetComponentInChildren<Text>().text = myOption[2];
                        optionButton5.gameObject.SetActive(false);
                        optionButton5.GetComponentInChildren<Text>().text = "";
                        break;
                    case 4:
                        optionButton1.gameObject.SetActive(true);
                        optionButton1.GetComponentInChildren<Text>().text = myOption[0];
                        optionButton2.gameObject.SetActive(true);
                        optionButton2.GetComponentInChildren<Text>().text = myOption[1];
                        optionButton3.gameObject.SetActive(false);
                        optionButton3.GetComponentInChildren<Text>().text = "";
                        optionButton4.gameObject.SetActive(true);
                        optionButton4.GetComponentInChildren<Text>().text = myOption[2];
                        optionButton5.gameObject.SetActive(true);
                        optionButton5.GetComponentInChildren<Text>().text = myOption[3];
                        break;
                    case 5:
                        optionButton1.gameObject.SetActive(true);
                        optionButton1.GetComponentInChildren<Text>().text = myOption[0];
                        optionButton2.gameObject.SetActive(true);
                        optionButton2.GetComponentInChildren<Text>().text = myOption[1];
                        optionButton3.gameObject.SetActive(true);
                        optionButton3.GetComponentInChildren<Text>().text = myOption[2];
                        optionButton4.gameObject.SetActive(true);
                        optionButton4.GetComponentInChildren<Text>().text = myOption[3];
                        optionButton5.gameObject.SetActive(true);
                        optionButton5.GetComponentInChildren<Text>().text = myOption[4];
                        break;
                }
            }
            else
            {
                nextScreen();
            }

        }  
    }


    // Evaluate Answer (and call Anims)
    public void clickOption(Text myAnswer)
    {

        var myOptions = Options[currentQ];
        string[] Answers = myOptions.Split(new string[] { "," }, StringSplitOptions.None);
        string rightAnswer = Answers[0];
        top.SetActive(false);
        bottom.SetActive(false);
        timer.SetActive(false);
        currentTime = 15;
        myAudioSource.Stop();


        if (rightAnswer == myAnswer.text)
        {
            Debug.Log("Yeah!");
            totalKills++;
            killCount.text = totalKills.ToString();
            StartCoroutine(shootBadGuy());
        }
        else
        {
            Debug.Log("Oops...");
            StartCoroutine(hitGoodGuy());
        }

    }

    public void nextScreen()
    {
        //Check if not dead (pending)
        if (currentmadlife == 0)
        {
            if (PlayerPrefs.GetInt("best") < totalKills)
                PlayerPrefs.SetInt("best", totalKills);
            PlayerPrefs.SetInt("score", totalKills);
            SceneManager.LoadScene("gameover");
        }


        if ((currentQ+1) >= Questions.Count)
        {
            switch (thisLevel)
            {
                case "1-1": thisLevel = "1-2"; break;
                case "1-2": thisLevel = "1-3"; break;
                case "1-3": thisLevel = "1-4"; break;
                case "1-4": thisLevel = "1-5"; break;
                case "1-5": thisLevel = "1-6"; break;
                case "1-6": thisLevel = "1-7"; break;
                case "1-7": thisLevel = "1-8"; break;
                case "1-8": thisLevel = "1-9"; break;
                case "1-9": thisLevel = "1-10"; break;
                case "1-10": thisLevel = "1-11"; break;
                case "1-11": thisLevel = "1-12"; break;
                case "1-12": thisLevel = "1-13"; break;
                case "1-13": thisLevel = "1-14"; break;
                case "1-14": thisLevel = "1-15"; break;
                case "1-15": thisLevel = "2-1"; break;
                case "2-1":
                    SceneManager.LoadScene("gameover");
                   break;
            }


            clearGUI();
            LoadLevel(thisLevel);
            StartCoroutine(moveMad());

        }
        else
        {
            currentQ++;
            StartCoroutine(bringinBad());

        }
    }


    IEnumerator moveMad()
    {
        Mad.instance.doAnim("Mad_Jump");
        myAudioSource.clip = clockTock;
        myAudioSource.loop = true;
        myAudioSource.volume = 0.10f;
        myAudioSource.Play();
        StartCoroutine(moveBG());
        yield return new WaitForSeconds(2.5f);
        Mad.instance.doAnim("Mad_Idle");
        myAudioSource.Stop();
        StartCoroutine(bringinBad());
    }

    IEnumerator moveBG()
    {
        bg.transform.DOMoveX(bg.transform.position.x - 500, 1.5f, true);
        yield return new WaitForSeconds(1.5f);
    }

    IEnumerator bringinBad()
    {
        yield return new WaitForSeconds(0);
        Bad.instance.doAnim("Bad_Idle");
        Bad.instance.transform.DOMove(BadStartPos,1);
        PlayLevel();
    }


    IEnumerator shootBadGuy()
    {
        countDown = false;
        laserPrefab.SetActive(true);
        myAudioSource.PlayOneShot(LaserBeamSound);
        Mad.instance.doAnim("Mad_Shoot");
        Bad.instance.doAnim("Bad_Hurt");
        Bad.instance.transform.DOMoveX(Bad.instance.transform.position.x + 10, 1f, true);
        yield return new WaitForSeconds(1.5f);
        laserPrefab.SetActive(false);
        Bad.instance.doAnim("Bad_Die");
        Mad.instance.doAnim("Mad_Idle");
        yield return new WaitForSeconds(1.5f);
        Bad.instance.transform.position = new Vector3(Bad.instance.transform.position.x + 1000, Bad.instance.transform.position.y, Bad.instance.transform.position.z);

        nextScreen();
    }

    IEnumerator hitGoodGuy()
    {
        Bad.instance.doAnim("Bad_Walk");

        var tempPos = new Vector3(Bad.instance.transform.position.x, Bad.instance.transform.position.y, 0);

        Bad.instance.transform.DOMove(new Vector3(Mad.instance.transform.position.x+ 200,
               Mad.instance.transform.position.y, 0), 0.75f);

        yield return new WaitForSeconds(0.75f);
        Bad.instance.doAnim("Bad_Hit");
        Mad.instance.doAnim("Mad_Hurt");
        myAudioSource.clip = BadHitSound;
        myAudioSource.loop = true;
        myAudioSource.Play();

        yield return new WaitForSeconds(1f);


        myAudioSource.Stop();

        Bad.instance.doAnim("Bad_Idle");
        Mad.instance.doAnim("Mad_Idle");

        Bad.instance.transform.DOMove(tempPos, 1);

        currentmadlife--;
        madLife.text = currentmadlife.ToString();

        if (currentmadlife == 0)
            Mad.instance.doAnim("Mad_Die");

        nextScreen();

    }


    // Create array of Feasible questions 
    public void LoadLevel(string filename)
    {
        initLevel();
        StartCoroutine(LoadFile(filename));
        currentLevel.text = "Level " + filename;

        while (Questions.Count < levelNumElems)
        {
            var Quest = getOperation();
            var Grouped = GroupElems(Quest);

            var myResult = getResult(Grouped);
            if (useNeg && myResult < 0)
            {
                Questions.Add(Grouped);
                Options.Add(CreateOptions(myResult));
            }
            else if (myResult >= 0)
            {
                Questions.Add(Grouped);
                Options.Add(CreateOptions(myResult));
            }
        }
    }

    // Create list of Multiple Choice options
    public string CreateOptions(float answer)
    {
        var myOptions = new List<string>();
        myOptions.Add(answer.ToString());
        while (myOptions.Count < numberOptions)
        {
            var temp = getRandomNumber();
            if(!myOptions.Contains(temp.ToString()))
                myOptions.Add(temp.ToString());
        }
        string myString = string.Join(",", myOptions.ToArray());
        return myString;
    }

    // Group with Parenthesis when needeed
    public string GroupElems(List<string> thisOP)
    {
        string EvalXP = "";
        for (int i = 0; i < thisOP.Count; i++) { EvalXP = EvalXP + thisOP[i]; }

        if (usePar)
        { 
            switch (thisOP.Count)
            {
                case 3:
                    return thisOP[0] + thisOP[1] + thisOP[2];
                case 5:
                    if (UnityEngine.Random.Range(0, 1f) > 0.5)
                        return "(" + thisOP[0] + thisOP[1] + thisOP[2] + ")" + thisOP[3] + thisOP[4];
                    else
                        return thisOP[0] + thisOP[1] + "(" + thisOP[2] + thisOP[3] + thisOP[4] + ")";
                default:
                    return EvalXP;
            }
        }
        else
        {
            return EvalXP;
        }
    }

    // Calculate correct Answer
    public float getResult(string XP)
    {
        DataTable dt = new DataTable();
        var answer = dt.Compute(XP, "");
        return float.Parse(answer.ToString());
    } 

    // returns a List of numbers and operands (using setup params)
    public List<string> getOperation()
    {
        var elems = new List<string>();
        System.Random rnd = new System.Random();
        int topOP; 

        while (elems.Count < (opsNumElems+opsNumElems-1))
        {
            elems.Add(getRandomNumber().ToString());

            if (elems.Count < (opsNumElems + opsNumElems - 1))
            {
                if (useSum && useSub && useMul && useDiv) topOP = rnd.Next(4);
                else if (useSum && useSub && useMul) topOP = rnd.Next(3);
                else if (useSum && useSub) topOP = rnd.Next(2);
                else topOP = 0;  

                switch (topOP)
                {
                    case 0: elems.Add("+"); break;
                    case 1: elems.Add("-"); break;
                    case 2: elems.Add("*"); break;
                    case 3: elems.Add("/"); break;
                }
            }
        }
        return elems;
    }

    // returns a Random Number (using setup params)
    public float getRandomNumber()
    {
        return (float)Math.Round(UnityEngine.Random.Range(minBaseNumber, maxBaseNumber), Decimals);
    }

    // Load JSON File Level
    IEnumerator LoadFile(string levelname)
    {
        string myfilename = levelname + ".json";
        filePath = System.IO.Path.Combine(Application.streamingAssetsPath, myfilename);

        if (filePath.Contains("://"))
        {
            using (var w = UnityWebRequest.Get(filePath) )
            {
                result = w.downloadHandler.text;
                yield return w.SendWebRequest();
            }
      }
        else
        {
            result = System.IO.File.ReadAllText(filePath);
        }

        Levels mylevel = JsonUtility.FromJson<Levels>(result);
        useSum = mylevel.useSum;
        useSub = mylevel.useSub;
        useMul = mylevel.useMul;
        useDiv = mylevel.useDiv;
        usePar = mylevel.usePar;
        Decimals = mylevel.Decimals;
        minBaseNumber = mylevel.minBaseNumber;
        maxBaseNumber = mylevel.maxBaseNumber;
        opsNumElems = mylevel.opsNumElems;
        levelNumElems = mylevel.levelNumElems;
        numberOptions = mylevel.numberOptions;
        maxseconds = mylevel.maxseconds;
        isready = true;
        yield return true;
    }

    // Used To shuffle option array
    public string[] reshuffle(string[] texts)
    {
        // Knuth shuffle algorithm :: courtesy of Wikipedia :)
        for (int t = 0; t < texts.Length; t++)
        {
            string tmp = texts[t];
            int r = UnityEngine.Random.Range(t, texts.Length);
            texts[t] = texts[r];
            texts[r] = tmp;
        }
        return texts;
    }

}
