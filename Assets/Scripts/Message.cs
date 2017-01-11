using UnityEngine;
using System.Collections;
using System;

public class Message : Bean {

    protected ByteArray buff;
    protected override bool reading()
    {
        throw new NotImplementedException();
    }

    protected override bool writing()
    {
        throw new NotImplementedException();
    }
    public void read(ByteArray ba)
    {
        buff = ba;
    }
    public void write()
    {

    }
    public ByteArray getBuff()
    {
        return buff;
    }
    public int getId()
    {
        return 0;
    }
}
