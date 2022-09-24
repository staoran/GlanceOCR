﻿namespace GlanceHotkey
{
    static class Extensions
    {
        public static bool HasFlag(this Keys keys, Keys flag)
        {
            return (keys & flag) == flag;
        }
    }
}
