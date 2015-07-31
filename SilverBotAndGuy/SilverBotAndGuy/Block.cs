namespace SilverBotAndGuy
{
    enum Block : byte
    {
        Floor = 0x0,
        Exit = 0x1,
        //0x10 or 16 is the solid bit. Anything from 0x10 to 0x1F is solid.
        Wall = 0x10,
        LaserProofWall = 0x11,
        LaserGunRight = 0x12,
        LaserGunDown = 0x13,
        LaserGunLeft = 0x14,
        LaserGunUp = 0x15,
        Bomb = 0x16,
        Panel = 0x17,
        Crate = 0x18,
        None = 0x19,
        Ice = 0x1A,
        //0xFD is reserved as reserved as "\\".
        //0xFE is reserved as reserved as "/".
        //0xFF is reserved for temporary start position.
    }
}
