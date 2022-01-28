using UnityEngine;
using System.Collections.Generic;

namespace Player.Controls
{
    public class CustomTag : MonoBehaviour
    {
#pragma warning disable S1104 // Unity can't serialize properties.
        public bool IsEnabled = true;
        public List<string> Tags = new List<string>();
#pragma warning restore S1104 // Unity can't serialize properties.

        public int Count
        {
            get { return Tags.Count; }
        }

        public bool HasTag(string tag)
        {
            return Tags.Contains(tag);
        }

        public IEnumerable<string> GetTags()
        {
            return Tags;
        }

        public void Rename(int index, string tagName)
        {
            Tags[index] = tagName;
        }

        public string GetAtIndex(int index)
        {
            return Tags[index];
        }
    }
}
