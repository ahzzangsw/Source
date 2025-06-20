using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectManager", menuName = "Pixel Heroes/EffectManager")]
public class EffectManager : ScriptableObject
{
    public SpriteEffect SpriteEffectPrefab;
    public AudioClip FireAudioClip;

    private static Material _baseMaterial;
    private static Material _blinkMaterial;

    public static EffectManager Instance;

    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        Instance = Resources.Load<EffectManager>("EffectManager");
    }

    public void Blink(Character character)
    {
        if (_baseMaterial == null) _baseMaterial = character.m_Body.sharedMaterial;
        if (_blinkMaterial == null) _blinkMaterial = new Material(Shader.Find("GUI/Text Shader"));

        character.StartCoroutine(BlinkCoroutine(character));
    }

    private IEnumerator BlinkCoroutine(Character character)
    {
        character.m_Body.material = _blinkMaterial;
        yield return new WaitForSeconds(0.1f);
        character.m_Body.material = _baseMaterial;
    }

    public SpriteEffect CreateSpriteEffect(Character character, string clipName, int direction = 0, Transform parent = null)
    {
        var instance = Instantiate(SpriteEffectPrefab, character.transform.position, Quaternion.identity, parent);
        instance.name = clipName;
        instance.transform.position = parent == null ? character.transform.position : parent.transform.position;
        instance.GetComponent<SpriteRenderer>().sortingOrder = character.m_Body.sortingOrder + 1;
        instance.Play(clipName, direction == 0 ? Math.Sign(character.transform.localScale.x) : direction);
        return instance;
    }
}