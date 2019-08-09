using UnityEngine;
using System.Collections;

public class GameMaster : MonoBehaviour
{
    /*هذا الملف يستمر من اول فتح اللعبه حتي قفلها شغال لانه يحمل عدد الارواح*/

    private static GameMaster instance;
    /*هذا المتغير استعمله لادخل الي هذا الملف من اللمفات الاخري وهذه تسمي singlton design pattern*/

    public static GameMaster Instance{ get { return instance; } }
    /* من هنا  انادي المتغير الذي اعلاه*/

    public int music;
    public int sound;

    public int lifes = 3;
    /*هذا المتغير الي يحمل عدد الارواح لللاعب*/

    public Transform uiContainer;

    /*هذي الداله هيا اول داله تستدعي في هذا الملف وهي مسئوله عن استمرار الملف طول فتره تشغيل اللعبه*/
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        if (PlayerPrefs.HasKey("lifes"))
        {
            lifes = PlayerPrefs.GetInt("lifes");
        }
        else
        {
            lifes = 3;
            PlayerPrefs.SetInt("lifes", GameMaster.Instance.lifes);
        }
            
        if (PlayerPrefs.HasKey("tutorial"))
        {
            if (PlayerPrefs.GetInt("tutorial") == 0)
            {
                uiContainer.GetChild(1).gameObject.SetActive(true);
                uiContainer.GetChild(4).gameObject.SetActive(false);
            }
            else if (PlayerPrefs.GetInt("tutorial") == 1)
            {
                uiContainer.GetChild(1).gameObject.SetActive(false);
                uiContainer.GetChild(4).gameObject.SetActive(true);
            }
        }
        else
        {
            uiContainer.GetChild(1).gameObject.SetActive(false);
            uiContainer.GetChild(4).gameObject.SetActive(true);
            PlayerPrefs.SetInt("tutorial", 0);
        }
    }
}
