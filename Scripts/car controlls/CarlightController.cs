using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarlightController : MonoBehaviour
{
    public Light headlightLeft;
    public Light headlightRight;
    public Light taillightLeft;
    public Light taillightRight;

    // For stylized cars, use MeshRenderer and color swap here instead

    public void SetHeadlights(bool on)
    {
        if (headlightLeft) headlightLeft.enabled = on;
        if (headlightRight) headlightRight.enabled = on;
    }

    public void SetBrakeLights(bool on)
    {
        if (taillightLeft) taillightLeft.enabled = on;
        if (taillightRight) taillightRight.enabled = on;
    }

    // Add similar methods for turn signals, reverse lights, etc.
}
