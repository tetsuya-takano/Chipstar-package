using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Builder
{
    public interface IFileCollection
    {
        IReadOnlyList<string> GetFiles();
    }
    public abstract class FileCollection : ScriptableObject, IFileCollection
    {
        public abstract IReadOnlyList<string> GetFiles();
    }
}
