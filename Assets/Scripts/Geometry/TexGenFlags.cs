using System;

namespace RealtimeCSG
{
    [Flags]
    public enum TexGenFlag : int
    {
        None = 0,

        WorldSpaceTexture = 1,

        NoRender = 2,

        NoCastShadows = 4,

        NoReceiveShadows = 8,

        NoCollision = 16
    }
}
