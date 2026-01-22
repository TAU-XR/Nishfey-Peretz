using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AR51.Unity.SDK;

public class CharacterSwitcher : MonoBehaviour
{
    public bool isMale = true;
    public bool IsMirrorEnabled = true;
    public bool IsTentScene = true;
    public bool IsTextEnabled = false;
    public GameObject Mirror;
    public GameObject TentScene;
    public GameObject HouseScene;
    public GameObject TentSceneText;
    public GameObject HouseSceneText;
    public GameObject TextObject;
    public string FemaleCharName;
    public string MaleCharName;

    void Start()
    {
        // Ensure text visibility is set correctly at the start
        UpdateTextVisibility();
    }

    void Update()
    {
        // Regular updates if needed
    }

    public void SwitchGender()
    {
        if (SkeletonConsumer.Instance != null)
        {
            if (isMale && !string.IsNullOrEmpty(FemaleCharName))
            {
                SkeletonConsumer.Instance.SetActiveCharacter(FemaleCharName);
            }
            else if (!isMale && !string.IsNullOrEmpty(MaleCharName))
            {
                SkeletonConsumer.Instance.SetActiveCharacter(MaleCharName);
            }
            isMale = !isMale;
        }
    }

    public void SetCharacter(string CharName)
    {
        if (SkeletonConsumer.Instance != null && !string.IsNullOrEmpty(CharName))
        {
            SkeletonConsumer.Instance.SetActiveCharacter(CharName);
        }
    }

    public void SwitchScene()
    {
        if (IsTentScene)
        {
            if (TentScene != null) TentScene.SetActive(false);
            if (HouseScene != null) HouseScene.SetActive(true);
            if (HouseSceneText != null) HouseSceneText.SetActive(IsTextEnabled);
        }
        else
        {
            if (TentScene != null) TentScene.SetActive(true);
            if (HouseScene != null) HouseScene.SetActive(false);
            if (TentSceneText != null) TentSceneText.SetActive(IsTextEnabled);
        }
        IsTentScene = !IsTentScene;
        TextObject = IsTentScene ? TentSceneText : HouseSceneText;
        UpdateTextVisibility();
    }

    public void ToggleMirror()
    {
        if (Mirror != null)
        {
            Mirror.SetActive(!IsMirrorEnabled);
        }
        IsMirrorEnabled = !IsMirrorEnabled;
    }

    public void ToggleText()
    {
        IsTextEnabled = !IsTextEnabled;
        UpdateTextVisibility();
    }

    private void UpdateTextVisibility()
    {
        if (TextObject != null)
        {
            TextObject.SetActive(IsTextEnabled);
        }
    }
}
