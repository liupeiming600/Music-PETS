using System;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR

using System.Net;
using System.Net.Sockets;
using System.Threading;

#elif UNITY_WSA

using Windows.Foundation;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

#endif

namespace HololensPETS
{
    /**
     * The UDPHelper class is a utility class for handling
     * behavior related to initializing a UDP connection, either
     * as a client or as a server, as well as sending and receiving
     * data through a UDP connection.
     * 
     * This class currently contains implementations for both
     * System.Net and UWP, but should be split in the future.
     */
    public class UDPHelper : MonoBehaviour
    {
        public string listenPort = "11000";

        public delegate void MessageReceivedDelegate(NetInMessage message);
        public event MessageReceivedDelegate MessageReceived;

        private Queue<NetInMessage> m_incomingMessageQueue;
        private object lockObj;

#if !UNITY_EDITOR && UNITY_WSA
        private DatagramSocket m_udpSocket;
#else
        private UdpClient m_udpClient;
        private Thread m_udpReceiveThread;
#endif

        public void Send(NetOutMessage message, string destIP, int destPort )
        {
            InternalSend( message, destIP, destPort );
        }

        private void Update()
        {
            lock (lockObj)
            {
                while (m_incomingMessageQueue.Count > 0)
                {
                    NetInMessage receivedMessage = m_incomingMessageQueue.Dequeue();
                    if( MessageReceived != null )
                    {
                        Delegate[] invocationList = MessageReceived.GetInvocationList();
                        if( invocationList != null )
                        {
                            for( int i = 0; i < invocationList.Length; i++ )
                            {
                                receivedMessage.ResetIndex();

                                invocationList[i].DynamicInvoke( receivedMessage );
                            }
                        }
                    }
                }
            }
        }

        #region System.Net Implementation
#if UNITY_EDITOR
        private void Awake()
        {
            m_udpClient = new UdpClient(int.Parse(listenPort));

            m_udpReceiveThread = new Thread(new ThreadStart(ReceiveData));
            m_udpReceiveThread.IsBackground = true;

            m_incomingMessageQueue = new Queue<NetInMessage>();

            lockObj = new object();
        }

        private void Start()
        {
            m_udpReceiveThread.Start();
        }

        private void OnApplicationQuit()
        {
            m_udpReceiveThread.Abort();
        }

        private void InternalSend( NetOutMessage message, string ip, int port )
        {
            IPAddress ipAddress = null;
            if( !IPAddress.TryParse(ip, out ipAddress) )
            {
                return;
            }

            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);

            byte[] messageBytes = message.GetMessageBytes();

            NetOutMessage modifiedMessage = new NetOutMessage();
            modifiedMessage.WriteInt32(messageBytes.Length);
            modifiedMessage.WriteBytes(messageBytes);
            
            m_udpClient.Send(modifiedMessage.GetMessageBytes(), modifiedMessage.GetMessageBytes().Length, ipEndPoint);
        }

        private void ReceiveData()
        {
            while( true )
            {
                try
                {
                    IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, int.Parse(listenPort) );
                    if (m_udpClient.Available > 0)
                    {
                        byte[] data = m_udpClient.Receive(ref anyIP);

                        lock (lockObj)
                        {
                            // Our convention for each UDP packet is to first write the
                            // size of the data in bytes. In the receiving end, we then use this
                            // size value to initialize the byte array. However, when reading via System.Net,
                            // it automatically detects the size of the packet, so we don't use the size value.
                            // In this case, we just skip it.
                            byte[] modifiedData = new byte[data.Length - 4];
                            Array.Copy( data, 4, modifiedData, 0, data.Length - 4 );

                            NetInMessage receivedMessage = new NetInMessage( modifiedData );

                            /**
                             * Because polling for data from the UDP connection is done by
                             * a different thread (and not the Unity thread), we cannot simply
                             * call the registered delegate functions that will handle how the data
                             * will be processed. To overcome this, we put them in a queue, and then
                             * in the Update() function of Unity, we go over the queue, and see if there
                             * are UDP data that needs to be processed.
                             */
                            m_incomingMessageQueue.Enqueue( receivedMessage );
                        }
                    }

                    Thread.Sleep(1);
                }
                catch( Exception e )
                {
                    Debug.LogError( e.ToString() );
                }
            }
        }
#endif
        #endregion

        #region UNET Implementation
#if !UNITY_EDITOR && UNITY_WSA
        private void Awake()
        {
            m_udpSocket = new DatagramSocket();
            m_udpSocket.MessageReceived += Socket_MessageReceived;

            m_incomingMessageQueue = new Queue<NetInMessage>();

            lockObj = new object();
        }

        private async void Start()
        {
            await m_udpSocket.BindServiceNameAsync( listenPort );
        }

        private void OnDestroy()
        {
            if( m_udpSocket != null )
            {
                m_udpSocket.Dispose();
            }
        }

        private async void InternalSend( NetOutMessage message, string destIP, int destPort )
        {
            HostName partnerHostName = new HostName( destIP );

            using ( IOutputStream stream = await m_udpSocket.GetOutputStreamAsync( partnerHostName, destPort.ToString() ) )
            {
                using ( DataWriter writer = new DataWriter( stream ) )
                {
                    byte[] data = message.GetMessageBytes();

                    writer.WriteInt32( data.Length );
                    writer.WriteBytes( data );

                    await writer.StoreAsync();
                }
            }
        }
        
        private void Socket_MessageReceived( DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args )
        {
            DataReader networkDataReader = args.GetDataReader();

            int len = networkDataReader.ReadInt32();
            byte[] data = new byte[len];
            networkDataReader.ReadBytes( data );
            
            NetInMessage message = new NetInMessage(data);
            lock (lockObj)
            {
                m_incomingMessageQueue.Enqueue(message);
            }
        }
#endif
        #endregion
    }
}