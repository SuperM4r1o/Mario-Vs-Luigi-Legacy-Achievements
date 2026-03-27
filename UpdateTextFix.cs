public void UpdateText() {
        colorStrip.color = Utils.GetPlayerColor(player, 1f, 1f);
        enabled = player.HasRainbowName();

        // 1. FIXED: Get the player's unique rank from Photon Properties, NOT PlayerPrefs
        // Using PlayerPrefs was making everyone appear to have YOUR rank.
        int rank = 1250; 
        if (player.CustomProperties.TryGetValue("Rank", out object rankObj)) {
            rank = System.Convert.ToInt32(rankObj);
        }

        // 2. Map Rank to Sprite Index (0-15)
        int spriteIndex = Mathf.Clamp((rank - 1250) / 250, 0, 15);

        // 3. DEFINE SYMBOLS AND ICONS
        // Ensure "RankIcons" is the exact name of your TMP Sprite Asset.
        string rankIcon = $"<sprite=\"RankIcons\" index={spriteIndex}> "; 
        string characterSymbol = Utils.GetCharacterData(player).uistring; 
        
        // 4. Determine if room is ranked for brackets vs parentheses
        bool isRanked = false;
        if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("Ranked", out object rObj)) {
            isRanked = (bool)rObj;
        }
        string rankTag = isRanked ? $"({rank})" : $"[{rank}]";

        // 5. Build Permission Symbols
        string permissionSymbol = "";
        if (player.IsMasterClient) permissionSymbol += "<sprite=5>";

        Utils.GetCustomProperty(Enums.NetPlayerProperties.Status, out bool status, player.CustomProperties);
        if (status) permissionSymbol += "<sprite=26>";

        // 6. FIXED: Apply formatting once
        // Removed the second "nameText.text =" line that was overwriting the rank icon.
        nameText.text = $"{permissionSymbol}{rankIcon}{characterSymbol}{player.GetUniqueNickname()} {rankTag}";

        // 7. Update Ping Logic
        Utils.GetCustomProperty(Enums.NetPlayerProperties.Ping, out int ping, player.CustomProperties);
        string pingColor = ping switch {
            < 0 => "black",
            < 80 => "#00b900",
            < 120 => "#d8b30c",
            < 140 => "orange",
            < 160 => "red",
            _ => "#6e228b"
        };
        pingText.text = $"<color={pingColor}>{ping}";

        // Handle Layout Priority
        Transform parent = transform.parent;
        int childIndex = 0;
        for (int i = 0; i < parent.childCount; i++) {
            if (parent.GetChild(i) != gameObject) continue;
            childIndex = i;
            break;
        }
        layout.layoutPriority = transform.parent.childCount - childIndex;
    }