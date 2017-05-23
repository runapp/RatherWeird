﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RatherWeird
{
    public class Preferences
    {
        public static SettingEntries Load()
        {
            SettingEntries settings = new SettingEntries();
            
            if (!File.Exists(Constants.SettingsFile))
                return settings;

            XmlSerializer serial = new XmlSerializer(settings.GetType());
            settings = (SettingEntries) serial.Deserialize(new StreamReader(Constants.SettingsFile));

            return settings;
        }

        public static bool Write(SettingEntries settings)
        {
            bool result = false;

            try
            {
                XmlSerializer serial = new XmlSerializer(settings.GetType());
                serial.Serialize(new StreamWriter(Constants.SettingsFile), settings);

                result = true;
            }
            catch (IOException ex)
            {
                // TODO log or whatever
            }
            catch (Exception ex)
            {
                // TODO: Same 
            }

            return result;
        }
    }

    public class SettingEntries
    {
        public bool LockCursor;
        public bool InvokeAltUp;

        public SettingEntries()
        {
            LockCursor = true;
            InvokeAltUp = true;
        }
    }


}