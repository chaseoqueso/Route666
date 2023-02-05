using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadEnemyMaterials : MonoBehaviour
{
    private List<Material> punkClothesMaterials = new List<Material>();
    private List<Material> punkHairMaterials = new List<Material>();
    private List<Material> punkSkinMaterials = new List<Material>();
    private List<Material> punkEarringsMaterials = new List<Material>();

    void Start()
    {
        Object[] punkClothesMatList = Resources.LoadAll("PunkEnemy/Clothes", typeof(Material));
        foreach(Object m in punkClothesMatList){
            punkClothesMaterials.Add( (Material)m );
        }

        Object[] punkHairMatList = Resources.LoadAll("PunkEnemy/Hair", typeof(Material));
        foreach(Object m in punkHairMatList){
            punkHairMaterials.Add( (Material)m );
            punkClothesMaterials.Add( (Material)m );
        }

        Object[] punkSkinMatList = Resources.LoadAll("PunkEnemy/Skin", typeof(Material));
        foreach(Object m in punkSkinMatList){
            punkSkinMaterials.Add( (Material)m );
        }

        Object[] punkEarringsMatList = Resources.LoadAll("PunkEnemy/Earrings", typeof(Material));
        foreach(Object m in punkEarringsMatList){
            punkEarringsMaterials.Add( (Material)m );
        }
    }

    public List<Material> GetPunkClothesMats()
    {
        return punkClothesMaterials;
    }

    public List<Material> GetPunkHairMats()
    {
        return punkHairMaterials;
    }

    public List<Material> GetPunkSkinMats()
    {
        return punkSkinMaterials;
    }

    public List<Material> GetPunkEarringsMats()
    {
        return punkEarringsMaterials;
    }
}
