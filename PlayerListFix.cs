using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using NSMB.Utils;

public class PlayerListEntry : MonoBehaviour {

    public Player player;

    [SerializeField] private TMP_Text nameText, pingText;
    [SerializeField] private Image colorStrip;
    [SerializeField] private RectTransform background, options;
    [SerializeField] private GameObject blockerTemplate, firstButton;
    [SerializeField] private Canvas rootCanvas;
    [SerializeField] private LayoutElement layout;
    [SerializeField] private GameObject[] adminOnlyOptions;

    private GameObject blockerInstance;

    public void Update() {
        if (player != null && player.HasRainbowName()) {
            nameText.color = Utils.GetRainbowColor();
        }
    }

    public void UpdateText() {
        colorStrip.color = Utils.GetPlayerColor(player, 1f, 1f);
        enabled = player.HasRainbowName();

        // FIXED: Get rank from the specific player's network properties
        int rank = 1000;
        if (player.CustomProperties.TryGetValue("Rank", out object rankObj)) {
            rank = System.Convert.ToInt32(rankObj);
        }

        // Map Rank to Sprite Index (0-15)
        int spriteIndex = Mathf.Clamp((rank - 1250) / 250, 0, 15);

        // Define Icons and Symbols
        // Use the RankIcons TMP Sprite Asset specifically
        string rankIcon = $"<sprite=\"RankIcons\" index={spriteIndex}> "; 
        string characterSymbol = Utils.GetCharacterData(player).uistring; 
        
        bool isRanked = false;
        if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("Ranked", out object rObj)) {
            isRanked = (bool)rObj;
        }
        string rankTag = isRanked ? $"({rank})" : $"[{rank}]";

        string permissionSymbol = "";
        if (player.IsMasterClient) permissionSymbol += "<sprite=5>";

        Utils.GetCustomProperty(Enums.NetPlayerProperties.Status, out bool status, player.CustomProperties);
        if (status) permissionSymbol += "<sprite=26>";

        // FIXED: Combined string to prevent overwriting previous icon logic
        nameText.text = $"{permissionSymbol}{rankIcon}{characterSymbol}{player.GetUniqueNickname()} {rankTag}";

        // Ping Logic
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

        // Layout priority
        layout.layoutPriority = transform.parent.childCount - transform.GetSiblingIndex();
    }
    // ... (Keep ShowDropdown, HideDropdown, etc. as they were)
}