using System;
using System.Net.Sockets;
using System.Text;

namespace MatchingEngine
{
    class OrderProtocol
    {
        const int maxReceiveDataSize = 1024 * 10; // 10KB
        const int serverProtocolVersion = 0;

        enum handshakeResponses : byte { HANDSHAKE_OK, PROTOCOL_NOT_SUPPORTED };
        public enum opCodes : byte { PASS, MOVE };

        public static Boolean handshake(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            int clientProtocolVersion = stream.ReadByte();
            return serverProtocolVersion == clientProtocolVersion;
        }

        public static int[] readAction(TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            if (stream.DataAvailable)
            {
                //Read header
                int messageLength = stream.ReadByte();

                //Read payload
                //byte[] data;

                //if (messageLength <= maxReceiveDataSize)
                //{
                //    data = new byte[messageLength];
                //}
                //else
                //{
                //    return new int[2] { 0, 0 };
                //}

                //int bytesRead = 0;
                //byte[] buffer = new byte[2048]; // read in chunks of 2KB
                //int readLength = buffer.Length;

                //while (bytesRead < messageLength)
                //{
                //    if ((bytesRead + buffer.Length) > messageLength)
                //    {
                //        readLength = messageLength - bytesRead;
                //    }
                //    bytesRead += stream.Read(buffer, 0, readLength);
                //    Array.Copy(buffer, 0, data, data.Length - bytesRead, bytesRead);
                //}

                int opCode = stream.ReadByte();
                int parameter = stream.ReadByte();

                return new int[2] { opCode, parameter };
            }
            return new int[2] { 0, 0 };
        }

        public static byte[] screenToBytes(char[,] screen)
        {
            StringBuilder screenString = new StringBuilder();
            for (int x = 0; x < screen.GetLength(0); x++)
            {
                for (int y = 0; y < screen.GetLength(1); y++)
                {
                    screenString.Append(screen[x, y]);
                }
                screenString.Append(Environment.NewLine);
            }
            return Encoding.UTF8.GetBytes(screenString.ToString());
        }
    }
}
