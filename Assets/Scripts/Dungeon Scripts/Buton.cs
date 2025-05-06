using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public AudioClip buttonPressSound;
    public AudioClip doorOpenSound;
    public AudioSource audioSource;
    public DoorScript doorScript;
    public Sprite pressedSprite;
    private SpriteRenderer spriteRenderer;

    private bool isPressed = false;
    public static int buttonPressCount = 0;

    //initializes the spriteRenderer component
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //handles button press, plays sound, updates sprite, and opens door when threshold met
    public void PressButton()
    {
        if (isPressed) return;

        Debug.Log("Button Pressed!");

        if (audioSource != null && buttonPressSound != null)
        {
            Debug.Log("Playing button press sound.");
            audioSource.PlayOneShot(buttonPressSound);
        }
        else
        {
            Debug.LogWarning("AudioSource or Button Press Sound is not assigned.");
        }

        if (spriteRenderer != null && pressedSprite != null)
        {
            spriteRenderer.sprite = pressedSprite;
        }
        else
        {
            Debug.LogWarning("SpriteRenderer or Pressed Sprite is not assigned.");
        }

        isPressed = true;

        buttonPressCount++;
        Debug.Log("Button Press Count: " + buttonPressCount);

        if (buttonPressCount == 3)
        {
            if (doorScript != null)
            {
                doorScript.OpenDoor();
            }
            else
            {
                Debug.LogWarning("DoorScript is not assigned to the button.");
            }
        }
    }

    //resets the button press count
    public static void ResetButtonPressCount()
    {
        buttonPressCount = 0;
    }
}
