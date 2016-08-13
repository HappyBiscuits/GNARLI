using IniParser.Model;

namespace GNARLI
{
    public class IntConfigValue<TSection, TSetting> : ConfigValue<int, TSection, TSetting>
    {
        public IntConfigValue(TSection section, TSetting setting, int defaultValue)
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