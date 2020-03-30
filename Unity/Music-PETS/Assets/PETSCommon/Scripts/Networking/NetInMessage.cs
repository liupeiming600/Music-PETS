using System;
using System.Text;

using UnityEngine;

namespace HololensPETS
{
    /**
     * NetInMessage is a utility class that
     * provides functions for easily reading data (in different formats)
     * received from a net connection.
     */
    public class NetInMessage
    {
        private byte[] m_data;

        private int m_readIndex;

        public NetInMessage(byte[] data)
        {
            m_data = data;

            m_readIndex = 0;
        }

        public void ResetIndex()
        {
            m_readIndex = 0;
        }

        public int ReadInt32()
        {
            byte[] bytes = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                bytes[i] = m_data[m_readIndex + i];
            }

            m_readIndex += bytes.Length;

            if(BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return BitConverter.ToInt32(bytes, 0);
        }

        public double ReadDouble()
        {
            byte[] buffer = new byte[sizeof(double)];
            for( int i = 0; i < buffer.Length; i++ )
            {
                buffer[i] = m_data[m_readIndex + i];
            }
            m_readIndex += buffer.Length;
        
            if(BitConverter.IsLittleEndian)
            {
                Array.Reverse(buffer);
            }
            return System.BitConverter.ToDouble(buffer, 0);
        }

        public float ReadFloat()
        {
            byte[] buffer = new byte[sizeof(float)];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = m_data[m_readIndex + i];
            }
            m_readIndex += buffer.Length;

            if(BitConverter.IsLittleEndian)
            {
                Array.Reverse(buffer);
            }
            return System.BitConverter.ToSingle(buffer, 0);
        }

        public byte[] ReadBytes(int length)
        {
            byte[] b = new byte[length];
            for (int i = 0; i < length; i++)
            {
                b[i] = m_data[m_readIndex + i];
            }

            m_readIndex += length;

            return b;
        }

        public string ReadString()
        {
            int numBytes = ReadInt32();
            byte[] b = ReadBytes(numBytes);

            return Encoding.ASCII.GetString(b);
        }

        public Vector3 ReadVector3()
        {
            Vector3 ret = new Vector3();
            ret.x = ReadFloat();
            ret.y = ReadFloat();
            ret.z = ReadFloat();

            return ret;
        }

        public Vector2 ReadVector2()
        {
            Vector2 ret = new Vector2();
            ret.x = ReadFloat();
            ret.y = ReadFloat();

            return ret;
        }
    }
}