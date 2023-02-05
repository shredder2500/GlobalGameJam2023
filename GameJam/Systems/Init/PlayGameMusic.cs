using GameJam.Engine.Audio;
using GameJam.Engine.Resources;
using Silk.NET.OpenAL;

namespace GameJam.Systems.Init;

public class PlayGameMusic : ISystem
{
    public GamePhase Phase => GamePhase.Init;

    private readonly AudioClip _clip;

    public PlayGameMusic(IResourceManager resources)
    {
        _clip = resources.Load<AudioClip>("audio.creepy");
    }
    
    public ValueTask Execute(CancellationToken cancellationToken)
    {
        var source = _clip.Source;
        var buffer = _clip.Buffer;
        var al = AL.GetApi();
        al.SetSourceProperty(source, SourceInteger.Buffer, buffer);
        al.SourcePlay(source);
        return ValueTask.CompletedTask;
    }
}