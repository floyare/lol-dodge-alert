using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace lol_dodge_alert
{
    public class api
    {
        /// <summary>
        /// Store players from lobby
        /// </summary>
        public static List<string> lobby_players = new List<string>();

        /// <summary>
        /// Struct for storing every player summonerId
        /// </summary>
        public class Summoners
        {
            public List<Players> myTeam { get; set; }

            public class Players
            {
                public string summonerId { get; set; }
            }
        }

        /// <summary>
        /// New struct for storing every player
        /// </summary>
        public class SummonersNew
        {
            public List<Players> participants { get; set; }

            public class Players
            {
                public string game_name { get; set; }
            }
        }

        /// <summary>
        /// Get players from blocked players list
        /// </summary>
        /// <returns></returns>
        public string get_blocked_players()
        {
            ValueTuple<string, string> info = new ValueTuple().GetInfo(false);
            dynamic blocked = api_request(info.Item1, info.Item2, "/lol-chat/v1/blocked-players/", false, 0, true);
            JArray array = blocked;
            string nicknames = null;
            foreach(dynamic player in array)
            {
                nicknames = nicknames + "\n" + player.name;
            }
            return nicknames;
        }

        /// <summary>
        /// Get player's nickname from summonerId
        /// </summary>
        /// <param name="id">Summoner id</param>
        /// <returns></returns>
        public string get_playername(long id)
        {
            ValueTuple<string, string> info = new ValueTuple().GetInfo(false);
            if (id == 0)
                return "Bot";
            dynamic data = api_request(info.Item1, info.Item2, "/lol-summoner/v1/summoners/", true, id, false);
            return data.displayName;
        }

        /// <summary>
        /// Check if player is in Champion Select
        /// </summary>
        /// <returns>If player is in champ-select.</returns>
        public bool is_in_lobby()
        {
            ValueTuple<string, string> info = new ValueTuple().GetInfo(false);
            dynamic o = api_request(info.Item1, info.Item2, "/lol-gameflow/v1/session/", false, 0, false);
            string p = o.phase;
            switch (p)
            {
                case "ChampSelect":
                    return true;
                case null:
                    return false;
            }
            return false;
        }

        /// <summary>
        /// Checks if current game is ranked.
        /// </summary>
        /// <returns></returns>
        public bool isRanked()
        {
            ValueTuple<string, string> info = new ValueTuple().GetInfo(false);
            dynamic o = api_request(info.Item1, info.Item2, "/lol-gameflow/v1/session", false, 0, false);
            bool p = o.gameData.queue.isRanked;
            return p;
        }

        /// <summary>
        /// Add every player from Champion select to "api.lobby_players" for later check.
        /// </summary>
        public void get_players()
        {
            bool ranked = isRanked();

            if (ranked){
                ValueTuple<string, string> info = new ValueTuple().GetInfo(true);
                JObject s = api_request(info.Item1, info.Item2, "/chat/v5/participants/champ-select", false, 0, false);
                SummonersNew Summoners = JsonConvert.DeserializeObject<SummonersNew>(s.ToString());
                foreach (var myTm in Summoners.participants)
                {
                    lobby_players.Add(myTm.game_name);
                }
            }
            else
            {
                ValueTuple<string, string> info = new ValueTuple().GetInfo(false);
                JObject s = api_request(info.Item1, info.Item2, "/lol-champ-select/v1/session/", false, 0, false);
                Summoners Summoners = JsonConvert.DeserializeObject<Summoners>(s.ToString());
                foreach (var myTm in Summoners.myTeam)
                {
                    lobby_players.Add(get_playername(Convert.ToInt64(myTm.summonerId)));
                }
            }
        }

        /// <summary>
        ///  Main api manage function
        /// </summary>
        /// <param name="point">URL endpoint</param>
        /// <param name="needId">If need id for doing request (kinda pointless)</param>
        /// <param name="id">Pass ID of summoner</param>
        /// <returns></returns>
        public dynamic api_request(string string1, string string2, string point, bool needId, long id, bool useArray)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(ValueTuple.Security.Sec.Main));
            RestClient restClient = new RestClient("https://127.0.0.1:" + string2);
            restClient.Authenticator = new HttpBasicAuthenticator("riot", string1);
            RestRequest request = needId ? new RestRequest(point + id, Method.GET) : new RestRequest(point, Method.GET);
            IRestResponse restResponse = restClient.Execute(request);
            dynamic data = useArray ?  (object)JArray.Parse(restResponse.Content) : (object)JObject.Parse(restResponse.Content);
            return data;
        }
    }
}
