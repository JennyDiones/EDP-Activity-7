using System.Configuration;
using System.Runtime.CompilerServices;

namespace BookstoreIS.Properties
{
    [CompilerGenerated]
    internal sealed partial class Settings : ApplicationSettingsBase
    {
        private static Settings defaultInstance =
            (Settings)Synchronized(new Settings());

        public static Settings Default => defaultInstance;

        [UserScopedSetting()]
        [DefaultSettingValue("")]
        public string RememberedUser
        {
            get => (string)this[nameof(RememberedUser)];
            set => this[nameof(RememberedUser)] = value;
        }
    }
}
