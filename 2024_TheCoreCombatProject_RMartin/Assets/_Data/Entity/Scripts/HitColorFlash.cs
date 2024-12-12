using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EntityLife))]
public class HealthChangeFlash : MonoBehaviour
{
    [SerializeField] private Color damageFlashColor;
    [SerializeField] private Color healFlashColor;
    [SerializeField] private float flashDuration;

    private List<Material[]> originalMaterials = new();
    private List<Color[]> originalColors = new();
    private SkinnedMeshRenderer[] meshRenderers;
    private List<Tween> flashTweens = new();

    private EntityLife entityLife;


    private void Awake()
    {
        entityLife = GetComponent<EntityLife>();
        entityLife.onLifeChanged.AddListener(PerformDamageFlash);
        //entityLife.OnHealthGained.AddListener(PerformHealFlash);

        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer renderer in meshRenderers)
        {
            originalMaterials.Add(renderer.materials);
            List<Color> colors = new List<Color>();
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                if (renderer.materials[i].HasProperty("_BaseColor"))
                {
                    colors.Add(renderer.materials[i].color);
                }
                else
                {
                    colors.Add(Color.black); // Dummy color for materials without _Color property
                }
            }
            originalColors.Add(colors.ToArray());
        }
    }

    public void TriggerFlash(Color flashColor)
    {
        foreach (Tween tween in flashTweens)
        {
            tween.Kill();
        }
        flashTweens.Clear();

        ChangeMaterialsToColor(flashColor);

        foreach (SkinnedMeshRenderer renderer in meshRenderers)
        {
            foreach (Material material in renderer.materials)
            {
                if (material.HasProperty("_BaseColor"))
                {
                    // Tween the color property back to the original color
                    flashTweens.Add(material.DOColor(
                        Color.white,
                        flashDuration
                    ));
                }
            }
        }
    }

    private void ChangeMaterialsToColor(Color color)
    {
        foreach (SkinnedMeshRenderer renderer in meshRenderers)
        {
            var propertyBlock = new MaterialPropertyBlock();
            foreach (Material material in renderer.materials)
            {
                material.color = color;
            }
        }
    }

    private void PerformDamageFlash(float newHealth)
    {
        TriggerFlash(damageFlashColor);
    }

    private void PerformHealFlash(float newHealth)
    {
        TriggerFlash(healFlashColor);
    }
}