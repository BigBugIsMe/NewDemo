using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Text;
using System.Net;
using System;

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
        /// <summary>
        /// 用于消息转换
        /// </summary>
        ByteArray m_ByteTemp = null;
        /// <summary>
        /// 包体内容
        /// </summary>
        byte[] m_MsgPackContent = null;
        /// <summary>
        /// 错误信息
        /// </summary>
        SocketError m_Error;
        bool m_IsError = false;
        bool m_IsConnected = false;

        public ByteSocket()
        {
            EnterFrameManager.GetInstance().Register(EnterFrame);
        }
        void EnterFrame()
        {

        }
        public void F_Connect(string ip, int port, bool isLogin = false, bool isForceQuit = false)
        {
            if (string.IsNullOrEmpty(ip)) return;
            try
            {
                if (m_Socket == null)
                {
                    //缓存区下标重置
                    m_StreamBuffer.Position = 0;
                    IPAddress ipAddress = null;
                    IPEndPoint ipEndPoint = null;
                    if (IPAddress.TryParse(ip, out ipAddress))
                    {
                        ipEndPoint = new IPEndPoint(ipAddress, port);
                        if (ipEndPoint.AddressFamily == AddressFamily.InterNetwork)
                        {
                            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        }
                        else if (ipEndPoint.AddressFamily == AddressFamily.InterNetworkV6)
                        {
                            m_Socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                        }
                    }
                    else
                    {
                        try
                        {
                            IPHostEntry ipHost = Dns.GetHostEntry(ip);
                            IPAddress address = ipHost.AddressList[0];
                            if (address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            }
                            else if (address.AddressFamily == AddressFamily.InterNetworkV6)
                            {
                                m_Socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                            }
                            ipEndPoint = new IPEndPoint(address, port);
                        }
                        catch (Exception e)
                        {
                            Debug.Log("IpError:" + e);
                        }
                    }
                    m_Socket.BeginConnect(ipEndPoint,new AsyncCallback(ConnectCallBack), m_Socket);
                }
            }
            catch (SocketException e)
            {
                m_IsError = true;
                Debug.Log("SocketError:" + e);
            }
        }

        void ConnectCallBack(IAsyncResult result)
        {
            try
            {
                Socket so = (Socket)result.AsyncState;
            }
            catch (SocketException e1)
            { }
        }
    }
}

