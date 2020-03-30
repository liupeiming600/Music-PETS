using System;
using System.Text;

using UnityEngine;

namespace HololensPETS
{
    /**
     * NetOutMessage is a utility class that
     * provides functions for easily writing data (in different formats)
     * and packing them into a byte array that can be sent across the network.
     */
    public class NetOutMessage
    {
        private byte[] m_data;
        private int m_initialSize = 20;

        private int m_writeIndex;

        public NetOutMessage()
        {
            m_data = new byte[m_initialSize];

            m_writeIndex = 0;
        }

        public byte[] GetMessageBytes()
        {
            byte[] b = new byte[m_writeIndex];
            System.Buffer.BlockCopy(m_data, 0, b, 0, m_writeIndex);

            return b;
        }

        public void WriteInt32(int val)
        {
            byte[] b = BitConverter.GetBytes(val);
            if(BitConverter.IsLittleEndian)
            {
                Array.Reverse(b);
            }

            WriteBytes(b);
        }

        public void WriteDouble(double val)
        {
            byte[] b = BitConverter.GetBytes(val);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(b);
            }

            WriteBytes(b);
        }

        public void WriteFloat(float val)
        {
            byte[] b = BitConverter.GetBytes(val);
            if(BitConverter.IsLittleEndian)
            {
                Array.Reverse(b);
            }

            WriteBytes(b);
        }

        public void WriteByte(byte val)
        {
            UpdateBufferIfNeeded(1);

            m_data[m_writeIndex] = val;

            m_writeIndex++;
        }

        public void WriteBytes(byte[] val)
        {
            UpdateBufferIfNeeded(val.Length);

            for (int i = 0; i < val.Length; i++)
            {
                m_data[m_writeIndex + i] = val[i];
            }

            m_writeIndex += val.Length;
        }

        public void WriteString(string str)
        {
            byte[] b = Encoding.ASCII.GetBytes(str);

            WriteInt32(b.Length);
            WriteBytes(b);
        }

        public void WriteVector3( Vector3 v )
        {
            WriteFloat( v.x );
            WriteFloat( v.y );
            WriteFloat( v.z );
        }

        public void WriteVector2(Vector2 v)
        {
            WriteFloat(v.x);
            WriteFloat(v.y);
        }

        private void UpdateBufferIfNeeded(int numBytesToWrite)
        {
            if (m_writeIndex + numBytesToWrite > m_data.Length)
            {
                byte[] temp = new byte[m_data.Length + numBytesToWrite];

                for (int i = 0; i < m_writeIndex; i++)
                {
                    temp[i] = m_data[i];
                }

                m_data = temp;
            }
        }
    }
}