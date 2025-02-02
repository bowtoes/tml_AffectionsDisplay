using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace AffectionsDisplay;

// ModPlayer class so that keybinds work.
public class AffectionsDisplayPlayer : ModPlayer
{
	public override void ProcessTriggers(TriggersSet triggers)
	{
		if (AffectionsDisplay.toggleDisplay.JustPressed) {
			AffectionsDisplaySystem.displayInformation = !AffectionsDisplaySystem.displayInformation;
			SoundEngine.PlaySound(SoundID.Tink);
		}
	}
}
