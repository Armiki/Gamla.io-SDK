using System.Collections.Generic;
using Gamla.Logic;

namespace Gamla.Data
{
    public enum GUIWarningType
    {
        NoVideo = 0,
        NoBonusCash = 1,
        NoRegisterUserOnWithdraw = 2,
        InvalidAmount = 3,
        NoInternet = 4,
        UnavalibleRegion = 5,
        NotChangePersonInfo = 6,
        RematchRequest = 7,
        RematchRequestCallback = 8,
        CheckRegion = 9,
        LogOutAsk = 10,
        LowLevel = 11,
        GuestsUnavailable = 12,
        LogOutGuestAsk = 13
    }

    public enum GUIInfoType
    {
        MakeDepositSuccess = 0,
        SendResetLinkSuccess = 1,
        ResetPassSuccess = 2,
        TransferAmountSuccess = 3,
        TutorialReset = 4,
        ChangePersonInfoSuccess = 5,
        ConfirmCodeSuccess = 6,
        ReferralCodeSuccess = 7
    }

    public static class DataConstants
    {
        public static Dictionary<GUIWarningType, GUIInfoWinData> warningDict = new Dictionary<GUIWarningType, GUIInfoWinData>
        {
            {
                GUIWarningType.NoVideo,
                new GUIInfoWinData {
                    title = LocalizationManager.Text("gamla.novideo.title"),
                    description = LocalizationManager.Text("gamla.novideo.description"),
                    closeTitle = LocalizationManager.Text("gamla.novideo.closetitle") }
            },
            {
                GUIWarningType.NoBonusCash,
                new GUIInfoWinData {
                    title = LocalizationManager.Text("gamla.nobonuscash.title"),
                    description = LocalizationManager.Text("gamla.nobonuscash.description"),
                    closeTitle = LocalizationManager.Text("gamla.nobonuscash.closetitle") }
            },
             {
                GUIWarningType.NoRegisterUserOnWithdraw,
                new GUIInfoWinData {
                    title = LocalizationManager.Text("gamla.noregisteruseronwithdraw.title"),
                    description = LocalizationManager.Text("gamla.noregisteruseronwithdraw.description"),
                    closeTitle = LocalizationManager.Text("gamla.noregisteruseronwithdraw.closetitle") 
                    // custom action
                }
             },
             {
                GUIWarningType.InvalidAmount,
                new GUIInfoWinData {
                    title = LocalizationManager.Text("gamla.invalidamount.title"),
                    description = LocalizationManager.Text("gamla.invalidamount.description"),
                    closeTitle = LocalizationManager.Text("gamla.invalidamount.closetitle") }
             },
             {
                GUIWarningType.NoInternet,
                new GUIInfoWinData {
                    title = LocalizationManager.Text("gamla.nointernet.title"),
                    description = LocalizationManager.Text("gamla.nointernet.description"),
                    closeTitle = LocalizationManager.Text("gamla.nointernet.closetitle") }
             },
             {
                GUIWarningType.UnavalibleRegion,
                new GUIInfoWinData {
                    title = LocalizationManager.Text("gamla.unavalibleregion.title"),
                    description = LocalizationManager.Text("gamla.unavalibleregion.description"),
                    closeTitle = LocalizationManager.Text("gamla.unavalibleregion.closetitle"),
                    actionTitle = LocalizationManager.Text("gamla.unavalibleregion.actiontitle") }
             },
             {
                 GUIWarningType.NotChangePersonInfo,
                 new GUIInfoWinData {
                     title = LocalizationManager.Text("gamla.notchangepersoninfo.title"),
                     description = LocalizationManager.Text("gamla.notchangepersoninfo.description"),
                     closeTitle = LocalizationManager.Text("gamla.notchangepersoninfo.closetitle"),
                     actionTitle = LocalizationManager.Text("gamla.notchangepersoninfo.actiontitle") }
             },
             {
                 GUIWarningType.RematchRequest,
                 new GUIInfoWinData {
                     logo = "smile_state_4",
                     title = LocalizationManager.Text("gamla.rematchrequest.title"),
                     description = LocalizationManager.Text("gamla.rematchrequest.description"),
                     closeTitle = LocalizationManager.Text("gamla.rematchrequest.closetitle"),
                     actionTitle = LocalizationManager.Text("gamla.rematchrequest.actiontitle") }
             },
             {
                 GUIWarningType.RematchRequestCallback,
                 new GUIInfoWinData {
                     logo = "smile_state_4",
                     title = LocalizationManager.Text("gamla.rematchrequestcallback.title"),
                     description = LocalizationManager.Text("gamla.rematchrequestcallback.description"),
                     closeTitle = LocalizationManager.Text("gamla.rematchrequestcallback.closetitle"),
                     actionTitle = LocalizationManager.Text("gamla.rematchrequestcallback.actiontitle") }
             },
             {
                 GUIWarningType.CheckRegion,
                 new GUIInfoWinData {
                     logo = "smile_state_4",
                     title = LocalizationManager.Text("gamla.checkregion.title"),
                     description = LocalizationManager.Text("gamla.checkregion.description"),
                     closeTitle = LocalizationManager.Text("gamla.checkregion.closetitle"),
                     actionTitle = LocalizationManager.Text("gamla.checkregion.actiontitle") }
             },
             {
                 GUIWarningType.LogOutAsk,
                 new GUIInfoWinData {
                     logo = "smile_state_4",
                     title = LocalizationManager.Text("gamla.logoutask.title"),
                     description = LocalizationManager.Text("gamla.logoutask.description"),
                     closeTitle = LocalizationManager.Text("gamla.logoutask.closetitle"),
                     actionTitle = LocalizationManager.Text("gamla.logoutask.actiontitle") }
             },
             {
                 GUIWarningType.LogOutGuestAsk,
                 new GUIInfoWinData {
                     logo = "smile_state_4",
                     title = LocalizationManager.Text("gamla.logoutask.title"),
                     description = LocalizationManager.Text("gamla.logoutask.description.guest"),
                     closeTitle = LocalizationManager.Text("gamla.logoutask.closetitle"),
                     actionTitle = LocalizationManager.Text("gamla.logoutask.actiontitle") }
             },
             {
                 GUIWarningType.LowLevel,
                 new GUIInfoWinData {
                     logo = "smile_state_4",
                     title = LocalizationManager.Text("gamla.checkplayer.lowlevel.title"),
                     description = LocalizationManager.Text("gamla.checkplayer.lowlevel.description"),
                     closeTitle = LocalizationManager.Text("gamla.btn.close"),
                     actionTitle = "" }
             },             
             {
                 GUIWarningType.GuestsUnavailable,
                 new GUIInfoWinData {
                     logo = "smile_state_4",
                     title = LocalizationManager.Text("gamla.checkplayer.guest.title"),
                     description = LocalizationManager.Text("gamla.checkplayer.guest.description"),
                     closeTitle = LocalizationManager.Text("gamla.btn.close"),
                     actionTitle = LocalizationManager.Text("gamla.btn.signup") }
             },
        };

