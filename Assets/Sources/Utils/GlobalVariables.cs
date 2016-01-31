﻿using UnityEngine;
using System.Collections;

/**
* Liste de variables globales (tags, noms d'objets...)
**/
public class GlobalVariables : MonoBehaviour {

    //TAG LIST
    public static string T_FLY = "FlyPlayer";
    public static string T_HUMAN = "HumanPlayer";
    public static string T_WINDOW= "Window";
    public static string T_JAM = "Jam";
    public static string T_MENU_ITEM = "MenuItem";

    //NAME LIST

    //ANIM LIST
    public static string ANIM_WINDOW_OPENING = "Opening";
    public static string ANIM_WINDOW_OPENED = "Opened";
    public static string ANIM_WINDOW_CLOSING = "Closing";
    public static string ANIM_WINDOW_CLOSED = "Closed";

    //FLOAT LIST
    public static float JAM_LIFE_TIME = 20f; //in seconds
    public static float JAM_STICK_TIME = 5f; //in seconds
}
