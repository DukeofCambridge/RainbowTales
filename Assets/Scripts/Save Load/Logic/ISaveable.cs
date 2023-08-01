namespace Rainbow.Save
{
    public interface ISaveable
    {
        string GUID { get; }
        void RegisterSaveable()
        {
            SaveLoadManager.Instance.RegisterSaveable(this);
        }
        GameSaveData GenerateSaveData();
        void RestoreData(GameSaveData saveData);
    }
}