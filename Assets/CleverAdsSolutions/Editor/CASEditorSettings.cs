﻿//
//  Clever Ads Solutions Unity Plugin
//
//  Copyright © 2023 CleverAdsSolutions. All rights reserved.
//

using System;
using UnityEngine;

namespace CAS.UEditor
{
    [Serializable]
    public class CASEditorSettings : ScriptableObject
    {
        public bool autoCheckForUpdatesEnabled = true;
        public bool delayAppMeasurementGADInit = true;
        public bool buildPreprocessEnabled = true;

        /// <summary>
        /// ISO2 such as US, RU ...
        /// </summary>
        public string mostPopularCountryOfUsers = "BR";

        public bool multiDexEnabled = true;
        public Permission permissionAdId = Permission.Auto;
        [Obsolete("Exo player used in any case")]
        public bool exoPlayerIncluded = true;
        public bool generateAndroidQuerriesForPromo = true;
        public bool updateGradlePluginVersion = true;

        public bool generateIOSDeepLinksForPromo = true;
        public bool bitcodeIOSDisabled = true;
        public string attributionReportEndpoint = null;

        /// <summary>
        /// {key}.lproj Language code (ISO-639)
        /// NSUserTrackingUsageDescription = {value}
        /// 
        /// https://www.ibabbleon.com/iOS-Language-Codes-ISO-639.html
        /// </summary>
        public KeyValuePair[] userTrackingUsageDescription = new KeyValuePair[0];


        public static CASEditorSettings Load(bool createAsset = false)
        {
            var asset = CASEditorUtils.GetSettingsAsset("CASEditorSettings",
                CASEditorUtils.editorFolderPath, typeof(CASEditorSettings), createAsset, null);
            if (asset)
                return (CASEditorSettings)asset;
            return CreateInstance<CASEditorSettings>();
        }

        public bool isUseAdvertiserIdLimited(Audience audience)
        {
            var permission = permissionAdId;
            if (permission == Permission.Auto)
                return audience == Audience.Children;
            return permission == Permission.Removed;
        }

        public enum Permission
        {
            Auto = 0,
            Required = 1,
            Removed = 2
        }
    }
}
