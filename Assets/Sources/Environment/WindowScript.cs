using UnityEngine;
using System.Collections;

public class WindowScript : MonoBehaviour {

    public bool isOpened, isMoving;
    public float openingHeight; //hauteur d'ouverture de la fenetre
    public float speed;
    public int numberOfFlyAttacksRequired = 7;
    private int numberOfFlyAttacks = 0;

    private Vector3 openedPos, closedPos, destination;

    // Use this for initialization
    void Start () {

        openedPos = new Vector3(transform.position.x, transform.position.y + openingHeight, transform.position.z);
        closedPos = transform.position;
        destination = closedPos;

        isOpened = false;
        isMoving = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position == openedPos)
            isOpened = true;
        else isOpened = false;
        if (transform.position != destination)
        {
            isMoving = true;
            transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * speed);
        }
        else
            isMoving = false;    
    }

    //Animate the window
    public void changeWindowState()
    {
        //open window
        if(!isOpened && !isMoving)
        {
            isMoving = true;
            destination = openedPos;
        }

        //close window
        if(isOpened && !isMoving)
        {
            isMoving = true;
            destination = closedPos;
        }
    }

    void OnCollisionEnter (Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Tapette"))
        {
            isMoving = true;
            destination = closedPos;
        }
        else if (collision.gameObject.tag == "Fly" && numberOfFlyAttacks < numberOfFlyAttacksRequired)
        {
            isMoving = true;
            numberOfFlyAttacks++;
            destination = Vector3.Lerp(transform.position, openedPos, (float)numberOfFlyAttacks / (float)numberOfFlyAttacksRequired);
        }
    }

    
}
