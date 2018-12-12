namespace ECTL.Properties
{
    using System;
    using System.CodeDom.Compiler;
    using System.Configuration;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [CompilerGenerated, GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    internal sealed class Settings : ApplicationSettingsBase
    {
        private static Settings defaultInstance = ((Settings) SettingsBase.Synchronized(new Settings()));

        [DebuggerNonUserCode, UserScopedSetting, DefaultSettingValue("True")]
        public bool Beep
        {
            get
            {
                return (bool) this["Beep"];
            }
            set
            {
                this["Beep"] = value;
            }
        }

        public static Settings Default
        {
            get
            {
                return defaultInstance;
            }
        }

        [DebuggerNonUserCode, DefaultSettingValue(""), UserScopedSetting]
        public string filepath
        {
            get
            {
                return (string) this["filepath"];
            }
            set
            {
                this["filepath"] = value;
            }
        }

        [DebuggerNonUserCode, DefaultSettingValue("0"), UserScopedSetting]
        public int GatingValue
        {
            get
            {
                return (int) this["GatingValue"];
            }
            set
            {
                this["GatingValue"] = value;
            }
        }

        [DefaultSettingValue("False"), UserScopedSetting, DebuggerNonUserCode]
        public bool IsReader1
        {
            get
            {
                return (bool) this["IsReader1"];
            }
            set
            {
                this["IsReader1"] = value;
            }
        }

        [UserScopedSetting, DebuggerNonUserCode, DefaultSettingValue("False")]
        public bool IsReader2
        {
            get
            {
                return (bool) this["IsReader2"];
            }
            set
            {
                this["IsReader2"] = value;
            }
        }

        [DefaultSettingValue("0"), UserScopedSetting, DebuggerNonUserCode]
        public int LineConfig
        {
            get
            {
                return (int) this["LineConfig"];
            }
            set
            {
                this["LineConfig"] = value;
            }
        }

        [UserScopedSetting, DebuggerNonUserCode, DefaultSettingValue("")]
        public string Reader1
        {
            get
            {
                return (string) this["Reader1"];
            }
            set
            {
                this["Reader1"] = value;
            }
        }

        [DefaultSettingValue("0"), DebuggerNonUserCode, UserScopedSetting]
        public int Reader1Channel
        {
            get
            {
                return (int) this["Reader1Channel"];
            }
            set
            {
                this["Reader1Channel"] = value;
            }
        }

        [DebuggerNonUserCode, UserScopedSetting, DefaultSettingValue("30")]
        public double Reader1Power
        {
            get
            {
                return (double) this["Reader1Power"];
            }
            set
            {
                this["Reader1Power"] = value;
            }
        }

        [DefaultSettingValue(""), UserScopedSetting, DebuggerNonUserCode]
        public string Reader2
        {
            get
            {
                return (string) this["Reader2"];
            }
            set
            {
                this["Reader2"] = value;
            }
        }

        [DebuggerNonUserCode, UserScopedSetting, DefaultSettingValue("0")]
        public int Reader2Channel
        {
            get
            {
                return (int) this["Reader2Channel"];
            }
            set
            {
                this["Reader2Channel"] = value;
            }
        }

        [DefaultSettingValue("30"), DebuggerNonUserCode, UserScopedSetting]
        public double Reader2Power
        {
            get
            {
                return (double) this["Reader2Power"];
            }
            set
            {
                this["Reader2Power"] = value;
            }
        }

        [DefaultSettingValue("0"), DebuggerNonUserCode, UserScopedSetting]
        public int SearchMode
        {
            get
            {
                return (int) this["SearchMode"];
            }
            set
            {
                this["SearchMode"] = value;
            }
        }

        [DefaultSettingValue("10000"), DebuggerNonUserCode, UserScopedSetting]
        public int TcpIpPort
        {
            get
            {
                return (int) this["TcpIpPort"];
            }
            set
            {
                this["TcpIpPort"] = value;
            }
        }
    }
}

