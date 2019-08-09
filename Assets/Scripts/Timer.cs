using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    /*هذا الملف خاص با timer وحساب الوقت*/

    private static Timer instance;
    /*هذا المتغير استعمله لادخل الي هذا الملف من اللمفات الاخري وهذه تسمي singlton design pattern*/

    public static Timer Instance{ get { return instance; } }
    /* من هنا  انادي المتغير الذي اعلاه*/

    public float timerSecond;
    public int timerMin;

    public bool finished;

    /*هذي الداله هيا اول داله تستدعي في هذا الملف*/
    void Awake()
    {
        instance = this;
        finished = false;
    }

    /*هذي الداله تنادي كل عدد ثابت من الframes ويتم فيها تشغيل الtimer او ايقافه*/
    void FixedUpdate()
    {
        if (timerSecond > 0 && !finished)
        {
            timerSecond -= Time.deltaTime;
            if (timerSecond <= 0 && timerMin > 0)
            {
                timerSecond = 60;
                timerMin--;
            }
            GetComponent<Text>().text = "Time Left\n" + timerMin + ":" + (int)timerSecond;
        }
        else if (timerSecond <= 0)
        {
            timerSecond = 1;
            LevelManager.Instance.checkEnd = true;
            finished = true;
            GetComponent<Text>().text = "Time Left\n00:00";
        }
    }
}
