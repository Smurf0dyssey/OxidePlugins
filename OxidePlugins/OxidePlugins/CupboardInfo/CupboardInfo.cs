﻿using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Oxide.Plugins
{
    [Info("MJSU", "MJSU", "0.0.1")]
    [Description("Cupboard Info")]
    // ReSharper disable once UnusedMember.Global
    class CupboardInfo : RustPlugin
    {

        private PluginConfig _pluginConfig;

        private readonly string _usePermission = "cupboardinfo.use";

        private void Loaded()
        {
            _pluginConfig = Config.ReadObject<PluginConfig>();

            lang.RegisterMessages(new Dictionary<string, string>()
            {
                ["Cleared"] = "You have successfully cleared the cupboard list",
                ["Authorized"] = "List of users also authorized on this cupboard:",
                ["StillAuthoried"] = "List of users still authorized on this cupboard:"
            }, this);

            permission.RegisterPermission(_usePermission, this);
        }

        protected override void LoadDefaultConfig()
        {
            Config.WriteObject(DefaultConfig(), true);
        }

        private PluginConfig DefaultConfig()
        {
            return new PluginConfig
            {
                Prefix = "[<color=yellow>Cupboard Info</color>]",
                UsePermission = false
            };
        }

        // ReSharper disable once UnusedMember.Local
        void OnCupboardClearList(BuildingPrivlidge privilege, BasePlayer player)
        {
            if(CheckPermission(player, _usePermission, false))
                PrintToChat(player, $"{_pluginConfig.Prefix} {Lang("Cleared", player.UserIDString)}");
        }

        // ReSharper disable once UnusedMember.Local
        void OnCupboardAuthorize(BuildingPrivlidge privilege, BasePlayer player)
        {
            if (privilege.authorizedPlayers.Count <= 0) return;
            if (!CheckPermission(player, _usePermission, false)) return;
            PrintToChat(player, $"{_pluginConfig.Prefix} {Lang("Authorized", player.UserIDString)}");
            DisplayCupboardData(privilege, player);
        }

        // ReSharper disable once UnusedMember.Local
        void OnCupboardDeauthorize(BuildingPrivlidge privilege, BasePlayer player)
        {
            if (privilege.authorizedPlayers.Count <= 0) return;
            if (!CheckPermission(player, _usePermission, false)) return;
            PrintToChat(player, $"{_pluginConfig.Prefix} {Lang("StillAuthoried", player.UserIDString)}");
            DisplayCupboardData(privilege, player);
        }

        private void DisplayCupboardData(BuildingPrivlidge privilege, BasePlayer player)
        {
            foreach (ProtoBuf.PlayerNameID user in privilege.authorizedPlayers)
            {
                if (user.userid != player.userID)
                {
                    PrintToChat(player, $" {user.userid} {user.username}");
                }
            }
        }

        #region Helper Methods

        //////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// @credit to Exel80 - code from EasyVote
        /// </summary>
        /// <param name="key"></param>
        /// <param name="id"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        ///  //////////////////////////////////////////////////////////////////////////////////////
        private string Lang(string key, string id = null, params object[] args)
            => string.Format(lang.GetMessage(key, this, id), args);

        //////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Checks if the user has the given permissions. Displays an error to the user if ShowText is true
        /// </summary>
        /// <param name="player">Player to be checked</param>
        /// <param name="perm">Permission to check for</param>
        /// <param name="showText">Should display no permission</param>
        /// <returns></returns>
        /// //////////////////////////////////////////////////////////////////////////////////////
        private bool CheckPermission(BasePlayer player, string perm, bool showText)
        {
            if (!_pluginConfig.UsePermission || permission.UserHasPermission(player.UserIDString, perm))
            {
                return true;
            }

            if (showText) //player doesn't have permission. Should we show them a no permission message
            {
                PrintToChat(player, $"{Lang(_pluginConfig.Prefix)} {Lang("NoPermission", player.UserIDString)}");
            }

            return false;
        }

        #endregion

        class PluginConfig
        {
            public string Prefix;
            public bool UsePermission;
        }
    }
}