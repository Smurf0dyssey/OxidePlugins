﻿using Oxide.Core;
using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Oxide.Plugins
{
    [Info("HeliTargetInfo", "MJSU", "0.0.1")]
    [Description("Displays who the heli locks onto")]
    // ReSharper disable once UnusedMember.Global
    class HeliTargetInfo : RustPlugin
    {
        private PluginConfig _pluginConfig;
        private readonly Hash<ulong, DateTime> _heliTargetCooldown = new Hash<ulong, DateTime>();

        #region Plugin Loading & Setup
        //////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Called when the plugin is loading
        /// </summary>
        /// //////////////////////////////////////////////////////////////////////////////////////
        // ReSharper disable once UnusedMember.Local
        private void Loaded()
        {
            _pluginConfig = Config.ReadObject<PluginConfig>();

            lang.RegisterMessages(new Dictionary<string, string>()
            {
                ["Locked"] = "<color=yellow>{0}</color> the heli has locked onto you."
            }, this);
        }

        ////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Used to load a versioned config
        /// </summary>
        /// ////////////////////////////////////////////////////////////////////////
        private void LoadVersionedConfig()
        {
            try
            {
                _pluginConfig = Config.ReadObject<PluginConfig>();

                if (_pluginConfig.ConfigVersion == null)
                {
                    PrintWarning("Config failed to load correctly. Backing up to HeliTargetInfo.error.json and using default config");
                    Config.WriteObject(_pluginConfig, true, Interface.Oxide.ConfigDirectory + "/HeliTargetInfo.error.json");
                    _pluginConfig = DefaultConfig();
                }
            }
            catch
            {
                _pluginConfig = DefaultConfig();
            }

            Config.WriteObject(_pluginConfig, true);
        }

        //////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Loads the plugins default config
        /// </summary>
        /// //////////////////////////////////////////////////////////////////////////////////////
        protected override void LoadDefaultConfig()
        {
            Config.WriteObject(DefaultConfig(), true);
        }

        //////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Plugins default config
        /// </summary>
        /// <returns></returns>
        /// //////////////////////////////////////////////////////////////////////////////////////
        private PluginConfig DefaultConfig()
        {
            PrintWarning("Loading Default Config");
            return new PluginConfig
            {
                Prefix = "[<color=yellow>Heli Target Info</color>]",
                Cooldown = new TimeSpan(0, 0, 5, 0),
                ConfigVersion = Version.ToString()
            };
        }
        #endregion

        #region Oxide Hooks      
        //////////////////////////////////////////////////////////////////////////////////////  
        /// <summary>
        /// Called when the heli locks onto a player
        /// </summary>
        /// <param name="turret"></param>
        /// <param name="entity"></param>
        /// //////////////////////////////////////////////////////////////////////////////////////
        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once UnusedParameter.Local
        void OnHelicopterTarget(HelicopterTurret turret, BaseCombatEntity entity)
        {
            BasePlayer player = entity.ToPlayer();
            if (player == null) return;
            if (!_heliTargetCooldown.ContainsKey(player.userID) || (_heliTargetCooldown.ContainsKey(player.userID) && _heliTargetCooldown[player.userID] < DateTime.Now))
            {
                PrintToChat($"{_pluginConfig.Prefix}  {Lang("Locked", player.UserIDString, player.displayName)}");
                _heliTargetCooldown[player.userID] = DateTime.Now + _pluginConfig.Cooldown;
            }
        }
        #endregion

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
        #endregion

        //////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Config for the plugin
        /// </summary>
        /// //////////////////////////////////////////////////////////////////////////////////////
        class PluginConfig
        {
            public string Prefix { get; set; }
            public TimeSpan Cooldown { get; set; }
            public string ConfigVersion { get; set; }
        }
    }
}
