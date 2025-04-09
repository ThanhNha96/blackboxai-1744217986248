using UnityEngine;

[System.Serializable]
public class DogAppearance : MonoBehaviour
{
    [Header("Body Parts")]
    public SpriteRenderer bodyRenderer;
    public SpriteRenderer headRenderer;
    public SpriteRenderer tailRenderer;

    [Header("Clothing")]
    public SpriteRenderer hatRenderer;
    public SpriteRenderer shirtRenderer;
    public SpriteRenderer pantsRenderer;
    public SpriteRenderer shoesRenderer;

    [Header("Accessories")]
    public SpriteRenderer collarRenderer;
    public SpriteRenderer backpackRenderer;
    public SpriteRenderer glassesRenderer;

    [Header("Available Options")]
    public Sprite[] bodySprites;
    public Sprite[] headSprites;
    public Sprite[] tailSprites;
    public Sprite[] hatSprites;
    public Sprite[] shirtSprites;
    public Sprite[] pantsSprites;
    public Sprite[] shoesSprites;
    public Sprite[] collarSprites;
    public Sprite[] backpackSprites;
    public Sprite[] glassesSprites;

    [Header("Colors")]
    public Color[] availableColors;
    
    // Current selections
    private int currentBodyIndex;
    private int currentHeadIndex;
    private int currentTailIndex;
    private int currentHatIndex;
    private int currentShirtIndex;
    private int currentPantsIndex;
    private int currentShoesIndex;
    private int currentCollarIndex;
    private int currentBackpackIndex;
    private int currentGlassesIndex;
    private int currentColorIndex;

    public void ChangeBodySprite(int index)
    {
        currentBodyIndex = index % bodySprites.Length;
        bodyRenderer.sprite = bodySprites[currentBodyIndex];
    }

    public void ChangeHeadSprite(int index)
    {
        currentHeadIndex = index % headSprites.Length;
        headRenderer.sprite = headSprites[currentHeadIndex];
    }

    public void ChangeTailSprite(int index)
    {
        currentTailIndex = index % tailSprites.Length;
        tailRenderer.sprite = tailSprites[currentTailIndex];
    }

    public void ChangeHat(int index)
    {
        currentHatIndex = index % hatSprites.Length;
        hatRenderer.sprite = hatSprites[currentHatIndex];
        hatRenderer.enabled = (index >= 0);
    }

    public void ChangeShirt(int index)
    {
        currentShirtIndex = index % shirtSprites.Length;
        shirtRenderer.sprite = shirtSprites[currentShirtIndex];
        shirtRenderer.enabled = (index >= 0);
    }

    public void ChangePants(int index)
    {
        currentPantsIndex = index % pantsSprites.Length;
        pantsRenderer.sprite = pantsSprites[currentPantsIndex];
        pantsRenderer.enabled = (index >= 0);
    }

    public void ChangeShoes(int index)
    {
        currentShoesIndex = index % shoesSprites.Length;
        shoesRenderer.sprite = shoesSprites[currentShoesIndex];
        shoesRenderer.enabled = (index >= 0);
    }

    public void ChangeCollar(int index)
    {
        currentCollarIndex = index % collarSprites.Length;
        collarRenderer.sprite = collarSprites[currentCollarIndex];
        collarRenderer.enabled = (index >= 0);
    }

    public void ChangeBackpack(int index)
    {
        currentBackpackIndex = index % backpackSprites.Length;
        backpackRenderer.sprite = backpackSprites[currentBackpackIndex];
        backpackRenderer.enabled = (index >= 0);
    }

    public void ChangeGlasses(int index)
    {
        currentGlassesIndex = index % glassesSprites.Length;
        glassesRenderer.sprite = glassesSprites[currentGlassesIndex];
        glassesRenderer.enabled = (index >= 0);
    }

    public void ChangeColor(int index)
    {
        currentColorIndex = index % availableColors.Length;
        Color newColor = availableColors[currentColorIndex];
        bodyRenderer.color = newColor;
    }

    public DogCustomizationData GetCurrentCustomization()
    {
        return new DogCustomizationData
        {
            bodyIndex = currentBodyIndex,
            headIndex = currentHeadIndex,
            tailIndex = currentTailIndex,
            hatIndex = currentHatIndex,
            shirtIndex = currentShirtIndex,
            pantsIndex = currentPantsIndex,
            shoesIndex = currentShoesIndex,
            collarIndex = currentCollarIndex,
            backpackIndex = currentBackpackIndex,
            glassesIndex = currentGlassesIndex,
            colorIndex = currentColorIndex
        };
    }

    public void ApplyCustomization(DogCustomizationData data)
    {
        ChangeBodySprite(data.bodyIndex);
        ChangeHeadSprite(data.headIndex);
        ChangeTailSprite(data.tailIndex);
        ChangeHat(data.hatIndex);
        ChangeShirt(data.shirtIndex);
        ChangePants(data.pantsIndex);
        ChangeShoes(data.shoesIndex);
        ChangeCollar(data.collarIndex);
        ChangeBackpack(data.backpackIndex);
        ChangeGlasses(data.glassesIndex);
        ChangeColor(data.colorIndex);
    }
}

[System.Serializable]
public class DogCustomizationData
{
    public int bodyIndex;
    public int headIndex;
    public int tailIndex;
    public int hatIndex;
    public int shirtIndex;
    public int pantsIndex;
    public int shoesIndex;
    public int collarIndex;
    public int backpackIndex;
    public int glassesIndex;
    public int colorIndex;
}
