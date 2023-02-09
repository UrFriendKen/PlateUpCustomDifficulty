using Kitchen;
using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using System.Runtime.InteropServices;

namespace KitchenCustomDifficulty
{
    public class FullSaveAtStartOfDay : StartOfDaySystem
    {

        [StructLayout(LayoutKind.Sequential, Size = 1)]
        private struct SHasSaved : IComponentData { }

        [ReadOnly]
        private EntityQuery _SingletonEntityQuery_SHasSaved;

        protected override void OnUpdate()
        {
            if (HasSingleton<SIsDayFirstUpdate>() && HasSingleton<SHasSaved>())
            {
                base.EntityManager.DestroyEntity(_SingletonEntityQuery_SHasSaved.GetSingletonEntity());
            }
            else if (!HasSingleton<SHasSaved>() && !base.Time.IsPaused)
            {
                if (Main.RestartFromPrepEndPreference.Load(Main.MOD_GUID) == 1 ? true : false)
                {
                    Main.LogInfo("Restart from end of prep is enabled. Saving.");
                    Debug.LogWarning("Performing a full save");
                    base.World.Add<SHasSaved>();
                    base.World.Add(new CRequestSave
                    {
                        SaveType = SaveType.AutoFull
                    });
                }
                else
                {
                    Main.LogInfo("Restart from end of prep disabled.");
                }
            }
        }

        protected override void OnCreateForCompiler()
        {
            base.OnCreateForCompiler();
            _SingletonEntityQuery_SHasSaved = GetEntityQuery(ComponentType.ReadOnly<SHasSaved>());
        }
    }
}
