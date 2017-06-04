using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

/**
 * customized stream 
 * 
 * */

namespace TestClient
{
    class ByteBuffer
    {
        MemoryStream stream = null;
        BinaryWriter writer = null;
        BinaryReader reader = null;

        /// <summary>
        /// 无参构造
        /// </summary>
        public ByteBuffer()
        {
            stream = new MemoryStream();
            writer = new BinaryWriter(stream);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="data">byte数组</param>
        public ByteBuffer(byte[] data)
        {
            if (data != null)
            {
                stream = new MemoryStream(data);
                reader = new BinaryReader(stream);
            }
            else
            {
                stream = new MemoryStream();
                writer = new BinaryWriter(stream);
            }
        }

        /// <summary>
        /// 关闭流
        /// </summary>
        public void Close()
        {
            if (writer != null) writer.Close();
            if (reader != null) reader.Close();

            stream.Close();
            writer = null;
            reader = null;
            stream = null;
        }

        /// <summary>
        /// 以byte类型写入，即转换为byte类型
        /// </summary>
        /// <param name="v">byte</param>
        public void WriteByte(byte v)
        {
            writer.Write(v);
        }

        /// <summary>
        /// 以int类型写入，即转换为int类型
        /// </summary>
        /// <param name="v">int</param>
        public void WriteInt(int v)
        {
            writer.Write((int)v);
        }

        /// <summary>
        /// 以ushort类型写入，即转换为ushort类型
        /// </summary>
        /// <param name="v">ushort</param>
        public void WriteShort(ushort v)
        {
            writer.Write((ushort)v);
        }

        /// <summary>
        /// 以long类型写入，即转换为long类型
        /// </summary>
        /// <param name="v">long</param>
        public void WriteLong(long v)
        {
            writer.Write((long)v);
        }


        /// <summary>
        /// 以float类型写入，即转换为float类型
        /// </summary>
        /// <param name="v">float</param>
        public void WriteFloat(float v)
        {
            byte[] temp = BitConverter.GetBytes(v);
            Array.Reverse(temp);
            writer.Write(BitConverter.ToSingle(temp, 0));
        }


        /// <summary>
        /// 以double类型写入，即转换为double类型
        /// </summary>
        /// <param name="v">double</param>
        public void WriteDouble(double v)
        {
            byte[] temp = BitConverter.GetBytes(v);
            Array.Reverse(temp);
            writer.Write(BitConverter.ToDouble(temp, 0));
        }

        /// <summary>
        /// 以string类型写入，即转换为string类型
        /// </summary>
        /// <param name="v">string</param>
        public void WriteString(string v)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(v);
            writer.Write((ushort)bytes.Length);
            writer.Write(bytes);
        }

        /// <summary>
        /// 以byte[]类型写入，即转换为byte[]类型
        /// </summary>
        /// <param name="v">byte[]</param>
        public void WriteBytes(byte[] v)
        {
            writer.Write((int)v.Length);
            writer.Write(v);
        }

        /// <summary>
        /// 将byte读取并转化为byte类型
        /// </summary>
        /// <returns>int</returns>
        public byte ReadByte()
        {
            return reader.ReadByte();
        }

        /// <summary>
        /// 将byte读取并转化为int类型
        /// </summary>
        /// <returns>int</returns>
        public int ReadInt()
        {
            return (int)reader.ReadInt32();
        }

        /// <summary>
        /// 将byte读取并转化为ushort类型
        /// </summary>
        /// <returns>ushort</returns>
        public ushort ReadShort()
        {
            return (ushort)reader.ReadInt16();
        }

        /// <summary>
        /// 将byte读取并转化为long类型
        /// </summary>
        /// <returns>long</returns>
        public long ReadLong()
        {
            return (long)reader.ReadInt64();
        }


        /// <summary>
        /// 将byte读取并转化为float类型
        /// </summary>
        /// <returns>float</returns>
        public float ReadFloat()
        {
            byte[] temp = BitConverter.GetBytes(reader.ReadSingle());
            Array.Reverse(temp);
            return BitConverter.ToSingle(temp, 0);
        }

        /// <summary>
        /// 将byte读取并转化为double类型
        /// </summary>
        /// <returns>double</returns>
        public double ReadDouble()
        {
            byte[] temp = BitConverter.GetBytes(reader.ReadSingle());
            Array.Reverse(temp);
            return BitConverter.ToDouble(temp, 0);
        }

        /// <summary>
        /// 将byte读取并转化为string类型
        /// </summary>
        /// <returns>string</returns>
        public string ReadString()
        {
            ushort len = ReadShort();
            byte[] buffer = new byte[len];
            buffer = reader.ReadBytes(len);
            return Encoding.UTF8.GetString(buffer);
        }

        /// <summary>
        /// 将byte读取并转化为byte[]类型
        /// </summary>
        /// <returns>byte[]</returns>
        public byte[] ReadBytes()
        {
            int len = ReadInt();
            return reader.ReadBytes(len);
        }

        /// <summary>
        /// 转化为byte[] 
        /// </summary>
        /// <returns>byte[] </returns>
        public byte[] ToBytes()
        {
            writer.Flush();
            return stream.ToArray();
        }

        /// <summary>
        /// 将所有的完全写入
        /// </summary>
        public void Flush()
        {
            writer.Flush();
        }
    }

}
