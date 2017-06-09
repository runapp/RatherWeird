using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace RatherWeird
{
    [DataContract]
    public class SerializedData
    {
        [DataMember(Name = "bfme")]
        public Game Bfme { get; set; }

        [DataMember(Name = "bfme2")]
        public Game Bfme2 { get; set; }

        [DataMember(Name = "cnc3")]
        public Game Cnc3 { get; set; }

        [DataMember(Name = "cnc3kw")]
        public Game Cnc3Kw { get; set; }

        [DataMember(Name = "generals")]
        public Game Generals { get; set; }

        [DataMember(Name = "generalszh")]
        public Game Generalszh { get; set; }

        [DataMember(Name = "ra3")]
        public Game Ra3 { get; set; }

        [DataMember(Name = "rotwk")]
        public Game Rotwk { get; set; }
    }

    [DataContract]
    public class Game
    {
        [DataMember(Name = "games")]
        public Matches Matches { get; set; }

        [DataMember(Name = "lobbies")]
        public Lobby Lobbies { get; set; }

        [DataMember(Name = "users")]
        public Dictionary<string, Player> Users { get; set; }
    }

    [DataContract]
    public class Lobby
    {
        [DataMember(Name = "chat")]
        public int Chat { get; set; }

        [DataMember(Name = "hosting")]
        public int Hosting { get; set; }

        [DataMember(Name = "playing")]
        public int Playing { get; set; }
    }
    
    [DataContract]
    public class Matches
    {
        [DataMember(Name = "playing")]
        public Match[] Playing { get; set; }

        [DataMember(Name = "staging")]
        public Match[] Staging { get; set; }
    }

    [DataContract]
    public class Match
    {
        [DataMember(Name = "cmdCRC")]
        public string CmdCrc { get; set; }

        [DataMember(Name = "exeCRC")]
        public string ExeCrc { get; set; }

        [DataMember(Name = "gamever")]
        public string GameVersion { get; set; }

        [DataMember(Name = "host")]
        public Player Host { get; set; }

        [DataMember(Name = "iniCRC")]
        public string IniCrc { get; set; }

        [DataMember(Name = "map")]
        public string Mao { get; set; }

        [DataMember(Name = "maxReadPlayers")]
        public string MaxRealPlayers { get; set; }

        [DataMember(Name = "maxplayers")]
        public string MaxPlayers { get; set; }

        [DataMember(Name = "numObservers")]
        public string NumberOfObservers { get; set; }

        [DataMember(Name = "numReadPlayers")]
        public string NUmberOfRealPlayers { get; set; }

        [DataMember(Name = "numplayers")]
        public string NumberOfPlayers { get; set; }

        [DataMember(Name = "obs")]
        public string Observer { get; set; }

        [DataMember(Name = "pings")]
        public int Pings { get; set; }

        [DataMember(Name = "players")]
        public Player[] Players { get; set; }

        [DataMember(Name = "pw")]
        public string Password { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "version")]
        public string Version { get; set; }
    }

    [DataContract]
    public class Player : INotifyPropertyChanged
    {
        private string _nickname;
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "nickname")]
        public string Nickname
        {
            get { return _nickname; }
            set
            {
                if (_nickname != value)
                {
                    _nickname = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Nickname"));
                }
            }
        }

        [DataMember(Name = "pid")]
        public int Pid { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class CncOnlineInfo
    {
        public static async Task<SerializedData> FetchEverything(string url)
        {
            try
            {
                HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;

                Task<WebResponse> task = Task.Factory.FromAsync(
                    req.BeginGetResponse,
                    asyncResult => req.EndGetResponse(asyncResult),
                    null
                );

                return await task.ContinueWith(t =>
                {
                    using (Stream responseStream = t.Result.GetResponseStream())
                    using (StreamReader sr = new StreamReader(responseStream))
                    {
                        //Need to return this response 
                        string strContent = sr.ReadToEnd();


                        SerializedData data = (SerializedData)JsonConvert.DeserializeObject<SerializedData>(strContent);
                        return data;
                    }
                });
                
            }
            catch (Exception e)
            {
                return null;
            }

        }

        public static async Task<Game> FetchRa3(string url)
        {
            SerializedData everything = await FetchEverything(url);
            return everything.Ra3;
        }
    }
}
