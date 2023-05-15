using Kitchen;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using static Kitchen.EnforcePlayerBounds;

namespace KitchenCustomDifficulty
{
    public class EnforceNewPlayerBounds : RestaurantSystem
    {
        private const float BOUNDS_OFFSET = 0.65f;

        private Bounds NewBounds
        {
            get
            {
                Bounds bounds = new Bounds()
                {
                    center = Bounds.center,
                    extents = Bounds.extents,
                    max = Bounds.max,
                    min = Bounds.min,
                    size = Bounds.size
                };

                bounds.Encapsulate(GetFrontDoor() + new Vector3(0f, 0f, -2f));
                bounds.Expand(BOUNDS_OFFSET);
                return bounds;
            }
        }


        EntityQuery Players;
        EntityQuery OutOfBoundsRooms;
        protected override void Initialise()
        {
            base.Initialise();
            Players = GetEntityQuery(new QueryHelper()
                .All(typeof(CPlayer), typeof(CPosition)));
            OutOfBoundsRooms = GetEntityQuery(new QueryHelper()
                .All(typeof(CRoomMarker)));
        }

        protected override void OnUpdate()
        {
            bool allowOutOfBounds = Main.PrefSysManager.Get<int>(Main.PLAYER_OUT_OF_BOUNDS_ID) == 1;
            bool canPassThroughOutsideMovementBlocker = (GameInfo.IsPreparationTime ? Main.PrefSysManager.Get<int>(Main.PLAYER_COLLISION_PREP_ID) : Main.PrefSysManager.Get<int>(Main.PLAYER_COLLISION_ID)) > 0;
            if (allowOutOfBounds && canPassThroughOutsideMovementBlocker)
                return;

            NativeArray<Entity> entities = Players.ToEntityArray(Allocator.Temp);
            NativeArray<CPosition> positions = Players.ToComponentDataArray<CPosition>(Allocator.Temp);
            NativeArray<CRoomMarker> outOfBoundsRooms = OutOfBoundsRooms.ToComponentDataArray<CRoomMarker>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                Entity entity = entities[i];
                CPosition position = positions[i];

                bool isInBounds = !NewBounds.Contains(position.Position);
                if (!isInBounds)
                {
                    CLayoutRoomTile tile = GetTile(position);
                    foreach (CRoomMarker oob_room in outOfBoundsRooms)
                    {
                        if (tile.RoomID == oob_room.RoomID)
                        {
                            isInBounds = true;
                            break;
                        }
                    }
                }
                if (isInBounds)
                {
                    position.Position = GetNearestInBoundsPosition(position);
                    position.ForceSnap = true;
                }

                Set(entity, position);
            }

            entities.Dispose();
            positions.Dispose();
            outOfBoundsRooms.Dispose();
        }

        private Vector3 GetNearestInBoundsPosition(Vector3 position)
        {
            if (NewBounds.Contains(position))
                return position;
            return NewBounds.ClosestPoint(position);
        }
    }
}
