namespace GameJam.Engine.Rendering;

public readonly record struct BitmapFont(byte CharOffset, int Width, int Height, int CellWidth, int CellHeight, Memory<byte> CharWidths, uint Handle);