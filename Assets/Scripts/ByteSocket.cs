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
        List<Message> toHandleList = new List<Message>();
        public ByteSocket()
        {
            EnterFrameManager.GetInstance().Register(EnterFrame);
        }
        void EnterFrame()
        {
            toHandleList.Clear();
            lock(m_MsgQueue)
            {
                if(m_MsgQueue.Count>0)
                {
                    int numCount = m_MsgQueue.Count;
                    for(int i=0;i<numCount;i++)
                    {
                        if(m_MsgQueue.Count>0)
                        {
                            Message msg = null;
                            msg = m_MsgQueue.Dequeue();
                            if(msg!=null)
                            {
                                toHandleList.Add(msg);
                            }
                        }
                    }
                }
            }
            //消息分发
            for(int i=0;i<toHandleList.Count;i++)
            {

            }
        }
        /// <summary>
        /// 开始连接服务器
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="isLogin"></param>
        /// <param name="isForceQuit"></param>
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
        /// <summary>
        /// 连接回调
        /// </summary>
        /// <param name="result"></param>
        void ConnectCallBack(IAsyncResult result)
        {
            try
            {
                Socket so = (Socket)result.AsyncState;
                if (so != null)
                {
                    so.EndConnect(result);
                }
                m_Socket = so;
                if (m_Socket == null || !m_Socket.Connected)
                {
                    m_IsError = true;
                    //链接错误处理
                    return;
                }
                else
                {
                    m_IsError = false;
                }
                StartReceive();
            }
            catch (SocketException e1)
            { }
        }
        /// <summary>
        /// 开始接受数据
        /// </summary>
        void StartReceive()
        {
            m_IsConnected = (m_Socket != null && m_Socket.Connected);
            if(m_IsConnected)
            {
                try
                {
                    //开启线程接收数据
                    if(m_ReceiveThread==null||!m_ReceiveThread.IsAlive)
                    {
                        m_ReceiveThread = new Thread(new ThreadStart(ReceiveData));
                        m_ReceiveThread.IsBackground = true;
                        m_ReceiveThread.Start();
                    }
                }
                catch(SocketException e)
                {

                }
            }
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        void ReceiveData()
        {
            while(m_IsConnected)
            {
                if(m_Socket!=null)
                {
                    if (!m_Socket.Connected) break;
                }
                //一直等待服务器消息
                m_RecvMsgLength = m_Socket.Available;
                //最起码要拿到消息的包头长度
                if(m_RecvMsgLength>MSG_HEADER_LENGTH)
                {
                    //取出包头数据
                    m_Socket.Receive(m_RecvMsgHead, MSG_HEADER_LENGTH, SocketFlags.None);
                    //写入缓冲区
                    m_StreamBuffer.Write(m_RecvMsgHead, 0, MSG_HEADER_LENGTH);
                    ByteArray ba = new ByteArray(m_RecvMsgHead);
                    //取到包头大小（
                    m_MsgBodyLength = ba.ReadInt();
                    ba.Dispose();
                    //创建包内容缓冲区
                    byte[] receiveBuff = new byte[m_MsgBodyLength];
                    int size = 0, recvSize = 0;
                    while(size<m_MsgBodyLength&&m_IsConnected&&m_Socket!=null&&m_Socket.Connected)
                    {
                        recvSize=m_Socket.Receive(receiveBuff, m_MsgBodyLength - size, SocketFlags.None);
                        m_StreamBuffer.Write(receiveBuff, 0, recvSize);
                        size += recvSize;
                    }
                    UnPackMsg();
                }
            }
        }
        /// <summary>
        /// 消息解包
        /// </summary>
        void UnPackMsg()
        {
            //包体长度错误
            if (m_MsgBodyLength <= 0)
            {
                Debug.Log("消息长度错误：" + m_MsgBodyLength);
                m_StreamBuffer.Position = 0;
                return;
            }
            //缓冲区是否有多个包待解析
            if (m_StreamBuffer.Position != m_MsgBodyLength + MSG_HEADER_LENGTH)
            {
                Debug.Log("消息尺寸错误，收到：" + m_StreamBuffer.Length + "实际应该为：" + m_MsgBodyLength + MSG_HEADER_LENGTH);
                m_StreamBuffer.Position = 0;
                return;
            }
            //减掉包头长度
            m_MsgPackContent = new byte[m_MsgBodyLength - 4];
            //获得消息Id
            System.Array.Copy(m_StreamBuffer.GetBuffer(), MSG_HEADER_LENGTH, m_RecvMsgHead, 0, 4);
            //消息具体内容
            System.Array.Copy(m_StreamBuffer.GetBuffer(), MSG_HEADER_LENGTH + 4, m_MsgPackContent, 0, m_MsgBodyLength - 4);

            m_ByteTemp = new ByteArray(m_RecvMsgHead);
            int msgId = m_ByteTemp.ReadInt();
            m_ByteTemp.Dispose();
            Message msg = MessagePool.GetInstance().GetMessageInstance(msgId);
            if (msg != null)
            {
                try
                {
                    m_ByteTemp = new ByteArray(m_MsgPackContent);
                    msg.read(m_ByteTemp);
                    m_ByteTemp.Dispose();
                    lock(m_MsgQueue)
                    {
                        m_MsgQueue.Enqueue(msg);
                    }
                }
                catch(Exception e)
                {

                }
            }
            //每次消息解析完要将缓存区归位
            m_StreamBuffer.Position = 0;
        }
        /// <summary>
        /// 发消息
        /// </summary>
        /// <param name="message"></param>
        public void F_Send(Message message)
        {
            if (m_Socket != null && m_Socket.Connected)
            {
                message.write();
                ByteArray ba = new ByteArray();
                byte[] sendBytes = message.getBuff().Buffer;
                //写入包体实际长度+包头
                ba.WriteInt32(sendBytes.Length + 4);
                //写入消息id
                ba.WriteInt32(message.getId());
                //写入消息的实际内容
                ba.WriteBytes(sendBytes);
                message.Dispose();
                m_Socket.BeginSend(ba.Buffer, 0, ba.Buffer.Length, SocketFlags.None, new AsyncCallback((IAsyncResult result) =>
                    {
                        Socket so = result.AsyncState as Socket;
                        SocketError error = SocketError.TryAgain;
                        if(so==null)
                        {

                        }
                        else
                        {
                            so.EndSend(result, out error);
                        }
                        if(error!=SocketError.Success)
                        {
                            //发送失败，请处理
                        }
                        ba.Dispose();
                    }),m_Socket);
            }
        }
    }
}

