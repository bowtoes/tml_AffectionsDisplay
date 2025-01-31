using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace PreferencesDisplay;

// ModPlayer class so that keybinds work.
public class PreferencesDisplayPlayer : ModPlayer
{
	public override void ProcessTriggers(TriggersSet triggers)
	{
		if (PreferencesDisplay.toggleDisplay.JustPressed) {
			PreferencesDisplaySystem.displayInformation = !PreferencesDisplaySystem.displayInformation;
			SoundEngine.PlaySound(SoundID.Tink);
		}
	}
}
