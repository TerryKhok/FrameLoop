using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Testply : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Gamepad gamepad = Gamepad.current;

        
        if (Input.GetKeyDown(KeyCode.L))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);

        }

        //if(gamepad != null)
        //{
        //    if (gamepad.buttonNorth.wasPressedThisFrame)
        //    {
        //        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        //    }
            
        //}
    }
}
