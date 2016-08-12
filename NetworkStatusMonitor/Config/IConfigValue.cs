using IniParser.Model;

namespace GNARLI
{
    public interface IConfigValue
    {
        ConfigSection GetSection();
        ConfigSetting GetSetting();
        void WriteData(IniData data);
        void ReadData(IniData data);
    }
}