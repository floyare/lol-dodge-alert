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
        ///Support for multiple blacklisted players in one lobby
        public static List<Tuple<string, string>> blacklisted_in_session = new List<Tuple<string, string>>();
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "Dodge Alert | v0.1 by floyare";
            print("‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾", ConsoleColor.DarkBlue, false);
            print(@"       __                  __                                      __                      __     ", ConsoleColor.Blue, false);
            print(@"  ____| $$  ______    ____| $$  ______    ______          ______  | $$  ______    ______ _| $$_   ", ConsoleColor.Blue, false);
            print(@" /      $$ /      \  /      $$ /      \  /      \        |      \ | $$ /      \  /      \   $$ \  ", ConsoleColor.Blue, false);
            print(@"|  $$$$$$$|  $$$$$$\|  $$$$$$$|  $$$$$$\|  $$$$$$\        \$$$$$$\| $$|  $$$$$$\|  $$$$$$\$$$$$$  ", ConsoleColor.Blue, false);
            print(@"| $$  | $$| $$  | $$| $$  | $$| $$  | $$| $$    $$       /      $$| $$| $$    $$| $$   \$$| $$ __ ", ConsoleColor.Blue, false);
            print(@"| $$__| $$| $$__/ $$| $$__| $$| $$__| $$| $$$$$$$$      |  $$$$$$$| $$| $$$$$$$$| $$      | $$|  \", ConsoleColor.Blue, false);
            print(@" \$$    $$ \$$    $$ \$$    $$ \$$    $$ \$$     \       \$$    $$| $$ \$$     \| $$       \$$  $$", ConsoleColor.Blue, false);
            print(@"  \$$$$$$$  \$$$$$$   \$$$$$$$ _\$$$$$$$  \$$$$$$$        \$$$$$$$ \$$  \$$$$$$$ \$$        \$$$$ ", ConsoleColor.Blue, false);
            print(@"                              |  \__| $$                                                          ", ConsoleColor.Blue, false);
            print(@"                               \$$    $$                                                          ", ConsoleColor.Blue, false);
            print(@"                                \$$$$$$                                                           ", ConsoleColor.Blue, false);
            print("___________________________________________________________________________________________________", ConsoleColor.DarkBlue, false);
            ///Pre-setup token.
            print(Encoding.ASCII.GetString(ValueTuple.Security.token), ConsoleColor.DarkBlue, false);
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
                            print("| Lobby Found!", ConsoleColor.White, true);
                            /// Download player list and store them in "api.lobby_players"
                            api.get_players(info.Item1, info.Item2);
                            print("| Got player names gained!", ConsoleColor.White, true);
                            ///After collecting players check if any of them are on blacklist
                            should_dodge(globals.path);
                            if (globals.should_dodge)
                            {
                                ///Check for multiple blacklisted user in same lobby
                                foreach (Tuple<string, string> player in blacklisted_in_session)
                                {
                                    print("", ConsoleColor.Gray, false);
                                    print("| Blacklisted player in lobby!", ConsoleColor.Red, true);
                                    print("| Nickname: " + player.Item1, ConsoleColor.Red, true);
                                    print("| Details: " + player.Item2, ConsoleColor.Red, true);
                                    print("", ConsoleColor.Gray, false);
                                }
                                int n = 5;
                                int frequency = 1000;
                                int duration = 400;
                                for (int i = 1; i < n; i++)
                                    Console.Beep(frequency, duration);
                            }
                            else
                            {
                                print("| Every player is good", ConsoleColor.Green, true);
                            }
                            inLobby = true;
                        }
                    }
                    catch (Exception ex) {
                        print("| Error occured: " + ex.Message, ConsoleColor.DarkRed, true);
                        Console.WriteLine(ex.ToString());
                    }
                }
                else
                {
                    /// If player left the lobby
                    if (aftermatch)
                    {
                        print("| Waiting for lobby...", ConsoleColor.White, true);
                        aftermatch = false;
                    }
                    /// Reset lobby variable and clear the player list
                    inLobby = false;
                    api.lobby_players.Clear();
                    blacklisted_in_session.Clear();
                    ///Reset dodge variable
                    globals.should_dodge = false;
                }
                System.Threading.Thread.Sleep(1000);
            }
        }

        public static void print(string text, ConsoleColor clr, bool w)
        {
            Console.ForegroundColor = clr;
            //pointless but why not
            switch (w)
            {
                case true:
                    Console.WriteLine(DateTime.Now.ToString("h:mm:ss tt") + " " + text);
                    break;
                case false:
                    Console.WriteLine(text);
                    break;
            }
        }

        /// If player from "api.lobby_players" is in blacklist
        public static void should_dodge(string path)
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
                        ///Add player to "blacklist in session" list (support for multiple blacklisted players in one lobby)
                        blacklisted_in_session.Add(new Tuple<string, string>(item, s));
                        globals.should_dodge = true;
                    }
                    else
                    {
                        globals.should_dodge = false;
                    }
                }
            }
        }
    }
}
