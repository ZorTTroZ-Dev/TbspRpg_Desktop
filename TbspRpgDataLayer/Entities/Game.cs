using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace TbspRpgDataLayer.Entities
{
    public class Game
    {
        public int Id { get; set; }
        public int AdventureId { get; set; }
        public int LocationId { get; set; }
        public string Language { get; set; }
        public long LocationUpdateTimeStamp { get; set; }
        public string GameState { get; set; }

        public Adventure Adventure { get; set; }
        public Location Location { get; set; }
        public ICollection<Content> Contents { get; set; }
        public ICollection<AdventureObjectGameState> AdventureObjectGameStates { get; set; }

        public JsonObject GameStateJson { get; set; }

        public void LoadGameStateJson()
        {
            if (GameStateJson != null) return;
            GameState ??= "{}";
            try
            {
                var node = JsonNode.Parse(GameState);
                if (node != null)
                    GameStateJson = node.AsObject();
            
                if(GameStateJson == null)
                    throw new JsonException("invalid game state json");
            }
            catch (Exception e)
            {
                throw new JsonException("invalid game state json");
            }
        }
        
        public void SetGameStatePropertyNumber(string key, decimal value)
        {
            LoadGameStateJson();
            GameStateJson[key] = value;
            GameState = GameStateJson.ToJsonString();
        }
        
        public void SetGameStatePropertyString(string key, string value)
        {
            LoadGameStateJson();
            GameStateJson[key] = value;
            GameState = GameStateJson.ToJsonString();
        }
        
        public void SetGameStatePropertyBoolean(string key, bool value)
        {
            LoadGameStateJson();
            GameStateJson[key] = value;
            GameState = GameStateJson.ToJsonString();
        }

        public decimal GetGameStatePropertyNumber(string key)
        {
            LoadGameStateJson();
            var numberValue = GameStateJson[key];
            if(numberValue != null)
                return (decimal) GameStateJson[key];
            return 0;
        }
        
        public string GetGameStatePropertyString(string key)
        {
            LoadGameStateJson();
            var stringValue = GameStateJson[key];
            if(stringValue != null)
                return (string) GameStateJson[key];
            return null;
        }
        
        public bool GetGameStatePropertyBoolean(string key)
        {
            LoadGameStateJson();
            var boolValue = GameStateJson[key];
            if(boolValue != null)
                return (bool) GameStateJson[key];
            return false;
        }
    }
}