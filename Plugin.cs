using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

namespace EmitSoundVolumeFix;

public class EmitSoundVolumeFixPlugin : BasePlugin
{
    public override string ModuleName => "EmitSoundVolumeFix";
    public override string ModuleVersion => "1.0.1";
    public override string ModuleAuthor => "samyyc";

    // Found by searching string "CSoundOpGameSystem::SetSoundEventParam: Failed cached soundevent param message\n"
    public MemoryFunctionWithReturn<nint, nint, nint, uint, nint, uint, uint, byte> CSoundOpGameSystem_SetSoundEventParam_Windows = 
        new(GameData.GetSignature("CSoundOpGameSystem_SetSoundEventParam_2"));
    public MemoryFunctionWithReturn<int, int, nint, uint, nint, short, uint, nint> CSoundOpGameSystem_SetSoundEventParam_Linux = 
        new(GameData.GetSignature("CSoundOpGameSystem_SetSoundEventParam_2"));


    public HookResult OnSetSoundEventParam(DynamicHook hook)
    {
        var hash = hook.GetParam<uint>(3);
        if (hash == 0x2D8464AF) {
            hook.SetParam(3, 0xBD6054E9);
        }
        return HookResult.Continue;
    }
    public override void Load(bool hotReload)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            CSoundOpGameSystem_SetSoundEventParam_Windows.Hook(OnSetSoundEventParam, HookMode.Pre);
        } else {
            CSoundOpGameSystem_SetSoundEventParam_Linux.Hook(OnSetSoundEventParam, HookMode.Pre);
        }
    }

    public override void Unload(bool hotReload) 
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            CSoundOpGameSystem_SetSoundEventParam_Windows.Unhook(OnSetSoundEventParam, HookMode.Pre);
        } else {
            CSoundOpGameSystem_SetSoundEventParam_Linux.Unhook(OnSetSoundEventParam, HookMode.Pre);
        }
    }
}
