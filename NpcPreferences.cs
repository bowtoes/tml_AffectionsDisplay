using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace PreferencesDisplay;

public record NpcPreferences
{
	private static PersonalityDatabase _personalityDatabase;
	private static Dictionary<int, NpcPreferences> _allPreferences = new();

	private readonly int _type;
	private readonly Dictionary<string, AffectionLevel> _biomePreferences = new();
	private readonly Dictionary<int, AffectionLevel> _npcPreferences = new();

	public Dictionary<string, AffectionLevel>.Enumerator BiomesEnumerator => _biomePreferences.GetEnumerator();
	public Dictionary<int, AffectionLevel>.Enumerator NpcsEnumerator => _npcPreferences.GetEnumerator();
	public AffectionLevel Biome(string name) => _biomePreferences.GetValueOrDefault(name);
	public AffectionLevel Npc(int type) => _npcPreferences.GetValueOrDefault(type);
	public AffectionLevel Npc(NPC npc) => Npc(npc.type);
	public AffectionLevel Npc(ModNPC npc) => Npc(npc.Type);
	public PersonalityProfile Profile => _personalityDatabase.TryGetProfileByNPCID(_type, out PersonalityProfile x) ? x : null;

	/// <summary>
	/// The complete styled text description for an npc's preferences.
	/// </summary>
	public string DisplayText {
		get {
			if (_type == int.MinValue)
				return "<UNINITIALIZED NpcPreferences>";

			StringBuilder sb = new();
			if (_biomePreferences.Count > 0) {
				sb.AppendLine("Biome preferences:");
				foreach ((string name, AffectionLevel level) in _biomePreferences.OrderBy(x => (-(int)x.Value, x.Key))) {
					sb.AppendLine($"  {level}s {Language.GetTextValue(name)}");
				}
			}

			if (_npcPreferences.Count > 0) {
				sb.AppendLine("NPC preferences:");
				foreach ((int type, AffectionLevel level) in _npcPreferences.OrderBy(x => (-(int)x.Value, x.Key))) {
					sb.AppendLine($"  {level}s {NPC.GetFullnameByID(type)}");
				}
			}

			return sb.ToString();
		}
	}

	/// <summary>
	/// Get the cached preferences for the NPC id, and if no cached entry exists, create it.
	/// </summary>
	/// <returns>null on bad input (type &lt; 0).</returns>
	public static NpcPreferences Get(int type)
	{
		if (type < 0) {
			PreferencesDisplay.Instance.Logger.Error($"Tried to get preference information for invalid NPC id {type}");
			return null;
		}

		if (!_allPreferences.ContainsKey(type)) {
			var x = new NpcPreferences(type);
			if (x.Profile is null) {
				PreferencesDisplay.Instance.Logger.Error(
					$"NPC Preferences profile for {Lang.GetNPCNameValue(x._type)} is NULL");
				return null;
			}

			foreach (IShopPersonalityTrait modifier in x.Profile.ShopModifiers) {
				if (modifier is BiomePreferenceListTrait biomes) {
					foreach (var biome in biomes.Preferences) {
						if (biome is null) {
							PreferencesDisplay.Instance.Logger.Warn(
								$"Found NULL biome in preferences list for {Lang.GetNPCNameValue(x._type)}");
						} else if (biome.Biome is null) {
							PreferencesDisplay.Instance.Logger.Warn(
								$"biome.Biome is NULL for {Lang.GetNPCNameValue(x._type)}, has _personalityDatabase been updated?");
						} else {
							x._biomePreferences.Add(biome.Biome.NameKey, biome.Affection);
						}
					}
				} else if (modifier is NPCPreferenceTrait npc) {
					x._npcPreferences.Add(npc.NpcId, npc.Level);
				}
			}
			_allPreferences.Add(type, x);
		}

		return _allPreferences[type];
	}

	private NpcPreferences(int type = int.MinValue) => _type = type;

	public static bool InitializePersonalityDatabase()
	{
		_personalityDatabase = null;
		FieldInfo dbField = typeof(ShopHelper).GetField("_database", BindingFlags.Instance | BindingFlags.NonPublic);
		if (dbField is null) {
			PreferencesDisplay.Instance.Logger.Error("Failed to get personality database field, something's gone terribly wrong.");
			return false;
		}

		_personalityDatabase = dbField.GetValue(Main.ShopHelper) as PersonalityDatabase;
		if (_personalityDatabase is null) {
			PreferencesDisplay.Instance.Logger.Error("Couldn't retrieve personality database from Main.ShopHelper, something's gone terribly wrong.");
			return false;
		}

		return true;
	}
}
