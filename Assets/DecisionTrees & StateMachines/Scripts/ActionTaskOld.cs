using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTaskOld : MonoBehaviour
{
    public virtual void Action() 
    {
        BlackBoard.instance.health ++;
    }
}

public class ConditionTask : MonoBehaviour 
{
    public virtual void Condition()
    {
        if (BlackBoard.instance.health == 0) 
        {
            //die Action Task
        }
    }
}

public class Task : MonoBehaviour
{
    
}

public class MoveTask : ActionTaskOld 
{
    
}

public class BlackBoard : MonoBehaviour
{
    public static BlackBoard instance;
    public int health;
    public string name;
    public int ammo;

    private void Awake()
    {
        instance = this;
    }
}