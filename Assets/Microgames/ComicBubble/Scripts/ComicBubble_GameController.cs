﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ComicBubble_GameController : MonoBehaviour {


    [SerializeField]
    List<ComicBubble_ComicData> comicDataList;

    [SerializeField]
    Color deactivatedStripColor;

    [SerializeField]
    Color bubbleShadowColor;

    int currentIndex;

    GameObject currentBubbleShadow;

    ComicBubble_SpeechBubble currentBubbleScript;

    Animator animator;

    // Use this for initialization
    void Start() {

        print(comicDataList.getStrip(1).name);

        // Initialize the bubble gameobjects (stablish the relationship between them, the target and speech speed)
        comicDataList.initializeBubbleGameObjects();

        // Disables all of bubbles so they don't appear all of them at once
        comicDataList.disableAllSpeechBubbles();

        // Hide every panel at first so they don't appear all of them at once
        comicDataList.hideAllStrips(deactivatedStripColor);

        // Initalize the bubble index in 0
        currentIndex = 0;

        // Get the animator controller
        animator = GetComponent<Animator>();

        // Show the elements asociated with that index
        showCurrentElements();


    }


    // Update is called once per frame
    void Update() {

        //  Update animator progress parameter
        updateAnimatorProgressParameter();

    }



    // Event for changing to the next bubble
    public void eventBubbleHasEnded()
    {
        unfollowCurrentBubble();

        hideCurrentBubbleShadow();

        StartCoroutine(moveBubbleToFinalPosition(currentIndex));

        currentIndex++;

        updateAnimatorIndexParameter();

        if (currentIndex < comicDataList.Count)
        {
            showCurrentElements();

            // If the next step doesn't have a speechbubble attached, then it's assumes that the game has finished...
            if (getCurrentBubble() == null)
            {
                microgameEnd();
            }
        }

        // No more bubbles or panels
        else
        {
            microgameEnd();
        }
    }


   


    //  This is called after one of the bubbles ends and the current index is updated. It shows everything related to that index. 
    void showCurrentElements()
    {
        showCurrentStrip();

        showCurrentBubble();

        showCurrentBubbleShadow();

        updateCurrentBubbleTextScript();

  
    }


    //  To finally end the game
    public void microgameEnd()
    {
        MicrogameController.instance.setVictory(true);
    }


    //  ANIMATOR STUFF


    //  To update the animator index parameter
    void updateAnimatorIndexParameter()
    {
        if (animator != null)
        {
            animator.SetInteger("CurrentIndex", currentIndex);
        }
    }


    //  To update the animator progress parameter
    void updateAnimatorProgressParameter()
    {
        if (animator != null)
        {
            if (currentBubbleScript != null)
            {
                var currentProgress = currentBubbleScript.getBubbleProgress();
                animator.SetFloat("CurrentProgress", currentProgress);
            }
            else
            {
                // Put it on 100 since there is no bubble
                animator.SetFloat("CurrentProgress", 100);
            }
        }
    }

    

//  CURRENT STRIP RELATED STUFF


    //  To get the current strip displayed.
    GameObject getCurrentStrip()
    {
        return comicDataList[currentIndex].getStrip();
    }


    //  For showing the strip related to the current index
    void showCurrentStrip()
    {
        //  Previous strip is sent to the back 
        if (currentIndex > 0)
        {
            // But only if it's different from tu current strip
            var previousStrip = comicDataList.getStrip(currentIndex - 1);
            var actualStrip = comicDataList.getStrip(currentIndex);
            if (previousStrip != actualStrip)
            {
                comicDataList.sendStripToTheBack(currentIndex - 1);
            }
        }


        comicDataList.showStrip(currentIndex);
    }


//  CURRENT BUBBLE RELATED STUFF


    //  Function to get the current bubble displayed.
    GameObject getCurrentBubble()
    {
        return comicDataList[currentIndex].getBubble();
    }


    // Enables (shows) the current bubble object
    void showCurrentBubble()
    {
        comicDataList.enableSpeechBubble(currentIndex);
    }


    // Disables FollowCursor of the current bubble object
    void unfollowCurrentBubble()
    {
        comicDataList.unfollowSpeechBubble(currentIndex);
    }


    //  Updates the speechbubble script for easy access
    void updateCurrentBubbleTextScript()
    {
        currentBubbleScript = comicDataList.getSpeechBubbleScript(currentIndex);
    }


    // Move the bubble, reference by the index, slightly upwards
    IEnumerator moveBubbleToFinalPosition(int index)
    {

        Vector2 originalPosition = (Vector2)comicDataList.getBubble(index).transform.position;

        Vector2 targetPosition = originalPosition + new Vector2(0, comicDataList.getDistanceAfterCompletion(index));

        Transform bubbleTransform = comicDataList[index].getBubble().transform;

        float step = comicDataList.getSpeedAfterCompletion(index) * Time.deltaTime;

        while (!Mathf.Approximately(((Vector2)bubbleTransform.position - targetPosition).sqrMagnitude, 0))
        {
            bubbleTransform.position = Vector2.MoveTowards(bubbleTransform.position, targetPosition, step);
            yield return null;
        }
    }


//  CURRENT BUBBLE SHADOW RELATED STUFF


    //  Instantiate (shows) a shadow from the current bubble
    void showCurrentBubbleShadow()
    {
        // Just one bubble shadow at the time
        hideCurrentBubbleShadow();

        // Get the speechbubble image
        GameObject speechBubble = getCurrentBubble();

        if (speechBubble != null)
        {
            GameObject bubbleImage = speechBubble.GetComponentInChildren<SpriteRenderer>().gameObject;

            // Instatiating shadow
            currentBubbleShadow = Instantiate(speechBubble, comicDataList[currentIndex].getStrip().GetComponentInChildren<Canvas>().transform) as GameObject;
            currentBubbleShadow.transform.position = Vector2.zero;
            currentBubbleShadow.transform.SetSiblingIndex(0);

            // Delete non needed commponents
            var shadowSprite = currentBubbleShadow.GetComponentInChildren<SpriteRenderer>();
            GameObject shadowObject = shadowSprite.gameObject;
            foreach (Transform child in shadowSprite.transform) GameObject.Destroy(child.gameObject);
            Destroy(shadowSprite.GetComponent<ComicBubble_SpeechBubble>());

            // Add the remaining elements
            shadowSprite.color = bubbleShadowColor;
            shadowSprite.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            shadowSprite.sortingOrder = 1;
        }

    }


    //  Destroys the shadow from the current bubble
    void hideCurrentBubbleShadow()
    {
        // Destroy the current bubble shadow
        if (currentBubbleShadow != null)
        {
            GameObject.Destroy(currentBubbleShadow);
            currentBubbleShadow = null;
        }
    }



}
