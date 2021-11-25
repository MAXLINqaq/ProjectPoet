using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameplayController : MonoBehaviour
{

    public string[] queue;
    public GameObject[] platforms;

    public PlayerController playerController;

    public int j;
    public  bool isWaitingForChangeColor;

    private List<string> poems = new List<string>();

    private Color colorRed = new Vector4(0.8f, 0, 0, 1f);
    private Color colorBlack = new Vector4(0f, 0f, 0f, 0f);

    // Start is called before the first frame update
    private void Awake()
    {
        j = 0;
        isWaitingForChangeColor = true;
    }

    // Update is called once per frame
    void Update()
    {
        ChangeColor();
    }
    private void ChangeColor()
    {
        if (isWaitingForChangeColor)
        {
            for (int i = 0; i < platforms.Length; i++)
            {
                if (platforms[i].tag == queue[j])
                {
                    platforms[i].GetComponentInChildren<SpriteRenderer>().color = colorRed;
                    platforms[i].GetComponentInChildren<Text>().color = Color.white;
                }
                else
                {

                    platforms[i].GetComponentInChildren<SpriteRenderer>().color = Color.white;
                    platforms[i].GetComponentInChildren<Text>().color = Color.black;
                }
            }
            isWaitingForChangeColor = false;
        }
    }
    private void OnCollisionEnter2D(Collision2D coll)
    {

        if (coll.gameObject.tag == queue[j])
        {
            isWaitingForChangeColor = true;
            poems.Add(queue[j]);
            j++;
            if (j == queue.Length)
            {
                j = 0;
                poems.Add("。\n");
            }
        }
        else if (coll.gameObject.tag == "Untagged")
        {

        }
        else
        {
            playerController.isDead = true;
            j = 0;
            isWaitingForChangeColor = true;
            for (int i = poems.Count - 1; i >= 0; i--)
            {
                poems.Remove(poems[i]);
            }
        }
    }
}
