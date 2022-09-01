using UnityEngine;
using UnityEngine.SceneManagement; // LoadScene 

public class ObstacleBehaviour : MonoBehaviour
{

    [Tooltip("How long to wait before restarting the game")]
    public float waitTime = 2.0f;
    [Tooltip("Get gameobject")]
    [SerializeField] private GameObject obstacleObject;

    private void Start()
    {
        ////Change obstacle colors
        obstacleObject.GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f);
    }

    void OnCollisionEnter(Collision collision)
    {
        // First check if we collided with the player 
        if (collision.gameObject.GetComponent<PlayerBehaviour>())
        {
            // Destroy the player 
            Destroy(collision.gameObject);

            // Call the function ResetGame after waitTime has passed 
            Invoke("ResetGame", waitTime);
        }
    }

    /// <summary> 
    /// Will restart the currently loaded level 
    /// </summary> 
    void ResetGame()
    {
        // Restarts the current level 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}