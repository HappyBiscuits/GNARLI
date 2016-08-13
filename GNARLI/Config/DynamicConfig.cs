using System;
using System.Collections.Generic;
using System.IO;
using IniParser.Model;

namespace GNARLI
{
    public class DynamicConfig<T> : IDynamicConfig
    {
        private readonly string _groupName;
        private readonly Func<KeyDataCollection, T> _setObject;
        private readonly Action<T, KeyDataCollection> _readObject;

        public List<T> Data;

        public DynamicConfig(string groupName, Func<KeyDataCollection, T> setObject, Action<T, KeyDataCollection> readObject, List<T> defaults)
        {
            _groupName = groupName;
            _setObject = setObject;
            _readObject = readObject;
            Data = defaults;
        }

        public string GetGroupName()
        {
            return _groupName;
        }

        public void LoadData(IniData iniData)
        {
            var loop = true;
            var i = 0;
            var data = new List<T>();
            do
            {
                var dat = _groupName + i;
                if (iniData.Sections.ContainsSection(dat))
                {
                    data.Add(GetTypeData(iniData[dat]));
                    i++;
                }
                else
                {
                    loop = false;
                }
            } while (loop);
            if (data.Count > 0)
            {
                Data = data;
            }
        }
        public void SaveData(IniData iniData)
        {
            for (var i = 0; i < Data.Count; i++)
            {
                var dat = _groupName + i;
                iniData.Sections.AddSection(dat);
                _readObject(Data[i], iniData[dat]);
            }
        }
        private T GetTypeData(KeyDataCollection data)
        {

            try
            {
                return _setObject(data);
            }
            catch (Exception e)
            {
                throw new InvalidDataException("Missing/Incorrect SetObject Function: " + e.ToString());
            }

        }

        public void Add(T data)
        {
            Data.Add(data);
        }

        public void AddRange(List<T> dataList)
        {
            Data.AddRange(dataList);
        }

        public void Remove(T data)
        {
            Data.Remove(data);
        }
    }
}