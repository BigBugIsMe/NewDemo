using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Text;
using System.Net;
namespace Core.Net.Sockets
{
    public class ByteSocket
    {
        /// <summary>
        /// socket引用
        /// </summary>
        Socket m_Socket = null;
        /// <summary>
        /// 消息的包头长度,4字节
        /// </summary>
        const int MSG_HEADER_LENGTH = 4;
        /// <summary>
        /// 每一帧处理的消息上限
        /// </summary>
        const int MAX_MSG_FRAME = 30;
        /// <summary>
        /// 消息缓冲区
        /// </summary>
        MemoryStream m_StreamBuffer = new MemoryStream();
        /// <summary>
        /// 接收消息的线程
        /// </summary>
        Thread m_ReceiveThread;
        /// <summary>
        /// 消息队列
        /// </summary>
        Queue<Message> m_MsgQueue = new Queue<Message>();
        /// <summary>
        /// 接收到的消息长度
        /// </summary>
        int m_RecvMsgLength;
        /// <summary>
        /// 解包后的消息体长度
        /// </summary>
        int m_MsgBodyLength;
        /// <summary>
        /// 包头消息体，一般用于解析消息Id,看需求
        /// </summary>
        byte[] m_RecvMsgHead = new byte[MSG_HEADER_LENGTH];
    }
}

