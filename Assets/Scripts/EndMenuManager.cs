using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndMenuManager : MonoBehaviour
{
    /*هذا الملف يتحكم في المشهد النهائي الذي يعرض النتائج*/

    /*هذا المتغير يحمل الloading screen*/
    public Transform loadingContainer;

    /*هذان المتغيران لحمل مصادر الصوت سواء موسيقي او الصوت */
    private AudioSource sound, music;

    /*هذا المتغير يحمل كل اشكال المستويات في المشهد*/
    public GameObject[] Levels;

    /*هذه الداله تعمل اول واحده في هذا الclass فقط 
    واقوم فيها باستحضار بيانات الصوت والموسيقي من الذاكره لفتح وقفل الموسيقي او الصوت*/
    void Awake()
    {
        GetData();
        sound = GetComponent<AudioSource>();
        music = GameMaster.Instance.gameObject.GetComponent<AudioSource>();
        if (PlayerPrefs.HasKey("Music"))
        {
            music.mute = (PlayerPrefs.GetInt("Music") == 1) ? false : true;
        }

        if (PlayerPrefs.HasKey("Sound"))
        {
            sound.mute = (PlayerPrefs.GetInt("Sound") == 1) ? false : true;
        }
    }

    /*هذه الداله تستحضر البيانات الخاصه بكل مستوي وتعرضها للاعب*/
    void GetData()
    {
        for (int i = 0; i < 5; i++)
        {
            LevelData levelData = new LevelData(i + 1);
            Levels[i].transform.GetChild(0).GetComponent<Text>().text = levelData.BestScore + " $\n" + levelData.timerMin + " : " + levelData.timerSecond;
            Levels[i].transform.GetChild(1).gameObject.SetActive(true);
            for (int j = 0; j < levelData.StarsNum; j++)
            {
                Levels[i].transform.GetChild(1).GetChild(j).gameObject.SetActive(true);
            }

        }
    }

    /*هذه الداله المسئوله عن الانتقال للقائمه الرئيسيه*/
    public void ToMainMenu()
    {
        sound.Play();
        Time.timeScale = 1;
        loadingContainer.gameObject.SetActive(true);
        StartCoroutine(LoadAsynchronously(0));
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
}
