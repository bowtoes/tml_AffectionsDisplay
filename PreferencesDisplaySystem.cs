using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.UI.Gamepad;

namespace PreferencesDisplay;

public class PreferencesDisplaySystem : ModSystem
{
	internal static bool displayInformation = true;

	internal static string GetHoveredNPCPreferencesText()
	{
		if (UILinkPointNavigator.Shortcuts.NPCS_LastHovered < 0)
			return null;
		int id = Main.npc[UILinkPointNavigator.Shortcuts.NPCS_LastHovered].type;
		NpcPreferences pref = NpcPreferences.Get(id);
		string hoverText = pref is null
			? $"\nFailed to get NPC preferences for {Lang.GetNPCNameValue(id)}"
			: string.IsNullOrEmpty(pref.DisplayText)
				? $"\nNo preferences."
				: $"\n{pref.DisplayText}";
		return hoverText;
	}

	internal static (Vector2, Vector2) FitTextToScreen(string text, Vector2 scale, DynamicSpriteFont font, Vector2 basePosition, Vector2 screenPadding)
	{
		Vector2 screenSize = Main.ScreenSize.ToVector2();
		Vector2 maxSize = screenSize - 2 * screenPadding * scale;
		Vector2 bottomRight = screenSize - screenPadding * scale;

		Vector2 baseStringSize = font.MeasureString(text);
		float screenAspect = screenSize.X / screenSize.Y;
		float stringAspect = baseStringSize.X / baseStringSize.Y;
		if (stringAspect >= 1 && baseStringSize.X * scale.X > maxSize.X) {
			float old = scale.X;
			scale.X = maxSize.X / baseStringSize.X;
			scale.Y *= scale.X / old;
		}
		if (stringAspect <= 1 && baseStringSize.Y * scale.Y > maxSize.Y) {
			float old = scale.Y;
			scale.Y = maxSize.Y / baseStringSize.Y;
			scale.X *= scale.Y / old;
		}
		Vector2 maxPosition = bottomRight - baseStringSize * scale;
		return (Vector2.Min(basePosition, maxPosition), scale);
	}

	internal static (Vector2, Vector2) FitTextToScreen(string text, Vector2 scale, DynamicSpriteFont font,
		Vector2 basePosition, float screenPadding) =>
		FitTextToScreen(text, scale, font, basePosition, new Vector2(screenPadding));

	internal static (Vector2, Vector2) FitTextToScreen(string text, float scale, DynamicSpriteFont font,
		Vector2 basePosition, Vector2 screenPadding) =>
		FitTextToScreen(text, new Vector2(scale), font, basePosition, screenPadding);

	internal static (Vector2, Vector2) FitTextToScreen(string text, float scale, DynamicSpriteFont font,
		Vector2 basePosition, float screenPadding) =>
		FitTextToScreen(text, new Vector2(scale), font, basePosition, new Vector2(screenPadding));

	public override void PostAddRecipes()
	{
		if (!Main.dedServ) {
			if (!NpcPreferences.InitializePersonalityDatabase())
				PreferencesDisplay.Instance.Logger.Error("Failed to update NPC preferences");
		}
	}

	public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
	{
		if (Main.dedServ || !displayInformation || !Main.playerInventory || Main.EquipPage != 1)
			return;

		int mouseTextLayer = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Interface Logic 4"));
		if (mouseTextLayer < 0)
			return;

		// Draw the preferences text in a separate layer for simplicity
		layers.Insert(mouseTextLayer + 1, new LegacyGameInterfaceLayer("PreferencesDisplay: Display",
			delegate {
				if (Main.mouseItem.type != ItemID.None)
					return true;
				string hoverText = GetHoveredNPCPreferencesText();
				if (hoverText is not null) {
					var font = FontAssets.CombatText[0].Value;
					var offset = new Vector2(16);
					(Vector2 position, Vector2 scale) = FitTextToScreen(hoverText, PreferencesDisplayConfig.TextScale, font, Main.MouseScreen + offset, 4);
					ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch,
						font,
						hoverText,
						position,
						new Color(PreferencesDisplayConfig.TextColor.ToVector3() * Main.mouseTextColor / 255f),
						0f,
						Vector2.Zero,
						scale);
				}
				return true;
			},
			InterfaceScaleType.UI));
	}
}
