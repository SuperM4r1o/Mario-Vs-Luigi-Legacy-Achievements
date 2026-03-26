using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnifiedSeasonSystem : MonoBehaviour
{
    [System.Serializable]
    public class SeasonData
    {
        public string seasonName;
        // Drag all objects/tilemaps for this season here
        public List<GameObject> seasonObjects; 
        [HideInInspector] public List<SpriteRenderer> renderers = new List<SpriteRenderer>();
    }

    public List<SeasonData> seasons;
    
    [Header("Timing")]
    public float stayDuration = 120f; // 2 minutes
    public float fadeDuration = 45f;  // 45 seconds
    
    private int currentSeasonIndex = 0;

    void Awake()
    {
        // Automatically find all SpriteRenderers in the objects you provided
        foreach (var season in seasons)
        {
            foreach (var obj in season.seasonObjects)
            {
                if (obj == null) continue;
                
                // Get renderers from the object and all its children
                var foundRenderers = obj.GetComponentsInChildren<SpriteRenderer>(true);
                season.renderers.AddRange(foundRenderers);
            }
        }
    }

    void Start()
    {
        // Initial state: Spring (0) starts visible, others invisible
        for (int i = 0; i < seasons.Count; i++)
        {
            bool isStartSeason = (i == 0);
            SetSeasonAlpha(seasons[i], isStartSeason ? 1f : 0f);
            
            foreach(var obj in seasons[i].seasonObjects) 
                obj.SetActive(isStartSeason);
        }

        StartCoroutine(SeasonCycle());
    }

    IEnumerator SeasonCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(stayDuration);

            int nextIndex = (currentSeasonIndex + 1) % seasons.Count;
            yield return StartCoroutine(Crossfade(seasons[currentSeasonIndex], seasons[nextIndex]));

            currentSeasonIndex = nextIndex;
        }
    }

    IEnumerator Crossfade(SeasonData outSeason, SeasonData inSeason)
    {
        // Turn on the next season's objects so they can start fading in
        foreach(var obj in inSeason.seasonObjects) obj.SetActive(true);

        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            SetSeasonAlpha(outSeason, 1f - t);
            SetSeasonAlpha(inSeason, t);

            yield return null;
        }

        SetSeasonAlpha(outSeason, 0f);
        SetSeasonAlpha(inSeason, 1f);

        // Turn off the old season objects to save FPS
        foreach(var obj in outSeason.seasonObjects) obj.SetActive(false);
    }

    void SetSeasonAlpha(SeasonData season, float alpha)
    {
        foreach (var sr in season.renderers)
        {
            if (sr == null) continue;
            Color c = sr.color;
            sr.color = new Color(c.r, c.g, c.b, alpha);
        }
    }
}