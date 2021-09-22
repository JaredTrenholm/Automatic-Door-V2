using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject rightPanel;
    public GameObject leftPanel;
    public Vector3 openDistance;
    public Vector3 openSpeed;
    public AudioSource audioSource;
    public AudioClip openAudio;
    public AudioClip closeAudio;

    private Vector3 rightPanelOriginalPos;
    private Vector3 leftPanelOriginalPos;
    private Vector3 distanceOpen = Vector3.zero;
    private Vector3 currentOpenSpeed;
    private DoorState state = DoorState.Closed;
    private bool startClosing = false;
    private bool startOpening = false;

    private enum DoorState { 
        Opened,
        Closed,
        Opening,
        Closing
    }
    private void Start()
    {
        rightPanelOriginalPos = rightPanel.transform.position;
        leftPanelOriginalPos = leftPanel.transform.position;
    }
    private void Update()
    {
        switch (state) {
            case DoorState.Opened:
                if (startClosing)
                {
                    state = DoorState.Closing;
                    startClosing = false;
                    currentOpenSpeed = openSpeed;
                }
                break;
            case DoorState.Closed:
                if (startOpening)
                {
                    state = DoorState.Opening;
                    startOpening = false;
                    currentOpenSpeed = openSpeed;
                }
                break;
            case DoorState.Opening:
                Open();
                break;
            case DoorState.Closing:
                Close();
                break;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if(state != DoorState.Opened)
            {
                state = DoorState.Opening;
                currentOpenSpeed = openSpeed;
                audioSource.Stop();
            }
            else
            {
                startOpening = true;
                startClosing = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (state == DoorState.Opened)
            {
                state = DoorState.Closing;
                currentOpenSpeed = openSpeed;
                audioSource.Stop();
            } else
            {
                startClosing = true;
                startOpening = false;
            }
        }
    }

    private void Open()
    {
        if (!audioSource.isPlaying)
            audioSource.PlayOneShot(openAudio);
        rightPanel.transform.Translate(new Vector3(currentOpenSpeed.x * Time.deltaTime, currentOpenSpeed.y* Time.deltaTime, currentOpenSpeed.z * Time.deltaTime), Space.Self);
        leftPanel.transform.Translate(new Vector3(-currentOpenSpeed.x * Time.deltaTime, -currentOpenSpeed.y * Time.deltaTime, -currentOpenSpeed.z * Time.deltaTime), Space.Self);
        distanceOpen.x += currentOpenSpeed.x * Time.deltaTime;
        distanceOpen.y += currentOpenSpeed.y * Time.deltaTime;
        distanceOpen.z += currentOpenSpeed.z * Time.deltaTime;
        ClampOpen();
    }
    private void Close()
    {
        if (!audioSource.isPlaying)
            audioSource.PlayOneShot(openAudio);
        rightPanel.transform.Translate(new Vector3(-currentOpenSpeed.x * Time.deltaTime, -currentOpenSpeed.y * Time.deltaTime, -currentOpenSpeed.z * Time.deltaTime), Space.Self);
        leftPanel.transform.Translate(new Vector3(currentOpenSpeed.x * Time.deltaTime, currentOpenSpeed.y * Time.deltaTime, currentOpenSpeed.z * Time.deltaTime), Space.Self);
        distanceOpen.x -= currentOpenSpeed.x * Time.deltaTime;
        distanceOpen.y -= currentOpenSpeed.y * Time.deltaTime;
        distanceOpen.z -= currentOpenSpeed.z * Time.deltaTime;
        ClampClose();
    }
    private void ClampOpen()
    {
        if (distanceOpen.x >= openDistance.x)
        {
            currentOpenSpeed.x = 0f;
        }

        if (distanceOpen.y >= openDistance.y)
        {
            currentOpenSpeed.y = 0f;
        }

        if (distanceOpen.z >= openDistance.z)
        {
            currentOpenSpeed.z = 0f;
        }

        if(currentOpenSpeed == Vector3.zero)
        {
            state = DoorState.Opened;
            audioSource.Stop();
        }

    }

    private void ClampClose()
    {
        if (distanceOpen.x <= 0f)
        {
            currentOpenSpeed.x = 0f;
        }

        if (distanceOpen.y <= 0f)
        {
            currentOpenSpeed.y = 0f;
        }

        if (distanceOpen.z <= 0f)
        {
            currentOpenSpeed.z = 0f;
        }

        if (currentOpenSpeed == Vector3.zero)
        {
            state = DoorState.Closed;
            rightPanel.transform.position = rightPanelOriginalPos;
            leftPanel.transform.position = leftPanelOriginalPos;
            audioSource.Stop();
        }

    }
}
