namespace PowderPPC.System.SaveData
{

    public class SoundSaveData
    {
        public float MasterVolume;
        public float BGMVolume;
        public float SEVolume;

        public int BGMIndex;
        public int SEIndex;

        string fileName = "SoundData.es3";

        public SoundSaveData()
        {
            MasterVolume = 1f;
            BGMVolume = 0.6f;
            SEVolume = 0.6f;
            BGMIndex = 2;
            SEIndex = 2;
        }

        public void Save()
        {

            var ES3File = new ES3File(fileName);

            ES3File.Save<float>("MasterVolume", MasterVolume);
            ES3File.Save<float>("BGMVolume", BGMVolume);
            ES3File.Save<float>("SEVolume", SEVolume);
            ES3File.Save<int>("BGMIndex", BGMIndex);
            ES3File.Save<int>("SEIndex", SEIndex);

            ES3File.Sync();
        }

        public void Load()
        {
            var ES3File = new ES3File(fileName);

            MasterVolume = ES3File.Load<float>("MasterVolume", MasterVolume);
            BGMVolume = ES3File.Load<float>("BGMVolume", BGMVolume);
            SEVolume = ES3File.Load<float>("SEVolume", SEVolume);
            BGMIndex = ES3File.Load<int>("BGMIndex", BGMIndex);
            SEIndex = ES3File.Load<int>("SEIndex", SEIndex);

            ES3File.Sync();
        }

    }
}

