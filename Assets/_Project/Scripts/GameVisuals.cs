using UnityEngine;

public static class GameVisuals
{
    public static void StylePlayer(Transform root)
    {
        SetRenderers(root, new Color(0.15f, 0.55f, 1f));
        AddChildPrimitive(root, "Helmet", PrimitiveType.Sphere, new Vector3(0f, 0.55f, 0f), new Vector3(0.45f, 0.35f, 0.45f), new Color(0.08f, 0.25f, 0.55f));
        AddChildPrimitive(root, "Visor", PrimitiveType.Cube, new Vector3(0f, 0.58f, 0.34f), new Vector3(0.46f, 0.12f, 0.06f), new Color(0.55f, 0.95f, 1f));
    }

    public static void StyleEnemy(Transform root, int wave = 1)
    {
        float tier = Mathf.Clamp01((wave - 1) / 4f);
        Color enemyColor = Color.Lerp(new Color(0.9f, 0.18f, 0.12f), new Color(0.45f, 0.02f, 0.02f), tier);

        SetRenderers(root, enemyColor);
        AddChildPrimitive(root, "Eye L", PrimitiveType.Sphere, new Vector3(-0.18f, 0.25f, 0.42f), new Vector3(0.12f, 0.12f, 0.12f), Color.white);
        AddChildPrimitive(root, "Eye R", PrimitiveType.Sphere, new Vector3(0.18f, 0.25f, 0.42f), new Vector3(0.12f, 0.12f, 0.12f), Color.white);
        AddChildPrimitive(root, "Spike", PrimitiveType.Capsule, new Vector3(0f, 0.72f, 0f), new Vector3(0.22f, 0.35f, 0.22f), new Color(0.35f, 0.02f, 0.02f));

        if (wave >= 3)
            AddChildPrimitive(root, "Elite Crest", PrimitiveType.Cube, new Vector3(0f, 1.05f, 0f), new Vector3(0.55f, 0.12f, 0.2f), new Color(1f, 0.78f, 0.1f));
    }

    public static void StyleTower(Transform root, int damage, float fireInterval)
    {
        Color color = damage >= 7 ? new Color(0.95f, 0.25f, 0.18f) :
                      fireInterval <= 0.35f ? new Color(0.1f, 0.85f, 0.4f) :
                      new Color(0.22f, 0.45f, 1f);

        SetRenderers(root, color);
        AddChildPrimitive(root, "Tower Base", PrimitiveType.Cylinder, new Vector3(0f, -0.18f, 0f), new Vector3(0.85f, 0.25f, 0.85f), color * 0.75f);
        AddChildPrimitive(root, "Tower Barrel", PrimitiveType.Cylinder, new Vector3(0f, 0.36f, 0.45f), new Vector3(0.18f, 0.5f, 0.18f), new Color(0.08f, 0.08f, 0.1f), new Vector3(90f, 0f, 0f));
    }

    public static void StyleBuildPad(Transform root, int towerCost)
    {
        Color color = towerCost >= 80 ? new Color(0.95f, 0.28f, 0.18f) :
                      towerCost >= 60 ? new Color(0.25f, 0.55f, 1f) :
                      new Color(0.15f, 0.9f, 0.75f);

        SetRenderers(root, color);
        AddChildPrimitive(root, "Build Ring", PrimitiveType.Cylinder, new Vector3(0f, 0.04f, 0f), new Vector3(1.25f, 0.04f, 1.25f), color * 0.65f);
    }

    public static void StyleCore(Transform root)
    {
        SetRenderers(root, new Color(1f, 0.78f, 0.18f));
        AddChildPrimitive(root, "Core Glow", PrimitiveType.Sphere, new Vector3(0f, 0.38f, 0f), new Vector3(0.55f, 0.55f, 0.55f), new Color(1f, 0.95f, 0.35f));
    }

    private static GameObject AddChildPrimitive(Transform parent, string name, PrimitiveType type, Vector3 localPosition, Vector3 localScale, Color color, Vector3? localEulerAngles = null)
    {
        if (parent.Find(name) != null)
            return parent.Find(name).gameObject;

        GameObject child = GameObject.CreatePrimitive(type);
        child.name = name;
        child.transform.SetParent(parent, false);
        child.transform.localPosition = localPosition;
        child.transform.localScale = localScale;
        child.transform.localEulerAngles = localEulerAngles ?? Vector3.zero;

        Collider collider = child.GetComponent<Collider>();
        if (collider != null)
            Object.Destroy(collider);

        SetRenderers(child.transform, color);
        return child;
    }

    private static void SetRenderers(Transform root, Color color)
    {
        Renderer[] renderers = root.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (renderer.transform.name.StartsWith("Bar"))
                continue;

            if (renderer.sharedMaterial == null)
                continue;

            renderer.material = new Material(renderer.sharedMaterial);
            renderer.material.color = color;
        }
    }
}
