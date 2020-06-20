﻿namespace Norns.Destiny.Abstraction.Structure
{
    public interface ISymbolInfo
    {
        object Origin { get; }
        string Name { get; }
        bool IsStatic { get; }
        AccessibilityInfo Accessibility { get; }
    }
}