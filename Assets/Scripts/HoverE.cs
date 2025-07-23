using System.Collections;
using UnityEngine;

public class HoverE : MonoBehaviour
{
    public GameObject keybindEPrefab;
    private GameObject currentKeybind;
    private Camera mainCam;
    public Circulin Circulin;
    public Hook hook;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mousePos);

        if (hit != null && hit.CompareTag("Enganchable"))
        {
            if (currentKeybind == null)
            {
                hook.HookDisp = true;
                Vector3 spawnPos = hit.transform.position + new Vector3(0.4f, -0.2f, 0);
                currentKeybind = Instantiate(keybindEPrefab, mousePos, Quaternion.identity);
            }
        }
        else
        {
            hook.HookDisp = false;
            if (currentKeybind != null)
            {
                Destroy(currentKeybind);
                currentKeybind = null;
                
            }
        }
    }
}
