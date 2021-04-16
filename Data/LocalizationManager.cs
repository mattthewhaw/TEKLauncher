﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Windows.Application;
using static TEKLauncher.Data.Settings;
using static TEKLauncher.Utils.TEKArchive;
using static TEKLauncher.Utils.UtilFunctions;

namespace TEKLauncher.Data
{
    internal static class LocalizationManager
    {
        internal static string LocCulture;
        private static readonly Dictionary<LocCode, string> List = new Dictionary<LocCode, string>();
        internal static readonly string[] SupportedCultures = new[] { "en", "es", "pt", "el", "ru", "ar" };
        internal static void LoadLocalization(string CultureCode)
        {
            if (Lang.Length == 0)
            {
                if (CultureCode.Length < 2)
                    LocCulture = "en";
                else
                {
                    LocCulture = CultureCode.Substring(0, 2);
                    if (LocCulture == "be" || LocCulture == "uk")
                        LocCulture = "ru";
                    if (!SupportedCultures.Contains(LocCulture))
                        LocCulture = "en";
                }
                Lang = LocCulture;
            }
            else
                LocCulture = Lang;
            using (Stream ResourceStream = GetResourceStream(new Uri($"pack://application:,,,/Resources/Localizations/{LocCulture}.ta")).Stream)
            using (MemoryStream Stream = new MemoryStream())
            {
                DecompressSingleFile(ResourceStream, Stream);
                Stream.Position = 0L;
                using (StreamReader Reader = new StreamReader(Stream))
                {
                    for (LocCode Code = LocCode.CantLaunchExeMissing; Code <= LocCode.PrepCommunismMode; Code++)
                        List.Add(Code, Reader.ReadLine().Replace(@"\n", "\n"));
                    string AboutFeatures = string.Empty;
                    while (!Reader.EndOfStream)
                    {
                        AboutFeatures += $"• {Reader.ReadLine()}";
                        if (!Reader.EndOfStream)
                            AboutFeatures += "\n";
                    }
                    List.Add(LocCode.AboutFeatures, AboutFeatures);
                }
            }
            HoursString = List[LocCode.Hours];
            MinutesString = List[LocCode.Minutes];
            SecondsString = List[LocCode.Seconds];
        }
        internal static string LocString(LocCode Code) => List[Code];
        internal enum LocCode
        {
            CantLaunchExeMissing,
            CantLaunchCorruped,
            CantLaunchNoSteam,
            CantLaunchNoCreamAPI,
            CantJoinNoInternet,
            CantLaunchNoSpacewar,
            Status,
            DLCUninstPrompt,
            NotInstalled,
            Installed,
            CheckingForUpdates,
            UpdateAvailable,
            Downloading,
            PlayPage,
            ServersPage,
            DLCsPage,
            ModsPage,
            SettingsPage,
            LauncherPage,
            AboutPage,
            InfoFileMissing,
            ModID,
            OriginalID,
            Subscribed,
            NotSubscribed,
            ModIDCopied,
            InstallingMod,
            ModInstallError,
            FailedToSub,
            CantUninstModGameRunning,
            UninstModPrompt,
            FailedToUnsub,
            Dismiss,
            KB,
            MB,
            GB,
            s,
            Hours,
            Minutes,
            Seconds,
            Join,
            IPNotResolved,
            DLCNotInstalled,
            Install,
            LastUpdated,
            KeyFeatures,
            Links,
            DownloadLink,
            LocalizationFile,
            Discords,
            MainInfo,
            AddServers,
            CPServers,
            CPInfo,
            CPMods,
            CPDiscord,
            ClusterName,
            HostedBy,
            Subscribe,
            Loading,
            FixMods,
            GamePath,
            DelSettings,
            CleanDwCache,
            ChangeSID,
            DwThreadsCountTooltip,
            DwThreadsCount,
            ValThreadsCountTooltip,
            ValThreadsCount,
            CloseOnGameRun,
            DowngradeModeTooltip,
            DowngradeMode,
            CommunismMode,
            GamePathPrompt,
            CantUsePath,
            CleanDwCachePrompt,
            DelSettingsPrompt,
            MIARKID,
            MIPlaceholder,
            MIOr,
            BrowseWorkshop,
            MISpacewarID,
            MISelectID,
            Idle,
            ValidatorExc,
            MISubscribing,
            MISubFailed,
            MISuccess,
            Cancelling,
            MIEnterARKID,
            MIARKdictedPrompt,
            MIRequestingDetails,
            MIDefaultModName,
            MIWaitingForSteam,
            MISubSuccess,
            MIEnterSpacewarID,
            MIIDInUse,
            MIRequestFailed,
            MINoARKMod,
            MINotAnARKMod,
            MINoSpacewarMod,
            MINotASpacewarMod,
            MIPvNotASpacewarMod,
            MIPvNotAnARKMod,
            MIPvFailed,
            MIPvNoMod,
            MIAllIDsUsed,
            InstallMod,
            MINoSpacewar,
            RunGameAsAdmin,
            UseSpacewar,
            UseBattlEye,
            Play,
            Lang,
            GameLoc,
            CustomLaunchParameters,
            SwitchLangPrompt,
            NeedCreamAPI,
            NeedNoCreamAPI,
            ServersPageNote,
            ClusterServers,
            ClusterPlayers,
            Update,
            FixBE,
            UnlockSkins,
            UninstallAllMods,
            Validate,
            FixBloom,
            InstallSpacewar,
            InstallReqs,
            FilesUpToDate,
            FilesOutdated,
            FilesMissing,
            CreamAPI,
            Reapply,
            UseGlobalFonts,
            LaunchParameters,
            UseCacheDesc,
            UseCache,
            UseAllCoresDesc,
            UseAllCores,
            HighPriorDesc,
            HighPrior,
            NoFXAAFontsDesc,
            NoFXAAFonts,
            SM4Desc,
            SM4,
            DX10Desc,
            DX10,
            NoSplashDesc,
            NoSplash,
            NoSkyDesc,
            NoSky,
            NoMemBiasDesc,
            NoMemBias,
            LowMemDesc,
            LowMem,
            NoRHIDesc,
            NoRHI,
            NoVSyncDesc,
            NoVSync,
            NoHibDesc,
            NoHib,
            AnselDesc,
            Ansel,
            SPDownloading,
            SPDownloadFailed,
            CantFixBEGameRunning,
            CantFixBEExeMissing,
            BattlEye,
            ExtractingArchive,
            BEFixSuccess,
            FailedToExtract,
            CantFixBloom,
            BloomFixSuccess,
            Requirements,
            InstallingReq,
            InstallReqSuccess,
            SpacewarInstalled,
            SpacewarNeedsSteam,
            GameDownloadBegan,
            SPPrepToDownload,
            CantReapplyGameRunning,
            ReapplySuccess,
            GlobalFonts,
            GlobalFontsSuccess,
            Pause,
            CantInstallCA,
            CantUninstallCA,
            InstallCASuccess,
            UninstallCASuccess,
            CantUnlockSkins,
            LocalProfile,
            SkinUnlockSuccess,
            Uninstall,
            CantUpdateGameRunning,
            CantUpdateNoInternet,
            Search,
            WBTimeout,
            WBPage,
            ConnectToSteamFailed,
            MaxDinoLvl,
            Taming,
            Experience,
            Harvesting,
            Breeding,
            Stacks,
            YourServers,
            DecryptionFailure,
            DecompressionFailure,
            AdlerHashMismatch,
            ReadingOldManifest,
            DwLatestManifest,
            DwManifestFailed,
            ReadingLatestManifest,
            FetchServersFailed,
            ManifestCorrupted,
            ManifestDecryptFailed,
            Game,
            CantInstallUpdate,
            ApplyingRelocs,
            InstallationCorrupted,
            InstallFailedVCDupe,
            PathTooLong,
            NotEnoughSpace,
            ManifestDecryptedIncorrectly,
            ComputingChanges,
            ValidatingFiles,
            ModAlrUpdating,
            ConnectingToSteam,
            RequestingData,
            Timeout,
            Preallocating,
            DownloadingMod,
            DwPaused,
            CInstallingMod,
            ItemAlrUpdating,
            NoPrevManifest,
            GameAlrUpToDate,
            ValidationPaused,
            AlrUpToDate,
            DownloadingPrev,
            DownloadingUpd,
            InstallingUpd,
            DowngradeSuccess,
            UpdateSuccess,
            ModAlrUpToDate,
            CommittingUpd,
            ModDowngradeSuccess,
            ModUpdSuccess,
            CantStartLauncher,
            FailedToInitiateDw,
            ProgressTrackerTimeout,
            ProgresTrackerStopped,
            SelectFolder,
            ModDecompressFailed,
            AddServer,
            ServerAddress,
            EnterAddress,
            Add,
            ResolvingIP,
            NoIPFound,
            ScanningServers,
            NoServersFound,
            AddServersSuccess,
            RequestingSrvInfo,
            SrvDidnRespond,
            SrvNotSpacewar,
            AddServerSuccess,
            WhatsNew,
            WNUpdate,
            WNPatch,
            Crash,
            CrashCaption,
            CrashUploading,
            NFRequired1,
            NFRequired2,
            NFRequired3,
            CrashSent,
            CrashPost1,
            CrashPost2,
            SIDChanger,
            OldID,
            NewID,
            Change,
            CantChangeSIDGameRunning,
            OIDNotFound,
            NIDAlrInUse,
            SIDChangerRequesting,
            SIDChangerReqFailed,
            SIDChangerNotSpacewarMod,
            SIDChangerModDoesntExist,
            SIDChangerSuccess,
            GameVersion,
            CritFilesMissing,
            LUpdateAvailable,
            NoInternet,
            Latest,
            Outdated,
            ARKUpdateAvailable,
            None,
            CheckingForDLCUpds,
            DLCUpdsAvailable,
            NA,
            LauncherClosePrompt,
            MWError,
            MWInfo,
            MWWarning,
            Yes,
            No,
            OK,
            MFCaption,
            MFVerifying,
            MFVerificationComplete,
            MFApplyingModifications,
            MFSteamRestartRequired,
            MFSuccess,
            MUCaption,
            MUSelectOptions,
            MUUnsubAllMods,
            MUDelWorkshop,
            MUDelInstalled,
            MUCleanACF,
            MUCleanDwCache,
            MUUninstallMods,
            MUCantUninst,
            MUGameRunning,
            MUPrompt,
            MUUninstalling,
            MUFail,
            SteamShutdownPrompt,
            WaitingForSteamShutdown,
            MUSuccess,
            StartupMessage,
            SelectARKFolder,
            ReqDiskSpace,
            FreeDiskSpace,
            Continue,
            UpdaterCaption,
            UpdaterPrep,
            UpdaterDownloading,
            UpdaterFail,
            UpdaterFailLink,
            DLCValidator,
            ModValidator,
            ValidatorClosePrompt,
            ValidatorWaitingForCancel,
            ValidatorCloseWindow,
            Retry,
            PrepCommunismMode,
            AboutFeatures
        }
    }
}