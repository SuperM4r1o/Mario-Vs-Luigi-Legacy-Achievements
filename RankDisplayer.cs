using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class RankIconDisplay : MonoBehaviour
{
    public Image iconImage;
    public Sprite[] rankSprites; // Array of 16 sprites (0-15)
    public Player targetPlayer;

    public void UpdateRankIcon()
    {
        if (targetPlayer == null) return;

        // 1. Get the rank from the player's network properties
        int rank = 1000;
        if (targetPlayer.CustomProperties.TryGetValue("Rank", out object rankObj))
        {
            rank = System.Convert.ToInt32(rankObj);
        }

        // 2. Calculate which sprite to show (baseline 1250, increments of 250)
        int index = Mathf.Clamp((rank - 1250) / 250, 0, rankSprites.Length - 1);

        // 3. Update the UI Image component
        if (rankSprites.Length > index)
        {
            iconImage.sprite = rankSprites[index];
            iconImage.enabled = true;
        }
    }
}