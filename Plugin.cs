﻿// -----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace ShootBang
{
    using System;
    using Exiled.API.Features;

    /// <summary>
    /// The main plugin class.
    /// </summary>
    public class Plugin : Plugin<Config>
    {
        private EventHandlers eventHandlers;

        /// <inheritdoc />
        public override string Author => "Snow";

        /// <inheritdoc />
        public override Version RequiredExiledVersion { get; } = new Version(5, 0, 0);

        /// <summary>
        /// Gets an instance of the <see cref="ShootBang.Methods"/> class.
        /// </summary>
        public Methods Methods { get; private set; }

        /// <inheritdoc />
        public override void OnEnabled()
        {
            Methods = new Methods(this);
            eventHandlers = new EventHandlers(this);
            Exiled.Events.Handlers.Player.Shooting += eventHandlers.OnShooting;
            base.OnEnabled();
        }

        /// <inheritdoc />
        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Shooting -= eventHandlers.OnShooting;
            eventHandlers = null;
            Methods = null;
            base.OnDisabled();
        }
    }
}