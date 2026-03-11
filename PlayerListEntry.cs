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

    private void OnDestroy() {
        if (blockerInstance)
            Destroy(blockerInstance);
    }

    public void Update() {
        // Only run this if the player has a rainbow name to update colors
        if (player != null && player.HasRainbowName()) {
            nameText.color = Utils.GetRainbowColor();
        }
    }

    public void UpdateText() {
        colorStrip.color = Utils.GetPlayerColor(player, 1f, 1f);
        enabled = player.HasRainbowName();

        // --- RANK SYSTEM LOGIC ---
        // 1. Get the player's rank (default to 1000 as the starting point)
        int rank = PlayerPrefs.GetInt("PlayerRank", 1000);
        if (player.CustomProperties.TryGetValue("Rank", out object rankObj)) {
            rank = System.Convert.ToInt32(rankObj);
        }

        // 2. Map Rank to Sprite Index (0-15)
        // Progression starts at 1250 and increases by 250 per icon
        int spriteIndex = Mathf.Clamp((rank - 1250) / 250, 0, 15);

        // 3. Update the UI Text
        string rankIcon = $"<sprite={spriteIndex}> ";
        string characterSymbol = Utils.GetCharacterData(player).uistring;
        
        // Determine if room is ranked for brackets vs parentheses
        bool isRanked = false;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("Ranked", out object rObj)) {
            isRanked = (bool)rObj;
        }
        string rankTag = isRanked ? $"({rank})" : $"[{rank}]";

        // Apply formatting: [Rank Icon] [Char Icon] [Name] [Rank Number]
        nameText.text = $"{rankIcon}{characterSymbol}{player.GetUniqueNickname()} {rankTag}";
        
        // Ensure the color logic still runs
        if (player.HasRainbowName()) {
            nameText.color = Utils.GetRainbowColor();
        }
        // -------------------------

        string permissionSymbol = "";
        if (player.IsMasterClient)
            permissionSymbol += "<sprite=5>";

        Utils.GetCustomProperty(Enums.NetPlayerProperties.Status, out bool status, player.CustomProperties);
        if (status)
            permissionSymbol += "<sprite=26>";

        Utils.GetCustomProperty(Enums.NetPlayerProperties.Ping, out int ping, player.CustomProperties);

        string pingColor;
        if (ping < 0) {
            pingColor = "black";
        } else if (ping < 80) {
            pingColor = "#00b900"; // This is very good
        } else if (ping < 120) {
            pingColor = "#d8b30c"; // This is good
        } else if (ping < 140) {
            pingColor = "orange"; // This is okay..?
        } else if (ping < 160) {
            pingColor = "red"; // This is pretty bad.
        } else {
            pingColor = "#6e228b"; // This is very bad.
        }

        nameText.text = $"{permissionSymbol}{characterSymbol}{player.GetUniqueNickname()} [{rank}]";
        pingText.text = $"<color={pingColor}>{ping}";

        Transform parent = transform.parent;
        int childIndex = 0;
        for (int i = 0; i < parent.childCount; i++) {
            if (parent.GetChild(i) != gameObject)
                continue;
            childIndex = i;
            break;
        }
        layout.layoutPriority = transform.parent.childCount - childIndex;
    }

    public void ShowDropdown() {
        if (blockerInstance)
            Destroy(blockerInstance);

        bool admin = PhotonNetwork.IsMasterClient && !player.IsMasterClient;
        foreach (GameObject option in adminOnlyOptions) {
            option.SetActive(admin);
        }

        Canvas.ForceUpdateCanvases();

        blockerInstance = Instantiate(blockerTemplate, rootCanvas.transform);
        RectTransform blockerTransform = blockerInstance.GetComponent<RectTransform>();
        blockerTransform.offsetMax = blockerTransform.offsetMin = Vector2.zero;
        blockerInstance.SetActive(true);

        background.offsetMin = new(background.offsetMin.x, -options.rect.height);
        options.anchoredPosition = new(options.anchoredPosition.x, -options.rect.height);

        EventSystem.current.SetSelectedGameObject(firstButton);
        MainMenuManager.Instance.sfx.PlayOneShot(Enums.Sounds.UI_Cursor.GetClip());
    }

    public void HideDropdown(bool didAction) {
        Destroy(blockerInstance);

        background.offsetMin = new(background.offsetMin.x, 0);
        options.anchoredPosition = new(options.anchoredPosition.x, 0);

        MainMenuManager.Instance.sfx.PlayOneShot((didAction ? Enums.Sounds.UI_Decide : Enums.Sounds.UI_Back).GetClip());
    }

    public void BanPlayer() {
        MainMenuManager.Instance.Ban(player);
        HideDropdown(true);
    }

    public void KickPlayer() {
        MainMenuManager.Instance.Kick(player);
        HideDropdown(true);
    }

    public void MutePlayer() {
        MainMenuManager.Instance.Mute(player);
        HideDropdown(true);
    }

    public void PromotePlayer() {
        MainMenuManager.Instance.Promote(player);
        HideDropdown(true);
    }

    public void CopyPlayerId() { // Why does this lag in the builds but not in the editor?
        TextEditor te = new();
        te.text = player.UserId;
        te.SelectAll();
        te.Copy();
        HideDropdown(true);
    }
}