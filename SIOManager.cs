using System;
using System.Text;
using System.IO.Ports;

namespace SerialTerminal
{
    public class SIOManager
    {

        public static SerialPort port;
        public static string[] names = SerialPort.GetPortNames();

        public SIOManager()
        {
            Console.WriteLine("SIOManager Object");
        }

        public static void Close()
        {
            try
            {
                port.Close();
            }
            catch
            {
                Console.WriteLine("Serial Port Was Not Open For Closing!");
            }
        }

        public static void getSerialPortList()
        {
            foreach (string name in names)
            {
                Console.Write(name);
                Console.Write(" ");
            }
            Console.WriteLine();
        }
		
		public static void SIOInit(string sioPort, int baud, int dataBits, int parity, int stopBits)
        {
            port = new SerialPort(sioPort);
            port.BaudRate = baud;
            port.DataBits = dataBits;
            //port.Encoding = Encoding.GetEncoding(28591);
            
            if (parity == 0)
                port.Parity = Parity.None;
            else if (parity == 1)
                port.Parity = Parity.Even;
            else if (parity == 2)
                port.Parity = Parity.Odd;

            if (stopBits == 0)
                port.StopBits = StopBits.None;
            else if (stopBits == 1)
                port.StopBits = StopBits.One;
            else if (stopBits == 2)
                port.StopBits = StopBits.Two;

            try
            {
                port.Open();
            }
            catch
            {
                Console.WriteLine("Serial port failed to open, could be in use! type 'quit' to exit!");
            }
        }

        public static void SIOInit(string sioPort, int baud, int dataBits, int parity, int stopBits, SerialDataReceivedEventHandler callBack)
        {
            port = new SerialPort(sioPort);
            port.BaudRate = baud;
            port.DataBits = dataBits;
            //port.Encoding = Encoding.GetEncoding(28591);
            
            if (parity == 0)
                port.Parity = Parity.None;
            else if (parity == 1)
                port.Parity = Parity.Even;
            else if (parity == 2)
                port.Parity = Parity.Odd;

            if (stopBits == 0)
                port.StopBits = StopBits.None;
            else if (stopBits == 1)
                port.StopBits = StopBits.One;
            else if (stopBits == 2)
                port.StopBits = StopBits.Two;

            port.DataReceived += new SerialDataReceivedEventHandler(callBack);

            try
            {
                port.Open();
            }
            catch
            {
                Console.WriteLine("Serial port failed to open, could be in use! type 'quit' to exit!");
            }
        }

        public static void SIOLogin()
        {
            try
            {
                port.WriteLine("");

                string checkStr = port.ReadTo("in:");
                int index1 = checkStr.LastIndexOf("log", StringComparison.OrdinalIgnoreCase);

                if (index1 != -1)
                    Console.WriteLine("Login Ready!");
                else
                    Console.WriteLine("Could not identify login state!");
            }
            catch
            {
                Console.WriteLine("Error in 'SIOLogin()' function!");
            }
        }
    }
}