using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovePlate : MonoBehaviour
{
    public GameObject controller, stepDialog;

    GameObject reference = null;

    //Board Positions
    int BoardX;
    int BoardY;

    //false : movement, true: attacking
    public bool attack = false;



    //Functions
    public void Start()
    {
        if (attack)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f); 
        }
    }

    public void Update()
    {
   
    }

    public void OnMouseUp()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        if (attack)
        {
            GameObject cp = controller.GetComponent<Game>().GetPosition(BoardX, BoardY);
                /*if (!stepDialog.activeInHierarchy)
                {
                    Debug.Log("Toggled!");
                    stepDialog.SetActive(true);
                    Time.timeScale = 0f;
                }
                else
                {
                Debug.Log("Toggledoff!");
                    stepDialog.SetActive(false);
                    Time.timeScale = 1f;
                }*/
            Destroy(cp);
        } 

        controller.GetComponent<Game>().SetPositionEmpty(reference.GetComponent<Chessman>().GetXBoard(),
            reference.GetComponent<Chessman>().GetYBoard());

        reference.GetComponent<Chessman>().SetXBoard(BoardX);
        reference.GetComponent<Chessman>().SetYBoard(BoardY);
        reference.GetComponent<Chessman>().SetCoords();

        controller.GetComponent<Game>().SetPosition(reference);

        if (!controller.GetComponent<Game>().IsGameOver())
            controller.GetComponent<Game>().NextTurn();

        reference.GetComponent<Chessman>().DestroyMovePlate();
    }

    

    public void SetCoords(int x, int y)
    {
        BoardX = x;
        BoardY = y;
    }

    public void SetReference(GameObject obj)
    {
        reference = obj;
    }

    public GameObject GetReference()
    {
        return reference;
    }

}
