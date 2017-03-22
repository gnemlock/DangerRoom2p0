using UnityEngine;

/*
 * this is a block of code
 * 
 * */

public class BasicMovement : MonoBehaviour
{
    public float speed = 1;
    private string horizontalTag = "Horizontal";
    private string verticalTag = "Vertical";
    
    /// <summary>
    /// Update this instance.
    /// </summary>
    void Update()
    {
        float verticalInput = Input.GetAxis(verticalTag) * speed * Time.deltaTime;
        float horizontalInput = Input.GetAxis(horizontalTag) * speed * Time.deltaTime;
        
        transform.Translate(horizontalInput, verticalInput, 0);
    }
}
