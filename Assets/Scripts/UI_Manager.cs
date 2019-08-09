using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*هذا الفايل يتحتوي علي two classe واحده لاستحضار البيانات الخاصه بالمستويات
واخري للتحكم في الUI الخاص بالقائمه الرئيسيه*/
public class LevelData
{
    /*هذا الclass المسئول عن استحضار البيانات*/
    public LevelData(int levelNum)
    {
        string data = PlayerPrefs.GetString(levelNum.ToString());
        if (data.Equals(""))
        {
            return;
        }
        string[] allData = data.Split('&');
        StarsNum = int.Parse(allData[0]);
        BestScore = int.Parse(allData[1]);
        timerMin = int.Parse(allData[2]);
        timerSecond = int.Parse(allData[3]);
    }

    public int BestScore{ set; get; }

    public int StarsNum{ set; get; }

    public int timerMin{ set; get; }

    public int timerSecond{ set; get; }

}

public class UI_Manager : MonoBehaviour
{
    /*هذا الclass المسئول عن التحكم في القائمه الرئيسيه*/

    /*هذا المتغير الذي يحمل كل الui الموجود في القائمه الرئسيه */
    public Transform uiContainer;

    /*هذان المتغيران المسئولان عن الbutton التي تكون موجوده في القائمه الفرعيه التي نختار منها المستوي*/
    public GameObject levelBtnContainer, levelWindow;

    /*هذا المتغير يستخدم للتاكد من فتح المستوي للاعب ام لا*/
    private bool nextLevelLocked = false;

    /*هذا المتغير يحمل الbuttons الموجود بقائمة اختيار المستوي*/
    public Button[] girdBtns;

    /*هذا المتغير يحمل الbuttons الخاصه بالموسيقي والصوت*/
    public Image[] audioBtns;

    /*هذا المتغير يحمل جميل اللوان الbuttons الخاصه بالموسيقي للتغير بينها عند غلق الموسيقي او الصوت او فتحهمها */
    public Sprite[] audioBtnSprites;

    /*هذان المتغيران لحمل مصادر الصوت سواء موسيقي او الصوت */
    private AudioSource sound, music;

    /*هذا المتغير يستخدم في عمليه فتح قائمه اختيار المتسويات اذا كان هناك اكتر من قائمها ويستخدم لتحديد ارقامهم للتنقل بينهم*/
    private int gridIndex = -1;

    public Sprite[] tutorialSprites;
    private int tutorialSpriteIndex;

    /*هذه الداله تعمل اول واحده في هذا الclass فقط 
    واقوم فيها باستحضار بيانات الصوت والموسيقي من الذاكره لفتح وقفل الموسيقي او الصوت*/
    void Awake()
    {
        tutorialSpriteIndex = 0;
        sound = GetComponent<AudioSource>();
        music = GameMaster.Instance.gameObject.GetComponent<AudioSource>();
        if (PlayerPrefs.HasKey("Music"))
        {
            audioBtns[0].sprite = (PlayerPrefs.GetInt("Music") == 1) ? audioBtnSprites[0] : audioBtnSprites[1];
            music.mute = (PlayerPrefs.GetInt("Music") == 1) ? false : true;
        }

        if (PlayerPrefs.HasKey("Sound"))
        {
            audioBtns[1].sprite = (PlayerPrefs.GetInt("Sound") == 1) ? audioBtnSprites[2] : audioBtnSprites[3];
            sound.mute = (PlayerPrefs.GetInt("Sound") == 1) ? false : true;
        }
    }

