using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The Path.
/// </summary>
public class Path
{

    /// <summary>
    /// The nodes.
    /// </summary>
    protected List<Node> m_Nodes = new List<Node>();

    /// <summary>
    /// The length of the path.
    /// </summary>
    protected float m_Length = 0f;

    /// <summary>
    /// Gets the nodes.
    /// </summary>
    /// <value>The nodes.</value>
    public virtual List<Node> nodes
    {
        get
        {
            return m_Nodes;
        }
    }

    /// <summary>
    /// Gets the length of the path.
    /// </summary>
    /// <value>The length.</value>
    public virtual float length
    {
        get
        {
            return m_Length;
        }
    }

    /// <summary>
    /// Bake the path.
    /// Making the path ready for usage, Such as caculating the length.
    /// </summary>
    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    /// <filterpriority>2</filterpriority>
}
