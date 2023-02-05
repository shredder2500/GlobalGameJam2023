using System.Buffers.Binary;
using System.Text;
using GameJam.Engine.Audio;
using JasperFx.Core;
using Silk.NET.OpenAL;

namespace GameJam.Engine.Resources.Loaders;

internal class AudioClipLoader : IResourceLoader<AudioClip>
{
    private readonly ALContext _alc;
    private readonly AL _al;
    private readonly unsafe Device* _device;
    private readonly unsafe Context* _context;

    public unsafe AudioClipLoader()
    {
        _alc = ALContext.GetApi();
        _al = AL.GetApi();
        
        _device = _alc.OpenDevice("");
        if (_device == null)
        {
            throw new ("Could not create device");
        }

        _context = _alc.CreateContext(_device, null);
        _alc.MakeContextCurrent(_context);
            
        _al.GetError();
    }
    
    public unsafe AudioClip Load(Stream stream)
    {
        ReadOnlySpan<byte> file = stream.ReadAllBytes();
        
        int index = 0;
        if (file[index++] != 'R' || file[index++] != 'I' || file[index++] != 'F' || file[index++] != 'F')
        {
            throw new ("Given file is not in RIFF format");
        }

        var chunkSize = BinaryPrimitives.ReadInt32LittleEndian(file.Slice(index,  4));
        index += 4;

        if (file[index++] != 'W' || file[index++] != 'A' || file[index++] != 'V' || file[index++] != 'E')
        {
            throw new("Given file is not in WAVE format");
        }
        
        short numChannels = -1;
            int sampleRate = -1;
            int byteRate = -1;
            short blockAlign = -1;
            short bitsPerSample = -1;
            BufferFormat format = 0;

            var source = _al.GenSource();
            var buffer = _al.GenBuffer();
            _al.SetSourceProperty(source, SourceBoolean.Looping, true);


            while (index + 4 < file.Length)
            {
                var identifier = "" + (char) file[index++] + (char) file[index++] + (char) file[index++] + (char) file[index++];
                var size = BinaryPrimitives.ReadInt32LittleEndian(file.Slice(index, 4));
                index += 4;
                if (identifier == "fmt ")
                {
                    if (size != 16)
                    {
                        Console.WriteLine($"Unknown Audio Format with subchunk1 size {size}");
                    }
                    else
                    {
                        var audioFormat = BinaryPrimitives.ReadInt16LittleEndian(file.Slice(index, 2));
                        index += 2;
                        if (audioFormat != 1)
                        {
                            Console.WriteLine($"Unknown Audio Format with ID {audioFormat}");
                        }
                        else
                        {
                            numChannels = BinaryPrimitives.ReadInt16LittleEndian(file.Slice(index, 2));
                            index += 2;
                            sampleRate = BinaryPrimitives.ReadInt32LittleEndian(file.Slice(index, 4));
                            index += 4;
                            byteRate = BinaryPrimitives.ReadInt32LittleEndian(file.Slice(index, 4));
                            index += 4;
                            blockAlign = BinaryPrimitives.ReadInt16LittleEndian(file.Slice(index, 2));
                            index += 2;
                            bitsPerSample = BinaryPrimitives.ReadInt16LittleEndian(file.Slice(index, 2));
                            index += 2;
                    
                            if (numChannels == 1)
                            {
                                if (bitsPerSample == 8)
                                    format = BufferFormat.Mono8;
                                else if (bitsPerSample == 16)
                                    format = BufferFormat.Mono16;
                                else
                                {
                                    Console.WriteLine($"Can't Play mono {bitsPerSample} sound.");
                                }
                            }
                            else if (numChannels == 2)
                            {
                                if (bitsPerSample == 8)
                                    format = BufferFormat.Stereo8;
                                else if (bitsPerSample == 16)
                                    format = BufferFormat.Stereo16;
                                else
                                {
                                    Console.WriteLine($"Can't Play stereo {bitsPerSample} sound.");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Can't play audio with {numChannels} sound");
                            }
                        }
                    }
                } 
                else if (identifier == "data")
                {
                    var data = file.Slice(index, size);
                    index += size;
                    
                    fixed(byte* pData = data)
                        _al.BufferData(buffer, format, pData, size, sampleRate);
                    Console.WriteLine($"Read {size} bytes Data");
                }
                else if (identifier == "JUNK")
                {
                    // this exists to align things
                    index += size;
                }
                else if (identifier == "iXML")
                {
                    var v = file.Slice(index, size);
                    var str = Encoding.ASCII.GetString(v);
                    Console.WriteLine($"iXML Chunk: {str}");
                    index += size;
                }
                else
                {
                    Console.WriteLine($"Unknown Section: {identifier}");
                    index += size;
                }
            }

            Console.WriteLine
            (
                $"Success. Detected RIFF-WAVE audio file, PCM encoding. {numChannels} Channels, {sampleRate} Sample Rate, {byteRate} Byte Rate, {blockAlign} Block Align, {bitsPerSample} Bits per Sample"
            );

            return new(source, buffer);
    }

    public unsafe void Unload(AudioClip resource)
    {
        var source = resource.Source;
        var buffer = resource.Buffer;
        _al.SourceStop(source);

        _al.DeleteSource(source);
        _al.DeleteBuffer(buffer);
        _alc.DestroyContext(_context);
        _alc.CloseDevice(_device);
        _al.Dispose();
        _alc.Dispose();
    }
}