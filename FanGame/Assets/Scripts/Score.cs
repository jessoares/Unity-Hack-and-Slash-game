using UnityEngine.UI;
using UnityEngine;

public class Score : MonoBehaviour
{
    public int killCount;

    public GameObject spawner;
    public Text scoreText;
    private void Start()
    {
        spawner = GameObject.FindGameObjectWithTag("Manager");
    }

    private void Update()
    {
        scoreText.text = spawner.GetComponent<WaveSpawner>().killCount.ToString() + "!!";
    }


}
