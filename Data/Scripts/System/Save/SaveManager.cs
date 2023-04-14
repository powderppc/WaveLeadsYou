using KanKikuchi.AudioManager;

namespace PowderPPC.System.SaveData
{
    public class SaveManager : SingletonMonoBehaviour<SaveManager>
    {


        public AchievementSaveData AchievementSaveData { get; private set; }

        public SoundSaveData SoundSaveData { get; private set; }


        // Start is called before the first frame update
        void Start()
        {

            AchievementSaveData = new AchievementSaveData();
            AchievementSaveData.Load();

            SoundSaveData = new SoundSaveData();
            SoundSaveData.Load();

        }

    }
}