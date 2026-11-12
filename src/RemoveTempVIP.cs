using System;
using System.Collections.Generic;

#if EXTERNAL_EDITOR
public class RemoveTempVIP : CPHInlineBase
#else
public class CPHInline
#endif
{
    public bool Execute()
    {
        // THE PLAN
        // 1. Get users that are in the TEMPVIP group
        string groupName = "TEMPVIP";

        if (!CPH.GroupExists(groupName))
        {
            CPH.AddGroup(groupName);
        }

        List<GroupUser> groupUsers = CPH.UsersInGroup(groupName);

        foreach(GroupUser userInfo in groupUsers)
        {
            string uid = userInfo.Id;
            string uname = userInfo.Username;
            long curTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long tempvip_expiry = CPH.GetTwitchUserVarById<long>(uid,"tempvip_expiry", true);
            if (curTime > tempvip_expiry)
            {
                CPH.RemoveUserIdFromGroup(uid,Platform.Twitch,groupName);
                CPH.TwitchRemoveVip(uname);
            }
        }

        return true;
    }
}