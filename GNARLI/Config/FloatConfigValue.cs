using IniParser.Model;

namespace GNARLI
{
    public class FloatConfigValue<TSection, TSetting> : ConfigValue<float, TSection, TSetting>
    {
        public FloatConfigValue(TSection section, TSetting setting, float defaultValue)
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