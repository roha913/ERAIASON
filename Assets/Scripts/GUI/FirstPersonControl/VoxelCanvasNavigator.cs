using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static WorldAutomaton.Elemental;

public class VoxelCanvasNavigator : MonoBehaviour
{
    public List<Button> buttons;
    static int buttonIndex = 0;
    public static VoxelCanvasNavigator instance;
    // Start is called before the first frame update
    void Start()
    {
        if (buttons != null)
        {
            //If there are, select the first one
            SelectButton(0);
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.RightShift) && buttons.Count > 1)
		{
			VoxelCanvasNavigator.buttonIndex++;
            UpdateGUIForNewSelection();
        }
	}
    
    public void UpdateGUIForNewSelection()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].interactable = true;
        }

        buttons[buttonIndex].interactable = false;
        buttons[buttonIndex].Select();
    }


	public static Element GetButtonCellState()
    {
		return (Element)VoxelCanvasNavigator.buttonIndex;
    }

    public void SelectButton(int i)
    {
        VoxelCanvasNavigator.buttonIndex = i;
        UpdateGUIForNewSelection();
    }
}
