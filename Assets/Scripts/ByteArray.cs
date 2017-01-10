using UnityEngine;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
public class ByteArray {

    MemoryStream m_Stream = new MemoryStream();
    BinaryReader m_Reader = null;
    BinaryWriter m_Writer = null;
    public ByteArray(byte[] b)
    {

    }
    public int ReadInt()
    {
        return m_Reader.ReadInt32();
    }
}
