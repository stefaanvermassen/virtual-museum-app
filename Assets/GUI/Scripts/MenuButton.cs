using UnityEngine;
using System.Collections;

public class MenuButton : MonoBehaviour {
	private MenuPanel parentMenu;
	public MenuPanel childMenu;
	//this buttons menu is opened
	private bool open=false;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void setParentMenu( MenuPanel parentMenu){
		this.parentMenu = parentMenu;

	}
	public void onClick(){
		if (childMenu != null) {
			if(open){
				//close if submenu active
				if(childMenu.gameObject.activeSelf){
					//close submenu 
					showSubMenu(false);
					//center parent
					parentMenu.move(0,0);
					//set closed
					parentMenu.open(false);
				}
				//if open but not own submenu, open it and close others
				else{
					parentMenu.showSubMenus(false);
					showSubMenu(true);
				}
			}
			//if not open 
			else{
				//open submenu and center
				showSubMenu(true);
				childMenu.move(0,0);
				//move parent to left
				parentMenu.move(0,-200);
				//set open
				parentMenu.open(true);
			}

		}

	}

	public void setOpen(bool open){
		this.open = open;
	}
	public void showSubMenu(bool show){
		if (childMenu == null) {
			Debug.Log ("no childmenu");
			return;
		}
		childMenu.gameObject.SetActive(show);
	}
	public MenuPanel getChildMenu(){
		return childMenu;
	}
	public void move(int baseX, int moveX){
		RectTransform buttonTransform = ((RectTransform)this.transform);
		buttonTransform.anchoredPosition=new Vector2(baseX+moveX,buttonTransform.anchoredPosition.y);
	}

}
