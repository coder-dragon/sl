using System.Collections.Generic;

namespace SL.Configs
{
    class GiftConfig
    {
        public RandomDelayConfig delay = new RandomDelayConfig();

        public GiftConfigItem[] gifts;

        public GroupConfigItem[] groups;

        public void Init()
        {
            foreach (var group in groups)
            {
                mNameToGroup[group.name] = group;
            }

            foreach (var gift in gifts)
            {
                mGiftToGroup[gift.name] = mNameToGroup[gift.@group];
            }
        }

        public GroupConfigItem GetGroup(string gift)
        {
            if (mGiftToGroup.TryGetValue(gift, out var groupName))
                return groupName;
            return null;
        }

        private readonly Dictionary<string, GroupConfigItem> mGiftToGroup = new Dictionary<string, GroupConfigItem>();
        private readonly Dictionary<string, GroupConfigItem> mNameToGroup = new Dictionary<string, GroupConfigItem>();
    }
}