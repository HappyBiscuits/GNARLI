using IniParser.Model;

namespace GNARLI
{
    public interface IDynamicConfig
    {
        string GetGroupName();
        void LoadData(IniData iniData);
        void SaveData(IniData iniData);
    }
}