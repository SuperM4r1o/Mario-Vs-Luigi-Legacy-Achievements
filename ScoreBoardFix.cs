using UnityEngine;
using TMPro;
using Photon.Realtime;
using NSMB.Utils;

public class ScoreboardEntry : MonoBehaviour {
    [SerializeField] private TMP_Text nameText;
    public Player player;

    public void UpdateText() {
        if (player == null) return;

        // FIXED: Pulling rank specifically from this player
        int rank = 1000;
        if (player.CustomProperties.TryGetValue("Rank", out object rankObj)) {
            rank = System.Convert.ToInt32(rankObj);
        }

        int spriteIndex = Mathf.Clamp((rank - 1250) / 250, 0, 15);
        string rankIcon = $"<sprite=\"RankIcons\" index={spriteIndex}> ";
        
        // Ensure the scoreboard name matches the lobby rank style
        nameText.text = $"{rankIcon}{player.GetUniqueNickname()} ({rank})";

        // Apply rainbow name if they have it
        if (player.HasRainbowName()) {
            nameText.color = Utils.GetRainbowColor();
        }
    }
}