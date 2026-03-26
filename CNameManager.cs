using UnityEngine;
using Photon.Pun;
using TMPro;
using System.Collections.Generic;

public class ChatColorSystem : MonoBehaviourPunCallbacks
{
    private const string COLOR_KEY = "CustomNameColor";

    // Call this from your Chat Input's "OnEndEdit" or "OnSubmit" event
    public void OnChatMessageEntered(string input)
    {
        if (string.IsNullOrEmpty(input)) return;

        if (input.StartsWith("/cname "))
        {
            string hex = input.Replace("/cname ", "").Trim().Replace("#", "");
            
            // Validate hex length
            if (hex.Length == 6)
            {
                ExitGames.Client.Photon.Hashtable prop = new() { { COLOR_KEY, hex } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(prop);
                Debug.Log($"Color updated to #{hex}");
            }
        }
    }

    // This logic handles the "Automatic" coloring without editing other scripts
    void Update()
    {
        // We find all PlayerListEntries or NameTags in the scene
        // Adjust "PlayerListEntry" to the name of your target script
        PlayerListEntry[] entries = FindObjectsOfType<PlayerListEntry>();

        foreach (var entry in entries)
        {
            if (entry.player == null) continue;

            // Check if that specific player has a custom color set
            if (entry.player.CustomProperties.TryGetValue(COLOR_KEY, out object hexObj))
            {
                // We use TryParse to convert the hex string back to a Unity Color
                if (ColorUtility.TryParseHtmlString("#" + (string)hexObj, out Color customColor))
                {
                    // Access the text component directly from the entry
                    // Note: This requires 'nameText' in PlayerListEntry to be public or accessible
                    var textMesh = entry.GetComponentInChildren<TMP_Text>();
                    if (textMesh != null)
                    {
                        textMesh.color = customColor;
                    }
                }
            }
        }
    }

    // Optional: Also check when a player joins to ensure colors are updated
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey(COLOR_KEY))
        {
            // The Update() loop above will catch this, but forcing a refresh here is smoother
            Debug.Log($"{targetPlayer.NickName} changed their color!");
        }
    }
}