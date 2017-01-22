using System;
using System.Text;
using System.IO.Ports;


namespace SerialTerminal
{
    class Program
    {
        static void Main(string[] args)
        {
            string serPort, inputLine;
            int serBaud;
            byte serDataBits = 0, serParity = 0, serStopBits = 0;

            int TESTMODE = 0;

            if (TESTMODE == 1)
            {
                string dataChar = "HelloWorld";
                string dataCharHex = "48656C6C6F576F726C64"; //HelloWorld

                string asciiToHex = STUtil.ASCIIStrToHex(dataChar);
                string hexToASCII = STUtil.HexStrToASCII(dataCharHex);
                
                Console.WriteLine("Converted 'HelloWorld' to Hex:");
                Console.WriteLine("{0}", asciiToHex);
                Console.WriteLine("Convert Hex back to ASCII: {0}", hexToASCII);
                
                Console.ReadLine();   
            }

            if (TESTMODE == 0)
            {
                // Set Terminal Mode
                Console.WriteLine("SerialTerminal ~ DevolutionXLimited");
                // List Serial Ports
                Console.WriteLine("Serial Ports:");
                SIOManager.getSerialPortList();
                Console.WriteLine("");
                // Set Serial Port
                Console.Write("Enter the name of the desired serial port: ");
                serPort = Console.ReadLine();
                // Set Serial Port Baudrate
                Console.Write("Enter the baud rate: ");
                if (Int32.TryParse(Console.ReadLine(), out serBaud))
                {
                    // Set Serial Port Databits
                    Console.Write("Enter the size of DataBits: ");
                    if (byte.TryParse(Console.ReadLine(), out serDataBits))
                    {
                        // Set Serial Port Parity
                        Console.Write("Enter the Parity value. 0 = None, 1 = Even, 2 = Odd: ");
                        if (byte.TryParse(Console.ReadLine(), out serParity))
                        {
                            // Set Serial Port Stopbits
                            Console.Write("Enter the StopBits value. 0 = None, 1 = One, 2 = Two: ");
                            if (byte.TryParse(Console.ReadLine(), out serStopBits))
                            {
                                Console.WriteLine("");
                                Console.WriteLine("Terminal Setup Completed.");
                                Console.WriteLine("Commands: ");
                                Console.WriteLine("   |   'quit'   |   'writeBytes'   |   ");
                                Console.WriteLine("If the user does not pass a command into the terminal,");
                                Console.WriteLine("the terminal assumes pass through mode, which means,");
                                Console.WriteLine("any INPUT will be passed to the OUTPUT.");
                                Console.WriteLine("");
                            }
                        }
                    }
                }

                SIOManager.SIOInit(serPort, serBaud, serDataBits, serParity, serStopBits, string_DataReceived);

                while (true)
                {
                    inputLine = Console.ReadLine();

                    // Available Commands
                    switch (inputLine)
                    {
                        case "quit":
                            Console.WriteLine("Closing Program!");
                            SIOManager.Close();
                            Environment.Exit(0);
                            break;

                        case "writeBytes":
                            Console.WriteLine("Enter a string of bytes in hex:");
                            Console.WriteLine("Example - '48656C6C6F576F726C64'");

                            string byteStr = Console.ReadLine();
                            byte[] hexToByteA = STUtil.ToByteArray(byteStr);
                            SIOManager.port.Write(hexToByteA, 0, hexToByteA.Length);

                            break;

                        // Passthrough 
                        default:
                            SIOManager.port.Write(inputLine);
                            break;
                    }
                }
            }
        }

        static void string_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Console.WriteLine("Data Recieved:");
            Console.WriteLine((sender as SerialPort).ReadExisting());
        }
    }
}
