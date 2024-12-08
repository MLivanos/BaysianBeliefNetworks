using System.Collections;
using UnityEngine;

public class CollegeLifeCutscene : CutsceneBehavior
{
	[SerializeField] private GameObject solidLine;
	[SerializeField] private GameObject brokenLine;
	[SerializeField] private float timeBeforeText;

	protected override IEnumerator PlayScene()
    {
    	yield return new WaitForSeconds(timeBeforeText);
    	solidLine.SetActive(false);
    	brokenLine.SetActive(true);
    	yield return ViewPanel();
        AnimateText();
    }

    public override void Interrupt()
    {
    	return;
    }

    protected override IEnumerator ExitTransition()
    {
    	yield return null;
    }
}