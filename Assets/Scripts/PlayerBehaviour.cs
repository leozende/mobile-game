using UnityEngine;

/// <summary> 
/// Responsible for moving the player automatically and 
/// reciving input. 
/// </summary> 
[RequireComponent(typeof(Rigidbody))]
public class PlayerBehaviour : MonoBehaviour
{
    /// <summary> 
    /// A reference to the Rigidbody component 
    /// </summary> 
    private Rigidbody rb;

    [Tooltip("How fast the ball moves left/right")]
    [SerializeField] private float dodgeSpeed = 5;

    [Tooltip("How fast the ball moves forwards automatically")]
    [Range(0, 10)]
    [SerializeField] private float rollSpeed = 5;

    [Header("Swipe Properties")]

    [Tooltip("How far will the player move upon swiping")]
    [SerializeField] private float swipeMove = 2f;

    [Tooltip("How far must the player swipe before we will execute the action")]
    [SerializeField] private float minSwipeDistance = 2f;

    /// <summary>
    /// Stores the starting position of mobile touche events
    /// </summary>
    private Vector2 touchStart;

    // Start is called before the first frame update
    void Start()
    {
        // Get access to our Rigidbody component 
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// FixedUpdate is called at a fixed framerate and is a prime place to put
    /// Anything based on time.
    /// </summary>
    void FixedUpdate()
    {
        // Movement in the x axis
        float horizontalSpeed = 0;
        
        //Check if we are running either in the Unity editor or in a Standalone build.
        #if UNITY_STANDALONE || UNITY_WEBPLAYER

        // Check if we're moving to the side
        horizontalSpeed = Input.GetAxis("Horizontal") * dodgeSpeed;

        //if the mouse is held down (or the screen is tapped on mobile)
        if(Input.GetMouseButton(0))
            horizontalSpeed = CalculateMovement(Input.mousePosition);
        
        //Check if we are running on a mobile device
        #elif UNITY_IOS || UNITY_ANDROID

        //Check if Input has registered more than zero touches
        if(Input.touchCount > 0)
        {
            //Store the first touch detected
            Touch myTouch = Input.touches[0];

            //Uncomment to use left and right movement
            //horizontalSpeed = CalculateMovement(myTouch.position);

            SwipeTeleport(myTouch);
        }

        #endif

        // Apply our auto-moving and movement forces;
        rb.AddForce(horizontalSpeed, 0, rollSpeed);
    }

    /// <summary>
    /// Will figure out where to move the player horizontally
    /// </summary>
    /// <param name="pixelPos">The position the player has touched/clicked on </param>
    /// <returns>The direction to move in the x axis</returns>
    float CalculateMovement(Vector3 pixelPos)
    {
        // Converts to a 0 to 1 scale
        var worldPos = Camera.main.ScreenToViewportPoint(pixelPos);

        float xMove = 0;

        //If we press the right side of the screen
        if (worldPos.x < 0.5f)
            xMove = -1;
        else // Otherwise we're on the left
            xMove = 1;

        //Replace horizontalSpeed with our own value
        return xMove * dodgeSpeed;
    }

    /// <summary>
    /// Will teleport the player if swiped to the left or right
    /// </summary>
    /// <param name = "myTouch"> Current touch event </param>

    private void SwipeTeleport(Touch myTouch)
    {
        // Check if the touch just started
        if(myTouch.phase == TouchPhase.Began)
            touchStart = myTouch.position; //If so, set touchStart
        
        // If the touch has ended
        else if (myTouch.phase == TouchPhase.Ended)
        {
            // Get the position the touch ended at
            Vector2 touchEnd = myTouch.position;

            // Calculate the difference between the beginning and end of the touch on the x axis
            float x = touchEnd.x - touchStart.x;

            // if we are not moving far enough, don't do the teleport
            if (Mathf.Abs(x) < minSwipeDistance)
                return;
            
            Vector3 moveDirection;

            // If moved negatively in the x axis, move left
            if(x < 0)
                moveDirection = Vector3.left;
            else // Otherwise we're on the right
                moveDirection = Vector3.right;
            
            RaycastHit hit;

            //Only move if we wouldn't hit something
            if(!rb.SweepTest(moveDirection, out hit, swipeMove))
                rb.MovePosition(rb.position + (moveDirection * swipeMove)); //Move the Player
            
        }
    }


}