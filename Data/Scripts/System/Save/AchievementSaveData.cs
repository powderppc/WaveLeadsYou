public class AchievementSaveData {
    public int ClearLevel;

    string fileName = "AchievementData.es3";

    public AchievementSaveData()
    {
        ClearLevel = 0;
    }

    /// <summary>
    /// クリアしたレベルを保存（既にクリア済みなら何もしない）
    /// </summary>
    /// <param name="level"></param>
    public void SaveClearLevel(int level)
    {
        if(level> ClearLevel)
        {
            ClearLevel = level;
            Save();
        }
        Save();
    }

    void Save()
    {

        var ES3File = new ES3File(fileName);
        ES3File.Save<int>("ClearLevel", ClearLevel);

        ES3File.Sync();
    }

    public void Load()
    {
        var ES3File = new ES3File(fileName);

        ClearLevel = ES3File.Load<int>("ClearLevel", ClearLevel);

        ES3File.Sync();
    }

}
