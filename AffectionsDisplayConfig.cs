using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace AffectionsDisplay;

public class AffectionsDisplayConfig : ModConfig
{
	public override ConfigScope Mode => ConfigScope.ClientSide;

	[ColorHSLSlider, ColorNoAlpha] public Color textColor;
	public static Color TextColor => ModContent.GetInstance<AffectionsDisplayConfig>().textColor;

	[Range(0.1f, 3)] public float textScale = 1;
	public static float TextScale => ModContent.GetInstance<AffectionsDisplayConfig>().textScale;

	public bool showAllNPCs = false;
	public static bool ShowAllNPCs => ModContent.GetInstance<AffectionsDisplayConfig>().showAllNPCs;
}
