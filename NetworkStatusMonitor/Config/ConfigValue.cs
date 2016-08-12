using IniParser.Model;

namespace GNARLI
{
    public abstract class ConfigValue<T> : IConfigValue
    {
        public ConfigSection Section;
        public ConfigSetting Setting;
        public T Value;
        public T DefaultValue;

        public ConfigSection GetSection()
        {
            return Section;
        }

        public ConfigSetting GetSetting()
        {
            return Setting;
        }

        public virtual void WriteData(IniData data)
        {
            Validate(data);
            data[Section.ToString()][Setting.ToString()] = Value.ToString();
        }

        protected ConfigValue(ConfigSection section, ConfigSetting setting, T defaultValue)
        {
            Section = section;
            Setting = setting;
            DefaultValue = defaultValue;
        }

        protected void Validate(IniData data)
        {
            if (!data.Sections.ContainsSection(Section.ToString()))
            {
                data.Sections.AddSection(Section.ToString());
            }
            if (!data[Section.ToString()].ContainsKey(Setting.ToString()))
            {
                data[Section.ToString()].AddKey(Setting.ToString());
                data[Section.ToString()][Setting.ToString()] = DefaultValue.ToString();
            }

        }

        public abstract void ReadData(IniData data);

    }
}