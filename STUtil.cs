using System;
using System.Diagnostics;
using System.Text;

namespace SerialTerminal
{
    public class STUtil
    {

        public STUtil()
        {
            Console.WriteLine("STUtil Object");
        }

        public static byte[] ToByteArray(string value)
        {
            char[] charArr = value.ToCharArray();
            byte[] bytes = new byte[charArr.Length];
            for (int i = 0; i < charArr.Length; i++)
            {
                byte current = Convert.ToByte(charArr[i]);
                bytes[i] = current;
            }
            return bytes;
        }

        public static char[] ToCharArray(string value)
        {
            char[] charArr = value.ToCharArray();
            return charArr;
        }

        public static string ASCIIStrToHex(String asciiString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in asciiString)
                sb.AppendFormat("{0:X}", (int)c);
            return sb.ToString().Trim();
        }

        public static string HexStrToASCII(String hexString)
        {
            string ascii = string.Empty;
            for (int i = 0; i < hexString.Length; i += 2)
            {
                String hs = string.Empty;
                hs = hexString.Substring(i, 2);
                uint decval = Convert.ToUInt32(hs, 16);
                char character = Convert.ToChar(decval);
                ascii += character;
            }
            return ascii;
        }

        public static void Pause(int milli)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; ; i++)
            {
                if (i % 100000 == 0)
                {
                    sw.Stop();
                    if (sw.ElapsedMilliseconds >= milli)
                        break;
                    else
                        sw.Start();
                }
            }
        }
    }
}
