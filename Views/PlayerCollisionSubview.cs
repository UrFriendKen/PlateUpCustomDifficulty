using Kitchen;
using MessagePack;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KitchenCustomDifficulty.Views
{
    public class PlayerCollisionSubview : UpdatableObjectView<PlayerCollisionSubview.ViewData>
    {
        public enum PhysicsLayer
        {
            None = 0,
            Players = 1,
            Customers = 2,
            Appliances = 4,
            Walls = 8,

            AppliancesAndWalls = 12,
            AllExceptPlayer = 14,
            All = 15
        }

        public class UpdateView : IncrementalViewSystemBase<ViewData>
        {
            private static Dictionary<int, PhysicsLayer> _preferenceMap = new Dictionary<int, PhysicsLayer>()
            {
                { -1, PhysicsLayer.All },
                { 0, PhysicsLayer.AllExceptPlayer },
                { 1, PhysicsLayer.AppliancesAndWalls },
                { 2, PhysicsLayer.Walls },
                { 3, PhysicsLayer.None }
            };

            EntityQuery Views;

            protected override void Initialise()
            {
                base.Initialise();
                Views = GetEntityQuery(new QueryHelper()
                    .All(typeof(CPlayer), typeof(CLinkedView)));
            }

            protected override void OnUpdate()
            {

                NativeArray<CLinkedView> views = Views.ToComponentDataArray<CLinkedView>(Allocator.Temp);


                int prefValue = GameInfo.IsPreparationTime ? Main.PrefSysManager.Get<int>(Main.PLAYER_COLLISION_PREP_ID) : Main.PrefSysManager.Get<int>(Main.PLAYER_COLLISION_ID);

                PhysicsLayer collidesWith = GameInfo.CurrentScene == SceneType.Kitchen ? _preferenceMap[prefValue] : PhysicsLayer.All;

                for (int i = 0; i < views.Length; i++)
                {
                    SendUpdate(views[i], new ViewData()
                    {
                        CollidesWith = collidesWith
                    });
                }

                views.Dispose();
            }
        }

        [MessagePackObject(false)]
        public class ViewData : ISpecificViewData, IViewData.ICheckForChanges<ViewData>
        {
            [Key(0)]public PhysicsLayer CollidesWith;

            public IUpdatableObject GetRelevantSubview(IObjectView view)
            {
                GameObject gO = view.GameObject;
                if (gO == null)
                {
                    Main.LogError("GameObject to add subview does not exist");
                    return null;
                }

                if (!gO.transform.Find("CollisionManagement"))
                {
                    GameObject collisionManagement = new GameObject("CollisionManagement");
                    collisionManagement.transform.SetParent(gO.transform);
                    if (!collisionManagement.GetComponent<PlayerCollisionSubview>())
                    {
                        collisionManagement.AddComponent<PlayerCollisionSubview>();
                        Main.LogInfo("Added PlayerCollisionSubview");
                    }
                }
                
                return view.GetSubView<PlayerCollisionSubview>();
            }

            public bool IsChangedFrom(ViewData check)
            {
                return CollidesWith != check.CollidesWith;
            }
        }

        private static Dictionary<PhysicsLayer, int> _layers;
        public static Dictionary<PhysicsLayer, int> LayerMasks => new Dictionary<PhysicsLayer, int>(_layers); 

        public PhysicsLayer CollideWithLayers { get; protected set; } = PhysicsLayer.None;

        protected override void UpdateData(ViewData data)
        {
            if (_layers == null)
            {
                _layers = new Dictionary<PhysicsLayer, int>()
                {
                    { PhysicsLayer.Players, LayerMask.NameToLayer("Players") },
                    { PhysicsLayer.Customers, LayerMask.NameToLayer("Customers") },
                    { PhysicsLayer.Appliances, LayerMask.NameToLayer("Default") },
                    { PhysicsLayer.Walls, LayerMask.NameToLayer("Statics") }
                };
            }

            CollideWithLayers = data.CollidesWith;
            foreach (KeyValuePair<PhysicsLayer, int> layer in _layers)
            {
                Physics.IgnoreLayerCollision(_layers[PhysicsLayer.Players], layer.Value, !CollideWithLayers.HasFlag(layer.Key));
            }
        }
    }
}
