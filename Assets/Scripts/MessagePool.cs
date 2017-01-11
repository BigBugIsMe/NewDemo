using UnityEngine;
using System.Collections;

public class MessagePool {

    static MessagePool instance;
    public static MessagePool GetInstance()
    {
        if (instance == null)
            instance = new MessagePool();
        return instance;
    }

    public Message GetMessageInstance(int messageId)
    {
        return null;
    }
}
