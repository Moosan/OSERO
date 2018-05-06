using UnityEngine;

public class MouseInput : MonoBehaviour
{
    public GameObject Brack, White;
    public GameObject Prefab;
    public Vector3 PrefabPos;
    public GameObject AI;
    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var screenPoint = Input.mousePosition;
            screenPoint.z = 10;
            var v = GetComponent<Camera>().ScreenToWorldPoint(screenPoint);
            PrefabPos = new Vector3(Mathf.Floor(v.x + 0.5f), 0, Mathf.Floor(v.z + 0.5f));
            var obj = GameController.Turn ? Brack : White;
            Prefab = Instantiate(obj, PrefabPos, Quaternion.Euler(GameController.Turn ? 0 : 180, 0, 0));
        }
        if (Input.GetMouseButton(0))
        {
            var screenPoint = Input.mousePosition;
            screenPoint.z = 10;
            var v = GetComponent<Camera>().ScreenToWorldPoint(screenPoint);
            PrefabPos=new Vector3(Mathf.Floor(v.x + 0.5f), 0, Mathf.Floor(v.z + 0.5f));
            if(Prefab)Prefab.transform.position = PrefabPos;
        }
        if (Input.GetMouseButtonUp(0))
        {
            Destroy(Prefab);
            if (AI && !GameController.Turn)
            {
                GameObject.FindWithTag("GameController")
                .SendMessage("MoveText","あんたのターンちゃうで～");
                return;
            }
            var screenPoint = Input.mousePosition;
            screenPoint.z = 10;
            var v = GetComponent<Camera>().ScreenToWorldPoint(screenPoint);
            GameObject.FindWithTag("GameController")
                .SendMessage("PutCoin", new Vector3(Mathf.Floor(v.x + 0.5f), 0, Mathf.Floor(v.z + 0.5f)));
        }
        if (Input.GetMouseButtonDown(1))
        {
            int count = 1;
            if (AI) {
                count = 2;
            }
            GameObject.FindWithTag("GameController").SendMessage("ReTurnChange",count);
            
        }
    }
}


