using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
public class EnterFrameManager
{
    static EnterFrameManager instance;
    public static EnterFrameManager GetInstance()
    {
        if (instance == null)
            instance = new EnterFrameManager();
        return instance;
    }

    public void Register(Action method)
    {

    }
}
