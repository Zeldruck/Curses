using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryBar : MonoBehaviour
{
    private Player player;

    [SerializeField] private float timeSwap = 1f;
    [Space]
    [SerializeField] private GameObject[] slots = null;
    [SerializeField] private GameObject[] leftSlots = null;
    [SerializeField] private GameObject[] rightSlots = null;

    void Start()
    {
        player = FindObjectOfType<Player>();
        RefreshUiSlots();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad2))
            SwapSlot(1);
        else if (Input.GetKeyDown(KeyCode.Keypad3))
            SwapSlot(2);
        else if (Input.GetKeyDown(KeyCode.Keypad4))
            SwapSlot(3);
        else if (Input.GetKeyDown(KeyCode.Keypad5))
            SwapSlot(4);
    }

    public void RefreshUiSlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (player.GetWeapon(i) == null)
            {
                Image spr = slots[i].transform.GetChild(1).GetComponent<Image>();
                spr.sprite = null;
                spr.color = new Color(1f, 1f, 1f, 0f);
            }
            else
            {
                Image spr = slots[i].transform.GetChild(1).GetComponent<Image>();

                if (spr == null)
                    spr.color = new Color(1f, 1f, 1f, 1f);

                spr.sprite = player.GetWeapon(i).spr;
            }
        }
    }

    public void ArrowInv(bool swapLeft)
    {
        //player.SwapWeapon(swapLeft);
    }

    public void SwapSlot(int index)
    {
        StartCoroutine(IESwap(index));
    }

    IEnumerator IESwap(int nbWeapon)
    {
        bool weaponExist = player.GetWeapon(nbWeapon) != null;

        yield return null;

        if (weaponExist)
        {
            /* Set the images */
            leftSlots[0].transform.GetChild(1).GetComponent<Image>().sprite = slots[slots.Length - 1].transform.GetChild(1).GetComponent<Image>().sprite;
            leftSlots[1].transform.GetChild(1).GetComponent<Image>().sprite = slots[slots.Length - 2].transform.GetChild(1).GetComponent<Image>().sprite;
            rightSlots[0].transform.GetChild(1).GetComponent<Image>().sprite = slots[0].transform.GetChild(1).GetComponent<Image>().sprite;
            rightSlots[1].transform.GetChild(1).GetComponent<Image>().sprite = slots[1].transform.GetChild(1).GetComponent<Image>().sprite;

            int direction = nbWeapon <= slots.Length / 2 ? -1 : 1;
            int nbCase = direction < 0 ? nbWeapon : slots.Length - nbWeapon;

            RectTransform container = slots[0].transform.parent.GetComponent<RectTransform>();
            float slotSize = 150f;

            for (int i = 0; i < nbCase; i++)
            {
                float xStartContainer = container.anchoredPosition.x;
                float xDestinationContainer = container.anchoredPosition.x + slotSize * direction;
                float time = 0f;

                while (time <= 1f)
                {
                    time += Time.deltaTime / (timeSwap / nbCase);
                    container.anchoredPosition = new Vector2(Mathf.Lerp(xStartContainer, xDestinationContainer, time), container.anchoredPosition.y);

                    yield return null;
                }
            }

            /* Reset pos + set the images to the good positions */
            if (direction < 0)
            {
                if (nbCase == 1)
                {
                    for (int i = 0; i < slots.Length - nbCase; i++)
                    {
                        slots[i].transform.GetChild(1).GetComponent<Image>().sprite = slots[i + 1].transform.GetChild(1).GetComponent<Image>().sprite;
                    }
                    slots[slots.Length - 1].transform.GetChild(1).GetComponent<Image>().sprite = rightSlots[0].transform.GetChild(1).GetComponent<Image>().sprite;
                }
                else if (nbCase == 2)
                {
                    for (int i = 0; i < slots.Length - nbCase; i++)
                    {
                        slots[i].transform.GetChild(1).GetComponent<Image>().sprite = slots[i + 2].transform.GetChild(1).GetComponent<Image>().sprite;
                    }

                    slots[slots.Length - 1].transform.GetChild(1).GetComponent<Image>().sprite = rightSlots[1].transform.GetChild(1).GetComponent<Image>().sprite;
                    slots[slots.Length - 2].transform.GetChild(1).GetComponent<Image>().sprite = rightSlots[0].transform.GetChild(1).GetComponent<Image>().sprite;
                }
            }
            else
            {
                if (nbCase == 1)
                {
                    for (int i = slots.Length - 1; i >= nbCase; i--)
                    {
                        slots[i].transform.GetChild(1).GetComponent<Image>().sprite = slots[i - 1].transform.GetChild(1).GetComponent<Image>().sprite;
                    }

                    slots[0].transform.GetChild(1).GetComponent<Image>().sprite = leftSlots[0].transform.GetChild(1).GetComponent<Image>().sprite;
                }
                else if (nbCase == 2)
                {
                    for (int i = slots.Length - 1; i >= nbCase; i--)
                    {
                        slots[i].transform.GetChild(1).GetComponent<Image>().sprite = slots[i - 2].transform.GetChild(1).GetComponent<Image>().sprite;
                    }

                    slots[0].transform.GetChild(1).GetComponent<Image>().sprite = leftSlots[1].transform.GetChild(1).GetComponent<Image>().sprite;
                    slots[1].transform.GetChild(1).GetComponent<Image>().sprite = leftSlots[0].transform.GetChild(1).GetComponent<Image>().sprite;
                }
            }

            container.anchoredPosition = new Vector2(0f, container.anchoredPosition.y);

            /* Change player weapon */
            player.SwapWeapon(nbWeapon);
        }
    }
}
