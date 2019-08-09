using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    /*هذا الكود الخاص بنظام المستوي الواحد في اللعبه 
    يقوم بحساب بعمل random لقطع الpuzzle
    يقوم بانقاص حياة عند الخساره
    يقوم باظهار الشاشه الصغيره التي تظهر عند انتهاء المستوي الواحد وفيها مكان للنجوم والscore
    يقوم بحفظ بيانات اللاعب للمستوي عند المكسب فقط
    عند الخساره 3 مرات تنهي نقط الحياه ويتم حذف البيانات الخاصه بكل المستويات وتظهر الشاشه الصغيره لتخرج الي القائمة الرئيسيه */


    private static LevelManager instance;
    /*هذا المتغير استعمله لادخل الي هذا الملف من اللمفات الاخري وهذه تسمي singlton design pattern*/

    public static LevelManager Instance{ get { return instance; } }
    /* من هنا  انادي المتغير الذي اعلاه*/

    public Transform puzzleHolder, puzzleInventory, puzzleContainer;
    /*  هذي المتغيرات لل gameobjects موجود في ال في المشهد بتاع المستوي لاتمكن من التحكم فيهم*/

    private ArrayList puzzleIndex;
    /*هذا المتغير ليساعدني في اختيار random كي اقوم بتغير امكان الفطع من الinventory الذي اسحب منه القطع لاضعها في مكانها*/
    public Sprite[] pieces;
    /*هذا المتغير لحفظ صور القطع كي اقوم بلاختيار منها*/
     
    public int nPieces, p;
    /*هذان المتيغيران لحفظ عدد القطع التي لم توضع في اماكنها*/

    public Text scoreText, lifeText;
    /*هذان المتغيران يحملان قيمة الscore والlife*/
    public Transform winMenu;
    /*هذا المتغير يحتوي علي القائمة الصغيره التي تظهر عند انتهاء المستوي سواء مكسب او خساره*/
    public Button[] mainUIBtn;
    /*هذا يحمل زر الpause*/
    public float timerSecond;
    /*هذا المتغير يحمل الوقت الموجود في الtimer بالثانية*/
    public int timerMin;
    /*هذا المتغير يحمل الوقت الموجود في الtimer بالدقيقه*/
    public bool checkEnd;

    public AudioClip[] rightPlacementSound;

    /*هذه الداله تعمل اول واحده بالنسبه لهذا الملف فقط 
    وانا استخدمها لاعداد القطع وبعض الاشياء قبل اظهار المستوي امام اللاعب*/
    void Awake()
    {
        instance = this;
        lifeText.text = "Lifes: " + GameMaster.Instance.lifes;
        nPieces = puzzleHolder.childCount;
        puzzleIndex = new ArrayList();
        for (int i = 0; i < puzzleHolder.childCount; i++)
        {
            puzzleIndex.Add(i);
        }

        CreatePieces();
    }

    /*هذه الداله مسئولة عن عمل random للقطع كي يتغير ترتيبها كل مره يفتح فيها المستوي*/
    void CreatePieces()
    {
        for (int i = 0; i < puzzleHolder.childCount; i++)
        {
            int index = Random.Range(0, puzzleIndex.Count);
            int spriteIndex = (int)puzzleIndex[index];

            puzzleInventory.GetChild(0).GetChild(i).GetComponent<SpriteRenderer>().sprite = pieces[spriteIndex];
            puzzleHolder.GetChild(i).GetComponent<SpriteNameMatch>().spriteName = pieces[i].name;
            puzzleIndex.RemoveAt(index);
        }
    }

    /*هذه الداله تنادي كل عدد ثابت من الframes واقوم بها بالتاكد هل انتهت اللعبه ام لا ام انتهي الوقت ام لا */
    void FixedUpdate()
    {
        if (BeginingTimer.Instance.timerSecond == 1 && BeginingTimer.Instance.finished)
        {
            BeginingTimer.Instance.timerSecond = 2;
            puzzleInventory.gameObject.SetActive(true);
            puzzleContainer.transform.GetChild(0).gameObject.SetActive(false);
        }
        if (nPieces == 0 || checkEnd)
        {
            Move_Piece.checkEnable = false;
            p = nPieces;
            nPieces = -1;
            checkEnd = false;
            if (p == 0)
            {
                timerMin = Timer.Instance.timerMin;
                timerSecond = Timer.Instance.timerSecond;
                Timer.Instance.finished = true;

                HandlePuzzle();
            }

            FinshTheGame();
        }
    }

    /*هذه الداله تقوم بتجميع القطع بعد انتهاء المستوي بالمكسب فقط ولتجعل الصور صوره واحده فقط*/
    void HandlePuzzle()
    {
        puzzleContainer.GetComponent<AudioSource>().Play();
        puzzleContainer.transform.position = new Vector2(puzzleContainer.transform.position.x, -0.95f);
        puzzleContainer.transform.localScale = new Vector2(1, 1);
        puzzleContainer.transform.GetChild(0).gameObject.SetActive(true);
        for (int i = 0; i < puzzleHolder.childCount; i++)
        {
            puzzleInventory.GetChild(0).GetChild(i).gameObject.SetActive(false);
        }
    }

    /*هذي الداله مسئولة عن حساب التقيمات وعدد النجوم الخاصه باللاعب في المستوي الواحد
    وتحدد المكسب والخساره
    تكسب اذا وضعت كل الاوراق في امكانها الصحيحه وقبل انتهاء الوقت وتكون نقط الscore اعلي من صفر
    تخسر عندما ينتهي الوقت قبل وضع كل الاوراق في اماكنها او عندما يكون الscore اقل من صفر 
    وعند الخساره يقترح عليك اعاده المستوي لتكسبه او الخروج الي القائمه الرثيسه
    وعند الخساره3 مرات يقترح عليك العوده للقائمه الرائسيه ويتم مسح كل تقدمك في اللعبه لتبداء من جديد*/
    public void FinshTheGame()
    {
        int starNum = 0;
        string[] s = scoreText.text.Split(':');
        float score = float.Parse(s[1]);
        float percent = ((float)score) / ((float)(pieces.Length * 10));
        if (percent <= 0 || p > 0)
        {// no stars level not complete
            GameMaster.Instance.lifes--;
            PlayerPrefs.SetInt("lifes", GameMaster.Instance.lifes);
            lifeText.text = "Lifes: " + GameMaster.Instance.lifes;
            winMenu.GetChild(0).GetChild(7).gameObject.SetActive(false);
            winMenu.GetChild(0).GetChild(8).gameObject.SetActive(true);
            winMenu.GetChild(0).GetChild(2).gameObject.SetActive(false);
            if (GameMaster.Instance.lifes == 0)
            {
                winMenu.GetChild(0).GetChild(0).gameObject.SetActive(false);
                int x = PlayerPrefs.GetInt("tutorial");
                PlayerPrefs.DeleteAll();
                PlayerPrefs.SetInt("tutorial", x);
            }
        }
        else if (percent <= 1.0f && percent >= .75f)
        {// 3 stars
            for (int i = 4; i < 7; i++)
            {
                starNum++;
                winMenu.GetChild(0).GetChild(i).gameObject.SetActive(true);
            }
        }
        else if (percent < .75f && percent >= .5f)
        {// 2 stars
            for (int i = 4; i < 6; i++)
            {
                starNum++;
                winMenu.GetChild(0).GetChild(i).gameObject.SetActive(true);
            }
        }
        else if (percent < .5f && percent > 0.0f)
        {// 1 star
            for (int i = 4; i < 5; i++)
            {
                starNum++;
                winMenu.GetChild(0).GetChild(i).gameObject.SetActive(true);
            }
        }

        if (SceneManager.GetActiveScene().buildIndex == 5 && starNum != 0 && p == 0)
        {
            winMenu.GetChild(0).GetChild(0).gameObject.SetActive(false);
            winMenu.GetChild(0).GetChild(7).gameObject.SetActive(true);
            winMenu.GetChild(0).GetChild(1).gameObject.SetActive(false);
        }
        winMenu.GetChild(0).GetChild(3).GetChild(0).GetComponent<Text>().text = score.ToString();

        if (starNum != 0 && p == 0)
        {
            SavalevelData(starNum);
        }
        OpenWinWindow();
    }

    /*هذي الداله هيا المسئوله عن حفظ بيانات اللاعب الخاص بكل مستوي*/
    void SavalevelData(int starNum)
    {
        string[] s = scoreText.text.Split(':');
        float score = float.Parse(s[1]);
        string saveString = "";
        LevelData level = new LevelData(SceneManager.GetActiveScene().buildIndex);
        saveString += (starNum > level.StarsNum) ? starNum.ToString() : level.StarsNum.ToString();
        saveString += "&";
        saveString += ((int)score > level.BestScore) ? ((int)score).ToString() : level.BestScore.ToString();
        saveString += "&";
        saveString += (timerMin > level.timerMin) ? timerMin.ToString() : level.timerMin.ToString();
        saveString += "&";
        saveString += ((int)timerSecond > level.timerSecond) ? ((int)timerSecond).ToString() : level.timerSecond.ToString();
        PlayerPrefs.SetString(SceneManager.GetActiveScene().buildIndex.ToString(), saveString);
    }

    /*هذه الداله المسئوله عن اظهار القائمه الصغيره الخاصه بالتقيمات*/
    void OpenWinWindow()
    {
        foreach (Button i in mainUIBtn)
        {
            i.gameObject.SetActive(false);
        }

        winMenu.gameObject.SetActive(!winMenu.gameObject.activeSelf);
    }

}
