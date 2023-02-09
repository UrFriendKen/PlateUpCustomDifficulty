using Kitchen;
using MessagePack;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenCustomDifficulty.Views
{
    public class PlayerCollisionView : UpdatableObjectView<PlayerCollisionView.ViewData>
    {
        [MessagePackObject]
        public struct ViewData : ISpecificViewData, IViewData.ICheckForChanges<ViewData>
        {
            [Key(0)]
            public int IgnoreCollisionThreshold;

            public IUpdatableObject GetRelevantSubview(IObjectView view)
            {
                return view.GetSubView<PlayerCollisionView>();
            }

            public bool IsChangedFrom(ViewData check)
            {
                return IgnoreCollisionThreshold != check.IgnoreCollisionThreshold;
            }
        }

        //static int PlayersLayer;
        //static int CustomersLayer;
        //static int DefaultLayer;
        //static int WallsLayer;

        //static int[] Layers;

        public override void Initialise()
        {
            base.Initialise();
            //PlayersLayer = LayerMask.NameToLayer("Players");
            //CustomersLayer = LayerMask.NameToLayer("Customers");
            //DefaultLayer = LayerMask.NameToLayer("Default");
            //WallsLayer = LayerMask.NameToLayer("Statics");
            //Layers = new int[] { PlayersLayer, CustomersLayer, DefaultLayer, WallsLayer };
        }

        protected override void UpdateData(ViewData view_data)
        {
            ChangeGameObjectCollision(view_data.IgnoreCollisionThreshold);
        }

        private void ChangeGameObjectCollision(int ignoreCollisionThreshold)
        {
            int PlayersLayer = LayerMask.NameToLayer("Players");
            int CustomersLayer = LayerMask.NameToLayer("Customers");
            int DefaultLayer = LayerMask.NameToLayer("Default");
            int WallsLayer = LayerMask.NameToLayer("Statics");
            int[] Layers = new int[] { PlayersLayer, CustomersLayer, DefaultLayer, WallsLayer };

            for (int i = 0; i < Layers.Length; i++)
            {
                bool ignoreCollision = i <= ignoreCollisionThreshold;
                //Main.LogInfo(ignoreCollision);
                //List<GameObject> gameObjects = GetGameObjectsInLayer(Layers[i]);
                //Main.LogInfo(gameObjects);
                //SetCollidersEnabled(gameObjects, !ignoreCollision);
                //Main.LogInfo($"Layers[{i}] Ignore Collision = {ignoreCollision}");
                Physics.IgnoreLayerCollision(PlayersLayer, Layers[i], ignore: ignoreCollision);
            }

        }
        private List<GameObject> GetGameObjectsInLayer(int layer)
        {
            var gameObjectsInLayer = new List<GameObject>();
            foreach (var go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
            {
                if (go.layer == layer && go.GetComponent<Collider>() != null)
                {
                    gameObjectsInLayer.Add(go);
                }
            }
            return gameObjectsInLayer;
        }

        private void SetCollidersEnabled(List<GameObject> gameObjects, bool enabled)
        {
            foreach (GameObject go in gameObjects)
            {
                var collider = go.GetComponent<Collider>();
                if (collider != null)
                {
                    collider.enabled = enabled;
                }
            }
        }
    }
}
