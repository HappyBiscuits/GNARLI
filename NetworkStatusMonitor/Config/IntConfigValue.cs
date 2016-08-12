using IniParser.Model;

namespace GNARLI
{
    public class IntConfigValue : ConfigValue<int>
    {
        public IntConfigValue(ConfigSection section, ConfigSetting setting, int defaultValue)
            : base(section, setting, defaultValue)
        {

        }

        public override void ReadData(IniData data)
        {
            Validate(data);
            var val = 0;
            if (int.TryParse(data[Section.ToString()][Setting.ToString()], out val))
            {
                Value = val;
            }

        }
    }
}