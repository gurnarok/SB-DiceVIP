using System;


#if EXTERNAL_EDITOR
public class TempVIP : CPHInlineBase
#else
public class CPHInline
#endif
{
    public bool Execute()
    {
        // THE PLAN
        // 1. User rolls D20
        // 2. if 20 -> TEMP VIP
        // 3. if 1 < roll < 20 -> Witty message
        // 4. if 1 -> timeout with msg

        CPH.TryGetArg("userId", out string userId);
        CPH.TryGetArg("username", out string username);
        CPH.TryGetArg("msgId", out string msgId);

        int tempvip_duration = CPH.GetGlobalVar<int>("tempvip_duration", true);
        if(tempvip_duration<1)
        {
            // DEFAULT DAYS
            tempvip_duration=30;
            CPH.SetGlobalVar("tempvip_duration", tempvip_duration);
        }
        
        string groupName = "TEMPVIP";
        if (!CPH.GroupExists(groupName))
        {
            CPH.AddGroup(groupName);
        }
        int usr_roll = CPH.Between(1,20);

        string outputMsg = "";

        //Check if user is in group already
        if(CPH.UserIdInGroup(userId, Platform.Twitch, groupName))
        {
            outputMsg = $"You are TEMP VIP already! But you rolled {usr_roll}";
        }
        else
        {
            // Get the current time and add tempvip_duration in days
            DateTimeOffset curTime = DateTimeOffset.UtcNow;
            long tempvip_expiry = curTime.AddDays(tempvip_duration).ToUnixTimeSeconds();

            // Users roll
            if(usr_roll==1)
            {
                // Crit failure o7
                outputMsg = $"You rolled a NAT1 RAAAHHH";
                CPH.TwitchTimeoutUser(username,30,"[GAME] Rolled a critical failure in !d20.");
            }
            else if (usr_roll == 20)
            {
                // Crit success \o/
                outputMsg = $"WAHOO You just rolled a NAT20 and became a VIP! PogChamp";
                CPH.AddUserIdToGroup(userId,Platform.Twitch,groupName);
                CPH.SetTwitchUserVarById(userId,"tempvip_expiry",tempvip_expiry,true);
                CPH.TwitchAddVip(username);
            }
            else
            {
                // Meh success -.-
                outputMsg = $"Sorry, it's not your day. Rolled {usr_roll}";
            }
        }

        CPH.TwitchReplyToMessage(outputMsg, msgId);
        return true;
    }
}