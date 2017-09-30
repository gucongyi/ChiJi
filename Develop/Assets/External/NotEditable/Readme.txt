NotEditable In Inspector

Online Docs: http://www.tiinoo.com/not-editable-docs

--------------------------------------------------
1. Overview
--------------------------------------------------
NotEditable In Inspector
- Prevent users from modifying the values of fileds in the inspector

Is this a tool for you?
- Is it possible to make fields read-only in the inspector so that users can't change them?
- Is there a way to make private variables displayed in the inspector but not editable?

If you have met any of the above issues, this is the tool for you.

Features:
- Make fields not editable in the inspector.

--------------------------------------------------
2. Quick Start
--------------------------------------------------
Before you begin the upgrade, don't forget to remove the old version of this plugin. (See the section "How to remove the plugin?")

Import the plugin, then read the following instructions.

--------------------------------------------------
2.1 How to use?
--------------------------------------------------
If you want to make a filed read-only in the inspector, just tag it with the attribute [NotEditableInInspector]. 
You will see the fields tagged with [NotEditableInInspector] go grey and not editable.
For example:
public class Demo : MonoBehaviour
{
	[NotEditableInInspector]
	public int num1 = 2;

	[SerializeField][NotEditableInInspector]
	private int num2 = 120;

	[NotEditableInInspector]
	public float range = 1.5f;

	[NotEditableInInspector]
	public string text = "Hello";

	[NotEditableInInspector]
	public Vector3 position = new Vector3(1, 2, 3);
}

Note: If you want to make a private or protected field displayed in the inspector and not editable, you should use both [SerializeField] and [NotEditableInInspector] attributes. 
For example:
[SerializeField][NotEditableInInspector]
private int num2 = 120;

--------------------------------------------------
2.2 How to remove the plugin?
--------------------------------------------------
To remove the plugin, just delete the folder Assets/Tiinoo/NotEditable.

--------------------------------------------------
3. Support
--------------------------------------------------
If you have any questions or suggestions, please send an email to me.

Email: support@tiinoo.com