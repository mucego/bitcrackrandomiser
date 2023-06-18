using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using System.Diagnostics.SymbolStore;
using System.Net;

namespace BitcrackRandomiser
{
    internal class Helpers
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public static void WriteLine(string message, MessageType type = MessageType.normal, bool withClear = false)
        {
            // Clear console
            if (withClear)
            {
                Console.Clear();
            }

            // Message type
            if (type == MessageType.error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else if (type == MessageType.success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else if (type == MessageType.info)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }

            // Write message
            Console.WriteLine(string.Format("[{0}] [Info] {1}", DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss"), message));
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="Color"></param>
        public static void Write(string message, ConsoleColor Color = ConsoleColor.Blue)
        {
            // Write message
            Console.ForegroundColor = Color;
            Console.Write(string.Format("{0}",message));
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Get settings from settings.txt
        /// </summary>
        /// <returns></returns>
        public static Settings GetSettings()
        {
            Settings settings = new Settings();
            string Path = AppDomain.CurrentDomain.BaseDirectory + "settings.txt";
            foreach (var line in System.IO.File.ReadLines(Path))
            {
                if (line.Contains('='))
                {
                    string key = line.Split('=')[0];
                    string value = line.Split("=")[1];

                    switch (key)
                    {
                        case "target_puzzle":
                            settings.TargetPuzzle = value;
                            break;
                        case "bitcrack_path":
                            settings.BitcrackPath = value;
                            break;
                        case "bitcrack_arguments":
                            settings.BitcrackArgs = value;
                            break;
                        case "wallet_address":
                            settings.WalletAddress = value;
                            break;
                        case "scan_type":
                            ScanType _e = ScanType.@default;
                            _ = Enum.TryParse(value, true, out _e);
                            settings.ScanType = _e;
                            break;
                        case "custom_range":
                            settings.CustomRange = value;
                            break;
                        case "telegram_share":
                            bool _v;
                            _ = bool.TryParse(value, out _v);
                            settings.TelegramShare = _v;
                            break;
                        case "telegram_acesstoken":
                            settings.TelegramAccessToken = value;
                            break;
                        case "telegram_chatid":
                            settings.TelegramChatId = value;
                            break;
                        case "telegram_share_eachkey":
                            bool _s;
                            _ = bool.TryParse(value, out _s);
                            settings.TelegramShareEachKey = _s;
                            break;
                        case "untrusted_computer":
                            bool _u;
                            _ = bool.TryParse(value, out _u);
                            settings.UntrustedComputer = _u;
                            break;
                        case "test_mode":
                            bool _t;
                            _ = bool.TryParse(value, out _t);
                            settings.TestMode = _t;
                            break;
                    }
                }
            }
            return settings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static Result CheckJobStatus(object o, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                if (e.Data.Contains("Reached"))
                {
                    return new Result { OutputType = OutputType.finished };
                }
                else if (e.Data.Contains("Private key"))
                {
                    return new Result { OutputType = OutputType.privateKeyFound, Content = e.Data.Trim() };
                }
                else if (e.Data.Contains("Found key"))
                {
                    return new Result { OutputType = OutputType.privateKeyFound };
                }
                else if (e.Data.Contains("Initializing"))
                {
                    string GpuModel = e.Data.Substring(e.Data.IndexOf("Initializing")).Replace("Initializing", "").Trim();
                    return new Result { OutputType = OutputType.gpuModel, Content = GpuModel };
                }
                else
                {
                    try
                    {
                        Console.SetCursorPosition(0, 9);
                        Console.Write(e.Data + new string(' ', Console.WindowWidth - e.Data.Length));
                    }
                    catch { }
                    return new Result { OutputType = OutputType.running };
                }
            }

            return new Result { OutputType = OutputType.unknown };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="_Settings"></param>
        public static async void ShareTelegram(string Message, Settings _Settings)
        {
            try
            {
                var botClient = new TelegramBotClient(_Settings.TelegramAccessToken);

                Message _Message = await botClient.SendTextMessageAsync(
                chatId: _Settings.TelegramChatId,
                text: Message);
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WalletAddress"></param>
        /// <returns></returns>
        public static string StringParser(string Value, int Length = 8)
        {
            if (Value.Length > Length)
            {
                string Start = Value.Substring(0, Length);
                string End = Value.Substring(Value.Length - Length);
                return string.Format("{0}...{1}", Start, End);
            }
            return Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PrivateKey"></param>
        /// <param name="Address"></param>
        public static void SaveFile(string PrivateKey, string Address)
        {
            // Save file
            string[] Lines = { PrivateKey, Address, DateTime.Now.ToLongDateString() };
            string AppPath = AppDomain.CurrentDomain.BaseDirectory;
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(AppPath, Address + ".txt")))
            {
                foreach (string Line in Lines)
                    outputFile.WriteLine(Line);
            }
        }
    }
}
