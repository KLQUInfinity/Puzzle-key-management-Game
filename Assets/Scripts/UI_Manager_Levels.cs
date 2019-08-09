using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_Manager_Levels : MonoBehaviour
{
    /*هذا الملف يتحكم في كل الui الخاص بالمستويات فقط*/

    /*هذان المتغيران يحملون الpause menu والloading screen*/
    public Transform pauseMenu, loadingContainer;

    /*هذا المتغير يتحمل الbuttons الموجود في الui الخاص بالمستوي*/
    public Button[] mainUIBtn;

    /*هذا المتغير يحمل صور متغيرات الصوت والموسيقي للتحكم باشكالهم عند الضغط عليهم عن طريق المتغير الذي اسفله*/
    public Image[] audioBtns;
    public Sprite[] audioBtnSprites;

    //public Slider healthSlider;

    /*هذان المتغيران لحمل مصادر الصوت سواء موسيقي او الصوت */
    private AudioSource sound, music;

    /*هذه الداله تعمل اول واحده في هذا الclass فقط 
    واقوم فيها باستحضار بيانات الصوت والموسيقي من الذاكره لفتح وقفل الموسيقي او الصوت*/
    void Awake()
    {
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

    /*هذه الداله تنادي كل عدد غير ثابت من الframes واقوم فيها بالتاكد هل تم الضغط علي زرار escape او لا لفتح قائمة الpause بدون الضفط علي زرار pause*/
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            sound.mute = !sound.mute;
            Toggel_PauseMenu();
            sound.mute = !sound.mute;
        }
    }

    /*هذه الداله المسئوله عن فتح او قفل قائمة pause*/
    public void Toggel_PauseMenu()
    {
        Move_Piece.checkEnable = !Move_Piece.checkEnable;
        sound.Play();
        Time.timeScale = (Time.timeScale == 0) ? 1 : 0;

        pauseMenu.gameObject.SetActive(!pauseMenu.gameObject.activeSelf);
        foreach (Button i in mainUIBtn)
        {
            i.interactable = !i.interactable;
        }
    }

    /*public void DieMenu()
    {
        if (healthSlider.value <= 0)
        {
            Time.timeScale = (Time.timeScale == 0) ? 1 : 0;
            pauseMenu.GetChild(0).GetChild(3).gameObject.SetActive(false);
            pauseMenu.gameObject.SetActive(!pauseMenu.gameObject.activeSelf);
            foreach (Button i in mainUIBtn)
            {
                i.interactable = !i.interactable;
            }
        }
    }*/

    /*هذه الداله المسئوله عن اعاده تحميل المستوي في حاله الضفط علي زرار restart*/
    public void RestartLevel()
    {
        sound.Play();
        Time.timeScale = 1;
        loadingContainer.gameObject.SetActive(true);
        StartCoroutine(LoadAsynchronously(SceneManager.GetActiveScene().buildIndex));
    }

    /*هذه الداله المسئوله عن الانتقال للقائمه الرئيسيه*/
    public void ToMainMenu()
    {
        sound.Play();
        Time.timeScale = 1;
        loadingContainer.gameObject.SetActive(true);
        StartCoroutine(LoadAsynchronously(0));
    }

    /*هذه الداله المسئوله عن الانتقال للمستوي التالي في حاله الفوز*/
    public void NextLevel()
    {
        sound.Play();
        Time.timeScale = 1;
        loadingContainer.gameObject.SetActive(true);
        StartCoroutine(LoadAsynchronously(SceneManager.GetActiveScene().buildIndex + 1));
    }

    /*هذه الداله تنادي عند الحاجه الي الانتقال او اعاده المستوي وتقوم باظهار الloading screen*/
    IEnumerator LoadAsynchronously(int sceneNum)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneNum);
        while (!operation.isDone)
        {
            loadingContainer.GetChild(0).GetComponent<Slider>().value = operation.progress;
            yield return null;
        }
    }

    /*هذه الداله المسئوله عن التحكم في جميع buttons الموجوده في الpause menu*/
    public void PauseMenuUI(int index)
    {
        sound.Play();
        switch (index)
        {
            case 0:
                audioBtns[0].sprite = (audioBtns[0].sprite == audioBtnSprites[1]) ? audioBtnSprites[0] : audioBtnSprites[1];
                int musicValue = (audioBtns[0].sprite == audioBtnSprites[0]) ? 1 : 0;
                PlayerPrefs.SetInt("Music", musicValue);
                music.mute = !music.mute;
                break;
            case 1:
                audioBtns[1].sprite = (audioBtns[1].sprite == audioBtnSprites[3]) ? audioBtnSprites[2] : audioBtnSprites[3];
                int soundValue = (audioBtns[1].sprite == audioBtnSprites[2]) ? 1 : 0;
                PlayerPrefs.SetInt("Sound", soundValue);
                sound.mute = !sound.mute;
                break;
        }
    }
}
