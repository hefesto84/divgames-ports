﻿using System;
using Newtonsoft.Json;

namespace pacman_port.Game.Services
{
    [Serializable]
    public class SpriteDataEntry
    {
        [JsonProperty("id")]
        public string Id;
        [JsonProperty("iX")]
        public int X;
        [JsonProperty("iY")]
        public int Y;
        [JsonProperty("iW")]
        public int Width;
        [JsonProperty("iH")]
        public int Height;
    }
}