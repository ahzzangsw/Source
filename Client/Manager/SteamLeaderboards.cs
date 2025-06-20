using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDefines;

public class CSteamLeaderboards : Singleton<CSteamLeaderboards>
{
    private SteamLeaderboard_t m_SteamLeaderboard;
    private bool m_SteamAPIFailure = false;
    private bool m_SteamAPIProcessing = false;

    private List<RankInfo_Spawn> m_leaderboardEntries = new List<RankInfo_Spawn>();
    private int entryTotalCount = 0;
    private int m_DetailsLength = 1;

    public static int Compare(RankInfo_Spawn A, RankInfo_Spawn B)
    {
        if (A.score != B.score)
            return A.score > B.score ? -1 : 1;

        return A.time > B.time ? 1 : -1;
    }
    public void UploadScores(int stage, int second)
    {
        StartCoroutine(OnUploadScores(stage, second));
    }
    private IEnumerator OnUploadScores(int stage, int second)
    {
        if (SteamManager.Initialized == false)
            yield break;

        CallResult<LeaderboardFindResult_t> findResult = new CallResult<LeaderboardFindResult_t>();
        SteamAPICall_t hSteamAPICall = SteamUserStats.FindLeaderboard("SPAWNRANKSTAGE");

        findResult.Set(hSteamAPICall, (pCallback, failure) => {
            m_SteamAPIFailure = failure;
            m_SteamLeaderboard = pCallback.m_hSteamLeaderboard;
            m_SteamAPIProcessing = false;
        });

        yield return new WaitUntil(() => !m_SteamAPIProcessing);

        if (m_SteamAPIFailure)
            yield break;

        CSteamID[] users = {SteamUser.GetSteamID()};
        hSteamAPICall = SteamUserStats.DownloadLeaderboardEntriesForUsers(m_SteamLeaderboard, users, users.Length);

        bool shouldUploadNewScore = true;
        CallResult<LeaderboardScoresDownloaded_t> downloadResult = new CallResult<LeaderboardScoresDownloaded_t>();
        m_SteamAPIProcessing = true;

        downloadResult.Set(hSteamAPICall, (pCallback, failure) => {
            m_SteamAPIFailure = failure;

            if (failure != true && pCallback.m_cEntryCount > 0)
            {
                int count = pCallback.m_cEntryCount;

                LeaderboardEntry_t leaderboardEntry;
                if (SteamUserStats.GetDownloadedLeaderboardEntry(pCallback.m_hSteamLeaderboardEntries, 0, out leaderboardEntry, null, 0))
                {
                    CSteamID userId = leaderboardEntry.m_steamIDUser;
                    string userName = SteamFriends.GetFriendPersonaName(userId);

                    if (leaderboardEntry.m_nScore >= stage)
                    {
                        shouldUploadNewScore = false;
                    }
                }

                m_SteamAPIProcessing = false;
            }
        });

        yield return new WaitUntil(() => !m_SteamAPIProcessing);

        if (shouldUploadNewScore == false)
            yield break;

        int[] details = new int[] { second };
        CallResult<LeaderboardScoreUploaded_t> uploadResult = new CallResult<LeaderboardScoreUploaded_t>();
        m_SteamAPIProcessing = true;

        hSteamAPICall = SteamUserStats.UploadLeaderboardScore(m_SteamLeaderboard, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate, stage, details, m_DetailsLength);

        uploadResult.Set(hSteamAPICall, (pCallback, failure) => {
            m_SteamAPIFailure = failure;
            m_SteamAPIProcessing = false;
        });

        yield return new WaitUntil(() => !m_SteamAPIProcessing);
    }

    public void DownloadScores()
    {
        StartCoroutine(OnDownloadScores());
    }
    private IEnumerator OnDownloadScores()
    {
        if (SteamManager.Initialized == false)
            yield break;

        CallResult<LeaderboardFindResult_t> findResult = new CallResult<LeaderboardFindResult_t>();
        SteamAPICall_t hSteamAPICall = SteamUserStats.FindLeaderboard("SPAWNRANKSTAGE");

        findResult.Set(hSteamAPICall, (pCallback, failure) => {
            m_SteamAPIFailure = failure;
            m_SteamLeaderboard = pCallback.m_hSteamLeaderboard;
            m_SteamAPIProcessing = false;
        });

        yield return new WaitUntil(() => !m_SteamAPIProcessing);

        if (m_SteamAPIFailure)
            yield break;

        m_SteamAPIProcessing = true;

        entryTotalCount = SteamUserStats.GetLeaderboardEntryCount(m_SteamLeaderboard);
        if (entryTotalCount > 10)
            entryTotalCount = 10;
        else if (entryTotalCount <= 0)
            entryTotalCount = 3;

        hSteamAPICall = SteamUserStats.DownloadLeaderboardEntries(m_SteamLeaderboard, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, 1, entryTotalCount);
        CallResult<LeaderboardScoresDownloaded_t> downloadedResult = new CallResult<LeaderboardScoresDownloaded_t>();
        downloadedResult.Set(hSteamAPICall, (pCallback, failure) => {
            m_SteamAPIFailure = failure;

            if (!failure && pCallback.m_cEntryCount > 0)
            {
                int entryCount = pCallback.m_cEntryCount;

                for (int i = 0; i < entryCount; ++i)
                {
                    LeaderboardEntry_t leaderboardEntry;
                    int[] details = new int[m_DetailsLength];
                    SteamUserStats.GetDownloadedLeaderboardEntry(pCallback.m_hSteamLeaderboardEntries, i, out leaderboardEntry, details, m_DetailsLength);

                    if (details.Length != m_DetailsLength)
                        continue;

                    string playerName = SteamFriends.GetFriendPersonaName(leaderboardEntry.m_steamIDUser);
                    bool bMine = SteamUser.GetSteamID() == leaderboardEntry.m_steamIDUser;
                    RankInfo_Spawn rankInfo = new RankInfo_Spawn(leaderboardEntry.m_nScore, details[0], playerName, bMine);
                    m_leaderboardEntries.Add(rankInfo);
                }
                m_leaderboardEntries.Sort(Compare);
            }

            m_SteamAPIProcessing = false;
        });

        yield return new WaitUntil(() => !m_SteamAPIProcessing);
    }

    public RankInfo_Spawn GetRankInfoSpawn(int index)
    {
        if (index < 0 || index >= m_leaderboardEntries.Count)
            return new RankInfo_Spawn(0, 0, "", false);

        return m_leaderboardEntries[index];
    }
};
