using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuPanel : MonoBehaviour {
	public List<MenuButton> buttons;
	// Use this for initialization
	void Start () {

		foreach (MenuButton button in buttons) {
			button.setParentMenu(this);
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void open(bool open){
		foreach (MenuButton button in buttons) {
			button.setOpen(open);
		}
	}

	public void move(int baseX, int moveX){
		//move each button to the right
		
		foreach (MenuButton button in buttons) {
			button.move( baseX,moveX);
		}
	}



	public void showSubMenus(bool show){
		foreach (MenuButton button in buttons) {
			button.showSubMenu(show);
		}
	}



}
