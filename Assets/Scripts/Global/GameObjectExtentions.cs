using UnityEngine;

public static class UnityClassesExtentions
{
    /// <summary>
    /// Returns the full hierarchy name of the game object.
    /// </summary>
    /// <param name="go">The game object.</param>
    public static string GetFullName(this GameObject go)
    {
        string name = go.name;
        while (go.transform.parent != null)
        {

            go = go.transform.parent.gameObject;
            name = go.name + "/" + name;
        }
        return name;
    }

    /// <summary>
    /// Transform.Find() doesn't search children recursively. This function does.
    /// </summary>
    /// <returns>Child with name of <paramref name="childName"/>.</returns>
    public static Transform RecursiveFind(this Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child;
            }
            else
            {
                Transform found = RecursiveFind(child, childName);
                if (found != null)
                {
                    return found;
                }
            }
        }
        return null;
    }
}
