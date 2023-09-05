using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class gamestate_script : NetworkBehaviour
{

    public class PlayerScoreData{
        public int id;
        public int kills;
        public int deaths;
    }

    List<PlayerScoreData> players;
    public Text score_text;
    // Start is called before the first frame update
    void Start()
    {
        players = new List<PlayerScoreData>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnPlayerConnected(NetworkIdentity player)
    {
        player.gameObject.name = "player_" + player.netId;
        players.Add(new PlayerScoreData() {id = (int)netId, deaths = 0, kills = 0});
        score_text.text = "0";
        //players.Add(new PlayerScoreData() {id = (int)player.assetId});
    }
}