using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace lol_dodge_alert
{
    class Program
    {
        static api api = new api();
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "Dodge Alert | v0.1 by floyare";
            print("‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾", ConsoleColor.DarkBlue);
            print(@"       __                  __                                      __                      __     ", ConsoleColor.Blue);
            print(@"  ____| $$  ______    ____| $$  ______    ______          ______  | $$  ______    ______ _| $$_   ", ConsoleColor.Blue);
            print(@" /      $$ /      \  /      $$ /      \  /      \        |      \ | $$ /      \  /      \   $$ \  ", ConsoleColor.Blue);
            print(@"|  $$$$$$$|  $$$$$$\|  $$$$$$$|  $$$$$$\|  $$$$$$\        \$$$$$$\| $$|  $$$$$$\|  $$$$$$\$$$$$$  ", ConsoleColor.Blue);
            print(@"| $$  | $$| $$  | $$| $$  | $$| $$  | $$| $$    $$       /      $$| $$| $$    $$| $$   \$$| $$ __ ", ConsoleColor.Blue);
            print(@"| $$__| $$| $$__/ $$| $$__| $$| $$__| $$| $$$$$$$$      |  $$$$$$$| $$| $$$$$$$$| $$      | $$|  \", ConsoleColor.Blue);
            print(@" \$$    $$ \$$    $$ \$$    $$ \$$    $$ \$$     \       \$$    $$| $$ \$$     \| $$       \$$  $$", ConsoleColor.Blue);
            print(@"  \$$$$$$$  \$$$$$$   \$$$$$$$ _\$$$$$$$  \$$$$$$$        \$$$$$$$ \$$  \$$$$$$$ \$$        \$$$$ ", ConsoleColor.Blue);
            print(@"                              |  \__| $$                                                          ", ConsoleColor.Blue);
            print(@"                               \$$    $$                                                          ", ConsoleColor.Blue);
            print(@"                                \$$$$$$                                                           ", ConsoleColor.Blue);
            print("___________________________________________________________________________________________________", ConsoleColor.DarkBlue);
            ///Pre-setup token.
            print(Encoding.ASCII.GetString(ValueTuple.Security.token), ConsoleColor.DarkBlue);
            /// Start lobby checker
            Thread th = new Thread(thr);
            th.Start();
        }

        public static bool inLobby = false;
        public static bool aftermatch = true;

        public static void thr()
        {
            while (true)
            {
                /// Get Riot auth
                ValueTuple<string, string> info = new ValueTuple().GetInfo();
                /// If player is in Champion Select
                if (api.is_in_lobby(info.Item1, info.Item2))
                {
                    /// Variable for after match check.
                    aftermatch = true;
                    try
                    {
                        /// If player is already in lobby (prevent executing the same functions in loop)
                        if (!inLobby)
                        {
                            print("| Lobby Found!", ConsoleColor.White);
                            /// Download player list and store them in "api.lobby_players"
                            api.get_players(info.Item1, info.Item2);
                            print("| Got player names gained!", ConsoleColor.White);
                            if (should_dodge(globals.path))
                            {
                                print("", ConsoleColor.Gray);
                                print("| Blacklisted player in lobby!", ConsoleColor.Red);
                                print("| Nickname: " + globals.blacklist_nickname, ConsoleColor.Red);
                                print("| Details: " + globals.blacklist_details, ConsoleColor.Red);
                                print("", ConsoleColor.Gray);
                                int n = 5;
                                int frequency = 1000;
                                int duration = 400;
                                for (int i = 1; i < n; i++)
                                    Console.Beep(frequency, duration);
                            }
                            else
                            {
                                print("| Every player is good", ConsoleColor.Green);
                            }
                            inLobby = true;
                        }
                    }
                    catch (Exception ex) {
                        print("| Error occured: " + ex.Message, ConsoleColor.DarkRed);
                        Console.WriteLine(ex.ToString());
                    }
                }
                else
                {
                    /// If player left the lobby
                    if (aftermatch)
                    {
                        print("| Waiting for lobby...", ConsoleColor.White);
                        aftermatch = false;
                    }
                    /// Reset lobby variable and clear the player list
                    inLobby = false;
                    api.lobby_players.Clear();
                }
                System.Threading.Thread.Sleep(1000);
            }
        }

        public static void print(string text, ConsoleColor clr)
        {
            Console.ForegroundColor = clr;
            Console.WriteLine(text);
        }

        /// If player from "api.lobby_players" is in blacklist
        public static bool should_dodge(string path)
        {
            foreach (string item in api.lobby_players)
            {
                StreamReader sr = new StreamReader(path);
                string list = sr.ReadToEnd();
                string[] lines = list.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                IEnumerable<string> results = lines.Where(l => l.StartsWith(item));
                string result = lines.SingleOrDefault(l => l.StartsWith(item));
                string[] textLines = File.ReadAllLines(path);

                foreach (string line in textLines.Where(l => l.StartsWith(item)))
                {
                    /// If someone from Champion select is from blacklist
                    if (api.lobby_players.Contains(item))
                    {
                        string s = line.Replace(item + " - ", "");
                        globals.blacklist_details = s;
                        globals.blacklist_nickname = item;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
