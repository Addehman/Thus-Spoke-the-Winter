using UnityEngine;

/// <summary>
/// Attach this script to any gameObject for which you want to put a note.
/// </summary>
public class Notes : MonoBehaviour
{
	[TextArea(4, 5)]
	public string comment = "Leave notes here";
}
