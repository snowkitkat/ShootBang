// -----------------------------------------------------------------------
// <copyright file="Methods.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Player;
using Exiled.API.Features.Items;

namespace ShootBang
{
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.ThrowableProjectiles;
    using Mirror;
    using UnityEngine;

    /// <summary>
    /// Handles miscellaneous methods and tracking required by the <see cref="EventHandlers"/>.
    /// </summary>
    public class Methods
    {
        private static readonly int PickupMask = LayerMask.GetMask("Pickup");
        private static readonly int GrenadeMask = LayerMask.GetMask("Grenade");
        private readonly Plugin plugin;

        /// <summary>
        /// Initializes a new instance of the <see cref="Methods"/> class.
        /// </summary>
        /// <param name="plugin">An instance of the <see cref="Plugin"/> class.</param>
        public Methods(Plugin plugin) => this.plugin = plugin;

        /// <summary>
        /// Checks a players shot to see if a grenade item was hit.
        /// </summary>
        /// <param name="ev">The produced <see cref="ShootingEventArgs"/>.</param>
        /// <returns>A value indicating whether a pickup was detected with a linecast.</returns>
        public bool CheckPickup(ShootingEventArgs ev)
        {
            if (!Physics.Raycast(ev.Player.CameraTransform.transform.position, ev.Player.CameraTransform.forward, out RaycastHit raycastHit, 100f, PickupMask))
                return false;

            ItemPickupBase item = raycastHit.transform.GetComponent<ItemPickupBase>();
            if (item == null)
                return true;

            Pickup pickup = Pickup.Get(item);
            if (pickup.Type.IsThrowable())
            {
                NetworkServer.Destroy(pickup.Base.gameObject);

                SpawnGrenade(pickup.Type, raycastHit.point, ev.Player);
            }

            return true;
        }

        /// <summary>
        /// Checks a players shot to see if a thrown grenade was hit.
        /// </summary>
        /// <param name="ev">The produced <see cref="ShootingEventArgs"/>.</param>
        public void CheckAir(ShootingEventArgs ev)
        {
            if (!Physics.Raycast(ev.Player.CameraTransform.transform.position, ev.Player.CameraTransform.forward, out RaycastHit raycastHit, 100f, GrenadeMask))
                return;

            ExplosionGrenade explosionGrenade = raycastHit.transform.GetComponent<ExplosionGrenade>();
            if (explosionGrenade != null && !(explosionGrenade is Scp018Projectile))
            {
                NetworkServer.Destroy(explosionGrenade.gameObject);
                SpawnGrenade(ItemType.GrenadeHE, raycastHit.point, ev.Player);
                return;
            }

            var flash = raycastHit.transform.GetComponent<FlashbangGrenade>();
            if (flash != null)
            {
                NetworkServer.Destroy(flash.gameObject);
                SpawnGrenade(ItemType.GrenadeFlash, raycastHit.point, ev.Player);
            }
        }

        private void SpawnGrenade(ItemType type, Vector3 position, Player owner)
        {
            switch (type)
            {
                case ItemType.GrenadeFlash:
                    FlashGrenade flashGrenade = (FlashGrenade)Item.Create(ItemType.GrenadeFlash);
                    flashGrenade.FuseTime = plugin.Config.FuseDuration;
                    flashGrenade.SpawnActive(position, owner);
                    break;
                case ItemType.GrenadeHE:
                    ExplosiveGrenade explosiveGrenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
                    explosiveGrenade.FuseTime = plugin.Config.FuseDuration;
                    explosiveGrenade.SpawnActive(position, owner);
                    break;
                case ItemType.SCP2176:
                    Scp2176 scp2176 = (Scp2176)Item.Create(ItemType.SCP2176);
                    scp2176.FuseTime = plugin.Config.FuseDuration;
                    scp2176.SpawnActive(position, owner);
                    break;
            }
        }
    }
}