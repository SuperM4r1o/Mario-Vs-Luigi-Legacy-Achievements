//Add this to PlayerListEntry

RankIconDisplay rankDisplay = GetComponentInChildren<RankIconDisplay>();
if (rankDisplay != null) {
    rankDisplay.targetPlayer = player;
    rankDisplay.UpdateRankIcon();
}