using System;
using System.Text;
using System.IO.Ports;
using System.Threading;


namespace SerialTerminal
{
    class Program
    {
        static bool continueToTransmit = false;
        static string serPort, inputLine;
        static int serBaud, TESTMODE;
        static byte serDataBits, serParity, serStopBits;
        static Thread readThread, writeThread;

        static void Main(string[] args)
        {
            
            if (args.Length == 0)
            {
                TESTMODE = 0;
            }
            else if (args.Length > 0)
            {
                TESTMODE = Int32.Parse(args[0]);
            }
            
            readThread = new Thread(Read);
            writeThread = new Thread(Write);

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
            {   // Set Terminal Mode
                Console.WriteLine("SerialTerminal ~ DevolutionXLimited");
                Console.WriteLine();
                // List Serial Ports
                Console.WriteLine("Serial Ports");
                SIOManager.getSerialPortList();
                Console.WriteLine();
                // Set Serial Port
                Console.Write("Enter the name of the desired serial port: ");
                serPort = Console.ReadLine();
                if (String.IsNullOrEmpty(serPort))
                {
                    Console.WriteLine("Did not set serial port!");
                    End();
                }
                // Set Serial Port Baudrate
                Console.WriteLine();
                Console.Write("Enter the baud rate: ");
                if (Int32.TryParse(Console.ReadLine(), out serBaud))
                {   // Set Serial Port Databits
                    Console.WriteLine();
                    Console.Write("Enter the size of DataBits: ");
                    if (byte.TryParse(Console.ReadLine(), out serDataBits))
                    {   // Set Serial Port Parity
                        Console.WriteLine();
                        Console.WriteLine("Enter the Parity value.");
                        Console.Write("0 = None, 1 = Even, 2 = Odd: ");
                        if (byte.TryParse(Console.ReadLine(), out serParity))
                        {   // Set Serial Port Stopbits
                            Console.WriteLine();
                            Console.WriteLine("Enter the StopBits value.");
                            Console.Write("0 = None, 1 = One, 2 = Two: ");
                            if (byte.TryParse(Console.ReadLine(), out serStopBits))
                            {
                                Console.WriteLine();
                                Console.WriteLine("Terminal Setup Completed.");
                                Motd();
                            }
                            else
                            {
                                Console.WriteLine("Did not set stopbits!");
                                End();
                            }
                        }
                        else
                        {
                            Console.WriteLine("Did not set parity!");
                            End();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Did not set databits!");
                        End();
                    }
                }
                else
                {
                    Console.WriteLine("Did not set baud!");
                    End();
                }

                SIOManager.SIOInit(serPort, serBaud, serDataBits, serParity, serStopBits, port_DataReceived);
                Update();
            }
        }

        static void Update()
        {
            while (true)
            {
                while (continueToTransmit == false)
                {
                    inputLine = Console.ReadLine();
                    // Available Commands
                    switch (inputLine)
                    {
                        case "quit":
                            End();
                            break;

                        case "writeBytes":
                            Console.WriteLine("Enter a string of bytes in hex:");
                            Console.WriteLine("Example - '48656C6C6F576F726C64'");

                            string byteStr = Console.ReadLine();
                            byte[] hexToByteA = STUtil.ToByteArray(byteStr);

                            SIOManager.port.Write(hexToByteA, 0, hexToByteA.Length);
                            break;

                        case "login":
                            Console.WriteLine("Console over serial login for Linux or BSD.");

                            SIOManager.SIOLogin();
                            break;

                        case "twoway":
                            SIOManager.port.Close();
                            Console.WriteLine("Closing Serial Port!");
                            SIOManager.SIOInit(serPort, serBaud, serDataBits, serParity, serStopBits);
                            Console.WriteLine("Reloading Last Serial Port in twoway mode!");

                            continueToTransmit = true;
                            Threads();
                            Console.WriteLine("\t\tEntered TwoWay mode");
                            Console.WriteLine();
                            break;

                        // Passthrough 
                        default:
                            SIOManager.port.WriteLine(inputLine);
                            break;
                    }
                }
            }
        }

        static void Motd()
        {
            Console.WriteLine();
            Console.WriteLine("Commands: ");
            Console.WriteLine("|  'quit'  |  'writeBytes'  |   'login'   |   'twoway'   |");
            Console.WriteLine("    If the user does not pass a command into the terminal ");
            Console.WriteLine("    the terminal assumes pass through mode; which sends   ");
            Console.WriteLine("    any INPUT over the serial port. In addition the       ");
            Console.WriteLine("    program will return the input back to the console.    ");
            Console.WriteLine();
        }

        static void Read()
        {
            while (continueToTransmit)
            {
                try
                {
                    string message = SIOManager.port.ReadLine();
                    Console.Write(message);
                    Console.WriteLine();
                }
                catch
                {
                    Console.WriteLine("Error in 'Read()' Thread!");
                }
            }
        }

        static void Write()
        {
            while (continueToTransmit)
            {
                try
                {
                    string message = Console.ReadLine();

                    switch (message)
                    {
                        case "return":
                            continueToTransmit = false;
                            SIOManager.port.Close();
                            Console.WriteLine("Closing Serial Port!");
                            SIOManager.SIOInit(serPort, serBaud, serDataBits, serParity, serStopBits, port_DataReceived);
                            Console.WriteLine("Reloading Serial Port and Returning to Primary Mode!");
                            Console.WriteLine("\t\tEntered Primary Mode");
                            Console.WriteLine();
                            Motd();
                            break;

                        case "quit":
                            End();
                            break;

                        default:
                            SIOManager.port.Write(message);
                            SIOManager.port.WriteLine("");
                            break;
                    }  
                }
                catch
                {
                    Console.WriteLine("Error in 'Write()' Thread!");
                }
            }
        }

        static void Threads()
        {   
            try
            {
                readThread.Start();
                writeThread.Start();
            }
            catch
            {
                Console.WriteLine("Error in 'Threads()' function: Could not start threads!");
            }
        }

        static void End()
        {
            Console.WriteLine("Closing Program!");
            readThread.Abort();
            writeThread.Abort();
            SIOManager.Close();
            STUtil.Pause(1000);
            Environment.Exit(0);
        }

        // This Callback returns the same value placed into the serial terminal
        static void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort sp = (SerialPort)sender;
                string indata = sp.ReadExisting();
                Console.WriteLine(indata);
            }
            catch
            {
                Console.WriteLine("Error in 'port_DataReceived()' function!");
            }
        }
    }
}
