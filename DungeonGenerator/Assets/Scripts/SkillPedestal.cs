using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPedestal : MonoBehaviour
{
    [SerializeField] protected List<SkillInfo> possibleSkills = new List<SkillInfo>();

    SkillInfo selectedSkill;
    Skill heldSkill;

    SkillPedestal partnerPedestal;

    bool isActive = true;

    private void Start()
    {
        ChooseRandomSkill();
    }

    void ChooseRandomSkill()
    {
        //Go through list of skills (scriptable objects) and choose a random one (can also add rarity?)

        int randomIndex = Random.Range(0, possibleSkills.Count);

        selectedSkill = possibleSkills[randomIndex];

        var skill = Instantiate(selectedSkill.skillPrefab, Vector3.zero, Quaternion.identity);

        heldSkill = skill.GetComponent<Skill>();

        Instantiate(selectedSkill.pedestalModel, new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), selectedSkill.pedestalModel.transform.rotation, transform);
    }

    public void GiveSkill()
    {
        if (isActive)
        {
            PlayerController.Instance.EquipSkill(heldSkill);

            Destroy(gameObject.transform.GetChild(0).gameObject);

            isActive = false;

            partnerPedestal.Deactivate();
        }
    }

    public void SetPartner(SkillPedestal partner)
    {
        partnerPedestal = partner;
    }

    public void Deactivate()
    {
        isActive = false;

        Destroy(gameObject.transform.GetChild(0).gameObject);
    }

    //public SkillPedestal GetPartner()
    //{
    //    return partnerPedestal;
    //}

    //public bool GetActive()
    //{
    //    return isActive;
    //}
}
