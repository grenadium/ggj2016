using UnityEngine;
using System.Collections;

public class WindowScript : MonoBehaviour {
    private Animator animator;
    private AnimatorStateInfo currentState, previousState;
    public bool isOpened, isMoving;
    private Vector3 previousPosition;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        currentState = animator.GetCurrentAnimatorStateInfo(0);
        previousState = currentState;
        previousPosition = transform.position;

        isOpened = false;
        isMoving = false;

    }

    //Animate the window
    public void changeWindowState()
    {
        //open window
        if(!isOpened && !isMoving)
        {
            animator.Play(GlobalVariables.ANIM_WINDOW_OPENING);
        }

        //close window
        if(isOpened && !isMoving)
        {
            animator.Play(GlobalVariables.ANIM_WINDOW_CLOSING);
        }

    }

    //Fonctions appelees en fin d'animation ouverture / fermeture
    public void setWindowOpened()
    {
        isOpened = true;
        isMoving = false;
    }

    public void setWindowClosed()
    {
        isOpened = false;
        isMoving = false;
    }

    // Update is called once per frame
    void Update ()
    {

    }

}
