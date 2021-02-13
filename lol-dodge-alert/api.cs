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

        /// Get player's nickname from summonerId
        public string get_playername(string string1, string string2, long id)
        {
            ValueTuple<string, string> info = new ValueTuple().GetInfo();
            if (id == 0)
                return "Bot";
            dynamic data = api_request(info.Item1, info.Item2, "/lol-summoner/v1/summoners/", true, id);
            return data.displayName;
        }

        /// Check if player is in Champion Select
        public bool is_in_lobby(string string1, string string2)
        {
            ValueTuple<string, string> info = new ValueTuple().GetInfo();
            dynamic o = api_request(info.Item1, info.Item2, "/lol-gameflow/v1/session/", false, 0);
            string p = o.phase;
            switch (p)
            {
                case "ChampSelect":
                    return true;
                case "Lobby":
                    return false;
                case null:
                    return false;
            }
            return false;
        }

        /// Add every player from Champion select to "api.lobby_players" for later check.
        public void get_players(string string1, string string2)
        {
            ValueTuple<string, string> info = new ValueTuple().GetInfo();
            JObject s = api_request(info.Item1, info.Item2, "/lol-champ-select/v1/session/", false, 0);
            Summoners Summoners = JsonConvert.DeserializeObject<Summoners>(s.ToString());
            foreach (var myTm in Summoners.myTeam)
            {
                lobby_players.Add(get_playername(info.Item1, info.Item2, Convert.ToInt64(myTm.summonerId)));
            }
        }

        /// Main api manage function
        public dynamic api_request(string string1, string string2, string point, bool needId, long id)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(ValueTuple.Security.Sec.Main));
            RestClient restClient = new RestClient("https://127.0.0.1:" + string2);
            restClient.Authenticator = new HttpBasicAuthenticator("riot", string1);
            RestRequest request = needId ? new RestRequest(point + id, Method.GET) : new RestRequest(point, Method.GET);
            IRestResponse restResponse = restClient.Execute(request);
            dynamic data = JObject.Parse(restResponse.Content);
            return data;
        }
    }
}
