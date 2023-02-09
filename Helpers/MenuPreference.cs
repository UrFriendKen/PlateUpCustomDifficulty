using System;
using KitchenLib.Utils;
using UnityEngine;

namespace KitchenCustomDifficulty
{
    public class MenuPreference
    {
        private dynamic _initialValue;
        private bool TrySetInitialValue(dynamic val)
        {
            if (this._initialValue == null)
            {
                this._initialValue = val;
            }
            else if (this._initialValue.GetType() == val.GetType())
            {
                this._initialValue = val;
            }
            else
                return false;
            return true;
        }

        public dynamic InitialValue
        {
            get => _initialValue;
            set => TrySetInitialValue(value);
        }

        public Type PreferenceType
        {
            get => InitialValue.GetType();
        }

        public string PreferenceID;

        public string PreferenceDisplayText;

        private dynamic _kLPreference;

        #region Constructors
        public MenuPreference(string preference_id, int initial_value, string preference_display_name)
        {
            InitMemberVariables(preference_id, initial_value, preference_display_name);
        }
        public MenuPreference(string preference_id, float initial_value, string preference_display_name)
        {
            InitMemberVariables(preference_id, initial_value, preference_display_name);
        }
        public MenuPreference(string preference_id, bool initial_value, string preference_display_name)
        {
            InitMemberVariables(preference_id, initial_value, preference_display_name);
        }
        public MenuPreference(string preference_id, string initial_value, string preference_display_name)
        {
            InitMemberVariables(preference_id, initial_value, preference_display_name);
        }
        private void InitMemberVariables(string preference_id, dynamic initial_value, string preference_display_name)
        {
            this.PreferenceID = preference_id;
            this.InitialValue = initial_value;
            this.PreferenceDisplayText = preference_display_name;
        }
        #endregion

        public void Register(string mod_guid)
        {
            if (PreferenceType == typeof(int))
            {
                this._kLPreference = PreferenceUtils.Register<KitchenLib.IntPreference>(mod_guid, this.PreferenceID, this.PreferenceDisplayText);
            }
            else if (PreferenceType == typeof(float))
            {
                this._kLPreference = PreferenceUtils.Register < KitchenLib.FloatPreference>(mod_guid, this.PreferenceID, this.PreferenceDisplayText);
            }
            else if (PreferenceType == typeof(bool))
            {
                this._kLPreference = PreferenceUtils.Register<KitchenLib.BoolPreference>(mod_guid, this.PreferenceID, this.PreferenceDisplayText);
            }
            else if (PreferenceType == typeof(string))
            {
                this._kLPreference = PreferenceUtils.Register<KitchenLib.StringPreference>(mod_guid, this.PreferenceID, this.PreferenceDisplayText);
            }
            this._kLPreference.Value = this.InitialValue;
        }

        public void SetValue(string mod_guid, dynamic value)
        {
            if (PreferenceType == typeof(int) && value.GetType() == typeof(int))
            {
                PreferenceUtils.Get<KitchenLib.IntPreference>(mod_guid, this.PreferenceID).Value = value;
            }
            else if (PreferenceType == typeof(float) && value.GetType() == typeof(float))
            {
                PreferenceUtils.Get<KitchenLib.FloatPreference>(mod_guid, this.PreferenceID).Value = value;
            }
            else if (PreferenceType == typeof(bool) && value.GetType() == typeof(bool))
            {
                PreferenceUtils.Get<KitchenLib.BoolPreference>(mod_guid, this.PreferenceID).Value = value;
            }
            else if (PreferenceType == typeof(string) && value.GetType() == typeof(string))
            {
                PreferenceUtils.Get<KitchenLib.StringPreference>(mod_guid, this.PreferenceID).Value = value;
            }
            throw new ArgumentException($"Type of value does not match type of InitialValue ({PreferenceType})");
        }

        public dynamic Load(string mod_guid)
        {
            if (PreferenceType == typeof(int))
            {
                return PreferenceUtils.Get<KitchenLib.IntPreference>(mod_guid, this.PreferenceID).Value;
            }
            else if (PreferenceType == typeof(float))
            {
                return PreferenceUtils.Get<KitchenLib.FloatPreference>(mod_guid, this.PreferenceID).Value;
            }
            else if (PreferenceType == typeof(bool))
            {
                return PreferenceUtils.Get<KitchenLib.BoolPreference>(mod_guid, this.PreferenceID).Value;
            }
            else if (PreferenceType == typeof(string))
            {
                return PreferenceUtils.Get<KitchenLib.StringPreference>(mod_guid, this.PreferenceID).Value;
            }
            return null;
        }
    }
}