    /*هذه الداله تنادي بعد الداله اعلاها بالنسبه لهذا الclass فقط
    واقوم فيها بتكوين الbutton في قائمة اختيار المستوي واستعاده بيانات كل مستوي هل هو لعب من قبل واضع التقيم الخاصه به المتمثل في النجوم
    او هذا المستوي المفترض ان تكمل من عنده 
    او هذا المستوي لم يفتح لتلعبه بعد*/
    void Start()
    {
        //x num of levels, y num of gridcontainers, z num of gridcontainers that i suppost start with 
        int x = 1, y = -1, z = -1;
        GameObject gridcontainer = new GameObject();
        GameObject[] levels = Resources.LoadAll<GameObject>("Levels");

        foreach (GameObject i in levels)
        {
            if (x == 1)
            {
                gridcontainer = Instantiate(levelBtnContainer)as GameObject;
                gridcontainer.transform.SetParent(levelWindow.transform, false);
            }
            x++;
            string[] ss = i.name.Split('_');
            int sceneNum = int.Parse(ss[1].ToString());
            GameObject s = Instantiate(i)as GameObject;
            s.transform.SetParent(gridcontainer.transform, false);

            LevelData level = new LevelData(sceneNum);
            if (!nextLevelLocked)
            {
                s.transform.GetChild(0).GetComponent<Text>().text = sceneNum.ToString();
            }
            else
            {
                s.transform.GetChild(0).GetComponent<Text>().text = "";
            }
            s.GetComponent<Button>().interactable = !nextLevelLocked;
            s.transform.GetChild(1).gameObject.SetActive(!nextLevelLocked);

            if (level.StarsNum == 0)
            {
                //not played yet
                nextLevelLocked = true;
                if (z == -1)
                {
                    z = y;
                }
            }
            else if (level.StarsNum == 3)
            {
                //three stars
                for (int j = 0; j < level.StarsNum; j++)
                {
                    s.transform.GetChild(1).GetChild(j).gameObject.SetActive(true);
                }
            }
            else if (level.StarsNum == 2)
            {
                //two stars
                for (int j = 0; j < level.StarsNum; j++)
                {
                    s.transform.GetChild(1).GetChild(j).gameObject.SetActive(true);
                }
            }
            else if (level.StarsNum == 1)
            {
                //one star
                for (int j = 0; j < level.StarsNum; j++)
                {
                    s.transform.GetChild(1).GetChild(j).gameObject.SetActive(true);
                }
            }


            s.GetComponent<Button>().onClick.AddListener(() => LoadLevel(sceneNum));
            if (x == 6)
            {
                x = 1;
                gridcontainer = null;
                y++;
            }
        }
        if (z == -1)
        {
            z = y;
        }
        gridIndex = z;
        showLevelGrid(gridIndex);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    /*هذه الداله المسئوله عن التنقل من المشهد الرئيسي والذهاب الي المشهد الخاص بالمستوي الذي ضغط عليه ليفتح اذا كان مسموح اللعب فيه*/
    private void LoadLevel(int sceneNum)
    {
        uiContainer.GetChild(3).gameObject.SetActive(true);
        StartCoroutine(LoadAsynchronously(sceneNum));
    }

    /*هذي الداله تنادي من الداله اعلاها لتظهر قائمه الloading قبل فتح المستوي*/
    IEnumerator LoadAsynchronously(int sceneNum)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneNum);
        while (!operation.isDone)
        {
            uiContainer.GetChild(3).GetChild(0).GetComponent<Slider>().value = operation.progress;
            yield return null;
        }
    }

    /*هذه الداله تتحكم في كل الbuttons الموجوده في القائمه شرح اللعبه او التعليمات*/
    public void TutorialUI(int index)
    {
        sound.Play();
        switch (index)
        {
            case 0:
                uiContainer.GetChild(1).gameObject.SetActive(true);
                uiContainer.GetChild(4).gameObject.SetActive(false);
                PlayerPrefs.SetInt("tutorial", 0);
                break;
            case 1:
                tutorialSpriteIndex++;
                ShowTutorialSprites(tutorialSpriteIndex);
                break;
            case -1:
                tutorialSpriteIndex--;
                ShowTutorialSprites(tutorialSpriteIndex);
                break;
        }
    }

    void ShowTutorialSprites(int index)
    {
        uiContainer.GetChild(4).GetChild(3).GetComponent<Image>().sprite = tutorialSprites[index];


        if (index == 0)
        {
            uiContainer.GetChild(4).GetChild(1).GetComponent<Button>().interactable = true; //nextBtn
            uiContainer.GetChild(4).GetChild(2).GetComponent<Button>().interactable = false; //previousBtn
        }
        else if (index > 0 && index < tutorialSprites.Length - 1)
        {
            uiContainer.GetChild(4).GetChild(1).GetComponent<Button>().interactable = true; //nextBtn
            uiContainer.GetChild(4).GetChild(2).GetComponent<Button>().interactable = true; //previousBtn
        }
        else if (index == tutorialSprites.Length - 1)
        {
            uiContainer.GetChild(4).GetChild(1).GetComponent<Button>().interactable = false; //nextBtn
            uiContainer.GetChild(4).GetChild(2).GetComponent<Button>().interactable = true; //previousBtn
        }
    }

    /*هذه الداله تتحكم في كل الbuttons الموجوده في القائمه الرئيسيه*/
    public void mainMenuUI(int index)
    {
        sound.Play();
        switch (index)
        {
            case 0:
                uiContainer.GetChild(2).gameObject.SetActive(true);
                uiContainer.GetChild(1).gameObject.SetActive(false);
                break;
            case 1:
                Application.Quit();
                break;
            case 2:
                audioBtns[0].sprite = (audioBtns[0].sprite == audioBtnSprites[1]) ? audioBtnSprites[0] : audioBtnSprites[1];
                int musicValue = (audioBtns[0].sprite == audioBtnSprites[0]) ? 1 : 0;
                PlayerPrefs.SetInt("Music", musicValue);
                music.mute = !music.mute;
                break;
            case 3:
                audioBtns[1].sprite = (audioBtns[1].sprite == audioBtnSprites[3]) ? audioBtnSprites[2] : audioBtnSprites[3];
                int soundValue = (audioBtns[1].sprite == audioBtnSprites[2]) ? 1 : 0;
                PlayerPrefs.SetInt("Sound", soundValue);
                sound.mute = !sound.mute;
                break;
            case 4:
                tutorialSpriteIndex = 0;
                ShowTutorialSprites(tutorialSpriteIndex);
                uiContainer.GetChild(1).gameObject.SetActive(false);
                uiContainer.GetChild(4).gameObject.SetActive(true);
                break;
        }
    }

    /*هذه الداله المسئوله عن التحكم في كل الbuttons في قائمة اختيار المستوي
    وحيث انهم 5 مستويات فقط فستظهر مجموعه واحده فقط*/
    public void levelSelectionUI(int index)
    {
        sound.Play();
        switch (index)
        {
            case 0:
                uiContainer.GetChild(1).gameObject.SetActive(true);
                uiContainer.GetChild(2).gameObject.SetActive(false);
                break;
            case 1:
                showLevelGrid(++gridIndex);
                break;
            case -1:
                showLevelGrid(--gridIndex);
                break;
        }
    }

    /*هذه الداله تظهر مجموعه المستويات التي عليها الدور للعب*/
    void showLevelGrid(int index)
    {
        for (int i = 0; i < levelWindow.transform.childCount; i++)
        {
            if (i == index)
            {
                levelWindow.transform.GetChild(i).gameObject.SetActive(true);
                continue;
            }
            levelWindow.transform.GetChild(i).gameObject.SetActive(false);
        }

        if (index == 0)
        {
            girdBtns[0].interactable = true; //nextBtn
            girdBtns[1].interactable = false;//previousBtn
        }
        else if (index > 0 && index < levelWindow.transform.childCount - 1)
        {
            girdBtns[0].interactable = true; //nextBtn
            girdBtns[1].interactable = true;//previousBtn
        }
        else if (index == levelWindow.transform.childCount - 1)
        {
            girdBtns[0].interactable = false; //nextBtn
            girdBtns[1].interactable = true;//previousBtn
        }
    }
}
