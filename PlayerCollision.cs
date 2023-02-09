using Kitchen;
using KitchenCustomDifficulty.Views;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;

namespace KitchenCustomDifficulty
{
    public class PlayerCollision : IncrementalViewSystemBase<PlayerCollisionView.ViewData>, IModSystem
    {
        private EntityQuery Views;
        private EntityQuery Players;

        public struct CPlayerCollisionUpdater : IComponentData
        {
            public int IgnoreCollisionThreshold;
        }

        protected override void Initialise()
        {
            base.Initialise();
            Views = GetEntityQuery(new QueryHelper()
                .All(typeof(CLinkedView), typeof(CPlayerCollisionUpdater)));


            Players = GetEntityQuery(new QueryHelper()
                .All(typeof(CPlayer))
                .None(typeof(CPlayerCollisionUpdater)));

        }


        protected override void OnUpdate()
        {
            //if (!Main.PlayerCollisionViewPrefabAdded)
            //{
            //    return;
            //}

            //else if (!Has<CPlayerCollisionUpdater>())
            if (!Has<CPlayerCollisionUpdater>())
            {
                Main.LogInfo("Create Entity");
                Entity e = EntityManager.CreateEntity();
                EntityManager.AddComponent<CRequiresView>(e);
                EntityManager.AddComponent<CPlayerCollisionUpdater>(e);
                EntityManager.SetComponentData(e, new CRequiresView { Type = (ViewType)Main.PLAYER_COLLISION_VIEW_ID });
            }
            else
            {
                int ignoreCollisionThreshold;
                if (Has<SIsNightTime>())
                {
                    ignoreCollisionThreshold = Main.PlayerCollisionPrepPreference.Load(Main.MOD_GUID);
                }
                else
                {
                    ignoreCollisionThreshold = Main.PlayerCollisionPreference.Load(Main.MOD_GUID);
                }
                NativeArray<CLinkedView> views = Views.ToComponentDataArray<CLinkedView>(Allocator.Temp);
                for (var i = 0; i < views.Length; i++)
                {
                    CLinkedView view = views[i];
                    SendUpdate(view, new PlayerCollisionView.ViewData
                    {
                        IgnoreCollisionThreshold = ignoreCollisionThreshold
                    }, MessageType.SpecificViewUpdate);
                }
            }
        }


        //protected override void OnUpdate()
        //{
        //    NativeArray<Entity> players = Players.ToEntityArray(Allocator.Temp);
        //    for (int i = 0; i < players.Length; i++)
        //    {
        //        Set<CPlayerCollisionUpdater>(players[i]);
        //    }

        //    int ignoreCollisionThreshold;
        //    if (Has<SIsNightTime>())
        //    {
        //        ignoreCollisionThreshold = Main.PlayerCollisionPrepPreference.Load(Main.MOD_GUID);
        //    }
        //    else
        //    {
        //        ignoreCollisionThreshold = Main.PlayerCollisionPreference.Load(Main.MOD_GUID);
        //    }
        //    NativeArray<CLinkedView> views = Views.ToComponentDataArray<CLinkedView>(Allocator.Temp);
        //    for (var i = 0; i < views.Length; i++)
        //    {
        //        CLinkedView view = views[i];
        //        SendUpdate(view, new PlayerCollisionView.ViewData
        //        {
        //            IgnoreCollisionThreshold = ignoreCollisionThreshold
        //        }, MessageType.SpecificViewUpdate);
        //    }
        //    players.Dispose();
        //    views.Dispose();
        //}
    }
}
