using UnityEngine;
using System.Collections;

public class Move_Piece : MonoBehaviour
{
    /*هذا الملف مسئول عن تحريك كل قطعه وعن استقبالها في مكانها الصحيح وعن تحريك المستودع لاختيار منه قطعه*/

    /*هذا المتغير خاص بحالة القطعه الواحده هل هي مرفوعه pickedup ام مازلت في المستودع scrolling ام خرجت من الموستود وليست مرفوعه وليست في مكانها الصحيح idle
   ام موضوعه في مكانها الصحيح locked */
    public string pieceStatus = "scrolling";

    /*هذا المتغير المسئول عن المؤثرات التي تحدث عند وضع القطعه في مكانها الصحيح (دخان)*/
    public GameObject edgeParticles;

    private string checkPlacement = "n";

    /*هذا المتغير مسئول عن ايقاف كل دوال هذا الملف في حاله الضغط علي زر pause*/
    public static bool checkEnable;

    /*هذه الداله تعمل اول واحده بالنسبه لهذا الملف فقط 
    وانا استخدمها لاعداد المتغير الذي اعلاها*/
    void Awake()
    {
        checkEnable = true;
    }
        
    /*هذه الداله تنادي كل عدد غير ثابت من الframes واقوم بها بتحريك القطعه مع الموس بعد الضغط القطعه من المستودع */
    void Update()
    {
        InvControl();
        if (pieceStatus.Equals("pickedup") && checkEnable)
        {
            Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 objPos = Camera.main.ScreenToWorldPoint(mousePos);
            transform.position = objPos;
        }
    }

    /*هذه الداله تنادي عندما يحدث تصادم بين الورقعه واماكن الرقع ويتم التاكد اذا كان تم الضغط علي الورقه مره اخره لتوضع في مكانها او لا 
     * واذا وضعت في المكان الصحيح يتم اضافه 10 نقط للscore واذا وضعت في مكان خطاء يتم انقاص 5 نقط من الscore */
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag.Equals("Slot"))
        {
            if (other.GetComponent<SpriteNameMatch>().spriteName.Equals(gameObject.GetComponent<SpriteRenderer>().sprite.name) && checkPlacement.Equals("y"))
            {
                LevelManager.Instance.nPieces--;
                string[] s = LevelManager.Instance.scoreText.text.Split(':');
                int score = int.Parse(s[1]);
                LevelManager.Instance.scoreText.text = "Score:" + (score + 10).ToString();
                LevelManager.Instance.puzzleHolder.GetComponent<AudioSource>().clip = LevelManager.Instance.rightPlacementSound[Random.Range(0, 2)];
                LevelManager.Instance.puzzleHolder.GetComponent<AudioSource>().Play();
                pieceStatus = "locked";
                other.GetComponent<BoxCollider2D>().enabled = false;
                GetComponent<BoxCollider2D>().enabled = false;
                transform.position = other.gameObject.transform.position;
                checkPlacement = "n";
                GetComponent<Renderer>().sortingOrder = -1;
                GameObject particle = Instantiate(edgeParticles, other.gameObject.transform.position, edgeParticles.transform.rotation)as GameObject;
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                Destroy(particle, 1f);
            }
            if (!other.GetComponent<SpriteNameMatch>().spriteName.Equals(gameObject.GetComponent<SpriteRenderer>().sprite.name) && checkPlacement.Equals("y"))
            {
                string[] s = LevelManager.Instance.scoreText.text.Split(':');
                int score = int.Parse(s[1]);
                LevelManager.Instance.scoreText.text = "Score:" + (score - 5).ToString();
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, .5f);
                checkPlacement = "n";
            }
        }
    }

    /*هذه الداله المسئوله عن رفع ووضع القطع*/
    void OnMouseDown()
    {
        if (checkEnable)
        {
            if (pieceStatus.Equals("idle") || pieceStatus.Equals("scrolling"))
            {
                pieceStatus = "pickedup";
                checkPlacement = "n";
                GetComponent<Renderer>().sortingOrder = 10;
            }
            else if (pieceStatus.Equals("pickedup"))
            {
                pieceStatus = "idle";
                checkPlacement = "y";
                GetComponent<Renderer>().sortingOrder = 0;
            }
        }
    }

    /*هذه الداله المسئوله عن تحريك القطع في المستودع صعودا وهبوطا لتختار القطعه المناسبه*/
    void InvControl()
    {
        if (pieceStatus.Equals("scrolling") && checkEnable)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                transform.position = new Vector2(transform.position.x, transform.position.y - 2.4f);
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                transform.position = new Vector2(transform.position.x, transform.position.y + 2.4f);
            }
        }
    }
}
