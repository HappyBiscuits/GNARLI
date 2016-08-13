using IniParser.Model;

namespace GNARLI
{
    public class BoolConfigValue<TSection, TSetting> : ConfigValue<bool, TSection, TSetting>
    {
        public BoolConfigValue(TSection section, TSetting setting, bool defaultValue) : base(section, setting, defaultValue)
        {
        }

        public override void ReadData(IniData data)
        {
            Validate(data);
            var val = true;
           
            if (bool.TryParse(data[Section.ToString()][Setting.ToString()], out val))
            {
                Value = val;
            }
        }
    }
}
