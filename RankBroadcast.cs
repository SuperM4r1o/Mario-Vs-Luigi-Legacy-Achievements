using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class RankBroadcaster : MonoBehaviourPunCallbacks
{
    public void BroadcastRank(int rank)
    {
        // 1. Update the local player's rank globally
        Hashtable props = new Hashtable
        {
            { "Rank", rank }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    // 2. This triggers whenever ANY player updates their properties
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("Rank"))
        {
            // Find the UI bar associated with this player and tell it to update
            RankIconDisplay[] displays = FindObjectsOfType<RankIconDisplay>();
            foreach (var display in displays)
            {
                if (display.targetPlayer == targetPlayer)
                {
                    display.UpdateRankIcon();
                }
            }
        }
    }
}