using IniParser.Model;

namespace GNARLI
{
    public class FloatConfigValue : ConfigValue<float>
    {
        public FloatConfigValue(ConfigSection section, ConfigSetting setting, float defaultValue)
            : base(section, setting, defaultValue)
        {
        }

        public override void ReadData(IniData data)
        {
            Validate(data);
            var val = 0f;
            if (float.TryParse(data[Section.ToString()][Setting.ToString()], out val))
            {
                Value = val;
            }

        }
    }
}