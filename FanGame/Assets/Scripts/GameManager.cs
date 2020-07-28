using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using CodeMonkey.Utils;
using CodeMonkey;


public class GameManager : MonoBehaviour
{
    [SerializeField] private CameraFollow cameraFollow;
    public Transform followTransform;
    [SerializeField] private bool cameraAim;
    bool gameHasEnded = false;
    public float restartDelay = 2f;
    private void Start()
    {
        followTransform = GameObject.FindGameObjectWithTag("Player").transform;
        cameraFollow.Setup(GetCameraPosition, () => 60f, true, true);
    }
    private Vector3 GetCameraPosition()
    {
        if (cameraAim)
        {
            Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
            Vector3 playerToMouseDirection = mousePosition - followTransform.position;
            return followTransform.position + playerToMouseDirection * .22f;
        }
        else
        {

            return followTransform.position;

        }    }
    public void EndGame()
    {
        if(gameHasEnded == false)
        {
            Debug.Log("GAMEOVER");
            gameHasEnded = true;
            Invoke("Restart", restartDelay);
        }  
    }
 
    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
    
    

}