        public static Dictionary<GUIInfoType, GUIInfoWinData> infoDict = new Dictionary<GUIInfoType, GUIInfoWinData>
        {
            {
                GUIInfoType.MakeDepositSuccess,
                new GUIInfoWinData {
                    title = LocalizationManager.Text("gamla.makedepositsuccess.title"),
                    description = LocalizationManager.Text("gamla.makedepositsuccess.description"),
                    closeTitle = LocalizationManager.Text("gamla.makedepositsuccess.closetitle") }
            },
            {
                GUIInfoType.SendResetLinkSuccess,
                new GUIInfoWinData {
                    title = LocalizationManager.Text("gamla.sendresetlinksuccess.title"),
                    description = LocalizationManager.Text("gamla.sendresetlinksuccess.description"),
                    closeTitle = LocalizationManager.Text("gamla.sendresetlinksuccess.closetitle") }
            },
             {
                GUIInfoType.ResetPassSuccess,
                new GUIInfoWinData {
                    title = LocalizationManager.Text("gamla.resetpasssuccess.title"),
                    description = LocalizationManager.Text("gamla.resetpasssuccess.description"),
                    closeTitle = LocalizationManager.Text("gamla.resetpasssuccess.closetitle") }
            },
             {
                GUIInfoType.TransferAmountSuccess,
                new GUIInfoWinData {
                    title = LocalizationManager.Text("gamla.transferamountsuccess.title"),
                    description = LocalizationManager.Text("gamla.transferamountsuccess.description"),
                    closeTitle = LocalizationManager.Text("gamla.transferamountsuccess.closetitle") }
            },
             {
                GUIInfoType.TutorialReset,
                new GUIInfoWinData {
                    title = LocalizationManager.Text("gamla.tutorialreset.title"),
                    description = LocalizationManager.Text("gamla.tutorialreset.description"),
                    closeTitle = LocalizationManager.Text("gamla.tutorialreset.closetitle") }
             },
             {
                GUIInfoType.ChangePersonInfoSuccess,
                new GUIInfoWinData {
                    title = LocalizationManager.Text("gamla.changepersoninfosuccess.title"),
                    description = LocalizationManager.Text("gamla.changepersoninfosuccess.description"),
                    closeTitle = LocalizationManager.Text("gamla.changepersoninfosuccess.closetitle") }
             },
             {
                 GUIInfoType.ReferralCodeSuccess,
                 new GUIInfoWinData {
                     title = LocalizationManager.Text("gamla.referralcodesuccess.title"),
                     description = LocalizationManager.Text("gamla.referralcodesuccess.description"),
                     closeTitle = LocalizationManager.Text("gamla.referralcodesuccess.closetitle") }
             },
        };

    }
}
